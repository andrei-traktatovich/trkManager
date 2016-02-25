using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TicketDataModel;
using EMailNotification;
using System.Linq.Expressions;
using System.Data.Entity;
using TicketManager.Models;

// TODO: remove any functionality related to tickets

namespace TicketManager.Controllers
{
    //[Authorize]
    
    public class HomeController : ADController
    {
        public ActionResult Index()
        {
            if (!Authenticated)
                return View("UserUnauthenticated"); // TODO 

            ViewData["isHR"] = CurrentUser.CanEditEmployees(); 
            ViewData["offices"] = GetOfficesDrDn(CurrentUser.OfficeID);
            ViewData["activestatuses"] = GetActiveStatusesDrDn();
            ViewData["popup-change-staff-statusId"] = GetStaffStatusesDrDn();
            ViewData["popup-change-titleid"] = GetTitlesDrDn();

            return View();
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


    }
}
