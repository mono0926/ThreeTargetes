using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Mono.App.ThreeTargets.Model;

namespace Mono.App.ThreeTargets.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
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
    public class MainViewModel : ViewModelBase
    {
        public ObservableCollection<ManifestModel> Manifests { get; private set; }

        public bool IsDataLoaded
        {
            get;
            private set;
        }

        private bool selectedDone;

        public bool SelectedDone
        {
            get
            {
                return selectedDone;
            }
            set
            {
                selectedDone = value;
                RaisePropertyChanged("SelectedDone");
            }
        }

        //public RelayCommand EditCommand { get; set; }

        /// <summary>
        /// いくつかの ItemViewModel オブジェクトを作成し、Items コレクションに追加します。
        /// </summary>
        public void LoadData()
        {
            var manifests = IOManager.GetManager().LoadManifests().Reverse().ToList();// as List<ManifestModel>;
            if (manifests.Count > 0)
            {
                for (var i = 0; i < manifests.Count; i++)// m in manifests)
                {
                    var m = manifests[i];
                    if (m.Contents.Count == 0)
                    {
                        m.Contents.Add(new ContentModel { Description = "タップすると、目標の編集・小目標の追加ができます。" });
                    }
                    Manifests.Add(m);
                }
                (App.Current as App).SelectedManifestID = Manifests[0].ID;
                SelectedDone = Manifests[0].IsDone;

                IOManager.GetManager().LoadSetting();
                this.IsDataLoaded = true;
                Messenger.Default.Send("", Contract.UpdateAgent);
            }
            else
            {
                Messenger.Default.Send("", Contract.RedirectToInitial);
                Messenger.Default.Send("", Contract.InitializeFail);
            }
        }

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel()
        {
            this.Manifests = new ObservableCollection<ManifestModel>();

            Messenger.Default.Register<string>("", Contract.InitializeFail,
                s =>
                {
                    this.IsDataLoaded = false;
                    this.Manifests = new ObservableCollection<ManifestModel>();
                });

            Messenger.Default.Register<Guid>(this, Contract.ChangeCurrentManifest,
                id =>
                {
                    (App.Current as App).SelectedManifestID = id;
                    SelectedDone = (App.Current as App).Manifests.Where(m => m.ID.Equals(id)).First().IsDone;
                });

            Messenger.Default.Register<ManifestModel>(this, Contract.UpdateMain,
                m =>
                {
                    var old = Manifests.Where(mm => mm.ID.Equals(m.ID)).FirstOrDefault();
                    if (old != null)
                    {
                        Manifests.Remove(old);
                        Manifests.Insert(0, m);
                    }
                });

            if (IsInDesignMode)
            {
                // Code runs in Blend --> create design time data.
            }
            else
            {
            }
        }

        ////public override void Cleanup()
        ////{
        ////    // Clean up if needed

        ////    base.Cleanup();
        ////}
    }
}