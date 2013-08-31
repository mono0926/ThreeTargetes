using System;
using System.Collections.Generic;
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
    public class ManifestModel : ViewModelBase
    {
        public Guid ID { get; set; }

        public string Title { get; set; }

        public IList<ContentModel> Contents { get; set; }

        private bool isDone;

        public bool IsDone
        {
            get { return isDone; }
            set
            {
                isDone = value;
                RaisePropertyChanged("IsDone");
            }
        }

        public ManifestModel()
        {
            Contents = new List<ContentModel>();
        }
    }
}