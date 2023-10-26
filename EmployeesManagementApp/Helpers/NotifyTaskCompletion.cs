using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EmployeesManagement.Service.DTO;
using EmployeesManagementApp.Models;

namespace EmployeesManagementApp.Helpers
{
    public sealed class NotifyTaskCompletion<TResultDto, TResult> : INotifyPropertyChanged
        where TResultDto : IDto
        where TResult : ICastable<TResult, TResultDto>, new()
    {
        public Task<TResultDto> Task { get; private set; }
        public Task TaskCompletion { get; private set; }
        public TResult? Result
        {
            get => Task.Status == TaskStatus.RanToCompletion ?
                new TResult().FromDto(Task.Result)
                : new TResult();
        }
        public TaskStatus Status { get => Task.Status; }
        public bool IsCompleted { get => Task.IsCompleted; }
        public bool IsNotCompleted { get => !Task.IsCompleted; }
        public bool IsSuccessfullyCompleted
        {
            get => Task.Status == TaskStatus.RanToCompletion;
        }
        public bool IsCanceled { get => Task.IsCanceled; }
        public bool IsFaulted { get => Task.IsFaulted; }
        public AggregateException? Exception { get => Task.Exception; }
        public Exception? InnerException
        {
            get => Exception == null ? null : Exception.InnerException;
        }
        public string? ErrorMessage
        {
            get => InnerException == null ? null : InnerException.Message;
        }
        public event PropertyChangedEventHandler? PropertyChanged;

        public NotifyTaskCompletion(Task<TResultDto> task)
        {
            Task = task;
            if (task.IsCompleted)
                TaskCompletion = System.Threading.Tasks.Task.FromResult(0);
            else
                TaskCompletion = WatchTaskAsync();
        }
        private async Task WatchTaskAsync()
        {
            try
            {
                await Task;
            }
            catch
            {
            }

            var propertyChanged = PropertyChanged;

            if (propertyChanged == null)
                return;

            propertyChanged(this, new PropertyChangedEventArgs(nameof(Status)));
            propertyChanged(this, new PropertyChangedEventArgs(nameof(IsCompleted)));
            propertyChanged(this, new PropertyChangedEventArgs(nameof(IsNotCompleted)));

            if (Task.IsCanceled)
            {
                propertyChanged(this, new PropertyChangedEventArgs(nameof(IsCanceled)));
            }
            else if (Task.IsFaulted)
            {
                propertyChanged(this, new PropertyChangedEventArgs(nameof(IsFaulted)));
                propertyChanged(this, new PropertyChangedEventArgs(nameof(Exception)));
                propertyChanged(this,
                  new PropertyChangedEventArgs(nameof(InnerException)));
                propertyChanged(this, new PropertyChangedEventArgs(nameof(ErrorMessage)));
            }
            else
            {
                propertyChanged(this,
                  new PropertyChangedEventArgs(nameof(IsSuccessfullyCompleted)));
                propertyChanged(this, new PropertyChangedEventArgs(nameof(Result)));
            }
        }
    }
}
