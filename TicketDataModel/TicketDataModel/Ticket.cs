//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace TicketDataModel
{
    using System;
    using System.Collections.Generic;
    
    public partial class Ticket
    {
        public Ticket()
        {
            this.TicketAttachments = new HashSet<TicketAttachment>();
            this.TicketComments = new HashSet<TicketComment>();
        }
    
        public int ID { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public int AuthorID { get; set; }
        public string Text { get; set; }
        public Nullable<int> AssigneeID { get; set; }
        public Nullable<System.DateTime> ChangedDate { get; set; }
        public Nullable<int> ChangedByID { get; set; }
        public int LevelID { get; set; }
        public int StatusID { get; set; }
        public int TypeID { get; set; }
        public string KeyWords { get; set; }
        public int AssigneeOfficeID { get; set; }
        public string TicketStatusComment { get; set; }
        public Nullable<System.DateTime> ClosedDate { get; set; }
        public Nullable<System.DateTime> AssignedDate { get; set; }
        public Nullable<System.DateTime> TakenUpDate { get; set; }
        public Nullable<int> AssignedBy { get; set; }
        public Nullable<int> TakenUpBy { get; set; }
    
        public virtual Translator Author { get; set; }
        public virtual Translator Assignee { get; set; }
        public virtual Translator ChangedBy { get; set; }
        public virtual TicketLevel TicketLevel { get; set; }
        public virtual TicketStatus TicketStatus { get; set; }
        public virtual TicketType TicketType { get; set; }
        public virtual Office AssigneeOffice { get; set; }
        public virtual ICollection<TicketAttachment> TicketAttachments { get; set; }
        public virtual ICollection<TicketComment> TicketComments { get; set; }
        public virtual Translator Assignor { get; set; }
        public virtual Translator TakerUp { get; set; }
        public virtual Ticket tickets1 { get; set; }
        public virtual Ticket ticket1 { get; set; }
    }
}
