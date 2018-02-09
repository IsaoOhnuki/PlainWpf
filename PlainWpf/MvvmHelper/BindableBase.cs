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
    /// ビューモデルの基底に使うベースクラス
    /// </summary>
    /// <inherited name="INotifyPropertyChanged">プロパティ変更通知</inherited>
    /// <inherited name="INotifyDataErrorInfo">プロパティ検証エラー通知</inherited>
    abstract public class BindableBase : INotifyPropertyChanged, INotifyDataErrorInfo
    {
        /// <summary>
        /// 基底コンストラクタ
        /// </summary>
        protected BindableBase() { }

        #region INotifyDataErrorInfo
        /// <summary>
        /// 検証エラーがあるプロパティ名と検証エラーメッセージのリスト
        /// </summary>
        private Dictionary<string, string> hasErrors = new Dictionary<string, string>();

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
            if (ErrorsChanged != null)
            {
                foreach (var hasError in hasErrors)
                {
                    ErrorsChanged.Invoke(this, new DataErrorsChangedEventArgs(hasError.Key));
                }
            }
        }

        /// <summary>
        /// エラー状態プロパティのエラーメッセージ
        /// </summary>
        /// <param name="propertyName">プロパティ名</param>
        /// <returns>エラーメッセージ</returns>
        public IEnumerable GetErrors(string propertyName)
        {
            return hasErrors[propertyName];
        }
        #endregion

        #region INotifyPropertyChanged
        /// <summary>
        /// 変更通知イベント
        /// </summary>
        /// <override name="INotifyPropertyChanged"></override>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// 変更通知イベント起動メソッド
        /// </summary>
        /// <param name="propertyName">変更通知するプロパティ名</param>
        protected virtual void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// プロパティフィールド代入メソッド
        /// </summary>
        /// <typeparam name="T">プロパティフィールドの型</typeparam>
        /// <param name="storage">プロパティフィールドへの参照</param>
        /// <param name="value">プロパティフィールドの代入値</param>
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
