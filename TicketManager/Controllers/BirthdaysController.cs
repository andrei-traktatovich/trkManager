using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TicketDataModel;
using Repository;
using BusinessLogic;

namespace TicketManager.Controllers
{
    public class BirthdaysController : Controller
    {
        //
        // GET: /Birthdays/
        TraktatEntities _ctx = new TraktatEntities();

        public ActionResult Index()
        {
            var repo = new EFRepository<Translator>(_ctx);

            var startDate = DateTime.Today.Date;
            var endDate = startDate.AddMonths(1);
             

            var candidates = repo.Query()
                .PermanentStaffEmployees()
                .Where(x => x.Dt_Rod.HasValue).ToList();

            var birthdays = candidates
                .Where(x => BirthdayMatches(x.Dt_Rod.Value, startDate, endDate))
                .OrderBy(x => x.Dt_Rod.Value)
                .ToList();

            return View(birthdays);
        }

        private bool BirthdayMatches(DateTime birthdate, DateTime startDate, DateTime endDate)
        {
            var thisYear = DateTime.Now.Year;
            var testDate = new DateTime(thisYear, birthdate.Month, birthdate.Day);
            return (testDate >= startDate && testDate <= endDate);
        }
    }
}
