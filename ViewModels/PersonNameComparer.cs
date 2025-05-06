using System;
using System.Collections;
using CollectionBindingDemo.Models;

namespace CollectionBindingDemo.ViewModels
{
    /// <summary>
    /// 自定义比较器 - 按姓氏拼音排序
    /// </summary>
    public class PersonNameComparer : IComparer
    {
        // 实现Compare方法
        public int Compare(object x, object y)
        {
            // 转换为Person对象
            var person1 = x as Person;
            var person2 = y as Person;
            
            if (person1 == null || person2 == null)
                return 0;
            
            // 自定义排序逻辑 - 按姓氏拼音排序
            string surname1 = GetSurname(person1.Name);
            string surname2 = GetSurname(person2.Name);
            
            return string.Compare(surname1, surname2, StringComparison.Ordinal);
        }
        
        // 获取中文姓氏
        private string GetSurname(string fullName)
        {
            // 简单处理：假设第一个字是姓氏
            if (!string.IsNullOrEmpty(fullName) && fullName.Length > 0)
                return fullName.Substring(0, 1);
            
            return string.Empty;
        }
    }
} 