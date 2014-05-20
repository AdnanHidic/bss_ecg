using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Visualiser.Models
{
    /// <summary>
    /// ECGPoint holds sampling time index, measured value, corresponding ECG annotations 
    /// and value indicating if it is R spike.
    /// </summary>
    public class ECGPoint
    {
        /// <summary>
        /// Corresponds to x-axis. It is given in seconds. e.g. if sampling rate is 360Hz, then the 1st point has TI of 0, 2nd of 0.002, 3rd of 0.004 etc.
        /// </summary>
        public double TimeIndex { get; set; }
        /// <summary>
        /// Corresponds to y-axis. It is given in millivolts (mV). 
        /// </summary>
        public double Value { get; set; }
    }
}
