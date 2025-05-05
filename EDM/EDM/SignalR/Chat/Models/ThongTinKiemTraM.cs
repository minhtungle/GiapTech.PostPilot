using EDM_DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EDM.SignalR.Chat.Models
{
    public class ThongTinKiemTraM
    {
        public List<string> LoaiHinhKiemTra { get; set; } = new List<string>();
        public List<tbNguoiDung> NguoiDungs { get; set; } = new List<tbNguoiDung>();
        public string NoiDungThongBao { get; set; } = string.Empty;
    }
}