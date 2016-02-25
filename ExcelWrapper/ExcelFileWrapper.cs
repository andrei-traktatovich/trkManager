using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OfficeOpenXml;
using System.Drawing;
using System.IO;

namespace ExcelWrapper
{
    public class ExcelFileWrapper : IExcelFileWrapper
    {
        private ExcelPackage _excelPackage;
        private ExcelWorkbook _workBook;
        private ExcelWorksheet _templateSheet;
        
        public ExcelFileWrapper(string templatePath)
        {
            if (string.IsNullOrEmpty(templatePath))
                throw new ArgumentNullException("templatePath");
            
            _excelPackage = new ExcelPackage(new FileInfo(templatePath), true);

            _workBook = _excelPackage.Workbook;
            
            _templateSheet = _workBook.Worksheets["template"];

            if (_templateSheet == null)
                throw new InvalidOperationException("Файл '" + templatePath + "' не содержит листа 'template'");
        }

        public void ClearSheets()
        {
            var count = _workBook.Worksheets.Count;

            for(var i = 1; i <= count; i++)
            {
                var s = _workBook.Worksheets[i];
                if (s.Name != "template")
                    _workBook.Worksheets.Delete(s);
            }
        }

        // activates new sheet, gives it a name and returns the number of that sheet 
        public int AddSheet(string name)
        {
            var activeSheet = _workBook.Worksheets.Add(name);
           
            return activeSheet.Index;
        }

        public int AddSheetFromTemplate(string name)
        {
            var lastSheetIndex = _workBook.Worksheets.Count;
            var targetSheet = _workBook.Worksheets[lastSheetIndex];
            var source = _workBook.Worksheets["template"];
            _activeSheet = _workBook.Worksheets.Add(name, source);

            return _activeSheet.Index;
        }

        public void SetPrintableArea(int width, int height)
        {

            _activeSheet.PrinterSettings.Orientation = eOrientation.Landscape;
            _activeSheet.PrinterSettings.PaperSize = ePaperSize.A4;
            _activeSheet.PrinterSettings.PrintArea = _activeSheet.Cells[1, 1, height, width];

            _activeSheet.PrinterSettings.FitToWidth = 1;

            _activeSheet.PrinterSettings.TopMargin = 0.2M;
            _activeSheet.PrinterSettings.BottomMargin = 0.2M;
            _activeSheet.PrinterSettings.LeftMargin = 0.2M;
            _activeSheet.PrinterSettings.RightMargin = 0.2M;

        }

        ExcelWorksheet _activeSheet;
        public void ActivateSheet(int number)
        {
            _activeSheet = _workBook.Worksheets[number];
        }

        public void SetCell(int x, int y, object value)
        {
            _activeSheet.Cells[y, x].Value = value;
        }

        public void MergeCells(int x, int y, int x1, int y1)
        {
            _activeSheet.Cells[y, x, y1, x1].Merge = true;
        }

        public void MergeCells(int x, int y, int length)
        {
            var x1 = x + length - 1;
            MergeCells(x, y, x1, y);
        }

        public void InsertRowsAt(int rowNo, int howMany = 1)
        {
            _activeSheet.InsertRow(rowNo, howMany);       
        }

        public void InsertColumnsAt(int colNo)
        {
            _activeSheet.InsertColumn(colNo, 1);
        }

        public void SetBackGroundColor(int x, int y, int x1, int y1, Color backColor)
        {
            _activeSheet.Cells[y, x, y1, x1].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            _activeSheet.Cells[y, x, y1, x1].Style.Fill.BackgroundColor.SetColor(backColor);
        }

        public void SetBold(int x, int y, int x1, int y1)
        {
            _activeSheet.Cells[y, x, y1, x1].Style.Font.Bold = true;
        }

        public void Close()
        {
            //Save();
        }

        public object CellValue(int x, int y)
        {
            return _activeSheet.Cells[y, x].Value;
        }


         

        // not sure if that will work for me...     
        public void SetRange(string rangeName, object value)
        {
            var range = _workBook.Names[rangeName];
            range.Value = value;
            
        }

        public void SetRange(int x, int y, int x1, int y1, object value)
        {
            _activeSheet.Cells[y,x,y1,x1].Value = value;
        }

        public void Select(int x, int y, int x1, int y1)
        {
            _activeSheet.Select(new ExcelAddress(y, x, y1, x1));
        }

        public void AutofitColumns()
        {
            _activeSheet.SelectedRange.AutoFitColumns();
        }

        public void SetBorders()
        {
            var border = _activeSheet.SelectedRange.Style.Border;
            border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
            border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
            border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
            border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
        }


        public void SetDateFormat()
        {
            _activeSheet.SelectedRange.Style.Numberformat.Format = "DD.MM";
            
        }


        public void AlignMiddle()
        {
            var currentRange = _activeSheet.SelectedRange;

            currentRange.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
            currentRange.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
        }


        public void SetColumnWidth(int width)
        {
            var currentRange = _activeSheet.SelectedRange;//.EntireRow; // (Excel.Range)(_app.Selection);
            for (var i = currentRange.Start.Column; i <= currentRange.End.Column; i++)
                _activeSheet.Column(i).Width = 4;
                
            // currentRange ColumnWidth = 4;

            _activeSheet.Column(2).Width = 12;
            _activeSheet.Column(3).Width = 12;
        }



        public Byte[] ToBytes()
        {
            Byte[] bin = _excelPackage.GetAsByteArray();
            return bin;
        }
    }
}


 