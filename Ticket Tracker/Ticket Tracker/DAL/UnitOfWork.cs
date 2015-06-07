using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Ticket_Tracker.DAL.Models;
using Ticket_Tracker.DAL.Repositories;

namespace Ticket_Tracker.DAL
{
    public class UnitOfWork : IDisposable
    {
        private ApplicationDbContext context = new ApplicationDbContext();
        private TicketRepository ticketRepository;
        private GenericRepository<Customer> customerRepository;
        private GenericRepository<DailyTicketCount> dailyTicketCountRepository;

        public TicketRepository TicketRepository 
        {
            get 
            {
                if (ticketRepository == null)
                {
                    this.ticketRepository = new TicketRepository(context);
                }
                return this.ticketRepository;
            }
        }

        public GenericRepository<Customer> CustomerRepository
        {
            get
            {
                if (customerRepository == null)
                {
                    this.customerRepository = new GenericRepository<Customer>(context);
                }
                return this.customerRepository;
            }
        }

        public GenericRepository<DailyTicketCount> DailyTicketCountRepository
        {
            get
            {
                if (dailyTicketCountRepository == null)
                {
                    this.dailyTicketCountRepository = new GenericRepository<DailyTicketCount>(context);
                }
                return this.dailyTicketCountRepository;
            }
        }

        public ApplicationDbContext GetDbContext
        {
            get
            {
                if (this.context == null)
                {
                    context = new ApplicationDbContext();
                }
                return this.context;
            }
        }

        public void SaveChanges()
        {
            context.SaveChanges();
        }

        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if(!this.disposed)
            {
                if(disposing)
                {
                    context.Dispose();
                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
    
}