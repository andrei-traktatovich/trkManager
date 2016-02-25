using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLogic.TimeSheets;
using ExcelWrapper;
using System.Drawing;
using System.IO;

namespace BusinessLogic.TimeSheets
{
    public class WorkDaySheetExcelExport
    {
        private IExcelFileWrapper _excelWrapper;
        private readonly int firstDayCol = 5;
        private readonly int zeroRow = 15;

        public WorkDaySheetExcelExport(IExcelFileWrapper excelWrapper)
        {
            if (excelWrapper == null)
                throw new ArgumentNullException("excelWrapper");
            _excelWrapper = excelWrapper;
        }

        public byte[] Build(IEnumerable<WorkDaySheet> sheets)
        {
            foreach (var sheet in sheets)
            {
                _excelWrapper.AddSheetFromTemplate(sheet.Name);
                WriteSheet(sheet);
                _excelWrapper.SetColumnWidth(5);
            }
            return _excelWrapper.ToBytes();
        }

        private void WriteSheet(WorkDaySheet sheet)
        {
            _excelWrapper.SetRange("OfficeName", sheet.Name);
            _excelWrapper.SetRange("StartDate", sheet.StartDate);
            _excelWrapper.SetRange("EndDate", sheet.EndDate.AddDays(-1));
            _excelWrapper.SetRange("TodayDate", DateTime.Now.Date.ToString("dd.MM.yy"));

            int width = sheet.Duration;
            var height = sheet.Count;
            CreateContainer(width, height);
            MarkDays(sheet.StartDate, sheet.EndDate, width, height);

            var counter = 1;

            foreach (var line in sheet.TimeLines)
                WriteTimeLine(counter++, line, sheet.Duration);
               
            _excelWrapper.SetCell(1, zeroRow, @"№ п/п");
            _excelWrapper.SetCell(2, zeroRow, @"ФИО");
            _excelWrapper.SetCell(3, zeroRow, @"Должность");
            _excelWrapper.SetCell(4, zeroRow, @"Офис выдачи з.п.");

            //this is disabled because it causes a printer error calling printer setup.
            _excelWrapper.SetPrintableArea(width + firstDayCol, height + zeroRow); 

        }

        private void WriteTimeLine(int counter, WorkDaysTimeLine line, int duration)
        {
            var row = zeroRow + counter;
            _excelWrapper.SetCell(1, row, counter);
            _excelWrapper.SetCell(2, row, line.Name);
            _excelWrapper.SetCell(3, row, line.Description);
            _excelWrapper.SetCell(4, row, line.ReceiptOfficeName);
            WritePeriods(line.Periods, row, line.StartDate);
            _excelWrapper.SetCell(firstDayCol + duration, row, line.PaidDuration);
        }

        private void WritePeriods(IEnumerable<WorkDaySheetTimeLinePeriod> periods, int row, DateTime startDate)
        {
            foreach (var period in periods)
            {
                var offset = period.Start.DifferenceInFullDays(startDate);
                var length = period.Duration;

                var cellText = period.Text;
                if (period.IsPaid)
                    cellText += Environment.NewLine + "(" + period.OfficeName + ")";

                _excelWrapper.MergeCells(offset + firstDayCol, row, length);
                _excelWrapper.SetRange(offset + firstDayCol, row, offset + firstDayCol + length - 1, row, cellText);

                if (!period.IsPaid)
                {
                    _excelWrapper.SetBackGroundColor(offset + firstDayCol, row, offset + firstDayCol, row, System.Drawing.Color.Gainsboro);
                }
                else
                {
                    if (period.Text.StartsWith("В "))
                        _excelWrapper.SetBackGroundColor(offset + firstDayCol, row, offset + firstDayCol, row, System.Drawing.Color.Coral);
                    else
                        _excelWrapper.SetBackGroundColor(offset + firstDayCol, row, offset + firstDayCol, row, System.Drawing.Color.LightGreen);
                }

            }
        }

        private void CreateContainer(int width, int height)
        {
            _excelWrapper.InsertRowsAt(zeroRow, height);
            _excelWrapper.Select(1, zeroRow, firstDayCol + width, zeroRow + height);
            _excelWrapper.AutofitColumns();
            _excelWrapper.SetColumnWidth(5);
            _excelWrapper.SetBorders();
            _excelWrapper.AlignMiddle();
        }

        private void MarkDays(DateTime startDate, DateTime endDate, int width, int height)
        {
            var counter = firstDayCol;
            var row = zeroRow;
            for (var d = startDate.Date; d < endDate.Date; d = d.AddDays(1))
            {
                _excelWrapper.SetCell(counter, row, d.Date);
                if (d.IsHoliday())
                    _excelWrapper.SetBackGroundColor(counter, row, counter, row, Color.Coral);

                counter++;
            }
            _excelWrapper.SetCell(counter, row, "Всего отраб.дней");
            _excelWrapper.Select(firstDayCol, zeroRow, firstDayCol + width, zeroRow);
            _excelWrapper.AutofitColumns();
            _excelWrapper.SetDateFormat();
        }
    }
}
