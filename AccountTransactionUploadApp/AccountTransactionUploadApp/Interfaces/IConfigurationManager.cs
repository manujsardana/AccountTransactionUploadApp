using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountTransactionUploadApp.Interfaces
{
    /// <summary>
    /// Interface for Config Manager to read the data from Config File.
    /// </summary>
    public interface IConfigurationManager
    {
        /// <summary>
        /// Method to Get the Connection string from the Config file.
        /// </summary>
        /// <returns>Returns the Conenction string.</returns>
        string GetConnectionString();
    }
}
