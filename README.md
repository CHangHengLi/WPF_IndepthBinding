# WPF集合绑定MVVM示例

这是一个基于.NET 8.0 WPF的MVVM架构示例项目，展示了如何使用ObservableCollection的扩展实现来处理高级集合绑定场景。
![image](https://github.com/user-attachments/assets/24f25acd-736e-4da9-9828-2ca7210c237d)


## 项目简介

本项目旨在展示WPF中集合绑定的高级用法，尤其是如何处理以下场景：
- 监听集合元素属性的变化
- 批量修改集合元素并高效通知UI
- 使用MVVM模式组织应用程序架构

## 主要功能

- **ItemPropertyObservableCollection**: 监听集合元素属性变化的ObservableCollection扩展实现
- **BulkObservableCollection**: 支持批量操作的ObservableCollection扩展实现
- 完整的MVVM架构示例
- 命令绑定和数据绑定示例

## 项目结构

- **Models**: 包含数据模型定义
  - Person类: 基本数据模型
  - ItemPropertyObservableCollection类: 监听元素属性变化的集合
  - BulkObservableCollection类: 支持批量操作的集合
- **ViewModels**: 包含视图模型
  - ViewModelBase: 基础视图模型类
  - MainViewModel: 主窗口视图模型
- **Commands**: 包含命令实现
  - DelegateCommand: 命令绑定实现

## 如何使用

1. 打开解决方案文件 `WPF_IndepthBinding.sln`
2. 构建并运行项目
3. 主界面展示了数据网格和操作按钮，可以添加、修改和删除数据

## 技术要点

- 使用ObservableCollection基类实现数据绑定
- 通过INotifyPropertyChanged接口实现属性变更通知
- 使用DelegateCommand实现命令绑定
- MVVM架构实现界面和逻辑分离

## 运行环境

- .NET 8.0 SDK
- 支持WPF的Windows操作系统

## 相关文档

详细实现原理请参考项目中的`WPF之集合绑定深入.md`文档。
