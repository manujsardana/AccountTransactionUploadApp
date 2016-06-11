using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using AccountTransactionUploadApp.Commands;
using AccountTransactionUploadApp.Implementation;
using AccountTransactionUploadApp.Interfaces;
using AccountTransactionUploadApp.Models;
using log4net;

namespace AccountTransactionUploadApp.ViewModels
{
    /// <summary>
    /// View Model for the View for all the Binding and Processing.
    /// </summary>
    public class FileDataUploaderViewModel : INotifyPropertyChanged
    {
        #region Private Fields

        private BackgroundWorker _fileUploaderBackgroundWorker;
        private readonly IOpenFileDialogService _openFileDialogService;
        private readonly IConfigurationManager _configManager;
        private readonly IMessageBoxService _messageBoxService;
        private readonly ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Public Properties

        /// <summary>
        /// Command for Upload File which fires when the Upload button is clicked.
        /// </summary>
        private ICommand _uploadFileCommand;
        public ICommand UploadFileCommand
        {
            get
            {
                if (_uploadFileCommand == null)
                    _uploadFileCommand = new DelegateCommand(OnUploadButtonClick);

                return _uploadFileCommand;
            }
            set
            {
                _uploadFileCommand = value;
            }
        }

        /// <summary>
        /// Property for Selected File Name.
        /// </summary>
        private string _selectedFileName;
        public string SelectedFileName
        {
            get
            {
                return _selectedFileName;
            }
            set
            {
                if(_selectedFileName != value)
                {
                    _selectedFileName = value;
                    OnPropertyChanged("SelectedFileName");
                }
            }
        }

        /// <summary>
        /// Property for Progress Percentage which shows the Progress on the Progress Bar.
        /// </summary>
        private int _progressPercentage;
        public int ProgressPercentage
        {
            get
            {
                return _progressPercentage;
            }
            set
            {
                if (_progressPercentage != value)
                {
                    _progressPercentage = value;
                    OnPropertyChanged("ProgressPercentage");
                }
            }
        }

        /// <summary>
        /// Property for the Lines Processed Message which shows the Number of Lines Processed.
        /// </summary>
        private string _linesProcessedMessage;
        public string LinesProcessedMessage
        {
            get
            {
                return _linesProcessedMessage;
            }
            set
            {
                if(_linesProcessedMessage != value)
                {
                    _linesProcessedMessage = value;
                    OnPropertyChanged("LinesProcessedMessage");
                }
            }
        }

