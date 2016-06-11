using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using AccountTransactionUploadApp.Interfaces;

namespace AccountTransactionUploadApp.Implementation
{
    /// <summary>
    /// Class for Message Box Service used to Show the Message Box.
    /// </summary>
    public class MessageBoxService : IMessageBoxService
    {
        /// <summary>
        /// Method to Show the Message Box.
        /// </summary>
        /// <param name="messageBoxText">Message Box Text.</param>
        /// <param name="caption">Message Box Caption.</param>
        public void ShowMessageBox(string messageBoxText, string caption)
        {
            MessageBox.Show(messageBoxText, caption);
        }
    }
}
