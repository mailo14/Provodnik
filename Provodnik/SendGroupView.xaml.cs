using FluentFTP;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using static Provodnik.PersonViewModel;
using Excel = Microsoft.Office.Interop.Excel;

namespace Provodnik
{    
    /// <summary>
    /// Логика взаимодействия для PersonView.xaml
    /// </summary>
    public partial class SendGroupView : Window
    {
        public SendGroupView()
        {
            InitializeComponent();
            //   this.DataContext = new PersonViewModel(new ProvodnikContext().Persons.FirstOrDefault()==null?(int?)null:1);//SendGroupViewModel
            Loaded += SendGroupView_Loaded;
        }

        private void SendGroupView_Loaded(object sender, RoutedEventArgs e)
        {
            //(DepoComboBox as TextBoxBase).TextChanged += SendGroupView_TextChanged;
            CityComboBox.AddHandler(TextBoxBase.TextChangedEvent, new RoutedEventHandler(OnDepoComboBoxTextChanged));
            DepoComboBox.AddHandler(TextBoxBase.TextChangedEvent, new RoutedEventHandler(OnDepoComboBoxTextChanged));
        }

        private void OnDepoComboBoxTextChanged(object sender, RoutedEventArgs e)
        {
            var vm = DataContext as SendGroupViewModel;
            vm.DepoRod = new Repository().GetF6Depo(vm.City, vm.Depo);
        }

        private void ComboBox_GotFocus(object sender, RoutedEventArgs e)
        {
            (sender as ComboBox).IsDropDownOpen = true;
        }

        public  void VSOP1()//DateTime start, DateTime end, ProgressBar progress)
        {
            var vm = DataContext as SendGroupViewModel;

            excelReport excel = new excelReport();
            excel.Init("ВСОП_1.xlsx", string.Format(@"ВСОП_1_{0}_{1}__{2}_{3}.xlsx",
                vm.OtprDat.Value.ToString("dd.MM.yyyy"), 
                "Новосибирск",
                vm.City,
                vm.Persons.Count), otchetDir: otchetDir);

            int            ri = 1;
            var db = new ProvodnikContext();
            var ids= vm.Persons.Select(pp=>pp.PersonId).ToList();
            var rr = db.Persons.Where(pp => ids.Contains(pp.Id));
            foreach (var r in rr)//vm.Persons)
            {
                    ri++;
                excel.cell[ri, 1].value2 = vm.RegOtdelenie;
                excel.cell[ri, 2].value2 = r.Fio;
                excel.cell[ri, 3].value2 = vm.Depo;
                excel.cell[ri, 4].value2 = Helper.FormatPhone(r.Phone);

                excel.cell[ri, 5].value2 = r.VaccineSert;
                excel.cell[ri, 6].value2 = r.VaccineSertDat?.ToString("dd.MM.yyyy");


                excel.cell[ri, 7].value2 = r.UchZavedenie;
                excel.cell[ri, 8].value2 = r.UchForma;
                excel.cell[ri, 9].value2 = r.Grazdanstvo;
                excel.cell[ri, 10].value2 = r.Otryad;
                excel.cell[ri, 11].value2 = r.UchebCentr;
                excel.cell[ri, 12].value2 = (r.ExamenDat.HasValue)?r.ExamenDat.Value.Year.ToString() 
                    : (r.UchebEndDat.HasValue?r.UchebEndDat.Value.Year.ToString():"");
                excel.cell[ri, 13].value2 = Helper.FormatPhone(r.RodPhone);
                excel.cell[ri, 14].value2 = r.RodFio;
              
            }
            excel.setAllBorders(excel.get_Range("A2", "N" + ri)); 
            excel.Finish();
        }
        public void VSOP2()//DateTime start, DateTime end, ProgressBar progress)
        {
            var vm = DataContext as SendGroupViewModel;

            excelReport excel = new excelReport();
            excel.Init("ВСОП_2.xlsx", string.Format(@"ВСОП_2_{0}_{1}__{2}_{3}.xlsx",
                vm.OtprDat.Value.ToString("dd.MM.yyyy"),
                "Новосибирск",
                vm.City,
                vm.Persons.Count), otchetDir: otchetDir);

            int ri = 1;
            var db = new ProvodnikContext();
            var ids = vm.Persons.Select(pp => pp.PersonId).ToList();
            var rr = db.Persons.Where(pp => ids.Contains(pp.Id));
            foreach (var r in rr)//vm.Persons)
            {
                ri++;
                excel.cell[ri, 1].value2 = ri - 1;
                excel.cell[ri, 2].value2 = vm.RegOtdelenie;
                var fio = new StringHelper().ParseFio(r.Fio);
                excel.cell[ri, 3].value2 = fio[0];
                excel.cell[ri, 4].value2 = fio[1];
                excel.cell[ri, 5].value2 = fio[2];
                excel.cell[ri, 6].value2 = r.BirthDat.HasValue ? r.BirthDat.Value.ToString("dd.MM.yyyy") : "";
                excel.cell[ri, 7].value2 = r.UchebCentr;
                excel.cell[ri, 8].value2 = (r.ExamenDat.HasValue) ? r.ExamenDat.Value.Year.ToString()
                    : (r.UchebEndDat.HasValue ? r.UchebEndDat.Value.Year.ToString() : "");
            }
            excel.setAllBorders(excel.get_Range("A2", "I" + ri));  
            excel.Finish();
        }

