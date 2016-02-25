using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TicketDataModel;

namespace BusinessLogic
{
    public class ExpenseItemClassification
    {
        public ExpenseItemClassification(TraktatEntities ctx)
        {
            _ctx = ctx;
        }

        private TraktatEntities _ctx;

        public List<ExpenseGroup> Create()
        {
            return _ctx.PaymentItemTypes.Where(x => x.IsExpenseItem && x.ParentItem == null)
                .Select(x => new ExpenseGroup
                {
                    name = x.Name,
                    items = x.ChildItems
                    .Select(y => new ExpenseType { id = y.ID, name = y.Name })
                    .Concat(new [] { new ExpenseType { id = x.ID, name = x.Name }})
                    .OrderBy(z => z.name)
                    .ToList()
                })
                .ToList();
        }

        public class ExpenseType
        {
            public int id { get; set; }
            public string name { get; set; }
        }

        public class ExpenseGroup
        {
            public string name { get; set; }
            public List<ExpenseType> items = new List<ExpenseType>();
        }
    }
}
