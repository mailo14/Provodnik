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

namespace Provodnik
{
    /// <summary>
    /// Логика взаимодействия для ProgressWindow.xaml
    /// </summary>
    public partial class ProgressWindow : Window, IProgressWindow
    {
        public ProgressWindow()
        {
            InitializeComponent();
        }
        public void ProgressChanged(double progressToAdd, string msg = null)
        {
            Dispatcher.Invoke((Action)(() =>
            {
                Bar.Value += progressToAdd;
                Bar.IsIndeterminate = false;
                if (msg != null) MessageBlock.Text = msg;
            }));
        }
    }
}
