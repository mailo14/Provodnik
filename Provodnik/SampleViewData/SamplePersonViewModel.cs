using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Provodnik.SampleViewData
{
    public class SamplePersonViewModel : PersonViewModel
    {

        public SamplePersonViewModel()
          : base(null,true)
        {
            Fio = "Иванов Иван Иванович";
            Phone = "9231234567";
            PaspVidan= @"обл Новосибирская 
гор.Новосибирск
район Советский
ул.Экваторная
дом 3, кв. 25";//город"+Environment.NewLine+ "город" + Environment.NewLine+"город" + Environment.NewLine + "город";
            Documents = new ObservableCollection<PersonDocViewModel>()
            { 
                new PersonDocViewModel() {
                    Description = "Паспорт",
                    //Bitmap = new System.Windows.Controls.Image() { Source = new BitmapImage(new Uri("pack://application:,,,/pic/loading.gif")) },
                    PrinesetK = new DateTime(2019, 05, 23) }
            ,    new PersonDocViewModel(){
                    Description ="Справка-подтверждение  МООО  «РСО»",
                    //Bitmap =new System.Windows.Controls.Image(){Source= new BitmapImage(new Uri("pack://application:,,,/pic/loading.gif")) },
                    }
            ,    new PersonDocViewModel(){
                    Description ="Согласие на обработку персональных данных",
                    //Bitmap =new System.Windows.Controls.Image(){Source= new BitmapImage(new Uri("pack://application:,,,/pic/loading.gif")) },
                    }
            };
            /*base.Customer.CalculateRank();
            base.Customer.Addresses.Add(
                new Address() { LineOne = "125 North St", City = "Greatville", State = "CA", Zip = "98004" });
            base.Customer.Addresses.Add(
                new Address() { LineOne = "3000 1st St NE", City = "Coolville", State = "CA", Zip = "98004" });*/
        }
    }
}
