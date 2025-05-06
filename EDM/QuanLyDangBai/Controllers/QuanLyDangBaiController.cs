using Applications.QuanLyDangBai.Dtos;
using Applications.QuanLyDangBai.Models;
using EDM_DB;
using Microsoft.Ajax.Utilities;
using Newtonsoft.Json;
using Public.Controllers;
using Public.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace QuanLyDangBai.Controllers
{
    public class QuanLyDangBaiController : RouteConfigController
    {
        #region Biến public để in hoa
        private readonly string VIEW_PATH = "~/Views/Admin/QuanLyDangBai";
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

        private List<tbBaiDang> baiDangRepo = new List<tbBaiDang>();
        private List<tbAnhMoTa> anhMoTaRepo = new List<tbAnhMoTa>();
        private List<tbNenTang> nenTangRepo = new List<tbNenTang>();
        private List<tbLichDangBai> lichDangBaiRepo = new List<tbLichDangBai>();
        public QuanLyDangBaiController()
        {
            baiDangRepo = db.tbBaiDangs.Where(x => x.TrangThai != 0 && x.MaDonViSuDung == per.DonViSuDung.MaDonViSuDung).ToList()
                ?? new List<tbBaiDang>();
            anhMoTaRepo = db.tbAnhMoTas.Where(x => x.TrangThai != 0 && x.MaDonViSuDung == per.DonViSuDung.MaDonViSuDung).ToList()
                ?? new List<tbAnhMoTa>();
            nenTangRepo = db.tbNenTangs.Where(x => x.TrangThai != 0 && x.MaDonViSuDung == per.DonViSuDung.MaDonViSuDung).ToList()
                ?? new List<tbNenTang>();
            lichDangBaiRepo = db.tbLichDangBais.Where(x => x.TrangThai != 0 && x.MaDonViSuDung == per.DonViSuDung.MaDonViSuDung).ToList()
                ?? new List<tbLichDangBai>();
        }
        #endregion

        public ActionResult Index()
        {
            #region Lấy các danh sách

            #region Thao tác
            List<ChucNangs> kieuNguoiDung_IdChucNang = JsonConvert.DeserializeObject<List<ChucNangs>>(per.KieuNguoiDung.IdChucNang);
            List<ThaoTac> thaoTacs = kieuNguoiDung_IdChucNang.FirstOrDefault(x => x.ChucNang.MaChucNang == "QuanLyDangBai").ThaoTacs ?? new List<ThaoTac>();
            #endregion

            #endregion

            THAOTACs = thaoTacs;

            return View($"{VIEW_PATH}/quanlydangbai.cshtml");
        }
        [HttpGet]
        public ActionResult getList_LichDangBai()
        {
            List<tbBaiDangExtend> baiDangs = getLichDangBais(loai: "all") ?? new List<tbBaiDangExtend>();
            return PartialView($"{VIEW_PATH}/tailieu/tailieu-getList.cshtml", baiDangs);
        }
        [HttpGet]
        public ActionResult getList_LoaiBaiDang()
        {
            List<tbLoaiBaiDang> loaiBaiDangs = get_LoaiBaiDang(loai: "all");
            return PartialView($"{VIEW_PATH}/nhandan/nhandan-getList.cshtml", loaiBaiDangs);
        }
        public List<tbBaiDangExtend> getLichDangBais(string loai = "all", List<Guid> idLichDangBais = null, LocThongTinDto locThongTin = null)
        {
            var lichDangBais = lichDangBaiRepo
                .Where(lich => loai != "single" || idLichDangBais.Contains(lich.IdLichDangBai))
                .Join(baiDangRepo,
                      lich => lich.IdBaiDang,
                      bai => bai.IdBaiDang,
                      (lich, bai) => new
                      {
                          LichDangBai = lich,
                          BaiDang = bai
                      })
                .Join(anhMoTaRepo,
                      input => input.BaiDang.IdBaiDang,
                      a => a.IdBaiDang,
                      (tl, a) => new
                      {
                          Output = tl,
                          Anh = a
                      })
                .GroupBy(x => x.Output.BaiDang.IdBaiDang)
                .Select(g => new tbBaiDangExtend
                {
                    BaiDang = g.First().Output.BaiDang,
                    DonViTien = g.First().Output.DonViTien,
                    AnhMoTas = g.Select(x => x.Anh).ToList()
                })
                .ToList() ?? new List<tbBaiDangExtend>();

            return lichDangBais;
        }
        [HttpPost]
        public ActionResult displayModal_CRUD(DisplayModel_CRUD_BaiDang_Input_Dto input)
        {
            var baiDang = getLichDangBais(loai: "single", idLichDangBais: new List<Guid> { input.IdBaiDang })?.FirstOrDefault() ?? new tbBaiDangExtend();

            return PartialView($"{VIEW_PATH}/baidang/baidang-crud.cshtml", new DisplayModel_CRUD_BaiDang_Output_Dto
            {
                Loai = input.Loai,
                BaiDang = baiDang,
            });
        }
        [HttpPost]
        public ActionResult displayModal_XemChiTiet(Guid idBaiDang)
        {
            var baiDang = getLichDangBais(loai: "single", idLichDangBais: new List<Guid> { idBaiDang })?.FirstOrDefault();

            //Uri uri = new Uri(HttpContext.Request.Url.AbsoluteUri);
            //string hostName = uri.GetLeftPart(UriPartial.Authority);

            //if (baiDang.BaiDang.LoaiTep.Contains("pdf"))
            //    ViewBag.iframeHtml = $"<iframe src=\"{hostName}/XemPDF?duongDanTep={idHoSo}&idVanBan={idVanBan}\" title=\"Chi tiết\" style=\"width: 100%;height: 70vh\"></iframe>";
            //else if (baiDang.BaiDang.LoaiTep.Contains("mp4"))
            //    ViewBag.iframeHtml = $"<video src=\"{baiDang.DuongDan}\" controls style=\"width: 100%; height: 70vh; border: 1px solid var(--bs-body-color)\"></video>";
            //else
            //    ViewBag.iframeHtml = $"<iframe src=\"{baiDang.DuongDan}\" title=\"Chi tiết\" style=\"width: 100%;height: 70vh; border: 1px solid var(--bs-body-color)\"></iframe>";
            return PartialView($"{VIEW_PATH}/baidang/baidang-xemchitiet.cshtml", baiDang);
        }
        //public ActionResult XemPDF(string duongDanTep, bool quyenTaiXuong = true)
        //{
        //    var model = new XemPDF_Output_Dto
        //    {
        //        DuongDanTep = duongDanTep,
        //        QuyenTaiXuong = quyenTaiXuong
        //    };

        //    return View("~/Views/_Shared/_lib/pdf_viewer.cshtml", model);
        //}
        [HttpPost]
        public tbBaiDangExtend kiemTra_BaiDang(tbBaiDangExtend baiDang)
        {
            List<string> ketQuas = new List<string>();
            // Kiểm tra còn tài liệu khác có trùng mã không
            var baiDang_OLD = db.tbBaiDangx.FirstOrDefault(x =>
            x.TenBaiDang == baiDang.BaiDang.TenBaiDang &&
            x.IdBaiDang != baiDang.BaiDang.IdBaiDang &&
            x.TrangThai != 0 && x.MaDonViSuDung == per.DonViSuDung.MaDonViSuDung);
            if (baiDang_OLD != null)
            {
                ketQuas.Add("Tài liệu đã tồn tại");
                baiDang.KiemTraExcel.TrangThai = 0;
            }
        ;
            baiDang.KiemTraExcel.KetQua = string.Join(", ", ketQuas);
            return baiDang;
        }
        [HttpPost]
        public ActionResult create_BaiDang()
        {
            string status = "success";
            string mess = "Thêm mới bản ghi thành công";
            using (var scope = db.Database.BeginTransaction())
            {
                try
                {
                    var baiDang_NEW = JsonConvert.DeserializeObject<tbBaiDangExtend>(Request.Form["tailieu"]);
                    var files = Request.Files;

                    if (baiDang_NEW == null)
                    {
                        status = "error";
                        mess = "Chưa có bản ghi nào";
                    }
                    else
                    {
                        if (kiemTra_BaiDang(baiDang: baiDang_NEW).KiemTraExcel.TrangThai == 0)
                        {
                            status = "warning";
                            mess = baiDang_NEW.KiemTraExcel.KetQua;
                        }
                        else
                        {
                            // Thêm mới
                            var baiDang = new tbBaiDang
                            {
                                IdBaiDang = Guid.NewGuid(),
                                TenBaiDang = baiDang_NEW.BaiDang.TenBaiDang,
                                GiaTien = baiDang_NEW.BaiDang.GiaTien,
                                SoLuong = baiDang_NEW.BaiDang.SoLuong,
                                IdDonViTien = baiDang_NEW.BaiDang.IdDonViTien,
                                MoTa = baiDang_NEW.BaiDang.MoTa,
                                GhiChu = baiDang_NEW.BaiDang.GhiChu,
                                //IdLoaiBaiDang = baiDang_NEW.BaiDang.IdLoaiBaiDang,

                                TrangThai = 1,
                                IdNguoiTao = per.NguoiDung.IdNguoiDung,
                                NgayTao = DateTime.Now,
                                MaDonViSuDung = per.DonViSuDung.MaDonViSuDung
                            };
                            db.tbBaiDangx.Add(baiDang);

                            for (int i = 0; i < files.Count; i++)
                            {
                                var f = Request.Files[i];
                                var key = Request.Files.GetKey(i);

                                string duongDanThuMucGoc = string.Format("/Assets/uploads/{0}/TAILIEU/{1}",
                                    per.DonViSuDung.MaDonViSuDung, baiDang.IdBaiDang);
                                string tenBaiDang_BANDAU = Path.GetFileName(f.FileName);
                                var duongDanTep = LayDuongDanTep(duongDanThuMucGoc: duongDanThuMucGoc, tenTep_BANDAU: tenBaiDang_BANDAU);

                                byte[] imgData = null;
                                using (var binaryReader = new BinaryReader(f.InputStream))
                                {
                                    imgData = binaryReader.ReadBytes(f.ContentLength);
                                }
                                ;

                                var anhMoTa = new tbAnhMoTa
                                {
                                    IdAnhMoTa = Guid.NewGuid(),
                                    IdBaiDang = baiDang.IdBaiDang,
                                    LaAnhDaiDien = key == "anhdaidien" ? true : false,
                                    ImgData = imgData,
                                    TenTepGoc = Path.GetFileNameWithoutExtension(f.FileName),
                                    TenTepMoi = duongDanTep.TenTep_CHUYENDOI,
                                    LoaiTep = duongDanTep.LoaiTep,

                                    DuongDanTepVatLy = duongDanTep.DuongDanTep_BANDAU,
                                    TrangThai = 1,
                                    IdNguoiTao = per.NguoiDung.IdNguoiDung,
                                    NgayTao = DateTime.Now,
                                    MaDonViSuDung = per.DonViSuDung.MaDonViSuDung
                                };
                                db.tbAnhMoTas.Add(anhMoTa);

                                #region Tạo và lưu
                                // Tạo thư mục
                                if (!System.IO.Directory.Exists(duongDanTep.DuongDanThuMuc_BANDAU_SERVER))
                                    System.IO.Directory.CreateDirectory(duongDanTep.DuongDanThuMuc_BANDAU_SERVER);
                                // (Nếu có rồi thì xóa)
                                if (System.IO.File.Exists(duongDanTep.DuongDanTep_BANDAU_SERVER))
                                    System.IO.File.Delete(duongDanTep.DuongDanTep_BANDAU_SERVER);
                                f.SaveAs(duongDanTep.DuongDanTep_BANDAU_SERVER);
                                #endregion
                            }
                            ;

                            db.SaveChanges();
                            scope.Commit();
                        }
                    }
                    ;
                }
                //catch (Exception ex)
                //{

                //}
                catch (DbEntityValidationException ex)
                {
                    status = "error";
                    mess = ex.Message;
                    scope.Rollback();

                    foreach (var eve in ex.EntityValidationErrors)
                    {
                        Console.WriteLine($"Entity: {eve.Entry.Entity.GetType().Name} - State: {eve.Entry.State}");
                        foreach (var ve in eve.ValidationErrors)
                        {
                            Console.WriteLine($"- Property: {ve.PropertyName}, Error: {ve.ErrorMessage}");
                        }
                    }

                    throw; // hoặc return lỗi ra view
                }
            }
            return Json(new
            {
                status,
                mess
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult _create_BaiDang(HttpPostedFileBase[] files)
        {
            string status = "success";
            string mess = "Thêm mới bản ghi thành công";
            using (var scope = db.Database.BeginTransaction())
            {
                try
                {
                    if (files == null || files.Length == 0)
                    {
                        status = "error";
                        mess = "Chưa có bản ghi nào";
                    }
                    else
                    {
                        var fAnhDaiDien = Request.Files["anhdaidien"];
                        var fAnhMoTas = Request.Files["anhmota"];

                        foreach (HttpPostedFileBase f in files)
                        {
                            /**
                             * Mô tả sơ đồ tệp
                             * Ghi chú: 
                             *  - ext: loại tệp (doc|docx|xls|xlsx|png|jpg|jpeg|mp4) không bao gồm pdf
                             * ---------------------------------------------
                             * (1) |(2)   |(3)      |(4)         |(5)
                             * HoSo|
                             *     |VanBan|VanBan.ext
                             *            |CHUYENDOI|[.docx]     |VanBan.pdf
                             *                      |[.xlsx]     |VanBan.pdf
                             *                      ...
                             *            |PHIEUMUON|(Mã phiếu 1)|VanBan.pdf
                             *                      |(Mã phiếu 2)|VanBan.pdf
                             *                      ...
                             */
                            string duongDanThuMucGoc = string.Format("/Assets/uploads/{0}/TAILIEU", per.DonViSuDung.MaDonViSuDung);
                            string tenBaiDang_BANDAU = Path.GetFileName(f.FileName);
                            var duongDanTep = LayDuongDanTep(duongDanThuMucGoc: duongDanThuMucGoc, tenTep_BANDAU: tenBaiDang_BANDAU);
                            #region Nếu chưa có văn bản này thì tạo mới trong db
                            tbBaiDang baiDang_OLD = db.tbBaiDangx.FirstOrDefault(x =>
                            x.TenTepGoc == duongDanTep.TenTep_KHONGDAU_KHONGLOAI &&
                            //x.TenVanBan_BanDau == Path.GetFileNameWithoutExtension(tenVanBan_BANDAU) &&
                            x.LoaiTep == duongDanTep.LoaiTep &&
                            x.MaDonViSuDung == per.DonViSuDung.MaDonViSuDung);
                            if (baiDang_OLD != null)
                            {
                                baiDang_OLD.TrangThai = 1; // Khôi phục dữ liệu này nếu đang xóa
                            }
                            else
                            {
                                var baiDang = new tbBaiDang
                                {
                                    IdBaiDang = Guid.NewGuid(),
                                    TenTepGoc = Path.GetFileNameWithoutExtension(tenBaiDang_BANDAU),
                                    TenTepMoi = duongDanTep.TenTep_KHONGDAU_KHONGLOAI,
                                    LoaiTep = duongDanTep.LoaiTep,
                                    DuongDanTepVatLy = duongDanTep.DuongDanTep_BANDAU,
                                    TrangThai = 1,
                                    IdNguoiTao = per.NguoiDung.IdNguoiDung,
                                    NgayTao = DateTime.Now,
                                    MaDonViSuDung = per.DonViSuDung.MaDonViSuDung
                                };
                                db.tbBaiDangx.Add(baiDang);
                            }
                            ;
                            #endregion

                            #region Tạo và lưu
                            // Tạo thư mục
                            if (!System.IO.Directory.Exists(duongDanTep.DuongDanThuMuc_BANDAU_SERVER))
                                System.IO.Directory.CreateDirectory(duongDanTep.DuongDanThuMuc_BANDAU_SERVER);
                            // (Nếu có rồi thì xóa)
                            if (System.IO.File.Exists(duongDanTep.DuongDanTep_BANDAU_SERVER))
                                System.IO.File.Delete(duongDanTep.DuongDanTep_BANDAU_SERVER);
                            f.SaveAs(duongDanTep.DuongDanTep_BANDAU_SERVER);
                            #endregion
                        }
                        ;
                        db.SaveChanges();
                        scope.Commit();
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
        public JsonResult delete_BaiDang()
        {
            string status = "success";
            string mess = "Xóa bản ghi thành công";
            using (var scope = db.Database.BeginTransaction())
            {
                try
                {
                    var idBaiDangs = Request.Form["idBaiDangs"].Split(',').Select(x => Guid.Parse(x)).ToList();
                    foreach (var idBaiDang in idBaiDangs)
                    {
                        var baiDang = db.tbBaiDangx.Find(idBaiDang);
                        baiDang.TrangThai = 0;
                        baiDang.IdNguoiSua = per.NguoiDung.IdNguoiDung;
                        baiDang.NgaySua = DateTime.Now;

                        #region Xóa tệp
                        string duongDanThuMucGoc = string.Format("/Assets/uploads/{0}/TAILIEU", per.DonViSuDung.MaDonViSuDung);
                        var duongDanTep = LayDuongDanTep(duongDanThuMucGoc: duongDanThuMucGoc, tenTep_BANDAU: baiDang.TenTepGoc);
                        if (System.IO.Directory.Exists(duongDanTep.DuongDanThuMuc_BANDAU_SERVER))
                            System.IO.Directory.Delete(duongDanTep.DuongDanThuMuc_BANDAU_SERVER, true);
                        #endregion
                    }
                    ;
                    db.SaveChanges();
                    scope.Commit();
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