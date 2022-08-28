using AutoMapper;
using FluentFTP;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Provodnik
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static MainWindow p;

        /*   public class Person:IEquatable<Person>
          {
              public string Name { get; set; }
              public List<PersonDocument> Docs { get; set; }

         public bool Equals(Person other)
              {
                  if (other == null)
                      return false;
                  return Name == other.Name;
              } 
             public override bool Equals(Object obj)
              {
                  if (obj == null)
                      return false;

                  Person personObj = obj as Person;
                  if (personObj == null)
                      return false;
                  else
                      return Equals(personObj);
              }

              public override int GetHashCode()
              {
                  return (Name).GetHashCode();
              }
          }*/

        private static string GetPropertyName<TPropertySource>
        (Expression<Func<TPropertySource, object>> expression)
        {
            var lambda = expression as LambdaExpression;
            MemberExpression memberExpression;
            if (lambda.Body is UnaryExpression)
            {
                var unaryExpression = lambda.Body as UnaryExpression;
                memberExpression = unaryExpression.Operand as MemberExpression;
            }
            else
            {
                memberExpression = lambda.Body as MemberExpression;
            }

            Debug.Assert(memberExpression != null,
               "Please provide a lambda expression like 'n => n.PropertyName'");

            if (memberExpression != null)
            {
                var propertyInfo = memberExpression.Member as PropertyInfo;

                return propertyInfo.Name;
            }

            return null;
        }
        private static PropertyInfo GetProperty<TPropertySource>
        (Expression<Func<TPropertySource, object>> expression)
        {
            var lambda = expression as LambdaExpression;
            MemberExpression memberExpression;
            if (lambda.Body is UnaryExpression)
            {
                var unaryExpression = lambda.Body as UnaryExpression;
                memberExpression = unaryExpression.Operand as MemberExpression;
            }
            else
            {
                memberExpression = lambda.Body as MemberExpression;
            }

            Debug.Assert(memberExpression != null,
               "Please provide a lambda expression like 'n => n.PropertyName'");

            if (memberExpression != null)
            {
                var propertyInfo = memberExpression.Member as PropertyInfo;

                return propertyInfo;
            }

            return null;
        }

        public static Lazy<IMapper> Mapper =
        new Lazy<IMapper>(() =>
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Person, PersonViewModel>().ForMember(dest => dest.Documents, act => act.Ignore());//.ForMember(dest => dest.Id, opts => opts.MapFrom(src => src.Id)); ;//.ForMember(dest => dest.Id, act => act.Ignore());//.ReverseMap();
                cfg.CreateMap<PersonViewModel, Person>().ForMember(dest => dest.Id, act => act.Ignore());//.ReverseMap();

                cfg.CreateMap<Person, PersonShortViewModel>()//.ForMember(dest => dest.Id, act => act.Ignore())
                .ForMember(dest => dest.Id, opts => opts.MapFrom(src => src.Id))
                .ForMember(dest => dest.Index, act => act.Ignore());//.ReverseMap();

                cfg.CreateMap<Person, SendGroupPersonViewModel>()
                .ForMember(dest => dest.Id, act => act.Ignore())
                .ForMember(dest => dest.PersonId, opts => opts.MapFrom(src => src.Id));//.ReverseMap();

                cfg.CreateMap<SendGroupPerson, SendGroupPersonViewModel>();

                cfg.CreateMap<PersonShortViewModel, SendGroupPersonViewModel>()
                .ForMember(dest => dest.Id, act => act.Ignore())
                .ForMember(dest => dest.PersonId, opts => opts.MapFrom(src => src.Id));//.ReverseMap();
                cfg.CreateMap<SendGroup, SendGroupViewModel>();//.ForMember(dest => dest.Id, opts => opts.MapFrom(src => src.Id.Value));
                cfg.CreateMap<SendGroup, SendGroupShortViewModel>();
                cfg.CreateMap<SendGroupViewModel, SendGroup>().ForMember(dest => dest.Id, act => act.Ignore());//.ReverseMap();




                cfg.CreateMap<Person, MedKomZayavkaPersonViewModel>()
                .ForMember(dest => dest.Id, act => act.Ignore())
                .ForMember(dest => dest.PersonId, opts => opts.MapFrom(src => src.Id));//.ReverseMap();

                cfg.CreateMap<MedKomZayavkaPerson, MedKomZayavkaPersonViewModel>();

                cfg.CreateMap<PersonShortViewModel, MedKomZayavkaPersonViewModel>()
                .ForMember(dest => dest.Id, act => act.Ignore())
                .ForMember(dest => dest.PersonId, opts => opts.MapFrom(src => src.Id));//.ReverseMap();
                cfg.CreateMap<MedKomZayavka, MedKomZayavkaViewModel>();//.ForMember(dest => dest.Id, opts => opts.MapFrom(src => src.Id.Value));
                cfg.CreateMap<MedKomZayavka, MedKomZayavkaShortViewModel>();
                cfg.CreateMap<MedKomZayavkaViewModel, MedKomZayavka>().ForMember(dest => dest.Id, act => act.Ignore());//.ReverseMap();


                cfg.CreateMap<UchebGruppaViewModel, ObuchenieViewModel>().ReverseMap();

            });
            var mapper = config.CreateMapper();
            return mapper;
        }
        );


        public MainWindow()
        {

           Patch_ExecOnceThanDelete();


            p = this;
            /*Task.Run(() =>
            {
                using (var dbContext = new ProvodnikContext())
                {
                    var model = dbContext.Persons.FirstOrDefault(); //force the model creation
                }
            });*/
            // Person pyy = null;
            //var rrr=pyy.Equals(new Person());

            InitializeComponent();

            var psw = new PersonsViewPage();
            var pswVm = psw.DataContext as PersonsViewModel;
            pswVm.ReadyOnly = null;
            pswVm.FindCommand.Execute(null);

            MainFrame.Navigate(psw);

            CheckAlarms();
        }

        private void Patch_ExecOnceThanDelete()
        {
            return;
             
            
          /*  using (var client = new FluentFTP.FtpClient())
            {
                App.ConfigureFtpClient(client);

                client.Connect();

                var remotePath = $@"ProvodnikDocs/";
                foreach (FtpListItem item in client.GetListing(remotePath))
                {
                    if (item.Type == FtpFileSystemObjectType.Directory)
                    {
                        var personId = int.Parse( item.Name);
                        if (db.Persons.FirstOrDefault(x => x.Id == personId) == null)
                        {
                            client.DeleteDirectory(remotePath + item.Name);
                        }
                    }
                }
            }*/

            return;

            /*using (var db = new ProvodnikContext())
            {
                 foreach (var p in db.Persons)
                  {
                      //db.PersonDocs.Add(new PersonDoc() { PersonId = p.Id, IsActive = true, DocTypeId = DocConsts.СвидетельствоВакцинации });
 //                     if (!new ProvodnikContext().PersonDocs.Any(x=>x.PersonId==p.Id && x.DocTypeId == DocConsts.СвидетельствоВакцинации2))
 //                         db.PersonDocs.Add(new PersonDoc() { PersonId = p.Id, IsActive = true, DocTypeId = DocConsts.СвидетельствоВакцинации2 });
                    //db.PersonDocs.Add(new PersonDoc() { PersonId = p.Id, IsActive = true, DocTypeId = DocConsts.ТрудоваяКнижка1});
                    //db.PersonDocs.Add(new PersonDoc() { PersonId = p.Id, IsActive = true, DocTypeId = DocConsts.ТрудоваяКнижка2 });

                }
                db.SaveChanges();
            }*/
        }

        public static byte[] ToByteArray(BitmapSource bitmapSource)
        {

            var encoder = new JpegBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(bitmapSource));

            using (var stream = new MemoryStream())
            {
                encoder.Save(stream);
                return stream.ToArray();
            }
        }

        public static BitmapSource ToBitmapSource(byte[] bytes)
        {

            using (var stream = new MemoryStream(bytes))
            {
                var decoder = new JpegBitmapDecoder(stream, BitmapCreateOptions.None, BitmapCacheOption.Default);
                return decoder.Frames.First();
            }
        }
        /**/
        private void Button_Click(object sender, RoutedEventArgs e)
        {//new SendGroupsView().ShowDialog();
         // new PersonsView(null).ShowDialog();
            new Sanknizki().ShowDialog();
        }

        private void SanknizkiMenuItem_Click(object sender, RoutedEventArgs e)
        {
            new Sanknizki().ShowDialog();
            (MainFrame.Content as PersonsViewPage).vm.FindCommand.Execute(null);
        }

        private void PsihOsvidsMenuItem_Click(object sender, RoutedEventArgs e)
        {
            new PsihOsvids().ShowDialog();
            (MainFrame.Content as PersonsViewPage).vm.FindCommand.Execute(null);
        }

        private void MedKommsMenuItem_Click(object sender, RoutedEventArgs e)
        {
            new MedKomms().ShowDialog();
            (MainFrame.Content as PersonsViewPage).vm.FindCommand.Execute(null);
        }

        private void PraktikasMenuItem_Click(object sender, RoutedEventArgs e)
        {
            new Praktikas().ShowDialog();
            (MainFrame.Content as PersonsViewPage).vm.FindCommand.Execute(null);
        }

        private void ExamensMenuItem_Click(object sender, RoutedEventArgs e)
        {
            new Examens().ShowDialog();
            (MainFrame.Content as PersonsViewPage).vm.FindCommand.Execute(null);
        }

        private void SendGroupsMenuItem_Click(object sender, RoutedEventArgs e)
        {
            new SendGroupsView().ShowDialog();
            (MainFrame.Content as PersonsViewPage).vm.FindCommand.Execute(null);
        }

        private void PersonsMenuItem_Click(object sender, RoutedEventArgs e)
        {
            var psw = new PersonsView(null);
            var pswVm = psw.DataContext as PersonsViewModel;
            pswVm.ReadyOnly = null;
            pswVm.FindCommand.Execute(null);//psw.FindButton_Click(null, null);

            psw.ShowDialog();
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            new AboutBox1().ShowDialog();
        }

        private async void TestMenuItem_Click(object sender, RoutedEventArgs e)
        {
            var db = new ProvodnikContext();
            foreach (var pe in db.Persons.Where(pp=>pp.UchebCentr==null).ToList())
            {
                var pvm = new PersonViewModel(pe.Id,false);
                var m = pvm.Messages;
                var ap = pvm.AllPasport;
                var asc = pvm.AllScans;
                pvm.FillMessagesAndAlls(pe);
                bool m1=false, m2 = false, m3 = false;
                if (m != pvm.Messages)
                    m1=true;
                if (ap != pvm.AllPasport)
                    m2 = true;

                if (asc != pvm.AllScans)
                    m3 = true;
                if (m1 || m2 || m3)
                    db.SaveChanges();
            }
            return;

            /*     FactorialAsync(-4);
                 FactorialAsync(6);
                 return;*/
            try
            {
                // await new ProgressRunner().RunAsync(doAction);      
                await new ProgressRunner().RunAsync(
                    new Action<ProgressHandler>((progressChanged) => {

                        progressChanged(1, "Сохранение полей");
                        Thread.Sleep(1000);
                        progressChanged(30, "Загрузка сканов");
                        Thread.Sleep(1000); //throw new Exception("Ffddffd");
                        Thread.Sleep(1000);
                        progressChanged(60, "Загрузка сканов");
                        Thread.Sleep(1000);
                        progressChanged(80, "Загрузка сканов");
                        progressChanged(100, "");
                    })


                    );        
            }
            catch (Exception ex){ MessageBox.Show(ex.Message); }

          /*  return;
            var runner = new ProgressRunner();
            var r =await runner.Run(doAction);
            if (!r) MessageBox.Show(runner.ex.Message);
            try
             {
                 new ProgressRunner().Run(doAction);
             }catch(Exception exep)
                 {
                     MessageBox.Show(exep.Message);

                    // throw exep;              
                 }*/
        }

        private  void doAction(ProgressHandler progressChanged)
        {
           
            progressChanged(1, "Сохранение полей");
        Thread.Sleep(1000);
            progressChanged(30, "Загрузка сканов");
        Thread.Sleep(1000); //throw new Exception("Ffddffd");
        Thread.Sleep(1000);
            progressChanged(60, "Загрузка сканов");
        Thread.Sleep(1000);
            progressChanged(80, "Загрузка сканов");
            progressChanged(100,"");
        }

        private void AlarmsMenuItem_Click(object sender, RoutedEventArgs e)
        {
            new AlarmsView().ShowDialog();
            CheckAlarms();
        }
        public void CheckAlarms()
        {
            using (var db = new ProvodnikContext())
            {
                var qq = new Repository().GetAlarmsCount();
                AlarmsMenuItem.Visibility = (qq > 0) ? Visibility.Visible : Visibility.Collapsed;
                AlarmsCountTextBlock.Text = qq.ToString();
            }
        }

        private void ObucheniyasMenuItem_Click(object sender, RoutedEventArgs e)
        {
            new Obucheniyas().ShowDialog();
            (MainFrame.Content as PersonsViewPage).vm.FindCommand.Execute(null);
        }

        private void InstrMenuItem_Click(object sender, RoutedEventArgs e)
        {
                System.Diagnostics.Process.Start("Инструкция Проводник.docx");
        }

        private void MedKomZayavkiMenuItem_Click(object sender, RoutedEventArgs e)
        {
            new MedKomZayavkiView().ShowDialog();
        }
    }
}
