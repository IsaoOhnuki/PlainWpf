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
    public class ObjectStack<T> : NotifyBase, IDisposable
    {
        private bool disposed;
        private int basePoint;
        private int stackPoint;
        private List<T> stack;

        /// <summary>
        /// Initializes a new instance of the <see cref="ObjectStack{T}"/> class.
        /// </summary>
        public ObjectStack()
        {
            basePoint = 0;
            stackPoint = basePoint;
            stack = new List<T>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ObjectStack{T}"/> class.
        /// </summary>
        /// <param name="firstObject">The first.</param>
        public ObjectStack(T firstObject)
        {
            basePoint = 0;
            stackPoint = basePoint;
            stack = new List<T>();
            FirstObject = firstObject;
        }

        /// <summary>
        /// Gets or sets the first object.
        /// </summary>
        /// <value>
        /// The first object.
        /// </value>
        public T FirstObject
        {
            get
            {
                if (basePoint == 1)
                    return stack[0];
                else
                    return default(T);
            }
            set
            {
                if (basePoint == stack.Count)
                {
                    basePoint = 0;
                    RewindAll();
                    basePoint = 1;
                    stack.Add(value);
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance can undo.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance can undo; otherwise, <c>false</c>.
        /// </value>
        public bool CanUndo { get { return stackPoint > basePoint && !disposed; } }
        /// <summary>
        /// Gets a value indicating whether this instance can redo.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance can redo; otherwise, <c>false</c>.
        /// </value>
        public bool CanRedo { get { return stackPoint < stack.Count && !disposed; } }
        /// <summary>
        /// Undoes this instance.
        /// </summary>
        /// <returns></returns>
        public T Undo()
        {
            if (!disposed)
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
            if (!disposed)
            {
                var t = stack[stackPoint++];
                OnPropertyChanged(nameof(CanUndo));
                OnPropertyChanged(nameof(CanRedo));
                return t;
            }
            else
                return default(T);
        }
        /// <summary>
        /// 全てクリアします
        /// </summary>
        public void RewindAll()
        {
            if (!disposed)
            {
                stackPoint = basePoint;
                Rewind();
                OnPropertyChanged(nameof(CanUndo));
                OnPropertyChanged(nameof(CanRedo));
            }
        }
        /// <summary>
        /// ポイント位置までクリアします
        /// </summary>
        public void Rewind()
        {
            if (!disposed)
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
                OnPropertyChanged(nameof(CanRedo));
            }
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
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        public void Dispose()
        {
            if (!disposed)
            {
                disposed = true;
                basePoint = 0;
                RewindAll();
            }
        }
    }
}
