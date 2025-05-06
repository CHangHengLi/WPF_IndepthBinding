using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
using System.Windows.Input;
using CollectionBindingDemo.Commands;
using CollectionBindingDemo.Models;
using CollectionBindingDemo.Services;

namespace CollectionBindingDemo.ViewModels
{
    /// <summary>
    /// CollectionView功能演示的视图模型
    /// </summary>
    public class CollectionViewDemoViewModel : ViewModelBase
    {
        private ItemPropertyObservableCollection<Person> _people;
        private ICollectionView _peopleView;
        private string _statusMessage;
        private CollectionViewManager _viewManager;
        
        // 排序选项
        private bool _isSortByName;
        private bool _isSortByAge;
        private bool _isCustomSort;
        private bool _isMultiSort;
        private bool _isNoSort = true;
        
        // 分组选项
        private bool _isGroupByGender;
        private bool _isGroupByAgeRange;
        private bool _isNoGroup = true;
        
        // 过滤选项
        private bool _isNoFilter = true;
        private bool _isFilterMale;
        private bool _isFilterFemale;
        private bool _isFilterOlder;
        
        // 命令
        public ICommand ReloadDataCommand { get; }
        public ICommand AddRandomPersonCommand { get; }
        
        public CollectionViewDemoViewModel()
        {
            // 使用共享数据源
            People = DataRepository.Instance.People;
            
            // 获取集合的默认视图
            _peopleView = CollectionViewSource.GetDefaultView(People);
            
            // 初始化视图管理器
            _viewManager = new CollectionViewManager(_peopleView);
            
            // 初始化命令
            ReloadDataCommand = new DelegateCommand(ExecuteReloadData);
            AddRandomPersonCommand = new DelegateCommand(ExecuteAddRandomPerson);
            
            // 监听属性变化
            People.ItemPropertyChanged += (sender, e) =>
            {
                StatusMessage = $"人员 {e.ChangedItem.Name} 的 {e.PropertyName} 属性已更改";
            };
            
            StatusMessage = $"集合视图功能演示已加载，当前共有{People.Count}位人员";
        }
        
        // 公开的属性
        public ItemPropertyObservableCollection<Person> People
        {
            get => _people;
            set => SetProperty(ref _people, value);
        }
        
        public ICollectionView PeopleView => _peopleView;
        
        public string StatusMessage
        {
            get => _statusMessage;
            set => SetProperty(ref _statusMessage, value);
        }
        
        #region 排序属性
        
        public bool IsSortByName
        {
            get => _isSortByName;
            set
            {
                if (SetProperty(ref _isSortByName, value) && value)
                {
                    IsCustomSort = false;
                    IsSortByAge = false;
                    IsMultiSort = false;
                    IsNoSort = false;
                    
                    _viewManager.ApplySorting("Name", ListSortDirection.Ascending);
                    StatusMessage = "已按姓名升序排序";
                }
            }
        }
        
        public bool IsSortByAge
        {
            get => _isSortByAge;
            set
            {
                if (SetProperty(ref _isSortByAge, value) && value)
                {
                    IsSortByName = false;
                    IsCustomSort = false;
                    IsMultiSort = false;
                    IsNoSort = false;
                    
                    _viewManager.ApplySorting("Age", ListSortDirection.Descending);
                    StatusMessage = "已按年龄降序排序";
                }
            }
        }
        
        public bool IsCustomSort
        {
            get => _isCustomSort;
            set
            {
                if (SetProperty(ref _isCustomSort, value) && value)
                {
                    IsSortByName = false;
                    IsSortByAge = false;
                    IsMultiSort = false;
                    IsNoSort = false;
                    
                    _viewManager.ApplyCustomSort(new PersonNameComparer());
                    StatusMessage = "已使用自定义比较器按姓氏排序";
                }
            }
        }
        
        public bool IsMultiSort
        {
            get => _isMultiSort;
            set
            {
                if (SetProperty(ref _isMultiSort, value) && value)
                {
                    IsSortByName = false;
                    IsSortByAge = false;
                    IsCustomSort = false;
                    IsNoSort = false;
                    
                    _viewManager.ApplyMultiSorting(
                        new SortDescription("Gender", ListSortDirection.Ascending),
                        new SortDescription("Age", ListSortDirection.Descending));
                    
                    StatusMessage = "已按多字段排序：先按性别，再按年龄降序";
                }
            }
        }
        
        public bool IsNoSort
        {
            get => _isNoSort;
            set
            {
                if (SetProperty(ref _isNoSort, value) && value)
                {
                    IsSortByName = false;
                    IsSortByAge = false;
                    IsCustomSort = false;
                    IsMultiSort = false;
                    
                    // 清除排序
                    _peopleView.SortDescriptions.Clear();
                    
                    if (_peopleView is ListCollectionView listView)
                    {
                        listView.CustomSort = null;
                    }
                    
                    _peopleView.Refresh();
                    StatusMessage = "已清除排序";
                }
            }
        }
        
        #endregion
        
        #region 分组属性
        
        public bool IsGroupByGender
        {
            get => _isGroupByGender;
            set
            {
                if (SetProperty(ref _isGroupByGender, value) && value)
                {
                    IsGroupByAgeRange = false;
                    IsNoGroup = false;
                    
                    _viewManager.ApplyGrouping("Gender");
                    StatusMessage = "已按性别分组";
                }
            }
        }
        
