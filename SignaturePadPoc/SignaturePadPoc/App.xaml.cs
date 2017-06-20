using System;
using System.Threading.Tasks;
using Plugin.Connectivity;
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
        public App()
        {
            InitializeComponent();

            MainPage = new NavigationPage(new MainPage());

            Device.StartTimer(TimeSpan.FromMinutes(3), () =>
            {
                if (CrossConnectivity.Current.IsConnected)
                {
                    Task.Run(async () =>
                    {
                        var syncAllTablesAsync = RepositoryManager.SyncAllTablesAsync();
                        var downloadAllUserDocumentsAsync = FileManager.DownloadAllUserDocumentsAsync();
                        await syncAllTablesAsync;
                        await downloadAllUserDocumentsAsync;
                    });
                }
                return true;
            });
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
