using Public.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DocumentAuth.Controllers
{
    public class DocumentAuth : RouteConfigController
    {
        public ActionResult Index()
        {
            return View("~/Views/Admin/_DocumentManage/DocumentAuth/Index.cshtml");
        }
        public ActionResult getList()
        {
            return PartialView("~/Views/Admin/_DocumentManage/DocumentAuth/documentauth-getList.cshtml");
        }
    }
}