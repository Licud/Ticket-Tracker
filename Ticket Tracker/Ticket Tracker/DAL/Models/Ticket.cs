using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Ticket_Tracker.DAL.Models
{
    public class Ticket
    {
        [Required]
        public int TicketId { get; set; }

        [Required]
        [Display(Name="Ticket #")]
        public int TicketNumber { get; set; }
        
        public string ActionWith {get; set;}

        [DataType(DataType.Date)]
        [Display(Name="Date Created")]
        public DateTime DateCreated {get; set;}

        [Display(Name = "Priority")]
        public string DefinedPriority {get; set;}

        public string Description {get; set;}

        [Display(Name="Progress with")]
        public string InProgressWith {get; set;}

        public string Notes {get; set;}

        public string Status {get; set;}

        public Customer Customer {get; set;}
    }
}