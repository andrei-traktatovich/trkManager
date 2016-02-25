using System.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;

namespace TicketDataModel
{
    [MetadataType(typeof(JobMetadata))]
    public partial class Job
    {
        // Note this class has nothing in it.  It's just here to add the class-level attribute.
    }

    public partial class JobMetadata
    {
        public string ID;
        
        [Required]
        [Display(Name="Номер")]
        public string Name;

        [Display(Name="Дата сдачи план.")]
        public Nullable<System.DateTime> PlannedCompletionDate;

        [Display(Name="Дата принятия")]
        public Nullable<System.DateTime> TakeUpDate;

        [Display(Name="Дата завершения факт.")]
        public Nullable<System.DateTime> CompletedDate;

        [Display(Name="Дата отправки заказчику факт.")]
        public Nullable<System.DateTime> SentToCustomerDate;
        
        [Display(Name="Вх.объем, зн.")]
        public Nullable<int> CharCount;

        [Display(Name="Вх.объем, стр.")]
        public Nullable<double> PagesCount;

        [Display(Name="Вх.объем, зн.с пр.")]
        public Nullable<int> CharCountWithSpaces;

        [Display(Name="Вх.объем, стр.с пр.")]
        public Nullable<double> PagesCountWithSpaces;

        [Display(Name="Исх.объем, зн.")]
        public Nullable<int> FinalCharCount;

        [Display(Name="Исх.объем, стр.")]
        public Nullable<double> FinalPagesCount;

        [Display(Name="Исх.объем, зн.с пр.")]
        public Nullable<int> FinalCharCountWithSpaces;

        [Display(Name="Исх.объем, стр.с пр.")]
        public Nullable<double> FinalPagesCountWithSpaces;

        [Display(Name="Стр.заказчику")] // is it really? 
        public Nullable<double> FinalPagesCountForCustomer;

        [Display(Name="Тариф")]
        public Nullable<double> Rate;
        
        // what is this
        public Nullable<double> AmountAtJobCreate;
        public Nullable<double> AmountCash;
        public Nullable<double> AmountFinal;

        [Display(Name="Комментарий")]
        public string Comment;
        public string InvoiceOrCashPaymentInfoString;
        public Nullable<System.DateTime> InvoiceDate;
        public Nullable<System.DateTime> InvoicePaidDate;
        
        
        public string SpecificationNumberString;
        [Display(Name="Дата бланка")]
        public Nullable<System.DateTime> SpecificationDate;
        [Display(Name="Поле \"документ\"")]
        public string DocumentString;

        [Display(Name="Нотариат")]
        public string Notarized;

        [Display(Name="Треб.заявка расп.")]
        public string OCRRequested;
        [Display(Name="Треб.заявка анализ")]
        public string AnalysisRequested;
        [Display(Name="Треб.извл.терм.")]
        public string TermsRequired;
        [Display(Name="Треб.предперевод")]
        public string PretransRequired;
        [Display(Name="Контактное лицо")]
        public string ContactName;
        [Display(Name="Тел.контактного лица")]
        public string ContactTel;
        [Display(Name="Эл.почта кон.лица")]
        public string ContactMail;
        [Display(Name="Публикация")]
        public string Publication;

        public string an_1 ;
        public string an_2 ;
        public string an_3 ;
        public string an_4 ;
        public string an_5 ;
        public string an_6 ;
        public string an_7 ;
        [Display(Name="Доп.№")]
        public string AdditionalNumber;
        [Display(Name="Договор")]
        public string Contract;


        [Display(Name = "Подразделение-создатель родительского заказа")]
        public string ParentOfficeName;
        [Display(Name="Менеджер-создатель родительского заказа")]
        public string ParentManagerName;
        [Display(Name="Создатель заказа")]
        public string CreatedBy;
        [Display(Name="Дата создания заказа")]
        public Nullable<System.DateTime> CreatedDate;
        [Display(Name="Автор последний изменений")]
        public string ChangedBy;
        [Display(Name="Дата последних изменений")]
        public Nullable<System.DateTime> ChangedDate ;

        [Display(Name="Дата распознания плановая")]
        public Nullable<System.DateTime> OCRDatePlanned;
        [Display(Name="Дата распознания факт")]
        public Nullable<System.DateTime> OCRDateActual;
        [Display(Name="Исполнитель распознавания")]
        public string OCRDoneBy;
        [Display(Name="Объем расп., стр.")]
        public Nullable<double> PagesOCR;
        [Display(Name="Путь к заказу")]
        public string FilePath ;
        
        [Display(Name="Дата сдачи редактору")]
        public Nullable<System.DateTime> DateForEditor;

    }
}
