using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Visualiser.Models;

namespace Visualiser.IO
{
    /// <summary>
    /// Static class used to perform IO operations on ECG signal model. 
    /// Class methods make it possible to load ECG model from .hea .dat and .atr PhysioNet files,
    /// and save edited ECG model to respective files as well.
    /// </summary>
    static public class IOManager
    {
        /// <summary>
        /// Used to generate ECG signal model from PhysioNet signal files. Method searches for .HEA, .DAT and .ATR files 
        /// with the name given as method call parameter.
        /// </summary>
        /// <param name="signalFileName">Full path to the HEA file (e.g. C:\100.HEA)</param>
        /// <returns>Generated ECG model.</returns>
        static public ECG loadECGFromSignalFile(String signalFileName)
        {
            // look for HEA ATR DAT & CUST on path etc.
            throw new NotImplementedException();
        }

        /// <summary>
        /// Used to save ECG signal model to PhysioNet signal files. Method generates .HEA, .DAT and .ATR files 
        /// with the name given as method call parameter.
        /// </summary>
        /// <param name="signalFileName">Path + signal name (e.g. C:\100)</param>
        static public void saveECGToSignalFiles(ECG signal, String signalFileName)
        {
            // save HEA ATR DAT & CUST
            throw new NotImplementedException();
        }
    }
}
