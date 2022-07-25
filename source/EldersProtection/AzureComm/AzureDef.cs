using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureComm
{
    /// <summary>
    /// 
    /// </summary>
    internal class AzureDef
    {
        /// <summary>
        /// Define parameters related to communication with Azure services
        /// </summary>
        public enum SpeechToTextMessageType
        {
            OnResponseReceived,
            OnPartialResponseReceived,
            OnConversationError
        }

        public static readonly string urlLUIS = @"https://westus.api.cognitive.microsoft.com/luis/v2.0/apps/a3f06ad5-15aa-421e-b90a-166cf492f91d?subscription-key=dac475825b924ea8840cdbd28d7a5aca&verbose=true&timezoneOffset=0&q=";

        public static string SubscriptionKey = "0c6e1d7e22644f63a0c7f7007dd47f3b";
        public static string DefaultLocale = "zh-TW";

        public class LUISResponse
        {
            public string query { get; set; }
            public lIntent[] intents { get; set; }
            public lEntity[] entities { get; set; }
        }

        public class lIntent
        {
            public string intent { get; set; }
            public float score { get; set; }
        }

        public class lEntity
        {
            public string entity { get; set; }
            public string type { get; set; }
            public int startIndex { get; set; }
            public int endIndex { get; set; }
            public float score { get; set; }
        }

        public class NotificationSettings
        {
            public static string SenderId = "1020648848413";
            public static string HubName = "EldersProtection";
            public static string HubListenConnectionString = "Endpoint=sb://eldersprotection.servicebus.windows.net/;SharedAccessKeyName=DefaultListenSharedAccessSignature;SharedAccessKey=3TJuZ+nSHNNL5aqDymXdR/jLt9Wr1fFDDSn9NLj3MwA=";
            public static string HubFullAccess = "Endpoint=sb://eldersprotection.servicebus.windows.net/;SharedAccessKeyName=DefaultFullSharedAccessSignature;SharedAccessKey=wGG0K2tMXLobdZuzHzlGNJl5skJjzOHtqfii53pq/E8=";
            public static string HubEndpoint = "https://eldersprotection.servicebus.windows.net/";
            public static string Token = "SharedAccessSignature sr=https%3a%2f%2feldersprotection.servicebus.windows.net%2feldersprotection%2fmessages%2f%3fapi-version%3d2015-01&sig=yuB68sBLDN5buCHNSljKBmMf1CcIS59DlcB0dH5uG90%3D&se=1508179689&skn=DefaultFullSharedAccessSignature";
        }
    }
}