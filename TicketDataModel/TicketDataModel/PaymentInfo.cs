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
    
    public partial class PaymentInfo
    {
        public Nullable<int> CashReceiptNoteNumber { get; set; }
        public string JobName { get; set; }
        public string JobGroupID { get; set; }
        public Nullable<int> p_id { get; set; }
        public string ContactPerson { get; set; }
        public Nullable<System.DateTime> PaymentDate { get; set; }
        public Nullable<double> PaymentAmount { get; set; }
        public string Comment { get; set; }
        public string PaymentNameText { get; set; }
        public Nullable<int> OfficeID { get; set; }
        public Nullable<int> zakaz { get; set; }
        public string Text { get; set; }
        public string SpecificationText { get; set; }
        public Nullable<System.DateTime> SpecificationDate { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<double> AmountDue { get; set; }
        public int LegalEntityID { get; set; }
        public Nullable<System.DateTime> ChangedDate { get; set; }
        public Nullable<System.DateTime> Izm_Dt { get; set; }
        public Nullable<int> Status { get; set; }
        public int PaymentStatusID { get; set; }
        public int PaymentMethodID { get; set; }
        public int ID { get; set; }
        public int CreatedByID { get; set; }
        public Nullable<int> ModifiedByID { get; set; }
    
        public virtual Office Office { get; set; }
        public virtual PaymentMethod PaymentMethod { get; set; }
        public virtual PaymentItemType ItemType { get; set; }
        public virtual Translator CreatedBy { get; set; }
        public virtual Translator ModifiedBy { get; set; }
    }
}
