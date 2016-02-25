using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Mail;
using System.Net.Mime;
using System.Configuration;
using System.Threading;
using System.Net;
 



namespace EMailNotification
{
    public class EmailInfo
    {
        public string RecipientAddress;
        public string Title;
        public string Body;
        public string ReplyTo;
        
    }
    

    public class Email
    {
        public string CurrentUserEmail = "";
        public string AuthorEmail = "";

        public void SendEmailNotificationOnTicketCreatedOrEdited(TicketDataModel.Ticket ticket, TicketDataModel.TraktatEntities ctx, string SubjectMessage = "", string Url=@"http://vs09/testapp")
        {
            // this will need to be refactored!!!
            var body = "<body style='font-family: segoe ui; color: blue; padding: 10px;'><table border='0'><tr><td>Автор</td><td>##Author</td></tr><tr><td>Отдел</td><td>##Office</td></tr><td>Тип</td><td>##Type</td></tr><tr><td>Срочность</td><td>##Priority</td></tr><tr><td>Статус</td><td>##Status</td></tr></table>";
            body = body + "<p>Заявка создана в ##CreatedDate, последнее изменение ##LastModified</p>";
            body = body + "<p>Email: <a href='mailto:##eMailAddress'>##eMailAddress</a><br/>";
            body = body +"<a href='"+Url+"'>Перейти к списку заявок</a><h2>##Keywords</h2><p>##Text</p>";
            body = body + "</body>";
            var _author = ctx.Translators.Where(x => x.TranslatorID == ticket.AuthorID).Single();
            var _mobilephones = string.Join(", ", _author.PersonalInfos.Select(x => x.MobilePhone).ToList());
            var _phones = string.Join(",",_author.PersonalInfos.Select(x => x.HomePhone).ToList());
            var _ownWorkPhones = string.Join(",",_author.PersonalInfos.Select(x=>x.Phone));
            string _workphones = "";
            try
            {

                _workphones = string.Join(", ", _author.Office.TranslatorReference.FirstOrDefault().PersonalInfos.Select(x => x.HomePhone));
            }
            catch
            {
            }
            var _allPhones = string.Format(" (<smaller>моб. {0}; дом. {1}; раб. {2}; {3}</smaller>)", _mobilephones, _phones, _workphones, _ownWorkPhones);

            var _authorData = _author.Name + _allPhones;
            body = body.Replace("##Author", _authorData);

            var _authorOffice = ctx.Offices.Where(x => x.ID == _author.OfficeID).Single();
            body = body.Replace("##Office", _authorOffice.Name);

            var _ticketType = ctx.TicketTypes.Find(ticket.TypeID).Name;
            body = body.Replace("##Type", _ticketType);

            body = body.Replace("##CreatedDate", ticket.CreatedDate.ToString());
            body = body.Replace("##LastModified", ticket.ChangedDate.HasValue ? ticket.ChangedDate.ToString() : ticket.CreatedDate.ToString());

            var _ticketLevel = ctx.TicketLevels.Find(ticket.LevelID).Name;
            body = body.Replace("##Priority", _ticketLevel);

            body = body.Replace("##Keywords", WebUtility.HtmlEncode(ticket.KeyWords));
            body = body.Replace("##Text", WebUtility.HtmlEncode(ticket.Text));

            var ticketStatus = ctx.TicketStatuses.Find(ticket.StatusID).Name;
            if (!string.IsNullOrEmpty(ticket.TicketStatusComment))
                ticketStatus = ticketStatus + " (комментарий: " + ticket.TicketStatusComment + ")";

            body = body.Replace("##Status", ticketStatus);

            var _authorEmails = string.Join(", ", _author.PersonalInfos.Select(x => x.Email).ToList());
            

            body = body.Replace("##eMailAddress", _authorEmails);
            var subject = SubjectMessage  + ticket.KeyWords + "(" + _ticketLevel + ", " + _ticketType + ")";
            
            var AssigneeOfficeTranslatorID = ctx.Translators.Where(x => x.pod_id == ticket.AssigneeOfficeID).Select(x => x.TranslatorID).Single();
            
            List<string> AssigneeEmail = ctx.PersonalInfos.Where(x => x.TranslatorID == AssigneeOfficeTranslatorID).Select(x => x.Email).ToList();

            
            if (ticket.Assignee != null)
            {
                var AssignedEmployeeEmails = ctx.PersonalInfos.Where(x => x.TranslatorID == ticket.AssigneeID).Select(x => x.Email);
                if (AssignedEmployeeEmails.Count() > 0)
                {
                    AssigneeEmail.AddRange(AssignedEmployeeEmails);
                }
            }
            
            if (AuthorEmail != "")
                AssigneeEmail.Add(AuthorEmail);

            if (CurrentUserEmail !="")
                AssigneeEmail.Add(CurrentUserEmail);
           
            var SanitizedRecipientEmails = AssigneeEmail.Where(x => x.Trim() != "").ToList();
            if (SanitizedRecipientEmails.Count > 0)
            {
                var RecipientEmails = string.Join(",", SanitizedRecipientEmails);

                var data = new EmailInfo() { RecipientAddress = RecipientEmails, Title = subject, Body = body, ReplyTo=_authorEmails };
                SendEmail(data);
            }
            else throw new Exception("Отсутствуют адреса для отправки сообщения по электронной почте");
        }
        
        public void SendEmail(EmailInfo Mail)
        {
            var SMTPUser = new System.Net.NetworkCredential()
            {
                UserName = "TraktatMailer@traktat.com",
                Password = "SendLetterSome1",
                Domain = "traktat"
            };

            var s = new SmtpClient()
            {
                Host = "exc.traktat.com",
                Port = 587,
                UseDefaultCredentials = false,
                Credentials = SMTPUser,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                EnableSsl = true
            };

            var m = new MailMessage();
            
            try
            {
                m.To.Add(Mail.RecipientAddress);
                if (!string.IsNullOrEmpty(Mail.ReplyTo))
                    m.ReplyToList.Add(new MailAddress(Mail.ReplyTo));

            }
            catch (Exception ex)
            {
                throw new Exception("Проверьте корректность следующего списка адресов электронной почты: " + Mail.RecipientAddress);
            }
            

            m.IsBodyHtml = true;
            m.Body = Mail.Body;
            m.Subject = Mail.Title;
            m.From = new MailAddress("TraktatMailer@traktat.com");
            
            m.Sender = m.From;
           

            try
            {
                System.Threading.ThreadPool.QueueUserWorkItem(delegate
                {
                    s.Send(m);
                }, null);
                
            }
            catch (Exception ex)
            {
                throw new Exception("Не удалось доставить сообщение. " + ex.Message);
            }
        }
    }
}
