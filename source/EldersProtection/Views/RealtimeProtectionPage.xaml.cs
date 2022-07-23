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

namespace EldersProtection.Views
{
    /// <summary>
    /// Interaction logic for StepsControl.xaml.
    /// </summary>
    public partial class RealtimeProtectionPage : IContent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StepsControl"/> class.
        /// </summary>
        public RealtimeProtectionPage()
        {
            InitializeComponent();
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

        private void Checking()
        {
            string testPhoneNo = tbPhoneNo.Text;
            tbPhoneNoAnalysis.Text = "分析中, 請稍後";
            Thread T = new Thread(() =>
            {
                string targetName = "";
                if (AzureStream.PhoneAnalyzer(testPhoneNo, ref targetName))
                {
                    Dispatcher.Invoke(() =>
                    {
                        tbPhoneNoAnalysis.Text = string.Format("該電話號碼{0}為詐騙電話,{1}來源為 : {2}", testPhoneNo, Environment.NewLine, targetName);
                    });
                }
                else
                {
                    Dispatcher.Invoke(() =>
                    {
                        tbPhoneNoAnalysis.Text = string.Format("該電話號碼為經判定後為安全電話");
                    });
                }
            });

            T.Start();
        }

        private void ModernButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Checking();
        }
    }
}