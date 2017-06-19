using Microsoft.WindowsAzure.MobileServices;

namespace SignaturePadPoc
{
    public class AppConstants
    {
        public static MobileServiceClient MobileService = new MobileServiceClient("https://cesofflinesignturesyncpoc.azurewebsites.net");
    }
}