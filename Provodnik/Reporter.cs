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

                        var labels = new string[] {//"№",
                "Номер договора", "Дата договора", "ФИО", "Пол", "Телефон", "Отряд", "Логин ВК", "Гражданство"
                , "Новичок", "Учебное заведение", "Форма обучения", "Год окончания обучения" ,"Есть льгота"
                , "ФИО родителя", "Контактный телефон родителей", "Есть форма"  , "Размер формы"
                ,       "Дата рождения", "Место рождения", "Серия паспорта", "Номер паспорта", "Кем выдан", "Когда выдан"
                , "Прописка по паспорту", "Адрес фактического места жительства", "Дата врем.регистрации", "Номер ПФ (снилс)", "ИНН"
                , "Учебный центр", "Номер учебной группы", "Дата окончания обучения"
                , "Дата выхода", "Желаемый город работы" , "Выбыл", "Причина", "Все сканы", "Примечания", "Заметки" };
                        int c = 0;
                        foreach (var l in labels)
                        {
                            c++; excel.cell[1, c].value2 = l;
                        }
                        progressChanged(5);
                        var share = 95.0 / rr.Count;
                        var errors = new List<string>();
                        foreach (var r in rr)//vm.Persons)
                        {
                            ri++;
                            c = 0;
                            //c++; excel.cell[ri, c].value2 = ri - 1;
                            c++; excel.cell[ri, c].value2 = r.Dogovor;
                            c++; excel.cell[ri, c].value2 = r.DogovorDat?.ToString("dd.MM.yyyy");
                            c++; excel.cell[ri, c].value2 = r.Fio;
                            c++; excel.cell[ri, c].value2 = r.Pol;
                            c++; excel.cell[ri, c].value2 = Helper.FormatPhone(r.Phone);
                            c++; excel.cell[ri, c].value2 = r.Otryad;
                            c++; excel.cell[ri, c].value2 = r.Vk;
                            c++; excel.cell[ri, c].value2 = r.Grazdanstvo;

                            c++; excel.cell[ri, c].value2 = r.IsNovichok ? "да" : "нет";
                            c++; excel.cell[ri, c].value2 = r.UchZavedenie;
                            c++; excel.cell[ri, c].value2 = r.UchForma;
                            c++; excel.cell[ri, c].value2 = r.UchGod;
                            c++; excel.cell[ri, c].value2 = /*r.HasLgota*/r.UchForma== UchFormaConsts.Ochnaya ? "да" : "нет";
                            

                            c++; excel.cell[ri, c].value2 = r.RodFio;
                            c++; excel.cell[ri, c].value2 = Helper.FormatPhone(r.RodPhone);
                            c++; excel.cell[ri, c].value2 = r.HasForma ? "да" : "нет";
                            c++; excel.cell[ri, c].value2 = r.RazmerFormi;

                            c++; excel.cell[ri, c].value2 = r.BirthDat?.ToString("dd.MM.yyyy");
                            c++; excel.cell[ri, c].value2 = r.MestoRozd;
                            c++; excel.cell[ri, c].value2 = r.PaspSeriya;
                            c++; excel.cell[ri, c].value2 = r.PaspNomer;
                            c++; excel.cell[ri, c].value2 = r.PaspVidan;
                            c++; excel.cell[ri, c].value2 = r.VidanDat?.ToString("dd.MM.yyyy");

                            c++; excel.cell[ri, c].value2 = r.PaspAdres;
                            c++; excel.cell[ri, c].value2 = r.FactAdres;
                            c++; excel.cell[ri, c].value2 = r.VremRegDat?.ToString("dd.MM.yyyy");
                            c++; excel.cell[ri, c].value2 = Helper.FormatSnils(r.Snils);
                            c++; excel.cell[ri, c].value2 = r.Inn?.ToString();

                            /* c++; excel.cell[ri, c].value2 = r.PsihDat;
                             c++; excel.cell[ri, c].value2 = r.IsPsih;
                             c++; excel.cell[ri, c].value2 = r.IsPsihZabral;
                             c++; excel.cell[ri, c].value2 = r.SanKnizkaDat;
                             c++; excel.cell[ri, c].value2 = r.IsSanKnizka;
                             c++; excel.cell[ri, c].value2 = r.MedKommDat;
                             c++; excel.cell[ri, c].value2 = r.IsMedKomm;
                             c++; excel.cell[ri, c].value2 = r.PraktikaDat;
                             c++; excel.cell[ri, c].value2 = r.IsPraktika;
                             c++; excel.cell[ri, c].value2 = r.ExamenDat;
                             c++; excel.cell[ri, c].value2 = r.IsExamen; */

                            c++; excel.cell[ri, c].value2 = r.UchebCentr;
                            c++; excel.cell[ri, c].value2 = r.UchebGruppa;               // c++; excel.cell[ri, c].value2 = r.UchebStartDat;
                            c++; excel.cell[ri, c].value2 = r.UchebEndDat?.ToString("dd.MM.yyyy");

                            c++; excel.cell[ri, c].value2 = r.VihodDat?.ToString("dd.MM.yyyy");
                            c++; excel.cell[ri, c].value2 = r.Gorod;
                            c++; excel.cell[ri, c].value2 = r.IsVibil ? "да" : "нет";
                            c++; excel.cell[ri, c].value2 = r.VibilPrichina;
                            c++; excel.cell[ri, c].value2 = r.AllScans ? "да" : "нет";

                            if (r.Messages == null) errors.Add(r.Fio);
                            c++; excel.cell[ri, c].value2 = r.Messages?.Replace(Environment.NewLine, "; ");

                            c++; excel.cell[ri, c].value2 = r.Zametki;
                            progressChanged(share);
                        }
                        excel.setAllBorders(excel.get_Range("A1", "AL" + ri));
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

                    })


                    );
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        public async void Badges(List<PersonShortViewModel> persons)
        {
            try
            {
                // await new ProgressRunner().RunAsync(doAction);      
                await new ProgressRunner().RunAsync(
                    new Action<ProgressHandler>((progressChanged) =>
                    {

                        progressChanged(1, "Формирование документа");

                        var path = (string.Format("{0}\\_шаблоны\\" + "Бейджи.dotx", AppDomain.CurrentDomain.BaseDirectory));
                        Microsoft.Office.Interop.Word.Application wordApp = new Microsoft.Office.Interop.Word.Application { Visible = false};
                        Microsoft.Office.Interop.Word.Document aDoc = wordApp.Documents.Add(path/*, ReadOnly: false, Visible: true*/);
                        aDoc.Activate();

                        object missing = Missing.Value;

                        var table = aDoc.Tables[1];
                        int iRow=2;

                        progressChanged(5);
                        var share = 95.0 / persons.Count;
                        //for (int i=0;i<persons.Count-1;i++) table.Rows.Add(table.Rows[i+2]); //table.Rows.Add(ref missing);
                        foreach (var p in persons)
                        {
                            if (iRow > 2) table.Rows.Add(ref missing);             
                            table.Cell(iRow, 1).Range.Text = (iRow-1).ToString() + '.';
                            table.Cell(iRow, 2).Range.Text = p.BadgeRus;
                            table.Cell(iRow, 3).Range.Text = p.BadgeEng;

                            iRow++;
                            progressChanged(share);
                        }
                        // string otchetDir = @"C:\_provodnikFTP";
                        //  aDoc.SaveAs(FileName: otchetDir + @"\Согласие на обработку персональных данных.DOCX");
                        wordApp.Visible = true;

                    })


                    );
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }
    }
}