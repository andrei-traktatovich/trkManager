using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLogic;
using TicketDataModel;

namespace BusinessLogic
{
    public class CashIncomeAndExpenseManager
    {
        public CashIncomeAndExpenseManager(TraktatEntities ctx)
        {
            _ctx = ctx;
        }

        private TraktatEntities _ctx;

        
         
        public int InsertOrUpdateIncome(IncomeAndExpenseDTO.IncomeItem item, int userID)
        {
            if (item == null)
                return -1;

            PaymentInfo paymentInfo;

            if (item.id == 0)
            {
                paymentInfo = new PaymentInfo();
                paymentInfo.PaymentDate = item.date;
                paymentInfo.CreatedDate = DateTime.Now;
                _ctx.PaymentInfos.Add(paymentInfo);
            }
            else
            {
                paymentInfo = _ctx.PaymentInfos.Find(item.id);
                if (paymentInfo == null)
                    return -1;
            }
                paymentInfo.OfficeID = item.officeID;
                paymentInfo.LegalEntityID = _ctx.Offices.Find(item.officeID).pod_ur.GetValueOrDefault();
                paymentInfo.PaymentAmount = item.amount;
                paymentInfo.AmountDue = item.amount;
                paymentInfo.CashReceiptNoteNumber = 0;
                paymentInfo.ContactPerson = item.text;
                paymentInfo.PaymentMethodID = 1;
                paymentInfo.PaymentStatusID = 1;

                Commit(userID);
                return paymentInfo.ID;
        }
    
        public int InsertOrUpdateExpense(IncomeAndExpenseDTO.ExpenseItem item, int userID)
        {
            if (item == null)
                return -1;

            PaymentInfo paymentInfo;

            if (item.id == 0)
            {
                paymentInfo = new PaymentInfo();
                paymentInfo.PaymentDate = item.date;
                paymentInfo.CreatedDate = DateTime.Now;
                _ctx.PaymentInfos.Add(paymentInfo);
            }
            else
            {
                paymentInfo = _ctx.PaymentInfos.Find(item.id);
                if (paymentInfo == null)
                    return -1;
            }

            paymentInfo.OfficeID = item.officeID;
            paymentInfo.LegalEntityID = _ctx.Offices.Find(item.officeID).pod_ur.GetValueOrDefault();
            paymentInfo.PaymentAmount = item.amount;
            paymentInfo.AmountDue = item.amount;
            paymentInfo.CashReceiptNoteNumber = 0;
            paymentInfo.Text = item.text;
            paymentInfo.PaymentStatusID = item.typeId;
            paymentInfo.PaymentMethodID = 1;
            Commit(userID);
            return paymentInfo.ID;
        }

        public void DeleteItem(int id)
        {
            
            var item = _ctx.PaymentInfos.Find(id);
            if (item != null)
                _ctx.Entry(item).State = System.Data.Entity.EntityState.Deleted;
            Commit();
        }

        private void Commit(int userID = 1)
        {
            try
            {
                foreach (var item in _ctx.ChangeTracker.Entries<PaymentInfo>())
                {
                    if (item.State == System.Data.Entity.EntityState.Added)
                    {
                        item.Entity.CreatedByID = userID;
                        item.Entity.CreatedDate = DateTime.Now;
                    }
                    if (item.State == System.Data.Entity.EntityState.Modified)
                    {
                        item.Entity.ModifiedByID = userID;
                        item.Entity.ChangedDate = DateTime.Now;
                    }
                }
                _ctx.SaveChanges();
            }
            catch (Exception ex)
            {
                var errorList = new List<string>();
                var errors = _ctx.GetValidationErrors();
                foreach (var error in errors)
                {
                    var item = (PaymentInfo)error.Entry.Entity;
                    var errorLine = string.Format("Сущность: {0} (сумма = {2})", item.ID, item.PaymentAmount);
                    
                    foreach (var ve in error.ValidationErrors)
                    {
                        errorLine = string.Join(Environment.NewLine, ve.ErrorMessage + " : " + ve.PropertyName);
                        errorList.Add(errorLine); 
                    }
                }
                var errorDump = string.Join(Environment.NewLine, errorList);
                throw new Exception(string.Format("Ошибка при сохранении изменений пользователя {0}. Подробности - почтой: " + errorDump, userID), ex);
            }
        }

        

        public class Context
        {
            public struct Month
            {
                public int id { get; set; }
                public string name { get; set; }
            }

