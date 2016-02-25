using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BusinessLogic.TimeSheets;
using ExcelWrapper;
using TicketDataModel;
using Repository;
using System.Data;

namespace BusinessLogic.Tests.TimeSheets
{
    [TestClass]
    public class WorkDaySheetExcelExportShould
    {
        [TestMethod, Ignore ]
        public void TestMethod1()
        {
            var wrapper = new ExcelWrapper.ExcelFileWrapper(@"c:\test\template.xls");
            var excelExport = new WorkDaySheetExcelExport(wrapper);
            var ctx = new TraktatEntities();
            var cs = ctx.Database.Connection;
            var repo = new Repository.EFRepository<Office>(ctx);

            //var builder = new WorkDaySheetBuilder(repo);
            //int[] ids = { 16, 34, 42 };
            //var sheets = builder.Build(ids, DateTime.Now.FirstMonthDay(), DateTime.Now.LastMonthDay());

            //excelExport.Build(sheets);

        }
    }
}
