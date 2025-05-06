using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using CollectionBindingDemo.Commands;
using CollectionBindingDemo.Models;

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

        // 姓氏和名字库，用于随机生成姓名
        private readonly string[] _familyNames = 
        {
            "赵", "钱", "孙", "李", "周", "吴", "郑", "王", 
            "冯", "陈", "褚", "卫", "蒋", "沈", "韩", "杨", 
            "朱", "秦", "尤", "许", "何", "吕", "施", "张", 
            "孔", "曹", "严", "华", "金", "魏", "陶", "姜"
        };
        
        private readonly string[] _givenNames = 
        {
            "伟", "芳", "娜", "秀英", "敏", "静", "丽", "强", 
            "磊", "军", "洋", "勇", "艳", "杰", "娟", "涛", 
            "明", "超", "秀兰", "霞", "平", "刚", "桂英", "华"
        };
        
        // 随机数生成器
        private readonly Random _random = new Random();

        public MainViewModel()
        {
            // 初始化集合
            People = new ItemPropertyObservableCollection<Person>();
            
            // 初始化命令
            AddPersonCommand = new DelegateCommand(ExecuteAddPerson, CanExecuteAddPerson);
            RemovePersonCommand = new DelegateCommand(ExecuteRemovePerson, CanExecuteRemovePerson);
            ClearPeopleCommand = new DelegateCommand(ExecuteClearPeople);
            AddRangeCommand = new DelegateCommand(ExecuteAddRange);
            FilterOlderThanCommand = new DelegateCommand(ExecuteFilterOlderThan);
            
            // 添加测试数据
            LoadInitialData();
            
            // 监听属性变化
            People.ItemPropertyChanged += (sender, e) =>
            {
                StatusMessage = $"人员 {e.ChangedItem.Name} 的 {e.PropertyName} 属性已更改";
            };
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
                var newPerson = new Person { Name = NewPersonName, Age = NewPersonAge };
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
            var bulkCollection = new BulkObservableCollection<Person>();
            
            // 复制当前集合中的所有项目
            foreach (var person in People)
            {
                bulkCollection.Add(person);
            }
            
            // 随机生成4位人员
            var newPeople = new List<Person>();
            for (int i = 0; i < 4; i++)
            {
                string randomName = GenerateRandomName();
                int randomAge = _random.Next(18, 61); // 18-60岁之间的随机年龄
                
                newPeople.Add(new Person { Name = randomName, Age = randomAge });
            }
            
            // 批量添加随机生成的数据
            bulkCollection.AddRange(newPeople);
            
            // 替换当前集合
            People = new ItemPropertyObservableCollection<Person>(bulkCollection);
            
            StatusMessage = $"已批量添加{newPeople.Count}位随机人员";
        }

        // 生成随机中文姓名
        private string GenerateRandomName()
        {
            string familyName = _familyNames[_random.Next(_familyNames.Length)];
            string givenName = _givenNames[_random.Next(_givenNames.Length)];
            
            return familyName + givenName;
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

        // 加载初始数据
        private void LoadInitialData()
        {
            People.Add(new Person { Name = "张三", Age = 28 });
            People.Add(new Person { Name = "李四", Age = 32 });
            People.Add(new Person { Name = "王五", Age = 25 });
            People.Add(new Person { Name = "赵六", Age = 41 });
            
            StatusMessage = "已加载4位初始人员";
        }
    }
} 