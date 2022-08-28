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
    public class MedKomZayavkaViewModel : ValidatableObject// : System.ComponentModel.INotifyPropertyChanged
    {
       /* public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(prop));
        }*/

        Repository repository = new Repository();
        public MedKomZayavkaViewModel(int? medKomZayavkaId =null)
        {
            Validator = GetValidator();
            Persons = new ObservableCollection<MedKomZayavkaPersonViewModel>();

            Depos.Clear(); foreach (var d in repository.GetDeposMed()) Depos.Add(d);

            var db = new ProvodnikContext();
            if (medKomZayavkaId .HasValue)
            {
                Id = medKomZayavkaId ;
                MainWindow.Mapper.Value.Map(db.MedKomZayavki.Single(pp => pp.Id == medKomZayavkaId ), this);

                var qq = (from pd in db.MedKomZayavkaPersons join p in db.Persons on pd.PersonId equals p.Id
                          where pd.MedKomZayavkaId == medKomZayavkaId 
                          select new { pd, p }).ToList();
                                
                foreach (var q in qq)
                {
                    MedKomZayavkaPersonViewModel pe = new MedKomZayavkaPersonViewModel();
                    MainWindow.Mapper.Value.Map(q.pd, pe);
                    MainWindow.Mapper.Value.Map(q.p, pe);
                    pe.MedKomZayavkaViewModel = this;
                    Persons.Add(pe);

                }
//Persons.Add(MainWindow.Mapper.Value.Map< MedKomZayavkaPersonViewModel>(db.MedKomZayavkas.Single(pp => pp.Id == MedKomZayavkaId), this);
            }
            else
            {
                var lastZayavka = db.MedKomZayavki.OrderByDescending(x => x.Id).FirstOrDefault();
                if (lastZayavka != null && !string.IsNullOrEmpty(lastZayavka.BolnicaName))
                {
                    BolnicaName = lastZayavka.BolnicaName;
                    BolnicaAdres = lastZayavka.BolnicaAdres;
                }
                else
                {
                    BolnicaName = @"Поликлиника: ЧУЗ «КБ «РЖД - Медицина» г.Новосибирск»  
Псих.освидетельствование:Медицинский психодиагностический центр ""МЕНТАЛ КОНСАЛТИНГ""";
                    BolnicaAdres = @"Поликлиника: г.Новосибирск, ул.Сибирская, 21
Псих.осведетельствование: г.Новосибирск, ул.Красный проспект, 99";
                }
            }
        }

        public ObservableCollection<MedKomZayavkaPersonViewModel> Persons { get; set; }

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

        public ObservableCollection<string> Depos { get; set; } = new ObservableCollection<string>();


        public string _Depo;
        [DisplayName(DisplayName = "Депо приписки")]
        public string Depo
        {
            get => _Depo;
            set
            {
                if (_Depo != value)
                {
                    _Depo = value;

                    // ChangeDepoRod();
                    OnPropertyChanged();
                }
            }
        }

        public string _BolnicaName;
        [DisplayName(DisplayName = "Наименование больницы")]
        public string BolnicaName
        {
            get => _BolnicaName;
            set
            {
                if (_BolnicaName != value)
                {
                    _BolnicaName = value;
                    OnPropertyChanged();
                }
            }
        }

        public string _BolnicaAdres;
        [DisplayName(DisplayName = "Адрес больницы")]
        public string BolnicaAdres
        {
            get => _BolnicaAdres;
            set
            {
                if (_BolnicaAdres != value)
                {
                    _BolnicaAdres = value;
                    OnPropertyChanged();
                }
            }
        }

        public DateTime? _NaprMedZakazanoDat;
        public DateTime? NaprMedZakazanoDat
        {
            get => _NaprMedZakazanoDat;
            set
            {
                if (_NaprMedZakazanoDat != value)
                {
                    _NaprMedZakazanoDat = value;
                    OnPropertyChanged();
                }
            }
        }

        public DateTime? _NaprMedPoluchenoDat;
        public DateTime? NaprMedPoluchenoDat
        {
            get => _NaprMedPoluchenoDat;
            set
            {
                if (_NaprMedPoluchenoDat != value)
                {
                    _NaprMedPoluchenoDat = value;
                    OnPropertyChanged();
                }
            }
        }

        private IObjectValidator GetValidator()
        {
            var builder = new ValidationBuilder<MedKomZayavkaViewModel>();

            builder.RuleFor(vm => vm.Name).MyNotEmpty();
            builder.RuleFor(vm => vm.Depo).MyNotEmpty();

            return builder.Build(this);
        }
    }
}
