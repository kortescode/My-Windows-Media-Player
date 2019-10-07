using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace MyWindowsMediaPlayer.ViewModel
{
    public class DelegateCommand : ICommand
    {
        #region Fields
        private Func<object, bool> canExecute;
        private Action<object> executeAction;
        private bool canExecuteCache;
        #endregion //!Fields

        #region Constructors
        public DelegateCommand(Action<object> executeAction, Func<object, bool> canExecute)
        {
            this.executeAction = executeAction;
            this.canExecute = canExecute;
        }
        #endregion //!Constructors

        #region ICommand Members
        public bool CanExecute(object parameter)
        {
            bool tmp = canExecute(parameter);

            if (canExecuteCache != tmp)
            {
                canExecuteCache = tmp;
                if (CanExecuteChanged != null)
                {
                    CanExecuteChanged(this, new EventArgs());
                }
            }

            return canExecuteCache;
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            executeAction(parameter);
        }
        #endregion //ICommand Members
    }
}
