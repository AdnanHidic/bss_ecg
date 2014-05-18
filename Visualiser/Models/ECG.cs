using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ComponentModel;
using OxyPlot;
using OxyPlot.Annotations;

namespace Visualiser.Models
{
    /// <summary>
    /// ECG signal class.
    /// </summary>
    public class ECG : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public int HeartRate { get; set; }
        public List<ECGPoint> Points { get; set; }
        public int SamplingRate { get; set; }
    }
}
