using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace AzureComm
{
    /// <summary>
    /// To work with AZURE Language Understanding (LUIS) services.
    /// </summary>
    internal class AzureLUIS
    {
        public static async Task<string> Run(string testString)
        {
            string requestPath = string.Format("{0}{1}", AzureDef.urlLUIS, testString);

            using (var client = new HttpClient())
            {
                string uri = requestPath;
                HttpResponseMessage msg = await client.GetAsync(uri);
                if (msg.IsSuccessStatusCode)
                {
                    var jsonResponse = await msg.Content.ReadAsStringAsync();

                    return jsonResponse;
                }
            }

            return string.Empty;
        }
    }
}