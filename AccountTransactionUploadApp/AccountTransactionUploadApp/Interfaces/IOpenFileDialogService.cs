using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountTransactionUploadApp.Interfaces
{
    /// <summary>
    /// Interface to Show the File Dialog.
    /// </summary>
    public interface IOpenFileDialogService
    {
        /// <summary>
        /// Method to Show the File Dialog.
        /// </summary>
        void ShowFileDialog();

        /// <summary>
        /// Property for the Selected File Name in the File Dialog.
        /// </summary>
        string SelectedFileName { get; }
    }
}
