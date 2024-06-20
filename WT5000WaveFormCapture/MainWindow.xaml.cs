using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WT5000WaveFormCapture {
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window {
        public MainWindow() {
            InitializeComponent();
        }

        //private void Window_SizeChanged(object sender, SizeChangedEventArgs e) {
        //    double width = this.ActualWidth;
        //    double height = this.ActualHeight;
        //    //tb.Text = $"{width},{height}";
        //}

        private void Window_Loaded(object sender, RoutedEventArgs e) {
            this.Left = 0;
            this.Top = 0;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
            MessageBoxResult result = System.Windows.MessageBox.Show("请确认是否关闭窗口?", "确认", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.No) {
                e.Cancel = true;
            }
            else {
                if (App.WT5k != null) {
                    App.WT5k.Finish();
                }
            }
        }
    }
}
