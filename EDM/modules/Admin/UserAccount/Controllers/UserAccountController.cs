using ClosedXML.Excel;
using EDM_DB;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Organization.Controllers;
using Public.Controllers;
using Public.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using UserAccount.Models;
using UserType.Models;
using System.Net.Http;
using System.Data.Entity.Core.Objects;
using System.Management;
using ObjectQuery = System.Management.ObjectQuery;
using System.Runtime.InteropServices;

namespace UserAccount.Controllers
{
    public class UserAccountController : RouteConfigController
    {
        #region Biến public để in hoa
        private readonly string VIEW_PATH = "~/Views/Admin/_SystemSetting/UserAccount";
        private List<tbKieuNguoiDung> KIEUNGUOIDUNGs
        {
            get
            {
                return Session["KIEUNGUOIDUNGs"] as List<tbKieuNguoiDung> ?? new List<tbKieuNguoiDung>();
            }
            set
            {
                Session["KIEUNGUOIDUNGs"] = value;
            }
        }
        private List<tbCoCauToChuc> COCAUTOCHUCs
        {
            get
            {
                return Session["COCAUTOCHUCs"] as List<tbCoCauToChuc> ?? new List<tbCoCauToChuc>();
            }
            set
            {
                Session["COCAUTOCHUCs"] = value;
            }
        }
        private List<default_tbChucVu> CHUCVUs
        {
            get
            {
                return Session["CHUCVUs"] as List<default_tbChucVu> ?? new List<default_tbChucVu>();
            }
            set
            {
                Session["CHUCVUs"] = value;
            }
        }
        private List<Tree<tbCoCauToChuc>> COCAUTOCHUCs_TREE
        {
            get
            {
                return Session["COCAUTOCHUCs_TREE"] as List<Tree<tbCoCauToChuc>> ?? new List<Tree<tbCoCauToChuc>>();
            }
            set
            {
                Session["COCAUTOCHUCs_TREE"] = value;
            }
        }
        private string HTMLCOCAUS
        {
            get
            {
                return Session["HTMLCOCAUS"] as string ?? string.Empty;
            }
            set
            {
                Session["HTMLCOCAUS"] = value;
            }
        }
        private List<tbNguoiDungExtend> EXCEL_NGUOIDUNGs_UPLOAD
        {
            get
            {
                return Session["EXCEL_NGUOIDUNGs_UPLOAD"] as List<tbNguoiDungExtend> ?? new List<tbNguoiDungExtend>();
            }
            set
            {
                Session["EXCEL_NGUOIDUNGs_UPLOAD"] = value;
            }
        }
        private List<tbNguoiDungExtend> EXCEL_NGUOIDUNGs_DOWNLOAD
        {
            get
            {
                return Session["EXCEL_NGUOIDUNGs_DOWNLOAD"] as List<tbNguoiDungExtend> ?? new List<tbNguoiDungExtend>();
            }
            set
            {
                Session["EXCEL_NGUOIDUNGs_DOWNLOAD"] = value;
            }
        }
        private List<ThongTinThietBiLuuTru> THIETBILUUTRUs
        {
            get
            {
                return Session["THIETBILUUTRUs"] as List<ThongTinThietBiLuuTru> ?? new List<ThongTinThietBiLuuTru>();
            }
            set
            {
                Session["THIETBILUUTRUs"] = value;
            }
        }
        private OrganizationController organizationController = new OrganizationController();
        #endregion
        public ActionResult Index()
        {
            #region Lấy các danh sách

            #region Kiểu người dùng
            List<tbKieuNguoiDung> kieuNguoiDungs = db.tbKieuNguoiDungs.Where(x => x.MaDonViSuDung == per.DonViSuDung.MaDonViSuDung && x.TrangThai != 0).ToList() ?? new List<tbKieuNguoiDung>();
            #endregion

            #region Cơ cấu tổ chức
            List<tbCoCauToChuc> coCauToChucs = new List<tbCoCauToChuc>();
            List<Tree<tbCoCauToChuc>> coCauToChucs_Tree = organizationController.get_CoCauToChucs_Tree(idCoCau: Guid.Empty, maDonViSuDung: per.DonViSuDung.MaDonViSuDung).nodes;
            organizationController.xuLy_TenCoCauToChuc(coCaus_IN: coCauToChucs_Tree, coCaus_OUT: coCauToChucs);
            #endregion

            #region Chức vụ
            List<default_tbChucVu> chucVus = db.default_tbChucVu.Where(x => x.TrangThai != 0).ToList() ?? new List<default_tbChucVu>();
            #endregion

            #region Thao tác
            List<ChucNangs> kieuNguoiDung_IdChucNang = JsonConvert.DeserializeObject<List<ChucNangs>>(per.KieuNguoiDung.IdChucNang);
            List<ThaoTac> thaoTacs = kieuNguoiDung_IdChucNang.FirstOrDefault(x => x.ChucNang.MaChucNang == "UserAccount").ThaoTacs;
            #endregion

            #endregion
            ViewBag.thaoTacs = thaoTacs;
            KIEUNGUOIDUNGs = kieuNguoiDungs;
            COCAUTOCHUCs = coCauToChucs; COCAUTOCHUCs_TREE = coCauToChucs_Tree;
            CHUCVUs = chucVus;
            return View($"{VIEW_PATH}/useraccount.cshtml");
        }
        [HttpGet]
        public JsonResult getList()
        {
            List<tbNguoiDungExtend> nguoiDungs = get_NguoiDungs(loai: "all");
            return Json(new
            {
                data = nguoiDungs
            }, JsonRequestBehavior.AllowGet);
        }
        private void downloadDialog(MemoryStream data, string fileName, string contentType)
        {
            Response.Buffer = true;
            Response.Clear();
            Response.ContentType = contentType;
            Response.AddHeader("content-disposition", "inline; filename=\"" + fileName + "\"");
            Response.BinaryWrite(data.ToArray());
            Response.Flush();
            Response.End();
        }

