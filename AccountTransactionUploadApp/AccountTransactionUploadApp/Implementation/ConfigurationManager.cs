using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AccountTransactionUploadApp.Interfaces;
using log4net;

namespace AccountTransactionUploadApp.Implementation
{
    /// <summary>
    /// Class for Config Manager to read the data from Config File.
    /// </summary>
    public class ConfigurationManager : IConfigurationManager
    {
        private readonly string CONNECTIONSTRING = "Connection";
        private readonly ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Method to Get the Connection string from the Config file.
        /// </summary>
        /// <returns>Returns the Conenction string.</returns>
        public string GetConnectionString()
        {
            string connectionString = string.Empty;
            try
            {
                connectionString = System.Configuration.ConfigurationManager.AppSettings[CONNECTIONSTRING];
            }
            catch (ConfigurationErrorsException ex)
            {
                logger.Error(string.Format("Exception occurred while reading the Config file with Message {0}", ex.Message));
            }

            return connectionString;
        }
    }
}
