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

        private PlotModel plotModel;
        public PlotModel PlotModel
        {
            get { return plotModel; }
            set { plotModel = value; OnPropertyChanged("PlotModel"); }
        }

        public ECG()
        {
            PlotModel = new PlotModel();
        }


        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
