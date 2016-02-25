using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TicketDataModel;
using EMailNotification;

namespace TicketManager.Controllers
{
    public abstract class ADController : Controller
    {
        protected TraktatEntities Context;

        protected Translator CurrentUser = null;

        protected bool Authenticated { get 
            { 
                return CurrentUser != null;  
            } 
        }

        protected string UserName;

        public ADController()
        {
            Context = new TraktatEntities();
            UserName = System.Web.HttpContext.Current.User.Identity.Name;

            if (!String.IsNullOrEmpty(UserName))
                CurrentUser = Context.GetUserProfile(UserName);
        }

        protected List<SelectListItem> GetOfficesDrDn(int? selectedOfficeID = -1)
        {
            var offices = Context
                            .GetActiveOffices()
                            .OrderBy(x => x.Name);

            List<System.Web.Mvc.SelectListItem> officesSelectList;

            if (selectedOfficeID.HasValue)
            {
                officesSelectList = new System.Web.Mvc.SelectList(offices, "ID", "Name", selectedOfficeID).ToList();
            }
            else
                officesSelectList = new SelectList(offices, "ID", "Name").ToList();

            officesSelectList.Insert(0, new SelectListItem { Text = "Все", Value = "-1", Selected = false });
            return officesSelectList;
        }

        protected SelectList GetTitlesDrDn()
        {
            return new SelectList(Context.Titles.Select(x => new { x.ID, x.Name }).OrderBy(x => x.Name), "ID", "Name");
        }
        
        protected SelectList GetStaffStatusesDrDn()
        {
            var staffStatuses = Context.GetStaffStatuses();
            return new SelectList(staffStatuses.Select(x => new { ID = (int)(x.ID), x.Name }).OrderBy(x => x.ID), "ID", "Name");
        }

        protected SelectList GetActiveStatusesDrDn()
        {
            var activeStatuses = Context.GetActiveStatuses();
            return new SelectList(activeStatuses);
        }

        protected IQueryable<Translator> GetStaffEmployees()
        {
            return Context.Translators.GetActiveEmployees();
        }
        
    }
}