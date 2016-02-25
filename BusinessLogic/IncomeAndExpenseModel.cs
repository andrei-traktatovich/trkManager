using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLogic;

namespace BusinessLogic
{
    public class IncomeAndExpenseViewModel
    {
        public class Office
        {
            public int id { get; set; }
            public string name { get; set; }
        }

        public IncomeAndExpenseViewModel()
        {
            days = new List<DayInfo>();
        }

        public float openingBalance { get; set; }
        public float closingBalance { get; set; }
        public float totalExpenses { get; set; }
        public float totalIncomes { get; set; }
        public bool isItemEditing { get; set; }
        public string author { get; set; }
        public int authorId { get; set; }
        public bool canSelectOffice { get; set; }
        public List<ExpenseItemClassification.ExpenseGroup> expensegroups { get; set; }
        public List<DayInfo> days { get; set; }
        public bool isDirty { get; set; }
    }
    public class UserInfo
    {
        public string name { get; set; }
        public int id { get; set; }
    }

    public class DayInfo
    {
        public DateTime date { get; set; }
        public float balance { get; set; }
        public List<ExpenseItem> expenses { get; set; }
        public List<IncomeItem> incomes { get; set; }
        public bool isFirstDay { get; set; }
        public DayInfo()
        {
            incomes = new List<IncomeItem>();
            expenses = new List<ExpenseItem>();

        }
    }

    public class IncomeInfo
    {
        public float total { get { return items.Sum(x => x.amount); } }
        public List<IncomeItem> items { get; set; }
        public IncomeInfo()
        {
            items = new List<IncomeItem>();
        }
    }

    public class ExpenseInfo
    {
        public float total { get { return items.Sum(x => x.amount); } }
        public List<ExpenseItem> items { get; set; }
        public ExpenseInfo()
        {
            items = new List<ExpenseItem>();
        }
    }


    public class IncomeItem
    {
        public int id { get; set; }
        public bool isEditable { get; set; }
        public bool isDeleted { get; set; }
        public bool isDirty { get; set; }
        public bool isDeletable { get; set; }
        public string officeName { get; set; }
        public int officeId { get; set; }
        public int noteNo { get; set; }
        public string payerName { get; set; }
        public string text { get; set; }
        public float amount { get; set; }
        public string author { get; set; }
        public IncomeItem()
        {
            isEditable = true;
            isDeletable = true;
        }
    }

    public class ExpenseItem
    {
        public int id { get; set; }
        public bool isEditable { get; set; }
        public bool isDeletable { get; set; }
        public bool isDeleted { get; set; }
        public bool isDirty { get; set; }
        public bool isEditing { get; set; }
        public bool isValid { get; set; }
        public int typeId { get; set; }
        //public ExpenseItemClassification.ExpenseType type { get; set; }
        public string officeName { get; set; }
        public int officeId { get; set; }
        public int noteNo { get; set; }

        public string text { get; set; }
        public float amount { get; set; }
        public string author { get; set; }
        public int authorID { get; set;}
        public string payerName { get; set;}                    
                    
        public ExpenseItem()
        {
            isEditable = true;
            isDeletable = true;
            isDeleted = false;
            isValid = true;
        }
    }
}
