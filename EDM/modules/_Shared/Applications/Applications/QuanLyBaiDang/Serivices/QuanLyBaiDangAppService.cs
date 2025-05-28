using Applications.QuanLyBaiDang.Dtos;
using Applications.QuanLyBaiDang.Interfaces;
using Applications.QuanLyBaiDang.Models;
using EDM_DB;
using Public.AppServices;
using Public.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;

namespace Applications.QuanLyBaiDang.Serivices
{
    public class QuanLyBaiDangAppService : BaseAppService, IQuanLyBaiDangAppService
    {
        private readonly EDM_DBEntities _db;

        public QuanLyBaiDangAppService(IUserContext userContext, EDM_DBEntities db)
            : base(userContext)
        {
            _db = db;
        }
        public List<tbBaiDangExtend> GetBaiDangs(Guid idChienDich, string loai = "all", List<Guid> idBaiDangs = null, LocThongTinDto locThongTin = null)
        {
            //var user = ClaimsPrincipal.Current;
            //var userId = Guid.Parse(user.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            //var maDonVi = Guid.Parse(user.FindFirst("MaDonViSuDung")?.Value);

            var baiDangRepo = _db.tbBaiDangs.Where(x => x.IdChienDich == idChienDich &&
            x.TrangThai != 0 && x.MaDonViSuDung == CurrentDonViId).ToList()
                ?? new List<tbBaiDang>();

            var baiDangs = baiDangRepo
                .Where(x => loai != "single" || idBaiDangs.Contains(x.IdBaiDang))
                .Select(g => new tbBaiDangExtend
                {
                    BaiDang = g,
                })
                .OrderByDescending(x => x.BaiDang.ThoiGian)
                .ToList() ?? new List<tbBaiDangExtend>();

            return baiDangs;
        }
    }
}