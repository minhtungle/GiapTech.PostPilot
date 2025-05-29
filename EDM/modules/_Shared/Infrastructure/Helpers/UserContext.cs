using EDM_DB;
using Infrastructure.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;

namespace Infrastructure.Helpers
{
    public class UserContext : IUserContext
    {
        private ClaimsPrincipal User => HttpContext.Current?.User as ClaimsPrincipal;

        public Guid UserId =>
            Guid.TryParse(User?.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var id) ? id : Guid.Empty;

        public Guid DonViId =>
            Guid.TryParse(User?.FindFirst("MaDonViSuDung")?.Value, out var id) ? id : Guid.Empty;
      
        public tbNguoiDung NguoiDung
        {
            get
            {
                var json = User?.FindFirst("NguoiDung")?.Value;
                return string.IsNullOrEmpty(json) ? null : JsonConvert.DeserializeObject<tbNguoiDung>(json);
            }
        }
        public tbDonViSuDung DonViSuDung
        {
            get
            {
                var json = User?.FindFirst("DonViSuDung")?.Value;
                return string.IsNullOrEmpty(json) ? null : JsonConvert.DeserializeObject<tbDonViSuDung>(json);
            }
        }
        public default_tbChucVu ChucVu
        {
            get
            {
                var json = User?.FindFirst("ChucVu")?.Value;
                return string.IsNullOrEmpty(json) ? null : JsonConvert.DeserializeObject<default_tbChucVu>(json);
            }
        }
        public tbKieuNguoiDung KieuNguoiDung
        {
            get
            {
                var json = User?.FindFirst("KieuNguoiDung")?.Value;
                return string.IsNullOrEmpty(json) ? null : JsonConvert.DeserializeObject<tbKieuNguoiDung>(json);
            }
        }
        public tbCoCauToChuc CoCauToChuc
        {
            get
            {
                var json = User?.FindFirst("CoCauToChuc")?.Value;
                return string.IsNullOrEmpty(json) ? null : JsonConvert.DeserializeObject<tbCoCauToChuc>(json);
            }
        }
    }
}