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
    public class TrudoustroistvoViewModel : ValidatableObject// : System.ComponentModel.INotifyPropertyChanged
    {
        /* public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
         public void OnPropertyChanged([CallerMemberName]string prop = "")
         {
             if (PropertyChanged != null)
                 PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(prop));
         }*/
        bool isLoading = false;
        Repository repository = new Repository();
        public TrudoustroistvoViewModel(int? TrudoustroistvoId =null)
        {
            Validator = GetValidator();
            Persons = new ObservableCollection<TrudoustroistvoPersonViewModel>();

            Depos.Clear(); foreach (var d in repository.GetDeposMed()) Depos.Add(d);

            var db = new ProvodnikContext();
            if (TrudoustroistvoId .HasValue)
            {
                Id = TrudoustroistvoId ;
                isLoading = true;
                MainWindow.Mapper.Value.Map(db.Trudoustroistva.Single(pp => pp.Id == TrudoustroistvoId ), this);
                //if (!string.IsNullOrEmpty(BolnicaName) && !BolnicaNames.Contains(BolnicaName)) BolnicaNames.Add(BolnicaName);
                isLoading = false;

                var qq = (from pd in db.TrudoustroistvoPersons join p in db.Persons on pd.PersonId equals p.Id
                          where pd.TrudoustroistvoId == TrudoustroistvoId 
                          select new { pd, p }).ToList();
                                
                foreach (var q in qq)
                {
                    TrudoustroistvoPersonViewModel pe = new TrudoustroistvoPersonViewModel();
                    MainWindow.Mapper.Value.Map(q.pd, pe);
                    MainWindow.Mapper.Value.Map(q.p, pe);
                    pe.TrudoustroistvoViewModel = this;
                    Persons.Add(pe);
                }
//Persons.Add(MainWindow.Mapper.Value.Map< TrudoustroistvoPersonViewModel>(db.Trudoustroistvos.Single(pp => pp.Id == TrudoustroistvoId), this);
            }
        }

        public ObservableCollection<TrudoustroistvoPersonViewModel> Persons { get; set; }

        public int? Id { get; set; }
        public int Kolvo { get; set; }

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

        public DateTime? _EndDate;
        public DateTime? EndDate
        {
            get => _EndDate;
            set
            {
                if (_EndDate != value)
                {
                    _EndDate = value;
                    OnPropertyChanged();
                }
            }
        }

        public DateTime? _StartDate;
        public DateTime? StartDate
        {
            get => _StartDate;
            set
            {
                if (_StartDate != value)
                {
                    _StartDate = value;
                    OnPropertyChanged();
                }
            }
        }

        private IObjectValidator GetValidator()
        {
            var builder = new ValidationBuilder<TrudoustroistvoViewModel>();

            builder.RuleFor(vm => vm.StartDate).MyNotEmptyDat();
            builder.RuleFor(vm => vm.EndDate).MyNotEmptyDat();
            builder.RuleFor(vm => vm.Depo).MyNotEmpty();

            return builder.Build(this);
        }
    }
}
