using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountTransactionUploadApp.Models
{
    /// <summary>
    /// Class for Lines which are Skipped.
    /// </summary>
    public class SkippedLines
    {
        /// <summary>
        /// Line Number for the Skipped Lines.
        /// </summary>
        public int LineNumber { get; set; }

        /// <summary>
        /// Property for Reason of why the line was skipped.
        /// </summary>
        public string Reason { get; set; }
    }

}
