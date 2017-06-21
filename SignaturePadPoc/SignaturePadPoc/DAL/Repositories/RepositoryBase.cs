using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.MobileServices;
using Microsoft.WindowsAzure.MobileServices.Sync;
using Plugin.Connectivity;
using Plugin.Connectivity.Abstractions;
using SignaturePadPoc.Common;
using SignaturePadPoc.DAL.Models;

namespace SignaturePadPoc.DAL.Repositories
{
    public class RepositoryBase<T> where T : ModelBase
    {
        protected readonly IMobileServiceSyncTable<T> CurrentTable;
        private DateTime _lastSuccessfulSyncDateTime = DateTime.MinValue;
        private bool _isSyncing;

        public RepositoryBase()
        {
            if (RepositoryManager.IsInitialized != true)
            {
                RepositoryManager.Initialize();
            }
            CurrentTable = ApplicationContext.MobileServiceClientInstance.GetSyncTable<T>();

            CrossConnectivity.Current.ConnectivityChanged += Current_ConnectivityChanged;
        }

        private async void Current_ConnectivityChanged(object sender, ConnectivityChangedEventArgs e)
        {
            if (e?.IsConnected == true)
            {
                await SyncAsync();
            }
        }

        public async Task SyncAsync()
        {
            if (_isSyncing || DateTime.Now.AddMinutes(-5) < _lastSuccessfulSyncDateTime)
            {
                return;
            }

            if (CrossConnectivity.Current.IsConnected == false)
            {
                return;
            }

            ReadOnlyCollection<MobileServiceTableOperationError> syncErrors = null;

            try
            {
                _isSyncing = true;

                await ApplicationContext.MobileServiceClientInstance.SyncContext.PushAsync();
                await CurrentTable.PullAsync($"all{nameof(T)}", CurrentTable.CreateQuery());

                _lastSuccessfulSyncDateTime = DateTime.Now;
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

            _isSyncing = false;
        }

        public async Task<IEnumerable<T>> GetAsync(Expression<Func<T, bool>> filterCondition = null, Expression<Func<T, bool>> orderBy = null)
        {
            try
            {
                await SyncAsync();

                var mobileServiceTableQuery = CurrentTable.CreateQuery();
                if (filterCondition != null)
                {
                    mobileServiceTableQuery = mobileServiceTableQuery.Where(filterCondition);
                }
                if (orderBy != null)
                {
                    mobileServiceTableQuery = mobileServiceTableQuery.OrderBy(orderBy);
                }
                return await mobileServiceTableQuery.ToEnumerableAsync();
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

        public async Task DeleteAsync(T item)
        {
            if (item?.Id == null)
            {
                return;
            }

            await CurrentTable.DeleteAsync(item);
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
            _isSyncing = false;
            _lastSuccessfulSyncDateTime = DateTime.MinValue;
            await SyncAsync();
        }
    }
}