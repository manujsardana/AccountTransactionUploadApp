using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using AccountTransactionUploadApp.Implementation;
using AccountTransactionUploadApp.Interfaces;
using AccountTransactionUploadApp.ViewModels;
using AccountTransactionUploadApp.Views;
using log4net;

namespace AccountTransactionUploadApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            log4net.Config.XmlConfigurator.Configure();
            IOpenFileDialogService openFileDialogService = new OpenFileDialogService();
            IMessageBoxService messageBoxService = new MessageBoxService();
            IConfigurationManager configManager = new AccountTransactionUploadApp.Implementation.ConfigurationManager();
            FileDataUploaderViewModel viewModel = new FileDataUploaderViewModel(openFileDialogService, messageBoxService, configManager);
            FileDataUploader fileDataUploader = new FileDataUploader();
            fileDataUploader.DataContext = viewModel;
            fileDataUploader.Show();
        }
    }
}
