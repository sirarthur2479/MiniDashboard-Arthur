using System;
using System.Threading.Tasks;
using System.Windows.Input;

// Relay command for asynchronous operations in MVVM
namespace MiniDashboard.App.ViewModels {
    public class AsyncRelayCommand : ICommand {
        private readonly Func<Task> _execute;
        private bool _isExecuting;

        public AsyncRelayCommand(Func<Task> execute) {
            _execute = execute;
        }

        public bool CanExecute(object? parameter) => !_isExecuting;
        public event EventHandler? CanExecuteChanged;

        public async void Execute(object? parameter) {
            if (_isExecuting) return;
            _isExecuting = true;
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);

            try { await _execute(); }
            finally {
                _isExecuting = false;
                CanExecuteChanged?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}
