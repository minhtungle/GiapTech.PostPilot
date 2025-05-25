using EDM_DB;
using Public.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Applications.UserAccount.Models
{
    public class tbNguoiDungExtend
    {
        public tbNguoiDung NguoiDung { get; set; } = new tbNguoiDung();
        public KiemTraExcel KiemTraExcel { get; set; } = new KiemTraExcel();
        public tbKieuNguoiDung KieuNguoiDung { get; set; } = new tbKieuNguoiDung();
        public tbCoCauToChuc CoCauToChuc { get; set; } = new tbCoCauToChuc();
        public default_tbChucVu ChucVu { get; set; } = new default_tbChucVu();
        public string MatKhauCu { get; set; } = string.Empty;
        public string MatKhauMoi { get; set; } = string.Empty;
    }
}