        public  void VSOP3(bool otdelno=false)//DateTime start, DateTime end, ProgressBar progress)
        {
            var vm = DataContext as SendGroupViewModel;

            excelReport excel = new excelReport();
          if (!otdelno)  excel.Init("ВСОП_3.xlsx", string.Format(@"ВСОП_3_{0}_{1}__{2}_{3}.xlsx",
                vm.OtprDat.Value.ToString("dd.MM.yyyy"), 
                "Новосибирск",
                vm.City,
                vm.Persons.Count),otchetDir: otchetDir);
            else excel.Init("ВСОП_3.xlsx", string.Format(@"ВСОП_3_{0}_{1}__{2}_{3}.xlsx",
                 vm.OtprDat.Value.ToString("dd.MM.yyyy"),
                 "Новосибирск",
                 vm.City,
                 vm.Persons.Count), true);

            int            ri = 1;
            var db = new ProvodnikContext();
            var ids= vm.Persons.Select(pp=>pp.PersonId).ToList();
            var rr = db.Persons.Where(pp => ids.Contains(pp.Id));

            ri = 2;
            excel.cell[ri, 2].value2 = vm.RegOtdelenie;
            excel.cell[ri, 3].value2 = vm.Persons.Count;
            excel.cell[ri, 4].value2 = vm.Poezd;
            excel.cell[ri, 5].value2 = vm.Vagon;
            excel.cell[ri, 6].value2 = vm.OtprDat.HasValue ? vm.OtprDat.Value.ToString("dd.MM.yyyy") : "";
            excel.cell[ri, 7].value2 = vm.PribDat.HasValue ? vm.PribDat.Value.ToString("dd.MM.yyyy") : "";
            excel.cell[ri, 8].value2 = vm.PribTime;
            var main = vm.Persons.FirstOrDefault(pp => pp.IsMain);
            if (main != null)
            {
                excel.cell[ri, 9].value2 = main.Fio;
                excel.cell[ri, 10].value2 = Helper.FormatPhone(main.Phone); 
            }
            excel.cell[ri, 11].value2 = vm.Vstrechat?"да":"нет";
            excel.cell[ri, 12].value2 = vm.Vokzal;

            ri =1;
            foreach (var r in rr)//vm.Persons)
            {
                    ri++;               
                excel.cell[ri, 13].value2 = r.Fio;         
            }
            excel.setAllBorders(excel.get_Range("B2", "M" + ri));  
            excel.Finish(!otdelno);
        }
        public  void F6()//DateTime start, DateTime end, ProgressBar progress)
        {
            var vm = DataContext as SendGroupViewModel;

            excelReport excel = new excelReport();
            excel.Init(/*(vm.City== "Москва")?"Ф6_Msk.xlsx":*/"Ф6_ost.xlsx",
                $@"Ф6_{vm.OtprDat.Value.ToString("dd.MM.yyyy")}_Новосибирск__{ vm.City}_{vm.Persons.Count}.xlsx",
                otchetDir: otchetDir//,visible:true
                );

            int            ri =7;
            var db = new ProvodnikContext();
            var ids= vm.Persons.Select(pp=>pp.PersonId).ToList();
            var rr = db.Persons.Where(pp => ids.Contains(pp.Id));

            excel.cell[4, 1].value2 = vm.DepoRod;// new Repository().GetF6Depo(vm.City, vm.Depo);

            if (vm.Persons.Count > 1)
            {
                excel.get_Range("A9", "A" + (8 + vm.Persons.Count - 1)).EntireRow.Select();
                excel.myExcel.Selection.Insert(Excel.XlInsertShiftDirection.xlShiftDown, Excel.XlInsertFormatOrigin.xlFormatFromLeftOrAbove);
            }

            foreach (var r in rr)//vm.Persons)
            {
                    ri++;

                excel.cell[ri, 1].value2 = ri-7;
                excel.cell[ri, 2].value2 = r.Fio+Environment.NewLine+ (r.BirthDat.HasValue ? r.BirthDat.Value.ToString("dd.MM.yyyy") : "")
                    + Environment.NewLine + r.MestoRozd;
                excel.cell[ri, 3].value2 = "Проводник" + Environment.NewLine + "пассажирского" + Environment.NewLine + "вагона";
                
                excel.cell[ri, 5].value2 = r.PaspAdres;
                excel.cell[ri, 6].value2 =
                    r.Grazdanstvo== "КЗ"? "Паспорт гр-на РК:" : "Паспорт гр-на РФ:"
                    + Environment.NewLine + r.PaspSeriya+" "+r.PaspNomer
                    +Environment.NewLine+ Helper.FormatSnils(r.Snils);

                excel.cell[ri, 7].value2 = (!string.IsNullOrWhiteSpace(vm.PeresadSt))
                    ? $"Новосибирск – {vm.PeresadSt} – {vm.City} – {vm.PeresadSt} – Новосибирск"
                    : $"Новосибирск – {vm.City} – Новосибирск";

                excel.cell[ri, 11].value2 = vm.Uvolnenie.HasValue ? vm.Uvolnenie.Value.ToString("dd.MM.yyyy") : "";
            }
            excel.setAllBorders(excel.get_Range("A8", "K" + ri));  
            excel.Finish();
        }

        
    public void PismoLgoti()
        {

            var vm = DataContext as SendGroupViewModel;

            var db = new ProvodnikContext();
            var ids = vm.Persons.Select(pp => pp.PersonId);
            var pe =            db.Persons.Where(pp => ids.Contains(pp.Id) && !pp.HasLgota).ToList();
            if (pe.Count == 0) return;


            var path=(string.Format("{0}\\_шаблоны\\" + "Письмо об отсутствии льготы.docx", AppDomain.CurrentDomain.BaseDirectory));
            Microsoft.Office.Interop.Word.Application wordApp = new Microsoft.Office.Interop.Word.Application { Visible = false };
            Microsoft.Office.Interop.Word.Document aDoc = wordApp.Documents.Open(path, ReadOnly: false, Visible: false);
            aDoc.Activate();


            Microsoft.Office.Interop.Word.Range range = aDoc.Content;
            range.Find.ClearFormatting();

            var list =new List<string>();
            for (int i = 0; i < pe.Count; i++)
                list.Add(  $"{i + 1}. {pe[i].Fio}");

            range.Find.Execute(FindText: "{list}", ReplaceWith: string.Join("\r", list ), Replace: Microsoft.Office.Interop.Word.WdReplace.wdReplaceAll);

                aDoc.SaveAs(FileName: otchetDir + @"\Письмо об отсутствии льготы.docx");
                aDoc.Close();




        }

