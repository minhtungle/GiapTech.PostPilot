using Applications.QuanLyBaiDang.Dtos;
using Applications.QuanLyBaiDang.Interfaces;
using Applications.QuanLyBaiDang.Models;
using EDM_DB;
using Infrastructure.Interfaces;
using Public.AppServices;
using Public.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace Applications.QuanLyBaiDang.Serivices
{
    public class QuanLyBaiDangAppService : BaseAppService, IQuanLyBaiDangAppService
    {
        private readonly IRepository<tbBaiDang, Guid> _baiDangRepo;

        public QuanLyBaiDangAppService(IUserContext userContext, IRepository<tbBaiDang, Guid> baiDangRepo)
            : base(userContext)
        {
            _baiDangRepo = baiDangRepo;
        }

        public async Task<IEnumerable<tbBaiDangExtend>> GetBaiDangs(
            Guid idChienDich, 
            string loai = "all", 
            List<Guid> idBaiDangs = null, 
            LocThongTinDto locThongTin = null)
        {
            var query = _baiDangRepo.Query()
                .Where(x => x.IdChienDich == idChienDich &&
                x.TrangThai != 0 &&
                x.MaDonViSuDung == CurrentDonViId);

            if (loai == "single" && idBaiDangs != null)
            {
                query = query.Where(x => idBaiDangs.Contains(x.IdBaiDang));
            };

            var baiDangs = await query
                .OrderByDescending(x => x.ThoiGian)
                .Select(x => new tbBaiDangExtend { BaiDang = x })
                .ToListAsync();

            return baiDangs;
        }
    }
}