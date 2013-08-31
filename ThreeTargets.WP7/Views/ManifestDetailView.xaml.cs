using System.Windows;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.Phone.Controls;

namespace Mono.App.ThreeTargets.Views
{
    /// <summary>
    /// Description for ManifestDetailView.
    /// </summary>
    public partial class ManifestDetailView : PhoneApplicationPage
    {
        /// <summary>
        /// Initializes a new instance of the ManifestDetailView class.
        /// </summary>
        public ManifestDetailView()
        {
            InitializeComponent();
        }

        private void ApplicationBarIconButton_Click(object sender, System.EventArgs e)
        {
            Messenger.Default.Send("", Contract.AddDesc);
        }

        private void ApplicationBarIconButton_Click_1(object sender, System.EventArgs e)
        {
            Messenger.Default.Send("", Contract.SaveManifest);
        }

        private void ApplicationBarIconButton_Click_2(object sender, System.EventArgs e)
        {
            Messenger.Default.Send("", Contract.ConfirmDone);
        }
    }
}