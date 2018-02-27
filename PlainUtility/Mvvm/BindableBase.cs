using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Utilitys;

namespace Mvvm
{
    /// <summary>
    /// ビューモデル等の基底に使うベースクラス
    /// </summary>
    /// <inherited name="INotifyPropertyChanged">プロパティ変更通知</inherited>
    /// <inherited name="INotifyDataErrorInfo">プロパティ検証エラー通知</inherited>
    abstract public class BindableBase : NotifyBase, INotifyDataErrorInfo
    {
        /// <summary>
        /// 基底コンストラクタ
        /// </summary>
        protected BindableBase() { }

        /// <summary>
        /// 検証エラーがあるプロパティ名と検証エラーメッセージのリスト
        /// </summary>
        private Dictionary<string, List<string>> hasErrors = new Dictionary<string, List<string>>();

        /// <summary>
        /// 検証エラー状態
        /// </summary>
        /// <override name="INotifyDataErrorInfo"></override>
        /// <returns>True=エラーあり,False=エラーなし</returns>
        public bool HasErrors { get { return hasErrors.Count > 0; } }

        /// <summary>
        /// 検証エラー通知イベント
        /// </summary>
        /// <override name="INotifyDataErrorInfo"></override>
        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        /// <summary>
        /// プロパティの検証後に呼び出す。検証エラー通知イベント起動メソッド
        /// </summary>
        /// <override name="INotifyPropertyChanged"></override>
        /// <param name="errorMessage">nullまたは空の文字列でエラーなしまたはエラー状態解除、空白文字列または文字列はエラー状態メッセージ</param>
        /// <param name="propertyName">エラー状態にあるプロパティ名</param>
        protected virtual void OnErrorsChanged(string errorMessage, [CallerMemberName] string propertyName = null)
        {
            if (!string.IsNullOrEmpty(errorMessage))
            {
                if (!hasErrors.ContainsKey(propertyName))
                {
                    hasErrors[propertyName] = new List<string>();
                }
                hasErrors[propertyName].Add(errorMessage);
            }
            else if (hasErrors.Any(x => x.Key == propertyName))
            {
                hasErrors.Remove(propertyName);
            }
            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
        }

        /// <summary>
        /// プロパティの検証後にエラー状態なら呼び出す。検証エラー通知イベント起動メソッドを呼び出す
        /// </summary>
        /// <param name="errorMessage">エラーメッセージ、メッセージが空なら自動生成</param>
        /// <param name="propertyName">エラー状態のプロパティ名</param>
        public void SetError(string errorMessage, [CallerMemberName] string propertyName = null)
        {
            OnErrorsChanged(string.IsNullOrEmpty(errorMessage) ? "Error : PropertyName is " + propertyName : errorMessage, propertyName);
        }

        /// <summary>
        /// プロパティの検証後にエラー状態解除なら呼び出す。検証エラー通知イベント起動メソッドを呼び出す
        /// </summary>
        /// <param name="propertyName">エラー解除状態のプロパティ名</param>
        public void ResetError([CallerMemberName] string propertyName = null)
        {
            OnErrorsChanged("", propertyName);
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
    }
}
