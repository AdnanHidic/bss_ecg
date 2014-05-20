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
    public class ECG
    {
        /// <summary>
        /// Name of the signal, e.g. "100" part of "100(.dat|.atr|.hea)"
        /// </summary>
        public String Name { get; set; }
        /// <summary>
        /// Heart rate is measured in BPM (beats per minute)
        /// </summary>
        public double HeartRate { get; set; }
        /// <summary>
        /// ECG points that are the result of ECG measurement. This property enables us to plot the actual signal.
        /// </summary>
        public List<ECGPoint> Points { get; set; }
        /// <summary>
        /// Sampling rate can be ascertained from the distance between two ECGPoints, but this is a conveinance.
        /// </summary>
        public int SamplingRate { get; set; }
        /// <summary>
        /// List of annotations for current signal.
        /// </summary>
        public List<ECGAnnotation> Annotations { get; set; }
        /// <summary>
        /// List of ECG points that are R-spikes for current signal. Is seperately updated from Points attribute.
        /// </summary>
        public List<ECGPoint> Spikes { get; set; }
    }
}
