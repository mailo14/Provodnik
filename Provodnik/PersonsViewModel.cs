using ReactiveValidation.Attributes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

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
        public List<string> Cities { get; set; }
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


        public List<string> Otryadi { get; set; }
        public List<string> Grazdanstva { get; set; }
        public List<string> UchZavedeniya { get; set; }

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

        void InitCollectionsForCombo()
        {
            Cities = new Repository().GetCities();
            Otryadi = repository.GetOtryadi();
            Grazdanstva = repository.GetGrazdanstva();
            UchZavedeniya = repository.GetUchZavedeniya();
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
                      Otryad = UchZavedenie = Grazdanstvo = null;
                      IsNovichok = null;
                      Gorod = null; VihodDat = null;

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

            var db = new ProvodnikContext();
            var query = db.Persons.AsQueryable();
            if (!string.IsNullOrWhiteSpace(PersonSearch))
            {
                query = (from p in query//db.Persons
                         where p.Fio.Contains(PersonSearch)
                         orderby p.Fio.IndexOf(PersonSearch)
                         select p);
                /*foreach (var i in (from p in db.Persons
                                   where p.Fio.Contains(PersonSearch)
                                   orderby p.Fio.IndexOf(PersonSearch)
                                   select new PersonShortViewModel() { Id = p.Id, Fio = p.Fio, Phone = p.Phone })) // IdName { Id = p.Id, Name = p.Fio }))
                    PersonList.Add(i);*/
            }
            else
            {
            if (ExceptVibil.HasValue)
                    query = query.Where(pp => pp.IsVibil == !ExceptVibil.Value);
                if (PasportEntered.HasValue)
                    query = query.Where(pp => pp.AllPasport == PasportEntered.Value);
                if (SanknizkaExist.HasValue)
                    query = query.Where(pp => pp.IsSanKnizka== SanknizkaExist.Value);
                if (PsihExist.HasValue)
                    query = query.Where(pp => PsihExist.Value==( pp.PersonDocs.FirstOrDefault(ppp => ppp.DocTypeId == DocConsts.Психосвидетельствование).FileName != null));                
                if (MedKommExist.HasValue)
                    query = query.Where(pp => PsihExist.Value == (pp.PersonDocs.FirstOrDefault(ppp => ppp.DocTypeId == DocConsts.ЗаключениеВЭК).FileName != null));
                //query = query.Where(pp => pp.IsMedKomm== MedKommExist.Value);
                if (PraktikaExist.HasValue)
                    query = query.Where(pp => pp.IsPraktika== PraktikaExist.Value);
                if (ExamenExist.HasValue)
                    query = query.Where(pp => PsihExist.Value == (pp.PersonDocs.FirstOrDefault(ppp => ppp.DocTypeId == DocConsts.СвидетельствоПрофессии).FileName != null));
                //query = query.Where(pp => pp.IsExamen== ExamenExist.Value);
                if (AllScansExist.HasValue)
                    query = query.Where(pp => pp.AllScans== AllScansExist.Value);

                if (!string.IsNullOrWhiteSpace(Gorod))
                    query = query.Where(pp => pp.Gorod ==null || pp.Gorod == Gorod);

                if (VihodDat.HasValue)
                    query = query.Where(pp => pp.VihodDat == null || (pp.VihodDat >= VihodDat && pp.VihodDat.Value.Year==DateTime.Today.Year));               

            }

            foreach (var ec in ExtendedChecks)
                if (ec.IsChecked.HasValue)
                switch (ec.DocType)
                {
                    case DocConsts.Паспорт:
                        query = query.Where(pp => ec.IsChecked.Value == (pp.PersonDocs.Where(ppp => ppp.DocTypeId == DocConsts.Паспорт || ppp.DocTypeId == DocConsts.Прописка).All(ppp => ppp.FileName != null)));
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
                                (pp.PersonDocs.Where(ppp => ppp.DocTypeId == DocConsts.Приписное1 || ppp.DocTypeId == DocConsts.Приписное2)
                                .All(ppp => ppp.FileName != null))==true
                        || (pp.PersonDocs.Where(ppp => ppp.DocTypeId == DocConsts.ВоенныйБилет).All(ppp => ppp.FileName != null))
                        );
                            }
                            else
                            query = query.Where(pp => pp.Pol== "мужской" &&
                            false==(pp.PersonDocs.Where(ppp => ppp.DocTypeId == DocConsts.Приписное1 || ppp.DocTypeId == DocConsts.Приписное2).All(ppp => ppp.FileName != null))
                        && false == (pp.PersonDocs.Where(ppp => ppp.DocTypeId == DocConsts.ВоенныйБилет).All(ppp => ppp.FileName != null))
                        );
                        break;
                        case DocConsts.СправкаУчебы:
                            query = query.Where(pp => ec.IsChecked.Value == (pp.PersonDocs.Where(ppp => ppp.DocTypeId == DocConsts.СправкаУчебы).All(ppp => ppp.FileName != null)));
                            break;
                        case DocConsts.СправкаРСО:
                            query = query.Where(pp => ec.IsChecked.Value == (pp.PersonDocs.Where(ppp => ppp.DocTypeId == DocConsts.СправкаРСО).All(ppp => ppp.FileName != null)));
                            break;
                        case DocConsts.Миграционная1:
                            query = query.Where(pp => pp.Grazdanstvo=="КЗ" && ec.IsChecked.Value == (pp.PersonDocs.Where(ppp => ppp.DocTypeId == DocConsts.Миграционная1
                            || ppp.DocTypeId == DocConsts.Миграционная2 || ppp.DocTypeId == DocConsts.ВремРегистрация1 || ppp.DocTypeId == DocConsts.ВремРегистрация2
                            ).All(ppp => ppp.FileName != null)));
                            break;
                    }
                
            if (IsNovichok.HasValue)
                query = query.Where(pp => pp.IsNovichok== IsNovichok.Value);
            if (!string.IsNullOrWhiteSpace(Otryad))
                query = query.Where(pp => pp.Otryad== Otryad);
            if (!string.IsNullOrWhiteSpace(UchZavedenie))
                query = query.Where(pp => pp.UchZavedenie == UchZavedenie);
            if (!string.IsNullOrWhiteSpace(Grazdanstvo))
                query = query.Where(pp => pp.Grazdanstvo == Grazdanstvo);
            if (BirthFrom.HasValue)
                query = query.Where(pp => pp.BirthDat>= BirthFrom);
            if (BirthTo.HasValue)
                query = query.Where(pp => pp.BirthDat <= BirthTo);


            int index = 0;
            foreach (var i in query.OrderBy(pp=>pp.Fio))
            {
                //select new PersonShortViewModel() { Id = p.Id, Fio = p.Fio, Phone = p.Phone })) // IdName { Id = p.Id, Name = p.Fio }))
                var pe=MainWindow.Mapper.Value.Map<PersonShortViewModel>(i);
                pe.Index = ++index;
                PersonList.Add(pe);

            }

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
