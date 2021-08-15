using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Chat.Client.Commands
{
    public class RelayCommandAsync : ICommand
    {
        private readonly Func<object, Task> _execute;
        private readonly Predicate<object> _canExecute;

        private bool _isExecuting;

        public RelayCommandAsync(Func<object, Task> execute, Predicate<object> canExecute = null)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        public event EventHandler CanExecuteChanged 
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public bool CanExecute(object parameter) =>
            _canExecute is null ? !_isExecuting : !_isExecuting && _canExecute(parameter);

        public async void Execute(object parameter)
        {
            try
            {
                _isExecuting = true;

                await _execute(parameter);
            }
            finally 
            {
                _isExecuting = false;
            }
        }
    }
}
