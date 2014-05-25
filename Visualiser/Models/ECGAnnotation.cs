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

        static public List<Tuple<int,String>> StandardAnnotationCodesAndDescs = new List<Tuple<int,String>>()
        {
            new Tuple<int,String>(1,"NORMAL"),
            new Tuple<int,String>(2,"LBBB"),
            new Tuple<int,String>(3,"RBBB"),
            new Tuple<int,String>(4,"ABERR"),
            new Tuple<int,String>(5,"PVC"),
            new Tuple<int,String>(6,"FUSION"),
            new Tuple<int,String>(7,"NPC"),
            new Tuple<int,String>(8,"APC"),
            new Tuple<int,String>(9,"SVPB"),
            new Tuple<int,String>(10,"VESC"),
            new Tuple<int,String>(11,"NESC"),
            new Tuple<int,String>(12,"PACE"),
            new Tuple<int,String>(13,"UNKNOWN"),
            new Tuple<int,String>(14,"NOISE"),
            new Tuple<int,String>(16,"ARFCT"),
            new Tuple<int,String>(18,"STCH"),
            new Tuple<int,String>(19,"TCH"),
            new Tuple<int,String>(20,"SYSTOLE"),
            new Tuple<int,String>(21,"DIASTOLE"),
            new Tuple<int,String>(22,"NOTE"),
            new Tuple<int,String>(23,"MEASURE"),
            new Tuple<int,String>(24,"PWAVE"),
            new Tuple<int,String>(25,"BBB"),
            new Tuple<int,String>(26,"PACESP"),
            new Tuple<int,String>(27,"TWAVE"),
            new Tuple<int,String>(28,"RHYTHM"),
            new Tuple<int,String>(29,"UWAVE"),
            new Tuple<int,String>(30,"LEARN"),
            new Tuple<int,String>(31,"FLWAV"),
            new Tuple<int,String>(32,"VFON"),
            new Tuple<int,String>(33,"VFOFF"),
            new Tuple<int,String>(34,"AESC"),
            new Tuple<int,String>(35,"SVESC"),
            new Tuple<int,String>(36,"LINK"),
            new Tuple<int,String>(37,"NAPC"),
            new Tuple<int,String>(38,"PFUS"),
            new Tuple<int,String>(39,"WFON"),
            new Tuple<int,String>(40,"WFOFF"),
            new Tuple<int,String>(41,"RONT")
        };

        static public String getStandardAnnotationTextFromCode(int annotationCode)
        {
            try {
                return StandardAnnotationCodesAndDescs.First(pair => { return pair.Item1 == annotationCode; }).Item2;
            }
            catch(Exception){
                return "";
            }
        }

        static public int getCodeFromStandardAnnotationText(String annotationText)
        {
            try
            {
                return StandardAnnotationCodesAndDescs.First(pair => { return pair.Item2 == annotationText; }).Item1;
            }
            catch (Exception)
            {
                return -1;
            }
        }
    }

    /// <summary>
    /// Annotations can be default physionet ones, or custom ones for exams, e.g. solution and answer.
    /// </summary>
    public enum ANNOTATION_TYPE { PHYSIONET_STANDARD, SOLUTION, ANSWER }
}
