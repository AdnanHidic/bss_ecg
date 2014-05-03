using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Visualiser.Models
{
    /// <summary>
    /// ECGPoint holds sampling timestamp, decimal measured value, corresponding ECG annotations 
    /// and value indicating if it is R spike.
    /// </summary>
    public class ECGPoint
    {
        public DateTime Timestamp { get; set; }
        public Decimal Value { get; set; }
        public Boolean IsRPoint { get; set; }
        public List<ECGAnnotation> Annotations { get; set; }
    }
}
