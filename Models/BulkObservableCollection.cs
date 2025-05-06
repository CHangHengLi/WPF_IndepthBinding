using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;

namespace CollectionBindingDemo.Models
{
    /// <summary>
    /// 支持批量操作的ObservableCollection扩展实现
    /// </summary>
    public class BulkObservableCollection<T> : ObservableCollection<T>
    {
        // 标记是否正在进行批量操作
        private bool _suppressNotification = false;
        
        /// <summary>
        /// 执行批量操作而不触发多次通知
        /// </summary>
        /// <param name="action">要执行的批量操作</param>
        public void ExecuteBulkOperation(Action action)
        {
            // 暂停通知
            _suppressNotification = true;
            
            try
            {
                // 执行批量操作
                action();
            }
            finally
            {
                // 恢复通知
                _suppressNotification = false;
                
                // 操作完成后发送一次重置通知
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(
                    NotifyCollectionChangedAction.Reset));
            }
        }
        
        /// <summary>
        /// 批量添加项目
        /// </summary>
        public void AddRange(IEnumerable<T> items)
        {
            ExecuteBulkOperation(() =>
            {
                foreach (var item in items)
                {
                    Add(item);
                }
            });
        }
        
        /// <summary>
        /// 批量移除项目
        /// </summary>
        public void RemoveRange(IEnumerable<T> items)
        {
            ExecuteBulkOperation(() =>
            {
                foreach (var item in items)
                {
                    Remove(item);
                }
            });
        }
        
        /// <summary>
        /// 重写基类的OnCollectionChanged方法，以支持通知抑制
        /// </summary>
        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if (!_suppressNotification)
            {
                base.OnCollectionChanged(e);
            }
        }
        
        /// <summary>
        /// 重写基类的OnPropertyChanged方法，以支持通知抑制
        /// </summary>
        protected override void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (!_suppressNotification)
            {
                base.OnPropertyChanged(e);
            }
        }
    }
} 