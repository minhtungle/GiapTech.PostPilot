using EDM_DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Applications.UserAccount.Models
{
    public class CapNhatTaiKhoanMail<T>
    {
        public T NguoiDung_OLD { get; set; }
        public T NguoiDung_NEW { get; set; }
        public tbDonViSuDung DonViSuDung { get; set; } = new tbDonViSuDung();
        public string HinhThucCapNhat { get; set; } = "thongtinnguoidung";
    }
}