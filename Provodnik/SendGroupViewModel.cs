using ReactiveValidation;
using ReactiveValidation.Attributes;
using ReactiveValidation.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Provodnik
{
    public class SendGroupViewModel : ValidatableObject// : System.ComponentModel.INotifyPropertyChanged
    {
        /* public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
         public void OnPropertyChanged([CallerMemberName]string prop = "")
         {
             if (PropertyChanged != null)
                 PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(prop));
         }*/
        private bool _isLoaded = false;
        Repository repository = new Repository();

        public SendGroupViewModel(int? sendGroupId=null)
        {
            Validator = GetValidator();

            RegOtdeleniya = repository.GetRegOtdeleniya();

               Cities = repository.GetCities();
            
            PeresadSts= repository.GetPeresadSts();

            Persons = new ObservableCollection<SendGroupPersonViewModel>();

            var db = new ProvodnikContext();
            if (sendGroupId.HasValue)
            {
                Id = sendGroupId;
                MainWindow.Mapper.Value.Map(db.SendGroups.Single(pp => pp.Id == sendGroupId), this);
                if (City != null) { LoadDepos(City); }

                var qq = (from pd in db.SendGroupPersons join p in db.Persons on pd.PersonId equals p.Id
                          where pd.SendGroupId == sendGroupId
                          select new { pd, p }).ToList();
                                
                foreach (var q in qq)
                {
                    SendGroupPersonViewModel pe = new SendGroupPersonViewModel();
                    MainWindow.Mapper.Value.Map(q.pd, pe);
                    MainWindow.Mapper.Value.Map(q.p, pe);
                    pe.SendGroupViewModel = this;
                    Persons.Add(pe);

                }
//Persons.Add(MainWindow.Mapper.Value.Map< SendGroupPersonViewModel>(db.SendGroups.Single(pp => pp.Id == sendGroupId), this);
            }
            else
            {
            RegOtdelenie = RegOtdeleniya[0];

            }

            _isLoaded = true;
        }
        private IObjectValidator GetValidator()
        {
            var builder = new ValidationBuilder<SendGroupViewModel>();

            builder.RuleFor(vm => vm.RegOtdelenie).MyNotEmpty();
            builder.RuleFor(vm => vm.City).MyNotEmpty();
            builder.RuleFor(vm => vm.Depo).MyNotEmpty(); 
          builder.RuleFor(vm => vm.Uvolnenie).MyNotEmptyDat();
            builder.RuleFor(vm => vm.Poezd).MyNotEmpty();
            builder.RuleFor(vm => vm.Vagon).MyNotEmpty();

             builder.RuleFor(vm => vm.OtprDat).MyNotEmptyDat();
               builder.RuleFor(vm => vm.PribDat).MyNotEmptyDat();

           
            //.Matches(@"^\d{1,2}:d{2}$")
            builder.RuleFor(vm => vm.PribTime)
                    .Must(IsValidTime)
                .WithMessage("{PropertyName} должно быть в формате ЧЧ:ММ");

          /*  builder.RuleFor(vm=>vm).NotEmpty()                
                .When(vm => vm.Persons, persons => !persons.Any(pp=>pp.IsMain))
                .WithMessage("В группе должен быть старший");
*/
            builder.RuleFor(vm => vm.Vokzal).MyNotEmpty();
            ;//TODO .When(vm => UchZavedenie, uchZavedenie => !string.IsNullOrEmpty(uchZavedenie)).Between("1950", "2050");




            return builder.Build(this);
        }
        private static bool IsValidTime(string pribTime)
        {
  if (string.IsNullOrWhiteSpace(pribTime)) return false;

            var match = System.Text.RegularExpressions.Regex.Match(pribTime, "^[0-9]{1,2}:[0-9]{2}$");
            if (!match.Success) return false;

                var hm = pribTime.Split(new char[] { ':' });
            int t;
            if (hm.Length != 2 || !int.TryParse(hm[0],out t) || !int.TryParse(hm[1], out t)) return false;

                return (int.Parse(hm[0]) < 24 && int.Parse(hm[1]) < 60);
            
        }

        public List<string> RegOtdeleniya { get; set; }
        public List<string> Cities { get; set; }
        public List<string> PeresadSts { get; set; }
        public ObservableCollection<string> Depos { get; set; } = new ObservableCollection<string>();


        public ObservableCollection<SendGroupPersonViewModel> Persons { get; set; }

        public int? Id { get; set; }

        private string _Name;
        [DisplayName(DisplayName = "Название")]
        public string Name
        {
            get => _Name;
            set
            {
                _Name = value;
                OnPropertyChanged();
            }
        }

        private string _RegOtdelenie;
        [DisplayName(DisplayName = "Рег. отделение")]
        public string RegOtdelenie
        {
            get => _RegOtdelenie;
            set
            {
                _RegOtdelenie = value;
                OnPropertyChanged();
            }
        }

        public string _City;
        [DisplayName(DisplayName = "Город назначения")]
        public string City
        {
            get => _City;
            set
            {
                if (string.IsNullOrWhiteSpace(value)) value = null;

                if (_City != value)
                {
                    _City = value;

                    if (_isLoaded)
                    {
                        Depo = null;
                        LoadDepos(City);

                        if (Depos.Count == 1)
                            Depo = Depos[0];
                        else
                        {
                            ClearDepoRod();
                        }
                    }

                  //  ChangeDepoRod();
                    OnPropertyChanged();
                }
            }
        }

        private void ClearDepoRod()
        {
            DepoRod =            Filial =             Sp =null;
        }

        private void LoadDepos(string city)
        {
            Depos.Clear();
            if (city!=null)
                foreach (var d in repository.GetDepos(city)) Depos.Add(d);
        }

        public string _Filial;
        //[DisplayName(DisplayName = "Станция пересадки")]
        public string Filial
        {
            get => _Filial;
            set
            {
                _Filial = value;
                OnPropertyChanged();
            }
        }

        public string _Sp;
        //[DisplayName(DisplayName = "Станция пересадки")]
        public string Sp
        {
            get => _Sp;
            set
            {
                _Sp = value;
                OnPropertyChanged();
            }
        }

        public string _PeresadSt;
        [DisplayName(DisplayName = "Станция пересадки")]
        public string PeresadSt
        {
            get => _PeresadSt;
            set
            {
                _PeresadSt = value;
                OnPropertyChanged();
            }
        }

        public string _Depo;
        [DisplayName(DisplayName = "Депо приписки")]
        public string Depo
        {
            get => _Depo;
            set
            {
                if (string.IsNullOrWhiteSpace(value)) value = null;

                if (_Depo != value)
                {
                    _Depo = value;

                    if (_isLoaded)
                    {
                        if (Depo != null) ChangeDepoRod();
                        else ClearDepoRod();
                    }

                    OnPropertyChanged();
                }
            }
        }

        private void ChangeDepoRod()
        {
            var labels = new Repository().GetDepoLabels(City, Depo);
            if (labels == null)
                ClearDepoRod();
            else
            {
                DepoRod = labels.DepoRod;
                Filial = labels.Filial;
                Sp = labels.Sp;
            }
        }

        public string _DepoRod;
        [DisplayName(DisplayName = "Депо приписки в родительном падеже")]
        public string DepoRod
        {
            get => _DepoRod;
            set
            {
                _DepoRod = value;
                OnPropertyChanged();
            }
        }

        string _Poezd;
       [DisplayName(DisplayName = "№ поезда")]
        public string Poezd
        {
            get => _Poezd;
            set
            {
                _Poezd = value;
                OnPropertyChanged();
            }
        }

        string _Vagon;
        [DisplayName(DisplayName = "Вагон")]
        public string Vagon 
        {
            get => _Vagon;
            set
            {
                _Vagon = value;
                OnPropertyChanged();
            }
        }

        DateTime? _OtprDat;
        [DisplayName(DisplayName = "Дата отправления")]
        public DateTime? OtprDat
        {
            get => _OtprDat;
            set
            {
                _OtprDat = value;
                OnPropertyChanged();
            }
        }

        DateTime? _PribDat;
        [DisplayName(DisplayName = "Дата прибытия")]
        public DateTime? PribDat
        {
            get => _PribDat;
            set
            {
                _PribDat = value;
                OnPropertyChanged();
            }
        }

        string _PribTime;
        [DisplayName(DisplayName = "Время прибытия (Мск)")]
        public string PribTime
        {
            get => _PribTime;
            set
            {
                _PribTime = value;
                OnPropertyChanged();
            }
        }

        bool _Vstrechat;
        public bool Vstrechat
        {
            get => _Vstrechat;
            set
            {
                _Vstrechat = value;
                OnPropertyChanged();
            }
        }

        string _Vokzal;
        [DisplayName(DisplayName = "Вокзал прибытие")]
        public string Vokzal
        {
            get => _Vokzal;
            set
            {
                _Vokzal = value;
                OnPropertyChanged();
            }
        }

        DateTime? _Uvolnenie;        

        [DisplayName(DisplayName = "Предполагаемая дата увольнения")]
        public DateTime? Uvolnenie
        {
            get => _Uvolnenie;
            set
            {
                _Uvolnenie = value;
                OnPropertyChanged();
            }
        }
    }
}
