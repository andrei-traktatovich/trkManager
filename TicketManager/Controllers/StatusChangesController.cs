using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TicketDataModel;
using BusinessLogic;

namespace TicketManager.Controllers
{
    public class StatusChangesController : ADController
    {
        //
        // GET: /StatusChanges/
        private TraktatEntities ctx = new TraktatEntities();

        public ActionResult Index()
        {
            var allLanguageDirections = ctx.Languages
                .OrderBy(x => x.grp.Value)
                .Select(x => x.Name).ToList();

            //ViewData["languageName"] = Office.GetOfficesDrDnList(ctx);


            ViewData["DrDnOffices"] = GetOfficesDrDn();

            return View();
        }

        [HttpGet]
        public JsonResult GetStatusAndBusyReport(string officeID, string startDate, string endDate, string jobName)
        {
            int oID;
            if(!int.TryParse(officeID, out oID))
                oID = -1;
            DateTime start, end;
            if (!DateTime.TryParse(startDate, out start))
                start = DateTime.MinValue;
            if (!DateTime.TryParse(endDate, out end))
                end = DateTime.Now;
            var result = new BusinessLogic.StatusAndBusyReport(ctx).Create(oID, jobName, start, end);
            var formattedResult = result.Select(x => new
            {
                x.CalleeName,
                x.Caller,
                x.JobName,
                x.StatusComment,
                x.ContactResult,
                x.Status,
                StatusEndDate = x.StatusEndDate.HasValue ? x.StatusEndDate.Value.ToString("dd.MM.yy HH:mm") : "",
                x.Office,
                ContactDate = x.ContactDate.ToString("dd.MM.yy HH:mm")
            }).ToList();
            var jsonResult = Json(new { items = formattedResult }, JsonRequestBehavior.AllowGet);
            return jsonResult;

        }
        

    }
}