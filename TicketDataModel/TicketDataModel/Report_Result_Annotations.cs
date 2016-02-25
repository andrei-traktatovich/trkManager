using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace TicketDataModel
{
    [MetadataType(typeof(ExpensesReport_Result_Annotations))]
    public partial class ExpensesReport_Result { }

    public partial class ExpensesReport_Result_Annotations
    {
        [Display(Name = "Вид")]
        public string TypeName { get; set; }

        [Display(Name = "Офис")]
        public string OfficeName { get; set; }

        [Display(Name = "Подвид")]
        public string SubTypeName { get; set; }

        [Display(Name = "Подофис")]
        public string SubOfficeName { get; set; }
        
        [Display(Name = "Месяц")]
        public Nullable<int> Month { get; set; }

        [Display(Name = "Квартал")]
        public Nullable<int> Quarter { get; set; }

        [Display(Name = "Описание")]
        public string Description { get; set; }

        [Display(Name = "Безнал")]
        public int NonCash { get; set; }

        [Display(Name = "Нал")]
        public Nullable<double> Cash { get; set; }

        [Display(Name = "Сумма")]
        public Nullable<double> Amount { get; set; }
        
        [Display(Name = "Дата")]
        public Nullable<System.DateTime> PaymentDate { get; set; }

        [Display(Name = "Источник")]
        public string Source { get; set; }
    }
}
