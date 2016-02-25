using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace TicketDataModel
{
    public partial class PersonalInfo
    {
        public static string CleanUpPhoneNumber(string phoneNumber, bool removePlus = true)
        {
            var result = phoneNumber;
            if (removePlus)
                result = result.Replace("+", "");
            result = Regex.Replace(result, @"[^\d]", "");
            return result;
        }
    }
}
