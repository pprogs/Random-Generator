using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace TestWPF
{
    class AsyncCommand : ICommand, INotifyPropertyChanged
    {
        public AsyncCommand(Func<Task> Action)
        {
            action = Action;
            canExecute = true;
        }

        public event EventHandler CanExecuteChanged;

        public event PropertyChangedEventHandler PropertyChanged;

        public bool CanExecute(object parameter)
        {
            return !IsExecuting && canExecute;
        }

        public async void Execute(object parameter)
        {
            if (IsExecuting)
                return;

            try
            {
                IsExecuting = true;
                await action();
            }

            catch
            {
                //
            }
            finally
            {
                IsExecuting = false;
            }
        }

        public bool IsExecuting
        {
            get { return isExecuting; }
            set
            {
                isExecuting = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IsExecuting"));
                OnCanExecuteChanged();
            }
        }

        private bool isExecuting;

        public void OverrideCanExecute(bool Value)
        {
            canExecute = Value;
            OnCanExecuteChanged();
        }

        private Func<Task> action;
        private bool canExecute;

        private void OnCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}