        public void ReestrPeredachi()
        {
            var vm = DataContext as SendGroupViewModel;

            var db = new ProvodnikContext();
            var ids = vm.Persons.Select(pp => pp.PersonId);
            var pe = db.Persons.Where(pp => ids.Contains(pp.Id)).ToList();
            if (pe.Count == 0) return;


            var path = (string.Format("{0}\\_шаблоны\\" + "Реестр передачи.docx", AppDomain.CurrentDomain.BaseDirectory));
            Microsoft.Office.Interop.Word.Application wordApp = new Microsoft.Office.Interop.Word.Application { Visible = false };
            Microsoft.Office.Interop.Word.Document aDoc = wordApp.Documents.Open(path, ReadOnly: false, Visible: false);
            aDoc.Activate();

            object missing = Missing.Value;

            Microsoft.Office.Interop.Word.Range range = aDoc.Content;
            range.Find.ClearFormatting();
            
            range.Find.Execute(FindText: "{ВЧ}", ReplaceWith: vm.DepoRod, Replace: Microsoft.Office.Interop.Word.WdReplace.wdReplaceAll); 

            var table = aDoc.Tables[1];
            int iRow = 2;

            foreach (var p in vm.Persons)
            {
                if (iRow > 2) table.Rows.Add(ref missing);
                table.Cell(iRow, 1).Range.Text = (iRow - 1).ToString() + '.';
                table.Cell(iRow, 2).Range.Text = p.Fio;
                table.Cell(iRow, 3).Range.Text = p.UchForma;
                table.Cell(iRow, 4).Range.Text = "ЕСТЬ";

                iRow++;
            }

            aDoc.SaveAs(FileName: otchetDir + $@"\Реестр передачи {vm.City}.docx");
            aDoc.Close();       }

