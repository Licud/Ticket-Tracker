﻿using System;
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
    public class CustomersController : Controller
    {
        private UnitOfWork unitOfWork = new UnitOfWork();

        // GET: Customers
        public ActionResult Index()
        {
            return View(unitOfWork.CustomerRepository.GetAllRecords());
        }

        // GET: Customers/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CustomerDetails customerDetails = new CustomerDetails();

            customerDetails.Customer = unitOfWork.CustomerRepository.GetSingleRecord(id.Value);

            customerDetails.Tickets = unitOfWork.TicketRepository.GetAllTicketsByCustomer(customerDetails.Customer.CustomerId);

            if (customerDetails.Customer == null)
            {
                return HttpNotFound();
            }
            return View(customerDetails);
        }

        // GET: Customers/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Customers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Active,MainCustomerContact,Name,NotesOne,RelaywareContact,RelaywareResource,ServiceDuration,ServiceStartDate,ServiceEndDate")] Customer customer)
        {
            if (ModelState.IsValid)
            {
                unitOfWork.CustomerRepository.AddRecord(customer);

                unitOfWork.SaveChanges();

                return RedirectToAction("Index", "DailyTicketCounts", null);
            }

            return View(customer);
        }

        // GET: Customers/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Customer customer = unitOfWork.CustomerRepository.GetSingleRecord(id.Value);
            if (customer == null)
            {
                return HttpNotFound();
            }
            return View(customer);
        }

        // POST: Customers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "CustomerId,Active,MainCustomerContact,Name,NotesOne,RelaywareContact,RelaywareResource,ServiceDuration,ServiceStartDate,ServiceEndDate")] Customer customer)
        {
            if (ModelState.IsValid)
            {
                unitOfWork.CustomerRepository.UpdateRecord(customer);

                unitOfWork.SaveChanges();

                return RedirectToAction("Index", "DailyTicketCounts", null);
            }
            return View(customer);
        }

        // GET: Customers/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Customer customer = unitOfWork.CustomerRepository.GetSingleRecord(id.Value);
            if (customer == null)
            {
                return HttpNotFound();
            }
            return View(customer);
        }

        // POST: Customers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Customer customer = unitOfWork.CustomerRepository.GetSingleRecord(id);

            IEnumerable<Ticket> tickets = unitOfWork.TicketRepository.GetAllTicketsByCustomer(customer.CustomerId);

            foreach(var ticket in tickets)
            {
                unitOfWork.TicketRepository.DeleteRecord(ticket);
            }

            unitOfWork.CustomerRepository.DeleteRecord(customer);

            unitOfWork.SaveChanges();

            return RedirectToAction("Index", "DailyTicketCounts", null);
        }

        [HttpPost]
        public JsonResult GetDetails(int id)
        {
            Customer customer = unitOfWork.CustomerRepository.GetSingleRecord(id);

            var result = new
            {
                active = customer.Active,
                custContact = customer.MainCustomerContact,
                name = customer.Name,
                notes = customer.NotesOne,
                numClosedTickets = customer.NumClosedTickets,
                numOpenTickets = customer.NumOpenTickets,
                openCust = customer.NumOpenTicketsCust,
                openRW = customer.NumOpenTicketsRelayware,
                rwContact = customer.RelaywareContact,
                rwResource = customer.RelaywareResource,
                servDuration = customer.ServiceDuration,
                endDate = customer.ServiceEndDate.ToShortDateString(),
                startDate = customer.ServiceStartDate.ToShortDateString(),
            };

            return Json(result);
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
