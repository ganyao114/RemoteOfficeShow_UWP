using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Web.Http;
using Windows.Web.Http.Headers;

namespace RemoteOfficeShow.Model.Net
{
    class HttpClientFactory
    {
        private static HttpClient client;
        private static String mylock = "";
        public static HttpClient getClient() {
            lock (mylock) {
                if (client == null)
                    newClient();
            }
            return client;
        }

        private static void newClient()
        {
            client = new HttpClient();
        }

        public static void addHeader(string key,string value) {
            if (client == null)
                getClient();
            client.DefaultRequestHeaders.Add(key,value);
        }
        public static HttpRequestHeaderCollection getheaders() {
            if (client == null)
                getClient();
            return client.DefaultRequestHeaders;
        }
    }
}
