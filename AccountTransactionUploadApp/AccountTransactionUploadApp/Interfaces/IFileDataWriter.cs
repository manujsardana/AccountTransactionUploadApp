using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountTransactionUploadApp.Interfaces
{
    /// <summary>
    /// Interface to Write the Data.
    /// </summary>
    public interface IFileDataWriter
    {
        /// <summary>
        /// Method to Write the Data to SQL using SQLBulkCopy.
        /// </summary>
        /// <param name="tableName">Table Name to which to write.</param>
        /// <param name="dataTable">Data table containing the data.</param>
        /// <param name="columnMappings">Column Mappings for the data.</param>
        void WriteDataToSQL(string tableName, System.Data.DataTable dataTable, IList<KeyValuePair<string, string>> columnMappings);
    }
}
