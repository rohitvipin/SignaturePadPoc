using System;
using System.Threading.Tasks;
using Plugin.Connectivity;
using SignaturePadPoc.Common;
using SignaturePadPoc.DAL;
using SignaturePadPoc.FileAccessLayer;
using SignaturePadPoc.Views;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace SignaturePadPoc
{
    public partial class App : Application
    {
        private bool _isSyncing;

        public App()
        {
            InitializeComponent();

            if (ApplicationContext.IsInitialized == false)
            {
                ApplicationContext.Initialize();
            }

            if (RepositoryManager.IsInitialized == false)
            {
                RepositoryManager.Initialize();
            }

            MainPage = new NavigationPage(new MainPage());

            Device.StartTimer(TimeSpan.FromMinutes(3), () =>
            {
                if (CrossConnectivity.Current.IsConnected)
                {
                    Task.Run(async () => await SyncAllDocumentsAsync());
                }
                return true;
            });
        }

        private async Task SyncAllDocumentsAsync()
        {
            if (_isSyncing)
            {
                return;
            }

            if (RepositoryManager.IsInitialized == false || ApplicationContext.IsInitialized == false)
            {
                return;
            }

            _isSyncing = true;

            var syncAllTablesAsync = RepositoryManager.SyncAllTablesAsync();
            var downloadAllUserDocumentsAsync = FileManager.DownloadAllUserDocumentsAsync();
            await syncAllTablesAsync;
            await downloadAllUserDocumentsAsync;

            _isSyncing = false;
        }

        protected override void OnStart()
        {
            if (CrossConnectivity.Current.IsConnected)
            {
                Task.Run(async () => await SyncAllDocumentsAsync());
            }
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            if (CrossConnectivity.Current.IsConnected)
            {
                Task.Run(async () => await SyncAllDocumentsAsync());
            }
        }
    }
}
