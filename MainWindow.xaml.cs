using System.Windows;
using CollectionBindingDemo.ViewModels;

namespace CollectionBindingDemo
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            // 设置DataContext为MainViewModel
            DataContext = new MainViewModel();
        }
    }
} 