using EDM_DB;
using Public.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Applications.QuanLySanPham.Models
{
	public class tbSanPhamExtend
	{
        public tbSanPham SanPham { get; set; } = new tbSanPham();
        public KiemTraExcel KiemTraExcel { get; set; } = new KiemTraExcel();
        public tbSanPham_LoaiSanPham LoaiSanPham { get; set; } = new tbSanPham_LoaiSanPham();
        public tbLoaiKhoaHoc LoaiKhoaHoc { get; set; } = new tbLoaiKhoaHoc();
        public List<tbSanPham_LichSuExtend> LichSus { get; set; } = new List<tbSanPham_LichSuExtend>();
        public tbDonViSuDung DonViSuDung { get; set; } = new tbDonViSuDung();
    }
}