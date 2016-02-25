using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TicketDataModel;
using System.Web.Mvc;

namespace TicketDataModel
{
    public static class TraktatEntitiesExt
    {
        public static Translator GetUserProfile(this TraktatEntities @this, string userName)
        {
            if (userName.Contains(@"\"))
                userName = userName.Substring(userName.IndexOf(@"\") + 1);

            var currentUser = @this.Translators
                            .Include("TranslatorRoles")
                            .Include("TranslatorRoles.Role")
                            .Include("Office")
                            .Where(x => x.s_del == 0 && x.Login == userName)
                            .SingleOrDefault();
            return currentUser;
        }

        public static List<string> GetActiveStatuses(this TraktatEntities @this) 
        {
            return @this.Translators.Select(x => x.active).Distinct().OrderBy(x => x).ToList();
        }
        public static List<StaffStatus> GetStaffStatuses(this TraktatEntities @this)
        {
            return @this.StaffStatuses.OrderBy(x => x.ID).ToList();
        }
        
        public static IQueryable<Office> GetActiveSalesOffices(this TraktatEntities @this) 
        {
            return @this.GetActiveOffices().Where(x => x.IsSalesOffice);
        }
        public static IQueryable<Office> GetActiveOffices(this TraktatEntities @this)
        {
            return @this.Offices.Where(x => x.pod_rod == 1 && x.Del == 0 && x.IsInHouseOffice);
        }
    }
}
