using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountTransactionUploadApp.Models
{
    /// <summary>
    /// Class for Lines which are processed.
    /// </summary>
    public class ProcessedLines
    {
        /// <summary>
        /// Property for Data Table which is inserted to SQL using SQLBulkCopy.
        /// </summary>
        public System.Data.DataTable DataTable { get; set; }

        /// <summary>
        /// Property for Number of Lines which are processed.
        /// </summary>
        public int LinesProcessed { get; set; }

        /// <summary>
        /// List of lines which are skipped which has the details of why they were Skipped.
        /// </summary>
        public IList<SkippedLines> SkippedLines { get; set; }

        /// <summary>
        /// Property for Total Bytes of the File which is used to calculate the Percentage.
        /// </summary>
        public long TotalBytes { get; set; }

        /// <summary>
        /// Property for Bytes which are read used to calculate the Percentage.
        /// </summary>
        public long BytesRead { get; set; }

        /// <summary>
        /// Property for Lines Imported to DB.
        /// </summary>
        public int LinesImported { get; set; }

    }
}
