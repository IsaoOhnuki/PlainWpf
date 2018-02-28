using Utilitys;
using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Mvvm;

namespace Utilitys
{
    /// <summary>
    /// アンドゥ、リドゥ機能スタック
    /// </summary>
    /// <typeparam name="T">保持するオブジェクト</typeparam>
    public class HistoryStack<T> : NotifyBase, IDisposable
    {
        private bool disposed;
        private int stackPoint;
        private List<T> stack;

        /// <summary>
        /// Initializes a new instance of the <see cref="HistoryStack{T}"/> class.
        /// </summary>
        public HistoryStack()
        {
            stackPoint = -1;
            stack = new List<T>();
        }

        /// <summary>
        /// Gets a value indicating whether this instance can undo.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance can undo; otherwise, <c>false</c>.
        /// </value>
        public bool CanUndo { get { return stackPoint > 0 && !disposed; } }
        /// <summary>
        /// Gets a value indicating whether this instance can redo.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance can redo; otherwise, <c>false</c>.
        /// </value>
        public bool CanRedo { get { return stackPoint < stack.Count - 1 && !disposed; } }
        /// <summary>
        /// Undoes this instance.
        /// </summary>
        /// <returns></returns>
        public T Undo()
        {
            if (!disposed && CanUndo)
            {
                var t = stack[--stackPoint];
                OnPropertyChanged(nameof(CanUndo));
                OnPropertyChanged(nameof(CanRedo));
                return t;
            }
            else
                return default(T);
        }
        /// <summary>
        /// Redoes this instance.
        /// </summary>
        /// <returns></returns>
        public T Redo()
        {
            if (!disposed && CanRedo)
            {
                var t = stack[++stackPoint];
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
            if (!disposed)
            {
                Rewind();
                stack.Add(t);
                ++stackPoint;
                OnPropertyChanged(nameof(CanUndo));
                OnPropertyChanged(nameof(CanRedo));
            }
        }
        /// <summary>
        /// ポイント位置までクリアします
        /// </summary>
        private void Rewind()
        {
            while (CanRedo)
            {
                var tt = stack[stack.Count - 1];
                if (tt is IDisposable)
                {
                    (tt as IDisposable).Dispose();
                }
                stack.RemoveAt(stack.Count - 1);
            }
        }
        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        public void Dispose()
        {
            if (!disposed)
            {
                disposed = true;
                stackPoint = 0;
                Rewind();
            }
        }
    }
}
