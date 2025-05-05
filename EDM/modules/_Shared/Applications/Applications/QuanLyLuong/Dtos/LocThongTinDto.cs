using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Applications.QuanLyLuong.Dtos
{
    public class LocThongTinDto
    {
        public string ThoiGian { get; set; } = DateTime.Now.ToString("MM/yyyy");
        public bool DaTinhLuong { get; set; } = false;
        public string ChucVu { get; set; } = "GV";
    }
}