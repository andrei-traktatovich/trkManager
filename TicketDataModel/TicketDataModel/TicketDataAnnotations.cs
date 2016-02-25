using System.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;

namespace TicketDataModel
{


    [MetadataType(typeof(TicketMetadata))]
    public partial class Ticket
    {
        // Note this class has nothing in it.  It's just here to add the class-level attribute.
    }

    public partial class TicketMetadata
    {
        [Required(ErrorMessage = "Необходимо ввести текст")]
        [StringLength(2999, ErrorMessage = "Слишком много букав")]
        public string Text;

        [Required(ErrorMessage = "Необходимо ввести текст")]
        [StringLength(49, ErrorMessage = "Слишком много букав")]
        public string KeyWords;

        
    }
}