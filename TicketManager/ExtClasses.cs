using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TicketDataModel;

namespace TicketManager
{
    public static class ExtClasses
    {
        public static bool OverlapsWith(this JobParticipant x, JobParticipant y, bool ShowHours)
        {
            try
            {
                var startX = (DateTime)x.StartDate;
                var endX = (DateTime)x.EndDate;
                var startY = (DateTime)y.StartDate;
                var endY = (DateTime)y.EndDate;
                if (ShowHours)
                {
                    // do smth
                }
                else
                {
                    startX = startX.Date;
                    startY = startY.Date;
                    endX = endX.Date;
                    endY = endY.Date;
                }
                if (endX < startY || startX > endY) return false;
                else return true;
            }
            catch
            {
                return false;
            }
        }
    }
}