            public List<IncomeAndExpenseViewModel.Office> offices { get; private set; }
            public List<ExpenseItemClassification.ExpenseGroup> expenseGroups { get; private set; }
            public int myOfficeId { get;  private set; }
            public string myOfficeName { get; private set; }
            public List<Month> months { get; private set; }
            public List<int> years { get; private set; }
            public int authorId { get; private set; }
            public string author { get; private set; }
            public bool canSelectOffice { get; private set; }
            public bool canSelectDate { get; set; }

            public static List<Month> GetMonths()
            {
                var monthNames = System.Globalization.DateTimeFormatInfo
                .CurrentInfo
                .MonthNames;
                
                var monthNo = 0;
                
                return monthNames
               .Select(x => new Month { id = ++monthNo, name = x }).ToList();
            }

            public static List<int> GetYears()
            {
                var years = new List<int>();
                var currentYear = DateTime.Today.Year;
                for (var year = currentYear; year > currentYear - 6; year--)
                    years.Add(year);
                return years;
            }

            public Context(TraktatEntities ctx, Translator user)
            {
                offices = ctx.GetActiveOffices()
                    //.Where(x => x.IsSalesOffice)
                    .Select(x => new IncomeAndExpenseViewModel.Office { id = x.ID, name = x.Name })
                    .OrderBy(x => x.name)
                    .ToList();

                expenseGroups = new ExpenseItemClassification(ctx).Create();

                myOfficeId = user.OfficeID.GetValueOrDefault();

                myOfficeName = user.Office.Name.ToUpper();

                months = GetMonths();

                years = GetYears();

                authorId = user.TranslatorID;
                
                author = user.Name;

                this.canSelectOffice = user.IsManagement();

                this.canSelectDate = user.IsBossAt(myOfficeId);
            }

        }

        public Context GetContext(Translator user)
        {
            return new Context(_ctx, user);
        }
        
        public List<ExpenseItem> GetExpenses(DateTime startDay, DateTime? endDate = null, int officeID = -1)
        {
            
            startDay = startDay.Date;
            
            var endDay = endDate ?? startDay.AddDays(1);

            endDate = endDay.Date;

            var result = _ctx.PaymentInfos.Where(x => x.PaymentDate.HasValue
                && (x.PaymentDate.Value >= startDay && x.PaymentDate.Value < endDay)
                && x.PaymentMethod.IsCashPayment
                && x.ItemType.IsExpenseItem
                && x.PaymentAmount.HasValue
                && (!x.Status.HasValue || x.Status != PaymentInfo.IsDeleted ))
                .Select(x => new ExpenseItem
                {
                    id = x.ID, 
                    amount = (float)x.PaymentAmount.Value, 
                    isDeletable = !x.CashReceiptNoteNumber.HasValue,
                    isDeleted = false,
                    isDirty = false,
                    isEditable = true,
                    isEditing = false,
                    isValid = true,
                    noteNo = x.CashReceiptNoteNumber ?? 0,
                    officeId = x.OfficeID ?? 0,
                    officeName = x.Office.Name,
                    text = x.Text ?? "",
                    //type = new ExpenseItemClassification.ExpenseType { id = x.ItemType.ID, name = x.ItemType.Name },
                    typeId = x.ItemType.ID,
                    author = (x.ModifiedBy == null ? x.CreatedBy.Name : x.ModifiedBy.Name),
                    authorID = x.ModifiedByID ?? x.CreatedByID,
                    payerName = ""
                     
                });

            if (officeID > -1)
                return result
                    .Where(x => x.officeId == officeID)
                    .ToList();
            else 
                return result.ToList();
        }


        // TODO: 
        // b) produce 3 kinds of page: for ordinary users (only today and save), for power users 
        // (for month and change month) and for admin (choice of offices).
        // remove search options from model and treat them separately. 

        private float GetBalanceAt(DateTime date, int officeID)
        {
            date = date.Date;

            double? totalExpenses = 0.0;
            double? totalIncomes = 0.0;

            var expenseItems = _ctx.PaymentInfos.Where(x => x.PaymentDate.HasValue && x.OfficeID == officeID
                
                && (x.PaymentDate.Value < date)
                && x.PaymentMethod.IsCashPayment
                && x.ItemType.IsExpenseItem
                && (!x.Status.HasValue || x.Status.Value != PaymentInfo.IsDeleted)
                && x.PaymentAmount.HasValue).Select(x => x.PaymentAmount.Value).ToList();
            if (expenseItems.Count > 0) 
                totalExpenses = expenseItems.Sum(x => x);

            var incomeItems = _ctx.PaymentInfos.Where(x => x.PaymentDate.HasValue && x.OfficeID == officeID
                
                && (x.PaymentDate.Value < date)
                && x.PaymentMethod.IsCashPayment
                && !x.ItemType.IsExpenseItem
                && x.PaymentAmount.HasValue).Select( x=> x.PaymentAmount.Value).ToList();
            if (incomeItems.Count > 0)
                totalIncomes = incomeItems.Sum(x => x);
            
            return (float)((totalIncomes ?? 0) - (totalExpenses ?? 0));

        }

