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
    
    public partial class AttractionChannel
    {
        public AttractionChannel()
        {
            this.Jobs = new HashSet<Job>();
            this.Customers = new HashSet<Customer>();
        }
    
        public int ID { get; set; }
        public string Name { get; set; }
    
        public virtual ICollection<Job> Jobs { get; set; }
        public virtual ICollection<Customer> Customers { get; set; }
    }
}
