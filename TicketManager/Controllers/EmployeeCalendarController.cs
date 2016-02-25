using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TicketDataModel;

namespace TicketManager.Controllers
{
    public class EmployeeCalendarController : ADController
    {
        TraktatEntities ctx = new TraktatEntities();
        //
        // GET: /EmployeeCalendar/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetGlobals()
        {
            var offices = Context.GetActiveOffices().Select(x => new { id = x.ID, name = x.Name }).ToList();

            var data = new { offices = offices };

            return Json(data, JsonRequestBehavior.AllowGet);
        }
    }
}
