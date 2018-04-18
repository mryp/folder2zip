using Folder2Zip.ViewModels;
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
using System.Windows.Shapes;

namespace Folder2Zip.Views
{
    /// <summary>
    /// ShellView.xaml の相互作用ロジック
    /// </summary>
    public partial class ShellView : Window
    {
        private ShellViewModel ViewModel
        {
            get
            {
                return DataContext as ShellViewModel;
            }
        }

        public ShellView()
        {
            InitializeComponent();
        }
    }
}
