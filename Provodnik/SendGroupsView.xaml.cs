using FluentFTP;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// Логика взаимодействия для PersonView.xaml
    /// </summary>
    public partial class SendGroupsView : Window
    {

        ObservableCollection<SendGroupShortViewModel> groups = new ObservableCollection<SendGroupShortViewModel>();
        public SendGroupsView()
        {
            InitializeComponent();
            var db = new ProvodnikContext();
            var qq = (from g in db.SendGroups orderby g.Id descending select g).ToList();

          // var ptt = MainWindow.Mapper.Value.Map<SendGroupViewModel>(new ProvodnikContext().SendGroups.First(pp => pp.Id == 2));
            foreach  (var q in qq)
            {
                var g = MainWindow.Mapper.Value.Map<SendGroupShortViewModel>(q);
                  groups.Add(g);
            }

            GroupsListView.ItemsSource = groups;
          //  this.DataContext = new PersonViewModel(new ProvodnikContext().Persons.FirstOrDefault()==null?(int?)null:1);
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        public static byte[] ToByteArray(BitmapSource bitmapSource)
        {

            var encoder = new JpegBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(bitmapSource));

            using (var stream = new System.IO.MemoryStream())
            {
                encoder.Save(stream);
                return stream.ToArray();
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var pw = new SendGroupView();
            pw.DataContext = new SendGroupViewModel(null);// new ProvodnikContext().Persons.FirstOrDefault()==null?(int?)null:1);
            if (pw.ShowDialog() == true)
            {
                groups.Clear();
                var db = new ProvodnikContext();
                var qq = (from g in db.SendGroups orderby g.Id descending select g);

                foreach (var q in qq)
                {
                    groups.Add(MainWindow.Mapper.Value.Map<SendGroupShortViewModel>(q));
                }
            }
        }

        private void TextBox_TextChanged_1(object sender, TextChangedEventArgs e)
        {

        }

        private void ComboBox_GotFocus(object sender, RoutedEventArgs e)
        {
            (sender as ComboBox).IsDropDownOpen = true;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {

        }


        void ListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var p = ((FrameworkElement)e.OriginalSource).DataContext as SendGroupShortViewModel;
            if (p != null)
            {
                var pw = new SendGroupView();
                pw.DataContext = new SendGroupViewModel(p.Id);// new ProvodnikContext().Persons.FirstOrDefault()==null?(int?)null:1);
                if (pw.ShowDialog() == true)
                {
                    var ind = groups.IndexOf(p);
                    groups.RemoveAt(ind);


                    p = MainWindow.Mapper.Value.Map<SendGroupShortViewModel>(new ProvodnikContext().SendGroups.First(pp => pp.Id == p.Id));
                    groups.Insert(ind, p);
                    //vm.RefreshPersonList();
                    //TODO goto if exist or add anyway and goto
                    /**/
                }
            }
        }

        private void PersonsListView_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
                if (GroupsListView.SelectedItem != null &&
                MessageBox.Show("Удалить?", "Подтверждение удаления", MessageBoxButton.OKCancel, MessageBoxImage.Question) == MessageBoxResult.OK)
                {
                    var g = GroupsListView.SelectedItem as SendGroupShortViewModel;
                    groups.Remove(g);
                    var db = new ProvodnikContext();
                    db.SendGroups.Remove(db.SendGroups.First(pp=>pp.Id==g.Id));
                    db.SaveChanges();
                }
        }
    }
}
