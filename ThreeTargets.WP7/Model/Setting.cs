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

namespace Mono.App.ThreeTargets.Model
{
    public struct Setting
    {
        public bool IsLiveChecked { get; set; }

        public Guid ID { get; set; }
    }
}