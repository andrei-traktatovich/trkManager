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
    
    public partial class ExpensesReport_Result
    {
        public string TypeName { get; set; }
        public string OfficeName { get; set; }
        public string SubTypeName { get; set; }
        public string SubOfficeName { get; set; }
        public Nullable<int> Month { get; set; }
        public Nullable<int> Quarter { get; set; }
        public string Description { get; set; }
        public int NonCash { get; set; }
        public Nullable<double> Cash { get; set; }
        public Nullable<double> Amount { get; set; }
        public Nullable<System.DateTime> PaymentDate { get; set; }
        public string Source { get; set; }
    }
}
