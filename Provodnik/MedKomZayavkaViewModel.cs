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


        DateTime? _Dat;
        [DisplayName(DisplayName = "Планируемая дата трудоустройства")]
        public DateTime? Dat
        {
            get => _Dat;
            set
            {
                _Dat = value;
                OnPropertyChanged();
            }
        }
        private IObjectValidator GetValidator()
        {
            var builder = new ValidationBuilder<MedKomZayavkaViewModel>();

            builder.RuleFor(vm => vm.Name).MyNotEmpty();
            builder.RuleFor(vm => vm.Dat).MyNotEmptyDat();

            return builder.Build(this);
        }
    }
}