        public bool IsGroupByAgeRange
        {
            get => _isGroupByAgeRange;
            set
            {
                if (SetProperty(ref _isGroupByAgeRange, value) && value)
                {
                    IsGroupByGender = false;
                    IsNoGroup = false;
                    
                    _viewManager.ApplyCustomGrouping(new AgeRangeGroupDescription());
                    StatusMessage = "已按年龄段分组";
                }
            }
        }
        
        public bool IsNoGroup
        {
            get => _isNoGroup;
            set
            {
                if (SetProperty(ref _isNoGroup, value) && value)
                {
                    IsGroupByGender = false;
                    IsGroupByAgeRange = false;
                    
                    _viewManager.ClearGrouping();
                    StatusMessage = "已清除分组";
                }
            }
        }
        
        #endregion
        
        #region 过滤属性
        
        public bool IsNoFilter
        {
            get => _isNoFilter;
            set
            {
                if (SetProperty(ref _isNoFilter, value) && value)
                {
                    IsFilterMale = false;
                    IsFilterFemale = false;
                    IsFilterOlder = false;
                    
                    _viewManager.ClearFilter();
                    StatusMessage = "已清除过滤，显示所有人员";
                }
            }
        }
        
        public bool IsFilterMale
        {
            get => _isFilterMale;
            set
            {
                if (SetProperty(ref _isFilterMale, value) && value)
                {
                    IsNoFilter = false;
                    IsFilterFemale = false;
                    IsFilterOlder = false;
                    
                    _viewManager.ApplyFilter(item => 
                    {
                        if (item is Person person)
                        {
                            return person.Gender == "男";
                        }
                        return false;
                    });
                    
                    StatusMessage = "已过滤，仅显示男性";
                }
            }
        }
        
        public bool IsFilterFemale
        {
            get => _isFilterFemale;
            set
            {
                if (SetProperty(ref _isFilterFemale, value) && value)
                {
                    IsNoFilter = false;
                    IsFilterMale = false;
                    IsFilterOlder = false;
                    
                    _viewManager.ApplyFilter(item => 
                    {
                        if (item is Person person)
                        {
                            return person.Gender == "女";
                        }
                        return false;
                    });
                    
                    StatusMessage = "已过滤，仅显示女性";
                }
            }
        }
        
        public bool IsFilterOlder
        {
            get => _isFilterOlder;
            set
            {
                if (SetProperty(ref _isFilterOlder, value) && value)
                {
                    IsNoFilter = false;
                    IsFilterMale = false;
                    IsFilterFemale = false;
                    
                    _viewManager.ApplyFilter(item => 
                    {
                        if (item is Person person)
                        {
                            return person.Age >= 30;
                        }
                        return false;
                    });
                    
                    StatusMessage = "已过滤，仅显示年龄≥30的人员";
                }
            }
        }
        
        #endregion
        
        // 命令执行方法
        private void ExecuteReloadData(object parameter)
        {
            // 清空并重新加载数据
            DataRepository.Instance.ReloadData();
            
            // 重置所有设置
            ResetAllSettings();
            
            StatusMessage = "已重新加载数据";
        }
        
        private void ExecuteAddRandomPerson(object parameter)
        {
            // 添加一个随机人员
            var addedPeople = DataRepository.Instance.AddRandomPeople(1);
            var person = addedPeople[0];
            
            StatusMessage = $"已添加随机人员: {person.Name}, {person.Age}岁, {person.Gender}";
        }
        
        // 重置所有设置
        private void ResetAllSettings()
        {
            // 重置排序选项
            _isSortByName = false;
            _isSortByAge = false;
            _isCustomSort = false;
            _isMultiSort = false;
            _isNoSort = true;
            
            // 重置分组选项
            _isGroupByGender = false;
            _isGroupByAgeRange = false;
            _isNoGroup = true;
            
            // 重置过滤选项
            _isNoFilter = true;
            _isFilterMale = false;
            _isFilterFemale = false;
            _isFilterOlder = false;
            
            // 清除CollectionView的设置
            _peopleView.SortDescriptions.Clear();
            _peopleView.GroupDescriptions.Clear();
            _peopleView.Filter = null;
            
            if (_peopleView is ListCollectionView listView)
            {
                listView.CustomSort = null;
            }
            
            _peopleView.Refresh();
            
            // 通知UI更新
            OnPropertyChanged(nameof(IsSortByName));
            OnPropertyChanged(nameof(IsSortByAge));
            OnPropertyChanged(nameof(IsCustomSort));
            OnPropertyChanged(nameof(IsMultiSort));
            OnPropertyChanged(nameof(IsNoSort));
            
            OnPropertyChanged(nameof(IsGroupByGender));
            OnPropertyChanged(nameof(IsGroupByAgeRange));
            OnPropertyChanged(nameof(IsNoGroup));
            
            OnPropertyChanged(nameof(IsNoFilter));
            OnPropertyChanged(nameof(IsFilterMale));
            OnPropertyChanged(nameof(IsFilterFemale));
            OnPropertyChanged(nameof(IsFilterOlder));
        }
    }
} 