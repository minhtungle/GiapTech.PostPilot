using Applications.QuanLyChienDich.Dtos;
using Applications.QuanLyChienDich.Models;
using EDM_DB;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Public.Controllers;
using Public.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace QuanLyChienDich.Controllers
{
    public class QuanLyChienDichController : RouteConfigController
    {
        #region Biến public để in hoa
        private readonly string VIEW_PATH = "~/Views/Admin/QuanLyChienDich";
        private List<default_tbChucNang> CHUCNANGs
        {
            get
            {
                return Session["CHUCNANGs"] as List<default_tbChucNang> ?? new List<default_tbChucNang>();
            }
            set
            {
                Session["CHUCNANGs"] = value;
            }
        }
        private List<ThaoTac> THAOTACs
        {
            get
            {
                return Session["THAOTACs"] as List<ThaoTac> ?? new List<ThaoTac>();
            }
            set
            {
                Session["THAOTACs"] = value;
            }
        }
        public QuanLyChienDichController()
        {
        }
        #endregion
        // GET: ChienDich
        public ActionResult Index()
        {
            #region Lấy các danh sách

            #region Thao tác
            List<ChucNangs> kieuNguoiDung_IdChucNang = JsonConvert.DeserializeObject<List<ChucNangs>>(per.KieuNguoiDung.IdChucNang);
            List<ThaoTac> thaoTacs = kieuNguoiDung_IdChucNang.FirstOrDefault(x => x.ChucNang.MaChucNang == "QuanLyChienDich").ThaoTacs ?? new List<ThaoTac>();
            #endregion

            #endregion

            THAOTACs = thaoTacs;

            return View($"{VIEW_PATH}/chiendich.cshtml");
        }
        [HttpGet]
        public ActionResult getList_ChienDich()
        {
            List<tbChienDichExtend> chienDichs = getChienDichs(loai: "all");
            return PartialView($"{VIEW_PATH}/chiendich-getList.cshtml", chienDichs);
        }
        public List<tbChienDichExtend> getChienDichs(string loai = "all", List<Guid> idChienDichs = null, LocThongTinDto locThongTin = null)
        {
            var chienDichRepo = db.tbChienDiches.Where(x => x.TrangThai != 0 && x.MaDonViSuDung == per.DonViSuDung.MaDonViSuDung).ToList()
                ?? new List<tbChienDich>();

            var chienDichs = chienDichRepo
                .Where(x => loai != "single" || idChienDichs.Contains(x.IdChienDich))
                .Select(g => new tbChienDichExtend
                {
                    ChienDich = g,
                })
                .OrderByDescending(x => x.ChienDich.NgayTao)
                .ToList() ?? new List<tbChienDichExtend>();

            return chienDichs;
        }
        [HttpPost]
        public ActionResult displayModal_CRUD_ChienDich(DisplayModel_CRUD_ChienDich_Input_Dto input)
        {
            //var baiDang = getChienDichs(loai: "single", idChienDichs: new List<Guid> { input.IdBaiDang })?.FirstOrDefault() ?? new tbBaiDangExtend();
            var chienDich = new tbChienDichExtend();
            var output = new DisplayModel_CRUD_ChienDich_Output_Dto
            {
                Loai = input.Loai,
                ChienDich = chienDich,
            };
            return PartialView($"{VIEW_PATH}/chiendich-crud.cshtml", output);
        }
        public bool kiemTra_ChienDich(tbChienDich chienDich)
        {
            // Kiểm tra còn hồ sơ khác có trùng mã không
            var chienDich_OLD = db.tbChienDiches.FirstOrDefault(x =>
            x.TenChienDich == chienDich.TenChienDich
            && x.IdChienDich != chienDich.IdChienDich
            && x.TrangThai != 0 && x.MaDonViSuDung == per.DonViSuDung.MaDonViSuDung);
            if (chienDich_OLD == null) return false;
            return true;
        }
        [HttpPost]
        public ActionResult create_ChienDich()
        {
            string status = "success";
            string mess = "Thêm mới bản ghi thành công";
            using (var scope = db.Database.BeginTransaction())
            {
                try
                {
                    var chienDich_NEW = JsonConvert.DeserializeObject<tbChienDichExtend>(Request.Form["chienDich"]);
                    if (chienDich_NEW == null)
                    {
                        status = "error";
                        mess = "Chưa có bản ghi nào";
                    }
                    else
                    {
                        if (kiemTra_ChienDich(chienDich: chienDich_NEW.ChienDich))
                        {
                            status = "datontai";
                            mess = "Tên chiến dịch đã tồn tại";
                        }
                        else
                        {
                            // Tạo hồ sơ
                            var chienDich = new tbChienDich
                            {
                                IdChienDich = Guid.NewGuid(),
                                TenChienDich = chienDich_NEW.ChienDich.TenChienDich,
                                GhiChu = chienDich_NEW.ChienDich.GhiChu,

                                TrangThaiHoatDong = 1,
                                TrangThai = 1,
                                IdNguoiTao = per.NguoiDung.IdNguoiDung,
                                NgayTao = DateTime.Now,
                                MaDonViSuDung = per.DonViSuDung.MaDonViSuDung
                            };
                            db.tbChienDiches.Add(chienDich);

                            db.SaveChanges();
                            scope.Commit();
                        }
                        ;
                    }
                    ;
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
        public JsonResult delete_ChienDichs()
        {
            string status = "success";
            string mess = "Xóa bản ghi thành công";
            using (var scope = db.Database.BeginTransaction())
            {
                try
                {
                    List<Guid> idChienDichs = JsonConvert.DeserializeObject<List<Guid>>(Request.Form["idChienDichs"]);
                    if (idChienDichs.Count > 0)
                    {
                        foreach (var idChienDich in idChienDichs)
                        {
                            var chienDich_OLD = db.tbChienDiches.Find(idChienDich);
                            if (chienDich_OLD != null)
                            {
                                chienDich_OLD.TrangThaiHoatDong = 10; // Đã xóa
                                chienDich_OLD.TrangThai = 0;
                                chienDich_OLD.IdNguoiSua = per.NguoiDung.IdNguoiDung;
                                chienDich_OLD.NgaySua = DateTime.Now;

                                string duongDanThuMucGoc = string.Format("/Assets/uploads/{0}/TEPDINHKEM/{1}",
                                     per.DonViSuDung.MaDonViSuDung, chienDich_OLD.IdChienDich);

                                string folderPath_SERVER = Request.MapPath(duongDanThuMucGoc);
                                // Xóa thư mục
                                if (System.IO.Directory.Exists(folderPath_SERVER))
                                    System.IO.Directory.Delete(folderPath_SERVER, true);

                                var baiDangs = db.tbBaiDangs.Where(x => x.IdChienDich == idChienDich).ToList();
                                foreach (var baiDang in baiDangs)
                                {
                                    baiDang.TrangThaiDangBai = 9; // Chờ xóa trên nền tảng
                                    baiDang.TrangThai = 0;
                                    baiDang.IdNguoiSua = per.NguoiDung.IdNguoiDung;
                                    baiDang.NgaySua = DateTime.Now;

                                    var baiDangTepDinhKems = db.tbBaiDangTepDinhKems
                                        .Where(x => x.IdBaiDang == baiDang.IdBaiDang)
                                        .ToList();

                                    // Ép danh sách ID tệp về danh sách Guid
                                    var tepIds = baiDangTepDinhKems.Select(y => y.IdTepDinhKem).ToList();

                                    var tepDinhKems = db.tbTepDinhKems
                                        .Where(x => tepIds.Contains(x.IdTep))
                                        .ToList();

                                    foreach (var tepDinhKem in tepDinhKems)
                                    {
                                        tepDinhKem.TrangThai = 0;
                                        tepDinhKem.IdNguoiSua = per.NguoiDung.IdNguoiDung;
                                        tepDinhKem.NgaySua = DateTime.Now;
                                    };
                                }

                            }
                            ;
                        }
                        ;
                        db.SaveChanges();
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
    }
}