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
    /// <summary>
    /// ビューモデル等の基底に使うベースクラス
    /// </summary>
    /// <inherited name="INotifyPropertyChanged">プロパティ変更通知</inherited>
    /// <inherited name="INotifyDataErrorInfo">プロパティ検証エラー通知</inherited>
    abstract public class BindableBase : INotifyPropertyChanged, INotifyDataErrorInfo
    {
        protected BindableBase() { }

        #region INotifyDataErrorInfo Implement
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

        /// <summary>
        /// プロパティの検証後にエラー状態なら呼び出す。検証エラー通知イベント起動メソッドを呼び出す
        /// </summary>
        /// <param name="errorMessage">エラーメッセージ、メッセージが空なら自動生成</param>
        /// <param name="propertyName">エラー状態のプロパティ名</param>
        public void SetError(string errorMessage, [CallerMemberName] string propertyName = null)
        {
            RaiseErrorsChanged(string.IsNullOrEmpty(errorMessage) ? "Error : PropertyName is " + propertyName : errorMessage);
        }

        /// <summary>
        /// プロパティの検証後にエラー状態解除なら呼び出す。検証エラー通知イベント起動メソッドを呼び出す
        /// </summary>
        /// <param name="propertyName">エラー解除状態のプロパティ名</param>
        public void ResetError([CallerMemberName] string propertyName = null)
        {
            RaiseErrorsChanged("");
        }

        /// <summary>
        /// エラー状態プロパティのエラーメッセージ
        /// </summary>
        /// <param name="propertyName">プロパティ名</param>
        /// <returns>指定したプロパティのエラーメッセージ</returns>
        public IEnumerable GetErrors(string propertyName)
        {
            return hasErrors[propertyName];
        }
        #endregion

        #region INotifyPropertyChanged Implement
        /// <summary>
        /// 変更通知イベント
        /// </summary>
        /// <override name="INotifyPropertyChanged"></override>
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// プロパティフィールド代入メソッド
        /// </summary>
        /// <typeparam name="T">プロパティフィールドの型</typeparam>
        /// <param name="storage">プロパティフィールドへの参照</param>
        /// <param name="value">プロパティフィールドへの代入値</param>
        /// <param name="propertyName">プロパティ名</param>
        /// <returns>True=フィールド値が変更された,False=されなかった</returns>
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
