using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeesManagementApp.Helpers
{
    using EmployeesManagement.Service.DTO;
    using EmployeesManagementApp.Models;
    using System;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using System.Threading.Tasks;

    public class AsyncCommand<TResultDto, TResult> : AsyncCommandBase, INotifyPropertyChanged
        where TResultDto : IDto
        where TResult : ICastable<TResult, TResultDto>, new()
    {
        private readonly Func<Task<TResultDto>> _command;
        private NotifyTaskCompletion<TResultDto, TResult>? _execution = null;
        private TResult? _savedResult = new();
        public event PropertyChangedEventHandler? PropertyChanged;

        public TResult? Result { 
            get => _savedResult;
            private set { 
                _savedResult = value;
                OnPropertyChanged();
            }
        }
        public NotifyTaskCompletion<TResultDto, TResult>? Execution
        {
            get { return _execution; }
            private set
            {
                _execution = value;
                OnPropertyChanged();
            }
        }
        public AsyncCommand(Func<Task<TResultDto>> command)
        {
            _command = command;
        }

        public override bool CanExecute(object? parameter)
        {
            return Execution == null || Execution.IsCompleted;
        }

        public override async Task ExecuteAsync(object? parameter)
        {
            Execution = new NotifyTaskCompletion<TResultDto, TResult>(_command());
            RaiseCanExecuteChanged();
            await Execution.TaskCompletion;
            RaiseCanExecuteChanged();
            Result = Execution.Result;
        }


        protected virtual void OnPropertyChanged(string? propertyName = null)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public static class AsyncCommand
    {
        public static AsyncCommand<TResultDto, TResult> Create<TResultDto, TResult>(Func<Task<TResultDto>> command)
            where TResultDto : IDto
            where TResult : ICastable<TResult, TResultDto>, new()
        {
            return new AsyncCommand<TResultDto, TResult>(command);
        }
    }
}
