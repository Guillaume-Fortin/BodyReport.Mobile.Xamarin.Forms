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
    /// and on https://gist.githubusercontent.com/dimitrispaxinos/f051d67a287bb34947a5/raw/60f104e96dc22a8e0d56c5d22122d53a8228f428/RelayCommandAsync.cs
    /// </summary>
    public class ViewModelCommandAsync : ICommand
    {
        private BaseViewModel _viewModel;
        readonly Func<object, bool> _canExecute;
        readonly Func<object, Task> _asyncExecute;

        public ViewModelCommandAsync(BaseViewModel viewModel, Func<object, Task> asyncExecute)
        {
            if (asyncExecute == null)
                throw new ArgumentNullException("execute");

            _asyncExecute = asyncExecute;
            _viewModel = viewModel;
        }

        public ViewModelCommandAsync(BaseViewModel viewModel, Func<Task> asyncExecute) : this(viewModel, o => asyncExecute())
        {
            if (asyncExecute == null)
                throw new ArgumentNullException("asyncExecute");
        }

        public ViewModelCommandAsync(BaseViewModel viewModel, Func<object, Task> asyncExecute, Func<object, bool> canExecute) : this(viewModel, asyncExecute)
        {
            if (canExecute == null)
                throw new ArgumentNullException("canExecute");

            _canExecute = canExecute;
            _viewModel = viewModel;
        }

        public ViewModelCommandAsync(BaseViewModel viewModel, Func<Task> asyncExecute, Func<bool> canExecute) : this(viewModel, o => asyncExecute(), o => canExecute())
        {
            if (asyncExecute == null)
                throw new ArgumentNullException("asyncExecute");
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

        public async void Execute(object parameter)
        {
            await ExecuteAsync(parameter);
        }

        protected async Task ExecuteAsync(object parameter)
        {
            if (_viewModel != null && _viewModel.ActionIsInProgress)
                return;
            try
            {
                if (_viewModel != null)
                    _viewModel.ActionIsInProgress = true;
                await _asyncExecute(parameter);
            }
            finally
            {
                if (_viewModel != null)
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
