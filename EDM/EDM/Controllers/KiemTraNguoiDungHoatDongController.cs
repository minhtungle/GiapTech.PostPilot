using EDM.SignalR.Chat;
using Microsoft.AspNet.SignalR;
using System;
using System.Runtime.Caching;
using System.Threading;
using Public.Models;
using EDM_DB;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Public.Controllers;

namespace EDM.Controllers
{
    public class KiemTraNguoiDungHoatDongController : StaticArgController
    {
        private static Timer timer;
        private static readonly object lockObject = new object();
        private const string CacheKey = "KiemTraNguoiDungHoatDong";
        private readonly int THOIGIANKIEMTRANGUOIDUNGDANGNHAP = 2;

        public KiemTraNguoiDungHoatDongController() { }
        public void TienHanhKiemTra(Permission per)
        {
            // Khởi tạo timer khi controller được tạo
            int delay = THOIGIANKIEMTRANGUOIDUNGDANGNHAP * 60 * 1000; // THOIGIANKIEMTRANGUOIDUNGDANGNHAP phút trong milliseconds
            timer = new Timer(TimerCallback, per, delay, Timeout.Infinite);
        }
        public void TimerCallback(object state)
        {
            // Đảm bảo chỉ có một luồng được phép thực thi hàm
            lock (lockObject)
            {
                // Kiểm tra xem hàm đã được thực thi trong THOIGIANKIEMTRANGUOIDUNGDANGNHAP phút gần nhất hay chưa
                bool functionExecuted = (bool)(MemoryCache.Default.Get(CacheKey) ?? false);

                if (!functionExecuted)
                {
                    // Hàm chạy mỗi THOIGIANKIEMTRANGUOIDUNGDANGNHAP phút trên server
                    //KiemTraNguoiDungDangDangNhap(nguoiDung: per.NguoiDung);
                    KiemTraNguoiDungDangHoatDong(per: state as Permission);
                    // Đánh dấu rằng hàm đã được thực thi trong THOIGIANKIEMTRANGUOIDUNGDANGNHAP phút gần nhất
                    MemoryCache.Default.Set(CacheKey, true, DateTimeOffset.UtcNow.AddMinutes(THOIGIANKIEMTRANGUOIDUNGDANGNHAP));
                };
            };

            // Reset lại timer để thực hiện tiếp theo
            int delay = THOIGIANKIEMTRANGUOIDUNGDANGNHAP * 60 * 1000; // THOIGIANKIEMTRANGUOIDUNGDANGNHAP phút trong milliseconds
            timer.Change(delay, Timeout.Infinite);
        }

        public void KiemTraNguoiDungDangHoatDong(Permission per)
        {
            // Cập nhật mọi người dùng về offline
            List<tbNguoiDung> nguoiDungs = db.tbNguoiDungs.Where(x => x.TrangThai != 0 &&
            x.MaDonViSuDung == per.DonViSuDung.MaDonViSuDung &&
            x.Online == true).ToList() ?? new List<tbNguoiDung>();
            foreach (var nguoiDung in nguoiDungs)
            {
                nguoiDung.Online = false;
            };
            db.SaveChanges();
            // Gửi tin nhắn tới tất cả các client đang hoạt động thông qua Hub
            //hubContext.Clients.All.capNhatNguoiDungHoatDong();
        }
    }
}