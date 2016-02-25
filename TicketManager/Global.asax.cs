using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using EMailNotification;

namespace TicketManager
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            
            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        void Application_Error(object sender, EventArgs e)
        {
            var error = Server.GetLastError();
            var code = (error is HttpException) ? (error as HttpException).GetHttpCode() : 500;

            var innerException = error.InnerException == null ? "" : "Внутреннее исключение: " + error.InnerException.Message;

            string userName = "неизвестный пользователь";
            try
            {
                userName = HttpContext.Current.User.Identity.Name;
            }
            catch
            {
            }



            var body = DateTime.Now.ToString() + " (пользователь - " +  userName + ") " + error.Message + Environment.NewLine + error.StackTrace + Environment.NewLine + Environment.NewLine + innerException;
 
            if (code != 404)
            {
                var email = new Email();
                
                var recipient = "nikolaev@traktat.com";
                var title = "exception report";
                email.SendEmail(new EmailInfo { Body = body, Title = title, RecipientAddress = recipient });
            }

            Response.Clear();
            Server.ClearError();
            var message = string.Format("<h1>Диагностическое сообщение</h1><h2>Произошла ошибка {0}</h2><p>{1}</p><h1>Сообщение об ошибке передано разработчику. Попытайтесь повторить операцию</h1>", code.ToString(), body);
            Response.Write(new HtmlString(message));
            //string path = Request.Path;

            //Context.RewritePath("~/Shared/Error.cshtml", false);
            //IHttpHandler httpHandler = new MvcHttpHandler();
            //httpHandler.ProcessRequest(Context);
            //Context.RewritePath(path, false);
        }

    }
}