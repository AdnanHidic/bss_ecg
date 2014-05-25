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
        
        /* Attributes specific for Standard MIT annotations */
        public int Code { get; set; }

        public int Chan { get; set; }
        
        public int SubTyp { get; set; }
        
        public int Num { get; set; }

        public String Aux { get; set; }
        /* End */

        public override String ToString()
        {
            return "Type " + Type + " at " + TimeIndex+ " [seconds]";
        }

        static public String getStandardAnnotationTextFromCode(int annotationCode)
        {
            switch(annotationCode)
            {
                case 1:
                    return "NORMAL";
                case 2:
                    return "LBBB";
                case 3:
                    return "RBBB";
                case 4:
                    return "ABERR";
                case 5:
                    return "PVC";
                case 6:
                    return "FUSION";
                case 7:
                    return "NPC";
                case 8:
                    return "APC";
                case 9:
                    return "SVPB";
                case 10:
                    return "VESC";
                case 11:
                    return "NESC";
                case 12:
                    return "PACE";
                case 13:
                    return "UNKNOWN";
                case 14:
                    return "NOISE";
                case 16:
                    return "ARFCT";
                case 18:
                    return "STCH";
                case 19:
                    return "TCH";
                case 20:
                    return "SYSTOLE";
                case 21:
                    return "DIASTOLE";
                case 22:
                    return "NOTE";
                case 23:
                    return "MEASURE";
                case 24:
                    return "PWAVE";
                case 25:
                    return "BBB";
                case 26:
                    return "PACESP";
                case 27:
                    return "TWAVE";
                case 28:
                    return "RHYTHM";
                case 29: 
                    return "UWAVE";
                case 30:
                    return "LEARN";
                case 31:
                    return "FLWAV";
                case 32:
                    return "VFON";
                case 33:
                    return "VFOFF";
                case 34:
                    return "AESC";
                case 35:
                    return "SVESC";
                case 36:
                    return "LINK";
                case 37:
                    return "NAPC";
                case 38:
                    return "PFUS";
                case 39:
                    return "WFON";
                case 40:
                    return "WFOFF";
                case 41:
                    return "RONT";
                default:
                    return "";
            }
        }
    }

    /// <summary>
    /// Annotations can be default physionet ones, or custom ones for exams, e.g. solution and answer.
    /// </summary>
    public enum ANNOTATION_TYPE { PHYSIONET_STANDARD, SOLUTION, ANSWER }
}
