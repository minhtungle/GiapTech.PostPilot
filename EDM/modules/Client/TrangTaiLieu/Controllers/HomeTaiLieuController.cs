using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TrangTaiLieu.Controllers
{
    public class HomeTaiLieuController : Controller
    {
        #region Biến public để in hoa
        private readonly string VIEW_PATH = "~/Views/User/TrangTaiLieu";
        IsoDateTimeConverter DATETIMECONVERTER = new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" };

        public HomeTaiLieuController()
        {

        }
        #endregion
        public ActionResult Index()
        {
            return View($"{VIEW_PATH}/trangchu.cshtml");
        }

    }
}