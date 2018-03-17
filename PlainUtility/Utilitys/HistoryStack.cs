using Utilitys;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Mvvm;
using System.Collections.Specialized;
using System.Collections;

namespace Utilitys
{
    public interface IHistory<T>
    {
        T Undo();
        T Redo();
        bool CanUndo { get; }
        bool CanRedo { get; }
        void Push(T t);
    }

    /// <summary>
    /// アンドゥ、リドゥ機能スタック
    /// </summary>
    /// <typeparam name="T">保持するオブジェクト</typeparam>
    public class HistoryStack<T> : NotifyBase, IHistory<T>, INotifyCollectionChanged, ICollection<T>
    {
        private int stackPoint;
        private List<T> stack;

        /// <summary>
        /// Initializes a new instance of the <see cref="HistoryStack{T}"/> class.
        /// </summary>
        /// <param name="KeepFirstItem"></param>
        public HistoryStack(bool KeepFirstItem = true)
        {
            this.KeepFirstItem = KeepFirstItem;
            stackPoint = -1;
            stack = new List<T>();
        }

        /// <summary>
        /// 
        /// </summary>
        public bool KeepFirstItem { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this instance can undo.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance can undo; otherwise, <c>false</c>.
        /// </value>
        public bool CanUndo { get { return stackPoint > (KeepFirstItem ? 0 : -1); } }
        /// <summary>
        /// Gets a value indicating whether this instance can redo.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance can redo; otherwise, <c>false</c>.
        /// </value>
        public bool CanRedo { get { return stackPoint < stack.Count - 1; } }

        /// <summary>
        /// Undoes this instance.
        /// </summary>
        /// <returns></returns>
        public T Undo()
        {
            T t = default(T);
            if (CanUndo)
            {
                T tt = stack[stackPoint];
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, tt));
                --stackPoint;
                if (stackPoint >= 0)
                {
                    t = stack[stackPoint];
                }
                OnPropertyChanged(nameof(CanUndo));
                OnPropertyChanged(nameof(CanRedo));
            }
            return t;
        }
        /// <summary>
        /// Redoes this instance.
        /// </summary>
        /// <returns></returns>
        public T Redo()
        {
            if (CanRedo)
            {
                var t = stack[++stackPoint];
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, t));
                OnPropertyChanged(nameof(CanUndo));
                OnPropertyChanged(nameof(CanRedo));
                return t;
            }
            else
                return default(T);
        }
        /// <summary>
        /// Pushes the specified t.
        /// </summary>
        /// <param name="t">The t.</param>
        public void Push(T t)
        {
            Rewind();
            stack.Add(t);
            ++stackPoint;
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, t));
            OnPropertyChanged(nameof(CanUndo));
            OnPropertyChanged(nameof(CanRedo));
        }
        /// <summary>
        /// ポイント位置までクリアします
        /// </summary>
        private void Rewind()
        {
            var list = new List<T>();
            while (CanRedo)
            {
                var tt = stack[stack.Count - 1];
                stack.Remove(tt);
                list.Add(tt);
            }
            if (list.Count > 0)
            {
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, list));
            }
        }

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs notifyCollectionChangedEventArgs)
        {
            CollectionChanged?.Invoke(this, notifyCollectionChangedEventArgs);
        }

        public int Count => stackPoint + 1;

        public bool IsReadOnly => throw new NotImplementedException();

        public void Add(T item)
        {
            Push(item);
        }

        public void Clear()
        {
            stackPoint = -1;
            Rewind();
            OnPropertyChanged(nameof(CanUndo));
            OnPropertyChanged(nameof(CanRedo));
        }

        public bool Contains(T item)
        {
            return stack.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            var items = array.Skip(arrayIndex).Take(array.Count() - arrayIndex).ToArray();
            if (items != null)
            {
                Rewind();
                foreach (var item in items)
                {
                    Push(item);
                }
            }
        }

        public bool Remove(T item)
        {
            throw new NotImplementedException();
        }

        public IEnumerator<T> GetEnumerator()
        {
            foreach (var val in stack.Take(stackPoint + 1).ToArray())
            {
                yield return val;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            foreach (var val in stack.Take(stackPoint + 1).ToArray())
            {
                yield return val;
            }
        }
    }
}
