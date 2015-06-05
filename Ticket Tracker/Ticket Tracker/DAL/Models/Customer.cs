using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Ticket_Tracker.DAL.Models
{
    public class Customer
    {
        [Required]
        public int CustomerId {get; set;}

        [Display(Name = "Active?")]
        public bool Active {get; set;}

        public string MainCustomerContact {get; set;}
        
        [Display(Name="Customer")]
        public string Name {get; set;}

        [Display(Name = "Notes")]
        public string NotesOne {get; set;}

        [Display(Name = "Closed Tickets")]
        public int NumClosedTickets {get; set;}

        [Display(Name = "Open Tickets")]
        public int NumOpenTickets {get; set;}

        [Display(Name = "Open with Customer")]
        public int NumOpenTicketsCust {get; set;}

        [Display(Name = "Open with Relayware")]
        public int NumOpenTicketsRelayware {get; set;}

        [Display(Name = "Relayware Contact")]
        public string RelaywareContact {get; set;}

        [Display(Name = "Relayware Resource")]
        public string RelaywareResource {get; set;}

        [Display(Name = "Service Duration")]
        public string ServiceDuration {get; set;}

        [DataType(DataType.Date)]
        [Display(Name = "Service Start Date")]
        public DateTime ServiceStartDate { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Service End Date")]
        public DateTime ServiceEndDate {get; set;}

        public IEnumerable<Ticket> Ticket {get; set;}

    }
}