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

        /// <summary>
        /// 打开主从视图示例窗口
        /// </summary>
        public void OpenMasterDetailWindow()
        {
            var window = new MasterDetailWindow();
            window.Owner = this;
            window.ShowDialog();
        }

        /// <summary>
        /// 打开CollectionView功能示例窗口
        /// </summary>
        public void OpenCollectionViewWindow()
        {
            var window = new CollectionViewWindow();
            window.Owner = this;
            window.ShowDialog();
        }
    }
} 