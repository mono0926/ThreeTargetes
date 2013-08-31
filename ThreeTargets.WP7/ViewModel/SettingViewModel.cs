using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Mono.App.ThreeTargets.Model;

namespace Mono.App.ThreeTargets.ViewModel
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// Use the <strong>mvvminpc</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// <para>
    /// You can also use Blend to data bind with the tool's support.
    /// </para>
    /// <para>
    /// See http://www.galasoft.ch/mvvm/getstarted
    /// </para>
    /// </summary>
    public class SettingViewModel : ViewModelBase
    {
        private ObservableCollection<SettingItem> items;

        public ObservableCollection<SettingItem> Items
        {
            get
            {
                return items;
            }
            set
            {
                items = value;
            }
        }

        private bool isLiveChecked = true;
        private string IsLivePropertyName = "IsLiveChecked";

        public bool IsLiveChecked
        {
            get
            {
                return isLiveChecked;
            }
            set
            {
                isLiveChecked = value;
                RaisePropertyChanged(IsLivePropertyName);
            }
        }

        private void Initialize()
        {
            var setting = (App.Current as App).Setting;

            IsLiveChecked = setting.IsLiveChecked;

            var random = new SettingItem { Title = "ランダム" };

            if (setting.ID.Equals(Guid.Empty))
            {
                random.IsChecked = true;
            }
            Items = new ObservableCollection<SettingItem>();
            Items.Add(random);
            foreach (var m in (App.Current as App).Manifests.Reverse())
            {
                var s = new SettingItem { Title = m.Title, ID = m.ID };
                if (m.ID.Equals(setting.ID))
                {
                    s.IsChecked = true;
                }
                Items.Add(s);
            }
        }

        /// Initializes a new instance of the SettingViewModel class.
        /// </summary>
        public SettingViewModel()
        {
            Initialize();

            Messenger.Default.Register<string>(this, Contract.ClearAll,
                s =>
                {
                    IOManager.GetManager().DeleteAll();
                    Messenger.Default.Send("", Contract.InitializeFail);
                });

            Messenger.Default.Register<string>(this, Contract.LoadSetting,
                s =>
                {
                    Initialize();
                });

            Messenger.Default.Register<string>(this, Contract.SaveSetting,
                s =>
                {
                    IOManager.GetManager().SaveSetting(this);
                    Messenger.Default.Send("", Contract.UpdateAgent);
                });
            ////if (IsInDesignMode)
            ////{
            ////    // Code runs in Blend --> create design time data.
            ////}
            ////else
            ////{
            ////    // Code runs "for real": Connect to service, etc...
            ////}
        }

        ////public override void Cleanup()
        ////{
        ////    // Clean own resources if needed

        ////    base.Cleanup();
        ////}
    }
}