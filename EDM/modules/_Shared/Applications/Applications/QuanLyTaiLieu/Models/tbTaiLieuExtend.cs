using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TaiLieu_DB;

namespace Applications.QuanLyTaiLieu.Models
{
    public class tbTaiLieuExtend
    {
        public tbTaiLieu TaiLieu { get; set; }
        public tbLoaiTaiLieu LoaiTaiLieu { get; set; }
    }
}