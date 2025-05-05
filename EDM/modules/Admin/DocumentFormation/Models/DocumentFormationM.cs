using EDM_DB;
using Public.Models;
using System;
using System.Collections.Generic;

namespace DocumentFormation.Models
{
    public class DocumentFormationM
    {
    }
    public class tbHoSoExtend : tbHoSo
    {
        public KiemTraExcel KiemTraExcel { get; set; } = new KiemTraExcel();
        public tbViTriLuuTru ViTriLuuTru { get; set; } = new tbViTriLuuTru();
        public tbDanhMucHoSo DanhMucHoSo { get; set; } = new tbDanhMucHoSo();
        public tbDonViSuDung_PhongLuuTru PhongLuuTru { get; set; } = new tbDonViSuDung_PhongLuuTru();
        public tbNguoiDung ThongTinNguoiTao { get; set; } = new tbNguoiDung();
        public tbDonViSuDung DonViSuDung { get; set; } = new tbDonViSuDung();
        public default_tbCheDoSuDung CheDoSuDung { get; set; } = new default_tbCheDoSuDung();
        public List<tbHoSo_VanBanExtend> VanBans { get; set; } = new List<tbHoSo_VanBanExtend>();
    }
    public class tbHoSo_VanBanExtend : tbHoSo_VanBan
    {
        //public string DuongDanBanDau { get; set; } = string.Empty;
        public string DuongDan { get; set; } = string.Empty;
        public tbBieuMauExtend BieuMau { get; set; } = new tbBieuMauExtend();
    }
    public class tbBieuMauExtend : tbBieuMau
    {
        public List<tbBieuMau_TruongDuLieuExtend> TruongDuLieus { get; set; } = new List<tbBieuMau_TruongDuLieuExtend>();
    }
    public class tbBieuMau_TruongDuLieuExtend : tbBieuMau_TruongDuLieu
    {
        public List<tbHoSo_VanBan_DuLieuSo> DuLieuSos { get; set; } = new List<tbHoSo_VanBan_DuLieuSo>();
    }
    public class HoSoThuocTree<T> : Tree<T>
    {
        public List<tbHoSoExtend> hoSos { get; set; } = new List<tbHoSoExtend>();
    }

    public class HoSo_LichSu_ChiTiet
    {
        public string TenTruongDuLieu { get; set; } = string.Empty;
        public string GiaTri_Cu { get; set; } = string.Empty;
        public string GiaTri_Moi { get; set; } = string.Empty;
    }
    public class tbHoSo_LichSuExtend : tbHoSo_LichSu { 
        public tbNguoiDung ThongTinNguoiTao { get; set; } = new tbNguoiDung();
    }
}