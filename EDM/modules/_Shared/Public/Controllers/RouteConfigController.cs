using EDM_DB;
using Newtonsoft.Json;
using Public.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Public.Controllers
{
    public class RouteConfigController : StaticArgController
    {
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            // Kiểm tra đăng nhập
            if (per.NguoiDung.IdNguoiDung == Guid.Empty)
            {
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Auth", action = "Login" }));
            }
            else
            {
                // Lấy tên controller và action hiện tại
                string currentController = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName;
                string currentAction = filterContext.ActionDescriptor.ActionName;

                // Lấy danh sách quyền chức năng của người dùng
                List<ChucNangs> kieuNguoiDung_IdChucNang = JsonConvert.DeserializeObject<List<ChucNangs>>(per.KieuNguoiDung.IdChucNang);

                // Kiểm tra xem người dùng có quyền truy cập controller hiện tại hay không
                var hasPermission = kieuNguoiDung_IdChucNang.Any(cn => cn.ChucNang.MaChucNang == currentController);

                if (!hasPermission && currentAction == "Index")
                {
                    // Nếu không có quyền, chuyển hướng về trang Home/Index
                    filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Home", action = "Index" }));
                };
            };
            base.OnActionExecuting(filterContext);
        }
    }
}