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
    
    public partial class JobType
    {
        public JobType()
        {
            this.Jobs = new HashSet<Job>();
            this.JobParticipants = new HashSet<JobParticipant>();
        }
    
        public int ID { get; set; }
        public string Name { get; set; }
        public string v_price { get; set; }
        public string v_ch { get; set; }
        public string v_gr { get; set; }
        public int v_napr { get; set; }
        public int v_tem { get; set; }
        public int v_zayv { get; set; }
        public int v_str { get; set; }
        public Nullable<int> v_men { get; set; }
        public Nullable<int> v_per { get; set; }
        public string v_dir { get; set; }
        public Nullable<int> v_red { get; set; }
        public string v_name_short { get; set; }
        public string v_name_long { get; set; }
        public int parent { get; set; }
    
        public virtual ICollection<Job> Jobs { get; set; }
        public virtual ICollection<JobParticipant> JobParticipants { get; set; }
    }
}