using EDM_DB;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Organization.Controllers;
using Public.Controllers;
using Public.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using UserAccount.Models;

namespace EDM.Controllers
{
    public class AccountInfoController : RouteConfigController
    {
        #region Biến public để in hoa
        private readonly string VIEW_PATH = "~/Views/Admin/__Home/AccountInfo";
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
        private OrganizationController organizationController = new OrganizationController();
        #endregion
        public ActionResult Index()
        {
            return View($"{VIEW_PATH}/accountinfo.cshtml");
        }
        public ActionResult getList()
        {
            tbNguoiDung nguoiDung = per.NguoiDung;
            #region Lấy các danh sách
            #region Kiểu người dùng
            List<tbKieuNguoiDung> kieuNguoiDungs = db.tbKieuNguoiDungs.Where(x => x.MaDonViSuDung == per.DonViSuDung.MaDonViSuDung && x.TrangThai == 1).ToList() ?? new List<tbKieuNguoiDung>();
            #endregion
            #region Cơ cấu tổ chức
            List<tbCoCauToChuc> coCauToChucs = new List<tbCoCauToChuc>();
            List<Tree<tbCoCauToChuc>> coCauToChucs_Tree = organizationController.get_CoCauToChucs_Tree(idCoCau: Guid.Empty, maDonViSuDung: per.DonViSuDung.MaDonViSuDung).nodes;
            organizationController.xuLy_TenCoCauToChuc(coCaus_IN: coCauToChucs_Tree, coCaus_OUT: coCauToChucs);
            #endregion
            #endregion
            KIEUNGUOIDUNGs = kieuNguoiDungs;
            COCAUTOCHUCs = coCauToChucs;
            ViewBag.nguoiDung = nguoiDung;
            ViewBag.loai = "update";
            return PartialView($"{VIEW_PATH}/accountinfo-getList.cshtml");
        }
        public bool kiemTra_NguoiDung(tbNguoiDung nguoiDung)
        {
            // Kiểm tra còn hồ sơ khác có trùng mã không
            tbNguoiDung nguoiDung_OLD = db.tbNguoiDungs.FirstOrDefault(x => x.TenDangNhap == nguoiDung.TenDangNhap && x.IdNguoiDung != nguoiDung.IdNguoiDung && x.TrangThai != 0 && x.MaDonViSuDung == per.DonViSuDung.MaDonViSuDung);
            if (nguoiDung_OLD == null) return false;
            return true;
        }
        [HttpPost]
        public ActionResult update_NguoiDung(string str_nguoiDung)
        {
            string status = "success";
            string mess = "Cập nhật tài khoản thành công, vui lòng đăng nhập lại";

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
                        if (kiemTra_NguoiDung(nguoiDung: nguoiDung_NEW))
                        {
                            status = "datontai";
                            mess = "Tên đăng nhập đã tồn tại";
                        }
                        else
                        {
                            tbNguoiDungExtend nguoiDung_OLD = db.Database.SqlQuery<tbNguoiDungExtend>($@"select * from tbNguoiDung where MaDonViSuDung = {per.DonViSuDung.MaDonViSuDung} and IdNguoiDung = {nguoiDung_NEW.IdNguoiDung}").FirstOrDefault();
                            if (nguoiDung_NEW.IdKieuNguoiDung != null)
                            {
                                nguoiDung_NEW.KieuNguoiDung = db.tbKieuNguoiDungs.Find(nguoiDung_NEW.IdKieuNguoiDung) ?? new tbKieuNguoiDung();
                                nguoiDung_OLD.KieuNguoiDung = db.tbKieuNguoiDungs.Find(nguoiDung_OLD.IdKieuNguoiDung) ?? new tbKieuNguoiDung();

                            };
                            if (nguoiDung_NEW.IdCoCauToChuc != null)
                            {
                                nguoiDung_NEW.CoCauToChuc = db.tbCoCauToChucs.Find(nguoiDung_NEW.IdCoCauToChuc) ?? new tbCoCauToChuc();
                                nguoiDung_OLD.CoCauToChuc = db.tbCoCauToChucs.Find(nguoiDung_OLD.IdCoCauToChuc) ?? new tbCoCauToChuc();
                            };
                            string sql_capNhatNguoiDung = $@"
                            update tbNguoiDung set
                                TenDangNhap = '{nguoiDung_NEW.TenDangNhap}',
                                TenNguoiDung = N'{nguoiDung_NEW.TenNguoiDung}',
                                GioiTinh = {(nguoiDung_NEW.GioiTinh.Value ? 1 : 0)},
                                KichHoat = {(nguoiDung_NEW.KichHoat.Value ? 1 : 0)},
                                Email = '{nguoiDung_NEW.Email}',
                                SoDienThoai = '{nguoiDung_NEW.SoDienThoai}',
                                NgaySinh = '{nguoiDung_NEW.NgaySinh}',
                                IdChucVu = '{nguoiDung_NEW.IdChucVu}',
                                IdKieuNguoiDung = {nguoiDung_NEW.IdKieuNguoiDung},
                                IdCoCauToChuc = {nguoiDung_NEW.IdCoCauToChuc},

                                NguoiSua = {per.NguoiDung.IdNguoiDung},
                                NgaySua = '{DateTime.Now}'
                            where MaDonViSuDung = {per.DonViSuDung.MaDonViSuDung} and IdNguoiDung = {nguoiDung_NEW.IdNguoiDung}";
                            db.Database.ExecuteSqlCommand(sql_capNhatNguoiDung);
                            // Cập nhật lại session
                            per.NguoiDung = nguoiDung_OLD;
                            db.SaveChanges();
                            #region Gửi mail
                            string mail()
                            {
                                var model = new CapNhatTaiKhoanMailM<tbNguoiDungExtend>
                                {
                                    NguoiDung_OLD = nguoiDung_OLD,
                                    NguoiDung_NEW = nguoiDung_NEW,
                                    DonViSuDung = per.DonViSuDung
                                };
                                // Gọi phương thức RenderViewToString() để chuyển đổi view thành chuỗi
                                string viewAsString = Public.Handle.RenderViewToString(this, $"~/Views/Admin/_SystemSetting/UserAccount/useraccount-mail.capnhattaikhoan.cshtml", model);
                                // Trả về chuỗi đã được tạo ra từ view
                                return viewAsString;
                            }
                            string tieuDeMail = "[📣 EDM] - CẬP NHẬT THÔNG TIN TÀI KHOẢN❗";
                            string mailBody = mail();
                            // Gửi mail
                            Public.Handle.SendEmail(sendTo: nguoiDung_OLD.Email, subject: tieuDeMail, body: mailBody, isHTML: true, donViSuDung: per.DonViSuDung);
                            if (nguoiDung_NEW.Email != nguoiDung_OLD.Email)
                            {
                                Public.Handle.SendEmail(sendTo: nguoiDung_NEW.Email, subject: tieuDeMail, body: mailBody, isHTML: true, donViSuDung: per.DonViSuDung);
                            }
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
        public ActionResult capNhat_MatKhau(string str_nguoiDung)
        {
            string status = "success";
            string mess = "Cập nhật tài khoản thành công, vui lòng đăng nhập lại";

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
                        if (nguoiDung_OLD.MatKhau != Public.Handle.HashToMD5(nguoiDung_NEW.MatKhauCu))
                        {
                            status = "matkhaucuchuachinhxac";
                            mess = "Mật khẩu cũ chưa chính xác";
                        }
                        else
                        {
                            string matKhau_MD5 = Public.Handle.HashToMD5(nguoiDung_NEW.MatKhauMoi);
                            nguoiDung_OLD.MatKhau = matKhau_MD5;
                            // Cập nhật lại session
                            per.NguoiDung = nguoiDung_OLD;
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
                                string viewAsString = Public.Handle.RenderViewToString(this, $"~/Views/Admin/_SystemSetting/UserAccount/useraccount-mail.capnhattaikhoan.cshtml", model);
                                // Trả về chuỗi đã được tạo ra từ view
                                return viewAsString;
                            }
                            string tieuDeMail = "[📣 EDM] - CẬP NHẬT THÔNG TIN TÀI KHOẢN❗";
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
    }
}