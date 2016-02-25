using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TicketDataModel;
using DoddleReport;
using DoddleReport.Web;
using DoddleReport.Writers;
using DoddleReport.OpenXml;

namespace TicketManager.Controllers
{
    [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
    [NoCache]
    public class IncomeAndExpenseController : ADController
    {
        
         
        

        private BusinessLogic.CashIncomeAndExpenseManager manager;

        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        [NoCache]
        public ActionResult Index()
        {
            return View();            
        }
        
        public ActionResult GetContext()
        {
            manager = new BusinessLogic.CashIncomeAndExpenseManager(Context);
            var context = manager.GetContext(CurrentUser);
            var result = Json(context, JsonRequestBehavior.AllowGet);
            return Json(context, JsonRequestBehavior.AllowGet);
        }

        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        [NoCache]
        public ActionResult IncomesAndExpenses(int office, int month, int year)
        {
            Response.DisableUserCache();
            Response.DisableUserCache();
            
            
            manager = new BusinessLogic.CashIncomeAndExpenseManager(Context);

            var context = manager.GetContext(CurrentUser);

            BusinessLogic.IncomeAndExpenseViewModel result;

            //office = office ?? user.OfficeID;

            //month = month ?? DateTime.Today.Month;

            //year = year ?? DateTime.Today.Year;

            if (!context.canSelectDate)
            {
                result = manager.GetIncomesAndExpenses(context, office, DateTime.Today, DateTime.Today);
            }
            else
            {
                var startDate = new DateTime(year, month, 1);
                var endDate = new DateTime(year, month, DateTime.DaysInMonth(year, month));
                result = manager.GetIncomesAndExpenses(context, office, startDate, endDate);
            }

            var jsonResult = new JsonResult() { Data = result, MaxJsonLength = 86753090, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            return jsonResult;
        }

        public DoddleReport.Web.ReportResult ExpensesExport(DateTime? from, DateTime? to)
        {
            var year = DateTime.Now.Year;
            var month = DateTime.Now.Month;
            var startDate = from ?? new DateTime(year, month, 1);

            var endDate = to ?? new DateTime(year, month, DateTime.DaysInMonth(year, month));
            
            endDate = endDate.AddDays(1);

            var result = Context.SP_ExpensesReport(startDate, endDate);
            var report = new Report(result.ToReportSource(), new DoddleReport.OpenXml.ExcelReportWriter());

            report.DataFields["TypeName"].HeaderText = "Вид";
            report.DataFields["OfficeName"].HeaderText = "ОФис";
            report.DataFields["SubTypeName"].HeaderText = "Подвид";
            report.DataFields["SubOfficeName"].HeaderText = "Подофис";
            report.DataFields["Month"].HeaderText = "Месяц";
            report.DataFields["Quarter"].HeaderText = "Квартал";
            report.DataFields["Description"].HeaderText = "Описание";
            report.DataFields["NonCash"].HeaderText = "Безнал";
            report.DataFields["Cash"].HeaderText = "Нал";
            report.DataFields["Amount"].HeaderText = "Сумма";
            report.DataFields["PaymentDate"].HeaderText = "Дата";
            report.DataFields["Source"].HeaderText = "Источник";

            return new ReportResult(report, new DoddleReport.OpenXml.ExcelReportWriter()) { FileName = "expenses.xlsx" };
        }

        //public ActionResult BalanceReport()
        //{
        //    var manager = new BusinessLogic.CashIncomeAndExpenseManager(_ctx);
        //    var today = DateTime.Today;
        //    var startDate = new DateTime(today.Year, today.Month, 1);
        //    var endDate = new DateTime(today.Year, today.Month, DateTime.DaysInMonth(today.Year, today.Month));
        //    var model = manager.GetBalanceReportViewModel(startDate, endDate);
        //    var months = BusinessLogic.CashIncomeAndExpenseManager.Context.GetMonths();
        //    ViewData["monthID"] = new SelectList(BusinessLogic.CashIncomeAndExpenseManager.Context.GetMonths(), dataValueField: "id", 
        //        dataTextField: "name");
        //    ViewData["year"] = new SelectList(BusinessLogic.CashIncomeAndExpenseManager.Context.GetYears());
        //    return View(model);
        //}
        
        public ActionResult BalanceReport(int? monthID, int? year, bool? xls)
        {
            var manager = new BusinessLogic.CashIncomeAndExpenseManager(Context);
            var thisYear = DateTime.Today.Year;
            var thisMonth = DateTime.Today.Month;
            var today = new DateTime(year ?? thisYear, monthID ?? thisMonth, 1);
            var startDate = new DateTime(today.Year, today.Month, 1);
            var endDate = new DateTime(today.Year, today.Month, DateTime.DaysInMonth(today.Year, today.Month));
            if (endDate > DateTime.Today)
                endDate = DateTime.Today.Date;

            if (xls == null || !xls.Value)
            {
                var months = new SelectList(BusinessLogic.CashIncomeAndExpenseManager.Context.GetMonths(), "id", "name");
                ViewData["monthID"] = new SelectList(BusinessLogic.CashIncomeAndExpenseManager.Context.GetMonths(),"id", "name", thisMonth);
                ViewData["year"] = new SelectList(BusinessLogic.CashIncomeAndExpenseManager.Context.GetYears());
                var model = manager.GetBalanceReportViewModel(startDate, endDate);
                return View(model);
            }
            else
                return ExpensesExport(startDate, endDate);
        }

        public ActionResult TodayBalanceReport()
        {
            var manager = new BusinessLogic.CashIncomeAndExpenseManager(Context);
            var startDate = DateTime.Today.AddDays(-1);
            var endDate = DateTime.Today;
            
            var model = manager.GetBalanceReportViewModel(startDate, endDate);
            return View(model);
        }

        [HttpPost]
        public ActionResult UpdateIncome(BusinessLogic.IncomeAndExpenseDTO.IncomeItem item)
        {
            var manager = new BusinessLogic.CashIncomeAndExpenseManager(Context);
            var id  = manager.InsertOrUpdateIncome(item, CurrentUser.TranslatorID);
            var result = new { success = true, id = id };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult UpdateExpense(BusinessLogic.IncomeAndExpenseDTO.ExpenseItem item)
        {
            var manager = new BusinessLogic.CashIncomeAndExpenseManager(Context);
            var id = manager.InsertOrUpdateExpense(item, CurrentUser.TranslatorID);
            var result = new { success = true, id = id };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public void Delete(int id)
        {
            var manager = new BusinessLogic.CashIncomeAndExpenseManager(Context);
            manager.DeleteItem(id);
        }


    

    }
}
