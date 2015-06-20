using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Ticket_Tracker.DAL;
using Ticket_Tracker.DAL.Models;
using Ticket_Tracker.ViewModels;

namespace Ticket_Tracker.Controllers
{
    [Authorize]
    public class TicketsController : Controller
    {
        private UnitOfWork unitOfWork = new UnitOfWork();

        // GET: Tickets
        public ActionResult Index()
        {
            return View(unitOfWork.TicketRepository.GetAllTicketsWithCustomers());
        }

        // GET: Tickets/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ticket ticket = unitOfWork.TicketRepository.GetSingleRecord(id.Value);
            if (ticket == null)
            {
                return HttpNotFound();
            }
            return View(ticket);
        }

        // GET: Tickets/Create
        public ActionResult Create()
        {
            ViewBag.Customer = this.GetCustomerList();

            return View();
        }

        // POST: Tickets/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CustomerTicket cTicket)
        {
            if (ModelState.IsValid)
            {
                Customer customer = unitOfWork.CustomerRepository.GetSingleRecord(Int32.Parse(cTicket.Customer));

                DailyTicketCount dailyCount = unitOfWork.DailyTicketCountRepository.LatestRecord();

                if (cTicket.Ticket.ActionWith.Equals("Relayware", StringComparison.Ordinal))
                {
                    customer.NumOpenTicketsRelayware += 1;
                    dailyCount.TotalOpenTicketsRelayware += 1;
                }
                else if (cTicket.Ticket.ActionWith.Equals("Customer", StringComparison.Ordinal))
                {
                    customer.NumOpenTicketsCust += 1;
                    dailyCount.TotalOpenTicketsCust += 1;
                }

                if (cTicket.Ticket.Status.Equals("Open", StringComparison.Ordinal))
                {
                    customer.NumOpenTickets += 1;
                    dailyCount.TotalOpenedTicket += 1;
                }
                else if (cTicket.Ticket.Status.Equals("Closed", StringComparison.Ordinal))
                {
                    customer.NumClosedTickets += 1;
                    dailyCount.TotalClosedTickets += 1;
                }

                unitOfWork.CustomerRepository.UpdateRecord(customer);

                cTicket.Ticket.Customer = customer;

                unitOfWork.TicketRepository.AddRecord(cTicket.Ticket);

                unitOfWork.DailyTicketCountRepository.UpdateRecord(dailyCount);

                unitOfWork.SaveChanges();

                return RedirectToAction("Index", "DailyTicketCounts", null);
            }

            return View();
        }

        // GET: Tickets/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Ticket ticket = unitOfWork.TicketRepository.GetSingleTicketDetails(id.Value);

            CustomerTicketWithState cTicket = new CustomerTicketWithState() { Ticket = ticket, CurrentStatus = ticket.Status, CurrentAction = ticket.ActionWith};

            ViewBag.Customer = this.GetCustomerList();

