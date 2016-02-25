﻿using System.Web;
using System.Web.Mvc;

namespace TicketManager
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new TraktatHandleErrorAttribute());
        }
    }
}