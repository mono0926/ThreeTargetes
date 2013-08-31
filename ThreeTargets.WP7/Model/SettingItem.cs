using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using GalaSoft.MvvmLight;

namespace Mono.App.ThreeTargets.Model
{
    public class SettingItem : ViewModelBase
    {
        private string title;
        private Guid id;
        private bool isChecked;

        private const string titlePropertyName = "Title";
        private const string IDPropertyName = "ID";
        private const string isCheckedPropertyName = "IsChecked";

        public string Title
        {
            get
            {
                return title;
            }
            set
            {
                title = value;
                RaisePropertyChanged(titlePropertyName);
            }
        }

        public Guid ID
        {
            get
            {
                return id;
            }
            set
            {
                id = value;
                RaisePropertyChanged(IDPropertyName);
            }
        }

        public bool IsChecked
        {
            get
            {
                return isChecked;
            }
            set
            {
                isChecked = value;
                RaisePropertyChanged(isCheckedPropertyName);
            }
        }
    }
}