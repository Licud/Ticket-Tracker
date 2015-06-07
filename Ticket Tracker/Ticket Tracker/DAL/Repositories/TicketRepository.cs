using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Ticket_Tracker.DAL.Models;

namespace Ticket_Tracker.DAL.Repositories
{
    public class TicketRepository : GenericRepository<Ticket>
    {
        public TicketRepository(ApplicationDbContext _context): base(_context)
        {
            
        }

        public IEnumerable<Ticket> GetAllTicketsByCustomer(int customerId)
        {
            return context.Ticket.Where(t => t.Customer.CustomerId == customerId);
        }

        public Ticket GetSingleTicketDetails(int id)
        {
            return context.Ticket.Include("Customer").Where(t => t.TicketId == id).FirstOrDefault();
        }
    }
}