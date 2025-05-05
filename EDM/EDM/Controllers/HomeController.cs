using DocumentFormat.OpenXml.EMMA;
using EDM.SignalR.Chat;
using EDM_DB;
using Microsoft.AspNet.SignalR;
using Newtonsoft.Json;
using Public.Controllers;
using Public.Models;
using QuanLyKhachHang.Models;
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

        private List<tbKhachHang_LoaiKhachHang> LOAIKHACHHANGs
        {
            get
            {
                return Session["LOAIKHACHHANGs"] as List<tbKhachHang_LoaiKhachHang> ?? new List<tbKhachHang_LoaiKhachHang>();
            }
            set
            {
                Session["LOAIKHACHHANGs"] = value;
            }
        }
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
                if (per.DonViSuDung.SuDungTrangNguoiDung.Value) view = "~/Views/User/TrangTaiLieu/__Home/home.cshtml";
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
            #region Loại khách hàng
            List<tbKhachHang_LoaiKhachHang> loaiKhachHangs = db.tbKhachHang_LoaiKhachHang.Where(x => x.TrangThai != 0).ToList() ?? new List<tbKhachHang_LoaiKhachHang>();
            #endregion
            #region Quốc gia
            List<default_tbQuocGia> quocGias = db.default_tbQuocGia.Where(x => x.TrangThai != 0).ToList() ?? new List<default_tbQuocGia>();
            #endregion
            #endregion

            LOAIKHACHHANGs = loaiKhachHangs;
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

        private JsonResult TOOL_capNhatCapDoDoanhThu()
        {
            string status = "success";
            string mess = "Thêm mới bản ghi thành công";
            using (var scope = db.Database.BeginTransaction())
            {
                try
                {
                    List<tool> nguoiDungs = new List<tool>
                    {
                        new tool {
                            NguoiDung_DoanhThu = new tbNguoiDung_DoanhThu {
                                IdNguoiDung_DoanhThu = Guid.NewGuid(),
                                DoanhThuMucTieu = 317617750,
                                DoanhThuThucTe = 317617750,
                                PhanTramHoanThien = 100,
                                NgayLenMucTieu = "08/2024",
                                NgayDatMucTieu = DateTime.Parse("2024-08-14 20:53:08.533"),
                                TrangThai = 1,
                                MaDonViSuDung = Guid.Parse("f4b89d3a-5246-4d90-83c2-2db9c3e4d9b7"),
                                NgayTao =  DateTime.Parse("2024-08-10 20:53:08.533"),
                                IdNguoiTao = db.tbNguoiDungs.FirstOrDefault(x => x.TenNguoiDung == "Dương Thị Minh Phương").IdNguoiDung,
                            },
                            TenCapDo_DoanhThu = "NS-2"
                        },
                        new tool {
                            NguoiDung_DoanhThu = new tbNguoiDung_DoanhThu {
                                IdNguoiDung_DoanhThu = Guid.NewGuid(),
                                DoanhThuMucTieu = 102597000,
                                DoanhThuThucTe = 102597000,
                                PhanTramHoanThien = 100,
                                NgayLenMucTieu = "08/2024",
                                NgayDatMucTieu = DateTime.Parse("2024-08-14 20:53:08.533"),
                                TrangThai = 1,
                                MaDonViSuDung = Guid.Parse("f4b89d3a-5246-4d90-83c2-2db9c3e4d9b7"),
                                NgayTao =  DateTime.Parse("2024-08-10 20:53:08.533"),
                                IdNguoiTao = db.tbNguoiDungs.FirstOrDefault(x => x.TenNguoiDung == "Cao Thị Khánh Huyền").IdNguoiDung,
                            },
                            TenCapDo_DoanhThu = "NS-1"
                        },
                        new tool {
                            NguoiDung_DoanhThu = new tbNguoiDung_DoanhThu {
                                IdNguoiDung_DoanhThu = Guid.NewGuid(),
                                DoanhThuMucTieu = 44018385,
                                DoanhThuThucTe = 44018385,
                                PhanTramHoanThien = 100,
                                NgayLenMucTieu = "08/2024",
                                NgayDatMucTieu = DateTime.Parse("2024-08-14 20:53:08.533"),
                                TrangThai = 1,
                                MaDonViSuDung = Guid.Parse("f4b89d3a-5246-4d90-83c2-2db9c3e4d9b7"),
                                NgayTao =  DateTime.Parse("2024-08-10 20:53:08.533"),
                                IdNguoiTao = db.tbNguoiDungs.FirstOrDefault(x => x.TenNguoiDung == "Chu Ngọc Hà Anh").IdNguoiDung,
                            },
                            TenCapDo_DoanhThu = "Thử việc"
                        },
                        new tool {
                            NguoiDung_DoanhThu = new tbNguoiDung_DoanhThu {
                                IdNguoiDung_DoanhThu = Guid.NewGuid(),
                                DoanhThuMucTieu = 1066524170,
                                DoanhThuThucTe = 1066524170,
                                PhanTramHoanThien = 100,
                                NgayLenMucTieu = "08/2024",
                                NgayDatMucTieu = DateTime.Parse("2024-08-14 20:53:08.533"),
                                TrangThai = 1,
                                MaDonViSuDung = Guid.Parse("f4b89d3a-5246-4d90-83c2-2db9c3e4d9b7"),
                                NgayTao =  DateTime.Parse("2024-08-10 20:53:08.533"),
                                IdNguoiTao = db.tbNguoiDungs.FirstOrDefault(x => x.TenNguoiDung == "Phan Thị Tường Vân").IdNguoiDung,
                            },
                            TenCapDo_DoanhThu = "NS-3"
                        },
                        new tool {
                            NguoiDung_DoanhThu = new tbNguoiDung_DoanhThu {
                                IdNguoiDung_DoanhThu = Guid.NewGuid(),
                                DoanhThuMucTieu = 710831500,
                                DoanhThuThucTe = 710831500,
                                PhanTramHoanThien = 100,
                                NgayLenMucTieu = "08/2024",
                                NgayDatMucTieu = DateTime.Parse("2024-08-14 20:53:08.533"),
                                TrangThai = 1,
                                MaDonViSuDung = Guid.Parse("f4b89d3a-5246-4d90-83c2-2db9c3e4d9b7"),
                                NgayTao =  DateTime.Parse("2024-08-10 20:53:08.533"),
                                IdNguoiTao = db.tbNguoiDungs.FirstOrDefault(x => x.TenNguoiDung == "Huỳnh Thị Lan Thanh").IdNguoiDung,
                            },
                            TenCapDo_DoanhThu = "NS-3"
                        },
                        new tool {
                            NguoiDung_DoanhThu = new tbNguoiDung_DoanhThu {
                                IdNguoiDung_DoanhThu = Guid.NewGuid(),
                                DoanhThuMucTieu = 73850155,
                                DoanhThuThucTe = 73850155,
                                PhanTramHoanThien = 100,
                                NgayLenMucTieu = "08/2024",
                                NgayDatMucTieu = DateTime.Parse("2024-08-14 20:53:08.533"),
                                TrangThai = 1,
                                MaDonViSuDung = Guid.Parse("f4b89d3a-5246-4d90-83c2-2db9c3e4d9b7"),
                                NgayTao =  DateTime.Parse("2024-08-10 20:53:08.533"),
                                IdNguoiTao = db.tbNguoiDungs.FirstOrDefault(x => x.TenNguoiDung == "Lê Thị Tố Uyên").IdNguoiDung,
                            },
                            TenCapDo_DoanhThu = "Thử việc"
                        },
                        new tool {
                            NguoiDung_DoanhThu = new tbNguoiDung_DoanhThu {
                                IdNguoiDung_DoanhThu = Guid.NewGuid(),
                                DoanhThuMucTieu = 330256087,
                                DoanhThuThucTe = 330256087,
                                PhanTramHoanThien = 100,
                                NgayLenMucTieu = "08/2024",
                                NgayDatMucTieu = DateTime.Parse("2024-08-14 20:53:08.533"),
                                TrangThai = 1,
                                MaDonViSuDung = Guid.Parse("f4b89d3a-5246-4d90-83c2-2db9c3e4d9b7"),
                                NgayTao =  DateTime.Parse("2024-08-10 20:53:08.533"),
                                IdNguoiTao = db.tbNguoiDungs.FirstOrDefault(x => x.TenNguoiDung == "Lê Thị Huyền Trang").IdNguoiDung,
                            },
                            TenCapDo_DoanhThu = "NS-2"
                        },
                        new tool {
                            NguoiDung_DoanhThu = new tbNguoiDung_DoanhThu {
                                IdNguoiDung_DoanhThu = Guid.NewGuid(),
                                DoanhThuMucTieu = 97369750,
                                DoanhThuThucTe = 97369750,
                                PhanTramHoanThien = 100,
                                NgayLenMucTieu = "08/2024",
                                NgayDatMucTieu = DateTime.Parse("2024-08-14 20:53:08.533"),
                                TrangThai = 1,
                                MaDonViSuDung = Guid.Parse("f4b89d3a-5246-4d90-83c2-2db9c3e4d9b7"),
                                NgayTao =  DateTime.Parse("2024-08-10 20:53:08.533"),
                                IdNguoiTao = db.tbNguoiDungs.FirstOrDefault(x => x.TenNguoiDung == "Phạm Thu Huyền").IdNguoiDung,
                            },
                            TenCapDo_DoanhThu = "NS-1"
                        },
                        new tool {
                            NguoiDung_DoanhThu = new tbNguoiDung_DoanhThu {
                                IdNguoiDung_DoanhThu = Guid.NewGuid(),
                                DoanhThuMucTieu = 9877000,
                                DoanhThuThucTe = 9877000,
                                PhanTramHoanThien = 100,
                                NgayLenMucTieu = "08/2024",
                                NgayDatMucTieu = DateTime.Parse("2024-08-14 20:53:08.533"),
                                TrangThai = 1,
                                MaDonViSuDung = Guid.Parse("f4b89d3a-5246-4d90-83c2-2db9c3e4d9b7"),
                                NgayTao =  DateTime.Parse("2024-08-10 20:53:08.533"),
                                IdNguoiTao = db.tbNguoiDungs.FirstOrDefault(x => x.TenNguoiDung == "Tô Huỳnh Kim Hoa").IdNguoiDung,
                            },
                            TenCapDo_DoanhThu = "Thử việc"
                        },
                        new tool {
                            NguoiDung_DoanhThu = new tbNguoiDung_DoanhThu {
                                IdNguoiDung_DoanhThu = Guid.NewGuid(),
                                DoanhThuMucTieu = 87013500,
                                DoanhThuThucTe = 87013500,
                                PhanTramHoanThien = 100,
                                NgayLenMucTieu = "08/2024",
                                NgayDatMucTieu = DateTime.Parse("2024-08-14 20:53:08.533"),
                                TrangThai = 1,
                                MaDonViSuDung = Guid.Parse("f4b89d3a-5246-4d90-83c2-2db9c3e4d9b7"),
                                NgayTao =  DateTime.Parse("2024-08-10 20:53:08.533"),
                                IdNguoiTao = db.tbNguoiDungs.FirstOrDefault(x => x.TenNguoiDung == "Nguyễn Phúc Ngân").IdNguoiDung,
                            },
                            TenCapDo_DoanhThu = "NS-1"
                        },
                        new tool {
                            NguoiDung_DoanhThu = new tbNguoiDung_DoanhThu {
                                IdNguoiDung_DoanhThu = Guid.NewGuid(),
                                DoanhThuMucTieu = 110934000,
                                DoanhThuThucTe = 110934000,
                                PhanTramHoanThien = 100,
                                NgayLenMucTieu = "08/2024",
                                NgayDatMucTieu = DateTime.Parse("2024-08-14 20:53:08.533"),
                                TrangThai = 1,
                                MaDonViSuDung = Guid.Parse("f4b89d3a-5246-4d90-83c2-2db9c3e4d9b7"),
                                NgayTao =  DateTime.Parse("2024-08-10 20:53:08.533"),
                                IdNguoiTao = db.tbNguoiDungs.FirstOrDefault(x => x.TenNguoiDung == "Lê Ngọc Vân Anh").IdNguoiDung,
                            },
                            TenCapDo_DoanhThu = "NS-1"
                        },
                        new tool {
                            NguoiDung_DoanhThu = new tbNguoiDung_DoanhThu {
                                IdNguoiDung_DoanhThu = Guid.NewGuid(),
                                DoanhThuMucTieu = 65750125,
                                DoanhThuThucTe = 65750125,
                                PhanTramHoanThien = 100,
                                NgayLenMucTieu = "08/2024",
                                NgayDatMucTieu = DateTime.Parse("2024-08-14 20:53:08.533"),
                                TrangThai = 1,
                                MaDonViSuDung = Guid.Parse("f4b89d3a-5246-4d90-83c2-2db9c3e4d9b7"),
                                NgayTao =  DateTime.Parse("2024-08-10 20:53:08.533"),
                                IdNguoiTao = db.tbNguoiDungs.FirstOrDefault(x => x.TenNguoiDung == "Chu Thị Thanh").IdNguoiDung,
                            },
                            TenCapDo_DoanhThu = "Thử việc"
                        },
                        new tool {
                            NguoiDung_DoanhThu = new tbNguoiDung_DoanhThu {
                                IdNguoiDung_DoanhThu = Guid.NewGuid(),
                                DoanhThuMucTieu = 688821436,
                                DoanhThuThucTe = 688821436,
                                PhanTramHoanThien = 100,
                                NgayLenMucTieu = "08/2024",
                                NgayDatMucTieu = DateTime.Parse("2024-08-14 20:53:08.533"),
                                TrangThai = 1,
                                MaDonViSuDung = Guid.Parse("f4b89d3a-5246-4d90-83c2-2db9c3e4d9b7"),
                                NgayTao =  DateTime.Parse("2024-08-10 20:53:08.533"),
                                IdNguoiTao = db.tbNguoiDungs.FirstOrDefault(x => x.TenNguoiDung == "Nguyễn Hồng Cẩm Nhung").IdNguoiDung,
                            },
                            TenCapDo_DoanhThu = "NS-3"
                        },
                        new tool {
                            NguoiDung_DoanhThu = new tbNguoiDung_DoanhThu {
                                IdNguoiDung_DoanhThu = Guid.NewGuid(),
                                DoanhThuMucTieu = 1938473013,
                                DoanhThuThucTe = 1938473013,
                                PhanTramHoanThien = 100,
                                NgayLenMucTieu = "08/2024",
                                NgayDatMucTieu = DateTime.Parse("2024-08-14 20:53:08.533"),
                                TrangThai = 1,
                                MaDonViSuDung = Guid.Parse("f4b89d3a-5246-4d90-83c2-2db9c3e4d9b7"),
                                NgayTao =  DateTime.Parse("2024-08-10 20:53:08.533"),
                                IdNguoiTao = db.tbNguoiDungs.FirstOrDefault(x => x.TenNguoiDung == "Nguyễn Thị Thanh Thảo").IdNguoiDung,
                            },
                            TenCapDo_DoanhThu = "NS-4"
                        },
                        new tool {
                            NguoiDung_DoanhThu = new tbNguoiDung_DoanhThu {
                                IdNguoiDung_DoanhThu = Guid.NewGuid(),
                                DoanhThuMucTieu = 0,
                                DoanhThuThucTe = 0,
                                PhanTramHoanThien = 0,
                                NgayLenMucTieu = "08/2024",
                                NgayDatMucTieu = DateTime.Parse("2024-08-14 20:53:08.533"),
                                TrangThai = 1,
                                MaDonViSuDung = Guid.Parse("f4b89d3a-5246-4d90-83c2-2db9c3e4d9b7"),
                                NgayTao =  DateTime.Parse("2024-08-10 20:53:08.533"),
                                IdNguoiTao = db.tbNguoiDungs.FirstOrDefault(x => x.TenNguoiDung == "Trần Thị Hồng Phượng").IdNguoiDung,
                            },
                            TenCapDo_DoanhThu = "Thử việc"
                        },
                        new tool {
                            NguoiDung_DoanhThu = new tbNguoiDung_DoanhThu {
                                IdNguoiDung_DoanhThu = Guid.NewGuid(),
                                DoanhThuMucTieu = 0,
                                DoanhThuThucTe = 0,
                                PhanTramHoanThien = 0,
                                NgayLenMucTieu = "08/2024",
                                NgayDatMucTieu = DateTime.Parse("2024-08-14 20:53:08.533"),
                                TrangThai = 1,
                                MaDonViSuDung = Guid.Parse("f4b89d3a-5246-4d90-83c2-2db9c3e4d9b7"),
                                NgayTao =  DateTime.Parse("2024-08-10 20:53:08.533"),
                                IdNguoiTao = db.tbNguoiDungs.FirstOrDefault(x => x.TenNguoiDung == "Dương Ngọc Linh").IdNguoiDung,
                            },
                            TenCapDo_DoanhThu = "Thử việc"
                        },
                        new tool {
                            NguoiDung_DoanhThu = new tbNguoiDung_DoanhThu {
                                IdNguoiDung_DoanhThu = Guid.NewGuid(),
                                DoanhThuMucTieu = 0,
                                DoanhThuThucTe = 0,
                                PhanTramHoanThien = 0,
                                NgayLenMucTieu = "08/2024",
                                NgayDatMucTieu = DateTime.Parse("2024-08-14 20:53:08.533"),
                                TrangThai = 1,
                                MaDonViSuDung = Guid.Parse("f4b89d3a-5246-4d90-83c2-2db9c3e4d9b7"),
                                NgayTao =  DateTime.Parse("2024-08-10 20:53:08.533"),
                                IdNguoiTao = db.tbNguoiDungs.FirstOrDefault(x => x.TenNguoiDung == "Đỗ Trần Tú Văn").IdNguoiDung,
                            },
                            TenCapDo_DoanhThu = "Thử việc"
                        },
                    };
                    foreach(tool nguoiDung in nguoiDungs)
                    {
                        db.tbNguoiDung_DoanhThu.Add(nguoiDung.NguoiDung_DoanhThu);
                        tbNguoiDung nguoiDung_ = db.tbNguoiDungs.FirstOrDefault(x => x.IdNguoiDung == nguoiDung.NguoiDung_DoanhThu.IdNguoiTao);
                        nguoiDung_.IdCapDo_DoanhThu = db.tbCapDo_DoanhThu.FirstOrDefault(x => x.TenCapDo_DoanhThu == nguoiDung.TenCapDo_DoanhThu).IdCapDo_DoanhThu;
                    };
                    db.SaveChanges();
                    //scope.Commit();
                }
                catch (Exception ex)
                {
                    status = "error";
                    mess = ex.Message;
                    scope.Rollback();
                }

                return Json(new
                {
                    status,
                    mess
                }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult nguoiDungSinhNhats()
        {
            var nds = db.tbNguoiDungs.Where(x=> x.TrangThai != 0 
            && x.MaDonViSuDung == per.DonViSuDung.MaDonViSuDung
            && x.KichHoat == true).ToList();
            var cvs = db.default_tbChucVu.Where(x => x.TrangThai != 0).ToList();
            var nguoiDungs = (from nd in nds
                              //join cv in cvs on nd.IdChucVu equals cv.IdChucVu
                              where nd.NgaySinh.HasValue && nd.NgaySinh.Value.ToString("dd/MM") == DateTime.Now.ToString("dd/MM")
                              select nd).ToList();
            var model = nguoiDungs;
            string viewAsString = Public.Handle.RenderViewToString(this, $"{VIEW_PATH}/quyenkinhdoanh/chucmungsinhnhat.cshtml", model);
            return Json(new
            {
                view = viewAsString,
                model
            }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Client - Tai lieu

        #endregion
    }
}
class tool
{
    public tbNguoiDung_DoanhThu NguoiDung_DoanhThu { get; set; } = new tbNguoiDung_DoanhThu();
    public string TenCapDo_DoanhThu { get; set; } = string.Empty;
}