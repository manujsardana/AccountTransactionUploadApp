using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace AccountTransactionUploadApp.Commands
{
    /// <summary>
    /// Class for Delegate Command which we can use for any command like Button Click event.
    /// </summary>
    public class DelegateCommand : ICommand
    {
        private readonly Predicate<object> _canExecute;
        private readonly Action<object> _executeCommand;

        public event EventHandler CanExecuteChanged;

        #region Constructor

        public DelegateCommand(Action<object> execute)
            : this(execute, null)
        {

        }

        public DelegateCommand(Action<object> execute,
                   Predicate<object> canExecute)
        {
            _executeCommand = execute;
            _canExecute = canExecute;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Method which gets called when Command is Executed.
        /// </summary>
        /// <param name="parameter">Parameter.</param>
        public void Execute(object parameter)
        {
            _executeCommand(parameter);
        }

        /// <summary>
        /// Method which determines whether Command can be executed or not.
        /// </summary>
        /// <param name="parameter">Parameter.</param>
        /// <returns>Returns a bool.</returns>
        public bool CanExecute(object parameter)
        {
            if (_canExecute == null)
            {
                return true;
            }

            return _canExecute(parameter);
        }

        /// <summary>
        /// Event which gets raised when the Can Execute changes.
        /// </summary>
        public void RaiseCanExecuteChanged()
        {
            if (CanExecuteChanged != null)
            {
                CanExecuteChanged(this, EventArgs.Empty);
            }
        }

        #endregion
    }
}
