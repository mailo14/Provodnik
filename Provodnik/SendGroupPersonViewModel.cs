using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Provodnik
{
  public  class SendGroupPersonViewModel : System.ComponentModel.INotifyPropertyChanged//: PersonDoc
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
        public string UchForma { get; set; }
        public string Phone { get; set; }
        public string Messages { get; set; }

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

        private bool _IsTrudoustroen;
        public bool IsTrudoustroen
        {
            get => _IsTrudoustroen;
            set
            {
                _IsTrudoustroen = value;
                OnPropertyChanged();
            }
        }

        bool _IsMain;
        public bool IsMain
        {
            get => _IsMain;
            set
            {//TODO event
                if (value && SendGroupViewModel!=null)
                {
                    var p = SendGroupViewModel.Persons.FirstOrDefault(pp => pp.IsMain);
                    if (p != null) p.IsMain = false;
                }
                _IsMain = value;
                OnPropertyChanged();
            }
        }
        //public List<string> Errors { get; set; }
        public SendGroupViewModel SendGroupViewModel { get; set; }//TODO event
    }
}
