using Utilitys;
using System;
using System.Collections.Generic;
using System.Text;

namespace Utilitys
{
    /// <summary>
    /// アンドゥ、リドゥ機能スタック
    /// </summary>
    /// <typeparam name="T">保持するオブジェクト</typeparam>
    public class ObjectStack<T> : BindableBase, IDisposable
    {
        private int stackPoint = 0;
        private List<T> stack = new List<T>();
        /// <summary>
        /// Gets a value indicating whether this instance can undo.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance can undo; otherwise, <c>false</c>.
        /// </value>
        public bool CanUndo { get { return stackPoint > 0; } }
        /// <summary>
        /// Gets a value indicating whether this instance can redo.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance can redo; otherwise, <c>false</c>.
        /// </value>
        public bool CanRedo { get { return stackPoint < stack.Count; } }
        /// <summary>
        /// Undoes this instance.
        /// </summary>
        /// <returns></returns>
        public T Undo()
        {
            var t = stack[--stackPoint];
            RaisePropertyChanged(nameof(CanUndo));
            RaisePropertyChanged(nameof(CanRedo));
            return t;
        }
        /// <summary>
        /// Redoes this instance.
        /// </summary>
        /// <returns></returns>
        public T Redo()
        {
            var t = stack[stackPoint++];
            RaisePropertyChanged(nameof(CanUndo));
            RaisePropertyChanged(nameof(CanRedo));
            return t;
        }
        /// <summary>
        /// 全てクリアします
        /// </summary>
        public void RewindAll()
        {
            stackPoint = 0;
            Rewind();
            RaisePropertyChanged(nameof(CanUndo));
            RaisePropertyChanged(nameof(CanRedo));
        }
        /// <summary>
        /// ポイント位置までクリアします
        /// </summary>
        public void Rewind()
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
            RaisePropertyChanged(nameof(CanRedo));
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
            RaisePropertyChanged(nameof(CanUndo));
            RaisePropertyChanged(nameof(CanRedo));
        }
        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        public void Dispose()
        {
            RewindAll();
        }
    }
}