            if (cTicket.Ticket == null)
            {
                return HttpNotFound();
            }
            return View(cTicket);
        }

        // POST: Tickets/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(CustomerTicketWithState cTicket)
        {
            if (ModelState.IsValid)
            {
                Customer customer = unitOfWork.CustomerRepository.GetSingleRecord(Int32.Parse(cTicket.Customer));

                cTicket.Ticket.CustomerId = customer.CustomerId;

                unitOfWork.TicketRepository.UpdateRecord(cTicket.Ticket);
                unitOfWork.SaveChanges();

                reCountCustomerTickets();

                return RedirectToAction("Index", "DailyTicketCounts", null);
            }

            return View(cTicket);
        }

        // GET: Tickets/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ticket ticket = unitOfWork.TicketRepository.GetSingleRecord(id.Value);
            if (ticket == null)
            {
                return HttpNotFound();
            }
            return View(ticket);
        }

        // POST: Tickets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Ticket ticket = unitOfWork.TicketRepository.GetSingleRecord(id);
            unitOfWork.TicketRepository.DeleteRecord(ticket);
            unitOfWork.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpPost]
        public JsonResult GetDetails(int id)
        {
            Ticket ticket = unitOfWork.TicketRepository.GetSingleRecord(id);

            var result = new {  number = ticket.TicketNumber,
                                progress = ticket.InProgressWith,
                                dateCreated = ticket.DateCreated.ToShortDateString(),
                                priority = ticket.DefinedPriority,
                                description = ticket.Description,
                                notes = ticket.Notes,
                                status = ticket.Status,
                                actionWith = ticket.ActionWith};

            return Json(result);
        }

        [HttpPost]
        public JsonResult CloseTicket(int id, int cid)
        {
            Ticket ticket = unitOfWork.TicketRepository.GetSingleRecord(id);

            DailyTicketCount dCount = unitOfWork.DailyTicketCountRepository.LatestRecord();

            Customer customer = unitOfWork.CustomerRepository.GetSingleRecord(cid);

            dCount.TotalClosedTickets += 1;

            if (ticket.ActionWith.Equals("Relayware", StringComparison.Ordinal))
            {
                dCount.TotalOpenTicketsRelayware -= 1;
                customer.NumOpenTicketsRelayware -= 1;
                
            }
            else if (ticket.ActionWith.Equals("Customer", StringComparison.Ordinal))
            {
                dCount.TotalOpenTicketsCust -= 1;
                customer.NumOpenTicketsCust -= 1;
            }

            unitOfWork.DailyTicketCountRepository.UpdateRecord(dCount);

            ticket.Status = "Closed";

            unitOfWork.TicketRepository.UpdateRecord(ticket);

            customer.NumClosedTickets += 1;
            customer.NumOpenTickets -= 1;

            unitOfWork.CustomerRepository.UpdateRecord(customer);

            unitOfWork.SaveChanges();

            return Json(new { status = ticket.Status, openTickets = customer.NumOpenTickets,
                              custId = customer.CustomerId,
                              dCountOpen = dCount.TotalOpenedTicket,
                              dCountClosed = dCount.TotalClosedTickets,
                              dCountRW = dCount.TotalOpenTicketsRelayware,
                              dCountCust = dCount.TotalOpenTicketsCust,
                              dCountId = dCount.DailyTicketCountId,
                              custOpenRW = customer.NumOpenTicketsRelayware,
                              custOpenCustomer = customer.NumOpenTicketsCust});
        }

        [HttpPost]
        public JsonResult OpenTicket(int id, int cid)
        {
            Ticket ticket = unitOfWork.TicketRepository.GetSingleRecord(id);

            DailyTicketCount dCount = unitOfWork.DailyTicketCountRepository.LatestRecord();

            Customer customer = unitOfWork.CustomerRepository.GetSingleRecord(cid);

            dCount.TotalOpenedTicket += 1;

            if (ticket.ActionWith.Equals("Relayware", StringComparison.Ordinal))
            {
                dCount.TotalOpenTicketsRelayware += 1;
                customer.NumOpenTicketsRelayware += 1;
            }
            else if (ticket.ActionWith.Equals("Customer", StringComparison.Ordinal))
            {
                dCount.TotalOpenTicketsCust += 1;
                customer.NumOpenTicketsCust += 1;
            }

            unitOfWork.DailyTicketCountRepository.UpdateRecord(dCount);

            ticket.Status = "Open";

            unitOfWork.TicketRepository.UpdateRecord(ticket);


            customer.NumClosedTickets -= 1;
            customer.NumOpenTickets += 1;

            unitOfWork.CustomerRepository.UpdateRecord(customer);

            unitOfWork.SaveChanges();

            return Json(new { status = ticket.Status, openTickets = customer.NumOpenTickets,
                    custId = customer.CustomerId, dCountOpen = dCount.TotalOpenedTicket,
                      dCountRW = dCount.TotalOpenTicketsRelayware, dCountCust = dCount.TotalOpenTicketsCust,
                      dCountClosed = dCount.TotalClosedTickets});
        }

        [HttpPost]
        public JsonResult ChangeAction(int id, int cid)
        {
            Ticket ticket = unitOfWork.TicketRepository.GetSingleRecord(id);

            Customer customer = unitOfWork.CustomerRepository.GetSingleRecord(cid);

            DailyTicketCount dCount = unitOfWork.DailyTicketCountRepository.LatestRecord();

            if (ticket.ActionWith.Equals("Relayware", StringComparison.Ordinal))
            {
                if (dCount.TotalOpenTicketsRelayware > 0) 
                {
                    dCount.TotalOpenTicketsRelayware -= 1;
                }
                
                dCount.TotalOpenTicketsCust += 1;

                if (ticket.Status.Equals("Open", StringComparison.Ordinal))
                {
                    customer.NumOpenTicketsCust += 1;
                    customer.NumOpenTicketsRelayware -= 1;
                }
                ticket.ActionWith = "Customer";
            }
            else if (ticket.ActionWith.Equals("Customer", StringComparison.Ordinal))
            {
                if (dCount.TotalOpenTicketsCust > 0)
                {
                    dCount.TotalOpenTicketsCust -= 1;
                }

                dCount.TotalOpenTicketsRelayware += 1;  

                if (ticket.Status.Equals("Open", StringComparison.Ordinal))
                {
                    customer.NumOpenTicketsCust -= 1;
                    customer.NumOpenTicketsRelayware += 1;
                }
                ticket.ActionWith = "Relayware";
            }

            unitOfWork.DailyTicketCountRepository.UpdateRecord(dCount);

            unitOfWork.TicketRepository.UpdateRecord(ticket);

            unitOfWork.CustomerRepository.UpdateRecord(customer);

            unitOfWork.SaveChanges();

            return Json(new { action = ticket.ActionWith, ticketId = ticket.TicketId, custId = customer.CustomerId,
                                custOpenRW = customer.NumOpenTicketsRelayware, custOpen = customer.NumOpenTicketsCust,
                                dCountId = dCount.DailyTicketCountId, dCountCust = dCount.TotalOpenTicketsCust,
                                dCountRW = dCount.TotalOpenTicketsRelayware});
        }

        public void reCountCustomerTickets()
        {
            IEnumerable<Customer> customers = unitOfWork.CustomerRepository.GetAllRecords();

            foreach(var customer in customers)
            {
                customer.NumOpenTickets = unitOfWork.TicketRepository.GetAllTicketsByCustomer(customer.CustomerId).Where( c => c.Status == "Open").Count();
                customer.NumClosedTickets = unitOfWork.TicketRepository.GetAllTicketsByCustomer(customer.CustomerId).Where(c => c.Status == "Closed").Count();
                customer.NumOpenTicketsRelayware = unitOfWork.TicketRepository.GetAllTicketsByCustomer(customer.CustomerId).Where(c => c.ActionWith == "Relayware" && c.Status == "Open").Count();
                customer.NumOpenTicketsCust = unitOfWork.TicketRepository.GetAllTicketsByCustomer(customer.CustomerId).Where(c => c.ActionWith == "Customer" && c.Status == "Open").Count();
                unitOfWork.CustomerRepository.UpdateRecord(customer);

                unitOfWork.SaveChanges();
            }
        }

        public List<SelectListItem> GetCustomerList()
        {
            IEnumerable<Customer> customers = unitOfWork.CustomerRepository.GetAllRecords();

            List<SelectListItem> customerList = new List<SelectListItem>();

            foreach (var customer in customers)
            {
                customerList.Add(new SelectListItem() { Text = customer.Name, Value = customer.CustomerId.ToString() });
            }

            return customerList;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                unitOfWork.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
