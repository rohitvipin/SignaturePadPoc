using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Plugin.Connectivity;

namespace SignaturePadPoc
{
    public static class RestApiHelper
    {
        public static async Task<MemoryStream> DownloadFileAsync(string url)
        {
            if (CrossConnectivity.Current.IsConnected == false)
            {
                return null;
            }

            var stream = new MemoryStream();
            using (var httpClient = new HttpClient())
            {
                var downloadStream = await httpClient.GetStreamAsync(new Uri(url));
                if (downloadStream != null)
                {
                    await downloadStream.CopyToAsync(stream);
                }
            }

            return stream;
        }
    }
}