        string otchetDir ;
        private void AddFromListButton_Click(object sender, RoutedEventArgs e)
        {
            var vm= this.DataContext as SendGroupViewModel;
            var psw=new PersonsView(1);
            var pswVm = psw.DataContext as PersonsViewModel;
            pswVm.ReadyOnly = true;
            pswVm.Gorod = vm.City;
            pswVm.VihodDat = vm.OtprDat;

            pswVm.FindCommand.Execute(null);//psw.FindButton_Click(null,null);
            if (psw.ShowDialog() == true)
            {
                var db = new ProvodnikContext();
                foreach (var id in psw.vm.PersonList.Where(pp => pp.IsSelected).Select(pp=>pp.Id))
                {

                    if (vm.Persons.FirstOrDefault(pp => pp.PersonId == id) != null) continue;

                    var np = MainWindow.Mapper.Value.Map<SendGroupPersonViewModel>(db.Persons.First(pp => pp.Id == id));

                     np.SendGroupViewModel = vm;
                    if (vm.Persons.Count == 0) np.IsMain = true;
                    vm.Persons.Add(np);
                    
                }
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var vm = this.DataContext as SendGroupViewModel;
                if (!vm.Validator.IsValid)
                    MessageBox.Show(string.Join(Environment.NewLine, vm.Validator.ValidationMessages));

                var db = new ProvodnikContext();

                SendGroup p;
                if (vm.Id.HasValue)
                {
                    p = db.SendGroups.Single(pp => pp.Id == vm.Id.Value);

                    MainWindow.Mapper.Value.Map(vm, p);
                    var currents = vm.Persons.Select(pp => pp.PersonId).ToList();
                    var toDelete = (from pd in db.SendGroupPersons
                                    where pd.SendGroupId == p.Id && !currents.Contains(pd.PersonId)
                                    select pd);
                    db.SendGroupPersons.RemoveRange(toDelete);
                    db.SaveChanges();
                }
                else
                {
                    p = new SendGroup();
                    MainWindow.Mapper.Value.Map(vm, p);
                    db.SendGroups.Add(p);
                    db.SaveChanges();
                }

                foreach (var d in vm.Persons)
                {
                    SendGroupPerson pd;
                    if (d.Id == null)
                    {
                        db.SendGroupPersons.Add(pd = new SendGroupPerson() { IsMain = d.IsMain, SendGroupId = p.Id, PersonId = d.PersonId });
                        db.SaveChanges();
                    }
                }
                DialogResult = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при сохранении" + Environment.NewLine + ex.Message);
            }
        }


        private void PersonsListView_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
                if (PersonsListView.SelectedItem != null &&
                MessageBox.Show("Удалить?", "Подтверждение удаления", MessageBoxButton.OKCancel, MessageBoxImage.Question) == MessageBoxResult.OK)
                {
                    var vm = this.DataContext as SendGroupViewModel;
                    var p = PersonsListView.SelectedItem as SendGroupPersonViewModel;
                    vm.Persons.Remove(p);                    
                }
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {

        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            //var vm = this.DataContext as SendGroupViewModel;
            //vm.IsChanged = false;
            DialogResult = false;
        }

