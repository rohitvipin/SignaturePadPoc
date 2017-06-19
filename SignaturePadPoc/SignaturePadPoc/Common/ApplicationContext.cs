using Microsoft.WindowsAzure.MobileServices;

namespace SignaturePadPoc.Common
{
    public class ApplicationContext
    {
        public static readonly MobileServiceClient MobileServiceClientInstance = new MobileServiceClient(Constants.ApplicationUrl);
        public static int LoggedInUserId { get; set; } = 123;
    }
}