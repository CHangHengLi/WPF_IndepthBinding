using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Data;
using CollectionBindingDemo.Models;

namespace CollectionBindingDemo.ViewModels
{
    /// <summary>
    /// CollectionView管理器，提供排序、分组和过滤功能
    /// </summary>
    public class CollectionViewManager
    {
        private readonly ICollectionView _collectionView;

        public CollectionViewManager(ICollectionView collectionView)
        {
            _collectionView = collectionView ?? throw new ArgumentNullException(nameof(collectionView));
        }

        /// <summary>
        /// 获取当前的CollectionView
        /// </summary>
        public ICollectionView CollectionView => _collectionView;

        /// <summary>
        /// 应用排序
        /// </summary>
        public void ApplySorting(string propertyName, ListSortDirection direction)
        {
            // 清除现有排序
            _collectionView.SortDescriptions.Clear();
            
            // 添加排序描述
            _collectionView.SortDescriptions.Add(new SortDescription(propertyName, direction));
            
            // 刷新视图
            _collectionView.Refresh();
        }

        /// <summary>
        /// 应用多字段排序
        /// </summary>
        public void ApplyMultiSorting(params SortDescription[] sortDescriptions)
        {
            // 清除现有排序
            _collectionView.SortDescriptions.Clear();
            
            // 添加所有排序描述
            foreach (var sort in sortDescriptions)
            {
                _collectionView.SortDescriptions.Add(sort);
            }
            
            // 刷新视图
            _collectionView.Refresh();
        }

        /// <summary>
        /// 应用自定义排序
        /// </summary>
        public void ApplyCustomSort(IComparer comparer)
        {
            if (_collectionView is ListCollectionView listView)
            {
                listView.CustomSort = comparer;
                _collectionView.Refresh();
            }
        }

        /// <summary>
        /// 应用过滤
        /// </summary>
        public void ApplyFilter(Predicate<object> filter)
        {
            _collectionView.Filter = filter;
            _collectionView.Refresh();
        }

        /// <summary>
        /// 清除过滤器
        /// </summary>
        public void ClearFilter()
        {
            _collectionView.Filter = null;
            _collectionView.Refresh();
        }

        /// <summary>
        /// 应用分组
        /// </summary>
        public void ApplyGrouping(string propertyName)
        {
            // 清除现有分组
            _collectionView.GroupDescriptions.Clear();
            
            // 添加分组描述
            _collectionView.GroupDescriptions.Add(new PropertyGroupDescription(propertyName));
            
            // 刷新视图
            _collectionView.Refresh();
        }

        /// <summary>
        /// 应用自定义分组
        /// </summary>
        public void ApplyCustomGrouping(GroupDescription groupDescription)
        {
            // 清除现有分组
            _collectionView.GroupDescriptions.Clear();
            
            // 添加自定义分组描述
            _collectionView.GroupDescriptions.Add(groupDescription);
            
            // 刷新视图
            _collectionView.Refresh();
        }

        /// <summary>
        /// 清除分组
        /// </summary>
        public void ClearGrouping()
        {
            _collectionView.GroupDescriptions.Clear();
            _collectionView.Refresh();
        }

        /// <summary>
        /// 移动到第一项
        /// </summary>
        public bool MoveToFirst()
        {
            return _collectionView.MoveCurrentToFirst();
        }

        /// <summary>
        /// 移动到最后一项
        /// </summary>
        public bool MoveToLast()
        {
            return _collectionView.MoveCurrentToLast();
        }

        /// <summary>
        /// 移动到下一项
        /// </summary>
        public bool MoveToNext()
        {
            return _collectionView.MoveCurrentToNext();
        }

        /// <summary>
        /// 移动到上一项
        /// </summary>
        public bool MoveToPrevious()
        {
            return _collectionView.MoveCurrentToPrevious();
        }

        /// <summary>
        /// 移动到指定项
        /// </summary>
        public bool MoveTo(object item)
        {
            return _collectionView.MoveCurrentTo(item);
        }
    }

    /// <summary>
    /// 按年龄段分组的自定义分组描述
    /// </summary>
    public class AgeRangeGroupDescription : GroupDescription
    {
        public override object GroupNameFromItem(object item, int level, CultureInfo culture)
        {
            // 转换为Person对象
            Person person = item as Person;
            if (person == null)
                return null;
            
            // 根据年龄范围分组
            if (person.Age < 20)
                return "20岁以下";
            else if (person.Age < 30)
                return "20-29岁";
            else if (person.Age < 40)
                return "30-39岁";
            else
                return "40岁及以上";
        }
    }
} 