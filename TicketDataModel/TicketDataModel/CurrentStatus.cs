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
    
    public partial class CurrentStatus
    {
        public CurrentStatus()
        {
            this.Translators = new HashSet<Translator>();
        }
    
        public int ID { get; set; }
        public string Name { get; set; }
        public string s_ch { get; set; }
    
        public virtual ICollection<Translator> Translators { get; set; }
    }
}