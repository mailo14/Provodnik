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
                .ForMember(dest => dest.Id, opts => opts.MapFrom(src => src.Id));//.ReverseMap();

                cfg.CreateMap<Person, SendGroupPersonViewModel>()
                .ForMember(dest => dest.Id, act => act.Ignore())
                .ForMember(dest => dest.PersonId, opts => opts.MapFrom(src => src.Id));//.ReverseMap();

                cfg.CreateMap<SendGroupPerson, SendGroupPersonViewModel>();

                cfg.CreateMap<PersonShortViewModel, SendGroupPersonViewModel>()
                .ForMember(dest => dest.Id, act => act.Ignore())
                .ForMember(dest => dest.PersonId, opts => opts.MapFrom(src => src.Id));//.ReverseMap();
                cfg.CreateMap<SendGroup, SendGroupViewModel>();//.ForMember(dest => dest.Id, opts => opts.MapFrom(src => src.Id.Value));
                cfg.CreateMap<SendGroupViewModel, SendGroup>().ForMember(dest => dest.Id, act => act.Ignore());//.ReverseMap();

                cfg.CreateMap<UchebGruppaViewModel, ObuchenieViewModel>().ReverseMap();

            });
            var mapper = config.CreateMapper();
            return mapper;
        }
        );


        public MainWindow()
        {
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
            return;

            /*
            var p1 = new Person { Name = "Ivanov", Docs = new List<PersonDocument> { new PersonDocument { TypeId = 1, Name = "111" } } };
            var p2 = ObjectCloner.Clone(p1); //p2.Name = "Petrov";p2.Docs[0].Name = "222";


            //var eq=(new List<Person> { new Person() { Name = p1.Name } }).SequenceEqual(new List<Person> { p1 });
            var l1 = (new List<Person> { new Person() { Name = p1.Name } });
            var l2=new List<Person> { p1 };
            var e1 = l1.Except(l2);
            var e2 = l2.Except(l1);


            string prefix = "Person";
            var pr=GetProperty<Person>(x => x.Name);
            var value1 = pr.GetValue(p1).ToString();
            var value2 = pr.GetValue(p2).ToString();
            if (value1 != value2)
            {
                string msg = $"{prefix} {pr.Name} from {value1} to {value2}";
            }
            for (int i=0;i<p1.Docs.Count;i++)
            {
                var prd = GetProperty<PersonDocument>(x => x.Name);
                var dvalue1 = prd.GetValue(p1.Docs[i]).ToString();
                var dvalue2 = prd.GetValue(p2.Docs[i]).ToString();
                if (dvalue1 != dvalue2)
                {
                    string msg = $"{prefix} +getdoctypename+ {prd.Name} (vs attrib) from {dvalue1} to {dvalue2}";
                }
            }

            var type = p1.GetType();
            //var res = new List<IEnumerable<T>>();
            foreach (var p in type.GetProperties())
            {
                // is IEnumerable<T>?
                // if (typeof(Enumerable).IsAssignableFrom(prop.PropertyType))
                //if (prop.PropertyType.GetInterfaces().Contains(typeof(IEnumerable)))
                if(p.PropertyType.IsGenericType
    //&& p.PropertyType.GetGenericTypeDefinition().Equals(typeof(ICollection<>))
    )
                {
                    var st = p.PropertyType.GenericTypeArguments[0];
                 var st2= p.PropertyType.GetGenericTypeDefinition();



                }
            }
            */
            // return;



            using (var client = new FtpClient("31.31.196.80", new System.Net.NetworkCredential("u0920601", "XP83yno_")))
            {
                {
                    client.Connect();
                    List<string> files = new List<string>() { @"C:\Users\Tc\Pictures\Рисунок (11)eee.JPG" };
                    var remoteDir = "/1_Иванов";
                    /* foreach (var f in files)
                     {
                         if (client.FileExists())
                     }*/

                    client.UploadFiles(files, remoteDir, FtpExists.Overwrite, true, FtpVerify.Retry);
                    /*/ upload a file
                               client.UploadFile(@"C:\MyVideo.mp4", "/htdocs/MyVideo.mp4");

                               // rename the uploaded file
                               client.Rename("/htdocs/MyVideo.mp4", "/htdocs/MyVideo_2.mp4");

                               // download the file again
                               client.DownloadFile(@"C:\MyVideo_2.mp4", "/htdocs/MyVideo_2.mp4");

                               // delete the file
                               client.DeleteFile("/htdocs/MyVideo_2.mp4");

                               // delete a folder recursively
                               client.DeleteDirectory("/htdocs/extras/");

                               // check if a file exists
                               if (client.FileExists("/htdocs/big2.txt")) { }

                               // check if a folder exists
                               if (client.DirectoryExists("/htdocs/extras/")) { }

                               // upload a file and retry 3 times before giving up
                               client.RetryAttempts = 3;
                               client.UploadFile(@"C:\MyVideo.mp4", "/htdocs/big.txt", FtpExists.Overwrite, false, FtpVerify.Retry);*/
                    // 
                    //Stream stream = new MemoryStream(); 
                    //client.Download(stream, remoteDir + "/" + "Рисунок (11)eee.JPG");
                    byte[] data;
                    client.Download(out data, remoteDir + "/" + "Рисунок (11)eee.JPG");
                    /*    using (MemoryStream stream = new MemoryStream(byteArray))
                    {
                        image.Source = BitmapFrame.Create(stream,
                                                          BitmapCreateOptions.None,
                                                          BitmapCacheOption.OnLoad);
                    }*/
                    /* byte[] b1 = null, b2 = null,b3=null,b4=null;
                     using (var stream = new MemoryStream(data))
                     {
                         var bmf = BitmapFrame.Create(stream,
                                                          BitmapCreateOptions.None,
                                                          BitmapCacheOption.OnLoad);
                         image1.Source = bmf;
                         var image = image1.Source as BitmapSource;
                         BitmapEncoder encoder = new JpegBitmapEncoder();
                         encoder.Frames.Add(BitmapFrame.Create(image));
                         using (MemoryStream ms = new MemoryStream())
                         {
                             encoder.Save(ms);
                             b1 = ms.ToArray();
                         }
                         b3 = ToByteArray(image1.Source as  BitmapSource);
                     }

                     Uri uri = new Uri(files[0]);
                     BitmapImage bitmap = new BitmapImage(uri);
                     image1.Source = bitmap;
                     {
                         var image = image1.Source as BitmapSource;
                         BitmapEncoder encoder = new JpegBitmapEncoder();
                         encoder.Frames.Add(BitmapFrame.Create(image));
                         using (MemoryStream ms = new MemoryStream())
                         {
                             encoder.Save(ms);
                             b2 = ms.ToArray();
                         }

                         b4 = ToByteArray(image1.Source as BitmapSource);
                     }*/



                    foreach (FtpListItem item in client.GetListing(remoteDir))
                    {

                        // if this is a file
                        if (item.Type == FtpFileSystemObjectType.File)
                        {

                            // get the file size
                            long size = client.GetFileSize(item.FullName);

                        }

                        // get modified date/time of the file or folder
                        DateTime time = client.GetModifiedTime(item.FullName);

                        // calculate a hash for the file on the server side (default algorithm)
                        // FtpHash hash = client.GetHash(item.FullName);

                    }
                }
            }
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
    }
}
