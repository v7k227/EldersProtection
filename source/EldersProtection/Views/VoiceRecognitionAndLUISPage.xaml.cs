// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StepsControl.xaml.cs" company="saramgsilva">
//   Copyright (c) 2014 saramgsilva. All rights reserved.
// </copyright>
// <summary>
//   Interaction logic for StepsControl.xaml
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Diagnostics;
using FirstFloor.ModernUI.Windows;
using FirstFloor.ModernUI.Windows.Navigation;
using System.Threading;
using AzurePublic;
using System;
using System.Media;
using System.Windows.Controls;
using System.Windows.Resources;
using System.Windows;
using Newtonsoft.Json;
using static AzurePublic.AzureDef;

namespace EldersProtection.Views
{
    /// <summary>
    /// Interaction logic for StepsControl.xaml.
    /// </summary>
    public partial class VoiceRecognitionAndLUISPage : IContent
    {
        private AzureSpeechToText speechToText;

        /// <summary>
        /// Initializes a new instance of the <see cref="StepsControl"/> class.
        /// </summary>
        public VoiceRecognitionAndLUISPage()
        {
            InitializeComponent();

            speechToText = new AzureSpeechToText();
            speechToText.ReceiveMessageEvent += SpeechToText_ReceiveMessageEvent;
        }

        private async void SpeechToText_ReceiveMessageEvent(AzureDef.SpeechToTextMessageType speechToTextMessageType, string message)
        {
            try
            {
                Dispatcher.Invoke(() =>
                {
                    UpdateInfo(message);
                });

                if (speechToTextMessageType == AzureDef.SpeechToTextMessageType.OnResponseReceived)
                {
                    Dispatcher.Invoke(() =>
                    {
                        tbShowResult.Text = string.Format("分析字串為 : {0}", message);
                    });

                    string result = await AzureLUIS.Run(message);

                    if (!string.IsNullOrEmpty(result))
                    {
                        Dispatcher.Invoke(() =>
                        {
                            UpdateInfo(result);
                        });

                        var _Data = JsonConvert.DeserializeObject<LUISResponse>(result);

                        Dispatcher.Invoke(() =>
                        {
                            tbShowResult.Text += Environment.NewLine + string.Format("分析結果為 : {0}({1})", _Data.intents[0].intent.Replace("Fraud", "詐騙"), _Data.intents[0].score);
                        });
                    }
                }
            }
            catch (Exception exc)
            {
            }
        }

        private void UpdateInfo(string msg)
        {
            tbInfo.Text += string.Format("{0} : {1}", DateTime.Now.ToString("HH:mm:ss fff"), msg) + Environment.NewLine;

            sv.ScrollToEnd();
            sv.ScrollToBottom();
        }

        /// <summary>
        /// Called when navigation to a content fragment begins.
        /// </summary>
        /// <param name="e">An object that contains the navigation data.</param>
        public void OnFragmentNavigation(FragmentNavigationEventArgs e)
        {
            Debug.WriteLine("StepsControl- OnFragmentNavigation");
        }

        /// <summary>
        /// Called when this instance is no longer the active content in a frame.
        /// </summary>
        /// <param name="e">An object that contains the navigation data.</param>
        public void OnNavigatedFrom(NavigationEventArgs e)
        {
            Debug.WriteLine("StepsControl -OnNavigatedFrom");
        }

        /// <summary>
        /// Called when a this instance becomes the active content in a frame.
        /// </summary>
        /// <param name="e">An object that contains the navigation data.</param>
        public void OnNavigatedTo(NavigationEventArgs e)
        {
            Debug.WriteLine("StepsControl- OnNavigatedTo");
        }

        /// <summary>
        /// Called just before this instance is no longer the active content in a frame.
        /// </summary>
        /// <param name="e">
        /// An object that contains the navigation data.
        /// </param>
        /// <remarks>
        /// The method is also invoked when parent frames are about to navigate.
        /// </remarks>
        public void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            Debug.WriteLine("StepsControl- OnNavigatingFrom");
        }

        private void ModernButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Checking();
        }

        private void Checking()
        {
            SoundPlayer snd = null;
            string audioName = (string)((ComboBoxItem)cbTalkDetail.SelectedItem).Tag;
            switch (audioName)
            {
                case "1.wav":
                    snd = new SoundPlayer(Properties.Resources._1);
                    break;

                case "2.wav":
                    snd = new SoundPlayer(Properties.Resources._2);
                    break;

                case "3.wav":
                    snd = new SoundPlayer(Properties.Resources._3);
                    break;
            }

            ManualResetEvent mre = new ManualResetEvent(false);
            ThreadPool.QueueUserWorkItem(arg =>
            {
                snd.PlaySync();
                mre.Set();
            });

            mre.WaitOne();

            Uri u = new Uri(string.Format(@"EldersProtection;component/res/{0}", audioName), UriKind.Relative);
            StreamResourceInfo resInfo = Application.GetResourceStream(u);
            speechToText.Run(resInfo.Stream);
            //
        }
    }
}