using System.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;

namespace TicketDataModel
{
    [MetadataType(typeof(JobStatusMetadata))]
    public partial class JobStatus
    {
        // Note this class has nothing in it.  It's just here to add the class-level attribute.
    }

    public partial class JobStatusMetadata
    {
        [Display(Name="Статус выполнения вида работ")]
        public string Name;
    }

    [MetadataType(typeof(AnalysisRequestStatusMetadata))]
    public partial class AnalysisRequestStatus
    {
        // Note this class has nothing in it.  It's just here to add the class-level attribute.
    }

    public partial class AnalysisRequestStatusMetadata
    {
        [Display(Name = "Статус заявки на анализ")]
        public string Name;
    }

    [MetadataType(typeof(JobParticipantStatusMetadata))]
    public partial class JobParticipantStatus
    {
        // Note this class has nothing in it.  It's just here to add the class-level attribute.
    }

    public partial class JobParticipantStatusMetadata
    {
        [Display(Name = "Статус исполнителя")]
        public string Name;
    }

    [MetadataType(typeof(CategoryMetaData))]
    public partial class Category
    {
        // Note this class has nothing in it.  It's just here to add the class-level attribute.
    }

    public partial class CategoryMetaData
    {
        [Display(Name = "Приоритет качества")]
        public string Name;
    }

    [MetadataType(typeof(MarketingClassMetaData))]
    public partial class MarketingClass
    {
        // Note this class has nothing in it.  It's just here to add the class-level attribute.
    }

    public partial class MarketingClassMetaData
    {
        [Display(Name = "Маркетинговая классификация")]
        public string Name;
    }


    [MetadataType(typeof(OCRRequestStatusMetadata))]
    public partial class OCRRequestStatus
    {
        // Note this class has nothing in it.  It's just here to add the class-level attribute.
    }

    public partial class OCRRequestStatusMetadata
    {
        [Display(Name = "Статус заявки на распознавание")]
        public string Name;
    }

    [MetadataType(typeof(AccountingStatusMetadata))]
    public partial class AccountingStatus
    {
        // Note this class has nothing in it.  It's just here to add the class-level attribute.
    }

    public partial class AccountingStatusMetadata
    {
        [Display(Name = "Статус бухгалтерии")]
        public string Name;
    }

    [MetadataType(typeof(OMKRequestStatusMetaData))]
    public partial class OMKRequestStatus
    {
        // Note this class has nothing in it.  It's just here to add the class-level attribute.
    }

    public partial class OMKRequestStatusMetaData
    {
        [Display(Name = "Статус заявки ОМК")]
        public string Name;
    }

    [MetadataType(typeof(JobTypeMetaData))]
    public partial class JobType
    {
        // Note this class has nothing in it.  It's just here to add the class-level attribute.
    }

    public partial class JobTypeMetaData
    {
        [Display(Name = "Вид работ")]
        public string Name;
    }

    [MetadataType(typeof(LanguageMetaData))]
    public partial class Language
    {
        // Note this class has nothing in it.  It's just here to add the class-level attribute.
    }

    public partial class LanguageMetaData
    {
        [Display(Name = "Напр.перевода")]
        public string Name;
    }

    [MetadataType(typeof(DomainMetaData))]
    public partial class Domain
    {
        // Note this class has nothing in it.  It's just here to add the class-level attribute.
    }

    public partial class DomainMetaData
    {
        [Display(Name = "Наименование")]
        public string Name;
    }

}