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
    public class ContentModel : ViewModelBase
    {
        private string description;
        private string detail;
        private const string descriptionPropertyName = "Description";
        private const string detailPropertyName = "Detail;";

        public string Description
        {
            get { return description; }
            set
            {
                description = value;
                RaisePropertyChanged(descriptionPropertyName);
            }
        }

        public string Detail
        {
            get { return detail; }
            set
            {
                detail = value;
                RaisePropertyChanged(detailPropertyName);
            }
        }
    }
}