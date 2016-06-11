using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AccountTransactionUploadApp.Interfaces;
using AccountTransactionUploadApp.Models;

namespace AccountTransactionUploadApp.Implementation
{
    /// <summary>
    /// Class to read the File Data.
    /// </summary>
    public class FileDataReader : IFileDataReader
    {
        #region Private Fields

        private readonly int rowsToBeProcessed;
        private const string DATATABLE_ID_COLUMN_NAME = "Id";
        private const string DATATABLE_ACCOUNT_COLUMN_NAME = "Account";
        private const string DATATABLE_DESCRIPTION_COLUMN_NAME = "AccountDescription";
        private const string DATATABLE_CURRENCYCODE_COLUMN_NAME = "CurrencyCode";
        private const string DATATABLE_CURRENCYVALUE_COLUMN_NAME = "CurrencyValue";
        private const int ACCOUNT_INDEX = 0;
        private const int DESCRIPTION_INDEX = 1;
        private const int CURRENCYCODE_INDEX = 2;
        private const int CURRENCYVALUE_INDEX = 3;
        private readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        #endregion

        #region Constructor

        /// <summary>
        /// Initializes an instance of the FileDataReader class.
        /// </summary>
        public FileDataReader()
        {
            rowsToBeProcessed = 5;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Method to create the Data Table
        /// </summary>
        /// <returns>Returns a Data table Instance.</returns>
        private DataTable CreateDataTable()
        {
            var dataTable = new DataTable("AccountTransactionData");
            dataTable.Columns.Add(DATATABLE_ID_COLUMN_NAME, typeof(Guid));
            dataTable.Columns.Add(DATATABLE_ACCOUNT_COLUMN_NAME, typeof(string));
            dataTable.Columns.Add(DATATABLE_DESCRIPTION_COLUMN_NAME, typeof(string));
            dataTable.Columns.Add(DATATABLE_CURRENCYCODE_COLUMN_NAME, typeof(string));
            dataTable.Columns.Add(DATATABLE_CURRENCYVALUE_COLUMN_NAME, typeof(decimal));
            return dataTable;
        }

        /// <summary>
        /// Method to Process the Row.
        /// </summary>
        /// <param name="data">Data is a row.</param>
        /// <param name="dataTable">Data table</param>
        /// <param name="lineNumber">Line Number for the data.</param>
        /// <param name="linesSkipped">List of Lines Skipped.</param>
        private bool ProcessRow(string[] data, DataTable dataTable, int lineNumber, IList<SkippedLines> linesSkipped)
        {
            DataRow newRow = dataTable.NewRow();
            bool isValid = true;

            if (data.Count() == dataTable.Columns.Count - 1)
            {
                for (int index = 0; index < dataTable.Columns.Count - 1; index++)
                {
                    if (index == ACCOUNT_INDEX)
                    {
                        if (string.IsNullOrWhiteSpace(data[index]))
                        {
                            isValid = AddToSkippedLines(lineNumber, linesSkipped, "Account Name cannot be Null or Empty ");
                            break;
                        }
                        else
                        {
                            newRow[DATATABLE_ACCOUNT_COLUMN_NAME] = data[index];
                        }
                    }
                    else if (index == DESCRIPTION_INDEX)
                    {
                        if (string.IsNullOrWhiteSpace(data[index]))
                        {
                            isValid = AddToSkippedLines(lineNumber, linesSkipped, "Account Description cannot be Null or Empty ");
                            break;
                        }
                        else
                        {
                            newRow[DATATABLE_DESCRIPTION_COLUMN_NAME] = data[index];
                        }
                    }
                    else if (index == CURRENCYCODE_INDEX)
                    {
                        if (!ValidateCurrencyCode(data[index]))
                        {
                            isValid = AddToSkippedLines(lineNumber, linesSkipped, "Curency Code is not valid as per ISO 4217");
                            break;
                        }
                        else
                        {
                            newRow[DATATABLE_CURRENCYCODE_COLUMN_NAME] = data[index];
                        }
                    }
                    else if (index == CURRENCYVALUE_INDEX)
                    {
                        int value;
                        if (int.TryParse(data[index], out value))
                        {
                            newRow[DATATABLE_CURRENCYVALUE_COLUMN_NAME] = data[index];
                        }
                        else
                        {
                            isValid = AddToSkippedLines(lineNumber, linesSkipped, "Curency Value is not a valid Number");
                            break;
                        }
                    }
                }

                if (isValid)
                {
                    newRow[DATATABLE_ID_COLUMN_NAME] = Guid.NewGuid();
                    dataTable.Rows.Add(newRow);
                }

            }
            else
            {
                linesSkipped.Add(new SkippedLines { LineNumber = lineNumber, Reason = "Data is the row is not valid and has more than 4 columns" });
            }

            return isValid;
        }

        /// <summary>
        /// Method to Add the Skipped Lines Informtion.
        /// </summary>
        /// <param name="lineNumber">Line Number which is skipped.</param>
        /// <param name="linesSkipped">Line Skipped Information.</param>
        /// <param name="reason">Reason for skipping the line.</param>
        /// <returns>returns false as validation failed.</returns>
        private bool AddToSkippedLines(int lineNumber, IList<SkippedLines> linesSkipped, string reason)
        {
            linesSkipped.Add(new SkippedLines { LineNumber = lineNumber, Reason = reason });
            return false;
        }

        /// <summary>
        /// Method to validate the Currency Code.
        /// </summary>
        /// <param name="code">Currency Code to be validated.</param>
        /// <returns>Returns true or false based on validation result.</returns>
        private bool ValidateCurrencyCode(string code)
        {
            if (!string.IsNullOrWhiteSpace(code))
            {
                IEnumerable<string> currencySymbols = CultureInfo.GetCultures(CultureTypes.SpecificCultures) //Only specific cultures contain region information
                                                     .Select(x => (new RegionInfo(x.LCID)).ISOCurrencySymbol)
                                                     .Distinct();

                return currencySymbols != null && currencySymbols.Contains(code) ? true : false;
            }
            else return false;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Method to read the Data from CSV file.
        /// </summary>
        /// <param name="filePath">File path.</param>
        /// <returns>Returns an enumeration of the processed data.</returns>
        public IEnumerable<ProcessedLines> ReadDataFromCSV(string filePath)
        {
            IList<SkippedLines> skippedLines = new List<SkippedLines>();
            int rowToBeProcessedOneTime = 0;
            bool firstLineInTheRowsToBeProcessed = true;
            int lineCount = 0;
            long bytesRead = 0;
            int linesImported = 0;
            DataTable taxDataTable = null;

            //Checking whether the File Exists
            if (File.Exists(filePath))
            {
                using (var sr = new StreamReader(filePath))
                {
                    //Getting the total bytes which will be used to calculate the progress on the UI.
                    long totalBytes = sr.BaseStream.Length;
                    string line = null;
                    while ((line = sr.ReadLine()) != null)
                    {
                        //Bytes read which will be used to calculate the progress on the UI.
                        bytesRead = bytesRead += Encoding.ASCII.GetBytes(line).Length;
                        if (firstLineInTheRowsToBeProcessed)
                        {
                            taxDataTable = CreateDataTable();
                            firstLineInTheRowsToBeProcessed = false;
                        }

                        lineCount++;
                        //Process the row for validations and adding to Data Table.
                        if (ProcessRow(line.Split(new[] { ',' }), taxDataTable, lineCount, skippedLines))
                            linesImported++;
                        rowToBeProcessedOneTime++;

                        //We are processing 5 rows and then sending the data back.
                        if (rowsToBeProcessed == rowToBeProcessedOneTime)
                        {
                            rowToBeProcessedOneTime = 0;
                            firstLineInTheRowsToBeProcessed = true;
                            yield return new ProcessedLines() { LinesImported = linesImported, LinesProcessed = lineCount, DataTable = taxDataTable, SkippedLines = skippedLines, TotalBytes = totalBytes, BytesRead = bytesRead };
                            skippedLines = new List<SkippedLines>();
                            taxDataTable = null;
                        }
                    }
                }
                if (null != taxDataTable)
                    yield return new ProcessedLines() { LinesProcessed = lineCount, DataTable = taxDataTable, SkippedLines = skippedLines, LinesImported = linesImported};
            }
            else
            {
                throw new FileNotFoundException("File does not exist");
            }
        }


        /// <summary>
        /// Method to read the Data from Excel file.
        /// </summary>
        /// <param name="filePath">File Path.</param>
        /// <returns>Returns the Processed Data.</returns>
        public IEnumerable<ProcessedLines> ReadDataFromExcel(string filePath)
        {
            //We can implementation logic of reading from Excel File.
            return null;
        }
        #endregion
    }
}
