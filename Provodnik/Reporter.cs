﻿using System;
using System.Collections.Generic;
using System.Linq;
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
                            c++; excel.cell[ri, c].value2 = r.HasLgota ? "да" : "нет";

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
                            c++; excel.cell[ri, c].value2 = r.Messages.Replace(Environment.NewLine, "; ");
                            c++; excel.cell[ri, c].value2 = r.Zametki;
                            progressChanged(share);
                        }
                        excel.setAllBorders(excel.get_Range("A1", "AL" + ri));
                        excel.myExcel.Visible = true;
                        //  excel.Finish();
                    })


                    );
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }
    }
}