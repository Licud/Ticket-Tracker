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
    public class DailyTicketCountsController : Controller
    {
        private UnitOfWork unitOfWork = new UnitOfWork();

        // GET: DailyTicketCounts
        public ActionResult Index()
        {
            Overview overview = new Overview()
            {
                Tickets = this.unitOfWork.TicketRepository.GetAllRecords(),
                Customers = this.unitOfWork.CustomerRepository.GetAllRecords(),
                DailyTicketCount = this.unitOfWork.DailyTicketCountRepository.GetAllRecords()
            };

            return View(overview);
        }

        // GET: DailyTicketCounts/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DailyTicketCount dailyTicketCount = unitOfWork.DailyTicketCountRepository.GetSingleRecord(id.Value);
            if (dailyTicketCount == null)
            {
                return HttpNotFound();
            }
            return View(dailyTicketCount);
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
