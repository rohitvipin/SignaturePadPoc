using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.MobileServices;
using Microsoft.WindowsAzure.MobileServices.SQLiteStore;
using Microsoft.WindowsAzure.MobileServices.Sync;
using SignaturePadPoc.Common;
using SignaturePadPoc.DAL.Models;

namespace SignaturePadPoc.DAL
{
    public class ManagerBase<T> where T : ModelBase
    {
        protected readonly IMobileServiceSyncTable<T> CurrentTable;

        protected readonly MobileServiceClient CurrentClient;

        public ManagerBase()
        {
            CurrentClient = new MobileServiceClient(Constants.ApplicationUrl);
            var store = new MobileServiceSQLiteStore(Constants.OfflineDbPath);
            store.DefineTable<T>();

            //Initializes the SyncContext using the default IMobileServiceSyncHandler.
            CurrentClient.SyncContext.InitializeAsync(store);

            CurrentTable = CurrentClient.GetSyncTable<T>();
        }

        protected async Task SyncAsync()
        {
            ReadOnlyCollection<MobileServiceTableOperationError> syncErrors = null;

            try
            {
                await CurrentClient.SyncContext.PushAsync();
                await CurrentTable.PullAsync($"all{nameof(T)}", CurrentTable.CreateQuery());
            }
            catch (MobileServicePushFailedException exc)
            {
                if (exc.PushResult != null)
                {
                    syncErrors = exc.PushResult.Errors;
                }
            }

            // Simple error/conflict handling. A real application would handle the various errors like network conditions,
            // server conflicts and others via the IMobileServiceSyncHandler.
            if (syncErrors != null)
            {
                foreach (var error in syncErrors)
                {
                    if (error.OperationKind == MobileServiceTableOperationKind.Update && error.Result != null)
                    {
                        //Update failed, reverting to server's copy.
                        await error.CancelAndUpdateItemAsync(error.Result);
                    }
                    else
                    {
                        // Discard local change.
                        await error.CancelAndDiscardItemAsync();
                    }

                    Debug.WriteLine(@"Error executing sync operation. Item: {0} ({1}). Operation discarded.", error.TableName, error.Item["id"]);
                }
            }
        }

        public async Task<IEnumerable<T>> GetTodoItemsAsync(bool syncItems = false, Expression<Func<T, bool>> filterCondition = null, Expression<Func<T, bool>> orderBy = null)
        {
            try
            {
                if (syncItems)
                {
                    await SyncAsync();
                }

                var mobileServiceSyncTable = CurrentTable;
                if (filterCondition != null)
                {
                    mobileServiceSyncTable.Where(filterCondition);
                }
                if (orderBy != null)
                {
                    mobileServiceSyncTable.OrderBy(orderBy);
                }
                return await mobileServiceSyncTable.ToEnumerableAsync();
            }
            catch (MobileServiceInvalidOperationException mobileServiceInvalidOperationException)
            {
                Debug.WriteLine(@"Invalid sync operation: {0}", mobileServiceInvalidOperationException.Message);
            }
            catch (Exception e)
            {
                Debug.WriteLine(@"Sync error: {0}", e.Message);
            }
            return null;
        }

        public async Task SaveAsync(T item)
        {
            if (item.Id == null)
            {
                await CurrentTable.InsertAsync(item);
            }
            else
            {
                await CurrentTable.UpdateAsync(item);
            }
        }
    }
}