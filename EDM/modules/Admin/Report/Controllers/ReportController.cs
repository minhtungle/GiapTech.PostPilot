using DocumentDirectory.Controllers;
using DocumentFormation.Models;
using EDM_DB;
using Newtonsoft.Json;
using Organization.Controllers;
using Public.Controllers;
using Public.Models;
using Report.Models;
using StorageLocation.Controllers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using UserType.Models;

namespace Report.Controllers
{
    public class ReportController : RouteConfigController
    {
        #region Biến public để in hoa
        private readonly string VIEW_PATH = "~/Views/Admin/Report";
        private List<tbDonViSuDung_PhongLuuTru> PHONGLUUTRUS
        {
            get
            {
                return Session["PHONGLUUTRUS"] as List<tbDonViSuDung_PhongLuuTru> ?? new List<tbDonViSuDung_PhongLuuTru>();
            }
            set
            {
                Session["PHONGLUUTRUS"] = value;
            }
        }
        private List<default_tbCheDoSuDung> CHEDOSUDUNGS
        {
            get
            {
                return Session["CHEDOSUDUNGS"] as List<default_tbCheDoSuDung> ?? new List<default_tbCheDoSuDung>();
            }
            set
            {
                Session["CHEDOSUDUNGS"] = value;
            }
        }
        private List<tbViTriLuuTru> VITRILUUTRUS
        {
            get
            {
                return Session["VITRILUUTRUS"] as List<tbViTriLuuTru> ?? new List<tbViTriLuuTru>();
            }
            set
            {
                Session["VITRILUUTRUS"] = value;
            }
        }
        private List<tbDanhMucHoSo> DANHMUCHOSOS
        {
            get
            {
                return Session["DANHMUCHOSOS"] as List<tbDanhMucHoSo> ?? new List<tbDanhMucHoSo>();
            }
            set
            {
                Session["DANHMUCHOSOS"] = value;
            }
        }
        private List<tbCoCauToChuc> COCAUTOCHUCS
        {
            get
            {
                return Session["COCAUTOCHUCS"] as List<tbCoCauToChuc> ?? new List<tbCoCauToChuc>();
            }
            set
            {
                Session["COCAUTOCHUCS"] = value;
            }
        }
        private List<Tree<tbViTriLuuTru>> VITRILUUTRUS_TREE
        {
            get
            {
                return Session["VITRILUUTRUS_TREE"] as List<Tree<tbViTriLuuTru>> ?? new List<Tree<tbViTriLuuTru>>();
            }
            set
            {
                Session["VITRILUUTRUS_TREE"] = value;
            }
        }
        private List<Tree<tbDanhMucHoSo>> DANHMUCHOSOS_TREE
        {
            get
            {
                return Session["DANHMUCHOSOS_TREE"] as List<Tree<tbDanhMucHoSo>> ?? new List<Tree<tbDanhMucHoSo>>();
            }
            set
            {
                Session["DANHMUCHOSOS_TREE"] = value;
            }
        }
        private List<Tree<tbCoCauToChuc>> COCAUTOCHUCS_TREE
        {
            get
            {
                return Session["COCAUTOCHUCS_TREE"] as List<Tree<tbCoCauToChuc>> ?? new List<Tree<tbCoCauToChuc>>();
            }
            set
            {
                Session["COCAUTOCHUCS_TREE"] = value;
            }
        }
        private static StorageLocationController storageLocationController = new StorageLocationController();
        private static DocumentDirectoryController documentDirectoryController = new DocumentDirectoryController();
        private static OrganizationController organizationController = new OrganizationController();
        #endregion
        public ActionResult Index()
        {
            #region Lấy các danh sách 
            #region Hồ sơ & Văn bản & Dữ liệu số & Phiếu mượn
            List<tbHoSoExtend> hoSos = get_HoSos(loai: "all");
            List<tbHoSo_VanBan> vanBans = db.tbHoSo_VanBan.Where(x => x.TrangThai != 0 && x.MaDonViSuDung == per.DonViSuDung.MaDonViSuDung).ToList() ?? new List<tbHoSo_VanBan>();
            List<tbPhieuMuon> phieuMuons = db.tbPhieuMuons.Where(x => x.TrangThai != 0 && x.MaDonViSuDung == per.DonViSuDung.MaDonViSuDung).ToList() ?? new List<tbPhieuMuon>();
            List<tbHoSo_VanBan_DuLieuSo> duLieuSos = db.tbHoSo_VanBan_DuLieuSo.Where(x => x.TrangThai == 1 && x.MaDonViSuDung == per.DonViSuDung.MaDonViSuDung).ToList() ?? new List<tbHoSo_VanBan_DuLieuSo>();
            List<tbHoSo_VanBan_DuLieuSo> duLieuSos_BangBieu = new List<tbHoSo_VanBan_DuLieuSo>();
            List<tbHoSo_VanBan_DuLieuSo> duLieuSos_VanBan = new List<tbHoSo_VanBan_DuLieuSo>();
            foreach (tbHoSo_VanBan_DuLieuSo duLieuSo in duLieuSos)
            {
                if (duLieuSo.DuLieuSo.Trim() != "")
                {
                };
                tbBieuMau bieuMau = db.tbBieuMaus.Find(duLieuSo.IdBieuMau);
                if (bieuMau != null)
                {
                    if (bieuMau.IdLoaiBieuMau == 1)
                    {
                        duLieuSos_BangBieu.Add(duLieuSo);
                    }
                    else
                    {
                        duLieuSos_VanBan.Add(duLieuSo);
                    };
                };
            };
            #endregion
            #region Phông lưu trữ
            List<tbDonViSuDung_PhongLuuTru> phongLuuTrus = db.tbDonViSuDung_PhongLuuTru.Where(x => x.MaDonViSuDung == per.DonViSuDung.MaDonViSuDung && x.TrangThai == 1).ToList() ?? new List<tbDonViSuDung_PhongLuuTru>();
            #endregion
            #region Chế độ sử dụng
            List<default_tbCheDoSuDung> cheDoSuDungs = db.default_tbCheDoSuDung.Where(x => x.TrangThai == 1).ToList() ?? new List<default_tbCheDoSuDung>();
            #endregion
            #region Vị trí lưu trữ
            List<tbViTriLuuTru> viTriLuuTrus = new List<tbViTriLuuTru>();
            List<Tree<tbViTriLuuTru>> viTriLuuTrus_Tree = storageLocationController.get_ViTriLuuTrus_Tree(idViTri: 0, maDonViSuDung: per.DonViSuDung.MaDonViSuDung).nodes;
            storageLocationController.xuLy_TenViTriLuuTru(viTris_IN: viTriLuuTrus_Tree, viTris_OUT: viTriLuuTrus, khoangCachDauDong: "");
            #endregion
            #region Danh mục hồ sơ
            List<tbDanhMucHoSo> danhMucHoSos = new List<tbDanhMucHoSo>();
            List<Tree<tbDanhMucHoSo>> danhMucHoSos_Tree = documentDirectoryController.get_DanhMucHoSos_Tree(idDanhMuc: 0, maDonViSuDung: per.DonViSuDung.MaDonViSuDung).nodes;
            documentDirectoryController.xuLy_TenDanhMucHoSo(danhMucs_IN: danhMucHoSos_Tree, danhMucs_OUT: danhMucHoSos, khoangCachDauDong: "");
            #endregion
            #region Cơ cấu tổ chức
            List<tbCoCauToChuc> coCauToChucs = new List<tbCoCauToChuc>();
            List<Tree<tbCoCauToChuc>> coCauToChucs_Tree = organizationController.get_CoCauToChucs_Tree(idCoCau: 0, maDonViSuDung: per.DonViSuDung.MaDonViSuDung).nodes;
            organizationController.xuLy_TenCoCauToChuc(coCaus_IN: coCauToChucs_Tree, coCaus_OUT: coCauToChucs, khoangCachDauDong: "");
            #endregion
            #region Thao tác
            List<ChucNangs> kieuNguoiDung_IdChucNang = JsonConvert.DeserializeObject<List<ChucNangs>>(per.KieuNguoiDung.IdChucNang);
            #endregion
            #endregion
            PHONGLUUTRUS = phongLuuTrus;
            CHEDOSUDUNGS = cheDoSuDungs;
            VITRILUUTRUS = viTriLuuTrus; VITRILUUTRUS_TREE = viTriLuuTrus_Tree;
            DANHMUCHOSOS = danhMucHoSos; DANHMUCHOSOS_TREE = danhMucHoSos_Tree;

            ViewBag.kieuNguoiDung_IdChucNang = kieuNguoiDung_IdChucNang;
            ViewBag.hoSos = hoSos;
            ViewBag.vanBans = vanBans;
            ViewBag.duLieuSos = duLieuSos;
            ViewBag.duLieuSos_BangBieu = duLieuSos_BangBieu;
            ViewBag.duLieuSos_VanBan = duLieuSos_VanBan;
            ViewBag.phieuMuons = phieuMuons;

            ViewBag.phongLuuTrus = PHONGLUUTRUS;
            ViewBag.cheDoSuDungs = CHEDOSUDUNGS;
            ViewBag.viTriLuuTrus = VITRILUUTRUS; ViewBag.VITRILUUTRUS_TREE = VITRILUUTRUS_TREE;
            ViewBag.danhMucHoSos = DANHMUCHOSOS; ViewBag.DANHMUCHOSOS_TREE = DANHMUCHOSOS_TREE;
            return View($"{VIEW_PATH}/report-trangchu.cshtml");
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
        public ActionResult ThongKeTheoTieuChi()
        {
            return View($"{VIEW_PATH}/report.thongketheotieuchi/thongketheotieuchi.cshtml");
        }
        [HttpPost]
        public JsonResult timKiem()
        {
            string _thongTinTimKiem = Request.Form["thongTinTimKiem"];
            ThongTinTimKiemM thongTinTimKiem = JsonConvert.DeserializeObject<ThongTinTimKiemM>(_thongTinTimKiem);
            List<tbHoSoExtend> hoSos = get_HoSos(loai: "timkiem", thongTinTimKiem: thongTinTimKiem);
            KetQuaTimKiemM model = new KetQuaTimKiemM
            {
                HoSos = hoSos,
                DanhMucHoSos = DANHMUCHOSOS,
                ViTriLuuTrus = VITRILUUTRUS,
                CoCauToChucs = COCAUTOCHUCS,
            };
            // Lấy danh sách thông tin tìm kiếm
            if (thongTinTimKiem.IdDanhMucHoSos.Count != 0)
            {
                model.DanhMucHoSos = DANHMUCHOSOS.Where(
                x => thongTinTimKiem.IdDanhMucHoSos.Contains(x.IdDanhMucHoSo)).ToList();
            };
            if (thongTinTimKiem.IdViTriLuuTrus.Count != 0)
            {
                model.ViTriLuuTrus = VITRILUUTRUS.Where(
                x => thongTinTimKiem.IdViTriLuuTrus.Contains(x.IdViTriLuuTru)).ToList();
            };
            if (thongTinTimKiem.IdCoCauToChucs.Count != 0)
            {
                model.CoCauToChucs = COCAUTOCHUCS.Where(
                x => thongTinTimKiem.IdCoCauToChucs.Contains(x.IdCoCauToChuc)).ToList();
            };
            string danhmuchoso_view = Public.Handle.RenderViewToString(
                controller: this,
                viewName: $"{VIEW_PATH}/report.thongketheotieuchi/danhmuchoso/danhmuchoso-ketquatimkiem.cshtml",
                model: model);
            string vitriluutru_view = Public.Handle.RenderViewToString(
                controller: this,
                viewName: $"{VIEW_PATH}/report.thongketheotieuchi/vitriluutru/vitriluutru-ketquatimkiem.cshtml",
                model: model);
            string cocautochuc_view = Public.Handle.RenderViewToString(
                controller: this,
                viewName: $"{VIEW_PATH}/report.thongketheotieuchi/cocautochuc/cocautochuc-ketquatimkiem.cshtml",
                model: model);
            return Json(new
            {
                danhmuchoso_view,
                vitriluutru_view,
                cocautochuc_view
            }, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public ActionResult displayModal_TimKiem_TheoTieuChi()
        {
            #region Tạo sơ đồ cây
            string _hienthi<T>(List<Tree<T>> objs, int capDo)
            {
                string html = default(string);
                string khoangCach = String.Join("", Enumerable.Repeat("<span class=\"ms-4 me-4\"></span>", (int)capDo).ToArray());
                foreach (Tree<T> obj in objs)
                {
                    T _obj = obj.root;
                    Type type = _obj.GetType();
                    string typeName = type.Name;

                    int id = 0, idCha = (int)type.GetProperty("IdCha").GetValue(_obj);
                    string ten;

                    if (typeName == "tbDanhMucHoSo")
                    {
                        id = (int)type.GetProperty("IdDanhMucHoSo").GetValue(_obj);
                        ten = type.GetProperty("TenDanhMucHoSo").GetValue(_obj).ToString();
                    }
                    else if (typeName == "tbViTriLuuTru")
                    {
                        id = (int)type.GetProperty("IdViTriLuuTru").GetValue(_obj);
                        ten = type.GetProperty("TenViTriLuuTru").GetValue(_obj).ToString();
                    }
                    else
                    {
                        id = (int)type.GetProperty("IdCoCauToChuc").GetValue(_obj);
                        ten = type.GetProperty("TenCoCauToChuc").GetValue(_obj).ToString();
                    };
                    html += $"<tr id=\"{id}\">" +
                        $"<td>" +
                        $"  {khoangCach}<input type=\"checkbox\" class=\"form-check-input ms-3 me-3\" data-id=\"{id}\" data-idcha=\"{idCha}\" data-capdo=\"{capDo}\"/> {ten}" +
                        $"</td>" +
                    "</tr>" + _hienthi(obj.nodes, capDo + 1);
                }
                return html;
            };
            ViewBag.HtmlDanhMucHoSos = _hienthi(DANHMUCHOSOS_TREE, 0);
            ViewBag.HtmlViTriLuuTrus = _hienthi(VITRILUUTRUS_TREE, 0);
            ViewBag.HtmlCoCauToChucs = _hienthi(COCAUTOCHUCS_TREE, 0);
            #endregion
            ViewBag.PHONGLUUTRUS = PHONGLUUTRUS;
            return PartialView($"{VIEW_PATH}/report.thongketheotieuchi/thongketheotieuchi-timkiem.cshtml");
        }
        public List<tbHoSoExtend> get_HoSos(string loai = "all", ThongTinTimKiemM thongTinTimKiem = null, string idHoSos = "")
        {
            List<tbHoSoExtend> hoSos = new List<tbHoSoExtend>();
            if (loai == "timkiem")
            {
                // Xử lý dữ liệu
                thongTinTimKiem.MaHoSo = thongTinTimKiem.MaHoSo ?? "";
                thongTinTimKiem.TieuDeHoSo = thongTinTimKiem.TieuDeHoSo ?? "";
                thongTinTimKiem.So_KyHieu = thongTinTimKiem.So_KyHieu ?? "";
                thongTinTimKiem.ThoiHanBaoQuan = thongTinTimKiem.ThoiHanBaoQuan ?? "";
                thongTinTimKiem._NguoiTao.TenNguoiDung = thongTinTimKiem._NguoiTao.TenNguoiDung ?? "";
                thongTinTimKiem._DonViSuDung.TenDonViSuDung = thongTinTimKiem._DonViSuDung.TenDonViSuDung ?? "";
                thongTinTimKiem.GhiChu = thongTinTimKiem.GhiChu ?? "";

                thongTinTimKiem.IdDanhMucHoSo = thongTinTimKiem.IdDanhMucHoSo ?? 0;
                thongTinTimKiem.IdPhongLuuTru = thongTinTimKiem.IdPhongLuuTru ?? 0;
                thongTinTimKiem.SoLuongTo = thongTinTimKiem.SoLuongTo ?? 0;
                thongTinTimKiem.SoLuongTrang = thongTinTimKiem.SoLuongTrang ?? 0;
                tbNguoiDung _nguoiTao = new tbNguoiDung();
                if (thongTinTimKiem._NguoiTao.TenNguoiDung != "")
                {
                    _nguoiTao = db.tbNguoiDungs.FirstOrDefault(x => x.TenNguoiDung.Contains(thongTinTimKiem._NguoiTao.TenNguoiDung)) ?? new tbNguoiDung();
                };
                tbDonViSuDung _donViSuDung = new tbDonViSuDung();
                if (thongTinTimKiem._DonViSuDung.TenDonViSuDung != "")
                {
                    _donViSuDung = db.tbDonViSuDungs.FirstOrDefault(x => x.TenDonViSuDung.Contains(thongTinTimKiem._DonViSuDung.TenDonViSuDung)) ?? new tbDonViSuDung();
                };
                // Tìm kiếm
                string timKiemSQL = $@"
                select * 
                from tbHoSo 
                where 
                    MaDonViSuDung = {per.DonViSuDung.MaDonViSuDung} and TrangThai <> 0 
                    {(thongTinTimKiem.IdDanhMucHoSo == 0 ? "" : $"and IdDanhMucHoSo = {thongTinTimKiem.IdDanhMucHoSo}")}
                    {(thongTinTimKiem.IdPhongLuuTru == 0 ? "" : $"and IdPhongLuuTru = {thongTinTimKiem.IdPhongLuuTru}")}

                    {(thongTinTimKiem.MaHoSo == "" ? "" : $"and MaHoSo like '%{thongTinTimKiem.MaHoSo}%'")}
                    {(thongTinTimKiem.TieuDeHoSo == "" ? "" : $"and TieuDeHoSo like '%{thongTinTimKiem.TieuDeHoSo}%'")}
                    {(thongTinTimKiem.So_KyHieu == "" ? "" : $"and So_KyHieu like '%{thongTinTimKiem.So_KyHieu}%'")}

                    {(thongTinTimKiem.ThoiGianBatDau == null ? "" : $"and ThoiGianBatDau like '{thongTinTimKiem.ThoiGianBatDau.Value.ToString("dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture)}%'")}
                    {(thongTinTimKiem.ThoiGianKetThuc == null ? "" : $"and ThoiGianKetThuc like '{thongTinTimKiem.ThoiGianKetThuc.Value.ToString("dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture)}%'")}
                    {(thongTinTimKiem.SoLuongTo == 0 ? "" : $"and SoLuongTo = {thongTinTimKiem.SoLuongTo}")}
                    {(thongTinTimKiem.SoLuongTrang == 0 ? "" : $"and SoLuongTrang = {thongTinTimKiem.SoLuongTrang}")}
                    {(thongTinTimKiem.ThoiHanBaoQuan == "" ? "" : $"and ThoiHanBaoQuan = '{thongTinTimKiem.ThoiHanBaoQuan}'")}
                    {(_nguoiTao.IdNguoiDung == 0 ? "" : $"and NguoiTao = {_nguoiTao.IdNguoiDung}")}
                    {(_donViSuDung.MaDonViSuDung == 0 ? "" : $"and MaDonViSuDung = '{_donViSuDung.MaDonViSuDung}'")}
                    {(thongTinTimKiem.GhiChu == "" ? "" : $"and GhiChu = '{thongTinTimKiem.GhiChu}'")}
                order by NgayTao desc";
                hoSos = db.Database.SqlQuery<tbHoSoExtend>(timKiemSQL).ToList() ?? new List<tbHoSoExtend>();
            }
            else if (loai == "byId")
            {
                if (idHoSos != "")
                {
                    string getSql = $@"select * from tbHoSo where MaDonViSuDung = {per.DonViSuDung.MaDonViSuDung} and TrangThai <> 0 and IdHoSo in ({idHoSos}) order by NgayTao desc";
                    hoSos = db.Database.SqlQuery<tbHoSoExtend>(getSql).ToList() ?? new List<tbHoSoExtend>();
                };
            }
            else
            {
                string getSql = $@"select * from tbHoSo where MaDonViSuDung = {per.DonViSuDung.MaDonViSuDung} and TrangThai <> 0 order by NgayTao desc";
                hoSos = db.Database.SqlQuery<tbHoSoExtend>(getSql).ToList() ?? new List<tbHoSoExtend>();
            }
            foreach (tbHoSoExtend hoSo in hoSos)
            {
                // Lấy danh mục từ danh sách danh mục đã xử lý tên
                //hoSo.DanhMucHoSo = db.tbDanhMucHoSoes.Find(hoSo.IdDanhMucHoSo) ?? new tbDanhMucHoSo();
                //hoSo.ViTriLuuTru = db.tbViTriLuuTrus.Find(hoSo.IdViTriLuuTru) ?? new tbViTriLuuTru();
                //hoSo.PhongLuuTru = db.tbDonViSuDung_PhongLuuTru.Find(hoSo.IdPhongLuuTru) ?? new tbDonViSuDung_PhongLuuTru();
                hoSo.PhongLuuTru = PHONGLUUTRUS.Where(x => x.IdPhongLuuTru == hoSo.IdPhongLuuTru).FirstOrDefault() ?? new tbDonViSuDung_PhongLuuTru();
                hoSo.DanhMucHoSo = DANHMUCHOSOS.Where(x => x.IdDanhMucHoSo == hoSo.IdDanhMucHoSo).FirstOrDefault() ?? new tbDanhMucHoSo();
                hoSo.ViTriLuuTru = VITRILUUTRUS.Where(x => x.IdViTriLuuTru == hoSo.IdViTriLuuTru).FirstOrDefault() ?? new tbViTriLuuTru();
                hoSo.CheDoSuDung = db.default_tbCheDoSuDung.Find(hoSo.IdCheDoSuDung) ?? new default_tbCheDoSuDung();
                hoSo.ThongTinNguoiTao = db.tbNguoiDungs.Find(hoSo.NguoiTao) ?? new tbNguoiDung();
                hoSo.DonViSuDung = db.tbDonViSuDungs.Find(hoSo.MaDonViSuDung) ?? new tbDonViSuDung();
            }
            return hoSos;
        }
    }
}