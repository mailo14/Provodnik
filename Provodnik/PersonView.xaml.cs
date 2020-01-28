using FluentFTP;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
{    public class PersonDocument
    {
        public int TypeId { get; set; }
        public string Name { get; set; }
        
}
    /// <summary>
    /// Логика взаимодействия для PersonView.xaml
    /// </summary>
    public partial class PersonView : Window
    {        
        public PersonView()
        {
            InitializeComponent();
          //  this.DataContext = new PersonViewModel(null);// new ProvodnikContext().Persons.FirstOrDefault()==null?(int?)null:1);

            PolComboBox.ItemsSource = new List<string> { "мужской", "женский" };
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
         List<string> GetErrors(PersonViewModel vm){
            List<string> errors = new List<string>();
            if (!vm.Validator.IsValid)
            {
                foreach (var ve in vm.Validator.ValidationMessages) errors.Add(ve.Message);
            }
            errors.AddRange(GetScanErrors(vm));

            return errors;
        }

        List<string> GetScanErrors(PersonViewModel vm) {
            
            List<string> errors = new List<string>();

            foreach (var d in vm.Documents)
            {

                ImageSource source = null;
                Application.Current.Dispatcher.Invoke((Action)(() => {
                    source = d.Bitmap.Source;
                }));
                if (source == null)
                { bool voenPripisEmpty = false;
                    if (d.DocTypeId == 14)
                    {
                Application.Current.Dispatcher.Invoke((Action)(() => {
                        var emptyPripisnoe = (from pdd in vm.Documents where (pdd.DocTypeId == 12 || pdd.DocTypeId == 13) && pdd.Bitmap.Source == null select pdd);
                        if (emptyPripisnoe.Any())
                            voenPripisEmpty = true;
                }));
                    }
                    else
                    if (d.DocTypeId == 12 || d.DocTypeId == 13)
                    {
                Application.Current.Dispatcher.Invoke((Action)(() => {
                        var emptyVoennik = (from pdd in vm.Documents where pdd.DocTypeId == 14 && pdd.Bitmap.Source == null select pdd);
                        if (emptyVoennik.Any())
                            voenPripisEmpty = true;
                }));
                    }
                    else errors.Add($"Не прикреплен скан документа: {d.Description}");
                    if (voenPripisEmpty) errors.Add($"Не прикреплен скан документа: Приписное или военнный билет");
                }
            }
            return errors;
        }

        public PersonViewModel vm;
        bool IsChanged = true;//TODO

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (Save())
                DialogResult = true; 
        }
        private bool Save()
        {
                        vm = this.DataContext as PersonViewModel;
                        List<string> errors = GetErrors(vm);
            var enterErrors = errors.Where(pp => !pp.StartsWith("Не прикреплен скан документа") && !pp.EndsWith(" не заполнено"));
            if (enterErrors.Any() && MessageBox.Show(string.Join(Environment.NewLine,enterErrors)+ Environment.NewLine+"Чтобы поправить, нажмите Отмена", "", MessageBoxButton.OKCancel) == MessageBoxResult.Cancel)
                return false;

            try
            {
                //await new ProgressRunner().RunAsync(
                //    new Action<ProgressHandler>((progressChanged) =>
                    {

                        //progressChanged(1, "Сохранение");


                        //MessageBox.Show(string.Join(Environment.NewLine, errors));
                        vm.Messages = string.Join(Environment.NewLine, errors);
                        

                        var db = new ProvodnikContext();

                        Person p;
                        if (vm.Id.HasValue)
                        {
                            p = db.Persons.Single(pp => pp.Id == vm.Id.Value);

                            MainWindow.Mapper.Value.Map(vm, p);
                            var currents = vm.Documents.Where(pp => pp.Id.HasValue).Select(pp => pp.Id.Value).ToList();
                            var toDelete = (from pd in db.PersonDocs
                                            where pd.PersonId == p.Id && !currents.Contains(pd.Id)
                                            select pd);
                            db.PersonDocs.RemoveRange(toDelete);
                            db.SaveChanges();
                        }
                        else
                        {
                            db.Persons.Add(p = new Person());// { Fio = vm.Fio });
                            MainWindow.Mapper.Value.Map(vm, p);
                            db.SaveChanges();
                        }

                        var allPasports = new string[]{ p.Fio,p.Phone,p.UchZavedenie,p.UchForma,p.Grazdanstvo//,p.Otryad
                ,p.UchebCentr,(p.ExamenDat.HasValue) ? p.ExamenDat.ToString(): p.UchebEndDat.ToString()//TODO
                ,p.RodPhone,p.RodFio
                ,p.BirthDat.ToString(),p.MestoRozd,p.PaspNomer,p.PaspSeriya,p.PaspAdres,p.Snils };
                        p.AllPasport = !allPasports.Any(pp => string.IsNullOrWhiteSpace(pp));
                        db.SaveChanges();

                        //progressChanged(29, "Загрузка сканов");
                        var share = 70.0 / vm.Documents.Count;
                        foreach (var d in vm.Documents)
                        {
                            PersonDoc pd;
                            if (d.Id.HasValue)
                            {
                                pd = db.PersonDocs.Single(pp => pp.Id == d.Id.Value);
                            }
                            else
                            {
                                db.PersonDocs.Add(pd = new PersonDoc() { PersonId = p.Id, IsActive = true, DocTypeId = d.DocTypeId, FileName = d.FileName });
                                db.SaveChanges();
                            }
                            
                            if (d.FileName == null)
                            {
                                ImageSource source = null;
                                Application.Current.Dispatcher.Invoke((Action)(() => {
                                    source = d.Bitmap.Source;
                                }));
                                if (source != null)
                                {
                                    using (var client = new FtpClient("31.31.196.80", new System.Net.NetworkCredential("u0920601", "XP83yno_")))
                                {
                                    client.RetryAttempts = 3;
                                    client.Connect();
                                        var remotePath = $@"ProvodnikDocs/{p.Id.ToString()}/{DateTime.Now.Ticks}.jpg";// "/1_Иванов";

                                        client.Upload(ToByteArray(source as BitmapSource), remotePath, FtpExists.Overwrite, true);//, FtpVerify.Retry);
                                        pd.FileName = remotePath;
                                    }
                                }
                                else pd.FileName = null;
                                db.SaveChanges();
                            }
                        pd.PrinesetK = d.PrinesetK;
                        db.SaveChanges();
                            //progressChanged(share);
                        }

                        p.AllScans = !GetScanErrors(vm).Any();
                        db.SaveChanges();
                    }//));
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }

            //ProgressChanged(100);
            IsChanged = false;
            return true;
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
            
            OpenFileDialog openFileDialog = new OpenFileDialog { Filter = "Jpeg Files |*.jpeg;*.jpg" };// { Filter = "Image Files (*.bmp;*.png;*.jpg)|*.bmp;*.png;*.jpg" };
            if (openFileDialog.ShowDialog() == true)
            {
                Uri uri = new Uri(openFileDialog.FileName);
                var dc = (sender as TextBlock).DataContext as PersonViewModel.PersonDocViewModel;
                dc.Bitmap = new Image() { Source = new BitmapImage(uri) };
                dc.FileName = null;
                dc.PrinesetK = null;
            }
        }


        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            IsChanged = false;
            DialogResult = false;
        }


        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // var vm = (DataContext as SanknizkiViewModel);
            if (IsChanged)
                switch (MessageBox.Show("Сохранить изменения?", "", MessageBoxButton.YesNoCancel))
                {
                    case MessageBoxResult.Yes:
                       if (Save())
                            DialogResult = true;
                       else
                            e.Cancel = true;
                        //else 
                        break;
                    case MessageBoxResult.Cancel:
                        e.Cancel = true;
                        break;
                }
        }

        public void Soglasie()
        {
            var path = (string.Format("{0}\\_шаблоны\\" + "Согласие на обработку персональных данных.DOCX", AppDomain.CurrentDomain.BaseDirectory));
            Microsoft.Office.Interop.Word.Application wordApp = new Microsoft.Office.Interop.Word.Application { Visible = true };
            Microsoft.Office.Interop.Word.Document aDoc = wordApp.Documents.Open(path, ReadOnly: false, Visible: true);
            aDoc.Activate();

           // var vm = DataContext as SendGroupViewModel;

            var db = new ProvodnikContext();

            var p = this.DataContext as PersonViewModel;

            Microsoft.Office.Interop.Word.Range range = aDoc.Content;
            range.Find.ClearFormatting();
            range.Find.Execute(FindText: "{today}", ReplaceWith: DateTime.Today.ToString("dd.MM.yyyy"), Replace: Microsoft.Office.Interop.Word.WdReplace.wdReplaceAll);
            range.Find.Execute(FindText: "{till}", ReplaceWith: DateTime.Today.AddYears(1).ToString("dd.MM.yyyy"), Replace: Microsoft.Office.Interop.Word.WdReplace.wdReplaceAll);
            string passport = $"{p.PaspSeriya} {p.PaspNomer} выдан {p.PaspVidan} {p.VidanDat}";
            range.Find.Execute(FindText: "{passport}", ReplaceWith: passport, Replace: Microsoft.Office.Interop.Word.WdReplace.wdReplaceAll);
            range.Find.Execute(FindText: "{fio}", ReplaceWith: p.Fio, Replace: Microsoft.Office.Interop.Word.WdReplace.wdReplaceAll);
            range.Find.Execute(FindText: "{addres}", ReplaceWith: p.PaspAdres, Replace: Microsoft.Office.Interop.Word.WdReplace.wdReplaceAll);
            range.Find.Execute(FindText: "{till}", ReplaceWith: DateTime.Today.AddYears(1).ToString("dd.MM.yyyy"), Replace: Microsoft.Office.Interop.Word.WdReplace.wdReplaceAll);

            string otchetDir = @"C:\_provodnikFTP";
            aDoc.SaveAs(FileName: otchetDir + @"\Согласие на обработку персональных данных.DOCX");

        }
        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            Soglasie();
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            var p = this.DataContext as PersonViewModel;
            if (!string.IsNullOrWhiteSpace( p.Vk))
            System.Diagnostics.Process.Start(@"https://vk.com/"+ p.Vk);
        }

        private void ClearDocsButton_Click(object sender, RoutedEventArgs e)
        {

            var vm = this.DataContext as PersonViewModel;
            vm.ReCreateDocs();
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {

        }

        private void CheckErrorsButton_Click(object sender, RoutedEventArgs e)
        {
            
            var vm = this.DataContext as PersonViewModel;
            List<string> errors = GetErrors(vm);
            if (errors.Any())
            MessageBox.Show(string.Join(Environment.NewLine, errors));
            else MessageBox.Show("Все данные введены!");
        }

        private void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

            var image = sender as Image;
            var w=new ScanView();
            w.image.Source = image.Source;
            w.Topmost = true;

            var s = System.Windows.Forms.Screen.PrimaryScreen.WorkingArea;
            w.MaxHeight = s.Height;            w.MaxWidth= s.Width;
            var k = 2.0 / 3;
            w.Height = k*s.Height; //w.Width = k*s.Width;
            w.Show();
            
        }
    }
}
