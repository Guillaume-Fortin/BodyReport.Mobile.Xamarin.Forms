using BodyReportMobile.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace BodyReportMobile.Core.Framework
{
    /// <summary>
    /// take on https://github.com/xamarin/Xamarin.Forms/blob/master/Xamarin.Forms.Core/Command.cs
    /// </summary>
    public class ViewModelCommand : ICommand
    {
        private BaseViewModel _viewModel;
        readonly Func<object, bool> _canExecute;
        readonly Action<object> _execute;

        public ViewModelCommand(BaseViewModel viewModel, Action<object> execute)
        {
            if (execute == null)
                throw new ArgumentNullException("execute");

            _execute = execute;
            _viewModel = viewModel;
        }

        public ViewModelCommand(BaseViewModel viewModel, Action execute) : this(viewModel, o => execute())
		{
            if (execute == null)
                throw new ArgumentNullException("execute");
        }

        public ViewModelCommand(BaseViewModel viewModel, Action<object> execute, Func<object, bool> canExecute) : this(viewModel, execute)
		{
            if (canExecute == null)
                throw new ArgumentNullException("canExecute");

            _canExecute = canExecute;
            _viewModel = viewModel;
        }

        public ViewModelCommand(BaseViewModel viewModel, Action execute, Func<bool> canExecute) : this(viewModel, o => execute(), o => canExecute())
		{
            if (execute == null)
                throw new ArgumentNullException("execute");
            if (canExecute == null)
                throw new ArgumentNullException("canExecute");
        }

        public bool CanExecute(object parameter)
        {
            if (_canExecute != null)
                return _canExecute(parameter);

            return true;
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            if (_viewModel != null && _viewModel.ActionIsInProgress)
                return;
            try
            {
                _viewModel.ActionIsInProgress = true;
                _execute(parameter);
            }
            finally
            {
                _viewModel.ActionIsInProgress = false;
            }
        }

        public void ChangeCanExecute()
        {
            EventHandler changed = CanExecuteChanged;
            if (changed != null)
                changed(this, EventArgs.Empty);
        }
    }
}
