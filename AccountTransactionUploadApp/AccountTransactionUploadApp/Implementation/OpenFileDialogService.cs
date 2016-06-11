using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AccountTransactionUploadApp.Interfaces;

namespace AccountTransactionUploadApp.Implementation
{
    /// <summary>
    /// Interface to Show the File Dialog.
    /// </summary>
    public class OpenFileDialogService : IOpenFileDialogService
    {
        /// <summary>
        /// Property for Selected File Name in File Dialog.
        /// </summary>
        public string SelectedFileName
        {
            get;
            private set;
        }

        /// <summary>
        /// Method to Show the File Dialog.
        /// </summary>
        public void ShowFileDialog()
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.DefaultExt = ".csv";
            fileDialog.Multiselect = false;
            fileDialog.Filter = "CSV File (*.csv)|*.csv";
            bool? showDialog = fileDialog.ShowDialog();
            if (showDialog.HasValue && showDialog.Value)
            {
                if (string.IsNullOrEmpty(fileDialog.FileName) || !fileDialog.FileName.Contains(".csv"))
                {
                    SelectedFileName = string.Empty;
                }
                else
                {
                    SelectedFileName = fileDialog.FileName;
                }
            }
            else
            {
                SelectedFileName = string.Empty;
            }
        }
    }
}
