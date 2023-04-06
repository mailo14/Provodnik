using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;

namespace Provodnik
{
    // [DbConfigurationType(typeof(MyDbConfiguration))]
    public class ProvodnikContext : DbContext
    {
        public ProvodnikContext()
        //    : base("DefaultConnection")
        :base(App.CurrentConfig.DbConnection)
        {
#if DEBUG
            Database.Log = s =>
            {
                System.Diagnostics.Debug.WriteLine("----------------------------------------");
                System.Diagnostics.Debug.WriteLine(s);
            };
#endif

            // Database.SetInitializer(new ProvodnikDbInitializer());
        }
        public virtual DbSet<Person> Persons { get; set; }
        public virtual DbSet<DocType> DocTypes { get; set; }
        public virtual DbSet<PersonDoc> PersonDocs { get; set; }
        public virtual DbSet<SendGroup> SendGroups { get; set; }
        public virtual DbSet<SendGroupPerson> SendGroupPersons { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<MedKomZayavka> MedKomZayavki { get; set; }
        public virtual DbSet<MedKomZayavkaPerson> MedKomZayavkaPersons { get; set; }
    }

    [Table("Person")]
    public class Person
    {
        public int Id { get; set; }
        //[Index]
        //[MaxLength(100)]
        public string Fio { get; set; }
        public string BadgeRus { get; set; }
        public string BadgeEng { get; set; }
        public string Pol { get; set; }
        public string Grazdanstvo { get; set; }
        public string Phone { get; set; }
        public string Vk { get; set; }
        //[Index]
        public string Otryad { get; set; }
        public string Sezon { get; set; }
        //[Index]
        public bool IsNovichok { get; set; }
        public string Dogovor { get; set; }
        public DateTime? DogovorDat { get; set; }
        //[Index]
        public string UchZavedenie { get; set; }
        public string UchForma { get; set; }
        public string UchFac { get; set; }
        public string UchGod { get; set; }
        //public bool IsUchFinish { get; set; }
        //public bool HasLgota { get; set; }
        public string RodFio { get; set; }
        public string RodPhone { get; set; }
        public bool HasForma { get; set; }
        public bool FormaPoluchena { get; set; }
        public string RazmerFormi { get; set; }
        public DateTime? BirthDat { get; set; }
        public string MestoRozd { get; set; }
        public string PaspSeriya { get; set; }
        public string PaspNomer { get; set; }
        public string PaspKodPodr { get; set; }
        public string PaspVidan { get; set; }
        public DateTime? VidanDat { get; set; }
        public string PaspAdres { get; set; }
        public DateTime? VremRegDat { get; set; }
        public string FactAdres { get; set; }
        public string Snils { get; set; }
        public string Inn { get; set; }
        public DateTime? PsihDat { get; set; }
        public bool IsPsih { get; set; }
        public bool IsPsihZabral { get; set; }
        public DateTime? SanKnizkaDat { get; set; }
        public bool IsSanKnizka { get; set; }
        public DateTime? SanGigObuchenieDat { get; set; }
        public bool IsSanGigObuchenie { get; set; }
        public DateTime? MedKommDat { get; set; }
        public bool IsMedKomm { get; set; }
        public bool IsMedKommNeGoden { get; set; }
        public bool IsGologram { get; set; }
        public DateTime? NaprMedZakazanoDat { get; set; }
        public bool IsNaprMedZakazano { get; set; }
        public DateTime? NaprMedPoluchenoDat { get; set; }
        public bool IsNaprMedPolucheno { get; set; }
        public bool IsNaprMedPoluchenoNePoln { get; set; }
        public bool IsNaprMedPoluchenoSOshibkoi { get; set; }
        public DateTime? NaprMedVidanoDat { get; set; }
        public bool IsNaprMedVidano { get; set; }
        public bool IsTrudoustroen { get; set; }
        public string TrudoustroenDepo { get; set; }
        public string VaccineSert { get; set; }
        public DateTime? VaccineSertDat { get; set; }
        public DateTime? VaccineSertDatTo { get; set; }
        /*public DateTime? VaccineOneDat { get; set; }
        public bool IsVaccineOne { get; set; }
        public DateTime? VaccineOneOnlyDat { get; set; }
        public bool IsVaccineOneOnly { get; set; }
        public DateTime? VaccineTwoDat { get; set; }
        public bool IsVaccineTwo { get; set; }
        public DateTime? RevacDat { get; set; }*/
        //[Index]
        public string UchebCentr { get; set; }
        //[Index]
        public string UchebGruppa { get; set; }
        public DateTime? UchebStartDat { get; set; }
        public DateTime? UchebEndDat { get; set; }
        public bool IsUchebFinish { get; set; }
        public DateTime? PraktikaDat { get; set; }
        public bool IsPraktika { get; set; }
        public DateTime? ExamenDat { get; set; }
        public bool IsExamen { get; set; }
        public bool IsExamenFailed { get; set; }

