using EDM_DB;
using Infrastructure.Interfaces;
using Newtonsoft.Json;
using Public.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Public.AppServices
{
    public abstract class BaseAppService
    {
        protected readonly IUserContext _userContext;

        protected BaseAppService(IUserContext userContext)
        {
            _userContext = userContext;
        }

        protected Guid CurrentUserId => _userContext.UserId;
        protected Guid CurrentDonViId => _userContext.DonViId;

        protected tbNguoiDung CurrentNguoiDung => _userContext.NguoiDung;
        protected tbDonViSuDung CurrentDonViSuDung => _userContext.DonViSuDung;
        protected default_tbChucVu CurrentChucVu => _userContext.ChucVu;
        protected tbKieuNguoiDung CurrentKieuNguoiDung => _userContext.KieuNguoiDung;
        protected tbCoCauToChuc CurrentCoCauToChuc => _userContext.CoCauToChuc;

        protected List<ThaoTac> GetThaoTacByIdChucNang(string maChucNang)
        {
            var kieuNguoiDung_IdChucNang = JsonConvert.DeserializeObject<List<ChucNangs>>(CurrentKieuNguoiDung.IdChucNang);
            var thaoTacs = kieuNguoiDung_IdChucNang
                .FirstOrDefault(x => x.ChucNang.MaChucNang == maChucNang)
                .ThaoTacs ?? new List<ThaoTac>();

            return thaoTacs;
        }
    }
}