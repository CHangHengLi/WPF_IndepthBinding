using System;
using System.ComponentModel;

namespace CollectionBindingDemo.Models
{
    public class Person : INotifyPropertyChanged
    {
        private string _name;
        private int _age;
        
        public event PropertyChangedEventHandler PropertyChanged;
        
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        
        public string Name
        {
            get => _name;
            set
            {
                if (_name != value)
                {
                    _name = value;
                    OnPropertyChanged(nameof(Name));
                }
            }
        }
        
        public int Age
        {
            get => _age;
            set
            {
                if (_age != value)
                {
                    if (value < 0)
                    {
                        throw new ArgumentOutOfRangeException(nameof(Age), "年龄不能为负数");
                    }
                    
                    _age = value;
                    OnPropertyChanged(nameof(Age));
                }
            }
        }

        public override string ToString()
        {
            return $"{Name}, {Age}岁";
        }
    }
} 