        private List<IncomeItem> GetIncomes(DateTime date, int officeID)
        {

            date = date.Date;
            var nextDay = date.AddDays(1).Date;

            var result = _ctx.PaymentInfos.Where(x => x.PaymentDate.HasValue && x.OfficeID == officeID
                && (x.PaymentDate.Value >= date && x.PaymentDate.Value < nextDay)
                && x.PaymentMethod.IsCashPayment
                && !x.ItemType.IsExpenseItem
                && (!x.Status.HasValue || x.Status != PaymentInfo.IsDeleted)
                && x.PaymentAmount.HasValue)
                .Select(x => new IncomeItem
                {
                    id = x.ID,
                    amount = (float)x.PaymentAmount.Value,
                    isDeletable = !x.CashReceiptNoteNumber.HasValue,
                    isDeleted = false,
                    isDirty = false,
                    isEditable = !x.CashReceiptNoteNumber.HasValue 
                     || (x.CashReceiptNoteNumber.Value == 0), 
                     
                    noteNo = x.CashReceiptNoteNumber ?? 0,
                    officeId = x.OfficeID ?? 0,
                    officeName = x.Office.Name,
                    text = x.Text, 
                    payerName = x.ContactPerson,
                    //type = new { id = x.ItemType.ID, name = x.ItemType.Name },
                    author = (x.ModifiedBy == null ? x.CreatedBy.Name : x.ModifiedBy.Name)
                    
                })
                .ToList();

            return result;
        }

        public List<BalanceReportItem> GetBalanceReport(DateTime startDate, DateTime endDate)
        {
            startDate = startDate.Date;
            endDate = endDate.Date;

            return _ctx.GetBalanceReport(startDate, endDate).ToList();
        }

        public class BalanceReportViewModel
        {
            public List<DateTime> Dates = new List<DateTime>();
            public List<OfficeLine> OfficeLines = new List<OfficeLine>();
            public List<double> TotalExpenses { get; set;}
            private List<double> GetTotalExpenses()
            {
                   var list = new List<double>();
                    for (var i = 0; i < Dates.Count; i++)
                    {
                        double sum = 0;
                        foreach (var office in OfficeLines)
                        {
                            sum = sum + office.Expenses[i];
                        }
                        list.Add(sum);
                    }
                    return list;
                }

            public List<double> TotalIncomes { get; set;}
            private List<double> GetTotalIncomes()
            
                {
                    var list = new List<double>();
                    for (var i = 0; i < Dates.Count; i++)
                    {
                        double sum = 0;
                        foreach (var office in OfficeLines)
                        {
                            sum = sum + office.Incomes[i];
                        }
                        list.Add(sum);
                    }
                    return list;
                }
            

            public List<double> TotalBalances { get; set;}
            public List<double> GetTotalBalances()
                {
                    var list = new List<double>();
                    for (var i = 0; i < Dates.Count; i++)
                    {
                        double sum = 0;
                        foreach (var office in OfficeLines)
                        {
                            sum = sum + office.Balances[i];
                        }
                        list.Add(sum);
                    }
                    return list;
              
            }


            public List<double> TotalInternalCash { get; set; }

            private List<double> GetTotalInternalCash()
            {
                var list = new List<double>();
                for (var i = 0; i < Dates.Count; i++)
                {
                    double sum = 0;
                    foreach (var office in OfficeLines)
                    {
                        sum = sum + office.InternalCashCollections[i];
                    }
                    list.Add(sum);
                }
                return list;
            }


            public List<double> TotalCashCollections { get; set;}

            private List<double> GetTotalCash()
            
                {
                    var list = new List<double>();
                    for (var i = 0; i < Dates.Count; i++)
                    {
                        double sum = 0;
                        foreach (var office in OfficeLines)
                        {
                            sum = sum + office.CashCollections[i];
                        }
                        list.Add(sum);
                    }
                    return list;
                }
            

            public BalanceReportViewModel(DateTime startDate, DateTime endDate)
            {
                for (var date = startDate; date <= endDate; date = date.AddDays(1))
                {
                    Dates.Add(date);
                }
            }

            public void GetTotals()
            {
                TotalBalances = GetTotalBalances();
                TotalExpenses = GetTotalExpenses();
                TotalIncomes = GetTotalIncomes();
                TotalInternalCash = GetTotalInternalCash();
                TotalCashCollections = GetTotalCash();
            }

