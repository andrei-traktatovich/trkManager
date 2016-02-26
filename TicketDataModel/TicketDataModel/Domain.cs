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
    
    public partial class Domain
    {
        public Domain()
        {
            this.JobsWhereThisDomainIsPrimary = new HashSet<Job>();
            this.JobsWhereThisIsSecondaryDomain = new HashSet<Job>();
            this.TranslatorsDomains = new HashSet<InternalDomain>();
        }
    
        public int ID { get; set; }
        public string Name { get; set; }
        public Nullable<int> ParentID { get; set; }
        public string t_ch { get; set; }
        public Nullable<int> t_kat { get; set; }
        public string Description { get; set; }
        public int CreatedByID { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> ModifiedByID { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public bool IsDeleted { get; set; }
    
        public virtual ICollection<Job> JobsWhereThisDomainIsPrimary { get; set; }
        public virtual ICollection<Job> JobsWhereThisIsSecondaryDomain { get; set; }
        public virtual ICollection<InternalDomain> TranslatorsDomains { get; set; }
        public virtual Translator CreatedBy { get; set; }
        public virtual Translator ModifiedBy { get; set; }
    }
}