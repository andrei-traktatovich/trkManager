using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.Entity;
using TicketDataModel;

namespace PaulMorozov
{
    public partial class TicketList : System.Web.UI.Page
    {
        protected TraktatEntities ctx = new TraktatEntities();
        protected IQueryable<Ticket> SelectedTickets; 

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.IsPostBack)
            {
                SelectedTickets = ctx.Tickets;
                if (SelectedTickets.Count() > 0)
                {
                    var selectedTicketsList = SelectedTickets.ToList();
                    dgrList.DataSource = selectedTicketsList;
                    dgrList.DataBind();
                }
                
            }
        }

        protected void CreateNewTicket_Click(object sender, EventArgs e)
        {
            var newTicket = new Ticket()
            {
                CreatedDate = DateTime.Now,
                AuthorID = 638,
                Text = "Fuck you all",
                AssigneeID = 638,
                LevelID = 0,
                StatusID = 0,
                TypeID = 0
            };
            ctx.Tickets.Add(newTicket);
            ctx.SaveChanges();

        }
    }
}