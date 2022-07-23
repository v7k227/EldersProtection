using Microsoft.CognitiveServices.SpeechRecognition;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static AzurePublic.AzureDef;

namespace AzurePublic
{
    /// <summary>
    /// Transform speech to text data for analyzing the key word.
    /// </summary>
    internal class AzureSpeechToText
    {
        private DataRecognitionClient dataClient;

        public void Run(string LongWaveFile)
        {
            if (null == this.dataClient)
                CreateDataRecoClient();

            this.SendAudioHelper(LongWaveFile);
        }

        public void Run(Stream LongWaveFile)
        {
            if (null == this.dataClient)
                CreateDataRecoClient();

            this.SendAudioHelper(LongWaveFile);
        }

        public delegate void ReceiveMessageHandler(SpeechToTextMessageType speechToTextMessageType, string message);

        public event ReceiveMessageHandler ReceiveMessageEvent;

        private void SendMessage(SpeechToTextMessageType speechToTextMessageType, string message)
        {
            if (ReceiveMessageEvent != null)
                ReceiveMessageEvent(speechToTextMessageType, message);
        }

        private void SendAudioHelper(string wavFileName)
        {
            using (FileStream fileStream = new FileStream(wavFileName, FileMode.Open, FileAccess.Read))
            {
                // Note for wave files, we can just send data from the file right to the server.
                // In the case you are not an audio file in wave format, and instead you have just
                // raw data (for example audio coming over bluetooth), then before sending up any
                // audio data, you must first send up an SpeechAudioFormat descriptor to describe
                // the layout and format of your raw audio data via DataRecognitionClient's sendAudioFormat() method.
                int bytesRead = 0;
                byte[] buffer = new byte[1024];

                try
                {
                    do
                    {
                        // Get more Audio data to send into byte buffer.
                        bytesRead = fileStream.Read(buffer, 0, buffer.Length);

                        // Send of audio data to service.
                        this.dataClient.SendAudio(buffer, bytesRead);
                    }
                    while (bytesRead > 0);
                }
                finally
                {
                    // We are done sending audio.  Final recognition results will arrive in OnResponseReceived event call.
                    this.dataClient.EndAudio();
                }
            }
        }

        private void SendAudioHelper(Stream fileStream)
        {
            // Note for wave files, we can just send data from the file right to the server.
            // In the case you are not an audio file in wave format, and instead you have just
            // raw data (for example audio coming over bluetooth), then before sending up any
            // audio data, you must first send up an SpeechAudioFormat descriptor to describe
            // the layout and format of your raw audio data via DataRecognitionClient's sendAudioFormat() method.
            int bytesRead = 0;
            byte[] buffer = new byte[1024];

            try
            {
                do
                {
                    // Get more Audio data to send into byte buffer.
                    bytesRead = fileStream.Read(buffer, 0, buffer.Length);

                    // Send of audio data to service.
                    this.dataClient.SendAudio(buffer, bytesRead);
                }
                while (bytesRead > 0);
            }
            finally
            {
                // We are done sending audio.  Final recognition results will arrive in OnResponseReceived event call.
                this.dataClient.EndAudio();
            }
        }

        private void CreateDataRecoClient()
        {
            this.dataClient = SpeechRecognitionServiceFactory.CreateDataClient(
                SpeechRecognitionMode.LongDictation,
                AzureDef.DefaultLocale,
                AzureDef.SubscriptionKey);
            this.dataClient.AuthenticationUri = "";

            // Event handlers for speech recognition results

            this.dataClient.OnResponseReceived += this.OnDataDictationResponseReceivedHandler;

            this.dataClient.OnPartialResponseReceived += this.OnPartialResponseReceivedHandler;
            this.dataClient.OnConversationError += this.OnConversationErrorHandler;
        }

        private void OnDataDictationResponseReceivedHandler(object sender, SpeechResponseEventArgs e)
        {
            this.WriteResponseResult(e);
        }

        private void OnPartialResponseReceivedHandler(object sender, PartialSpeechResponseEventArgs e)
        {
            SendMessage(SpeechToTextMessageType.OnPartialResponseReceived, "--- Partial result received by OnPartialResponseReceivedHandler() ---");
            SendMessage(SpeechToTextMessageType.OnPartialResponseReceived, string.Format("{0}", e.PartialResult));
        }

        private void OnConversationErrorHandler(object sender, SpeechErrorEventArgs e)
        {
            SendMessage(SpeechToTextMessageType.OnConversationError, string.Format("Error code: {0}", e.SpeechErrorCode.ToString()));
            SendMessage(SpeechToTextMessageType.OnConversationError, string.Format("Error text: {0}", e.SpeechErrorText));
        }

        private void WriteResponseResult(SpeechResponseEventArgs e)
        {
            if (e.PhraseResponse.Results.Length == 0)
            {
                //SendMessage(SpeechToTextMessageType.OnResponseReceived, "No phrase response is available.");
            }
            else
            {
                SendMessage(SpeechToTextMessageType.OnResponseReceived, e.PhraseResponse.Results[0].DisplayText);
            }
        }
    }
}