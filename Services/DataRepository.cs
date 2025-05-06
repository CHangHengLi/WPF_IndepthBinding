using System;
using System.Collections.Generic;
using CollectionBindingDemo.Models;

namespace CollectionBindingDemo.Services
{
    /// <summary>
    /// 数据仓库类，提供全局共享的数据源
    /// </summary>
    public class DataRepository
    {
        // 单例模式实现
        private static DataRepository _instance;
        public static DataRepository Instance => _instance ??= new DataRepository();

        // 共享的数据集合
        public ItemPropertyObservableCollection<Person> People { get; }
        
        // 随机生成数据用
        private readonly Random _random = new Random();
        
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

        private DataRepository()
        {
            People = new ItemPropertyObservableCollection<Person>();
            LoadInitialData();
        }

        // 初始化数据
        private void LoadInitialData()
        {
            People.Add(new Person { Name = "张三", Age = 28, Gender = "男" });
            People.Add(new Person { Name = "李四", Age = 32, Gender = "男" });
            People.Add(new Person { Name = "王五", Age = 25, Gender = "男" });
            People.Add(new Person { Name = "赵六", Age = 41, Gender = "男" });
            People.Add(new Person { Name = "小红", Age = 22, Gender = "女" });
            People.Add(new Person { Name = "小芳", Age = 35, Gender = "女" });
            People.Add(new Person { Name = "刘洋", Age = 29, Gender = "男" });
            People.Add(new Person { Name = "陈静", Age = 27, Gender = "女" });
            People.Add(new Person { Name = "杨光", Age = 45, Gender = "男" });
            People.Add(new Person { Name = "周敏", Age = 31, Gender = "女" });
        }
        
        /// <summary>
        /// 添加随机生成的人员
        /// </summary>
        /// <param name="count">要添加的人员数量</param>
        /// <returns>添加的人员列表</returns>
        public List<Person> AddRandomPeople(int count)
        {
            var newPeople = new List<Person>();
            for (int i = 0; i < count; i++)
            {
                string randomName = GenerateRandomName();
                int randomAge = _random.Next(18, 61); // 18-60岁之间的随机年龄
                string randomGender = _random.Next(2) == 0 ? "男" : "女";
                
                var person = new Person { Name = randomName, Age = randomAge, Gender = randomGender };
                newPeople.Add(person);
                People.Add(person);
            }
            
            return newPeople;
        }
        
        /// <summary>
        /// 批量添加随机人员（使用BulkObservableCollection优化性能）
        /// </summary>
        /// <param name="count">要添加的人员数量</param>
        /// <returns>添加的人员列表</returns>
        public List<Person> AddRandomPeopleBulk(int count)
        {
            var bulkCollection = new BulkObservableCollection<Person>();
            
            // 复制当前集合中的所有项目
            foreach (var person in People)
            {
                bulkCollection.Add(person);
            }
            
            // 随机生成人员
            var newPeople = new List<Person>();
            for (int i = 0; i < count; i++)
            {
                string randomName = GenerateRandomName();
                int randomAge = _random.Next(18, 61); // 18-60岁之间的随机年龄
                string randomGender = _random.Next(2) == 0 ? "男" : "女";
                
                var person = new Person { Name = randomName, Age = randomAge, Gender = randomGender };
                newPeople.Add(person);
            }
            
            // 批量添加随机生成的数据
            bulkCollection.AddRange(newPeople);
            
            // 清空并重建原集合
            People.Clear();
            foreach (var person in bulkCollection)
            {
                People.Add(person);
            }
            
            return newPeople;
        }
        
        /// <summary>
        /// 清空并重新加载初始数据
        /// </summary>
        public void ReloadData()
        {
            People.Clear();
            LoadInitialData();
        }
        
        // 生成随机中文姓名
        private string GenerateRandomName()
        {
            string familyName = _familyNames[_random.Next(_familyNames.Length)];
            string givenName = _givenNames[_random.Next(_givenNames.Length)];
            
            return familyName + givenName;
        }
    }
} 