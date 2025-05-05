using EDM.SignalR.Chat.Models;
using EDM_DB;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EDM.SignalR.Chat
{
    [HubName("chatService")]
    public class ChatHub : Hub
    {
        public ChatHub()
        {
        }
        public void guiTinNhan(TinNhanM tinNhan)
        {
            Clients.All.nhanTinNhan(res: tinNhan);
        }
        public void dangXuatNguoiDungHoatDong(ThongTinKiemTraM thongTinKiemTra)
        {
            Clients.All.dangXuatNguoiDungHoatDong(res: thongTinKiemTra);
        }
        public void capNhatNguoiDungHoatDong()
        {
            Clients.All.capNhatTrangThaiDangNhap();
        }
        public void test(dynamic thongTinNhan)
        {
            dynamic thongTinGui = new
            {
                idPhieuMuon = int.Parse(thongTinNhan.idNguoiDungs_CanKiemTra.ToString()),
                noiDungGui = thongTinNhan.noiDungGui.ToString(),
                tenNhom = thongTinNhan.tenNhom.ToString(),
            };
            string tenNhom = thongTinNhan.tenNhom.ToString();
            Clients.All.test(thongTinNhan);
        }
    }
}