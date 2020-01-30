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
    /// Логика взаимодействия для AlarmsView.xaml
    /// </summary>
    public partial class AlarmsView : Window
    {
        public AlarmsView()
        {
            InitializeComponent();
            RefreshList();
        }
        void RefreshList()
        {
            PersonsListView.ItemsSource = new Repository().GetAlarms();

        }

        private void ListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var pe = ((FrameworkElement)e.OriginalSource).DataContext as AlarmViewModel;
            if (pe != null)
            {
                var vm = this.DataContext as SendGroupViewModel;
                var pw = new PersonView();
                var db = new ProvodnikContext();
                var pvm = new PersonViewModel(pe.Id);
                pw.DataContext = pvm;
                if (pw.ShowDialog() == true)
                {
                    RefreshList();
                }
            }
        }
    }
}
