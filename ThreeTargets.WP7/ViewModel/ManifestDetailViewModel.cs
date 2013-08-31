using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
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
    public class ManifestDetailViewModel : ViewModelBase
    {
        public ICommand DeleteCommand { get; set; }

        public Guid ID { get; set; }

        public bool IsDone { get; set; }

        private string title;

        public string Title
        {
            get
            {
                return title;
            }
            set
            {
                title = value;
                RaisePropertyChanged("Title");
            }
        }

        public ObservableCollection<ContentModel> Contents { get; set; }

        /// <summary>
        /// Initializes a new instance of the ManifestDetailViewModel class.
        /// </summary>
        public ManifestDetailViewModel()
        {
            DeleteCommand = new RelayCommand<string>((s) =>
            {
                Debug.WriteLine(s);
                var remove = (from c in Contents where c.Description.Equals(s) select c).FirstOrDefault();
                if (remove != null)
                {
                    Contents.Remove(remove);
                }
            });
            Update();
            Messenger.Default.Register<string>(this, Contract.UpdateDetail,
                s =>
                {
                    Update();
                });
            Messenger.Default.Register<string>(this, Contract.AddDesc,
                s =>
                {
                    Contents.Add(new ContentModel { Description = "" });
                });

            Messenger.Default.Register<string>(this, Contract.SaveManifest,
                s =>
                {
                    var manifest = new ManifestModel() { ID = this.ID, Title = this.title, Contents = this.Contents, IsDone = this.IsDone };
                    Messenger.Default.Send(manifest, Contract.UpdateMain);
                    IOManager.GetManager().UpdateManifest(manifest);
                });

            Messenger.Default.Register<string>(this, Contract.ConfirmDone,
                s =>
                {
                    var message = "達成済みにしますか？";
                    if (IsDone)
                    {
                        message = "未達成にしますか？";
                    }
                    var result = MessageBox.Show(message, "確認", MessageBoxButton.OKCancel);
                    if (result == MessageBoxResult.OK)
                    {
                        IsDone = !IsDone;
                        Messenger.Default.Send("", Contract.SaveManifest);
                        Messenger.Default.Send<Guid>(ID, Contract.ChangeCurrentManifest);
                    }
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

        private void Update()
        {
            var manifest = (App.Current as App).Manifests.Where(m => m.ID.Equals((App.Current as App).SelectedManifestID)).First();
            Title = manifest.Title;
            ID = manifest.ID;
            IsDone = manifest.IsDone;
            Contents = new ObservableCollection<ContentModel>();
            foreach (var c in manifest.Contents)
            {
                Contents.Add(c);
            }
        }

        ////public override void Cleanup()
        ////{
        ////    // Clean own resources if needed

        ////    base.Cleanup();
        ////}
    }
}