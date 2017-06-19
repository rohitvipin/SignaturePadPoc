using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.MobileServices;
using Microsoft.WindowsAzure.MobileServices.Sync;
using SignaturePadPoc.Common;
using SignaturePadPoc.DAL.Models;

namespace SignaturePadPoc.DAL.Repositories
{
    public class RepositoryBase<T> where T : ModelBase
    {
        protected readonly IMobileServiceSyncTable<T> CurrentTable;

        public RepositoryBase()
        {
            if (RepositoryManager.IsInitialized != true)
            {
                RepositoryManager.Initialize();
            }
            CurrentTable = ApplicationContext.MobileServiceClientInstance.GetSyncTable<T>();

            Plugin.Connectivity.CrossConnectivity.Current.ConnectivityChanged += Current_ConnectivityChanged;
        }

        private async void Current_ConnectivityChanged(object sender, Plugin.Connectivity.Abstractions.ConnectivityChangedEventArgs e)
        {
            if (e?.IsConnected == true)
            {
                await SyncAsync();
            }
        }

        protected async Task SyncAsync()
        {
            if (RepositoryManager.IsSyncing || DateTime.Now.AddMinutes(-5) < RepositoryManager.LastSuccessfulSyncDateTime)
            {
                return;
            }

            if (Plugin.Connectivity.CrossConnectivity.Current.IsConnected == false)
            {
                return;
            }

            ReadOnlyCollection<MobileServiceTableOperationError> syncErrors = null;

            try
            {
                RepositoryManager.IsSyncing = true;

                await ApplicationContext.MobileServiceClientInstance.SyncContext.PushAsync();
                await CurrentTable.PullAsync($"all{nameof(T)}", CurrentTable.CreateQuery());

                RepositoryManager.LastSuccessfulSyncDateTime = DateTime.Now;
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

            RepositoryManager.IsSyncing = false;
        }

        public async Task<IEnumerable<T>> GetAsync(Expression<Func<T, bool>> filterCondition = null, Expression<Func<T, bool>> orderBy = null)
        {
            try
            {
                await SyncAsync();

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
            if (item == null)
            {
                return;
            }

            if (item.Id == null)
            {
                await CurrentTable.InsertAsync(item);
            }
            else
            {
                await CurrentTable.UpdateAsync(item);
            }
            await SyncAsync();
        }

        public async Task ForceSyncAsync()
        {
            RepositoryManager.IsSyncing = false;
            RepositoryManager.LastSuccessfulSyncDateTime = DateTime.MinValue;
            await SyncAsync();
        }
    }
}