using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace MvvmHelper
{
    abstract public class BindableBase : INotifyPropertyChanged, INotifyDataErrorInfo
    {
        protected BindableBase() { }

        #region INotifyDataErrorInfo
        /// <summary>
        /// 検証エラーがあるプロパティ名と検証エラーメッセージのリスト
        /// </summary>
        private Dictionary<string, string> hasErrors = new Dictionary<string, string>();

        public bool HasErrors { get { return hasErrors.Count > 0; } }

        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        /// <summary>
        /// プロパティの検証後に呼び出す
        /// errorMessageがnullまたは空の文字列でエラーなしまたはエラー状態解除、空白文字列はエラー状態
        /// </summary>
        /// <param name="errorMessage"></param>
        /// <param name="propertyName"></param>
        protected virtual void RaiseErrorsChanged(string errorMessage, [CallerMemberName] string propertyName = null)
        {
            if (!string.IsNullOrEmpty(errorMessage))
            {
                hasErrors[propertyName] = errorMessage;
            }
            else if (hasErrors.Any(x => x.Key == propertyName))
            {
                hasErrors.Remove(propertyName);
            }
            foreach (var hasError in hasErrors)
            {
                ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(hasError.Key));
            }
        }

        public IEnumerable GetErrors(string propertyName)
        {
            return hasErrors[propertyName];
        }
        #endregion

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] String propertyName = null)
        {
            if (object.Equals(storage, value))
                return false;
            storage = value;
            this.RaisePropertyChanged(propertyName);
            return true;
        }
        #endregion
    }
}
