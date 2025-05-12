using Applications.QuanLyChienDich.Dtos;
using Applications.QuanLyChienDich.Models;
using EDM_DB;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Public.Controllers;
using Public.Models;
using System;
using System.Collections.Generic;
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
            && x.IdChienDich!= chienDich.IdChienDich
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
    }
}