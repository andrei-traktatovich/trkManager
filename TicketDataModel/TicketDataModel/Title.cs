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
    
    public partial class Title
    {
        public Title()
        {
            this.Translators = new HashSet<Translator>();
        }
    
        public int ID { get; set; }
        public string Name { get; set; }
        public string d_ch { get; set; }
        public Nullable<int> superordinate_id { get; set; }
    
        public virtual ICollection<Translator> Translators { get; set; }
    }
}
