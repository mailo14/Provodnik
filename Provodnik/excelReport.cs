using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Excel = Microsoft.Office.Interop.Excel;
using System.IO;

namespace Provodnik
{
    public class excelReport
    {
        public string path;
        public string  otchetDir;
        public Excel.Application myExcel = null;
        public Excel.Range cell = null;
        public Excel.Range myRange = null;
        Excel._Worksheet _mySheet = null;
        public Excel._Worksheet mySheet
        {
            get { return _mySheet; }
                set {
                    _mySheet = value;
                if (_mySheet!=null)
                cell = mySheet.Cells;
                }
            
        }
       public object myEmp = System.Reflection.Missing.Value;

        Excel.Workbooks myBooks = null;
        Excel._Workbook myBook = null;
        public Excel.Sheets mySheets = null;
        /// <param name="шаблон">например: "работыПоСекциям.xls"</param>
        /// <param name="filename">имя файла отчета. например: "Исполнители.xls"</param>
        public bool Init(string шаблон, string filename, bool visible = false, string otchetDir= @"C:\_provodnikFTP")
        {
            try
            {
               this.otchetDir = otchetDir;
                   path = filename;
                //string path = string.Format(@"{2} {1} {0} - Исполнители.xls", r.remType, r.poezd, r.endDate.Value.ToString("yyyy.MM.dd"));
                try
                {
                    myExcel = (Excel.Application)System.Runtime.InteropServices.Marshal.GetActiveObject("Excel.Application");
                    foreach (Excel._Workbook b in (myBooks = (Excel.Workbooks)myExcel.Workbooks))
                        if (b.Name == path)
                        { b.Close(false); break; }
                }
                catch { }
                if (myExcel == null || myExcel != null && myBooks.Count > 0)
                    myExcel = new Excel.Application();
                myExcel.Visible = false;

                myBooks = (Excel.Workbooks)myExcel.Workbooks;
                myExcel.Visible = visible; //myExcel.Visible = true;

                myExcel.DisplayAlerts = false;
               // var pp = Environment.CurrentDirectory;
               // var pp2 = System.Reflection.Assembly.GetExecutingAssembly().Location;

                myBook = (Excel._Workbook)(myBooks.Open(string.Format("{0}\\_шаблоны\\" + шаблон, AppDomain.CurrentDomain.BaseDirectory)));
                mySheets = (Excel.Sheets)myBook.Worksheets;
                mySheet = (Excel._Worksheet)(mySheets.get_Item(1)); mySheet.Select(); 
                cell = mySheet.Cells;
                return true;
            }
            catch (Exception e)
            {
                System.Windows.MessageBox.Show("При создании отчета возникла ошибка:\n" + e.Message, "Ошибка Excel", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                return false;
            }
        }

        public void setAllBorders(Excel.Range range)
        {
            var bDiap = range.Borders;
            bDiap[Excel.XlBordersIndex.xlEdgeBottom].Weight = Excel.XlBorderWeight.xlThin;
            bDiap[Excel.XlBordersIndex.xlEdgeLeft].Weight = Excel.XlBorderWeight.xlThin;
            bDiap[Excel.XlBordersIndex.xlEdgeRight].Weight = Excel.XlBorderWeight.xlThin;
            bDiap[Excel.XlBordersIndex.xlEdgeTop].Weight = Excel.XlBorderWeight.xlThin;
            bDiap[Excel.XlBordersIndex.xlInsideHorizontal].Weight = Excel.XlBorderWeight.xlThin;
            bDiap[Excel.XlBordersIndex.xlInsideVertical].Weight = Excel.XlBorderWeight.xlThin;
        }

        public Excel.Range get_Range(string cell1, string cell2 = null) {
            if (cell2 == null) return mySheet.get_Range(cell1, myEmp);
            else return mySheet.get_Range(cell1, cell2);
        }

        public void Finish()
        {
            // path =  + @"\" + path;//string.Format(@"\{2} {1} {0} - Исполнители.xls", r.remType, r.poezd, r.endDate.Value.ToString("yyyy.MM.dd"));
            // Directory.SetCurrentDirectory();
            var wrongChars= System.IO.Path.GetInvalidFileNameChars();
            var existWrongChars = (from w in wrongChars join p in path on w equals p select w);
            foreach (var w in existWrongChars) path = path.Replace(w, '_');

            foreach (Excel._Workbook b in myBooks)
                if (b.Name == path) { b.Close(false); break; }

            if (!Directory.Exists(otchetDir))
                try { Directory.CreateDirectory(otchetDir); }
                catch
                {
                   // System.Windows.MessageBox.Show("Выберите папку для хранения отчетов!\n", "Ошибка сохранения", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning); MainWindow.p.Dispatcher.Invoke((Action)(() => { (new Выбор_папки_отчетов()).ShowDialog(); }));

                }

            path = otchetDir + @"\" + path;
            
            try { if (File.Exists(path)) File.Delete(path);
                myBook.SaveAs(path); }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("Невозможно сохранить документ\n" + path + "\n" + ex.Message, "Ошибка", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }

            myExcel.DisplayAlerts = true;
         // myExcel.Visible = true;
            myBook.Close();//+убрать
            cell = null; mySheet = null; mySheets = null; myBooks = null; myBook = null; myExcel = null; myRange = null;
            GC.Collect();
        }


        public void AutoFitMergedCellRowHeight(Excel.Range rng)
        {
            double mergedCellRgWidth = 0;
            double rngWidth, possNewRowHeight;

            if (rng.MergeCells)
            {
                // здесь использована самописная функция перевода стиля R1C1 в A1                
                if (xlRCtoA1(rng.Row, rng.Column) == xlRCtoA1(rng.Range["A1"].Row, rng.Range["A1"].Column))
                {
                    rng = rng.MergeArea;
                    if (rng.Rows.Count != 1 && rng.WrapText == true)
                    {
                        (rng.Parent as Excel._Worksheet).Application.ScreenUpdating = false;
                        rngWidth = rng.Cells.Item[1, 1].ColumnWidth;
                        for (int i = 1; i <= rng.Columns.Count; ++i)
                        {
                            mergedCellRgWidth += rng.Cells.Item[1, i].ColumnWidth;
                        }//for
                        rng.MergeCells = false;
                        rng.Cells.Item[1, 1].ColumnWidth = mergedCellRgWidth;
                        rng.EntireRow.AutoFit();
                        possNewRowHeight = rng.RowHeight;
                        rng.Cells.Item[1, 1].ColumnWidth = rngWidth;
                        rng.MergeCells = true;
                        rng.RowHeight = possNewRowHeight;
                        (rng.Parent as Excel._Worksheet).Application.ScreenUpdating = true;
                    }//if
                }//if                
            }//if
        }//AutoFitMergedCellRowHeight        
        private string xlRCtoA1(int ARow, int ACol, bool RowAbsolute = false, bool ColAbsolute = false)
        {
            int A1 = 'A' - 1;  // номер "A" минус 1 (65 - 1 = 64)
            int AZ = 'Z' - A1; // кол-во букв в англ. алфавите (90 - 64 = 26)

            int t, m;
            string S;

            t = ACol / AZ; // целая часть
            m = (ACol % AZ); // остаток?
            if (m == 0)
                t--;
            if (t > 0)
                S = Convert.ToString((char)(A1 + t));
            else S = String.Empty;

            if (m == 0)
                t = AZ;
            else t = m;

            S = S + (char)(A1 + t);

            //весь адрес.
            if (ColAbsolute) S = '$' + S;
            if (RowAbsolute) S = S + '$';

            S = S + ARow.ToString();
            return S;
        }//xlRCtoA1

        public bool ReplaceText(string textFrom, string textTo)
        {
            object m = Type.Missing;
            return this.mySheet.Cells.Replace(textFrom, textTo, Excel.XlLookAt.xlPart, Excel.XlSearchOrder.xlByRows,true,m,m,m);//, MatchCase: false, SearchFormat: false, ReplaceFormat:  false);
        }

        public Tuple<int, int> FindText(string text)
        {
            Excel.Range range = mySheet.Cells.Find(text);
            if (range != null)
            {
                range.Value = "";
                return new Tuple<int, int>(range.Row, range.Column);
            }
            return new Tuple<int, int>(0, 0);
        }
    }
}
