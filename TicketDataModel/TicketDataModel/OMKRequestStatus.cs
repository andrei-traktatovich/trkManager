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
    
    public partial class OMKRequestStatus
    {
        public OMKRequestStatus()
        {
            this.Jobs = new HashSet<Job>();
            this.JobsUnderAnalysis = new HashSet<Job>();
        }
    
        public int ID { get; set; }
        public string Name { get; set; }
        public string sa_ch { get; set; }
    
        public virtual ICollection<Job> Jobs { get; set; }
        public virtual ICollection<Job> JobsUnderAnalysis { get; set; }
    }
}