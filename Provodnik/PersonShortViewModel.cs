using ReactiveValidation;
using ReactiveValidation.Attributes;
using ReactiveValidation.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Media.Imaging;

namespace Provodnik
{
    public class PersonShortViewModel//: PersonViewModel //
        : System.ComponentModel.INotifyPropertyChanged//: PersonDoc
    {
        /*public PersonShortViewModel(int? personId):base(personId)
        {

        }*/
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(prop));
        }

        public int Id { get; set; }
        public string Phone { get; set; }
        public string Fio { get; set; }
        public string BadgeRus { get; set; }
        public string BadgeEng { get; set; }
        public DateTime? BirthDat { get; set; }
        public string MestoRozd { get; set; }
        public string PaspAdres { get; set; }
        public string UchZavedenie { get; set; }
        public string UchebGruppa { get; set; }
        public string NaprMedDepo { get; set; }
        public bool IsNovichok { get; set; }
        public string Novichok => IsNovichok ? "" : "старичок";

        public string UchFac { get; set; }
        public string Snils { get; set; }
        public string Inn { get; set; }

        public bool IsPsih { get; set; }
        public DateTime? SanKnizkaDat { get; set; }
        public bool IsSanKnizka { get; set; }
        public DateTime? SanGigObuchenieDat { get; set; }
        public bool IsSanGigObuchenie { get; set; }
        public DateTime? ExamenDat { get; set; }
        public bool IsExamen { get; set; }
        public bool IsPraktika { get; set; }
        public bool IsGologram { get; set; }
        public DateTime? PraktikaDat { get; set; }
        public bool IsMedKomm { get; set; }
        public DateTime? MedKommDat { get; set; }
        public bool IsNaprMedVidano { get; set; }

        public string Messages { get; set; }
        public string Zametki{ get; set; }
     public string    Dogovor { get; set; }
        public string       Otryad { get; set; }


        public bool _IsSelected;
        public bool IsSelected
        {
            get => _IsSelected;
            set
            {
                _IsSelected = value;
                Parent.RefreshIsPersonsSelected();
                OnPropertyChanged();
            }
        }
        public int _Index;
        public int Index
        {
            get => _Index;
            set
            {
                _Index = value;
                OnPropertyChanged();
            }
        }

        public int? Vozrast => Helper.GetVozrast(BirthDat);

        public PersonsViewModel Parent { get; internal set; }
    }
}