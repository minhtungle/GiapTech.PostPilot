using System;

namespace Applications.QuanLyBaiDang.Dtos
{
    public class LocThongTinDto
    {
        public string NoiDung { get; set; }
        public Guid? IdChienDich { get; set; }
        public Guid? IdNguoiTao { get; set; }
        public Guid? IdNenTang { get; set; }
        public string NgayTao { get; set; } = DateTime.Now.ToString("MM/yyyy");
        public string NgayDangBai { get; set; } = DateTime.Now.ToString("MM/yyyy");
    }
}