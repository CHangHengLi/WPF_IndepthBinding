using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace CollectionBindingDemo.ViewModels
{
    public abstract class ViewModelBase : INotifyPropertyChanged, IDataErrorInfo
    {
        private Dictionary<string, string> _errors = new Dictionary<string, string>();

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        // IDataErrorInfo接口实现
        public string Error => null;

        public string this[string columnName]
        {
            get
            {
                if (_errors.ContainsKey(columnName))
                {
                    return _errors[columnName];
                }
                return null;
            }
        }

        protected void SetError(string propertyName, string errorMessage)
        {
            _errors[propertyName] = errorMessage;
            OnPropertyChanged(nameof(Error));
            OnPropertyChanged($"Item[{propertyName}]");
        }

        protected void ClearError(string propertyName)
        {
            if (_errors.ContainsKey(propertyName))
            {
                _errors.Remove(propertyName);
                OnPropertyChanged(nameof(Error));
                OnPropertyChanged($"Item[{propertyName}]");
            }
        }
    }
} 