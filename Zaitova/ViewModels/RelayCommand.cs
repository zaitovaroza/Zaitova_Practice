using System.Windows.Input;

namespace Zaitova.ViewModels
{
    public class RelayCommand : ICommand
    {
        private readonly Action _execute;
        private readonly Func<bool>? _canExecute;

        public RelayCommand(Action execute, Func<bool>? canExecute = null)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        public event EventHandler? CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object? parameter) => _canExecute?.Invoke() ?? true;

        public void Execute(object? parameter) => _execute();
    }

    public class RelayCommand<T> : ICommand
    {
        private readonly Action<T?> _execute;
        private readonly Func<T?, bool>? _canExecute;

        public RelayCommand(Action<T?> execute, Func<T?, bool>? canExecute = null)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        public event EventHandler? CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object? parameter)
        {
            if (parameter == null && typeof(T).IsValueType)
                return _canExecute?.Invoke(default) ?? true;

            if (parameter is T typedParameter)
                return _canExecute?.Invoke(typedParameter) ?? true;

            return _canExecute?.Invoke(default) ?? true;
        }

        public void Execute(object? parameter)
        {
            if (parameter == null && typeof(T).IsValueType)
            {
                _execute(default);
                return;
            }

            if (parameter is T typedParameter)
            {
                _execute(typedParameter);
            }
            else
            {
                _execute(default);
            }
        }
    }
}