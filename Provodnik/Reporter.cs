using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Provodnik
{
    public class Reporter
    {
        public async void ExportToExcel(List<int> ids)
        {
            try
            {
                // await new ProgressRunner().RunAsync(doAction);      
                await new ProgressRunner().RunAsync(
                    new Action<ProgressHandler>((progressChanged) =>
                    {

                        progressChanged(1, "Экспорт");


                        excelReport excel = new excelReport();
                        excel.Init("Экспорт.xltx", $"Экспорт{DateTime.Now.Ticks}.xlsx");//,visible:true);//, otchetDir: otchetDir);

                        int ri = 1;
                        var db = new ProvodnikContext();
                        var rr = db.Persons.Where(pp => ids.Contains(pp.Id)).ToList();
                        List<(string Label, Func<Person, string> Func)> pairs = new List<(string, Func<Person, string>)>()
                        {
                            ("Номер договора",(r) => r.Dogovor),
                            ("Дата договора" ,(r) => r.DogovorDat?.ToString("dd.MM.yyyy")),
                            ("ФИО" ,(r) => r.Fio),
                            ("Пол" ,(r) => r.Pol),
                            ("Телефон" ,(r) => Helper.FormatPhone(r.Phone)),
                            ("Возраст" ,(r) => Helper.GetVozrast(r.BirthDat)?.ToString()),
                            ("Барабинск",(r) => r.IsBarabinsk ? "да" : "нет"),
                            ("Куйбышев" ,(r) => r.IsKuibishev ? "да" : "нет"),
                            ("Сезон" ,(r) => r.Sezon),
                            ("Отряд" ,(r) => r.Otryad),
                            ("Круглогодичный отряд" ,(r) => r.IsKruglogodOtryad ? "да" : "нет"),
                            ("Лето" ,(r) => r.IsLeto ? "да" : "нет"),
                            ("Логин ВК" ,(r) => r.Vk),
                            ("Логин Telegram" ,(r) => r.Tg),
                            ("Гражданство" ,(r) => r.Grazdanstvo),
                            ("Новичок" ,(r) => r.IsNovichok ? "да" : "нет"),
                            ("Учебное заведение" ,(r) => r.UchZavedenie),
                            ("Факультет" ,(r) => r.UchFac),
                            ("Форма обучения" ,(r) => r.UchForma),
                            ("Год окончания обучения" ,(r) => r.UchGod),
                            ("Есть льгота" ,(r) => r.UchForma== UchFormaConsts.Ochnaya ? "да" : "нет"),
                            ("ФИО родителя" ,(r) => r.RodFio),
                            ("Контактный телефон родителей" ,(r) => Helper.FormatPhone(r.RodPhone)),
                            ("Есть форма" ,(r) => r.HasForma ? "да" : "нет"),
                            ("Размер формы" ,(r) => r.RazmerFormi),
                            ("Дата рождения" ,(r) => r.BirthDat?.ToString("dd.MM.yyyy")),
                            ("Место рождения" ,(r) => r.MestoRozd),
                            ("Серия паспорта" ,(r) => r.PaspSeriya),
                            ("Номер паспорта" ,(r) => r.PaspNomer),
                            ("Кем выдан" ,(r) => r.PaspVidan),
                            ("Код подразделения" ,(r) => r.PaspKodPodr),
                            ("Когда выдан" ,(r) => r.VidanDat?.ToString("dd.MM.yyyy")),
                            ("Прописка по паспорту" ,(r) => r.PaspAdres),
                            ("Адрес фактического места жительства" ,(r) => r.FactAdres),
                            ("Дата врем.регистрации" ,(r) => r.VremRegDat?.ToString("dd.MM.yyyy")),
                            ("Номер ПФ (снилс)" ,(r) => Helper.FormatSnils(r.Snils)),
                            ("ИНН" ,(r) => r.Inn?.ToString()),
                            ("Направление МК Заказано", (r) => r.IsNaprMedZakazano ? "да" : "нет"),
                            ("Заказано дата", (r) => r.NaprMedZakazanoDat?.ToString("dd.MM.yyyy")),
                            ("От депо", (r) => r.NaprMedDepo),
                            ("В больнице", (r) => r.NaprMedBolnicaName),
                            ("Направление МК Получено", (r) => r.IsNaprMedPolucheno ? "да" : "нет"),
                            ("Получено дата", (r) => r.NaprMedPoluchenoDat?.ToString("dd.MM.yyyy")),
                            ("получено не в полном объёме", (r) => r.IsNaprMedPoluchenoNePoln ? "да" : "нет"),
                            ("ошибка в направлении", (r) => r.IsNaprMedPoluchenoSOshibkoi ? "да" : "нет"),
                            ("Направление МК Выдано", (r) => r.IsNaprMedVidano ? "да" : "нет"),
                            ("Выдано дата", (r) => r.NaprMedVidanoDat?.ToString("dd.MM.yyyy")),
                            ("Санкнижка дата", (r) => r.SanKnizkaDat?.ToString("dd.MM.yyyy")),
                            ("Получена", (r) => r.IsSanKnizka ? "да" : "нет"),
                            ("Своя санкнижка", (r) => r.IsSvoyaSanKnizka ? "да" : "нет"),
                            ("Сан.гиг. обучение дата", (r) => r.SanGigObuchenieDat?.ToString("dd.MM.yyyy")),
                            ("Пройдено", (r) => r.IsSanGigObuchenie ? "да" : "нет"),
                            ("Медкомиссия дата", (r) => r.MedKommDat?.ToString("dd.MM.yyyy")),
                            ("Пройдена", (r) => r.IsMedKomm ? "да" : "нет"),
                            ("Не годен", (r) => r.IsMedKommNeGoden ? "да" : "нет"),
                            ("Учебный центр" ,(r) => r.UchebCentr),
                            ("Группа" ,(r) => r.UchebGruppa),
                            ("Закончил" ,(r) => r.IsUchebFinish ? "да" : "нет"),
                            ("Дата окончания обучения" ,(r) => r.UchebEndDat?.ToString("dd.MM.yyyy")),
                            ("Экзамен Пройден" ,(r) => r.IsExamen? "да" : "нет"),
                            ("Экзамен Провален/пропущен" ,(r) => r.IsExamenFailed ? "да" : "нет"),
                            ("Сертификат" ,(r) => r.SertificatDat?.ToString("dd.MM.yyyy")),
                            ("Дата выхода" ,(r) => r.VihodDat?.ToString("dd.MM.yyyy")),
                            ("Желаемый город работы" ,(r) => r.Gorod),
                            ("В списке на проверку СБ" ,(r) => r.InSpisokSb ? "да" : "нет"),
                            ("Трудоустроен" ,(r) => r.IsTrudoustroen ? "да" : "нет"),
                            ("Депо" ,(r) => r.TrudoustroenDepo),
                            ("Заметки" ,(r) => r.Zametki),
                            ("Выбыл" ,(r) => r.IsVibil ? "да" : "нет"),
                            ("Причина" ,(r) => r.VibilPrichina),
                            ("Все сканы" ,(r) => r.AllScans ? "да" : "нет"),
                            ("Примечания" ,(r) => r.Messages?.Replace(Environment.NewLine, "; ")),
                        };
                        int c = 0;
                        foreach (var pair in pairs)
                        {
                            c++; excel.cell[1, c].value2 = pair.Label;
                        }
                       
                        progressChanged(5);
                        var share = 95.0 / rr.Count;
                        var errors = new List<string>();
                        foreach (var r in rr)//vm.Persons)
                        {
                            ri++;
                            c = 0;
                            foreach (var pair in pairs)
                            {
                                c++; excel.cell[ri, c].value2 = pair.Func(r);
                            }
                            progressChanged(share);
                        }
                        excel.setAllBorders(excel.get_Range("A1", "BT" + ri));
                        excel.mySheet.Columns.AutoFit();
                        excel.myExcel.Visible = true;
                        //  excel.Finish();
                        if (errors.Any())
                            throw new Exception("Выявлены ошибки примечаний для:" + Environment.NewLine +string.Join(Environment.NewLine+"  ", errors)
                                +Environment.NewLine+Environment.NewLine + "Возможно был сбой при сохранении карточки. Проверьте карточку и сохраните");
                    })


                    );
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        /*public async void PrintSendGroup(List<int> ids)
        {
            try
            {
                // await new ProgressRunner().RunAsync(doAction);      
                await new ProgressRunner().RunAsync(
                    new Action<ProgressHandler>((progressChanged) =>
                    {


                        progressChanged(1, "Формирование документов");

                        progressChanged(5);

                        //App.setCursor(true);
                        try
                        {
                            VSOP1();// VSOP2();
                                    //VSOP3();
                            if (vm.City != "Новосибирск")
                                F6();
                            PismoLgoti();
                            ReestrPeredachi();

                            string path = otchetDir;
                            if (vm.City != "Новосибирск") path += $@"\Ф6(Архив)_{vm.OtprDat.Value.ToString("dd.MM.yyyy")}_Новосибирск-{GetDepoCityForHeader(vm)}_{vm.Persons.Count}чел";
                            else path += $@"\{vm.OtprDat.Value.ToString("dd.MM.yyyy")}_Новосибирск_{vm.Persons.Count}чел";
                            if (!Directory.Exists(path))
                                Directory.CreateDirectory(path);
                            else;//TODO del?


                            var ids = vm.Persons.Select(pp => pp.PersonId).ToList();
                            var db = new ProvodnikContext();
                            //foreach (var d in vm.Persons)

                            var qq = (from p in db.Persons
                                      join pd in db.PersonDocs on p.Id equals pd.PersonId
                                      join pdt in db.DocTypes on pd.DocTypeId equals pdt.Id
                                      where ids.Contains(p.Id) && pd.FileName != null
                                      group new { pdt.Description, pd.FileName } by p.Fio into gr
                                      select gr).ToList();//new { p.Fio, pdt.Description, pd.FileName }

                            using (var client = new FluentFTP.FtpClient())
                            {
                                App.ConfigureFtpClient(client);
                                client.Connect();

                                foreach (var g in qq)
                                {
                                    var fio = new StringHelper().ParseFio(g.Key);
                                    var fioInic = fio[0];
                                    if (fio[1].Length > 0) fioInic += $" {fio[1][0]}.";
                                    if (fio[2].Length > 0) fioInic += $" {fio[2][0]}.";

                                    foreach (var d in g)
                                    {
                                        var fileName = $@"{path}\{g.Key}\{fioInic}_{d.Description}.jpg";

                                        var result = client.DownloadFile(fileName, @"http/" + d.FileName, FtpLocalExists.Overwrite, FtpVerify.Retry);//, true, FtpVerify.Retry);
                                        if (result != FtpStatus.Success)
                                            throw new Exception("Не удалось загрузить файл " + System.IO.Path.GetFileName(fileName));
                                    }
                                }
                            }
                        }
                        catch (Exception ex) { MessageBox.Show(ex.Message, "", MessageBoxButton.OK, MessageBoxImage.Error); }




                    })


                    );
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }*/

        public async void NapravleniyaMed(List<PersonShortViewModel> persons)
        {
            try
            {
                // await new ProgressRunner().RunAsync(doAction);      
                await new ProgressRunner().RunAsync(
                    new Action<ProgressHandler>((progressChanged) =>
                    {

                        progressChanged(1, "Формирование направлений");

                        var path = (string.Format("{0}\\_шаблоны\\" + "Направление мед.dotx", AppDomain.CurrentDomain.BaseDirectory));
                        Microsoft.Office.Interop.Word.Application wordApp = new Microsoft.Office.Interop.Word.Application { Visible = false };
                        Microsoft.Office.Interop.Word.Document aDoc = wordApp.Documents.Add(path/*, ReadOnly: false, Visible: true*/);
                        aDoc.Activate();

                        object missing = Missing.Value;
                        object what = Microsoft.Office.Interop.Word.WdGoToItem.wdGoToLine;
                        object which = Microsoft.Office.Interop.Word.WdGoToDirection.wdGoToLast;


                        Microsoft.Office.Interop.Word.Range baseRange = aDoc.Content;
                        baseRange.Cut();

                        Microsoft.Office.Interop.Word.Range range = null;

                        progressChanged(5);
                        var share = 95.0 / persons.Count;
                        foreach (var p in persons)
                        {
                            if (range != null)//!firstRow
                            {
                                range = aDoc.GoTo(ref what, ref which, ref missing, ref missing);
                                range.InsertBreak(Microsoft.Office.Interop.Word.WdBreakType.wdPageBreak);
                            }

                            range = aDoc.GoTo(ref what, ref which, ref missing, ref missing);

                            range.Paste();

                            range.Find.ClearFormatting();
                            range.Find.Execute(FindText: "{fio}", ReplaceWith: p.Fio, Replace: Microsoft.Office.Interop.Word.WdReplace.wdReplaceAll);
                            if (p.BirthDat.HasValue)
                                range.Find.Execute(FindText: "{birthDat}", ReplaceWith: p.BirthDat.Value.ToString("dd.MM.yyyy"), Replace: Microsoft.Office.Interop.Word.WdReplace.wdReplaceAll);

                            progressChanged(share);
                        }
                        // string otchetDir = @"C:\_provodnikFTP";
                        //  aDoc.SaveAs(FileName: otchetDir + @"\Согласие на обработку персональных данных.DOCX");
                        wordApp.Visible = true;
                        wordApp.Activate();

                    })


                    );
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        public async void VedomostSanKnizki(List<PersonShortViewModel> persons)
        {
            try
            {
                await new ProgressRunner().RunAsync(
                    new Action<ProgressHandler>((progressChanged) =>
                    {
                        var path = (string.Format("{0}\\_шаблоны\\" + "Ведомость санкнижки.dotx", AppDomain.CurrentDomain.BaseDirectory));
                        Microsoft.Office.Interop.Word.Application wordApp = new Microsoft.Office.Interop.Word.Application { Visible = false };
                        Microsoft.Office.Interop.Word.Document aDoc = wordApp.Documents.Add(path);
                        aDoc.Activate();

                        object missing = Missing.Value;

                        Microsoft.Office.Interop.Word.Range range = aDoc.Content;
                        range.Find.ClearFormatting();
                        range.Find.Execute(FindText: "{Count}", ReplaceWith: persons.Count(), Replace: Microsoft.Office.Interop.Word.WdReplace.wdReplaceAll);
                        var table = aDoc.Tables[1];
                        int iRow = 2;

                        foreach (var p in persons)
                        {
                            if (iRow > 2) table.Rows.Add(ref missing);
                            table.Cell(iRow, 1).Range.Text = (iRow - 1).ToString() + '.';
                            table.Cell(iRow, 2).Range.Text = p.Fio;
                            table.Cell(iRow, 3).Range.Text = p.BirthDat?.ToString("dd.MM.yyyy");
                            table.Cell(iRow, 4).Range.Text = p.PaspAdres;
                            table.Cell(iRow, 5).Range.Text = "Проводник пассажирского вагона";

                            iRow++;
                        }
                        wordApp.Visible = true;
                        wordApp.Activate();
                    })
                    );
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        public async void VedomostMedKomms(List<PersonShortViewModel> persons, DateTime? selectedDate)
        {
            try
            {
                await new ProgressRunner().RunAsync(
                    new Action<ProgressHandler>((progressChanged) =>
                    {
                        var path = (string.Format("{0}\\_шаблоны\\" + "Ведомость мед.комисии.dotx", AppDomain.CurrentDomain.BaseDirectory));
                        Microsoft.Office.Interop.Word.Application wordApp = new Microsoft.Office.Interop.Word.Application { Visible = false };
                        Microsoft.Office.Interop.Word.Document aDoc = wordApp.Documents.Add(path);
                        aDoc.Activate();

                        object missing = Missing.Value;

                        Microsoft.Office.Interop.Word.Range range = aDoc.Content;
                        range.Find.ClearFormatting();

                        range.Find.Execute(FindText: "{дата}", ReplaceWith: selectedDate?.ToString("dd.MM.yyyy"), Replace: Microsoft.Office.Interop.Word.WdReplace.wdReplaceAll);


                        var table = aDoc.Tables[1];
                        int iRow = 2;

                        foreach (var p in persons)
                        {
                            if (iRow > 2) table.Rows.Add(ref missing);
                            table.Cell(iRow, 1).Range.Text = (iRow - 1).ToString() + '.';
                            table.Cell(iRow, 2).Range.Text = p.Fio;
                            table.Cell(iRow, 3).Range.Text = p.BirthDat?.ToString("dd.MM.yyyy");
                            table.Cell(iRow, 4).Range.Text = p.IsNovichok ? "Новичок" : "Старичок";
                            table.Cell(iRow, 5).Range.Text = p.NaprMedDepo;

                            iRow++;
                        }
                        wordApp.Visible = true;
                        wordApp.Activate();
                    })
                    );
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        public async void VedomostSanGig(List<PersonShortViewModel> persons)
        {
            try
            {
                await new ProgressRunner().RunAsync(
                    new Action<ProgressHandler>((progressChanged) =>
                    {
                        var path = (string.Format("{0}\\_шаблоны\\" + "Ведомость сан.гиг обучение.dotx", AppDomain.CurrentDomain.BaseDirectory));
                        Microsoft.Office.Interop.Word.Application wordApp = new Microsoft.Office.Interop.Word.Application { Visible = false };
                        Microsoft.Office.Interop.Word.Document aDoc = wordApp.Documents.Add(path);
                        aDoc.Activate();

                        object missing = Missing.Value;

                        Microsoft.Office.Interop.Word.Range range = aDoc.Content;
                        range.Find.ClearFormatting();
                        range.Find.Execute(FindText: "{Count}", ReplaceWith: persons.Count(), Replace: Microsoft.Office.Interop.Word.WdReplace.wdReplaceAll);
                        var table = aDoc.Tables[1];
                        int iRow = 2;

                        foreach (var p in persons)
                        {
                            if (iRow > 2) table.Rows.Add(ref missing);
                            table.Cell(iRow, 1).Range.Text = (iRow - 1).ToString() + '.';
                            table.Cell(iRow, 2).Range.Text = p.Fio;
                            table.Cell(iRow, 3).Range.Text = p.BirthDat?.ToString("dd.MM.yyyy");
                            table.Cell(iRow, 4).Range.Text = p.PaspAdres;
                            table.Cell(iRow, 5).Range.Text = "Проводник пассажирского вагона";

                            iRow++;
                        }
                        wordApp.Visible = true;
                        wordApp.Activate();
                    })
                    );
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        public async void Badges(List<PersonShortViewModel> persons)
        {
            try
            {    
                await new ProgressRunner().RunAsync(
                    new Action<ProgressHandler>((progressChanged) =>
                    {

                        progressChanged(1, "Формирование документа");
                        excelReport excel = new excelReport();
                        excel.Init("Бейджи.xltx", $"Бейджи.xlsx");//,visible:true);//, otchetDir: otchetDir);

                        int ri = 0;  

                        progressChanged(5);
                        var share = 95.0 / persons.Count*2; 
                        foreach (var p in persons)
                        {
                            ri++;
                            excel.cell[ri, 1].value2 = p.BadgeRus;
                            excel.cell[ri, 2].value2 = p.BadgeEng;
                            progressChanged(share);
                        }
                        if (ri > 0)
                        {
                            excel.setAllBorders(excel.get_Range("A1", "B" + ri));
                            excel.mySheet.Columns.AutoFit();
                        }
                        excel.mySheet = (Microsoft.Office.Interop.Excel._Worksheet)(excel.mySheets.get_Item(2)); excel.mySheet.Select();
                        ri = 0;
                        foreach (var p in persons)
                        {
                            ri++;
                            excel.cell[ri, 1].value2 = p.BadgeRus?.Trim().Split(' ').LastOrDefault();
                            excel.cell[ri, 2].value2 = p.BadgeEng?.Trim().Split(' ').LastOrDefault();
                            progressChanged(share);
                        }
                        if (ri > 0)
                        {
                            excel.setAllBorders(excel.get_Range("A1", "B" + ri));
                            excel.mySheet.Columns.AutoFit();
                        }
                        excel.mySheet = (Microsoft.Office.Interop.Excel._Worksheet)(excel.mySheets.get_Item(1)); excel.mySheet.Select();
                        excel.myExcel.Visible = true;                        
                    })
                    );
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }
    }
}