            public class OfficeLine
            {
                public List<double> Expenses = new List<double>();
                public List<double> Incomes = new List<double>();
                public List<double> Balances = new List<double>();
                public List<double> CashCollections = new List<double>();
                public List<double> InternalCashCollections = new List<double>();
                public double ExpensesPerOffice;
                public double IncomesPerOffice;
                 
                public double CashCollectionsPerOffice;
                public double InternalCashCollectionsPerOffice;

                public string Name {get; private set;}

                public OfficeLine(Office office, DateTime startDate, DateTime endDate, IEnumerable<BalanceReportItem> items)
                {
                    Name = office.Name;
                    startDate = startDate.Date;
                    endDate = endDate.Date;
                    
                    for (var date = startDate; date <= endDate; date = date.AddDays(1))
                    {
                        var item = items.Where(x => x.Date.HasValue && x.Date.Value.Date == date).SingleOrDefault();
                        var defaultBalance = items.Where(x => x.Date == null).First().Balance;
                        if (item != null)
                        {
                            Expenses.Add(item.Expense ?? 0);
                            Incomes.Add(item.Income ?? 0);
                            Balances.Add(item.Balance ?? 0);
                            CashCollections.Add(item.CashCollection ?? 0);
                            InternalCashCollections.Add(item.InternalCashCollection);
                        }
                        else
                        {
                            if (date > startDate && date< DateTime.Now)
                            {
                                Balances.Add(Balances.Last());//  + Incomes.Last() - Expenses.Last() - CashCollections.Last());
                                CashCollections.Add(0);
                                Expenses.Add(0);
                                Incomes.Add(0);
                                InternalCashCollections.Add(0);
                            }
                            else
                            {
                                Expenses.Add(0);
                                Incomes.Add(0);
                                Balances.Add(defaultBalance ?? 0);
                                CashCollections.Add(0);
                                InternalCashCollections.Add(0);
                            }
                        }

                    }

                    ExpensesPerOffice = Expenses.Sum(x => x);
                    IncomesPerOffice = Incomes.Sum(x => x);
                    CashCollectionsPerOffice = CashCollections.Sum(x => x);
                    InternalCashCollectionsPerOffice = InternalCashCollections.Sum(x => x);
                }
            }
        }
        public BalanceReportViewModel GetBalanceReportViewModel(DateTime startDate, DateTime endDate)
        {
            var result = new BalanceReportViewModel(startDate, endDate);
            var bl = GetBalanceReport(startDate, endDate);
            var offices = this._ctx.GetActiveSalesOffices()
                .OrderBy(x => x.Name).ToList();

            foreach (var office in offices)
            {
                var items = bl.Where(x => x.OfficeID == office.ID);
                result.OfficeLines.Add(new BalanceReportViewModel.OfficeLine(office, startDate, endDate, items));
            }

            result.GetTotals();
            return result;
        }

        public IncomeAndExpenseViewModel GetIncomesAndExpenses(Context context, int office, DateTime startDate, DateTime endDate)
        {
            startDate = startDate.Date;
            endDate = endDate.Date;
            
            if (endDate < startDate)
            {
                var temp = endDate;
                endDate = startDate;
                startDate = temp;
            }
            
            var today = DateTime.Today;

            if (endDate > today) 
            {
                endDate = today;
            }

            if (startDate > today)
            {
                startDate = new DateTime(today.Year, today.Month, 1);
            }

            var result = new IncomeAndExpenseViewModel();
            result.author = context.author;
            result.authorId = context.authorId;
            result.canSelectOffice = context.canSelectOffice;
            result.expensegroups = new ExpenseItemClassification(_ctx).Create();

            // brute force implementation

            // let just construct something 

            startDate = startDate.Date;
            endDate = endDate.Date;

            var balance = GetBalanceAt(startDate, office);
            
            result.openingBalance = balance;


            for (var date = startDate; date <= endDate; date = date.AddDays(1))
            {
                var day = new DayInfo() { date = date };

                day.balance = balance;

                day.expenses = GetExpenses(date, null, office);
                day.incomes = GetIncomes(date, office);

                balance = balance + day.incomes.Sum(x => x.amount) - day.expenses.Sum(x => x.amount);
                result.days.Add(day);
            }
            result.days = result.days.OrderByDescending(x => x.date).ToList();
            result.days.First().isFirstDay = true;
            result.closingBalance = balance;
            result.totalExpenses = result.days.Sum(x => x.expenses.Sum(y => y.amount));
            result.totalIncomes = result.days.Sum(x => x.incomes.Sum(y => y.amount));
            return result;
        }
    }
}
