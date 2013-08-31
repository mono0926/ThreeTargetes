using System;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.Phone.Controls;

namespace Mono.App.ThreeTargets.Views
{
    /// <summary>
    /// Description for InitialView.
    /// </summary>
    public partial class InitialView : PhoneApplicationPage
    {
        /// <summary>
        /// Initializes a new instance of the InitialView class.
        /// </summary>
        public InitialView()
        {
            InitializeComponent();
            Messenger.Default.Register<string>(this, Contract.InitializeSuccess,
        s =>
        {
            NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
            //NavigationService.GoBack();
        }
        );
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            Messenger.Default.Send("", Contract.ClearInitial);
            NavigationService.RemoveBackEntry();
        }

        //protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        //{
        //    App.Quit();
        //}
    }
}