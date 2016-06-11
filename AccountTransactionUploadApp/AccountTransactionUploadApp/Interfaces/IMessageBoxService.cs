using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountTransactionUploadApp.Interfaces
{
    /// <summary>
    /// Interface for Message Box Service used to Show the Message Box.
    /// </summary>
    public interface IMessageBoxService
    {
        /// <summary>
        /// Method to Show the Message Box.
        /// </summary>
        /// <param name="messageBoxText">Message Box Text.</param>
        /// <param name="caption">Message Box Caption.</param>
        void ShowMessageBox(string messageBoxText, string caption);
    }
}
