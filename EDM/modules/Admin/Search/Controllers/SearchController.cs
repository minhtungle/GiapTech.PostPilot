using DocumentFormation.Models;
using EDM_DB;
using Newtonsoft.Json;
using Public.Controllers;
using Search.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;

namespace Search.Controllers
{
    public class SearchController : StaticArgController
    {
        #region Biến public để in hoa
        private readonly string VIEW_PATH = "~/Views/Admin/__Home/Search";
        private List<VanBanTimKiemsM> VANBANTIMKIEMS
        {
            get
            {
                return Session["VANBANTIMKIEMS"] as List<VanBanTimKiemsM> ?? new List<VanBanTimKiemsM>();
            }
            set
            {
                Session["VANBANTIMKIEMS"] = value;
            }
        }
        private List<tbBieuMauExtend> BIEUMAUs
        {
            get
            {
                return Session["BIEUMAUs"] as List<tbBieuMauExtend> ?? new List<tbBieuMauExtend>();
            }
            set
            {
                Session["BIEUMAUs"] = value;
            }
        }
        #endregion
        public ActionResult Index()
        {
            /**
             * Chuyển từ RouteConfigController sang để tránh việc gọi hàm trong controller này từ vai trò người dùng không vượt qua được kiểm tra quyền
             * */
            if (per.NguoiDung.IdNguoiDung == 0)
            {
                return new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Auth", action = "Login" }));
            };
            return new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Home", action = "Index" }));
        }
        [HttpPost]
        public ActionResult getList(string str_thongTinTimKiem)
        {
            VANBANTIMKIEMS = new List<VanBanTimKiemsM>();
            thongTinTimKiem thongTinTimKiem = JsonConvert.DeserializeObject<thongTinTimKiem>(str_thongTinTimKiem);
            /**
             * Tìm kiếm theo từng loại trường dữ liệu
             */
            foreach (thongTinTimKiem_data d in thongTinTimKiem.data)
            {
                string str_IdTruongDuLieus = d.str_IdTruongDuLieus;
                string tenTruong = d.TenTruong;
                string noiDungTimKiem = d.NoiDungTimKiem;
                int idViTriLuuTru = d.IdViTriLuuTru;
                int idDanhMucHoSo = d.IdDanhMucHoSo;
                int idPhongLuuTru = d.IdPhongLuuTru;
                if (noiDungTimKiem != "")
                {
                    #region Cách 1: NHANH - Đang dùng
                    // Phần select
                    string selectSQL = $@"
                    select 
                        hoSo.IdHoSo, hoSo.MaHoSo, hoSo.TieuDeHoSo, hoSo.GhiChu, hoSo.QuyenTruyCap,
                        isnull(vanBan.IdVanBan, 0) as IdVanBan, isnull(vanBan.TenVanBan, '') as TenVanBan, vanBan.TenVanBan_BanDau, vanBan.Loai,
                        isnull(cheDoSuDung.TenCheDoSuDung, '') as TenCheDoSuDung, cheDoSuDung.IdCheDoSuDung,
                        string_agg (isnull(duLieuSo.TrangSo, 1), ',') as TrangSo
                    ";
                    // Phần join
                    string joinSQL = $@"
                    from tbHoSo hoSo
		                left join tbHoSo_VanBan vanBan on vanBan.IdHoSo = hoSo.IdHoSo and vanBan.TrangThai = 1 and vanBan.MaDonViSuDung = {per.DonViSuDung.MaDonViSuDung}
                        left join tbHoSo_VanBan_DuLieuSo duLieuSo on duLieuSo.IdVanBan = vanBan.IdVanBan and duLieuSo.TrangThai = 1 and duLieuSo.MaDonViSuDung = {per.DonViSuDung.MaDonViSuDung}

					    left join tbBieuMau_TruongDuLieu truongDuLieu on truongDuLieu.IdTruongDuLieu = duLieuSo.IdTruongDuLieu and truongDuLieu.MaDonViSuDung = {per.DonViSuDung.MaDonViSuDung}
                        left join default_tbCheDoSuDung cheDoSuDung on cheDoSuDung.IdCheDoSuDung = hoSo.IdCheDoSuDung";
                    // Phần where
                    string whereSQL = $@"
                    where 
                        hoSo.TrangThai = 2 and hoSo.MaDonViSuDung = {per.DonViSuDung.MaDonViSuDung}
                    ";
                    // Phần groupBy
                    string groupBySQL = $@"
                    group by 
                        vanBan.IdVanBan, vanBan.TenVanBan, vanBan.TenVanBan_BanDau, vanBan.Loai,
                        hoSo.IdHoSo, hoSo.MaHoSo, hoSo.TieuDeHoSo, hoSo.GhiChu, hoSo.QuyenTruyCap, TenCheDoSuDung, cheDoSuDung.IdCheDoSuDung";
                    #region Điều kiện tìm kiếm
                    if (thongTinTimKiem.loai == "nangcao")
                    {
                        //whereSQL += $@"
                        //and duLieuSo.DuLieuSo like N'%{noiDungTimKiem}%' and truongDuLieu.IdTruongDuLieu in ({str_IdTruongDuLieus})
                        //";
                        whereSQL += $@"
                        and duLieuSo.DuLieuSo like N'%' + @noiDungTimKiem + N'%' and truongDuLieu.IdTruongDuLieu in ({str_IdTruongDuLieus})
                        ";
                        // Kiểm tra điều kiện
                        List<string> sql = new List<string>();
                        if (idViTriLuuTru != 0)
                            sql.Add($@"hoSo.IdViTriLuuTru = {idViTriLuuTru}");
                        if (idDanhMucHoSo != 0)
                            sql.Add($@"hoSo.IdDanhMucHoSo = {idDanhMucHoSo}");
                        if (idPhongLuuTru != 0)
                            sql.Add($@"hoSo.IdPhongLuuTru = {idPhongLuuTru}");
                        if (sql.Count > 0)
                            whereSQL += " and " + string.Join(" and ", sql);
                        else
                            whereSQL += string.Join(" and ", sql);
                    }
                    else
                    {
                        // Kiểm tra điều kiện
                        List<string> sql = new List<string>();
                        if (d.TieuChiTimKiem.Contains("MaHoSo"))
                            //sql.Add($@"hoSo.MaHoSo like N'%{noiDungTimKiem}%'");
                            sql.Add($@"hoSo.MaHoSo like N'%' + @noiDungTimKiem + N'%'");
                        if (d.TieuChiTimKiem.Contains("TieuDeHoSo"))
                            //sql.Add($@"hoSo.TieuDeHoSo like N'%{noiDungTimKiem}%'");
                            sql.Add($@"hoSo.TieuDeHoSo like N'%' + @noiDungTimKiem + N'%'");
                        if (d.TieuChiTimKiem.Contains("TenVanBan"))
                            //sql.Add($@"vanBan.TenVanBan like N'%{noiDungTimKiem}%'");
                            sql.Add($@"(vanBan.TenVanBan like N'%' + @noiDungTimKiem + '%' or vanBan.TenVanBan_BanDau like N'%' + @noiDungTimKiem + '%')");
                        if (d.TieuChiTimKiem.Contains("DuLieuSo"))
                            //sql.Add($@"duLieuSo.DuLieuSo like N'%{noiDungTimKiem}%'");
                            sql.Add($@"duLieuSo.DuLieuSo like N'%' + @noiDungTimKiem + N'%'");
                        whereSQL += string.Format("and ({0})", string.Join(" or ", sql));
                    };
                    #endregion
                    string timKiemSQL = $@"
                    {selectSQL}
                    {joinSQL}
                    {whereSQL}
                    {groupBySQL}";
                    List<VanBanTimKiemsM> ketQua = db.Database.SqlQuery<VanBanTimKiemsM>(
                        timKiemSQL,
                        new SqlParameter("@noiDungTimKiem", noiDungTimKiem)
                        ).ToList() ?? new List<VanBanTimKiemsM>();
                    /**
                     * Thêm kết quả vào danh sách chung
                     */
                    VANBANTIMKIEMS.AddRange(ketQua);
                    #endregion
                };
            };
            string viewAsString = Public.Handle.RenderViewToString(this, $"{VIEW_PATH}/search.timkiemhoso/timkiemhoso-ketqua.cshtml", VANBANTIMKIEMS);
            return Json(new
            {
                vanBans = VANBANTIMKIEMS,
                view = viewAsString,
            }, JsonRequestBehavior.AllowGet);
        }
        #region Văn bản
        public ActionResult VanBan(int idHoSo = 0)
        {
            tbHoSoExtend hoSo = get_HoSo(idHoSo: idHoSo, "all");
            ViewBag.hoSo = hoSo;
            return View($"{VIEW_PATH}/search.timkiemhoso/vanban.cshtml");
        }

