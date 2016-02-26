using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TicketDataModel;

namespace TicketManager.Controllers
{
    public class ApiController : ADController
    {
        [HttpGet]
        public ActionResult GetCalendarGlobals()
        {
            var allOffices = Context.GetActiveOffices().ToIdNamePairs().ToList();
            var allStaffStatuses = Context.StaffStatuses.Select(x => new { id = x.ID, name = x.Name }).ToList();
            /* 
            ViewData["isHR"] = CurrentUser.CanEditEmployees(); 
            ViewData["offices"] = GetOfficesDrDn(CurrentUser.OfficeID);
            ViewData["activestatuses"] = GetActiveStatusesDrDn();
            ViewData["popup-change-staff-statusId"] = GetStaffStatusesDrDn();
            ViewData["popup-change-titleid"] = GetTitlesDrDn();
            
            */
            return Json(new
            {
                offices = allOffices,
                staffStatuses = allStaffStatuses
            },
            JsonRequestBehavior.AllowGet);
        }

        public ActionResult FilterEmployees([System.Web.Http.FromUri] EmployeeFilterCriteria criteria)
        {
            Response.CacheControl = "no-cache";

            ViewData["offices"] = Context.GetActiveOffices()
                .OrderBy(x => x.Name)
                .ToArray();

            var list = this.GetStaffEmployees();

            list = list.Filter(criteria)
                        .OrderBy(x => x.Name);

            return PartialView(list.OrderBy(x => x.Name));
        }

        [HttpPost]

        public void FeedMonthToCalendar(int ID, int month, int year, Calendar.ClientDay[] Days)
        {
            // month in javascript format
            var calendar = new Calendar(Context, CurrentUser);
            calendar.TryUpdateCalendarFromClient(ID, month, year, Days);
        }

        public ActionResult GetCalendarData(int id = -1, int month = -1, int year = -1) // month in Javascript format!!!
        {
            Response.CacheControl = "no-cache";
            // if no translator is found by this id, return null JSON object
            var translator = Context.Translators.Find(id);

            if (CurrentUser == null || translator == null)
                return null;

            else
            {
                var hasEditingRights = CurrentUser.IsManagement() || CurrentUser.IsBossAt(translator.OfficeID.GetValueOrDefault()) || CurrentUser.IsHR() || CurrentUser.IsBossAt(translator.Podr_now.GetValueOrDefault());

                var calendar = new Calendar(Context, CurrentUser);
                var result = calendar.GetCalendar(translator, month + 1, year);

                var response = new { Name = translator.Name, ID = id, Weeks = result, UserName = CurrentUser.Name, UserID = CurrentUser.TranslatorID, CanEdit = hasEditingRights };
                //var json = Json(response, JsonRequestBehavior.AllowGet);
                return new JsonResult { Data = response, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
        }

        // will also need to create an overload accepting start and end dates for a Gantt chart
        public void UpdateCalendar(CalendarSelection Selection)
        {
            if (CurrentUser.IsManagement() || CurrentUser.IsBossAt(Selection.Office))
            {
                // gets a rectangle: startX, startY, endX, endY (counting from 1).
                // needs to be converted into several periods, which need to be inserted into database.

                var calendar = new Calendar(Context, CurrentUser);
                Selection.Month++; // month is in JavaScript format!!!
                for (var y = Selection.StartY; y <= Selection.EndY; y++)
                {
                    var startDate = calendar.GetDateFromWeekDayAndWeek(Selection.Month, Selection.Year, Selection.StartX, y);
                    var endDate = calendar.GetDateFromWeekDayAndWeek(Selection.Month, Selection.Year, Selection.EndX, y);

                    calendar.UpdateCalendar(Selection.Id, (TicketDataModel.StaffStatuses)Selection.Status, startDate, endDate, Selection.Office, Selection.StandBy, Selection.Confirmed, Selection.Comment);
                }
            }
            else
            {
                // do sotmething ... 
            }
        }



        [HttpPost]
        public void ConfirmStatus(int[] ids)
        {
            if (ids == null || ids.Length == 0)
                return;
            // do nothing
            var calendar = new Calendar(Context, CurrentUser);
            for (var i = 0; i < ids.Length; i++)
            {
                try
                {
                    calendar.ConfirmStatus(ids[i], DateTime.Now.Date);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("error in id = " + ids[i]);
                }
            }
        }

        [HttpPost]
        public void UpdateTempOffice(int id, int officeID)
        {
            var calendar = new Calendar(Context, CurrentUser);
            calendar.UpdateTempOffice(id, officeID, DateTime.Now.Date);
        }

        [HttpPost]
        public void UpdateTitle(int id, int titleID)
        {
            
            var employee = Context.Translators.Find(id);
            employee.TitleID = titleID;
            Context.SaveChanges();
        }

        [HttpPost]
        public void UpdateTempStatus(int id, int newStatus)
        {
            var calendar = new Calendar(Context, CurrentUser);
            calendar.UpdateTempStatus(id, newStatus, DateTime.Now.Date);
        }


        public ActionResult GetWorkSheetTable(int officeID, DateTime? startDate, DateTime? endDate)
        {
            var workSheetTable = new WorkSheetTable(officeID, Context, CurrentUser);
            if (startDate == null || endDate == null)
            {
                startDate = DateTime.Now.Date.AddDays(-DateTime.Today.Day + 1);
                endDate = startDate.Value.AddMonths(1).AddDays(-1);
            }
            workSheetTable.Build(startDate.Value, endDate.Value);
            return PartialView(workSheetTable);
        }

        public void DownloadWorkSheetTable(int officeID)
        {
            var workSheetTable = new WorkSheetTable(officeID, Context, CurrentUser);
            workSheetTable.Build(DateTime.Now, DateTime.Now.AddMonths(1));
            var excel = workSheetTable.ToExcel();

        }

        [HttpPost]
        public void UpdatePermOffice(int id, string officeID)
        {
            
            var translator = Context.Translators.Find(id);
            translator.Podr_now = int.Parse(officeID);
            Context.SaveChanges();
        }

        [HttpPost]
        public void UpdateActiveStatus(int id, string newStatus)
        {
            
            var translator = Context.Translators.Find(id);
            translator.active = newStatus;
            Context.SaveChanges();
        }
    }
}