using Ninject;
using ReactiveValidation.Attributes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Linq.Expressions;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using LinqKit;

namespace Provodnik
{
   public  class PersonsViewModel : System.ComponentModel.INotifyPropertyChanged//: PersonDoc
    {
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(prop));
        }
        //public  List<(string DisplayName, bool? IsChecked)> ExtendedChecks;
        public class ScanCheck : System.ComponentModel.INotifyPropertyChanged
        { public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
            public void OnPropertyChanged([CallerMemberName]string prop = "")
            {
                if (PropertyChanged != null)
                    PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(prop));
            }
            private string _DisplayName;
            public string DisplayName
            {
                get => _DisplayName;
                set
                {
                    _DisplayName = value;
                    OnPropertyChanged();
                }
            }
            private bool? _IsChecked=null;
            public bool? IsChecked
            {
                get => _IsChecked;
                set
                {
                    _IsChecked = value;
                    OnPropertyChanged();
                }
            }
           
            public int DocType;
        }
        public List<ScanCheck> ExtendedChecks { get; set; }


        public ObservableCollection<string> Cities { get; set; } = new ObservableCollection<string>();
        public ObservableCollection<string> Otryadi { get; set; } = new ObservableCollection<string>();
        public ObservableCollection<string> Grazdanstva { get; set; } = new ObservableCollection<string>();
        public ObservableCollection<string> Obucheniyas { get; set; } = new ObservableCollection<string>();
        public ObservableCollection<string> MedKommDats { get; set; } = new ObservableCollection<string>();
        public ObservableCollection<string> UchZavedeniya { get; set; } = new ObservableCollection<string>();
        public ObservableCollection<string> Sezons { get; set; } = new ObservableCollection<string>();

        string _UchZavedenie;
        [DisplayName(DisplayName = "Учебное заведение")]
        public string UchZavedenie
        {
            get => _UchZavedenie;
            set
            {
                _UchZavedenie = value;               
                OnPropertyChanged();
            }
        }

        string _Sezon;
        //[DisplayName(DisplayName = "Учебное заведение")]
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
                OnPropertyChanged();
            }
        }
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
        private bool? _IsNovichok=null;
        public bool? IsNovichok
        {
            get => _IsNovichok;
            set
            {
                _IsNovichok = value;
                OnPropertyChanged();
            }
        }
        private string _Obuchenie;
        public string Obuchenie
        {
            get => _Obuchenie;
            set
            {
                _Obuchenie = value;
                OnPropertyChanged();
            }
        }
        private string _MedKommDat;
        public string MedKommDat
        {
            get => _MedKommDat;
            set
            {
                _MedKommDat = value;
                OnPropertyChanged();
            }
        }

        private DateTime? _BirthFrom;
        public DateTime? BirthFrom
        {
            get => _BirthFrom;
            set
            {
                _BirthFrom = value;
                OnPropertyChanged();
            }
        }
        private DateTime? _BirthTo;
        public DateTime? BirthTo
        {
            get => _BirthTo;
            set
            {
                _BirthTo = value;
                OnPropertyChanged();
            }
        }

       public void InitCollectionsForCombo()
        {
            Cities.Clear(); foreach (var pp in repository.GetPersonCities()) Cities.Add(pp);
            Otryadi.Clear(); foreach (var pp in repository.GetOtryadi()) Otryadi.Add(pp);
            Grazdanstva.Clear(); foreach (var pp in repository.GetGrazdanstva()) Grazdanstva.Add(pp);
            UchZavedeniya.Clear(); foreach (var pp in repository.GetUchZavedeniya()) UchZavedeniya.Add(pp);

            Sezons.Clear(); foreach (var pp in repository.GetSezons()) Sezons.Add(pp);            Sezons.Insert(0, RepoConsts.NoSezons);
                        Sezon = new Repository().GetCurSezon();

            Obucheniyas.Clear(); foreach (var pp in repository.GetObucheniyas()) Obucheniyas.Add(pp);

            MedKommDats.Clear(); foreach (var pp in repository.GetMedKommDats()) MedKommDats.Add(pp);

        }

        Repository repository = new Repository();
        public PersonsViewModel()
        {
            InitCollectionsForCombo();
            ExtendedChecks = new List<ScanCheck>()
            {
                new ScanCheck{DisplayName="Паспорт",DocType=DocConsts.Паспорт },
                new ScanCheck{DisplayName="СНИЛС",DocType=DocConsts.СНИЛС },
                new ScanCheck{DisplayName="ИНН",DocType=DocConsts.ИНН },
                //new ScanCheck{DisplayName="Психиатрическое освидетельствование",DocType=DocConsts.Психосвидетельствование },
//new ScanCheck{DisplayName="Заключение ВЭК",DocType=DocConsts.ЗаключениеВЭК },
new ScanCheck{DisplayName="Согласие на обработку персональных данных",DocType=DocConsts.СогласиеПерс},
//new ScanCheck{DisplayName="Свидетельство о присвоении профессии",DocType=DocConsts.СвидетельствоПрофессии},
new ScanCheck{DisplayName="Реквизиты банковской (зарплатной) карты",DocType=DocConsts.РеквизитыКарты},
                new ScanCheck{DisplayName="Приписное/военный билет",DocType=DocConsts.ВоенныйБилет },
new ScanCheck{DisplayName="Справка с места учебы",DocType=DocConsts.СправкаУчебы},
new ScanCheck{DisplayName="Справка-подтверждение МООО  «РСО»",DocType=DocConsts.СправкаРСО},
new ScanCheck{DisplayName="Миграционная карта и временная регистрация",DocType=DocConsts.Миграционная1},

            };
        }

        private string _Gorod;
        public string Gorod
        {
            get => _Gorod;
            set
            {
                _Gorod = value;
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
        

        string _PersonSearch;
        public string PersonSearch
        {
            get => _PersonSearch;
            set
            {
                
                _PersonSearch = value;
                RefreshPersonList();
               /* if (!string.IsNullOrWhiteSpace(value) && PersonSearchList.Count == 0)
                    PersonSearchList.Add(new IdName { Id = 0, Name = "добавить нового?" });*/
                OnPropertyChanged();
            }
        }



        private RelayCommand _ClearFilterCommand;
        public RelayCommand ClearFilterCommand
        {
            get
            {
                return _ClearFilterCommand ??
                  (_ClearFilterCommand = new RelayCommand(obj =>
                  {

                      ReadyOnly = null;
                      foreach (var ec in ExtendedChecks) ec.IsChecked = null;
                      BirthFrom = BirthTo = null;
                      Otryad = UchZavedenie = Grazdanstvo = Obuchenie= MedKommDat= null;
                      IsNovichok = null;
                      Gorod = null; VihodDat = null;
                     Sezon= new Repository().GetCurSezon();

                      PersonSearch = null;//run find, should be last                      RefreshPersonList();
                  }));
            }
        }
private RelayCommand _FindCommand;
        public RelayCommand FindCommand
        {
            get
            {
                return _FindCommand ??
                  (_FindCommand = new RelayCommand(obj =>
                  {

                      PersonSearch = null;//run find, should be last                      RefreshPersonList();
                  }));
            }
        }
        public void RefreshPersonList()
        {
            foreach (var p in PersonList)
                p.IsSelected = false;

            PersonList.Clear();

            var db = NinjectContext.Get<ProvodnikContext>();//new ProvodnikContext();
            var query = db.Persons.AsQueryable();
            if (!string.IsNullOrWhiteSpace(PersonSearch))
            {
                var lines = PersonSearch.Trim().Split(new char[] { '+' }, StringSplitOptions.RemoveEmptyEntries);
                if (lines.Length == 1) {
                    var fio = lines[0];
                    query = (from p in query//db.Persons
                             where p.Fio.Contains(fio)
                             orderby p.Fio.IndexOf(fio)
                             select p);
                }
                else
                {
                    var predicate = PredicateBuilder.New<Person>(false);
                    foreach (var line in lines )
                    {
                        var fio = new StringHelper().ParseFio(line);
                        if (string.IsNullOrEmpty(fio[1]))
                        {
                            var f = fio[0] + " ";
                            predicate = predicate.Or(p => p.Fio.StartsWith(f));
                        }
                        else predicate = predicate.Or(p => p.Fio.StartsWith(line));
                    }

                    query = query.Where(predicate);
                }
                /*foreach (var i in (from p in db.Persons
                                   where p.Fio.Contains(PersonSearch)
                                   orderby p.Fio.IndexOf(PersonSearch)
                                   select new PersonShortViewModel() { Id = p.Id, Fio = p.Fio, Phone = p.Phone })) // IdName { Id = p.Id, Name = p.Fio }))
                    PersonList.Add(i);*/
            }
            else
            {
                if (IsNaprMedVidano != null)
                    query = query.Where(pp => pp.IsNaprMedVidano == IsNaprMedVidano);
                if (IsNaprMedPoluchenoSOshibkoi != null)
                    query = query.Where(pp => pp.IsNaprMedPoluchenoSOshibkoi == IsNaprMedPoluchenoSOshibkoi); 
                if (IsNaprMedPoluchenoNePoln != null)
                    query = query.Where(pp => pp.IsNaprMedPoluchenoNePoln == IsNaprMedPoluchenoNePoln);
                if (IsNaprMedPolucheno != null)
                    query = query.Where(pp => pp.IsNaprMedPolucheno == IsNaprMedPolucheno);
                if (IsNaprMedZakazano != null)
                    query = query.Where(pp => pp.IsNaprMedZakazano == IsNaprMedZakazano);

                if (Sezon!=null && Sezon!= RepoConsts.NoSezons)
                    query = query.Where(pp => pp.Sezon ==  Sezon); 
                if (ExceptVibil.HasValue)
                    query = query.Where(pp => pp.IsVibil == !ExceptVibil.Value);
                if (PasportEntered.HasValue)
                    query = query.Where(pp => pp.AllPasport == PasportEntered.Value);
                if (SanknizkaExist.HasValue)
                    query = query.Where(pp => pp.IsSanKnizka == SanknizkaExist.Value);
                if (PsihExist.HasValue)
                    query = query.Where(pp => PsihExist.Value == (pp.PersonDocs.FirstOrDefault(ppp => ppp.DocTypeId == DocConsts.Психосвидетельствование).FileName != null));
                    
                if (MedKommExist.HasValue)
                    //query = query.Where(pp => MedKommExist.Value == (pp.PersonDocs.FirstOrDefault(ppp => ppp.DocTypeId == DocConsts.ЗаключениеВЭК).FileName != null));
                    query = query.Where(pp => PsihExist.Value == (pp.PersonDocs.Count(ppp => (ppp.DocTypeId == DocConsts.ЗаключениеВЭК || ppp.DocTypeId == DocConsts.ЗаключениеВЭК2) && ppp.FileName != null) == 2));
                //query = query.Where(pp => pp.IsMedKomm== MedKommExist.Value);
                if (PraktikaExist.HasValue)
                    query = query.Where(pp => pp.IsPraktika == PraktikaExist.Value);
                if (ExamenExist.HasValue)
                    query = query.Where(pp => ExamenExist.Value == (pp.PersonDocs.FirstOrDefault(ppp => ppp.DocTypeId == DocConsts.СвидетельствоПрофессии).FileName != null));
                //query = query.Where(pp => pp.IsExamen== ExamenExist.Value);
                if (AllScansExist.HasValue)
                    query = query.Where(pp => pp.AllScans == AllScansExist.Value);

                if (!string.IsNullOrWhiteSpace(Gorod))
                    query = query.Where(pp => pp.Gorod == null || pp.Gorod == Gorod);

                if (VihodDat.HasValue)
                    query = query.Where(pp => pp.VihodDat == null || (pp.VihodDat >= VihodDat && pp.VihodDat.Value.Year == DateTime.Today.Year));

            }

            foreach (var ec in ExtendedChecks)
                if (ec.IsChecked.HasValue)
                    switch (ec.DocType)
                    {
                        case DocConsts.Паспорт:
                            query = query.Where(pp => ec.IsChecked.Value == db.PersonDocs.Where(ppp => ppp.PersonId == pp.Id && (ppp.DocTypeId == DocConsts.Паспорт || ppp.DocTypeId == DocConsts.Прописка)).All(ppp => ppp.FileName != null));
                            //query = query.Where(pp => ec.IsChecked.Value == (pp.PersonDocs.Where(ppp => ppp.DocTypeId == DocConsts.Паспорт || ppp.DocTypeId == DocConsts.Прописка).All(ppp => ppp.FileName != null)));
                            break;
                        case DocConsts.СНИЛС:
                            query = query.Where(pp => ec.IsChecked.Value == (pp.PersonDocs.Where(ppp => ppp.DocTypeId == DocConsts.СНИЛС).All(ppp => ppp.FileName != null)));
                            break;
                        case DocConsts.ИНН:
                            query = query.Where(pp => ec.IsChecked.Value == (pp.PersonDocs.Where(ppp => ppp.DocTypeId == DocConsts.ИНН).All(ppp => ppp.FileName != null)));
                            break;
                        /*case DocConsts.Психосвидетельствование:
                            query = query.Where(pp => ec.IsChecked.Value == (pp.PersonDocs.Where(ppp => ppp.DocTypeId == DocConsts.Психосвидетельствование).All(ppp => ppp.FileName != null)));
                            break;
                        case DocConsts.ЗаключениеВЭК:
                            query = query.Where(pp => ec.IsChecked.Value == (pp.PersonDocs.Where(ppp => ppp.DocTypeId == DocConsts.ЗаключениеВЭК).All(ppp => ppp.FileName != null)));
                            break;*/
                        case DocConsts.СогласиеПерс:
                            query = query.Where(pp => ec.IsChecked.Value == (pp.PersonDocs.Where(ppp => ppp.DocTypeId == DocConsts.СогласиеПерс).All(ppp => ppp.FileName != null)));
                            break;
                        /*case DocConsts.СвидетельствоПрофессии:
                            query = query.Where(pp => ec.IsChecked.Value == (pp.PersonDocs.Where(ppp => ppp.DocTypeId == DocConsts.СвидетельствоПрофессии).All(ppp => ppp.FileName != null)));
                            break;*/
                        case DocConsts.РеквизитыКарты:
                            query = query.Where(pp => ec.IsChecked.Value == (pp.PersonDocs.Where(ppp => ppp.DocTypeId == DocConsts.РеквизитыКарты).All(ppp => ppp.FileName != null)));
                            break;
                        case DocConsts.ВоенныйБилет:
                            if (ec.IsChecked.Value)
                            {
                                query = query.Where(pp =>
                                db.PersonDocs.Where(ppp => ppp.PersonId == pp.Id && (ppp.DocTypeId == DocConsts.Приписное1 || ppp.DocTypeId == DocConsts.Приписное2) && ppp.FileName != null)
                                .Count() == 2
                                ||
                                db.PersonDocs.Where(ppp => ppp.PersonId == pp.Id && (ppp.DocTypeId == DocConsts.ВоенныйБилет) && ppp.FileName != null)
                                .Count() == 1
                                );
                            }
                            else
                            {
                                query = query.Where(pp => pp.Pol == "мужской" 
                                &&
                                db.PersonDocs.Where(ppp => ppp.PersonId == pp.Id && (ppp.DocTypeId == DocConsts.Приписное1 || ppp.DocTypeId == DocConsts.Приписное2) && ppp.FileName != null)
                                .Count() != 2
                                && 
                                db.PersonDocs.Where(ppp => ppp.PersonId == pp.Id && (ppp.DocTypeId == DocConsts.ВоенныйБилет) && ppp.FileName != null)
                                .Count() != 1   
                                );
                            }

                            break;
                        case DocConsts.СправкаУчебы:
                            query = query.Where(pp => ec.IsChecked.Value == (pp.PersonDocs.Where(ppp => ppp.DocTypeId == DocConsts.СправкаУчебы).All(ppp => ppp.FileName != null)));
                            break;
                        case DocConsts.СправкаРСО:
                            query = query.Where(pp => ec.IsChecked.Value == (pp.PersonDocs.Where(ppp => ppp.DocTypeId == DocConsts.СправкаРСО).All(ppp => ppp.FileName != null)));
                            break;
                        case DocConsts.Миграционная1:
                            if (ec.IsChecked.Value)
                            {
                                query = query.Where(pp =>
                                db.PersonDocs.Where(ppp => ppp.PersonId == pp.Id && (ppp.DocTypeId == DocConsts.Миграционная1 || ppp.DocTypeId == DocConsts.Миграционная2 || ppp.DocTypeId == DocConsts.ВремРегистрация1 || ppp.DocTypeId == DocConsts.ВремРегистрация2) && ppp.FileName != null)
                                .Count() == 4
                                );
                            }
                            else
                            {
                                query = query.Where(pp => pp.Grazdanstvo == "КЗ" &&
                                db.PersonDocs.Where(ppp => ppp.PersonId == pp.Id && (ppp.DocTypeId == DocConsts.Миграционная1 || ppp.DocTypeId == DocConsts.Миграционная2 || ppp.DocTypeId == DocConsts.ВремРегистрация1 || ppp.DocTypeId == DocConsts.ВремРегистрация2) && ppp.FileName != null)
                                .Count() != 4
                                );
                            }
                            break;
                    }

            if (IsNovichok.HasValue)
                query = query.Where(pp => pp.IsNovichok == IsNovichok.Value);
            if (!string.IsNullOrWhiteSpace(Otryad))
                query = query.Where(pp => pp.Otryad == Otryad);
            if (!string.IsNullOrWhiteSpace(UchZavedenie))
                query = query.Where(pp => pp.UchZavedenie == UchZavedenie);
            if (!string.IsNullOrWhiteSpace(Grazdanstvo))
                query = query.Where(pp => pp.Grazdanstvo == Grazdanstvo);
            if (!string.IsNullOrWhiteSpace(Obuchenie))
            {
                if (Obuchenie == RepoConsts.NoObuchenie)
                {
                    query = query.Where(pp => pp.UchebGruppa == null);
                }
                else
                {
                    var yearStart = DateTime.Today;
                    yearStart = yearStart.AddDays(-yearStart.Day + 1).AddMonths(-yearStart.Month + 1);
                    query = query.Where(pp => pp.UchebGruppa == Obuchenie && pp.UchebStartDat > yearStart);
                }
            }
            if (!string.IsNullOrWhiteSpace(MedKommDat))
            {
                if (MedKommDat == RepoConsts.NoMedKommDat)
                {
                    query = query.Where(pp => pp.MedKommDat == null);
                }
                else
                {
                    var medKommDat = DateTime.Parse(MedKommDat);
                    query = query.Where(pp => pp.MedKommDat == medKommDat);
                }
            }
            if (BirthFrom.HasValue)
                query = query.Where(pp => pp.BirthDat >= BirthFrom);
            if (BirthTo.HasValue)
                query = query.Where(pp => pp.BirthDat <= BirthTo);


            foreach (var i in query.OrderBy(pp => pp.Fio))
            {
                //select new PersonShortViewModel() { Id = p.Id, Fio = p.Fio, Phone = p.Phone })) // IdName { Id = p.Id, Name = p.Fio }))
                var pe = MainWindow.Mapper.Value.Map<PersonShortViewModel>(i);
                PersonList.Add(pe);
            }
            Helper.SetPersonShortIndexes(PersonList);

            if (PersonList.Count == 1)
                PersonList[0].IsSelected = true;
        }

        IdName _PersonSearchItem;
        public IdName PersonSearchItem
        {
            get => _PersonSearchItem;
            set
            {

                _PersonSearchItem = value;
               if (value != null)
                {
                    if (value.Id == 0)
                        new PersonView().ShowDialog();
                    //else PersonList.Add(new PersonViewModel(value.Id));
                }
                OnPropertyChanged();
            }
        }

        public class IdName
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }

        ObservableCollection<IdName> _PersonSearchList=new ObservableCollection<IdName>();
        public ObservableCollection<IdName> PersonSearchList
        {
            get => _PersonSearchList;
            set
            {
                _PersonSearchList = value;
                OnPropertyChanged();
            }
        }

        ObservableCollection<PersonShortViewModel> _PersonList=new ObservableCollection<PersonShortViewModel>();
        public ObservableCollection<PersonShortViewModel> PersonList
        {
            get => _PersonList;
            set
            {
                _PersonList = value;
                OnPropertyChanged();
            }
        }

        bool CascadeMode = false;

        bool? _ReadyOnly;
        public bool? ReadyOnly
        {
            get => _ReadyOnly;
            set
            {
              //  if (_ReadyOnly != value)
                {
                    if (!CascadeMode)
                    {
                        ExceptVibil =
                       PasportEntered =
                       SanknizkaExist =
                       PsihExist =
                       MedKommExist =
                       PraktikaExist =
                    ExamenExist =
                    AllScansExist =            //ActualOnly
                  value;
                    }
                    _ReadyOnly = value;
                    OnPropertyChanged();
                }
            }
        }
        void ReCalcReady()
        {
                    CascadeMode = true;
                bool?[] subs  = new bool?[] {
               ExceptVibil,
               PasportEntered ,
               SanknizkaExist,
               PsihExist ,
               MedKommExist ,
               PraktikaExist,
            ExamenExist ,
            AllScansExist,            //ActualOnly
        };
            if (subs.All(pp => pp==true)) ReadyOnly = true;
            else if (subs.All(pp => pp==false)) ReadyOnly = false;
            else ReadyOnly = null;
                    CascadeMode = false;
        }
            
        bool? _ExceptVibil;
        public bool? ExceptVibil
        {
            get => _ExceptVibil;
            set
            {
                if (_ExceptVibil != value)
                {
                    _ExceptVibil = value;
                  ReCalcReady();
                    OnPropertyChanged();
                }
            }
        }

        bool? _PasportEntered ;
        public bool? PasportEntered
        {
            get => _PasportEntered;
            set
            {
                if (_PasportEntered != value)
                {
                    _PasportEntered = value;
                    ReCalcReady();
                    OnPropertyChanged();
                }
            }
        }

        bool? _SanknizkaExist;
        public bool? SanknizkaExist
        {
            get => _SanknizkaExist;
            set
            {
                if (_SanknizkaExist != value)
                {
                    _SanknizkaExist = value;
                    ReCalcReady();
                    OnPropertyChanged();
                }
            }
        }

        bool? _PsihExist;
        public bool? PsihExist
        {
            get => _PsihExist;
            set
            {
                if (_PsihExist != value)
                {
                    _PsihExist = value;
                    ReCalcReady();
                    OnPropertyChanged();
                }
            }
        }

        bool? _MedKommExist;
        public bool? MedKommExist
        {
            get => _MedKommExist;
            set
            {
                if (_MedKommExist != value)
                {
                    _MedKommExist = value;
                    ReCalcReady();
                    OnPropertyChanged();
                }
            }
        }

        bool? _ExamenExist;
        public bool? ExamenExist
        {
            get => _ExamenExist;
            set
            {
                if (_ExamenExist != value)
                {
                    _ExamenExist = value;
                    ReCalcReady();
                    OnPropertyChanged();
                }
            }
        }

        bool? _PraktikaExist;
        public bool? PraktikaExist
        {
            get => _PraktikaExist;
            set
            {
                if (_PraktikaExist != value)
                {
                    _PraktikaExist = value;
                    ReCalcReady();
                    OnPropertyChanged();
                }
            }
        }

        bool? _AllScansExist;
        public bool? AllScansExist
        {
            get => _AllScansExist;
            set
            {
                if (_AllScansExist != value)
                {
                    _AllScansExist = value;
                    ReCalcReady();
                    OnPropertyChanged();
                }
            }
        }

        private bool? _IsNaprMedZakazano;
        public bool? IsNaprMedZakazano
        {
            get => _IsNaprMedZakazano;
            set
            {
                _IsNaprMedZakazano = value;
                OnPropertyChanged();
            }
        }
        
        private bool? _IsNaprMedPolucheno;
        public bool? IsNaprMedPolucheno
        {
            get => _IsNaprMedPolucheno;
            set
            {
                _IsNaprMedPolucheno = value;
                OnPropertyChanged();
            }
        }

        private bool? _IsNaprMedPoluchenoNePoln;
        public bool? IsNaprMedPoluchenoNePoln
        {
            get => _IsNaprMedPoluchenoNePoln;
            set
            {
                _IsNaprMedPoluchenoNePoln = value;
                OnPropertyChanged();
            }
        }

        private bool? _IsNaprMedPoluchenoSOshibkoi;
        public bool? IsNaprMedPoluchenoSOshibkoi
        {
            get => _IsNaprMedPoluchenoSOshibkoi;
            set
            {
                _IsNaprMedPoluchenoSOshibkoi = value;
                OnPropertyChanged();
            }
        }

        private bool? _IsNaprMedVidano;
        public bool? IsNaprMedVidano
        {
            get => _IsNaprMedVidano;
            set
            {
                _IsNaprMedVidano = value;
                OnPropertyChanged();
            }
        }

        /*    bool? _ActualOnly;
            public bool? ActualOnly
            {
                get => _ActualOnly;
                set
                {
                    _ActualOnly = value;
                    OnPropertyChanged();
                }
            }*/
    }
}