        /// <summary>
        /// Property for the Skipped Lines Message which shows the Lines Skipped and their details.
        /// </summary>
        private string _skippedLinesMessage;
        public string SkippedLinesMessage
        {
            get
            {
                return _skippedLinesMessage;
            }
            set
            {
                if(_skippedLinesMessage != value)
                {
                    _skippedLinesMessage = value;
                    OnPropertyChanged("SkippedLinesMessage");
                }
            }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes an instance of the FileDataUploaderViewModel class.
        /// </summary>
        /// <param name="openFileDialogService">File Dialog Service instance.</param>
        /// <param name="messageBoxService">Message Box Service instance.</param>
        /// <param name="configManager">Config Manager Service instance.</param>
        public FileDataUploaderViewModel(IOpenFileDialogService openFileDialogService, IMessageBoxService messageBoxService, IConfigurationManager configManager)
        {
            _openFileDialogService = openFileDialogService;
            _messageBoxService = messageBoxService;
            _configManager = configManager;
            InitializeBackgroundWorker();
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Method to Initialize the Background Worker.
        /// </summary>
        private void InitializeBackgroundWorker()
        {
            _fileUploaderBackgroundWorker = new BackgroundWorker();
            _fileUploaderBackgroundWorker.WorkerReportsProgress = true;
            _fileUploaderBackgroundWorker.WorkerSupportsCancellation = true;
            _fileUploaderBackgroundWorker.DoWork += UploadFile;
            _fileUploaderBackgroundWorker.ProgressChanged += ProgressChanged;
            _fileUploaderBackgroundWorker.RunWorkerCompleted += FileUploadCompleted;
            logger.Info("Background Worker Initialized");
        }

        /// <summary>
        /// Method which executes when the Upload Button is clicked.
        /// </summary>
        /// <param name="input">Input to the method.</param>
        private void OnUploadButtonClick(object input)
        {
            _openFileDialogService.ShowFileDialog();
            if (string.IsNullOrEmpty(_openFileDialogService.SelectedFileName))
            {
                logger.Error("Selected File name null or Empty");
                _messageBoxService.ShowMessageBox("Please select a valid File to Upload", "TaxCalculatorApp");
            }
            else
            {
                SelectedFileName = _openFileDialogService.SelectedFileName;
                if (_fileUploaderBackgroundWorker.IsBusy)
                {
                    logger.Info("Background Worker already running");
                    MessageBox.Show("File Upload in Progress");
                }
                else
                {
                    _fileUploaderBackgroundWorker.RunWorkerAsync();
                }
            }
        }

        /// <summary>
        /// Method to Process the Skipped Lines and create a message to Display on the UI.
        /// </summary>
        /// <param name="skippedLines">List of Skipped Lines.</param>
        /// <returns>Returns a message to display on the UI.</returns>
        private string ProcessSkippedLines(IList<SkippedLines> skippedLines)
        {
            StringBuilder strBuilder = new StringBuilder();
            foreach(var line in skippedLines)
            {
                strBuilder.Append(string.Format("Line {0} skipped because of Reason - {1}", line.LineNumber, line.Reason));
                strBuilder.AppendLine();
            }

            if (strBuilder.ToString() != string.Empty)
            {
                return _skippedLinesMessage + strBuilder.ToString();
            }
            else
                return _skippedLinesMessage;
        }

        /// <summary>
        /// Event which is raised when the Background Worker is started.
        /// </summary>
        /// <param name="sender">Sender object for the event.</param>
        /// <param name="args">Event args for the event.</param>
        private void UploadFile(object sender, DoWorkEventArgs args)
        {
            string connectionString = _configManager.GetConnectionString();
            if (!string.IsNullOrEmpty(connectionString))
            {
                IFileDataReader fileReader = new FileDataReader();
                IFileDataWriter fileWriter = new FileDataWriter(connectionString);
                System.Diagnostics.Stopwatch stopWatch = System.Diagnostics.Stopwatch.StartNew();
                string fileExtension = System.IO.Path.GetExtension(SelectedFileName);
                if (fileExtension == ".csv")
                {
                    foreach (var v in fileReader.ReadDataFromCSV(SelectedFileName))
                    {
                        fileWriter.WriteDataToSQL("AccountTransactionData", v.DataTable, GetColumnMappings());
                        _fileUploaderBackgroundWorker.ReportProgress((int)CalculatePercentage(v.TotalBytes, v.BytesRead), string.Format("{0} Lines Processed, {1} Lines Imported", v.LinesProcessed, v.LinesImported));
                        SkippedLinesMessage = ProcessSkippedLines(v.SkippedLines);
                    }
                    logger.Info(string.Format("Processing of File Completed in {0} seconds", stopWatch.Elapsed.Seconds));
                }
                else if(fileExtension == ".xlsx")
                {
                    //Call the ReadDataFromExcel method.
                }
            }
            else
            {
                logger.Error("Connection String not valid in the config file");
                _messageBoxService.ShowMessageBox("Please check the Connection string in Configuration file", "AccountTransactionUploadApp");
            }
        }

        /// <summary>
        /// Method to get the Column mappings for use in SqlBulkCopy.
        /// </summary>
        /// <returns>Returns a list of Column mappings.</returns>
        private IList<KeyValuePair<string, string>> GetColumnMappings()
        {
            return new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("ID", "ID"),
                new KeyValuePair<string, string>("Account", "Account"),
                new KeyValuePair<string, string>("AccountDescription", "AccountDescription"),
                new KeyValuePair<string, string>("CurrencyCode", "CurrencyCode"),
                new KeyValuePair<string, string>("CurrencyValue", "CurrencyValue")
            };
        }

        /// <summary>
        /// Method to Get the Percentage based on the total bytes and the number of bytes read.
        /// </summary>
        /// <param name="totalBytes">Total bytes in the file.</param>
        /// <param name="bytesRead">Number of Bytes read.</param>
        /// <returns>Returns the Percentage as double.</returns>
        private double CalculatePercentage(long totalBytes, long bytesRead)
        {
            return ((double)bytesRead / (double)totalBytes) * 100;
        }

        /// <summary>
        /// Progress Changed event called from the Backgound worker.
        /// </summary>
        /// <param name="sender">Sender object for the event.</param>
        /// <param name="e">Event args for the event.</param>
        void ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if(e != null && e.UserState != null)
            {
                LinesProcessedMessage = e.UserState as string;
                ProgressPercentage = e.ProgressPercentage;
            }
        }

        /// <summary>
        /// Method which fires when the Backgound worker completes doing work.
        /// </summary>
        /// <param name="sender">Sender object for the event.</param>
        /// <param name="e">Event args for the event.</param>
        void FileUploadCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            ProgressPercentage = 100;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Property Changed event to notify the UI.
        /// </summary>
        /// <param name="propertyName">Property Name for which to raise the event.</param>
        public void OnPropertyChanged(string propertyName)
        {
            if(PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion
    }
}
