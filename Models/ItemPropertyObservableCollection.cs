using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;

namespace CollectionBindingDemo.Models
{
    /// <summary>
    /// 可以监听集合元素属性变化的ObservableCollection扩展实现
    /// </summary>
    public class ItemPropertyObservableCollection<T> : ObservableCollection<T>
        where T : INotifyPropertyChanged
    {
        /// <summary>
        /// 元素属性变化事件
        /// </summary>
        public event EventHandler<ItemPropertyChangedEventArgs<T>> ItemPropertyChanged;
        
        public ItemPropertyObservableCollection() : base() { }
        
        public ItemPropertyObservableCollection(IEnumerable<T> collection) : base(collection)
        {
            // 为所有初始项目添加事件处理程序
            AttachPropertyChangedHandlers(collection);
        }
        
        /// <summary>
        /// 重写InsertItem方法，为新项目添加属性变化处理程序
        /// </summary>
        protected override void InsertItem(int index, T item)
        {
            base.InsertItem(index, item);
            
            // 添加属性变化监听
            AttachPropertyChangedHandler(item);
        }
        
        /// <summary>
        /// 重写RemoveItem方法，移除项目的属性变化处理程序
        /// </summary>
        protected override void RemoveItem(int index)
        {
            // 移除属性变化监听
            DetachPropertyChangedHandler(this[index]);
            
            base.RemoveItem(index);
        }
        
        /// <summary>
        /// 重写ClearItems方法，移除所有项目的属性变化处理程序
        /// </summary>
        protected override void ClearItems()
        {
            foreach (var item in this)
            {
                DetachPropertyChangedHandler(item);
            }
            
            base.ClearItems();
        }
        
        /// <summary>
        /// 重写SetItem方法，为替换项目更新属性变化处理程序
        /// </summary>
        protected override void SetItem(int index, T item)
        {
            var oldItem = this[index];
            
            // 移除旧项目的属性变化监听
            DetachPropertyChangedHandler(oldItem);
            
            base.SetItem(index, item);
            
            // 添加新项目的属性变化监听
            AttachPropertyChangedHandler(item);
        }
        
        /// <summary>
        /// 为集合中的所有项目添加属性变化处理程序
        /// </summary>
        private void AttachPropertyChangedHandlers(IEnumerable<T> items)
        {
            foreach (var item in items)
            {
                AttachPropertyChangedHandler(item);
            }
        }
        
        /// <summary>
        /// 为单个项目添加属性变化处理程序
        /// </summary>
        private void AttachPropertyChangedHandler(T item)
        {
            if (item != null)
            {
                item.PropertyChanged += Item_PropertyChanged;
            }
        }
        
        /// <summary>
        /// 移除单个项目的属性变化处理程序
        /// </summary>
        private void DetachPropertyChangedHandler(T item)
        {
            if (item != null)
            {
                item.PropertyChanged -= Item_PropertyChanged;
            }
        }
        
        /// <summary>
        /// 项目属性变化处理方法
        /// </summary>
        private void Item_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            // 转发属性变化事件
            ItemPropertyChanged?.Invoke(this, new ItemPropertyChangedEventArgs<T>
            {
                ChangedItem = (T)sender,
                PropertyName = e.PropertyName
            });
            
            // 重新触发CollectionChanged事件，以便UI更新
            // 使用Reset操作来确保所有绑定都更新
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(
                NotifyCollectionChangedAction.Reset));
        }
    }
    
    /// <summary>
    /// 集合元素属性变化事件参数
    /// </summary>
    public class ItemPropertyChangedEventArgs<T> : EventArgs
    {
        public T ChangedItem { get; set; }
        public string PropertyName { get; set; }
    }
} 