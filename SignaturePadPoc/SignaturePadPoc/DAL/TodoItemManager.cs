using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.MobileServices;
using Microsoft.WindowsAzure.MobileServices.SQLiteStore;
using Microsoft.WindowsAzure.MobileServices.Sync;
using SignaturePadPoc.Common;
using SignaturePadPoc.DAL.Models;

namespace SignaturePadPoc.DAL
{
    public class TodoItemManager
    {
        private readonly IMobileServiceSyncTable<TodoItem> _todoTable;

        private const string OfflineDbPath = @"localstore.db";

        private TodoItemManager()
        {
            CurrentClient = new MobileServiceClient(Constants.ApplicationUrl);

            var store = new MobileServiceSQLiteStore(OfflineDbPath);
            store.DefineTable<TodoItem>();

            //Initializes the SyncContext using the default IMobileServiceSyncHandler.
            CurrentClient.SyncContext.InitializeAsync(store);

            _todoTable = CurrentClient.GetSyncTable<TodoItem>();
        }

        public static TodoItemManager DefaultManager { get; } = new TodoItemManager();

        public MobileServiceClient CurrentClient { get; }

        public bool IsOfflineEnabled => _todoTable != null;

        public async Task<IEnumerable<TodoItem>> GetTodoItemsAsync(bool syncItems = false)
        {
            try
            {
                if (syncItems)
                {
                    await SyncAsync();
                }

                return await _todoTable.Where(todoItem => !todoItem.Done).ToEnumerableAsync();
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

        public async Task SaveTaskAsync(TodoItem item)
        {
            if (item.Id == null)
            {
                await _todoTable.InsertAsync(item);
            }
            else
            {
                await _todoTable.UpdateAsync(item);
            }
        }

        public async Task SyncAsync()
        {
            ReadOnlyCollection<MobileServiceTableOperationError> syncErrors = null;

            try
            {
                await CurrentClient.SyncContext.PushAsync();

                await _todoTable.PullAsync(
                    //The first parameter is a query name that is used internally by the client SDK to implement incremental sync.
                    //Use a different query name for each unique query in your program
                    "allTodoItems",
                    _todoTable.CreateQuery());
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
    }
}
