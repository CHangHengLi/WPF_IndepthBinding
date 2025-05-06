using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Windows.Threading;

namespace CollectionBindingDemo.Models
{
    /// <summary>
    /// 线程安全的ObservableCollection实现
    /// </summary>
    public class ThreadSafeObservableCollection<T> : ObservableCollection<T>
    {
        private readonly Dispatcher _dispatcher;
        private readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim();
        
        /// <summary>
        /// 构造函数
        /// </summary>
        public ThreadSafeObservableCollection() : base()
        {
            // 存储创建集合的线程的Dispatcher
            _dispatcher = Dispatcher.CurrentDispatcher;
        }
        
        /// <summary>
        /// 使用现有集合创建新集合
        /// </summary>
        public ThreadSafeObservableCollection(IEnumerable<T> collection) : base(collection)
        {
            _dispatcher = Dispatcher.CurrentDispatcher;
        }
        
        /// <summary>
        /// 在UI线程上安全地执行操作
        /// </summary>
        private void ExecuteOnUIThread(Action action)
        {
            // 如果已经在UI线程上，直接执行
            if (_dispatcher.CheckAccess())
            {
                action();
            }
            else
            {
                // 否则调度到UI线程执行
                _dispatcher.Invoke(action);
            }
        }
        
        /// <summary>
        /// 添加项目
        /// </summary>
        public new void Add(T item)
        {
            _lock.EnterWriteLock();
            try
            {
                ExecuteOnUIThread(() => base.Add(item));
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }
        
        /// <summary>
        /// 移除项目
        /// </summary>
        public new bool Remove(T item)
        {
            _lock.EnterWriteLock();
            try
            {
                bool result = false;
                ExecuteOnUIThread(() => result = base.Remove(item));
                return result;
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }
        
        /// <summary>
        /// 插入项目
        /// </summary>
        public new void Insert(int index, T item)
        {
            _lock.EnterWriteLock();
            try
            {
                ExecuteOnUIThread(() => base.Insert(index, item));
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }
        
        /// <summary>
        /// 移除指定位置的项目
        /// </summary>
        public new void RemoveAt(int index)
        {
            _lock.EnterWriteLock();
            try
            {
                ExecuteOnUIThread(() => base.RemoveAt(index));
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }
        
        /// <summary>
        /// 清空集合
        /// </summary>
        public new void Clear()
        {
            _lock.EnterWriteLock();
            try
            {
                ExecuteOnUIThread(() => base.Clear());
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }
        
        /// <summary>
        /// 获取项目的索引
        /// </summary>
        public new int IndexOf(T item)
        {
            _lock.EnterReadLock();
            try
            {
                int result = -1;
                ExecuteOnUIThread(() => result = base.IndexOf(item));
                return result;
            }
            finally
            {
                _lock.ExitReadLock();
            }
        }
        
        /// <summary>
        /// 访问集合中的元素
        /// </summary>
        public new T this[int index]
        {
            get
            {
                _lock.EnterReadLock();
                try
                {
                    T result = default(T);
                    ExecuteOnUIThread(() => result = base[index]);
                    return result;
                }
                finally
                {
                    _lock.ExitReadLock();
                }
            }
            set
            {
                _lock.EnterWriteLock();
                try
                {
                    ExecuteOnUIThread(() => base[index] = value);
                }
                finally
                {
                    _lock.ExitWriteLock();
                }
            }
        }
        
        /// <summary>
        /// 批量添加项目（线程安全）
        /// </summary>
        public void AddRange(IEnumerable<T> items)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));
                
            _lock.EnterWriteLock();
            try
            {
                ExecuteOnUIThread(() =>
                {
                    var notificationBackup = BlockReentrancy();
                    try
                    {
                        foreach (var item in items)
                        {
                            base.InsertItem(Count, item);
                        }
                    }
                    finally
                    {
                        notificationBackup.Dispose();
                    }
                    
                    OnCollectionChanged(new NotifyCollectionChangedEventArgs(
                        NotifyCollectionChangedAction.Reset));
                });
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }
    }
} 