using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AccountTransactionUploadApp.Models;

namespace AccountTransactionUploadApp.Interfaces
{
    /// <summary>
    /// Class to read the File Data.
    /// </summary>
    public interface IFileDataReader
    {
        /// <summary>
        /// Method to read the Data from CSV file.
        /// </summary>
        /// <param name="filePath">File path.</param>
        /// <returns>Returns an enumeration of the processed data.</returns>
        IEnumerable<ProcessedLines> ReadDataFromCSV(string filePath);

        /// <summary>
        /// Method to read the Data from Excel file.
        /// </summary>
        /// <param name="filePath">File path.</param>
        /// <returns>Returns an enumeration of the processed data.</returns>
        IEnumerable<ProcessedLines> ReadDataFromExcel(string filePath);
    }
}
