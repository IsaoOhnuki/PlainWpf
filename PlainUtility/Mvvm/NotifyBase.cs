using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace Mvvm
{
    /// <summary>
    /// ビューモデル等の基底に使うベースクラス
    /// </summary>
    public abstract class NotifyBase : INotifyPropertyChanged
    {
        /// <summary>
        /// 変更通知イベント
        /// </summary>
        /// <override name="INotifyPropertyChanged"></override>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// 変更通知イベント起動メソッド
        /// </summary>
        /// <param name="propertyName">変更通知するプロパティ名</param>
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
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
            this.OnPropertyChanged(propertyName);
            return true;
        }
    }
}
