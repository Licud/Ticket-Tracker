﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ticket_Tracker.DAL.Models;

namespace Ticket_Tracker.ViewModels
{
    public class CustomerTicket
    {

        public string Customer { get; set; }

        public Ticket Ticket { get; set; }

    }
}