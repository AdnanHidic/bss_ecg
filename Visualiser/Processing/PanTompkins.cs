using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Visualiser.Models;

namespace Visualiser.Processing
{
    /// <summary>
    /// Static class that implements Pan-Tompkins QRS detection algorithm.
    /// </summary>
    public static class PanTompkins
    {
        /// <summary>
        /// Performs QRS-complex detection on ECG signal provided as call argument.
        /// </summary>
        /// <param name="signal">ECG signal object</param>
        /// <returns>List of ECG points determined to be R spikes and Decimal value for measured heart-rate</returns>
        public static Tuple<List<ECGPoint>,Decimal> QRS_Detect(ECG signal)
        {
            throw new NotImplementedException();
        }
    }
}
