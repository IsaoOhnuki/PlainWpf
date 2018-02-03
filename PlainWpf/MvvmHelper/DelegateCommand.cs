using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MvvmHelper
{
    public class DelegateCommand : ICommand
    {
        System.Action execute;
        System.Func<bool> canExecute;

        public bool CanExecute(object parameter)
        {
            return canExecute();
        }

        public event System.EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public void Execute(object parameter)
        {
            execute();
        }

        public DelegateCommand(System.Action execute)
        {
            this.execute = execute;
            this.canExecute = () => true;
        }

        public DelegateCommand(System.Action execute, System.Func<bool> canExecute)
        {
            this.execute = execute;
            this.canExecute = canExecute;
        }
    }

    public class DelegateCommand<T> : ICommand
    {
        System.Action<T> execute;
        System.Func<T, bool> canExecute;

        public bool CanExecute(object parameter)
        {
            return canExecute((T)parameter);
        }

        public event System.EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public void Execute(object parameter)
        {
            execute((T)parameter);
        }

        public DelegateCommand(System.Action<T> execute)
        {
            this.execute = execute;
            this.canExecute = x => true;
        }

        public DelegateCommand(System.Action<T> execute, System.Func<T, bool> canExecute)
        {
            this.execute = execute;
            this.canExecute = canExecute;
        }
    }
}
