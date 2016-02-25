using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EMailNotification;

namespace TicketManager
{
    public static class Log
    {
        public static void Error(Exception ex, string route)
        {
            var email = new Email();
            var body = DateTime.Now + " in " + route + Environment.NewLine + ex.Message + Environment.NewLine + ex.StackTrace + Environment.NewLine + ex.InnerException.Message;

            email.SendEmail(new EmailInfo { RecipientAddress = "nikolaev@traktat.com", Title = "Exception in TicketManager", Body = body });


        }
    }
}