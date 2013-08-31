using System;
using System.Windows;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.Phone.Controls;

namespace Mono.App.ThreeTargets.Views
{
    /// <summary>
    /// Description for SettingView.
    /// </summary>
    public partial class SettingView : PhoneApplicationPage
    {
        /// <summary>
        /// Initializes a new instance of the SettingView class.
        /// </summary>
        public SettingView()
        {
            InitializeComponent();
        }

        private void ApplicationBarIconButton_Click(object sender, System.EventArgs e)
        {
            Messenger.Default.Send("", Contract.SaveSetting);
        }

        private void ApplicationBarIconButton_Click_1(object sender, System.EventArgs e)
        {
            var result = MessageBox.Show("目標含めて全データ消去します", "確認", MessageBoxButton.OKCancel);
            if (result == MessageBoxResult.OK)
            {
                Messenger.Default.Send("", Contract.ClearAll);
                NavigationService.GoBack();
                //NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
            }
        }
    }
}