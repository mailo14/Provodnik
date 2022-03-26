using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Provodnik
{
  public  class MedKomZayavkaPersonViewModel : System.ComponentModel.INotifyPropertyChanged//: PersonDoc
    {
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(prop));
        }
        public int? Id { get; set; }
        public int PersonId { get; set; }
        public string Fio { get; set; }
        public string Phone { get; set; }
        public string Messages { get; set; }
        public DateTime? VihodDat { get; set; }


        public bool IsNaprMedZakazano { get; set; }

        public bool IsNaprMedPolucheno { get; set; }
        public bool IsNaprMedPoluchenoNePoln { get; set; }
        public bool IsNaprMedPoluchenoSOshibkoi { get; set; }

        public bool IsNaprMedVidano { get; set; }

        private DateTime? _BirthDat;
        public DateTime? BirthDat
        {
            get => _BirthDat;
            set
            {
                _BirthDat = value;
                OnPropertyChanged();
            }
        }
        //public List<string> Errors { get; set; }
        public MedKomZayavkaViewModel MedKomZayavkaViewModel { get; set; }//TODO event
    }
}