        #region Lấy danh sách dữ liệu
        public List<tbNguoiDungExtend> get_NguoiDungs(
            string loai,
            //List<Guid> idNguoiDungs = null
            string str_idNguoiDungs = "")
        {
            //var nguoiDungRepo = db.tbNguoiDungs.Where(x =>
            //x.TrangThai != 0 && x.MaDonViSuDung == per.DonViSuDung.MaDonViSuDung).OrderByDescending(x => x.Stt).ToList();
            var nguoiDungs = db.Database.SqlQuery<tbNguoiDungExtend>($@"
            select nguoiDung.*
            from tbNguoiDung nguoiDung
            where nguoiDung.TrangThai != 0 and nguoiDung.MaDonViSuDung = '{per.DonViSuDung.MaDonViSuDung}'
            {(loai != "single" ? "" : $"and nguoiDung.IdNguoiDung in ({str_idNguoiDungs}) ")}
            ").ToList() ?? new List<tbNguoiDungExtend>();

            foreach (tbNguoiDungExtend nguoiDung in nguoiDungs)
            {
                nguoiDung.KieuNguoiDung = KIEUNGUOIDUNGs.FirstOrDefault(x => x.IdKieuNguoiDung == nguoiDung.IdKieuNguoiDung) ?? new tbKieuNguoiDung();
                nguoiDung.CoCauToChuc = COCAUTOCHUCs.FirstOrDefault(x => x.IdCoCauToChuc == nguoiDung.IdCoCauToChuc) ?? new tbCoCauToChuc();
                nguoiDung.ChucVu = CHUCVUs.FirstOrDefault(x => x.IdChucVu == nguoiDung.IdChucVu) ?? new default_tbChucVu();
            };
            return nguoiDungs;
        }
        #endregion

        #region CRUD
        [HttpPost]
        public bool capNhatNguoiDungHoatDong(Guid idNguoiDung)
        {
            try
            {
                var nguoiDung = db.tbNguoiDungs.Find(idNguoiDung);
                if (nguoiDung != null)
                {
                    nguoiDung.Online = true;
                };
                db.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            };
        }
        [HttpPost]
        public ActionResult displayModal_CRUD(string loai, Guid idNguoiDung)
        {
            tbNguoiDungExtend nguoiDung = new tbNguoiDungExtend();
            if (loai != "create" && idNguoiDung != Guid.Empty)
                nguoiDung = get_NguoiDungs(loai: "single", str_idNguoiDungs: string.Format("'{0}'", idNguoiDung)).FirstOrDefault();
            ViewBag.nguoiDung = nguoiDung;
            ViewBag.loai = loai;
            return PartialView($"{VIEW_PATH}/useraccount-crud.cshtml");
        }
        [HttpPost]
        public ActionResult displayModal_Delete()
        {
            List<tbNguoiDungExtend> nguoiDungs = get_NguoiDungs(loai: "all");
            string str_idNguoiDungs_XOA = Request.Form["str_idNguoiDungs_XOA"];
            ViewBag.nguoiDungs = nguoiDungs;
            ViewBag.idNguoiDungs_XOA = JsonConvert.DeserializeObject<List<Guid>>(str_idNguoiDungs_XOA);
            return PartialView($"{VIEW_PATH}/useraccount-delete.cshtml");
        }
        public bool kiemTra_NguoiDung(tbNguoiDung nguoiDung)
        {
            // Kiểm tra còn hồ sơ khác có trùng mã không
            tbNguoiDung nguoiDung_OLD = db.tbNguoiDungs.FirstOrDefault(x => x.TenDangNhap == nguoiDung.TenDangNhap
            && x.IdNguoiDung != nguoiDung.IdNguoiDung
            && x.TrangThai != 0 && x.MaDonViSuDung == per.DonViSuDung.MaDonViSuDung);
            if (nguoiDung_OLD == null) return false;
            return true;
        }
        [HttpPost]
        public ActionResult create_NguoiDung(string str_nguoiDung)
        {
            string status = "success";
            string mess = "Thêm mới bản ghi thành công";
            using (var scope = db.Database.BeginTransaction())
            {
                try
                {
                    string format = "dd/MM/yyyy";
                    IsoDateTimeConverter dateTimeConverter = new IsoDateTimeConverter { DateTimeFormat = format };
                    tbNguoiDungExtend nguoiDung_NEW = JsonConvert.DeserializeObject<tbNguoiDungExtend>(str_nguoiDung ?? "", dateTimeConverter);
                    if (nguoiDung_NEW == null)
                    {
                        status = "error";
                        mess = "Chưa có bản ghi nào";
                    }
                    else
                    {
                        nguoiDung_NEW.TenDangNhap = taoTenDangNhap(tenDangNhap: nguoiDung_NEW.TenDangNhap);
                        if (kiemTra_NguoiDung(nguoiDung: nguoiDung_NEW))
                        {
                            status = "datontai";
                            mess = "Tên đăng nhập đã tồn tại";
                        }
                        else
                        {
                            string matKhau = Public.Handle.HashToMD5(nguoiDung_NEW.MatKhau);
                            // Tạo hồ sơ
                            tbNguoiDung nguoiDung = new tbNguoiDung
                            {
                                IdNguoiDung = Guid.NewGuid(),
                                TenDangNhap = nguoiDung_NEW.TenDangNhap,
                                MatKhau = matKhau,
                                TenNguoiDung = nguoiDung_NEW.TenNguoiDung,
                                GioiTinh = nguoiDung_NEW.GioiTinh,
                                KichHoat = nguoiDung_NEW.KichHoat,
                                SoLanDangNhap = 0,
                                YeuCauDoiMatKhau = true,
                                Online = false,
                                Email = nguoiDung_NEW.Email,
                                SoDienThoai = nguoiDung_NEW.SoDienThoai,
                                SoTaiKhoanNganHang = nguoiDung_NEW.SoTaiKhoanNganHang,
                                NgaySinh = nguoiDung_NEW.NgaySinh,
                                GhiChu = nguoiDung_NEW.GhiChu,
                                LinkLienHe = nguoiDung_NEW.LinkLienHe,

                                IdKieuNguoiDung = nguoiDung_NEW.IdKieuNguoiDung,
                                IdCoCauToChuc = nguoiDung_NEW.IdCoCauToChuc,
                                IdChucVu = nguoiDung_NEW.IdChucVu,

                                TrangThai = 1,
                                IdNguoiTao = per.NguoiDung.IdNguoiDung,
                                NgayTao = DateTime.Now,
                                MaDonViSuDung = per.DonViSuDung.MaDonViSuDung
                            };
                            db.tbNguoiDungs.Add(nguoiDung);

                            db.SaveChanges();
                            // Gửi mail
                            GuiMai(nguoiDung: nguoiDung, nguoiDung_NEW: nguoiDung_NEW);

                            scope.Commit();
                        };
                    };
                }
                catch (Exception ex)
                {
                    status = "error";
                    mess = ex.Message;
                    scope.Rollback();
                }
            }
            return Json(new
            {
                status,
                mess
            }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult update_NguoiDung(string str_nguoiDung)
        {
            string status = "success";
            string mess = "Cập nhật bản ghi thành công";

            using (var scope = db.Database.BeginTransaction())
            {
                try
                {
                    string format = "dd/MM/yyyy";
                    IsoDateTimeConverter dateTimeConverter = new IsoDateTimeConverter { DateTimeFormat = format };
                    tbNguoiDungExtend nguoiDung_NEW = JsonConvert.DeserializeObject<tbNguoiDungExtend>(str_nguoiDung ?? "", dateTimeConverter);
                    if (nguoiDung_NEW == null)
                    {
                        status = "error";
                        mess = "Chưa có bản ghi nào";
                    }
                    else
                    {
                        nguoiDung_NEW.TenDangNhap = taoTenDangNhap(tenDangNhap: nguoiDung_NEW.TenDangNhap);
                        if (kiemTra_NguoiDung(nguoiDung: nguoiDung_NEW))
                        {
                            status = "datontai";
                            mess = "Tên đăng nhập đã tồn tại";
                        }
                        else
                        {
                            tbNguoiDungExtend nguoiDung_OLD = db.Database.SqlQuery<tbNguoiDungExtend>($@"select * from tbNguoiDung where MaDonViSuDung = '{per.DonViSuDung.MaDonViSuDung}' and IdNguoiDung = '{nguoiDung_NEW.IdNguoiDung}'").FirstOrDefault();
                            if (nguoiDung_NEW.IdKieuNguoiDung != null || nguoiDung_NEW.IdKieuNguoiDung != Guid.Empty)
                            {
                                nguoiDung_NEW.KieuNguoiDung = db.tbKieuNguoiDungs.FirstOrDefault(x => x.IdKieuNguoiDung == nguoiDung_NEW.IdKieuNguoiDung) ?? new tbKieuNguoiDung();
                                nguoiDung_OLD.KieuNguoiDung = db.tbKieuNguoiDungs.FirstOrDefault(x => x.IdKieuNguoiDung == nguoiDung_OLD.IdKieuNguoiDung) ?? new tbKieuNguoiDung();

                            };
                            if (nguoiDung_NEW.IdCoCauToChuc != null || nguoiDung_NEW.IdCoCauToChuc != Guid.Empty)
                            {
                                nguoiDung_NEW.CoCauToChuc = db.tbCoCauToChucs.FirstOrDefault(x => x.IdCoCauToChuc == nguoiDung_NEW.IdCoCauToChuc) ?? new tbCoCauToChuc();
                                nguoiDung_OLD.CoCauToChuc = db.tbCoCauToChucs.FirstOrDefault(x => x.IdCoCauToChuc == nguoiDung_OLD.IdCoCauToChuc) ?? new tbCoCauToChuc();
                            };
                            if (nguoiDung_NEW.IdChucVu != null || nguoiDung_NEW.IdChucVu != Guid.Empty)
                            {
                                nguoiDung_NEW.ChucVu = db.default_tbChucVu.FirstOrDefault(x => x.IdChucVu == nguoiDung_NEW.IdChucVu) ?? new default_tbChucVu();
                                nguoiDung_OLD.ChucVu = db.default_tbChucVu.FirstOrDefault(x => x.IdChucVu == nguoiDung_OLD.IdChucVu) ?? new default_tbChucVu();
                            };
                            string sql_capNhatNguoiDung = $@"
                            update tbNguoiDung set
                                TenDangNhap = '{nguoiDung_NEW.TenDangNhap}',
                                TenNguoiDung = N'{nguoiDung_NEW.TenNguoiDung}',
                                GioiTinh = {(nguoiDung_NEW.GioiTinh.Value ? 1 : 0)},
                                KichHoat = {(nguoiDung_NEW.KichHoat.Value ? 1 : 0)},
                                Email = '{nguoiDung_NEW.Email}',
                                SoDienThoai = '{nguoiDung_NEW.SoDienThoai}',
                                SoTaiKhoanNganHang = '{nguoiDung_NEW.SoTaiKhoanNganHang}',
                                NgaySinh = '{nguoiDung_NEW.NgaySinh}',
                                GhiChu = N'{nguoiDung_NEW.GhiChu}',
                                LinkLienHe = '{nguoiDung_NEW.LinkLienHe}',
                                IdChucVu = '{nguoiDung_NEW.IdChucVu}',
                                IdKieuNguoiDung = '{nguoiDung_NEW.IdKieuNguoiDung}',
                                IdCoCauToChuc = '{nguoiDung_NEW.IdCoCauToChuc}',

                                IdNguoiSua = '{per.NguoiDung.IdNguoiDung}',
                                NgaySua = '{DateTime.Now}'
                            where MaDonViSuDung = '{per.DonViSuDung.MaDonViSuDung}' and IdNguoiDung = '{nguoiDung_NEW.IdNguoiDung}'
                            ";
                            db.Database.ExecuteSqlCommand(sql_capNhatNguoiDung);
                            // Cập nhật lại session
                            if (nguoiDung_OLD.IdNguoiDung == per.NguoiDung.IdNguoiDung)
                            {
                                status = "logout";
                                mess = "[Tài khoản đang sử dụng]";
                                per.NguoiDung = nguoiDung_OLD;
                            };

                            db.SaveChanges();
                            #region Gửi mail cập nhật
                            string mail()
                            {
                                var model = new CapNhatTaiKhoanMailM<tbNguoiDungExtend>
                                {
                                    NguoiDung_OLD = nguoiDung_OLD,
                                    NguoiDung_NEW = nguoiDung_NEW,
                                    DonViSuDung = per.DonViSuDung
                                };
                                // Gọi phương thức RenderViewToString() để chuyển đổi view thành chuỗi
                                string viewAsString = Public.Handle.RenderViewToString(this, $"{VIEW_PATH}/useraccount-mail.capnhattaikhoan.cshtml", model);
                                // Trả về chuỗi đã được tạo ra từ view
                                return viewAsString;
                            }
                            string tieuDeMail = "[📣 GIAPTECH] - CẬP NHẬT THÔNG TIN TÀI KHOẢN❗";
                            string mailBody = mail();
                            // Gửi mail
                            Public.Handle.SendEmail(sendTo: nguoiDung_OLD.Email, subject: tieuDeMail, body: mailBody, isHTML: true, donViSuDung: per.DonViSuDung);
                            if (nguoiDung_NEW.Email != nguoiDung_OLD.Email)
                            {
                                Public.Handle.SendEmail(sendTo: nguoiDung_NEW.Email, subject: tieuDeMail, body: mailBody, isHTML: true, donViSuDung: per.DonViSuDung);
                            };
                            #endregion

                            scope.Commit();
                        };
                    }
                }
                catch (Exception ex)
                {
                    status = "error";
                    mess = ex.Message;
                    scope.Rollback();
                }
            }
            return Json(new
            {
                status,
                mess
            }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult capNhat_MatKhau(string str_nguoiDung)
        {
            string status = "success";
            string mess = "Cập nhật bản ghi thành công";

            using (var scope = db.Database.BeginTransaction())
            {
                try
                {
                    string format = "dd/MM/yyyy";
                    IsoDateTimeConverter dateTimeConverter = new IsoDateTimeConverter { DateTimeFormat = format };
                    tbNguoiDungExtend nguoiDung_NEW = JsonConvert.DeserializeObject<tbNguoiDungExtend>(str_nguoiDung ?? "", dateTimeConverter);
                    if (nguoiDung_NEW == null)
                    {
                        status = "error";
                        mess = "Chưa có bản ghi nào";
                    }
                    else
                    {
                        tbNguoiDung nguoiDung_OLD = db.tbNguoiDungs.FirstOrDefault(x => x.MaDonViSuDung == per.DonViSuDung.MaDonViSuDung && x.IdNguoiDung == nguoiDung_NEW.IdNguoiDung);
                        string matKhau_MD5 = Public.Handle.HashToMD5(nguoiDung_NEW.MatKhauMoi);
                        if (nguoiDung_OLD.MatKhau != matKhau_MD5)
                        {
                            status = "matkhaucuchuachinhxac";
                            mess = "Mật khẩu cũ chưa chính xác";
                        }
                        else
                        {
                            // Kiểm tra độ bảo mật
                            var conditions = Public.Handle.CheckPassPattern(nguoiDung_NEW.MatKhauMoi);
                            // Kiểm tra từng điều kiện
                            foreach (var condition in conditions)
                            {
                                if (!condition.Value.status) return Json(new { status = "warning", mess = condition.Value.error });
                            };

                            //if (nguoiDung_NEW.MatKhauMoi != nguoiDung_NEW.MatKhauMoi) return Json(new { mess = "Mật khẩu xác nhận chưa trùng khớp" });

                            nguoiDung_OLD.MatKhau = matKhau_MD5;
                            // Cập nhật lại session
                            if (nguoiDung_OLD.IdNguoiDung == per.NguoiDung.IdNguoiDung)
                            {
                                status = "logout";
                                mess = "[Tài khoản đang sử dụng]";
                                per.NguoiDung = nguoiDung_OLD;
                            };

                            db.SaveChanges();
                            #region Gửi mail
                            string mail()
                            {
                                var model = new CapNhatTaiKhoanMailM<tbNguoiDungExtend>
                                {
                                    NguoiDung_NEW = nguoiDung_NEW,
                                    DonViSuDung = per.DonViSuDung,
                                    HinhThucCapNhat = "doimatkhau"
                                };
                                // Gọi phương thức RenderViewToString() để chuyển đổi view thành chuỗi
                                string viewAsString = Public.Handle.RenderViewToString(this, $"{VIEW_PATH}/useraccount-mail.capnhattaikhoan.cshtml", model);
                                // Trả về chuỗi đã được tạo ra từ view
                                return viewAsString;
                            }
                            string tieuDeMail = "[📣 GIAPTECH] - CẬP NHẬT THÔNG TIN TÀI KHOẢN❗";
                            string mailBody = mail();
                            // Gửi mail
                            Public.Handle.SendEmail(sendTo: nguoiDung_OLD.Email, subject: tieuDeMail, body: mailBody, isHTML: true, donViSuDung: per.DonViSuDung);
                            if (nguoiDung_NEW.Email != nguoiDung_OLD.Email)
                            {
                                Public.Handle.SendEmail(sendTo: nguoiDung_NEW.Email, subject: tieuDeMail, body: mailBody, isHTML: true, donViSuDung: per.DonViSuDung);
                            };
                            #endregion
                            scope.Commit();
                        };
                    };
                }
                catch (Exception ex)
                {
                    status = "error";
                    mess = ex.Message;
                    scope.Rollback();
                }
            }
            return Json(new
            {
                status,
                mess
            }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult delete_NguoiDungs()
        {
            string status = "success";
            string mess = "Xóa bản ghi thành công";
            using (var scope = db.Database.BeginTransaction())
            {
                try
                {
                    List<Guid> idNguoiDungs_XOA = JsonConvert.DeserializeObject<List<Guid>>(Request.Form["str_idNguoiDungs_XOA"]);
                    Guid idNguoiDung_THAYTHE = Guid.Parse(Request.Form["idNguoiDung_THAYTHE"]);
                    if (idNguoiDungs_XOA.Count > 0)
                    {
                        string str_idNguoiDungs_XOA = string.Join(",", idNguoiDungs_XOA.Select(x => string.Format("'{0}'", x)));
                        string capNhatSQL = $@"
                        -- Xóa người dùng
                        update tbNguoiDung 
                        set 
                            TrangThai = 0 , IdNguoiSua = '{per.NguoiDung.IdNguoiDung}' , NgaySua = '{DateTime.Now}' 
                        where 
                            MaDonViSuDung = '{per.DonViSuDung.MaDonViSuDung}' and IdNguoiDung in ({str_idNguoiDungs_XOA})

                        -- Cập nhật hồ sơ
                        DECLARE @idNguoiDungs_XOA TABLE (value NVARCHAR(100))
                        -- Tách chuỗi str_idNguoiDungs_XOA
                        INSERT INTO @idNguoiDungs_XOA (value)
                        SELECT value
                        FROM STRING_SPLIT('{idNguoiDungs_XOA.ToString()}', ',')

                        UPDATE tbKhachHang
                        SET 
                        IdNguoiSua = '{per.NguoiDung.IdNguoiDung}' , NgaySua = '{DateTime.Now}',
                        QuyenTruyCap = (
                            SELECT STRING_AGG(new_id, ',')
                            FROM (
                                SELECT value AS new_id
                                FROM STRING_SPLIT(QuyenTruyCap, ',')
                                WHERE value NOT IN (SELECT value FROM @idNguoiDungs_XOA)
                                UNION
                                SELECT value
                                FROM @idNguoiDungs_XOA
                                WHERE value NOT IN (SELECT value FROM STRING_SPLIT(QuyenTruyCap, ','))
                                UNION
                                SELECT '{idNguoiDung_THAYTHE}' AS value
                            ) AS NewValues
                        )
                        WHERE (EXISTS (SELECT value FROM STRING_SPLIT(QuyenTruyCap, ',') WHERE value IN (SELECT value FROM @idNguoiDungs_XOA))
                            OR EXISTS (SELECT value FROM @idNguoiDungs_XOA WHERE value IN (SELECT value FROM STRING_SPLIT(QuyenTruyCap, ','))))
                            AND TrangThai <> 0 AND MaDonViSuDung = '{per.DonViSuDung.MaDonViSuDung}'
                        ";
                        db.Database.ExecuteSqlCommand(capNhatSQL);

                        scope.Commit();
                    }
                }
                catch (Exception ex)
                {
                    status = "error";
                    mess = ex.Message;
                    scope.Rollback();
                }
            }
            return Json(new
            {
                status,
                mess
            }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Excel
        public tbNguoiDungExtend kiemTra_Excel_NguoiDung(tbNguoiDungExtend nguoiDung)
        {
            List<string> ketQuas = new List<string>();
            /**
             * Các thông tin cần kiểm tra
             * Tên người dùng
             * Trạng thái
             * Tên đăng nhập
             * Mật khẩu
             * Giới tính
             * Email
             * Kiểu người dùng
             * Cơ cấu tổ chức
             */
            if (kiemTra_NguoiDung(nguoiDung: nguoiDung))
            {
                ketQuas.Add("Tên đăng nhập đã tồn tại");
                nguoiDung.KiemTraExcel.TrangThai = 0;
            };
            if (nguoiDung.TenNguoiDung == "" || nguoiDung.KichHoat == null || nguoiDung.TenDangNhap == "" ||
                nguoiDung.MatKhau == "" || nguoiDung.GioiTinh == null || nguoiDung.Email == "" ||
                nguoiDung.ChucVu.IdChucVu == Guid.Empty || nguoiDung.ChucVu.IdChucVu == null ||
                nguoiDung.KieuNguoiDung.IdKieuNguoiDung == Guid.Empty || nguoiDung.KieuNguoiDung.IdKieuNguoiDung == null ||
                nguoiDung.CoCauToChuc.IdCoCauToChuc == Guid.Empty || nguoiDung.CoCauToChuc.IdCoCauToChuc == null
                )
            {
                ketQuas.Add("Thiếu thông tin");
                nguoiDung.KiemTraExcel.TrangThai = 0;
            };
            nguoiDung.KiemTraExcel.KetQua = string.Join(",", ketQuas);
            return nguoiDung;
        }
        [HttpPost]
        public ActionResult upload_Excel_NguoiDung(HttpPostedFileBase[] files)
        {
            string status = "success";
            string mess = "Thêm mới tệp thành công";
            try
            {
                if (files == null)
                {
                    status = "error";
                    mess = "Tệp đính kèm sai định dạng";
                }
                else
                {
                    foreach (HttpPostedFileBase f in files)
                    {
                        #region Đọc file
                        var workBook = new XLWorkbook(f.InputStream);
                        var workSheets = workBook.Worksheets;
                        EXCEL_NGUOIDUNGs_UPLOAD = new List<tbNguoiDungExtend>();
                        foreach (var sheet in workSheets)
                        {
                            if (sheet.Name.Contains("NguoiDung"))
                            {
                                /**
                                 * Xóa bảng đang có vì nó chiếm vùng dữ liệu nhưng không đầy đủ
                                 * Bảng này chỉ chưa dữ liệu được tạo mặc định trong hàm download
                                 * (Trước đó phải chuyển đổi chuỗi tên table cho giống kiểu chuỗi tên sheet)
                                 */
                                sheet.Tables.Remove(sheet.Name.Replace(" ", String.Empty));
                                var table = sheet.RangeUsed().AsTable(); // Tạo bảng mới trên vùng dữ liệu đầy đủ
                                foreach (var row in table.DataRange.Rows())
                                {
                                    if (!row.IsEmpty())
                                    {
                                        string ngaySinh = row.Field("Ngày-sinh").GetString();
                                        DateTime NgaySinh = DateTime.Now;
                                        if (ngaySinh != null)
                                        {
                                            NgaySinh = DateTime.ParseExact(row.Field("Ngày-sinh").GetString(), Public.Handle.DATETIMEFORMAT, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None);
                                        };
                                        tbNguoiDungExtend nguoiDung = new tbNguoiDungExtend();

                                        string tenKieuNguoiDung = row.Field("Tên-kiểu-người-dùng").GetString().Trim();
                                        nguoiDung.KieuNguoiDung = KIEUNGUOIDUNGs.Where(x => x.TenKieuNguoiDung.Contains(tenKieuNguoiDung)).FirstOrDefault() ?? new tbKieuNguoiDung();

                                        string tenCoCauToChuc = row.Field("Tên-cơ-cấu-tổ-chức").GetString().Trim();
                                        nguoiDung.CoCauToChuc = COCAUTOCHUCs.Where(x => x.TenCoCauToChuc.Contains(tenCoCauToChuc)).FirstOrDefault() ?? new tbCoCauToChuc();

                                        string tenChucVu = row.Field("Tên-chức-vụ").GetString().Trim();
                                        nguoiDung.ChucVu = CHUCVUs.Where(x => x.TenChucVu.Contains(tenChucVu)).FirstOrDefault() ?? new default_tbChucVu();

                                        nguoiDung.GioiTinh = row.Field("Giới-tính").GetString().Trim() == "Nam" ? true : false;
                                        nguoiDung.KichHoat = row.Field("Kích-hoạt").GetString().Trim() == "Kích hoạt" ? true : false;
                                        nguoiDung.TenNguoiDung = row.Field("Tên-người-dùng").GetString().Trim();
                                        nguoiDung.TenDangNhap = row.Field("Tên-đăng-nhập").GetString().Trim();
                                        nguoiDung.MatKhau = row.Field("Mật-khẩu").GetString().Trim();
                                        nguoiDung.Email = row.Field("Email").GetString().Trim();
                                        nguoiDung.SoDienThoai = row.Field("Số-điện-thoại").GetString().Trim();
                                        nguoiDung.SoTaiKhoanNganHang = row.Field("Số-tài-khoản").GetString().Trim();
                                        nguoiDung.GhiChu = row.Field("Ghi-chú").GetString().Trim();
                                        nguoiDung.LinkLienHe = row.Field("Link-liên-hệ").GetString().Trim();
                                        nguoiDung.NgaySinh = NgaySinh;

                                        EXCEL_NGUOIDUNGs_UPLOAD.Add(nguoiDung);
                                    };
                                };
                            };
                        };
                        #endregion
                    };
                }
            }
            catch (Exception ex)
            {
                status = "error";
                mess = ex.Message;
            }
            return Json(new
            {
                status,
                mess
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult download_Excel_NguoiDung(string chucVu = "GV", string thoiGian = "")
        {
            Response.Clear();
            Response.ClearHeaders();
            #region Tạo excel
            using (var workBook = new XLWorkbook())
            {
                #region Tạo sheet biểu mẫu
                DataTable tbNguoiDung = new DataTable();
                tbNguoiDung.Columns.Add("Tên-người-dùng", typeof(string)); // 1
                tbNguoiDung.Columns.Add("Tên-đăng-nhập", typeof(string)); // 2
                tbNguoiDung.Columns.Add("Mật-khẩu", typeof(string)); // 3
                tbNguoiDung.Columns.Add("Email", typeof(string)); // 4
                tbNguoiDung.Columns.Add("Số-điện-thoại", typeof(string)); // 5
                tbNguoiDung.Columns.Add("Số-tài-khoản", typeof(string)); // 6
                tbNguoiDung.Columns.Add("Ngày-sinh", typeof(string)); // 7
                tbNguoiDung.Columns.Add("Link-liên-hệ", typeof(string)); // 8
                tbNguoiDung.Columns.Add("Ghi-chú", typeof(string)); // 9

                tbNguoiDung.Columns.Add("Giới-tính", typeof(string)); // 10
                tbNguoiDung.Columns.Add("Kích-hoạt", typeof(string)); // 11
                tbNguoiDung.Columns.Add("Tên-chức-vụ", typeof(string)); // 12
                tbNguoiDung.Columns.Add("Tên-kiểu-người-dùng", typeof(string)); // 13
                tbNguoiDung.Columns.Add("Tên-cơ-cấu-tổ-chức", typeof(string)); // 14
                #region Thêm dữ liệu
                foreach (tbNguoiDungExtend nguoiDung in EXCEL_NGUOIDUNGs_DOWNLOAD)
                {
                    tbNguoiDung.Rows.Add(
                       nguoiDung.TenNguoiDung, // 1
                       nguoiDung.TenDangNhap, // 2
                       nguoiDung.MatKhau, // 3
                       nguoiDung.Email, // 4
                       nguoiDung.SoDienThoai, // 5
                       nguoiDung.SoTaiKhoanNganHang, // 6
                       nguoiDung.NgaySinh == null ? "01/01/2020" : nguoiDung.NgaySinh.Value.ToString("dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture), // 7
                       nguoiDung.LinkLienHe, // 8
                       nguoiDung.GhiChu, // 9

                       nguoiDung.GioiTinh.Value ? "Nam" : "Nữ", // 10
                       nguoiDung.KichHoat.Value ? "Kích hoạt" : "Vô hiệu hóa", // 11
                       CHUCVUs.Where(x => x.IdChucVu == nguoiDung.IdChucVu).FirstOrDefault().TenChucVu, // 12
                       KIEUNGUOIDUNGs.Where(x => x.IdKieuNguoiDung == nguoiDung.IdKieuNguoiDung).FirstOrDefault().TenKieuNguoiDung, // 13
                       COCAUTOCHUCs.Where(x => x.IdCoCauToChuc == nguoiDung.IdCoCauToChuc).FirstOrDefault().TenCoCauToChuc // 14
                       );
                };
                #endregion
                #endregion
                #region Tạo sheet danh sách
                DataTable tbDanhSach = new DataTable();
                tbDanhSach.Columns.Add("Giới-tính", typeof(string)); // 1
                tbDanhSach.Columns.Add("Kích-hoạt", typeof(string)); // 2
                tbDanhSach.Columns.Add("Chức-vụ", typeof(string)); // 3
                tbDanhSach.Columns.Add("Kiểu-người-dùng", typeof(string)); // 4
                tbDanhSach.Columns.Add("Cơ-cấu-tổ-chức", typeof(string)); // 5
                #region Thêm dữ liệu
                //var a = new List<List<object>>
                //{

                //    new List<dynamic> { "" , ""},
                //    new List<dynamic> { "" , ""},
                //};
                //a.Add(new List<dynamic> { "", "" });
                // Giới tính
                string[] gioiTinhs = { "Nam", "Nữ" };
                for (int i = 0; i < gioiTinhs.Length; i++)
                {
                    if (tbDanhSach.Rows.Count <= i)
                        tbDanhSach.Rows.Add(tbDanhSach.NewRow());
                    tbDanhSach.Rows[i][0] = gioiTinhs[i];
                };
                // Kích hoạt
                string[] kichHoats = { "Kích hoạt", "Vô hiệu hóa" };
                for (int i = 0; i < kichHoats.Length; i++)
                {
                    if (tbDanhSach.Rows.Count <= i)
                        tbDanhSach.Rows.Add(tbDanhSach.NewRow());
                    tbDanhSach.Rows[i][1] = kichHoats[i];
                };
                // Tên chức vụ
                int tenChucVu_Count = CHUCVUs.Count();
                for (int i = 0; i < tenChucVu_Count; i++)
                {
                    if (tbDanhSach.Rows.Count <= i)
                        tbDanhSach.Rows.Add(tbDanhSach.NewRow());
                    tbDanhSach.Rows[i][2] = CHUCVUs[i].TenChucVu;
                };
                // Tên kiểu người dùng
                int tenKieuNguoiDung_Count = KIEUNGUOIDUNGs.Count();
                for (int i = 0; i < tenKieuNguoiDung_Count; i++)
                {
                    if (tbDanhSach.Rows.Count <= i)
                        tbDanhSach.Rows.Add(tbDanhSach.NewRow());
                    tbDanhSach.Rows[i][3] = KIEUNGUOIDUNGs[i].TenKieuNguoiDung;
                };
                // Cơ cấu tổ chức
                int tenCoCauToChuc_Count = COCAUTOCHUCs.Count();
                for (int i = 0; i < tenCoCauToChuc_Count; i++)
                {
                    if (tbDanhSach.Rows.Count <= i)
                        tbDanhSach.Rows.Add(tbDanhSach.NewRow());
                    tbDanhSach.Rows[i][4] = COCAUTOCHUCs[i].TenCoCauToChuc;
                };
                #endregion
                #endregion
                #region Tạo file excel
                workBook.Worksheets.Add(tbNguoiDung, "NguoiDung");
                workBook.Worksheets.Add(tbDanhSach, "DanhSach");
                for (int i = 1; i <= tbNguoiDung.Rows.Count + 1; i++)
                {
                    // List
                    workBook.Worksheets.First().Cell(i, 10).CreateDataValidation().List($"=OFFSET(DanhSach!$A$2,0,0,COUNTA(DanhSach!$A:$A),1)");
                    workBook.Worksheets.First().Cell(i, 11).CreateDataValidation().List($"=OFFSET(DanhSach!$B$2,0,0,COUNTA(DanhSach!$B:$B),1)");
                    workBook.Worksheets.First().Cell(i, 12).CreateDataValidation().List($"=OFFSET(DanhSach!$C$2,0,0,COUNTA(DanhSach!$C:$C),1)");
                    workBook.Worksheets.First().Cell(i, 13).CreateDataValidation().List($"=OFFSET(DanhSach!$D$2,0,0,COUNTA(DanhSach!$D:$D),1)");
                    workBook.Worksheets.First().Cell(i, 14).CreateDataValidation().List($"=OFFSET(DanhSach!$E$2,0,0,COUNTA(DanhSach!$E:$E),1)");
                    // Date
                    //workBook.Worksheets.First().Cell(i, 6).CreateDataValidation().Date.GreaterThan(new DateTime(1990, 1, 1));
                };
                #endregion
                #region Tải file excel về máy client
                MemoryStream memoryStream = new MemoryStream();
                memoryStream.Position = 0;
                workBook.SaveAs(memoryStream);
                downloadDialog(data: memoryStream, fileName: Server.UrlEncode("NGUOIDUNG.xlsx"), contentType: "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
                #endregion
            }
            #endregion
            return Redirect("/UserAccount/Index");
        }
        public ActionResult save_Excel_NguoiDung()
        {
            string status = "";
            string mess = "";
            List<tbNguoiDungExtend> nguoiDung_HopLes = new List<tbNguoiDungExtend>();
            List<tbNguoiDungExtend> nguoiDung_KhongHopLes = new List<tbNguoiDungExtend>();
            using (var scope = db.Database.BeginTransaction())
            {
                try
                {
                    if (EXCEL_NGUOIDUNGs_DOWNLOAD.Count == 0)
                    {
                        status = "error";
                        mess = "Chưa có bản ghi nào";
                    }
                    else
                    {
                        foreach (tbNguoiDungExtend nguoiDung_NEW in EXCEL_NGUOIDUNGs_DOWNLOAD)
                        {
                            nguoiDung_NEW.TenDangNhap = taoTenDangNhap(tenDangNhap: nguoiDung_NEW.TenDangNhap);
                            // Kiểm tra excel
                            tbNguoiDungExtend nguoiDung_KhongHopLe = kiemTra_Excel_NguoiDung(nguoiDung_NEW);
                            if (nguoiDung_KhongHopLe.KiemTraExcel.TrangThai == 0)
                            {
                                nguoiDung_KhongHopLes.Add(nguoiDung_KhongHopLe);
                            }
                            else
                            {
                                // Kiểm tra tên biểu mẫu này đã được thêm ở bản ghi trước đó chưa
                                if (nguoiDung_HopLes.Any(x => x.TenDangNhap == nguoiDung_NEW.TenDangNhap))
                                {
                                    nguoiDung_NEW.KiemTraExcel.TrangThai = 2;
                                    nguoiDung_NEW.KiemTraExcel.KetQua = "Trùng tên đăng nhập";
                                    nguoiDung_KhongHopLes.Add(nguoiDung_NEW);
                                }
                                else
                                {
                                    // Tạo người dùng
                                    string matKhau = Public.Handle.HashToMD5(nguoiDung_NEW.MatKhau);
                                    tbNguoiDung nguoiDung = new tbNguoiDung
                                    {
                                        IdNguoiDung = Guid.NewGuid(),
                                        TenDangNhap = nguoiDung_NEW.TenDangNhap,
                                        MatKhau = matKhau,
                                        TenNguoiDung = nguoiDung_NEW.TenNguoiDung,
                                        GioiTinh = nguoiDung_NEW.GioiTinh,
                                        KichHoat = nguoiDung_NEW.KichHoat,
                                        SoLanDangNhap = 0,
                                        Online = false,
                                        YeuCauDoiMatKhau = true,
                                        Email = nguoiDung_NEW.Email,
                                        SoDienThoai = nguoiDung_NEW.SoDienThoai,
                                        SoTaiKhoanNganHang = nguoiDung_NEW.SoTaiKhoanNganHang,
                                        NgaySinh = nguoiDung_NEW.NgaySinh,
                                        GhiChu = nguoiDung_NEW.GhiChu,
                                        LinkLienHe = nguoiDung_NEW.LinkLienHe,
                                        IdChucVu = nguoiDung_NEW.IdChucVu,
                                        IdKieuNguoiDung = nguoiDung_NEW.IdKieuNguoiDung,
                                        IdCoCauToChuc = nguoiDung_NEW.IdCoCauToChuc,

                                        TrangThai = 1,
                                        IdNguoiTao = per.NguoiDung.IdNguoiDung,
                                        NgayTao = DateTime.Now,
                                        MaDonViSuDung = per.DonViSuDung.MaDonViSuDung
                                    };
                                    db.tbNguoiDungs.Add(nguoiDung);
                                    db.SaveChanges();
                                    // Gửi mail
                                    GuiMai(nguoiDung: nguoiDung, nguoiDung_NEW: nguoiDung_NEW);

                                    nguoiDung_HopLes.Add(nguoiDung_NEW);
                                };
                            };
                        };
                        if (nguoiDung_KhongHopLes.Count == 0)
                        { // Thêm bản ghi thành công và không tồn tại bản ghi không hợp lệ
                            status = "success";
                            mess = "Thêm mới bản ghi thành công";

                            db.SaveChanges();
                            scope.Commit();
                        }
                        else
                        { // Khi thêm thành công, thay thế EXCEL_NGUOIDUNGs_UPLOAD bằng nguoiDung_KhongHopLes
                            if (nguoiDung_KhongHopLes.Count == EXCEL_NGUOIDUNGs_DOWNLOAD.Count)
                            { // Tất cả đều không hợp lệ
                                status = "error-1";
                                mess = "Thêm mới bản ghi không thành công, vui lòng kiểm tra lại những bản ghi [CHƯA HỢP LỆ]";
                            }
                            else
                            {
                                status = "warning";
                                mess = "Thêm mới bản ghi [HỢP LỆ] thành công, vui lòng kiểm tra lại những bản ghi [CHƯA HỢP LỆ]";

                                db.SaveChanges();
                                scope.Commit();
                            };
                            // Trả lại danh sách bản ghi không hợp lệ
                            EXCEL_NGUOIDUNGs_UPLOAD = new List<tbNguoiDungExtend>();
                            EXCEL_NGUOIDUNGs_UPLOAD.AddRange(nguoiDung_KhongHopLes);
                        }
                    }
                }
                catch (Exception ex)
                {
                    status = "error-0";
                    mess = ex.Message;
                    scope.Rollback();
                }
            }
            return Json(new
            {
                nguoiDung_KhongHopLes,
                status,
                mess
            }, JsonRequestBehavior.AllowGet);
        }
        public void get_NguoiDungs_download()
        {
            string loaiTaiXuong = Request.Form["loaiTaiXuong"];
            string str_nguoiDungs = Request.Form["str_nguoiDungs"];
            EXCEL_NGUOIDUNGs_DOWNLOAD = new List<tbNguoiDungExtend>();
            EXCEL_NGUOIDUNGs_DOWNLOAD.Add(new tbNguoiDungExtend
            {
                TenNguoiDung = "Người dùng 1",
                TenDangNhap = "nguoidung1",
                MatKhau = "123456",
                Email = "email@gmail.com",
                SoDienThoai = "0359999999",
                SoTaiKhoanNganHang = "0359999999 MB",
                LinkLienHe = "",
                GhiChu = "",
                NgaySinh = DateTime.Now,
                GioiTinh = true,
                KichHoat = false,
                IdChucVu = CHUCVUs.FirstOrDefault().IdChucVu,
                ChucVu = new default_tbChucVu
                {
                    IdChucVu = CHUCVUs.FirstOrDefault().IdChucVu,
                    TenChucVu = CHUCVUs.FirstOrDefault().TenChucVu
                },
                IdKieuNguoiDung = KIEUNGUOIDUNGs.FirstOrDefault().IdKieuNguoiDung,
                KieuNguoiDung = new tbKieuNguoiDung
                {
                    IdKieuNguoiDung = KIEUNGUOIDUNGs.FirstOrDefault().IdKieuNguoiDung,
                    TenKieuNguoiDung = KIEUNGUOIDUNGs.FirstOrDefault().TenKieuNguoiDung,
                },
                IdCoCauToChuc = COCAUTOCHUCs.FirstOrDefault().IdCoCauToChuc,
                CoCauToChuc = new tbCoCauToChuc
                {
                    IdCoCauToChuc = COCAUTOCHUCs.FirstOrDefault().IdCoCauToChuc,
                    TenCoCauToChuc = COCAUTOCHUCs.FirstOrDefault().TenCoCauToChuc
                },
            });
            if (loaiTaiXuong == "data")
            {
                string format = "dd/MM/yyyy";
                IsoDateTimeConverter dateTimeConverter = new IsoDateTimeConverter { DateTimeFormat = format };
                EXCEL_NGUOIDUNGs_DOWNLOAD = JsonConvert.DeserializeObject<List<tbNguoiDungExtend>>(str_nguoiDungs ?? "", dateTimeConverter) ?? new List<tbNguoiDungExtend>();
            };
        }
        public ActionResult getList_Excel_NguoiDung(string loai)
        {
            if (loai == "reload") EXCEL_NGUOIDUNGs_UPLOAD = new List<tbNguoiDungExtend>();
            return PartialView($"{VIEW_PATH}/useraccount-excel.nguoidung/excel.nguoidung-getList.cshtml");
        }
        #endregion

        #region Private Methods
        private void GuiMai(tbNguoiDung nguoiDung, tbNguoiDungExtend nguoiDung_NEW)
        {
            string mail()
            {
                var model = new CapNhatTaiKhoanMailM<tbNguoiDungExtend>
                {
                    NguoiDung_NEW = new tbNguoiDungExtend
                    {
                        TenDangNhap = nguoiDung_NEW.TenDangNhap,
                        TenNguoiDung = nguoiDung_NEW.TenNguoiDung,
                        GioiTinh = nguoiDung_NEW.GioiTinh,
                        KichHoat = nguoiDung_NEW.KichHoat,
                        MatKhau = nguoiDung_NEW.MatKhau,
                        Email = nguoiDung_NEW.Email,
                        SoDienThoai = nguoiDung_NEW.SoDienThoai,
                        SoTaiKhoanNganHang = nguoiDung_NEW.SoTaiKhoanNganHang,
                        NgaySinh = nguoiDung_NEW.NgaySinh,

                        KieuNguoiDung = KIEUNGUOIDUNGs.FirstOrDefault(x => x.IdKieuNguoiDung == nguoiDung_NEW.IdKieuNguoiDung) ?? new tbKieuNguoiDung(),
                        CoCauToChuc = COCAUTOCHUCs.FirstOrDefault(x => x.IdCoCauToChuc == nguoiDung_NEW.IdCoCauToChuc) ?? new tbCoCauToChuc(),
                        ChucVu = CHUCVUs.FirstOrDefault(x => x.IdChucVu == nguoiDung_NEW.IdChucVu) ?? new default_tbChucVu(),
                    },
                    DonViSuDung = per.DonViSuDung
                };
                // Gọi phương thức RenderViewToString() để chuyển đổi view thành chuỗi
                string viewAsString = Public.Handle.RenderViewToString(this, $"{VIEW_PATH}/useraccount-mail.taotaikhoan.cshtml", model);
                // Trả về chuỗi đã được tạo ra từ view
                return viewAsString;
            }
            string tieuDeMail = "[📣 GIAPTECH] - THÔNG TIN TÀI KHOẢN CRM❗";
            string mailBody = mail();
            // Gửi mail
            Public.Handle.SendEmail(sendTo: nguoiDung.Email, subject: tieuDeMail, body: mailBody, isHTML: true, donViSuDung: per.DonViSuDung);
        }
        private string taoTenDangNhap(string tenDangNhap)
        {
            return string.Format("{0}@vietgenacademy.edu.vn", tenDangNhap.Replace("@vietgenacademy.edu.vn", ""));
        }
        #endregion
    }
}