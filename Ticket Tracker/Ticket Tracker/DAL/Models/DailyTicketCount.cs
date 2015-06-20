using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Ticket_Tracker.DAL.Models
{
    public class DailyTicketCount
    {
        [Required]
        public int DailyTicketCountId {get; set;}

        [DataType(DataType.Date)]
        public DateTime Day {get; set;}

        [Display(Name = "Total Closed Tickets")]
        public int TotalClosedTickets {get; set;}

        [Display(Name = "Total Opened Tickets")]
        public int TotalOpenedTicket { get; set; }

        [Display(Name = "Total Open Tickets - Customer")]
        public int TotalOpenTicketsCust {get; set;}

        [Display(Name = "Total Open Tickets - Relayware")]
        public int TotalOpenTicketsRelayware {get; set;}
    }
}