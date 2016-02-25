using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.IO;

namespace ExcelWrapper
{
    public interface IExcelFileWrapper
    {
        void Close();
        
        void ClearSheets();
        int AddSheet(string name);
        int AddSheetFromTemplate(string name);
        void ActivateSheet(int number);
        
        void SetCell(int x, int y, object value);
        void SetRange(string rangeName, object value);
        void SetRange(int x, int y, int x1, int y1, object value);
        void SetDateFormat();

        void MergeCells(int x, int y, int x1, int y1);
        void MergeCells(int x, int y, int length);

        void InsertRowsAt(int rowNo, int howMany = 1);
        void InsertColumnsAt(int colNo);

        void SetBackGroundColor(int x, int y, int x1, int y1, Color backColor);
        void SetBold(int x, int y, int x1, int y1);

        object CellValue(int x, int y);

        void Select(int x, int y, int x1, int y1);
        void AutofitColumns();
        void SetBorders();
        

        void AlignMiddle();

        void SetPrintableArea(int width, int height);

        void SetColumnWidth(int width);

        Byte[] ToBytes();
    }
}
 