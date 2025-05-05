using Public.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EDM.Controllers
{
    public class MessengerController : RouteConfigController
    {
        // GET: Messenger
        public ActionResult Index()
        {
            return View("~/Views/Admin/__Home/Messenger/messenger.cshtml");
        }
    }
}