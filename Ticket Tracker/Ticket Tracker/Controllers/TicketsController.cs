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
    public class TicketsController : Controller
    {
        private UnitOfWork unitOfWork = new UnitOfWork();

        // GET: Tickets
        public ActionResult Index()
        {
            return View(unitOfWork.TicketRepository.GetAllRecords());
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
            IEnumerable<Customer> customers = unitOfWork.CustomerRepository.GetAllRecords();

            List<SelectListItem> customerList = new List<SelectListItem>();

            foreach(var customer in customers )
            {
                customerList.Add(new SelectListItem() { Text = customer.Name, Value = customer.CustomerId.ToString()} );
            }

            ViewBag.Customer = customerList;

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

                if(cTicket.Ticket.ActionWith == "Relayware")
                {
                    customer.NumOpenTicketsRelayware += 1;
                }
                else if(cTicket.Ticket.ActionWith == "Customer")
                {
                    customer.NumOpenTicketsCust += 1;
                }

                if(cTicket.Ticket.Status == "Open")
                {
                    customer.NumOpenTickets += 1;
                }
                else if(cTicket.Ticket.Status == "Closed")
                {
                    customer.NumClosedTickets += 1;
                }

                unitOfWork.CustomerRepository.UpdateRecord(customer);

                cTicket.Ticket.Customer = customer;

                unitOfWork.TicketRepository.AddRecord(cTicket.Ticket);

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
            Ticket ticket = unitOfWork.TicketRepository.GetSingleRecord(id.Value);
            unitOfWork.SaveChanges();
            if (ticket == null)
            {
                return HttpNotFound();
            }
            return View(ticket);
        }

        // POST: Tickets/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "TicketId,ActionWith,DateCreated,DefinedPriority,Description,InProgressWith,Notes,Status")] Ticket ticket)
        {
            if (ModelState.IsValid)
            {
                unitOfWork.TicketRepository.UpdateRecord(ticket);
                unitOfWork.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(ticket);
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
        public JsonResult CloseTicket(int id)
        {
            Ticket ticket = unitOfWork.TicketRepository.GetSingleRecord(id);

            ticket.Status = "Closed";

            unitOfWork.TicketRepository.UpdateRecord(ticket);

            unitOfWork.SaveChanges();

            return Json(new { status = "Closed" });
        }

        [HttpPost]
        public JsonResult ChangeAction(int id)
        {
            Ticket ticket = unitOfWork.TicketRepository.GetSingleRecord(id);

            string newAction = null;

            if(ticket.ActionWith == "Relayware")
            {
                ticket.ActionWith = "Customer";
                newAction = "Customer";
            }
            else if (ticket.ActionWith == "Customer")
            {
                ticket.ActionWith = "Relayware";
                newAction = "Relayware";
            }

            unitOfWork.TicketRepository.UpdateRecord(ticket);

            unitOfWork.SaveChanges();

            return Json(new { action = newAction, ticketId = ticket.TicketId});
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
