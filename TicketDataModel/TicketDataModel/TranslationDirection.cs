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
    
    public partial class TranslationDirection
    {
        public TranslationDirection()
        {
            this.DomainsInLanguageDirection = new HashSet<InternalDomain>();
        }
    
        public int ID { get; set; }
        public int TranslatorID { get; set; }
        public string LanguageName { get; set; }
        public int LanguageID { get; set; }
        public Nullable<double> MinWrittenTranslationRate { get; set; }
        public Nullable<double> MaxWrittenTranslationRate { get; set; }
        public Nullable<int> l_lan_inout { get; set; }
        public Nullable<System.DateTime> Izm { get; set; }
        public Nullable<double> LanguageQA { get; set; }
        public string l_sin { get; set; }
        public string l_pos { get; set; }
        public string l_red { get; set; }
        public string l_nos { get; set; }
        public Nullable<double> l_pro { get; set; }
        public Nullable<double> l_pro_sro { get; set; }
        public string l_lan_dop { get; set; }
        public string l_ver { get; set; }
        public string l_per { get; set; }
        public Nullable<double> l_red_in { get; set; }
        public Nullable<double> l_red_out { get; set; }
        public Nullable<double> l_sin_in { get; set; }
        public Nullable<double> l_sin_out { get; set; }
        public Nullable<double> l_pos_in { get; set; }
        public Nullable<double> l_pos_out { get; set; }
        public Nullable<double> l_ver_in { get; set; }
        public Nullable<double> l_ver_out { get; set; }
        public Nullable<int> l_ver_inout { get; set; }
        public Nullable<int> l_red_inout { get; set; }
        public int Competence { get; set; }
        public string l_tem { get; set; }
    
        public virtual Translator Translator { get; set; }
        public virtual Language Languages { get; set; }
        public virtual ICollection<InternalDomain> DomainsInLanguageDirection { get; set; }
    }
}
