using ReactiveValidation;
using ReactiveValidation.Attributes;
using ReactiveValidation.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Provodnik
{
    public class PersonViewModel : ValidatableObject
    {

        public static BitmapSource ToBitmapSource(byte[] bytes)
        {

            using (var stream = new System.IO.MemoryStream(bytes))
            {
                var decoder = new JpegBitmapDecoder(stream, BitmapCreateOptions.None, BitmapCacheOption.Default);
                return decoder.Frames.First();
            }
        }
        Repository repository = new Repository();

        public PersonViewModel(int? personId, bool loadBitmaps = true)
        {
            Validator = GetValidator();

            // Load(personId);
            var db = new ProvodnikContext();

            Cities = repository.GetCities();
            VibilPrichini = db.Persons.Select(pp => pp.VibilPrichina).Distinct().ToList();
            Otryadi = db.Persons.Select(pp => pp.Otryad).Distinct().ToList();
            UchebCentri = repository.GetUchebCentri();
            Grazdanstva = repository.GetGrazdanstva();
            Sezons = repository.GetSezons();


            Documents = new ObservableCollection<PersonDocViewModel>();

            UchZavedeniya = repository.GetUchZavedeniya();
            UchFormas = new ObservableCollection<string>();/**/
                                                           //UchFacs = new ObservableCollection<string>();
                                                           //progressChanged(5);
            if (personId.HasValue)
            {
                var qq = (from pd in db.PersonDocs
                          join dt in db.DocTypes on pd.DocTypeId equals dt.Id
                          where pd.PersonId == personId.Value
                          select new PersonDocViewModel() { Id = pd.Id, DocTypeId = pd.DocTypeId, Description = dt.Description, FileName = pd.FileName, PrinesetK = pd.PrinesetK, Bitmap = new System.Windows.Controls.Image() }).ToList();
                //progressChanged(5);
                var progressShare = 80.0 / qq.Count;
                foreach (var d in qq)
                {
                    if (loadBitmaps && d.FileName != null)
                        LoadFile(d);
                    this.Documents.Add(d);
                    // progressChanged(progressShare);
                }

                MainWindow.Mapper.Value.Map(db.Persons.Single(pp => pp.Id == personId.Value), this);//must be after docs loaded
                                                                                                    // progressChanged(10);
            }
            else
            {
                //  progressChanged(45);
                if (UchebCentri.Count == 1)
                    UchebCentr = UchebCentri[0];
                Grazdanstvo = Grazdanstva[0];
                Sezon = Sezons[0];
                ReCreateDocs();
                //foreach (var d in db.DocTypes.Where(pp => pp.IsObyazat).ToList())
                //    Documents.Add(new PersonDocViewModel() { Description = d.Description, DocTypeId = d.Id, Bitmap = new System.Windows.Controls.Image() });
                //  progressChanged(50);
            }
            //  LoadPersonViewModelAsync(personId);
            var yearStart = DateTime.Today;
            yearStart = yearStart.AddDays(-yearStart.Day + 1).AddMonths(-yearStart.Month + 1);
            UchebGruppas = (from p in db.Persons
                            where p.UchebStartDat > yearStart && p.UchebGruppa != null
                            select new { p.UchebGruppa, p.UchebStartDat, p.UchebEndDat, p.UchebCentr }).Distinct()
                               .Select(pp => new UchebGruppaViewModel()
                               {
                                   UchebCentr = pp.UchebCentr,
                                   UchebGruppa = pp.UchebGruppa,
                                   UchebStartDat = pp.UchebStartDat,
                                   UchebEndDat = pp.UchebEndDat
                               }).ToList();
            var existed = UchebGruppas.FirstOrDefault(pp => pp.UchebGruppa == UchebGruppa && pp.UchebCentr == UchebCentr && pp.UchebStartDat == UchebStartDat);
            if (UchebGruppa != null && existed == null)
            {
                UchebGruppas.Insert(0, existed = new UchebGruppaViewModel() { UchebGruppa = UchebGruppa, UchebCentr = UchebCentr, UchebStartDat = UchebStartDat, UchebEndDat = UchebEndDat });
            }
            //UchebGruppas.Insert(0, new UchebGruppaViewModel {UchebGruppa="нет" });
            //if (existed == null)                existed = UchebGruppas[0];
            SelectedUchebGruppa = existed;
            //UchebGruppa UchebEndDat UchebStartDat UchebCentr
            IsLoading = false;
        }

        public bool GetAllPasport()
        {
            var allPasports = new string[]{ Fio,Phone,UchZavedenie,UchForma,Grazdanstvo//,Otryad
                ,UchebCentr,(ExamenDat.HasValue) ? ExamenDat.ToString(): UchebEndDat.ToString()//TODO
                ,RodPhone,RodFio
                ,BirthDat.ToString(),MestoRozd,PaspNomer,PaspSeriya,PaspAdres,Snils };
            return !allPasports.Any(pp => string.IsNullOrWhiteSpace(pp));
        }

        public List<string> GetModelErrors()
        {
            List<string> errors = new List<string>();
            if (!Validator.IsValid)
            {
                foreach (var ve in Validator.ValidationMessages) errors.Add(ve.Message);
            }

            return errors;
        }

        public class DocExist
        {
            public DocExist(int docTypeId, bool docExist)
            {
                DocTypeId = docTypeId;
                Exist = docExist;
            }

            public int DocTypeId { get; set; }
            public bool Exist { get; set; }
        }

        public List<string> GetScanErrors(bool checkOnlyFilename)
        {
            List<string> errors = new List<string>();
            //var ddd = Documents.Select(pp => new DocExist( pp.DocTypeId, pp.Bitmap.Source != null )).ToList();
            foreach (var d in Documents)
            {
                bool emptyScan;
                if (checkOnlyFilename)
                    emptyScan = d.FileName == null;
                else
                {
                    ImageSource source = null;
                    Application.Current.Dispatcher.Invoke((Action)(() =>
                    {
                        source = d.Bitmap.Source;
                    }));
                    emptyScan = source == null;
                }

                if (emptyScan)
                {
                    bool voenPripisEmpty = false;
                    if (d.DocTypeId == DocConsts.ВоенныйБилет)
                    {
                        Application.Current.Dispatcher.Invoke((Action)(() =>
                        {
                            var emptyPripisnoe = (from pdd in Documents
                                                  where (pdd.DocTypeId == DocConsts.Приписное1 || pdd.DocTypeId == DocConsts.Приписное2)
                                                  && (checkOnlyFilename && pdd.FileName == null || !checkOnlyFilename && pdd.Bitmap.Source == null)
                                                  select pdd);
                            if (emptyPripisnoe.Any())
                                voenPripisEmpty = true;
                        }));
                    }
                    else
                    if (d.DocTypeId == DocConsts.Приписное1 || d.DocTypeId == DocConsts.Приписное2)
                    {
                        Application.Current.Dispatcher.Invoke((Action)(() =>
                        {
                            var emptyVoennik = (from pdd in Documents
                                                where pdd.DocTypeId == DocConsts.ВоенныйБилет
                                                && (checkOnlyFilename && pdd.FileName == null || !checkOnlyFilename && pdd.Bitmap.Source == null)
                                                select pdd);
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

        public void FillMessagesAndAlls(Person p)
        {
            var scanErrors = GetScanErrors(true);
            var allErrors = GetModelErrors(); allErrors.AddRange(scanErrors);

            Messages = p.Messages = string.Join(Environment.NewLine, allErrors);
            AllPasport = p.AllPasport = GetAllPasport();
            AllScans = p.AllScans = !scanErrors.Any();
        }

        bool IsLoading = true;
        private UchebGruppaViewModel _SelectedUchebGruppa;
        public UchebGruppaViewModel SelectedUchebGruppa
        {
            get => _SelectedUchebGruppa;
            set
            {
                if (value != null && value.UchebCentr == null) value = null;
                if (_SelectedUchebGruppa != value)
                {
                    _SelectedUchebGruppa = value;
                    IsLoading = true;
                    if (value != null)
                    {
                        UchebGruppa = value.UchebGruppa;
                        UchebCentr = value.UchebCentr;
                        UchebStartDat = value.UchebStartDat;
                        //UchebEndDat = value.UchebEndDat;
                    }
                    else
                    {
                        UchebGruppa = null;
                        // UchebGruppa = UchebCentr = null;
                        // UchebStartDat = UchebEndDat = null;
                    }
                    IsLoading = false;
                    OnPropertyChanged();
                }
            }
        }
        private async void LoadFile(PersonDocViewModel d)
        {
            d.Bitmap.Source = new BitmapImage(new Uri("pack://application:,,,/pic/loading.gif"));

            byte[] bytes = null;
            try
            {
                await System.Threading.Tasks.Task.Run(
                    () =>
                    {
                        using (var client = new FluentFTP.FtpClient())
                        {
                            App.ConfigureFtpClient(client);
                            client.Connect();
                            client.Download(out bytes, d.FileName);
                            //Thread.Sleep(2000);
                        }
                    });

                //d.Bitmap.Source = ToBitmapSource(bytes);
                using (var stream = new System.IO.MemoryStream(bytes))
                {
                    var bmf = BitmapFrame.Create(stream,
                                                     BitmapCreateOptions.None,
                                                     BitmapCacheOption.OnLoad);
                    d.Bitmap.Source = bmf;
                }
            }
            catch (Exception oex)
            {
                MessageBox.Show(oex.Message + " " + d.Description + " " + d.FileName);
            }
        }

        public async void Load(int? personId)
        {
            List<PersonDocViewModel> qq = null;
            await new ProgressRunner().RunAsync(
                                    new Action<ProgressHandler>((progressChanged) =>
                                    {

                                        var db = new ProvodnikContext();
                                        var pp = (from pd in db.PersonDocs
                                                  join dt in db.DocTypes on pd.DocTypeId equals dt.Id
                                                  select new { pd, dt }).ToList();
                              //return;
                              qq = (from pd in db.PersonDocs
                                              join dt in db.DocTypes on pd.DocTypeId equals dt.Id
                                              where pd.PersonId == personId.Value
                                              select new PersonDocViewModel()
                                              {
                                                  Id = pd.Id,
                                                  DocTypeId = pd.DocTypeId,
                                                  Description = dt.Description,
                                                  FileName = pd.FileName,
                                                  PrinesetK = pd.PrinesetK
                                        // , Bitmap = new System.Windows.Controls.Image()
                                    }).ToList();

                                    }));
            Fio = string.Join(" ", qq.Select(pp => pp.Description));
        }
        /*   public  void LoadPersonViewModel(ProgressHandler progressChanged)//int? _personId)
           {
                             var personId = 1 as int?;//
              // _personId;


           }

       public async void LoadPersonViewModelAsync(int? _personId)
       {
           await new ProgressRunner().RunAsync(LoadPersonViewModel);
           }*/


        public void ReCreateDocs()
        {
            Documents.Clear();

            var db = new ProvodnikContext();
            foreach (var d in db.DocTypes.Where(pp => pp.IsObyazat).ToList())
                Documents.Add(new PersonDocViewModel() { Description = d.Description, DocTypeId = d.Id, Bitmap = new System.Windows.Controls.Image() });
            string prev;
            if (!string.IsNullOrWhiteSpace(Pol)) { prev = Pol; Pol = null; Pol = prev; }
            if (!string.IsNullOrWhiteSpace(Grazdanstvo)) { prev = Grazdanstvo; Grazdanstvo = null; Grazdanstvo = prev; }
            if (!string.IsNullOrWhiteSpace(UchZavedenie)) { prev = UchZavedenie; UchZavedenie = null; UchZavedenie = prev; }
        }

        private IObjectValidator GetValidator()
        {
            var builder = new ValidationBuilder<PersonViewModel>();

            builder.RuleFor(vm => vm.Fio).MyNotEmpty();
            builder.RuleFor(vm => vm.BadgeRus).MyNotEmpty(); builder.RuleFor(vm => vm.BadgeEng).MyNotEmpty();
            builder.RuleFor(vm => vm.Pol).MyNotEmpty();
            builder.RuleFor(vm => vm.Grazdanstvo).MyNotEmpty();
            builder.RuleFor(vm => vm.Phone).MyNotEmpty()
                .Matches(@"^\d{10}$").WithMessage("{PropertyName} должен содержать 10 цифр");
            builder.RuleFor(vm => vm.Vk).MyNotEmpty();
            builder.RuleFor(vm => vm.Dogovor).MyNotEmpty();
            builder.RuleFor(vm => vm.DogovorDat).MyNotEmptyDat();
            builder.RuleFor(vm => vm.UchZavedenie).MyNotEmpty();
            builder.RuleFor(vm => vm.UchForma).MyNotEmpty();
            // .When(vm => UchZavedenie, uchZavedenie => !string.IsNullOrEmpty(uchZavedenie));
            //builder.RuleFor(vm => vm.UchFac).MyNotEmpty()
            //    .When(vm => UchZavedenie, uchZavedenie => !string.IsNullOrEmpty(uchZavedenie) && uchZavedenie!= "не учится");
            builder.RuleFor(vm => vm.UchGod).MyNotEmpty()
                .When(vm => UchZavedenie, uchZavedenie => !string.IsNullOrEmpty(uchZavedenie) && uchZavedenie != RepoConsts.NoUchZavedenie).Between("1950", "2050");

            builder.RuleFor(vm => vm.RodFio).MyNotEmpty();
            builder.RuleFor(vm => vm.RodPhone).MyNotEmpty()
                .Matches(@"^\d{10}$").WithMessage("{PropertyName} должен содержать 10 цифр");
            builder.RuleFor(vm => vm.RazmerFormi).MyNotEmpty()
                .When(vm => vm.HasForma, hasForma => !hasForma);

            builder.RuleFor(vm => vm.BirthDat).MyNotEmptyDat();
            //System.Linq.Expressions.Expression<Func<PersonsViewModel, bool>> NotKazahPpedicate =  vm => vm.Grazdanstvo != "КЗ";
            builder.RuleFor(vm => vm.MestoRozd).MyNotEmpty().When(vm => vm.Grazdanstvo != "КЗ");

            builder.RuleFor(vm => vm.PaspSeriya).MyNotEmpty().When(vm => vm.Grazdanstvo != "КЗ")
                .Matches(@"^\d{4}$").WithMessage("{PropertyName} должна содержать 4 цифры").When(vm => vm.Grazdanstvo != "КЗ");
            builder.RuleFor(vm => vm.PaspNomer).MyNotEmpty();
            builder.RuleFor(vm => vm.PaspNomer).Matches(@"^\d{6}$").WithMessage("{PropertyName} должен содержать 6 цифр").When(vm => vm.Grazdanstvo != "КЗ");
            builder.RuleFor(vm => vm.PaspNomer).Matches(@"^\d{8}$").WithMessage("{PropertyName} должен содержать 8 цифр").When(vm => vm.Grazdanstvo == "КЗ");
            builder.RuleFor(vm => vm.PaspVidan).MyNotEmpty().When(vm => vm.Grazdanstvo != "КЗ");
            builder.RuleFor(vm => vm.VidanDat).MyNotEmptyDat();
            builder.RuleFor(vm => vm.PaspAdres).MyNotEmpty().When(vm => vm.Grazdanstvo != "КЗ");
            builder.RuleFor(vm => vm.FactAdres).MyNotEmpty();

            builder.RuleFor(vm => vm.VremRegDat).MyNotEmptyDat()
                .When(vm => vm.Grazdanstvo, grazdanstvo => grazdanstvo == "КЗ");

            builder.RuleFor(vm => vm.Snils).MyNotEmpty()
                .Matches(@"^\d{11}$").WithMessage("{PropertyName} должен содержать 11 цифр");
            builder.RuleFor(vm => vm.Inn).MyNotEmpty()
                 .Matches(@"^\d{12}$").WithMessage("{PropertyName} должен содержать 12 цифр");
            builder.RuleFor(vm => vm.UchebCentr).MyNotEmpty();
            builder.RuleFor(vm => vm.UchebEndDat).MyNotEmptyDat();
            //builder.RuleFor(vm => vm.UchebGruppa).MyNotEmpty();
            // builder.RuleFor(vm => vm.Gorod).MyNotEmpty();

            builder.RuleFor(vm => vm.VibilPrichina).MyNotEmpty()
                .When(vm => vm.IsVibil, isVibil => isVibil);
            ;//TODO .When(vm => UchZavedenie, uchZavedenie => !string.IsNullOrEmpty(uchZavedenie)).Between("1950", "2050");




            return builder.Build(this);
        }



        public List<string> Cities { get; set; }
        public List<string> VibilPrichini { get; set; }
        public List<string> Otryadi { get; set; }
        public List<string> UchebCentri { get; set; }
        public List<string> Grazdanstva { get; set; }
        public List<string> Sezons { get; set; }


        private static bool IsValidEmail(string email)
        {
            if (string.IsNullOrEmpty(email) == true)
                return true;

            return System.Text.RegularExpressions.Regex.IsMatch(email, @"^\w+@\w+.\w+$");
        }
        public class PersonDocViewModel : System.ComponentModel.INotifyPropertyChanged//: PersonDoc
        {
            public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
            public void OnPropertyChanged([CallerMemberName]string prop = "")
            {
                if (PropertyChanged != null)
                    PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(prop));
            }

            public int? Id { get; set; }
            public string Description { get; set; }
            public string FileName { get; set; }

            public DateTime? _PrinesetK;
            public DateTime? PrinesetK
            {
                get => _PrinesetK;
                set
                {
                    _PrinesetK = value;
                    OnPropertyChanged();
                }
            }

            public System.Windows.Controls.Image _Bitmap;
            public System.Windows.Controls.Image Bitmap
            {
                get => _Bitmap;
                set
                {
                    _Bitmap = value;
                    OnPropertyChanged();
                }
            }
            public int DocTypeId { get; set; }
        }

        public ObservableCollection<PersonDocViewModel> Documents { get; set; }
        private string _Phone;
        [DisplayName(DisplayName = "Телефон")]
        public string Phone
        {
            get => _Phone;
            set
            {
                _Phone = value;
                OnPropertyChanged();
            }
        }
        public int? Id { get; set; }

        private string _Pol;
        [DisplayName(DisplayName = "Пол")]
        public string Pol
        {
            get => _Pol;
            set
            {
                _Pol = value;
                if (Pol == "мужской")
                {
                    if (!Documents.Where(pp => pp.DocTypeId >= 12 && pp.DocTypeId <= 14).Any())
                        foreach (var d in new ProvodnikContext().DocTypes.Where(pp => pp.Id >= 12 && pp.Id <= 14))
                            Documents.Add(new PersonDocViewModel() { Description = d.Description, DocTypeId = d.Id, Bitmap = new Image() });
                }
                else
                    if (Pol == "женский")
                {
                    foreach (var d in Documents.Where(pp => pp.DocTypeId >= 12 && pp.DocTypeId <= 14).ToList())
                        Documents.Remove(d);
                }
                OnPropertyChanged();//TODO all nameof(Pol));
            }
        }

        private string _Fio;
        [DisplayName(DisplayName = "ФИО")]
        public string Fio
        {
            get => _Fio;
            set
            {
                _Fio = value;
                OnPropertyChanged();
            }
        }

        private string _BadgeRus;
        [DisplayName(DisplayName = "Бейдж на русском")]
        public string BadgeRus
        {
            get => _BadgeRus;
            set
            {
                _BadgeRus = value;
                OnPropertyChanged();
            }
        }

        private string _BadgeEng;
        [DisplayName(DisplayName = "Бейдж на английском")]
        public string BadgeEng
        {
            get => _BadgeEng;
            set
            {
                _BadgeEng = value;
                OnPropertyChanged();
            }
        }

        private string _Vk;
        [DisplayName(DisplayName = "Ссылка на ВК")]
        public string Vk
        {
            get => _Vk;
            set
            {
                _Vk = value;
                OnPropertyChanged();
            }
        }

        private string _Sezon;
        [DisplayName(DisplayName = "Сезон")]
        public string Sezon
        {
            get => _Sezon;
            set
            {
                _Sezon = value;
                OnPropertyChanged();
            }
        }

        private string _Grazdanstvo;
        [DisplayName(DisplayName = "Гражданство")]
        public string Grazdanstvo
        {
            get => _Grazdanstvo;
            set
            {
                _Grazdanstvo = value;

                var docIds = new int[] { 17, 18, 19, 20 };
                if (Grazdanstvo == "КЗ")
                {
                    if (!Documents.Where(pp => docIds.Contains(pp.DocTypeId)).Any())
                        foreach (var d in new ProvodnikContext().DocTypes.Where(pp => docIds.Contains(pp.Id)))
                            Documents.Add(new PersonDocViewModel() { Description = d.Description, DocTypeId = d.Id, Bitmap = new System.Windows.Controls.Image() });

                    Documents.Remove(Documents.FirstOrDefault(x => x.DocTypeId == DocConsts.Прописка));
                }
                else
                {
                    foreach (var d in Documents.Where(pp => docIds.Contains(pp.DocTypeId)).ToList())
                        Documents.Remove(d);

                    if (!Documents.Any(x => x.DocTypeId == DocConsts.Прописка))
                    {
                        var propiskaDoc = new ProvodnikContext().DocTypes.First(x => x.Id == DocConsts.Прописка);
                        Documents.Add(new PersonDocViewModel() { Description = propiskaDoc.Description, DocTypeId = propiskaDoc.Id, Bitmap = new System.Windows.Controls.Image() });
                    }
                }

                Validator.Revalidate();
                //TODO миграция Казахи
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsRussia));
            }
        }

        public bool IsRussia => Grazdanstvo != "КЗ";

        private string _Otryad;
        [DisplayName(DisplayName = "Отряд")]
        public string Otryad
        {
            get => _Otryad;
            set
            {
                _Otryad = value;
                OnPropertyChanged();
            }
        }

        private bool _IsNovichok;
        public bool IsNovichok
        {
            get => _IsNovichok;
            set
            {
                _IsNovichok = value;
                OnPropertyChanged();
            }
        }

        private string _Dogovor;
        [DisplayName(DisplayName = "Номер договора")]
        public string Dogovor
        {
            get => _Dogovor;
            set
            {
                _Dogovor = value;
                OnPropertyChanged();
            }
        }

        private DateTime? _DogovorDat;
        [DisplayName(DisplayName = "Дата договора")]
        public DateTime? DogovorDat
        {
            get => _DogovorDat;
            set
            {
                _DogovorDat = value;
                OnPropertyChanged();
            }
        }

        private string _UchZavedenie;

        [DisplayName(DisplayName = "Учебное заведение")]
        public string UchZavedenie
        {
            get => _UchZavedenie;
            set
            {
                _UchZavedenie = value;
                //if (UchFormas != null)
                {
                    UchFormas.Clear(); foreach (var u in repository.GetUchFormas(UchZavedenie)) UchFormas.Add(u);
                    if (UchFormas.Count == 1) UchForma = UchFormas[0];
                    //UchFacs.Clear(); foreach (var u in repository.GetUchFacs(UchZavedenie)) UchFacs.Add(u);

                    if (!string.IsNullOrWhiteSpace(UchZavedenie) && UchZavedenie != RepoConsts.NoUchZavedenie)
                    {
                        if (!Documents.Where(pp => pp.DocTypeId == 15).Any())
                            foreach (var d in new ProvodnikContext().DocTypes.Where(pp => pp.Id == 15))
                                Documents.Add(new PersonDocViewModel() { Description = d.Description, DocTypeId = d.Id, Bitmap = new System.Windows.Controls.Image() });
                    }
                    else
                    {
                        foreach (var d in Documents.Where(pp => pp.DocTypeId == 15).ToList())
                            Documents.Remove(d);
                    }
                }
                OnPropertyChanged();
            }
        }

        private string _UchForma;
        [DisplayName(DisplayName = "Форма обучения")]
        public string UchForma
        {
            get => _UchForma;
            set
            {
                _UchForma = value;
                OnPropertyChanged();
            }
        }

        /*private string _UchFac;
        [DisplayName(DisplayName = "Факультет")]
        public string UchFac
        {
            get => _UchFac;
            set
            {
                _UchFac = value;
                OnPropertyChanged();
            }
        }*/

        private string _UchGod;
        [DisplayName(DisplayName = "Год окончания обучения")]
        public string UchGod
        {
            get => _UchGod;
            set
            {
                _UchGod = value;
                OnPropertyChanged();
            }
        }

        private bool _HasLgota;
        [DisplayName(DisplayName = "Есть льгота")]
        public bool HasLgota
        {
            get => _HasLgota;
            set
            {
                _HasLgota = value;
                OnPropertyChanged();
            }
        }

        private string _RodFio;
        [DisplayName(DisplayName = "ФИО родителя")]
        public string RodFio
        {
            get => _RodFio;
            set
            {
                _RodFio = value;
                OnPropertyChanged();
            }
        }

        private string _RodPhone;
        [DisplayName(DisplayName = "Контактный телефон родителей")]
        public string RodPhone
        {
            get => _RodPhone;
            set
            {
                _RodPhone = value;
                OnPropertyChanged();
            }
        }

        private bool _HasForma;
        public bool HasForma
        {
            get => _HasForma;
            set
            {
                _HasForma = value;
                OnPropertyChanged();
            }
        }

        private string _RazmerFormi;
        [DisplayName(DisplayName = "Размер формы")]
        public string RazmerFormi
        {
            get => _RazmerFormi;
            set
            {
                _RazmerFormi = value;
                OnPropertyChanged();
            }
        }

        private DateTime? _BirthDat;
        [DisplayName(DisplayName = "Дата рождения")]
        public DateTime? BirthDat
        {
            get => _BirthDat;
            set
            {
                _BirthDat = value;
                OnPropertyChanged();
            }
        }

        private string _MestoRozd;
        [DisplayName(DisplayName = "Место рождения")]
        public string MestoRozd
        {
            get => _MestoRozd;
            set
            {
                _MestoRozd = value;
                OnPropertyChanged();
            }
        }

        private string _PaspSeriya;
        [DisplayName(DisplayName = "Серия паспорта")]
        public string PaspSeriya
        {
            get => _PaspSeriya;
            set
            {
                _PaspSeriya = value;
                OnPropertyChanged();
            }
        }

        private string _PaspNomer;
        [DisplayName(DisplayName = "Номер паспорта")]
        public string PaspNomer
        {
            get => _PaspNomer;
            set
            {
                _PaspNomer = value;
                OnPropertyChanged();
            }
        }

        private string _PaspVidan;
        [DisplayName(DisplayName = "Кем выдан")]
        public string PaspVidan
        {
            get => _PaspVidan;
            set
            {
                _PaspVidan = value;
                OnPropertyChanged();
            }
        }

        private DateTime? _VidanDat;
        public DateTime? VidanDat
        {
            get => _VidanDat;
            set
            {
                _VidanDat = value;
                OnPropertyChanged();
            }
        }

        private string _PaspAdres;
        [DisplayName(DisplayName = "Прописка по паспорту")]
        public string PaspAdres
        {
            get => _PaspAdres;
            set
            {
                _PaspAdres = value;
                OnPropertyChanged();
            }
        }

        private DateTime? _VremRegDat;
        [DisplayName(DisplayName = "Дата временной регистрации")]
        public DateTime? VremRegDat
        {
            get => _VremRegDat;
            set
            {
                _VremRegDat = value;
                OnPropertyChanged();
            }
        }

        private string _FactAdres;
        [DisplayName(DisplayName = "Адрес фактического места жительства")]
        public string FactAdres
        {
            get => _FactAdres;
            set
            {
                _FactAdres = value;
                OnPropertyChanged();
            }
        }

        private string _Snils;
        [DisplayName(DisplayName = "Номер ПФ (снилс)")]
        public string Snils
        {
            get => _Snils;
            set
            {
                _Snils = value;
                OnPropertyChanged();
            }
        }

        private string _Inn;
        [DisplayName(DisplayName = "ИНН")]
        public string Inn
        {
            get => _Inn;
            set
            {
                _Inn = value;
                OnPropertyChanged();
            }
        }

        private DateTime? _PsihDat;
        public DateTime? PsihDat
        {
            get => _PsihDat;
            set
            {
                _PsihDat = value;
                OnPropertyChanged();
            }
        }

        private bool _IsPsih;
        public bool IsPsih
        {
            get => _IsPsih;
            set
            {
                _IsPsih = value;
                OnPropertyChanged();
            }
        }

        private bool _IsPsihZabral;
        public bool IsPsihZabral
        {
            get => _IsPsihZabral;
            set
            {
                _IsPsihZabral = value;
                OnPropertyChanged();
            }
        }

        private DateTime? _SanKnizkaDat;
        public DateTime? SanKnizkaDat
        {
            get => _SanKnizkaDat;
            set
            {
                _SanKnizkaDat = value;
                OnPropertyChanged();
            }
        }

        private bool _IsSanKnizka;
        public bool IsSanKnizka
        {
            get => _IsSanKnizka;
            set
            {
                _IsSanKnizka = value;
                OnPropertyChanged();
            }
        }


        private DateTime? _NaprMedZakazanoDat;
        public DateTime? NaprMedZakazanoDat
        {
            get => _NaprMedZakazanoDat;
            set
            {
                _NaprMedZakazanoDat = value;
                OnPropertyChanged();
            }
        }

        private bool _IsNaprMedZakazano;
        public bool IsNaprMedZakazano
        {
            get => _IsNaprMedZakazano;
            set
            {
                _IsNaprMedZakazano = value;
                OnPropertyChanged();
            }
        }


        private DateTime? _NaprMedPoluchenoDat;
        public DateTime? NaprMedPoluchenoDat
        {
            get => _NaprMedPoluchenoDat;
            set
            {
                _NaprMedPoluchenoDat = value;
                OnPropertyChanged();
            }
        }

        private bool _IsNaprMedPolucheno;
        public bool IsNaprMedPolucheno
        {
            get => _IsNaprMedPolucheno;
            set
            {
                _IsNaprMedPolucheno = value;
                OnPropertyChanged();
            }
        }

        private bool _IsNaprMedPoluchenoNePoln;
        public bool IsNaprMedPoluchenoNePoln
        {
            get => _IsNaprMedPoluchenoNePoln;
            set
            {
                _IsNaprMedPoluchenoNePoln = value;
                OnPropertyChanged();
            }
        }

        private bool _IsNaprMedPoluchenoSOshibkoi;
        public bool IsNaprMedPoluchenoSOshibkoi
        {
            get => _IsNaprMedPoluchenoSOshibkoi;
            set
            {
                _IsNaprMedPoluchenoSOshibkoi = value;
                OnPropertyChanged();
            }
        }

        private DateTime? _NaprMedVidanoDat;
        public DateTime? NaprMedVidanoDat
        {
            get => _NaprMedVidanoDat;
            set
            {
                _NaprMedVidanoDat = value;
                OnPropertyChanged();
            }
        }

        private bool _IsNaprMedVidano;
        public bool IsNaprMedVidano
        {
            get => _IsNaprMedVidano;
            set
            {
                _IsNaprMedVidano = value;
                OnPropertyChanged();
            }
        }

        private DateTime? _MedKommDat;
        public DateTime? MedKommDat
        {
            get => _MedKommDat;
            set
            {
                _MedKommDat = value;
                OnPropertyChanged();
            }
        }

        private bool _IsMedKomm;
        public bool IsMedKomm
        {
            get => _IsMedKomm;
            set
            {
                _IsMedKomm = value;
                OnPropertyChanged();
            }
        }

        private DateTime? _VaccineOneDat;
        public DateTime? VaccineOneDat
        {
            get => _VaccineOneDat;
            set
            {
                _VaccineOneDat = value;
                OnPropertyChanged();
            }
        }

        private bool _IsVaccineOne;
        public bool IsVaccineOne
        {
            get => _IsVaccineOne;
            set
            {
                _IsVaccineOne = value;
                OnPropertyChanged();
            }
        }

        private DateTime? _VaccineOneOnlyDat;
        public DateTime? VaccineOneOnlyDat
        {
            get => _VaccineOneOnlyDat;
            set
            {
                _VaccineOneOnlyDat = value;
                OnPropertyChanged();
            }
        }

        private bool _IsVaccineOneOnly;
        public bool IsVaccineOneOnly
        {
            get => _IsVaccineOneOnly;
            set
            {
                _IsVaccineOneOnly = value;
                OnPropertyChanged();
            }
        }

        private DateTime? _VaccineTwoDat;
        public DateTime? VaccineTwoDat
        {
            get => _VaccineTwoDat;
            set
            {
                _VaccineTwoDat = value;
                OnPropertyChanged();
            }
        }

        private bool _IsVaccineTwo;
        public bool IsVaccineTwo
        {
            get => _IsVaccineTwo;
            set
            {
                _IsVaccineTwo = value;
                OnPropertyChanged();
            }
        }

        private DateTime? _RevacDat;
        public DateTime? RevacDat
        {
            get => _RevacDat;
            set
            {
                _RevacDat = value;
                OnPropertyChanged();
            }
        }

        private string _UchebCentr;
        [DisplayName(DisplayName = "Учебный центр")]
        public string UchebCentr
        {
            get => _UchebCentr;
            set
            {
                _UchebCentr = value;
                if (!IsLoading) { IsLoading = true; SelectedUchebGruppa = null; IsLoading = false; }
                OnPropertyChanged();
            }
        }

        private string _UchebGruppa;
        [DisplayName(DisplayName = "Номер учебной группы")]
        public string UchebGruppa
        {
            get => _UchebGruppa;
            set
            {
                _UchebGruppa = value;
                OnPropertyChanged();
            }
        }

        private DateTime? _UchebStartDat;
        //[DisplayName(DisplayName = "")]
        public DateTime? UchebStartDat
        {
            get => _UchebStartDat;
            set
            {
                _UchebStartDat = value;
                if (!IsLoading) { IsLoading = true; SelectedUchebGruppa = null; IsLoading = false; }
                OnPropertyChanged();
            }
        }

        private DateTime? _UchebEndDat;
        [DisplayName(DisplayName = "Дата окончания обучения")]
        public DateTime? UchebEndDat
        {
            get => _UchebEndDat;
            set
            {
                _UchebEndDat = value;
                if (!IsLoading) { IsLoading = true; SelectedUchebGruppa = null; IsLoading = false; }
                OnPropertyChanged();
            }
        }

        private DateTime? _PraktikaDat;
        public DateTime? PraktikaDat
        {
            get => _PraktikaDat;
            set
            {
                _PraktikaDat = value;
                OnPropertyChanged();
            }
        }

        private bool _IsPraktika;
        public bool IsPraktika
        {
            get => _IsPraktika;
            set
            {
                _IsPraktika = value;
                OnPropertyChanged();
            }
        }

        private DateTime? _ExamenDat;
        public DateTime? ExamenDat
        {
            get => _ExamenDat;
            set
            {
                _ExamenDat = value;
                OnPropertyChanged();
            }
        }

        private bool _IsExamen;
        public bool IsExamen
        {
            get => _IsExamen;
            set
            {
                _IsExamen = value;
                OnPropertyChanged();
            }
        }

        private bool _IsExamenFailed;
        public bool IsExamenFailed
        {
            get => _IsExamenFailed;
            set
            {
                _IsExamenFailed = value;
                OnPropertyChanged();
            }
        }

        private DateTime? _SertificatDat;
        public DateTime? SertificatDat
        {
            get => _SertificatDat;
            set
            {
                _SertificatDat = value;
                OnPropertyChanged();
            }
        }

        private bool _IsSertificatError;
        public bool IsSertificatError
        {
            get => _IsSertificatError;
            set
            {
                _IsSertificatError = value;
                OnPropertyChanged();
            }
        }

        private string _SertificatError;
        public string SertificatError
        {
            get => _SertificatError;
            set
            {
                _SertificatError = value;
                OnPropertyChanged();
            }
        }






        private string _Srez;
        [DisplayName(DisplayName = "Срез")]
        public string Srez
        {
            get => _Srez;
            set
            {
                _Srez = value;
                OnPropertyChanged();
            }
        }

        private DateTime? _VihodDat;
        public DateTime? VihodDat
        {
            get => _VihodDat;
            set
            {
                _VihodDat = value;
                OnPropertyChanged();
            }
        }

        private string _Gorod;
        [DisplayName(DisplayName = "Желаемый город работы")]
        public string Gorod
        {
            get => _Gorod;
            set
            {
                _Gorod = value;
                OnPropertyChanged();
            }
        }

        private bool _IsVibil;
        public bool IsVibil
        {
            get => _IsVibil;
            set
            {
                _IsVibil = value;
                OnPropertyChanged();
            }
        }

        private string _VibilPrichina;
        [DisplayName(DisplayName = "Причина")]
        public string VibilPrichina
        {
            get => _VibilPrichina;
            set
            {
                _VibilPrichina = value;
                OnPropertyChanged();
            }
        }

        private bool _AllPasport;
        public bool AllPasport
        {
            get => _AllPasport;
            set
            {
                _AllPasport = value;
                OnPropertyChanged();
            }
        }

        private bool _AllScans;
        public bool AllScans
        {
            get => _AllScans;
            set
            {
                _AllScans = value;
                OnPropertyChanged();
            }
        }


        private string _Messages;
        public string Messages
        {
            get => _Messages;
            set
            {
                _Messages = value;
                OnPropertyChanged();
            }
        }

        private string _Zametki;

        public string Zametki
        {
            get => _Zametki;
            set
            {
                _Zametki = value;
                OnPropertyChanged();
            }
        }

        //      LastUpdated

        public List<string> UchZavedeniya { get; set; }
        public List<UchebGruppaViewModel> UchebGruppas { get; set; }
        public ObservableCollection<string> UchFormas { get; set; }
        //public ObservableCollection<string> UchFacs { get;  set; }
    }
}