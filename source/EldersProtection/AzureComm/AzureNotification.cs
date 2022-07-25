using AzureComm;
using Microsoft.Azure.NotificationHubs;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace EldersProtection
{
    internal class AzureNotification
    {
        public static async Task SendMsgAsync(string msg)
        {
            SendNotificationAsync(msg);

            string url = string.Format("{0}{1}{2}",
                AzureDef.NotificationSettings.HubEndpoint,
                AzureDef.NotificationSettings.HubName,
                "/messages/?api-version=2015-01");

            using (var httpClient = new HttpClient())
            {
                //var request = new HttpRequestMessage(HttpMethod.Post, url);

                //request.Headers.Add("Authorization", createToken(url, "DefaultFullSharedAccessSignature", "wGG0K2tMXLobdZuzHzlGNJl5skJjzOHtqfii53pq/E8="));
                //// request.Headers.Add("Content-Type", "application/json;charset=utf-8");
                //request.Headers.Add("ServiceBusNotification-Tags", "EldersProtection");
                //request.Headers.Add("ServiceBusNotification-Format", "gcm");
                //string jsonMsg = "{\"data\":{\"message\":\"" + msg + "\"}}";
                //using (var content = new StringContent(jsonMsg, Encoding.UTF8, "application/json"))
                //{
                //    try
                //    {
                //        request.Content = content;
                //        var response = await httpClient.SendAsync(request).ConfigureAwait(false);
                //        response.EnsureSuccessStatusCode();
                //        await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                //    }
                //    catch (Exception ex)
                //    {
                //        string debug = ex.Message;
                //        debug = debug;
                //    }
                //}

                //try
                //{
                //    string jsonMsg = "{\"data\":{\"message\":\"" + msg + "\"}}";
                //    StringContent ggg = new StringContent("\"" + msg + "\"",
                //        System.Text.Encoding.UTF8, "application/json");

                //    httpClient.DefaultRequestHeaders.Add("Authorization", createToken(url, "DefaultFullSharedAccessSignature", "wGG0K2tMXLobdZuzHzlGNJl5skJjzOHtqfii53pq/E8="));
                //    httpClient.DefaultRequestHeaders.Add("ServiceBusNotification-Tags", "EldersProtection");
                //    httpClient.DefaultRequestHeaders.Add("ServiceBusNotification-Format", "gcm");
                //    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                //    var response = await httpClient.PostAsync(url, new StringContent(jsonMsg,
                //        System.Text.Encoding.UTF8));
                //    response.EnsureSuccessStatusCode();
                //    string result = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                //}
                //catch (Exception ex)
                //{
                //    string debug = ex.Message;
                //    debug = debug;
                //}
            }
        }

        private static async void SendNotificationAsync(string msg)
        {
            NotificationHubClient hub = NotificationHubClient
                .CreateClientFromConnectionString("Endpoint=sb://eldersprotection.servicebus.windows.net/;SharedAccessKeyName=DefaultFullSharedAccessSignature;SharedAccessKey=wGG0K2tMXLobdZuzHzlGNJl5skJjzOHtqfii53pq/E8=", "EldersProtection");
            var notif = "{ \"data\" : {\"message\":\"" + msg + "\"}}";
            await hub.SendGcmNativeNotificationAsync(notif);
        }

        private static string createToken(string resourceUri, string keyName, string key)
        {
            TimeSpan sinceEpoch = DateTime.Now - new DateTime(1970, 1, 1);
            var week = new TimeSpan(7, 0, 0, 0).Seconds;
            var expiry = Convert.ToString((int)sinceEpoch.TotalSeconds + week);
            string stringToSign = HttpUtility.UrlEncode(resourceUri) + "\n" + expiry;
            HMACSHA256 hmac = new HMACSHA256(Encoding.UTF8.GetBytes(key));
            var signature = Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(stringToSign)));
            var sasToken = String.Format(CultureInfo.InvariantCulture, "SharedAccessSignature sr={0}&sig={1}&se={2}&skn={3}", HttpUtility.UrlEncode(resourceUri), HttpUtility.UrlEncode(signature), expiry, keyName);
            return sasToken;
        }
    }
}