using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Applications.QuanLyChienDich.Dtos
{
    public class LocThongTinDto
    {
        public Guid? IdNguoiTao { get; set; }
        public string NgayTao { get; set; } = DateTime.Now.ToString("MM/yyyy");
    }
}