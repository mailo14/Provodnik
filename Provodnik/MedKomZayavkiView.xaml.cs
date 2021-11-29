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
    public partial class MedKomZayavkiView : Window
    {
        ObservableCollection<MedKomZayavkaViewModel> groups = new ObservableCollection<MedKomZayavkaViewModel>();
        public MedKomZayavkiView()
        {
            InitializeComponent();
            var db = new ProvodnikContext();
            var qq = (from g in db.MedKomZayavki orderby g.Id descending select g).ToList();

          // var ptt = MainWindow.Mapper.Value.Map<MedKomZayavkaViewModel>(new ProvodnikContext().MedKomZayavkas.First(pp => pp.Id == 2));
            foreach  (var q in qq)
            {
                var g = MainWindow.Mapper.Value.Map<MedKomZayavkaViewModel>(q);
  
                /*  MedKomZayavkaViewModel g = new MedKomZayavkaViewModel()
                {
                    City = q.City,
                    Depo = q.Depo,
                    Id = q.Id,
                    Marshrut = q.Marshrut,
                    OtprDat = q.OtprDat,
                    Poezd = q.Poezd,
                    PribDat = q.PribDat,
                    PribTime = q.PribTime,
                    RegOtdelenie = q.RegOtdelenie,
                    Uvolnenie = q.Uvolnenie,
                    Vagon = q.Vagon,
                    Vokzal = q.Vokzal,
                    Vstrechat = q.Vstrechat
                };*/
                //MainWindow.Mapper.Value.Map(q,g);g.Id = q.Id;
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
            var pw = new MedKomZayavkaView();
            pw.DataContext = new MedKomZayavkaViewModel(null);// new ProvodnikContext().Persons.FirstOrDefault()==null?(int?)null:1);
            if (pw.ShowDialog() == true)
            {
                groups.Clear();
                var db = new ProvodnikContext();
                var qq = (from g in db.MedKomZayavki select g);

                foreach (var q in qq)
                {
                    groups.Add(MainWindow.Mapper.Value.Map<MedKomZayavkaViewModel>(q));
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

        private void ClearScan_Button_Click(object sender, RoutedEventArgs e)
        {
            var dc = (sender as Button).DataContext as PersonViewModel.PersonDocViewModel;
            dc.Bitmap.Source = null;
            dc.FileName= null;
        }

        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                Uri uri = new Uri(openFileDialog.FileName);
                var dc = (sender as TextBlock).DataContext as PersonViewModel.PersonDocViewModel;
                dc.Bitmap = new Image() { Source = new BitmapImage(uri) };
                dc.FileName = null;
            }
        }

        void ListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var p = ((FrameworkElement)e.OriginalSource).DataContext as MedKomZayavkaViewModel;
            if (p != null)
            {
                var pw = new MedKomZayavkaView();
                pw.DataContext = new MedKomZayavkaViewModel(p.Id);// new ProvodnikContext().Persons.FirstOrDefault()==null?(int?)null:1);
                if (pw.ShowDialog() == true)
                {
                    var ind = groups.IndexOf(p);
                    groups.RemoveAt(ind);


                    p = MainWindow.Mapper.Value.Map<MedKomZayavkaViewModel>(new ProvodnikContext().MedKomZayavki.First(pp => pp.Id == p.Id));
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
                    var g = GroupsListView.SelectedItem as MedKomZayavkaViewModel;
                    groups.Remove(g);
                    var db = new ProvodnikContext();
                    db.MedKomZayavki.Remove(db.MedKomZayavki.First(pp=>pp.Id==g.Id));
                    db.SaveChanges();
                }
        }
    }
}
