using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BusinessLogic;
using TicketDataModel;

namespace TicketManager.Controllers
{
    public class CurrentJobsController : Controller
    {

        TraktatEntities ctx = new TraktatEntities();
        //
        // GET: /CurrentJobs/

        public ActionResult Index()
        {
            var newJobs = new BusinessLogic.EventTracer(ctx).GetRecentNewJobs(DateTime.Now.AddDays(-2));
            return View(newJobs);
        }

        //
        // GET: /GetAll/
        public JsonResult GetAll()
        {
            var startDate = DateTime.Today.Date;
            var endDate = startDate.AddDays(7);
            var result = new CurrentJobsSchedule(ctx).GetJobScheduleItems(startDate, endDate);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

    }
}