        public DateTime? SertificatDat { get; set; }
        public bool IsSertificatError { get; set; }
        public string SertificatError { get; set; }

        public string Srez { get; set; }
        public DateTime? VihodDat { get; set; }
        public string Gorod { get; set; }
        public bool IsVibil { get; set; }
        //[Index]
        public string VibilPrichina { get; set; }
        public bool AllPasport { get; set; }
        public bool AllScans { get; set; }
        public string Messages { get; set; }
        public string Zametki { get; set; }
        public virtual List<PersonDoc> PersonDocs { get; set; }
        public string NaprMedDepo { get;  set; }
        public string NaprMedBolnicaName { get;  set; }
        public bool InSpisokSb { get;  set; }
    }


    [Table("DocType")]
    public class DocType
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }
        //public int TypeId { get; set; }
        public int OrderId { get; set; } = 1;
        public bool IsObyazat { get; set; } = true;
        public string Description { get; set; }
    }

    [Table("PersonDoc")]
    public class PersonDoc
    {
        public int Id { get; set; }
        //[ForeignKey("Person")]
        public int PersonId { get; set; }
        public virtual Person Person { get; set; }
        public int DocTypeId { get; set; }
        //public DocType DocType { get; set; }
        public string FileName { get; set; }
        public DateTime? PrinesetK { get; set; }
        public bool IsActive { get; set; }
    }

    [Table("SendGroup")]
    public class SendGroup
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string City { get; set; }
        public string PeresadSt { get; set; }
        public string Depo { get; set; }
        public string DepoRod { get; set; }
        public string Sp { get; set; }
        public string Filial { get; set; }
        public string RegOtdelenie { get; set; }
        public string Poezd { get; set; }
        public string Vagon { get; set; }
        public DateTime? OtprDat { get; set; }
        public DateTime? PribDat { get; set; }
        public string PribTime { get; set; }
        public bool Vstrechat { get; set; }
        public string Vokzal { get; set; }
        public string Marshrut { get; set; }
        public DateTime? Uvolnenie { get; set; }
    }

    [Table("SendGroupPerson")]
    public class SendGroupPerson
    {
        public int Id { get; set; }
        public int SendGroupId { get; set; }
        public int PersonId { get; set; }
        public bool IsMain { get; set; }
    }

    [Table("MedKomZayavka")]
    public class MedKomZayavka
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime? Dat { get; set; }
        public string Depo { get; set; }
        public string BolnicaName { get; set; }
        public string BolnicaAdres { get; set; }
        public DateTime? NaprMedZakazanoDat { get; set; }
        public DateTime? NaprMedPoluchenoDat { get; set; }
    }

    [Table("MedKomZayavkaPerson")]
    public class MedKomZayavkaPerson
    {
        public int Id { get; set; }
        public int MedKomZayavkaId { get; set; }
        public int PersonId { get; set; }
    }

    [Table("User")]
    public class User
    {
        public int Id { get; set; }
        public string Login { get; set; }
        public string Wand { get; set; }        
        public string Tip { get; set; }
    }

    /* public class ProvodnikDbInitializer : DropCreateDatabaseAlways<ProvodnikContext>
     {
         protected override void Seed(ProvodnikContext context)
         {
             context.DocTypes.AddRange(new List<DocType>
             {
                 new DocType { Id = 1, OrderId = 1, Description = "Паспорт",IsObyazat=true },
                 new DocType { Id = 2, OrderId = 1, Description = "Прописка",IsObyazat=false },
                 new DocType { Id = 3, OrderId = 1, Description = "СНИЛС",IsObyazat=true },
                 new DocType { Id = 4, OrderId = 1, Description = "ИНН",IsObyazat=true },
                 new DocType { Id = 5, OrderId = 1, Description = "Психиатрическое освидетельствование",IsObyazat=true },
                 new DocType { Id = 6, OrderId = 1, Description = "Заключение ВЭК",IsObyazat=true },
                 new DocType { Id = 7, OrderId = 1, Description = "Заключение ВЭК 2",IsObyazat=true },
                 //new DocType { Id = 8, OrderId = 1, Description = "Заключение ВЭК 3",IsObyazat=true },
                 new DocType { Id = 9, OrderId = 1, Description = "Согласие на обработку персональных данных",IsObyazat=true },
                 new DocType { Id = 10, OrderId = 1, Description = "Свидетельство о присвоении профессии",IsObyazat=true },
                 new DocType { Id = 11, OrderId = 1, Description = "Реквизиты банковской (зарплатной) карты",IsObyazat=true },

                 new DocType { Id = 12, OrderId = 1, Description = "Приписное 1", IsObyazat = false },
             new DocType { Id = 13, OrderId = 1, Description = "Приписное 2", IsObyazat = false },
             new DocType { Id = 14, OrderId = 1, Description = "Военный билет", IsObyazat = false },
             new DocType { Id = 15, OrderId = 1, Description = "Справка с места учебы", IsObyazat = false },
             new DocType { Id = 16, OrderId = 1, Description = "Справка-подтверждение  МООО  «РСО»" ,IsObyazat=true},
             new DocType { Id = 17, OrderId = 1, Description = "Миграционная карта 1", IsObyazat = false },
             new DocType { Id = 18, OrderId = 1, Description = "Миграционная карта 2", IsObyazat = false },
             new DocType { Id = 19, OrderId = 1, Description = "Временная регистрация 1", IsObyazat = false },
             new DocType { Id = 20, OrderId = 1, Description = "Временная регистрация 2", IsObyazat = false },
             new DocType { Id = 21, OrderId = 1, Description = "Свидетельство о вакцинации", IsObyazat = true },

 }
        );
             base.Seed(context);
         }
     }*/
    public class DocConsts
    {
       public const int Паспорт = 1;
       public const int Прописка = 2;
        public const int СНИЛС = 3;
        public const int ИНН = 4;
        public const int Психосвидетельствование = 5;
        public const int ЗаключениеВЭК = 6;
        public const int ЗаключениеВЭК2 = 7;
        public const int СогласиеПерс = 9;
        public const int СвидетельствоПрофессии = 10;
        public const int РеквизитыКарты = 11;
        public const int Приписное1 = 12;
        public const int Приписное2 = 13;
        public const int ВоенныйБилет = 14;
        public const int СправкаУчебы = 15;
        public const int СправкаРСО = 16;
        public const int Миграционная1 = 17;
        public const int Миграционная2 = 18;
        public const int ВремРегистрация1 = 19;
        public const int ВремРегистрация2 = 20;
        public const int СвидетельствоВакцинации = 21;
        public const int ТрудоваяКнижка1 = 22;
        public const int ТрудоваяКнижка2 = 23;
        public const int СвидетельствоВакцинации2 = 24;
    }
}