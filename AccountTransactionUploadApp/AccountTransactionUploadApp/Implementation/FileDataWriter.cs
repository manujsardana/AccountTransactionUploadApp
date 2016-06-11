using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AccountTransactionUploadApp.Interfaces;

namespace AccountTransactionUploadApp.Implementation
{
    /// <summary>
    /// Class to Write the Data.
    /// </summary>
    public class FileDataWriter : IFileDataWriter
    {
        private readonly string _connectionString;

        #region Constructor

        /// <summary>
        /// Initializes an instance of the FileDatWriter class.
        /// </summary>
        /// <param name="connectionString">Connection string</param>
        public FileDataWriter(string connectionString)
        {
            _connectionString = connectionString;
        }

        #endregion

        /// <summary>
        /// Method to Write the Data to SQL using SQLBulkCopy.
        /// </summary>
        /// <param name="tableName">Table Name to which to write.</param>
        /// <param name="dataTable">Data table containing the data.</param>
        /// <param name="columnMappings">Column Mappings for the data.</param>
        public void WriteDataToSQL(string tableName, System.Data.DataTable dataTable, IList<KeyValuePair<string, string>> columnMappings)
        {
            using (var bulkCopy = new SqlBulkCopy(_connectionString, SqlBulkCopyOptions.Default))
            {
                bulkCopy.DestinationTableName = tableName;
                bulkCopy.BulkCopyTimeout = 0;
                foreach (KeyValuePair<string, string> map in columnMappings)
                {
                    bulkCopy.ColumnMappings.Add(map.Key, map.Value);
                }
                bulkCopy.WriteToServer(dataTable, DataRowState.Added);
            }
        }
    }
}
