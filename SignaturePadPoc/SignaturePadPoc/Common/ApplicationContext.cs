using Microsoft.WindowsAzure.MobileServices;

namespace SignaturePadPoc.Common
{
    public static class ApplicationContext
    {
        public static bool IsInitialized { get; private set; }

        public static MobileServiceClient MobileServiceClientInstance { get; private set; }

        public static int LoggedInUserId { get; set; } = 123;

        public static void Initialize()
        {
            IsInitialized = true;
            MobileServiceClientInstance = new MobileServiceClient(Constants.ApplicationUrl);
        }
    }
}