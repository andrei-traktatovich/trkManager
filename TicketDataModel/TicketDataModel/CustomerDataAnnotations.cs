using System.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;

namespace TicketDataModel
{
    [MetadataType(typeof(CustomerMetaData))]
    public partial class Customer
    {
        // Note this class has nothing in it.  It's just here to add the class-level attribute.
    }

    public partial class CustomerMetaData
    {
        [Display(Name = "Полное наименование заказчика")]
        public string Name;

        [Display(Name = "Сокращенное наименование заказчика")]
        public string ShortName;

        [Display(Name = "Код заказчика")]
        public string CustomerCode;

        [Display(Name = "Текущий договор с заказчиком")]
        public string CurrentContractName;
        [Display(Name = "Дата истечения текущего договора с заказчиком")]
        public Nullable<System.DateTime> CurrentContractExpiryDate;
        [Display(Name = "Юр.адрес заказчика")]
        public string LegalAddress;
        [Display(Name = "Факт.адрес заказчика")]
        public string ActualAddress;

        [Display(Name = "ИНН")]
        public string VATNo;
        [Display(Name = "КПП")]
        public string KPP;
        [Display(Name = "Банк")]
        public string Bank;
        [Display(Name = "БИК")]
        public string BIC;
        [Display(Name = "Расчетный счет")]
        public string SettlementAccount;
        [Display(Name = "Кор.счет")]
        public string CorrespondentAccount;
        [Display(Name = "Примечание")]
        public string Comment;
        [Display(Name = "Прайс")]
        public string PriceName;
        [Display(Name = "Источник привлечения")]
        public string Source;
        [Display(Name = "Деятельность")]
        public string ActivityType;
        [Display(Name = "Отрасль")]
        public string BranchOfEconomy;
        public Nullable<System.DateTime> Izm;
        public int LegalEntity;
        public int BranchID;
        public string pod_nic;
        public int klas;
        public int MarketingClassification;
        public string Site;
        public string Metro;
        public int Tax;
        public string Manager;
        public string DefaultPath;
        public Nullable<int> InternalCustomerOfficeID;
        public int IsDeleted;
    }
}