        public tbHoSoExtend get_HoSo(int idHoSo, string loai, string idVanBans = "")
        {
            // Thông tin hồ sơ đang chọn
            tbHoSoExtend hoSo = db.Database.SqlQuery<tbHoSoExtend>($@"
            select * from tbHoSo where IdHoSo = {idHoSo}").FirstOrDefault() ?? new tbHoSoExtend();
            if (hoSo.IdHoSo != 0)
            {
                // Thông tin vị trí lưu trữ - danh mục hồ sơ - phông lưu trữ - chế độ sử dụng
                hoSo.ViTriLuuTru = db.tbViTriLuuTrus.Find(hoSo.IdViTriLuuTru) ?? new tbViTriLuuTru();
                hoSo.DanhMucHoSo = db.tbDanhMucHoSoes.Find(hoSo.IdDanhMucHoSo) ?? new tbDanhMucHoSo();
                hoSo.PhongLuuTru = db.tbDonViSuDung_PhongLuuTru.Find(hoSo.IdPhongLuuTru) ?? new tbDonViSuDung_PhongLuuTru();
                hoSo.CheDoSuDung = db.default_tbCheDoSuDung.Find(hoSo.IdCheDoSuDung) ?? new default_tbCheDoSuDung();
                // Thông tin văn bản của hồ sơ
                string get_VanBanSQL = "";
                if (loai == "all")
                {
                    get_VanBanSQL = $@"select * from tbHoSo_VanBan where TrangThai = 1 and MaDonViSuDung = {per.DonViSuDung.MaDonViSuDung} and IdHoSo = {idHoSo}";
                }
                else if (idVanBans != "")
                {
                    get_VanBanSQL = $@"select * from tbHoSo_VanBan where TrangThai = 1 and MaDonViSuDung = {per.DonViSuDung.MaDonViSuDung} and IdVanBan in ({idVanBans})";
                }
                List<tbHoSo_VanBanExtend> vanBans = db.Database.SqlQuery<tbHoSo_VanBanExtend>(get_VanBanSQL).ToList() ?? new List<tbHoSo_VanBanExtend>();
                // Lấy thông tin biểu mẫu của văn bản
                foreach (tbHoSo_VanBanExtend vanBan in vanBans)
                {
                    string get_BieuMauSQL = $@"select * from tbBieuMau where TrangThai = 1 and MaDonViSuDung = {per.DonViSuDung.MaDonViSuDung} and IdBieuMau = {vanBan.IdBieuMau}";
                    vanBan.BieuMau = db.Database.SqlQuery<tbBieuMauExtend>(get_BieuMauSQL).FirstOrDefault() ?? new tbBieuMauExtend();
                    #region Lấy đường dẫn văn bản
                    string tenVanBan_BANDAU = string.Format("{0}{1}", Path.GetFileName(vanBan.TenVanBan), vanBan.Loai);
                    var duongDanVanBan = LayDuongDanVanBan(duongDanHoSo: hoSo.DuongDanFile, tenVanBan_BANDAU: tenVanBan_BANDAU);

                    vanBan.DuongDan = duongDanVanBan.duongDanVanBan_BANDAU;
                    if (vanBan.Loai.Contains("xls") || vanBan.Loai.Contains("doc"))
                    {
                        vanBan.DuongDan = duongDanVanBan.duongDanVanBan_CHUYENDOI;
                    };
                    #endregion
                };
                hoSo.VanBans = vanBans;
            }
            return hoSo;
        }
        [HttpPost]
        public ActionResult displayModal_Read_VanBan(int idHoSo, int idVanBan, int trangSo = 1)
        {
            /**
             * Tìm văn bản
             */
            tbHoSo_VanBanExtend vanBan = get_HoSo(idHoSo: idHoSo, loai: "single", idVanBans: idVanBan.ToString()).VanBans.FirstOrDefault() ?? new tbHoSo_VanBanExtend();
            // Lấy tên miền
            Uri uri = new Uri(HttpContext.Request.Url.AbsoluteUri);
            string hostName = uri.GetLeftPart(UriPartial.Authority);
            ViewBag.hostName = hostName;
            ViewBag.vanBan = vanBan;
            if (vanBan.Loai.Contains("pdf") || vanBan.Loai.Contains("xls") || vanBan.Loai.Contains("doc"))
                ViewBag.iframeHtml = $"<iframe src=\"{hostName}/Search/xemPDF?idHoSo={idHoSo}&idVanBan={idVanBan}\" title=\"Chi tiết\" style=\"width: 100%;height: 70vh\"></iframe>";
            else if (vanBan.Loai.Contains("mp4"))
                ViewBag.iframeHtml = $"<video src=\"{vanBan.DuongDan}\" controls style=\"width: 100%; height: 70vh; border: 1px solid var(--bs-body-color)\"></video>";
            else
                ViewBag.iframeHtml = $"<iframe src=\"{vanBan.DuongDan}\" title=\"Chi tiết\" style=\"width: 100%;height: 70vh; border: 1px solid var(--bs-body-color)\"></iframe>";
            return PartialView($"{VIEW_PATH}/search.timkiemhoso/vanban.detail.cshtml");
        }
        [HttpGet]
        public ActionResult getList_TruongDuLieus()
        {
            var _maDonVi = new SqlParameter("@maDonViSuDung", per.DonViSuDung.MaDonViSuDung);
            List<thongTinTimKiem_data> truongDuLieus = db.Database.SqlQuery<thongTinTimKiem_data>("dbo.Lay_TruongDuLieus @maDonViSuDung", _maDonVi).ToList() ?? new List<thongTinTimKiem_data>();
            return Json(new
            {
                data = truongDuLieus
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult get_DuLieuSos(int idVanBan = 0, int idBieuMau = 0)
        {
            tbBieuMau bieuMau = db.tbBieuMaus.Find(idBieuMau);
            /**
             * Tìm trường dữ liệu của biểu mẫu
             */
            List<tbBieuMau_TruongDuLieuExtend> truongDuLieus = db.Database.SqlQuery<tbBieuMau_TruongDuLieuExtend>(
                $@"select truongdulieu.* from tbBieuMau_TruongDuLieu truongdulieu  
                where truongdulieu.IdBieuMau = {idBieuMau} and truongdulieu.TrangThai = 1 and truongdulieu.MaDonViSuDung = {per.DonViSuDung.MaDonViSuDung}"
                ).ToList() ?? new List<tbBieuMau_TruongDuLieuExtend>();
            /**
             * Tìm dữ liệu số từng trường thuộc văn bản và sắp xếp theo nhóm
             * |---------------------------------------|
             * |   TRUONG1   |   TRUONG2  |   TRUONG3  |
             * |---------------------------------------|
             * |  DULIEUSO1  |  DULIEUSO1 |  DULIEUSO1 |
             * |  DULIEUSO2  |  DULIEUSO2 |  DULIEUSO2 |
             * |  DULIEUSO3  |  DULIEUSO3 |  DULIEUSO3 |
             * |  DULIEUSO4  |  DULIEUSO4 |  DULIEUSO4 |
             * |---------------------------------------|
             */
            foreach (tbBieuMau_TruongDuLieuExtend truongDuLieu in truongDuLieus)
            {
                truongDuLieu.DuLieuSos = db.tbHoSo_VanBan_DuLieuSo.Where(x => x.IdTruongDuLieu == truongDuLieu.IdTruongDuLieu && x.IdVanBan == idVanBan && x.TrangThai == 1 && x.MaDonViSuDung == per.DonViSuDung.MaDonViSuDung).OrderBy(x => x.Nhom).ThenBy(x => x.NgayTao).ToList() ?? new List<tbHoSo_VanBan_DuLieuSo>();
            };

            ViewBag.truongDuLieus = truongDuLieus;
            ViewBag.bieuMau = bieuMau;
            return PartialView($"{VIEW_PATH}/search.timkiemhoso/vanban.detail.truongdulieu.cshtml");
        }
        public ActionResult xemPDF(int idHoSo = 0, int idVanBan = 0)
        {
            tbHoSoExtend hoSo = get_HoSo(idHoSo: idHoSo, loai: "single", idVanBans: idVanBan.ToString()) ?? new tbHoSoExtend();
            if (hoSo == null) return View();
            tbHoSo_VanBanExtend vanBan = hoSo.VanBans.FirstOrDefault();
            if (vanBan == null) return View();
            // Trả view xem pdf
            ViewBag.DuongDanFile = vanBan.DuongDan;
            // Chỉ khi đăng ký mượn và không phải mượn đọc thì mới cho tải
            bool quyenTaiXuong = false;
            if (per.NguoiDung.IdNguoiDung == vanBan.NguoiTao) quyenTaiXuong = true;
            ViewBag.QuyenTaiXuong = quyenTaiXuong;
            return View("~/Views/_Shared/_lib/pdf_viewer.cshtml");
        }
        #endregion
    }
}