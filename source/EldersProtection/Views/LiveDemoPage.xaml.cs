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
using AzureComm;
using System;
using System.Media;
using System.Windows.Controls;
using System.Windows.Resources;
using System.Windows;
using Newtonsoft.Json;
using static AzureComm.AzureDef;
using System.Speech.Synthesis;
using System.Collections.Generic;
using System.IO;
using System.Windows.Media.Imaging;

namespace EldersProtection.Views
{
    /// <summary>
    /// Interaction logic for StepsControl.xaml.
    /// </summary>
    public partial class LiveDemoPage : IContent
    {
        private string faceHappy = "/EldersProtection;component/res/happy.png";
        private string faceAnger = "/EldersProtection;component/res/anger.png";
        private string faceSad = "/EldersProtection;component/res/sad.png";

        private List<UnmanagedMemoryStream> audioCollection = new List<UnmanagedMemoryStream>()
        {
            {Properties.Resources._01},
            {Properties.Resources._02},
            {Properties.Resources._03},
            {Properties.Resources._04},
            {Properties.Resources._05},
            {Properties.Resources._06},
            {Properties.Resources._07},
            {Properties.Resources._08},
        };

        private List<string> nameCollection = new List<string>()
        {
            {"01.wav"},
            {"02.wav"},
            {"03.wav"},
            {"04.wav"},
            {"05.wav"},
            {"06.wav"},
            {"07.wav"},
            {"08.wav"},
        };

        private int lastPlayIndex;

        private AzureSpeechToText speechToText;
        private SpeechSynthesizer synth;

        /// <summary>
        /// Initializes a new instance of the <see cref="StepsControl"/> class.
        /// </summary>
        public LiveDemoPage()
        {
            InitializeComponent();

            speechToText = new AzureSpeechToText();
            speechToText.ReceiveMessageEvent += SpeechToText_ReceiveMessageEvent;

            synth = new SpeechSynthesizer();
            synth.SpeakCompleted += Synth_SpeakCompleted;
        }

        private void Synth_VisemeReached(object sender, VisemeReachedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private async void Synth_SpeakCompleted(object sender, SpeakCompletedEventArgs e)
        {
            if (lastPlayIndex != 7)
                Next();
            else
                await AzureNotification.SendMsgAsync("我們發現您的家人正在受騙, 請注意!");
        }

        public void StartVoice(string text)
        {
            synth.SetOutputToDefaultAudioDevice();

            synth.Speak(text);
        }

        private async void SpeechToText_ReceiveMessageEvent(AzureDef.SpeechToTextMessageType speechToTextMessageType, string message)
        {
            try
            {
                //Dispatcher.Invoke(() =>
                //{
                //    UpdateInfo(message);
                //});

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

                        if (lastPlayIndex == 0)
                        {
                            Dispatcher.Invoke(() =>
                            {
                                UpdateFace(faceAnger);
                            });
                            Next();
                        }
                        else if (lastPlayIndex == 1)
                        {
                            string msg = "不要緊張，先問他是哪個電話號碼";

                            Dispatcher.Invoke(() =>
                            {
                                UpdateFace(faceHappy);
                                tbTalk.Text = msg;
                            });

                            synth.SpeakAsync(msg);
                        }
                        else if (lastPlayIndex == 2)
                        {
                            Dispatcher.Invoke(() =>
                            {
                                UpdateFace(faceHappy);
                                tbTalk.Text = "";
                            });

                            Next();
                        }
                        else if (lastPlayIndex == 3)
                        {
                            Dispatcher.Invoke(() =>
                            {
                                UpdateFace(faceHappy);
                                tbTalk.Text = "";
                            });

                            Next();
                        }
                        else if (lastPlayIndex == 4)
                        {
                            string msg = "希望您直接打去中華電信確認";
                            Dispatcher.Invoke(() =>
                            {
                                UpdateFace(faceAnger);
                                tbTalk.Text = msg;
                            });

                            synth.SpeakAsync(msg);
                        }
                        else if (lastPlayIndex == 5)
                        {
                            string msg = "電信業者不會直接要銀行帳戶，請您先打給中華電信確認";
                            Dispatcher.Invoke(() =>
                            {
                                UpdateFace(faceAnger);
                                tbTalk.Text = msg;
                            });

                            synth.SpeakAsync(msg);
                        }
                        else if (lastPlayIndex == 6)
                        {
                            Dispatcher.Invoke(() =>
                            {
                                UpdateFace(faceAnger);
                                tbTalk.Text = "";
                            });

                            Next();
                        }
                        else if (lastPlayIndex == 7)
                        {
                            string msg = "我將通知您最信任的人, 協助您確認這個狀況是否為詐騙";
                            Dispatcher.Invoke(() =>
                            {
                                UpdateFace(faceSad);
                                tbTalk.Text = msg;
                            });

                            synth.SpeakAsync(msg);
                        }
                    }
                }
            }
            catch (Exception exc)
            {
            }
        }

        private void UpdateFace(string face)
        {
            BitmapImage logo = new BitmapImage();
            logo.BeginInit();
            logo.UriSource = new Uri(string.Format("pack://application:,,,{0}", face));
            logo.EndInit();

            imgFace.Source = logo;
        }

        private void UpdateInfo(string msg)
        {
            tbInfo.Text = string.Format("{0} : {1}", DateTime.Now.ToString("HH:mm:ss fff"), msg) + Environment.NewLine;
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

        private void Next()
        {
            lastPlayIndex++;
            SoundPlayer snd = new SoundPlayer(audioCollection[lastPlayIndex]);

            ManualResetEvent mre = new ManualResetEvent(false);
            ThreadPool.QueueUserWorkItem(arg =>
            {
                snd.PlaySync();
                mre.Set();
            });

            mre.WaitOne();

            Uri u = new Uri(string.Format(@"EldersProtection;component/res/{0}", nameCollection[lastPlayIndex]), UriKind.Relative);
            StreamResourceInfo resInfo = Application.GetResourceStream(u);
            speechToText.Run(resInfo.Stream);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            lastPlayIndex = -1;
            Next();
        }
    }
}