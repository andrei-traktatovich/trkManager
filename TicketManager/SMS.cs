using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Twilio;

namespace TicketManager
{
    public class SMS
    {
        public void SendSMS(string Number, string Text)
            {
                string AccountSid = "AC9828d626d6d82e4f90d393723243d612";
                string AuthToken = "a646f76b0cc83cd4e33f9c1c366c23e8";
                var twilio = new TwilioRestClient(AccountSid, AuthToken);
                var message = twilio.SendSmsMessage("+18327862680", Number, Text, "");
            }

        
    }
}