using DocumentFormat.OpenXml.EMMA;
using EDM.SignalR.Chat;
using EDM_DB;
using Microsoft.AspNet.SignalR;
using Newtonsoft.Json;
using Public.Controllers;
using Public.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Runtime.Caching;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Helpers;
using System.Web.Http.Results;
using System.Web.Mvc;
using UserAccount.Models;
using UserType.Models;

namespace EDM.Controllers
{
    public class HomeController : StaticArgController
    {
        #region Biến public để in hoa
        private readonly string VIEW_PATH = "~/Views/Admin/__Home/TrangChu";

        private List<default_tbQuocGia> QUOCGIAs
        {
            get
            {
                return Session["QUOCGIAs"] as List<default_tbQuocGia> ?? new List<default_tbQuocGia>();
            }
            set
            {
                Session["QUOCGIAs"] = value;
            }
        }
        private string CHUCNANGS_HTML
        {
            get
            {
                return Session["CHUCNANGS_HTML"] as string ?? string.Empty;
            }
            set
            {
                Session["CHUCNANGS_HTML"] = value;
            }
        }
        private KiemTraNguoiDungHoatDongController kiemTraNguoiDungHoatDongController = new KiemTraNguoiDungHoatDongController();

        private static Timer timer;
        private static readonly object lockObject = new object();
        private const string CacheKey = "KiemTraNguoiDungHoatDong";
        private readonly IHubContext<ChatHub> _hubContext;
        private readonly int THOIGIANKIEMTRANGUOIDUNGDANGNHAP = 2;
        #endregion
        public HomeController()
        {
        }
        public ActionResult Index()
        {
            string view = "";
            // Kiểm tra quyền truy cập
            if (per.NguoiDung.IdNguoiDung == Guid.Empty)
            {
                string currentDomain = Request.Url.Host.ToLower();
                Permission per = new Permission
                {
                    DonViSuDung = layDonViSuDung(),
                    Role = "USER"
                };
                Session["Permission"] = per; // Phải set như này thì từ sau mới sử dụng được session
                view = "~/Views/Auth/auth.login.cshtml";
                if (per.DonViSuDung.SuDungTrangNguoiDung.Value) view = "~/Views/User/TrangKhoaHoc/__Home/home.cshtml";
            }
            else
            {
                per.Role = "ADMIN";
                CHUCNANGS_HTML = getSideBarMenu();
                view = $"{VIEW_PATH}/trangchu.cshtml";
                // Khởi tạo timer khi admin được tạo
                //kiemTraNguoiDungHoatDongController.TienHanhKiemTra(per: per);
            };

            #region Lấy thông tin chung
            #region Quốc gia
            List<default_tbQuocGia> quocGias = db.default_tbQuocGia.Where(x => x.TrangThai != 0).ToList() ?? new List<default_tbQuocGia>();
            #endregion
            #endregion

            QUOCGIAs = quocGias;
            return View(view);
        }

        #region Admin
        public tbDonViSuDung layDonViSuDung()
        {
            string currentDomain = Request.Url.Host.ToLower();
            //currentDomain = "vietgenedu.com"; // Dùng để test
            var donViSuDung = db.Database.SqlQuery<tbDonViSuDung>($@"
            select * from tbDonViSuDung
                where TrangThai = 1
                AND RTRIM(REPLACE(REPLACE(REPLACE(REPLACE(TenMien, 'https://', ''), 'http://', ''), 'www.', ''), '/','')) = '{currentDomain}'
            ").FirstOrDefault() ?? new tbDonViSuDung();
            return donViSuDung;
        }

        #region Tạo giao diện
        public default_tbChucNangExtend get_ChucNangs(default_tbChucNangExtend chucNang)
        {
            List<ChucNangs> kieuNguoiDung_IdChucNang = JsonConvert.DeserializeObject<List<ChucNangs>>(per.KieuNguoiDung.IdChucNang);
            List<string> idChucNangs = kieuNguoiDung_IdChucNang.Select(x => x.ChucNang).Select(x => string.Format("'{0}'", x.IdChucNang)).ToList();
            string chucnangSQL = $@"select * from default_tbChucNang where IdCha = '{chucNang.IdChucNang}' and TrangThai = 1 and IdChucNang in ({String.Join(",", idChucNangs)}) order by SoThuTu";
            List<default_tbChucNangExtend> chucNangs = db.Database.SqlQuery<default_tbChucNangExtend>(chucnangSQL).ToList() ?? new List<default_tbChucNangExtend>();
            foreach (var _chucNang in chucNangs)
            {
                get_ChucNangs(_chucNang);
            };
            chucNang.ChucNangs = chucNangs;
            return chucNang;
        }
        [HttpGet]
        public string getSideBarMenu()
        {
            if (CHUCNANGS_HTML == "")
            {
                default_tbChucNangExtend chucNang = get_ChucNangs(new default_tbChucNangExtend());
                string taoTree(List<default_tbChucNangExtend> ChucNangs)
                {
                    string HTML = string.Empty;
                    foreach (default_tbChucNangExtend _chucNang in ChucNangs)
                    {
                        int soLuongChilds = _chucNang.ChucNangs.Count;
                        string className = soLuongChilds > 0 ? $"has-sub page-group-{_chucNang.MaChucNang.ToLower()}" : $"page-{_chucNang.MaChucNang.ToLower()}";
                        className += _chucNang.IdCha == Guid.Empty ? " sidebar-item" : " submenu-item";
                        HTML +=
                            $"<!--{_chucNang.TenChucNang}-->" +
                            $"<li class=\"{className}\">" +
                            $"   <a href=\"{(soLuongChilds > 0 ? "" : $"/{_chucNang.MaChucNang}")}\" class=\"sidebar-link\">" +
                            $"       <i class=\"{_chucNang.Icon}\"></i>" +
                            $"       <span>{_chucNang.TenChucNang}</span>" +
                            "   </a>" +
                            (soLuongChilds > 0 ? $"<ul class=\"submenu page-group-{_chucNang.MaChucNang.ToLower()}\">{taoTree(_chucNang.ChucNangs)}</ul>" : string.Empty) +
                            "</li>";
                    }
                    return HTML;
                }
                return taoTree(chucNang.ChucNangs);
            }
            return CHUCNANGS_HTML;
        }
        #endregion

        #region Hàm xử lý phiên đăng nhập
        [HttpGet]
        public ActionResult KeepSessionAlive()
        {
            Session.Timeout = 20; // Cập nhật thời gian hết hạn của session là 20 phút
            return new EmptyResult();
        }
        #endregion

        #endregion

        #region Client - Tai lieu

        #endregion
    }
}
class tool
{
    public string TenCapDo_DoanhThu { get; set; } = string.Empty;
}