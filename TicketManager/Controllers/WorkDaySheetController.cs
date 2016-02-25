using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TicketDataModel;
using BusinessLogic;
using BusinessLogic.TimeSheets;
using Repository;
using ExcelWrapper;
using TicketManager.Models.WorkDaySheet;


namespace TicketManager.Controllers
{
    // TODO: remove file after download !!! 
    // or remove stale files somehow ... 
    public class WorkDaySheetController : ADController
    {

        public ActionResult Index()
        {
            List<Office> model;
            if (CurrentUser.IsHR() || CurrentUser.IsManagement())
            {
                model = Context.GetActiveOffices().ToList();
                ViewData["multipleOffices"] = true;
            }
            else if (CurrentUser.Office != null && CurrentUser.IsBossAt(CurrentUser.Office.ID))
            {
                model = new List<Office> { CurrentUser.Office };
                ViewData["multipleOffices"] = false;
            }
            else
                throw new InvalidOperationException("Пользователь " + CurrentUser.Name +
                    ": попытка неавторизаованного получения доступа к табелям");

            ViewData["startDate"] = DateTime.Now.FirstMonthDay().ToString("dd.MM.yyyy");
            ViewData["endDate"] = DateTime.Now.LastMonthDay().ToString("dd.MM.yyyy");

            return View(model);
        }

        public FileContentResult WorkSheetFile([System.Web.Http.FromUri] String ids, String startDate, String endDate, bool singleFile = false)
        {
            string templatePath = HttpContext.Server.MapPath(@"~/ExchangeFiles/ExcelTemplates/template.xlsx");

            
            var strs = ids.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);

            int[] officeIds = ids
                                .Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries)
                                .Select(x => int.Parse(x)).ToArray();

            List<WorkDaySheet> sheets;

            DateTime start = DateTime.Parse(startDate);
            DateTime end = DateTime.Parse(endDate);

            var officeRepo = new EFRepository<Office>(Context);
            var translatorRepo = new EFRepository<Translator>(Context);
            var builder = new WorkDaySheetBuilder(officeRepo, translatorRepo);

            if (singleFile)
            {
                var sheet = builder.BuildSheet(officeIds, start, end, "Сводная");
                sheets = new List<WorkDaySheet>() { sheet };
            }
            else
                sheets = builder.BuildSheets(officeIds, start, end);

            var excelWrapper = new ExcelFileWrapper(templatePath);
            
            var export = new WorkDaySheetExcelExport(excelWrapper);

            var result = export.Build(sheets);
            //Response.AppendHeader("content-disposition", "attachment; filename=" + "tabel.xlsx");
            
            //Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"; // "application/octet-stream";
            return new FileContentResult(result, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");

            //return new FileStreamResult(stream, "application/octet-stream");// ApplicationException/ new FileStreamResult(stream,  );//, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");

        }

        public ActionResult BreakupByOffice(int[] ids, DateTime startDate, DateTime endDate)
        {
            var officeRepo = new EFRepository<Office>(Context);
            var translatorRepo = new EFRepository<Translator>(Context);
            var builder = new WorkDaySheetBuilder(officeRepo, translatorRepo);
            var sheet = builder.BuildSheet(ids, startDate, endDate);
            var viewModel = new BreakupByOffice(sheet);
            return PartialView(viewModel);
        }

        public JsonResult WorkSheets(int[] ids, DateTime startDate, 
            DateTime endDate, bool singleFile = true)
        {
            var officeRepo = new EFRepository<Office>(Context);

            List<WorkDaySheet> sheets;

            var repo = new EFRepository<Office>(Context);
            var translatorRepo = new EFRepository<Translator>(Context);
            var builder = new WorkDaySheetBuilder(repo, translatorRepo);

            if (singleFile)
            {
                var sheet = builder.BuildSheet(ids, startDate, endDate);
                sheets = new List<WorkDaySheet>() { sheet };
            }
            else
                sheets = builder.BuildSheets(ids, startDate, endDate);
            
            var result = Json(sheets, JsonRequestBehavior.AllowGet);

            Console.WriteLine(result.ToString());
            
            return result;
        }
        
        //
        // GET: /WorkDaySheet/Details/5

        public ActionResult Details(int id)
        {
            return View();
        }

        //
        // GET: /WorkDaySheet/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /WorkDaySheet/Create

        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /WorkDaySheet/Edit/5

        public ActionResult Edit(int id)
        {
            return View();
        }

        //
        // POST: /WorkDaySheet/Edit/5

        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /WorkDaySheet/Delete/5

        public ActionResult Delete(int id)
        {
            return View();
        }

        //
        // POST: /WorkDaySheet/Delete/5

        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
