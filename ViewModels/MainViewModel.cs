using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using CollectionBindingDemo.Commands;
using CollectionBindingDemo.Models;
using CollectionBindingDemo.Services;

namespace CollectionBindingDemo.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private ItemPropertyObservableCollection<Person> _people;
        private Person _selectedPerson;
        private string _newPersonName;
        private string _newPersonAgeText = "0";
        private int _newPersonAge = 0;
        private string _statusMessage;

        // 命令
        public ICommand AddPersonCommand { get; }
        public ICommand RemovePersonCommand { get; }
        public ICommand ClearPeopleCommand { get; }
        public ICommand AddRangeCommand { get; }
        public ICommand FilterOlderThanCommand { get; }
        public ICommand OpenMasterDetailWindowCommand { get; }
        public ICommand OpenCollectionViewWindowCommand { get; }

        public MainViewModel()
        {
            // 使用共享数据源
            People = DataRepository.Instance.People;
            
            // 初始化命令
            AddPersonCommand = new DelegateCommand(ExecuteAddPerson, CanExecuteAddPerson);
            RemovePersonCommand = new DelegateCommand(ExecuteRemovePerson, CanExecuteRemovePerson);
            ClearPeopleCommand = new DelegateCommand(ExecuteClearPeople);
            AddRangeCommand = new DelegateCommand(ExecuteAddRange);
            FilterOlderThanCommand = new DelegateCommand(ExecuteFilterOlderThan);
            OpenMasterDetailWindowCommand = new DelegateCommand(ExecuteOpenMasterDetailWindow);
            OpenCollectionViewWindowCommand = new DelegateCommand(ExecuteOpenCollectionViewWindow);
            
            // 监听属性变化
            People.ItemPropertyChanged += (sender, e) =>
            {
                StatusMessage = $"人员 {e.ChangedItem.Name} 的 {e.PropertyName} 属性已更改";
            };
            
            StatusMessage = $"主窗口已加载，当前共有{People.Count}位人员";
        }

        // 公开的属性
        public ItemPropertyObservableCollection<Person> People
        {
            get => _people;
            set => SetProperty(ref _people, value);
        }

        public Person SelectedPerson
        {
            get => _selectedPerson;
            set => SetProperty(ref _selectedPerson, value);
        }

        public string NewPersonName
        {
            get => _newPersonName;
            set 
            { 
                SetProperty(ref _newPersonName, value);
                ((DelegateCommand)AddPersonCommand).RaiseCanExecuteChanged();
            }
        }

        public int NewPersonAge
        {
            get => _newPersonAge;
            set 
            {
                try
                {
                    if (value < 0)
                    {
                        SetError(nameof(NewPersonAge), "年龄不能为负数");
                    }
                    else
                    {
                        ClearError(nameof(NewPersonAge));
                        SetProperty(ref _newPersonAge, value);
                    }
                    ((DelegateCommand)AddPersonCommand).RaiseCanExecuteChanged();
                }
                catch (Exception ex)
                {
                    SetError(nameof(NewPersonAge), $"年龄格式错误: {ex.Message}");
                    ((DelegateCommand)AddPersonCommand).RaiseCanExecuteChanged();
                }
            }
        }

        public string NewPersonAgeText
        {
            get => _newPersonAgeText;
            set
            {
                if (SetProperty(ref _newPersonAgeText, value))
                {
                    try
                    {
                        if (int.TryParse(value, out int age))
                        {
                            NewPersonAge = age;
                        }
                        else
                        {
                            SetError(nameof(NewPersonAge), "请输入有效的数字");
                            ((DelegateCommand)AddPersonCommand).RaiseCanExecuteChanged();
                        }
                    }
                    catch (Exception ex)
                    {
                        SetError(nameof(NewPersonAge), $"年龄格式错误: {ex.Message}");
                        ((DelegateCommand)AddPersonCommand).RaiseCanExecuteChanged();
                    }
                }
            }
        }

        public string StatusMessage
        {
            get => _statusMessage;
            set => SetProperty(ref _statusMessage, value);
        }

        // 命令执行方法
        private void ExecuteAddPerson(object parameter)
        {
            if (string.IsNullOrEmpty(this[nameof(NewPersonAge)]) && int.TryParse(_newPersonAgeText, out int age))
            {
                var newPerson = new Person { Name = NewPersonName, Age = NewPersonAge, Gender = "未知" };
                People.Add(newPerson);
                StatusMessage = $"已添加人员: {newPerson.Name}, {newPerson.Age}岁";
                
                // 清空输入
                NewPersonName = string.Empty;
                NewPersonAgeText = "0";
                NewPersonAge = 0;
            }
        }

        private bool CanExecuteAddPerson(object parameter)
        {
            return !string.IsNullOrWhiteSpace(NewPersonName) && 
                   int.TryParse(_newPersonAgeText, out int _) &&
                   string.IsNullOrEmpty(this[nameof(NewPersonAge)]);
        }

        private void ExecuteRemovePerson(object parameter)
        {
            if (SelectedPerson != null)
            {
                string personInfo = $"{SelectedPerson.Name}, {SelectedPerson.Age}岁";
                People.Remove(SelectedPerson);
                StatusMessage = $"已删除人员: {personInfo}";
            }
        }

        private bool CanExecuteRemovePerson(object parameter)
        {
            return SelectedPerson != null;
        }

        private void ExecuteClearPeople(object parameter)
        {
            People.Clear();
            StatusMessage = "已清空所有人员";
        }

        private void ExecuteAddRange(object parameter)
        {
            var addedPeople = DataRepository.Instance.AddRandomPeopleBulk(4);
            StatusMessage = $"已批量添加{addedPeople.Count}位随机人员";
        }

        private void ExecuteFilterOlderThan(object parameter)
        {
            int ageLimit = 30;
            
            var oldPeople = People.Where(p => p.Age > ageLimit).ToList();
            StatusMessage = $"找到 {oldPeople.Count} 位年龄大于 {ageLimit} 岁的人员";
            
            // 高亮显示第一个符合条件的人员
            if (oldPeople.Any())
            {
                SelectedPerson = oldPeople.First();
            }
        }

        // 打开主从视图窗口
        private void ExecuteOpenMasterDetailWindow(object parameter)
        {
            // 获取当前主窗口
            if (System.Windows.Application.Current.MainWindow is MainWindow mainWindow)
            {
                mainWindow.OpenMasterDetailWindow();
            }
            
            StatusMessage = "已打开主从视图示例窗口";
        }

        // 打开CollectionView示例窗口
        private void ExecuteOpenCollectionViewWindow(object parameter)
        {
            // 获取当前主窗口
            if (System.Windows.Application.Current.MainWindow is MainWindow mainWindow)
            {
                mainWindow.OpenCollectionViewWindow();
            }
            
            StatusMessage = "已打开CollectionView功能示例窗口";
        }
    }
} 