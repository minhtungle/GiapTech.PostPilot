using Applications._Others.Interfaces;
using EDM_DB;
using Infrastructure.Interfaces;
using Public.AppServices;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace Applications._Others.AppServices
{
    public class OtherAppService : BaseAppService, IOtherAppService
    {
        private readonly IRepository<tbNenTang, Guid> _nenTangRepo;

        public OtherAppService(
           IRepository<tbNenTang, Guid> nenTangRepo,
            IUserContext userContext,
            IUnitOfWork unitOfWork) : base(userContext, unitOfWork)
        {
            _nenTangRepo = nenTangRepo;
        }
        public async Task<List<tbNenTang>> GetNenTangs(
            string loai = "all",
            List<Guid> idNenTangs = null)
        {
            var query = _nenTangRepo.Query()
              .Where(x =>
              x.TrangThai != 0 &&
              x.MaDonViSuDung == CurrentDonViSuDung.MaDonViSuDung);

            if (loai == "single" && idNenTangs != null)
            {
                query = query.Where(x => idNenTangs.Contains(x.IdNenTang));
            }
            ;

            var data = await query
                .OrderBy(x => x.Stt)
                .ToListAsync();

            return data;
        }
    }
}