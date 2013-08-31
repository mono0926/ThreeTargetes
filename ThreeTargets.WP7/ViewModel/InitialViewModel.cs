using System;
using System.Diagnostics;
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
    public class InitialViewModel : ViewModelBase
    {
        private string first;
        private string second;
        private string third;

        private const string firstPropertyName = "First";
        private const string secondPropertyName = "Second";
        private const string thirdPropertyName = "Third";

        public string First
        {
            get
            {
                return first;
            }
            set
            {
                first = value;
                RaisePropertyChanged(firstPropertyName);
                SubmitCommand.RaiseCanExecuteChanged();
            }
        }

        public string Second
        {
            get
            {
                return second;
            }
            set
            {
                second = value;
                RaisePropertyChanged(secondPropertyName);
                SubmitCommand.RaiseCanExecuteChanged();
            }
        }

        public string Third
        {
            get
            {
                return third;
            }
            set
            {
                third = value;
                RaisePropertyChanged(thirdPropertyName);
                SubmitCommand.RaiseCanExecuteChanged();
            }
        }

        public RelayCommand SubmitCommand { get; private set; }

        /// <summary>
        /// Initializes a new instance of the InitialViewModel class.
        /// </summary>
        public InitialViewModel()
        {
            SubmitCommand = new RelayCommand(
                () =>
                {
                    IOManager.GetManager().WriteInitial(new ManifestModel { Title = First }, new ManifestModel { Title = Second }, new ManifestModel { Title = Third });
                    App.UpdateTile(First, 3);
                    Messenger.Default.Send("", Contract.InitializeSuccess);
                },
            () =>
            {
                return !string.IsNullOrEmpty(First) && !string.IsNullOrEmpty(Second) && !string.IsNullOrEmpty(Third);
            });

            Messenger.Default.Register<string>(this, Contract.ClearInitial,
                s =>
                {
                    First = string.Empty;
                    Second = string.Empty;
                    Third = string.Empty;
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