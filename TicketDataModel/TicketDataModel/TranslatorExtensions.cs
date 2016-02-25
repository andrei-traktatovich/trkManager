using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicketDataModel
{
    public partial class Translator
    {

        public static string GetTendency(Translator translator, DateTime startDate)
        {
            double result;
            var thisMonthDate = startDate.Date;
            var ctx = new TraktatEntities();
            var allGrades = ctx.JobGrades.Where(x => x.TranslatorID == translator.TranslatorID).ToList();
            var allAverage = allGrades.Average(x => x.Grade);
            var thisMonth = allGrades.Where(x => x.GradeDate >= thisMonthDate).Average(x => x.Grade);
            result = Math.Round((thisMonth ?? 0) - (allAverage ?? 0),2);
            if (result < 0)
                return "minus";
            if (result == 0)
                return "same";
            return "plus";
        }
        


        public static List<Translator> GetTranslatorList(TraktatEntities ctx, int officeID = -1)
        {
            var list = ctx.Translators
                     .Where(x => x.TitleStatusID == 1 && x.TitleID != 4
                         && (x.active == "активный" || x.active == "новый"))
                         .OrderBy(x => x.Name)
                         .ToList();
            if (officeID >= 0)
            {
                return list.Where(x => x.OfficeID == officeID).ToList();
            }
            else
                return list;
        }
        
        
        public enum Titles
        {
            Translator = 1,
            Layout = 2,
            Editor =3,
            Subdivision = 4,
            Manager = 5,
            SeniorManager = 6,
            OfficeHead = 7,
            ChiefEditor = 8,
            Accountant = 9,
            Management = 10,
            Administrator = 11,
            ProductionPlanning = 12,
            SeniorEditor = 13,
            ViceChiefEditor = 14,
            OfficeManager = 15,
            ManagerTranslator = 16,
            SenorManagerTranslator = 17,
            ViceOfficeHead = 18,
            FirstCategoryTranslator = 19,
            TranslatorEditor = 20,
            SeniorTranslator = 21,
            SystemAdministrator = 22,
            Apostilizer = 23,
            Legalizer = 24,
            EquipmentAndServiceSupplier = 25 /* need to add new titles !!! */ 
        }
        


        public bool IsBossAt(int translatorID, int officeID)
        {
            using (var ctx = new TraktatEntities())
            {
                var translator = ctx.Translators.Find(translatorID);
                if (translator == null)
                    throw new ArgumentOutOfRangeException("Недействительный ID исполнителя");
                else
                    return translator.IsBossAt(officeID);
            }
        }

        public bool IsBossAt(int officeID)
        {
            int[] managementRoles = { 10, 11, 12, 14};
            
            if (this.OfficeID == officeID && (this.TitleID == (int)Translator.Titles.OfficeHead 
                || this.TitleID == (int)Translator.Titles.ViceOfficeHead
                || this.TranslatorRoles.Select(x=>x.RoleID).Intersect(managementRoles).Count() > 0 
                && (this.OfficeID == officeID)))
                return true;
            else
                return false;
        }

        public bool IsManagement()
        {
            int[] managementRoles = { 14 };

            var roles = this.TranslatorRoles.Select(x => x.RoleID);
            var result = (roles.Intersect(managementRoles).Count() > 0);
            return result;
        }

        public bool IsHR()
        {
            return this.TranslatorRoles.Any(x => x.RoleID == 11);
        }

        
    }

    
}