        private void PrintButton_Click(object sender, RoutedEventArgs e)
        {
            var vm = this.DataContext as SendGroupViewModel;
            if (!vm.Validator.IsValid)
            {
                MessageBox.Show(string.Join(Environment.NewLine, vm.Validator.ValidationMessages));
                return;
            }

            var dialog = new Ookii.Dialogs.Wpf.VistaFolderBrowserDialog();
            if (dialog.ShowDialog(this).GetValueOrDefault())
            {
                otchetDir = dialog.SelectedPath + @"\" + string.Format(@"{0}_{1}__{2}_{3}",
                vm.OtprDat.Value.ToString("dd.MM.yyyy"),
                "Новосибирск",
                vm.City,
                vm.Persons.Count);

                DirectoryInfo di =new DirectoryInfo(otchetDir);
                if (di.Exists)
                    try
                    {
                        di.Delete(true);
                    }
                    catch (Exception exx)
                    {
                        MessageBox.Show("Невозможно удалить существующую папку "+otchetDir
                            +Environment.NewLine+"закройте открытые документы");
                        return;
                    }

                App.setCursor(true);
                try
                {
                    VSOP1();// VSOP2();
                    VSOP3();
                    F6();
                    PismoLgoti();/**/
                    ReestrPeredachi();

                    string path = otchetDir + $@"\Ф6(Архив)_{vm.OtprDat.Value.ToString("dd.MM.yyyy")}_Новосибирск-{vm.City}_{vm.Persons.Count}чел";
                    if (!Directory.Exists(path))
                        Directory.CreateDirectory(path);
                    else;//TODO del?


                    var ids = vm.Persons.Select(pp => pp.PersonId).ToList();
                    var db = new ProvodnikContext();
                    //foreach (var d in vm.Persons)

                    var qq = (from p in db.Persons
                              join pd in db.PersonDocs on p.Id equals pd.PersonId
                              join pdt in db.DocTypes on pd.DocTypeId equals pdt.Id
                              where ids.Contains(p.Id) && pd.FileName != null
                              group new { pdt.Description, pd.FileName } by p.Fio into gr
                              select gr).ToList();//new { p.Fio, pdt.Description, pd.FileName }

                    using (var client = new FluentFTP.FtpClient())
                    {
                        App.ConfigureFtpClient(client);
                        client.Connect();

                        foreach (var g in qq)
                        {
                            var fio = new StringHelper().ParseFio(g.Key);
                            var fioInic = fio[0];
                            if (fio[1].Length > 0) fioInic += $" {fio[1][0]}.";
                            if (fio[2].Length > 0) fioInic += $" {fio[2][0]}.";

                            foreach (var d in g)
                            {
                                var fileName = $@"{path}\{g.Key}\{fioInic}_{d.Description}.jpg";

                                var result = client.DownloadFile(fileName, d.FileName, FtpLocalExists.Overwrite, FtpVerify.Retry);//, true, FtpVerify.Retry);
                                if (result != FtpStatus.Success)
                                    throw new Exception("Не удалось загрузить файл " + System.IO.Path.GetFileName(fileName));
                            }
                            /*using (WebClient wc = new WebClient() { Credentials = new NetworkCredential(App.CurrentConfig.FtpUser, App.CurrentConfig.FtpPassw) })
                            {
                               wc.DownloadFile("http://u0920601.plsk.regruhosting.ru/" + d.FileName,fileName);
                            bytes = wc.DownloadData("http://u0920601.plsk.regruhosting.ru/" + d.FileName);
                            }*/
                        }
                    }
                }
                catch (Exception ex) { MessageBox.Show(ex.Message, "", MessageBoxButton.OK, MessageBoxImage.Error); }

                App.setCursor(false);
                System.Diagnostics.Process.Start(otchetDir);
            }
        }

        void ListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var pe = ((FrameworkElement)e.OriginalSource).DataContext as SendGroupPersonViewModel;
            if (pe != null)
            {
                var vm = this.DataContext as SendGroupViewModel;
                var pw = new PersonView();
                var db = new ProvodnikContext();
              var pvm=  new PersonViewModel(pe.PersonId);
                pw.DataContext = pvm;
                if (pw.ShowDialog() == true)
                {

                    var ppp = new ProvodnikContext().Persons.First(pp => pp.Id == pe.PersonId);
                    var tmp=    MainWindow.Mapper.Value.Map(ppp, pe);

                    var ind = vm.Persons.IndexOf(pe);
                    vm.Persons.RemoveAt(ind);
                    vm.Persons.Insert(ind, pe);
                    //vm.RefreshPersonList();
                    //TODO goto if exist or add anyway and goto
                }
            }
        }

        private void PersonsListView_Sorting(object sender, DataGridSortingEventArgs e)
        {
            this.Dispatcher.BeginInvoke((Action)delegate ()
            {
                //runs after sorting is done
                Helper.SetPersonShortIndexes(PersonsListView);
            }, null);
        }

        private void PrintVSOP3Button_Click(object sender, RoutedEventArgs e)
        {
            var vm = this.DataContext as SendGroupViewModel;
            if (!vm.Validator.IsValid)
            {
                MessageBox.Show(string.Join(Environment.NewLine, vm.Validator.ValidationMessages));
                return;
            }

            VSOP3(true);
        }
    }
}
