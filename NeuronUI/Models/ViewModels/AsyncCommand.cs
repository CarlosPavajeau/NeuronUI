﻿using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace NeuronUI.Models.ViewModels
{
    public class AsyncCommand : ICommand
    {
        private readonly Func<Task> _execute;
        private readonly Func<object, Task> _executewithParams;
        private readonly Func<bool> _canExecute;
        private bool _isExecuting;

        public AsyncCommand(Func<Task> execute) : this(execute, () => true)
        {
        }

        public AsyncCommand(Func<object, Task> execute) : this(execute, () => true)
        {
        }

        public AsyncCommand(Func<Task> execute, Func<bool> canExecute)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        public AsyncCommand(Func<object, Task> execute, Func<bool> canExecute)
        {
            _executewithParams = execute;
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            return !(_isExecuting && _canExecute());
        }

        public event EventHandler CanExecuteChanged;

        public async void Execute(object parameter)
        {
            _isExecuting = true;
            OnCanExecuteChanged();
            try
            {
                if (parameter != null && _executewithParams != null)
                {
                    await _executewithParams(parameter);
                }
                else
                {
                    await _execute();
                }
            }
            finally
            {
                _isExecuting = false;
                OnCanExecuteChanged();
            }
        }

        protected virtual void OnCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, new EventArgs());
        }
    }
}