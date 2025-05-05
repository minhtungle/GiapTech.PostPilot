using DocumentFormation.Models;
using EDM_DB;
using System.Collections.Generic;

namespace Search.Models {
    public class SearchM {
    }
    public class default_tbChucNangExtend : default_tbChucNang {
        public List<default_tbChucNangExtend> ChucNangs { get; set; } = new List<default_tbChucNangExtend>();
    }
    public class thongTinTimKiem {
        public string loai { get; set; } = "coban";
        public List<thongTinTimKiem_data> data { get; set; } = new List<thongTinTimKiem_data>();
    }
    public class thongTinTimKiem_data {
        public string str_IdTruongDuLieus { get; set; } = string.Empty;
        public int IdViTriLuuTru { get; set; } = 0;
        public int IdDanhMucHoSo { get; set; } = 0;
        public int IdPhongLuuTru { get; set; } = 0;
        public string TenTruong { get; set; } = string.Empty;
        public string NoiDungTimKiem { get; set; } = string.Empty;
        public string TieuChiTimKiem { get; set;} = string.Empty;
    }
    public class VanBanTimKiemsM : tbHoSo_VanBan {
        public string MaHoSo { get; set; } = string.Empty;
        public string TieuDeHoSo { get; set; } = string.Empty;
        public string QuyenTruyCap { get; set; } = string.Empty;
        public string GhiChu { get; set; } = string.Empty;
        public string TrangSo { get; set; } = string.Empty;

        public int IdCheDoSuDung { get; set; }
        public string TenCheDoSuDung { get; set; } = string.Empty;
        public List<tbBieuMau_TruongDuLieuExtend> TruongDuLieus { get; set; } = new List<tbBieuMau_TruongDuLieuExtend>();
    }
}