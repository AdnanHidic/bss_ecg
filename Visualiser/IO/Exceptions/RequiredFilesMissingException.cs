using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Visualiser.IO.Exceptions
{
    public class RequiredFilesMissingException : Exception
    {
        public enum RequiredFiles { DAT, HEA, ATR };

        public List<RequiredFiles> MissingFiles { get; set; }

        public RequiredFilesMissingException(List<RequiredFiles> missingFiles)
        {
            MissingFiles = new List<RequiredFiles>(missingFiles);
        }
    }
}
