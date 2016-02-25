using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TicketDataModel;
using BusinessLogic;

namespace BusinessLogic.Tests
{
    [TestClass]
    public class expensegroupclassificationshould 
    {
        private TraktatEntities _ctx = new TraktatEntities();

        [TestMethod]
        public void do_something()
        {
            var classification = new BusinessLogic.ExpenseItemClassification(_ctx);
            var g = classification.Create();
        }

        [TestMethod]
        public void do_something1()
        {
            //var model = new CashIncomeAndExpenseManager(_ctx, 34,"fff",638,"aaa",true);
            //var context = model.GetContext();
            //var result = model.GetIncomesAndExpenses(638, 16);
        }
    }
}
