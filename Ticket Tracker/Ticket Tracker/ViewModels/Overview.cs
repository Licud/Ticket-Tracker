using System.Collections.Generic;
using Ticket_Tracker.DAL.Models;

namespace Ticket_Tracker.ViewModels
{
    public class Overview
    {
        
        public IEnumerable<Customer> Customers { get; set; }

        public IEnumerable<Ticket> Tickets { get; set; }

        public IEnumerable<DailyTicketCount> DailyTicketCount { get; set; }
    }
}