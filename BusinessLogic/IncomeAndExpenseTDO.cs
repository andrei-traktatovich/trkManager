using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic
{
    public class IncomeAndExpenseDTO
    {
        public List<int> deleteds { get; set; }
        public List<IncomeItem> incomes { get; set; }
        public List<ExpenseItem> expenses { get; set; }
        
        public class IncomeItem
        {
            public int id { get; set; }
            public int noteNo { get; set; }
            public float amount { get; set; }
            public string text { get; set; }
            public string payer { get; set; }
            public int officeID { get; set; }
            public DateTime date{ get; set; }
            
        }

        public class ExpenseItem
        {
            public int id { get; set; }
            public float amount { get; set; }
            public string text { get; set; }
            public int typeId { get; set; }
            public int officeID { get; set; }
            public DateTime date { get; set; }
        }
    }
}
