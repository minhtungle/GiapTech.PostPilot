using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuanLyLuong.Models
{
    public struct TienLuongM
    {
        public long TongTienLuong { get; set; }
        public List<LuongTheoLop> LuongTheoLops { get; set; }

    }
    public struct LuongTheoLop
    {
        public Guid IdLopHoc { get; set; }
        public long TongTienLuong { get; set; }
        public List<LuongTheoBuoi> LuongTheoBuois { get; set; }
    }
    public struct LuongTheoBuoi
    {
        public Guid IdBuoiHoc { get; set; }
        public long TongTienLuong { get; set; }
    }
}