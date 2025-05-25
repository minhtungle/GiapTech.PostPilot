using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Applications.UserAccount.Models
{
    public class NoiDungBanQuyen
    {
        public string MaBanQuyen { get; set; }
        public string KeyDonVi { get; set; }
        public string TenDonViSuDung { get; set; }
        public string TenNguoiDangKy { get; set; }
        public DateTime NgayKichHoat { get; set; }
        public DateTime NgayHetHan { get; set; }
        public int TongThoiGian { get; set; }
    }
}