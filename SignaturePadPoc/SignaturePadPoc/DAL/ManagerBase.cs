using Microsoft.WindowsAzure.MobileServices;
using SignaturePadPoc.Common;

namespace SignaturePadPoc.DAL
{
    public class ManagerBase
    {
        public ManagerBase()
        {
            CurrentClient = new MobileServiceClient(Constants.ApplicationUrl);
        }
        
        public MobileServiceClient CurrentClient { get; }
    }
}