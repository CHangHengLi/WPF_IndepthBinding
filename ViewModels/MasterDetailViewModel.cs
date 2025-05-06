using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;
using System.Windows.Input;
using CollectionBindingDemo.Commands;
using CollectionBindingDemo.Models;
using CollectionBindingDemo.Services;

namespace CollectionBindingDemo.ViewModels
{
    /// <summary>
    /// 主从视图的视图模型
    /// </summary>
    public class MasterDetailViewModel : ViewModelBase
    {
        private ItemPropertyObservableCollection<Person> _people;
        private ICollectionView _peopleView;
        private Person _currentPerson;
        private string _statusMessage;
        private CollectionViewManager _viewManager;
        private readonly ObservableCollection<string> _genderOptions = new ObservableCollection<string> { "男", "女", "其他" };

        // 命令
        public ICommand FirstCommand { get; }
        public ICommand PreviousCommand { get; }
        public ICommand NextCommand { get; }
        public ICommand LastCommand { get; }

        public MasterDetailViewModel()
        {
            // 使用共享数据源
            People = DataRepository.Instance.People;
            
            // 获取集合的默认视图
            _peopleView = CollectionViewSource.GetDefaultView(People);
            
            // 初始化视图管理器
            _viewManager = new CollectionViewManager(_peopleView);
            
            // 监听当前项变更
            _peopleView.CurrentChanged += PeopleView_CurrentChanged;
            
            // 初始化命令
            FirstCommand = new DelegateCommand(ExecuteFirstCommand);
            PreviousCommand = new DelegateCommand(ExecutePreviousCommand);
            NextCommand = new DelegateCommand(ExecuteNextCommand);
            LastCommand = new DelegateCommand(ExecuteLastCommand);
            
            // 监听属性变化
            People.ItemPropertyChanged += (sender, e) =>
            {
                StatusMessage = $"人员 {e.ChangedItem.Name} 的 {e.PropertyName} 属性已更改";
            };
            
            // 设置当前项
            if (People.Count > 0)
            {
                _peopleView.MoveCurrentToFirst();
            }
            
            StatusMessage = $"主从视图已加载，当前共有{People.Count}位人员";
        }

        // 集合视图当前项变更事件处理
        private void PeopleView_CurrentChanged(object sender, EventArgs e)
        {
            CurrentPerson = _peopleView.CurrentItem as Person;
            
            if (CurrentPerson != null)
            {
                StatusMessage = $"当前选中: {CurrentPerson.Name}, {CurrentPerson.Age}岁, {CurrentPerson.Gender}";
            }
        }

        // 公开的属性
        public ItemPropertyObservableCollection<Person> People
        {
            get => _people;
            set => SetProperty(ref _people, value);
        }

        public ICollectionView PeopleView => _peopleView;

        public ObservableCollection<string> GenderOptions => _genderOptions;

        public Person CurrentPerson
        {
            get => _currentPerson;
            set => SetProperty(ref _currentPerson, value);
        }

        public string StatusMessage
        {
            get => _statusMessage;
            set => SetProperty(ref _statusMessage, value);
        }

        // 命令执行方法
        private void ExecuteFirstCommand(object parameter)
        {
            _viewManager.MoveToFirst();
        }

        private void ExecutePreviousCommand(object parameter)
        {
            _viewManager.MoveToPrevious();
        }

        private void ExecuteNextCommand(object parameter)
        {
            _viewManager.MoveToNext();
        }

        private void ExecuteLastCommand(object parameter)
        {
            _viewManager.MoveToLast();
        }
    }
} 