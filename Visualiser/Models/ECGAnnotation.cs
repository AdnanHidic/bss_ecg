using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Visualiser.Models
{
    /// <summary>
    /// ECG Annotation is rendered along with the signal to describe it.
    /// </summary>
    public class ECGAnnotation
    {
        /// <summary>
        /// Gets or sets annotation type.
        /// </summary>
        public ANNOTATION_TYPE Type { get; set; }
        /// <summary>
        /// Gets or sets annotation text.
        /// </summary>
        public String Text { get; set; }
        /// <summary>
        /// Gets or sets timeindex which annotation belongs to. It is given in seconds. e.g. if sampling rate is 360Hz, then the 1st point has TI of 0, 2nd of 0.002, 3rd of 0.004 etc.
        /// </summary>
        public double TimeIndex { get; set; }

        public override String ToString()
        {
            return "Type " + Type + " at " + TimeIndex+ " [seconds]";
        }
    }

    /// <summary>
    /// Annotations can be default physionet ones, or custom ones for exams, e.g. solution and answer.
    /// </summary>
    public enum ANNOTATION_TYPE { PHYSIONET_STANDARD, SOLUTION, ANSWER }
}
