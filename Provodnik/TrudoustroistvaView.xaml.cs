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
    public partial class TrudoustroistvaView : Window
    {
        ObservableCollection<TrudoustroistvoShortViewModel> groups = new ObservableCollection<TrudoustroistvoShortViewModel>();
        public TrudoustroistvaView()
        {
            InitializeComponent();
            GroupsListView.ItemsSource = groups;

            FillGroups();

          //  this.DataContext = new PersonViewModel(new ProvodnikContext().Persons.FirstOrDefault()==null?(int?)null:1);
        }

        private void FillGroups()
        {
            groups.Clear();

               var db = new ProvodnikContext();
            var qq = (from g in db.Trudoustroistva where g.StartDate==null || g.StartDate.Year==DateTime.Now.Year select g)
                .ToList();
            qq = qq.Select(x => new { x, num = int.TryParse(x.Name?.Substring(0, Math.Max(1, x.Name.IndexOf("_"))), out var num) ? (int?)num : null })
             .ToList().OrderByDescending(x => x.num)
             .Select(x => x.x)
         .ToList();

            // var ptt = MainWindow.Mapper.Value.Map<TrudoustroistvoViewModel>(new ProvodnikContext().Trudoustroistvos.First(pp => pp.Id == 2));
            foreach (var q in qq)
            {
                var g = MainWindow.Mapper.Value.Map<TrudoustroistvoShortViewModel>(q);
                groups.Add(g);
            }
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
            var pw = new TrudoustroistvoView();
            pw.DataContext = new TrudoustroistvoViewModel(null);// new ProvodnikContext().Persons.FirstOrDefault()==null?(int?)null:1);
            if (pw.ShowDialog() == true)
            {
                FillGroups();
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
            var p = ((FrameworkElement)e.OriginalSource).DataContext as TrudoustroistvoShortViewModel;
            if (p != null)
            {
                var pw = new TrudoustroistvoView();
                pw.DataContext = new TrudoustroistvoViewModel(p.Id);// new ProvodnikContext().Persons.FirstOrDefault()==null?(int?)null:1);
                if (pw.ShowDialog() == true)
                {
                    var ind = groups.IndexOf(p);
                    groups.RemoveAt(ind);


                    p = MainWindow.Mapper.Value.Map<TrudoustroistvoShortViewModel>(new ProvodnikContext().Trudoustroistva.First(pp => pp.Id == p.Id));
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
                    var g = GroupsListView.SelectedItem as TrudoustroistvoShortViewModel;
                    groups.Remove(g);
                    var db = new ProvodnikContext();
                    db.Trudoustroistva.Remove(db.Trudoustroistva.First(pp=>pp.Id==g.Id));
                    db.SaveChanges();
                }
        }
    }
}
