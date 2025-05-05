using EDM_DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EDM.SignalR.Chat.Models
{
    public class TinNhanM
    {
        public tbNguoiDung NguoiGui { get; set; } = new tbNguoiDung();
        public string NoiDungGui { get; set; } = string.Empty;
        public NhomChatM NhomChat { get; set; } = new NhomChatM();
    }
    public class NhomChatM
    {
        public int IdNhom { get; set; } = 0;
        public string TenNhom { get; set; } = string.Empty;
        public ThanhVienM ThanhViens { get; set; } = new ThanhVienM();
    }
    public class ThanhVienM : tbNguoiDung
    {
        public string BietDanh { get; set; } = string.Empty;
    }
}