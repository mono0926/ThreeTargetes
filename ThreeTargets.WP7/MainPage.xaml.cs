using System;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Scheduler;
using Mono.App.ThreeTargets.Model;
using Mono.App.ThreeTargets.ViewModel;

namespace Mono.App.ThreeTargets
{
    public partial class MainPage : PhoneApplicationPage
    {
        private PeriodicTask periodicTask;
        string periodicTaskName = "periodicAgent";

        // Constructor
        public MainPage()
        {
            InitializeComponent();

            Messenger.Default.Register<string>(this, Contract.RedirectToInitial,
                s =>
                {
                    NavigationService.Navigate(new Uri("/Views/InitialView.xaml", UriKind.Relative));
                }
             );
        }

        private void StartPeriodicAgent()
        {
            periodicTask = ScheduledActionService.Find(periodicTaskName) as PeriodicTask;
            if (periodicTask != null && !periodicTask.IsEnabled)
            {
                MessageBox.Show("Background agents for this application have been disabled by the user.");
                return;
            }
            if (periodicTask != null && periodicTask.IsEnabled)
            {
                RemoveAgent(periodicTaskName);
            }
            var setting = (App.Current as App).Setting;
            var manifests = (App.Current as App).Manifests;
            var count = manifests.Where(m => !m.IsDone).Count();
            if (!setting.IsLiveChecked)
            {
                App.InValidateTile(count);
                return;
            }
            else if (!setting.ID.Equals(Guid.Empty))
            {
                var title = manifests.Where(m => m.ID.Equals(setting.ID)).First().Title;
                App.UpdateTile(title, count);
                return;
            }

            periodicTask = new PeriodicTask(periodicTaskName);
            periodicTask.Description = "設定画面でライブタイルの表示をランダムにした際、目標をランダム表示させます。";
            ScheduledActionService.Add(periodicTask);

#if(DEBUG)
            ScheduledActionService.LaunchForTest(periodicTaskName, TimeSpan.FromSeconds(60));
#endif
        }

        private void RemoveAgent(string periodicTaskName)
        {
            try
            {
                ScheduledActionService.Remove(periodicTaskName);
            }
            catch (Exception e)
            {
                //MessageBox.Show(e.Message);
            }
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (NavigationService.CanGoBack)
            {
                NavigationService.RemoveBackEntry();
            }

            Messenger.Default.Register<string>(this, Contract.UpdateAgent,
                s =>
                {
                    StartPeriodicAgent();
                });

            var viewModel = DataContext as MainViewModel;
            if (!viewModel.IsDataLoaded)
            {
                viewModel.LoadData();
            }

            (App.Current as App).Setting = IOManager.GetManager().LoadSetting();
        }

        private void Panorama_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                var manifest = e.AddedItems[0] as ManifestModel;
                Messenger.Default.Send(manifest.ID, Contract.ChangeCurrentManifest);
            }
        }

        private void ApplicationBarIconButton_Click(object sender, EventArgs e)
        {
            Messenger.Default.Send("", Contract.LoadSetting);
            NavigationService.Navigate(new Uri("/Views/SettingView.xaml", UriKind.Relative));
        }

        private void ListBox_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            Messenger.Default.Send("", Contract.UpdateDetail);
            NavigationService.Navigate(new Uri("/Views/ManifestDetailView.xaml", UriKind.Relative));
        }
    }
}