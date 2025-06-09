using Applications.QuanLyBaiDang.Interfaces;
using Applications.QuanLyChienDich.Dtos;
using Applications.QuanLyChienDich.Interfaces;
using EDM_DB;
using Infrastructure.Interfaces;
using Public.AppServices;
using Public.Helpers;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using TrangThaiDangBai_ChienDich = Applications.QuanLyChienDich.Enums.TrangThaiDangBaiEnum;

namespace Applications.QuanLyChienDich.Services
{
    public class QuanLyChienDichAppService : BaseAppService, IQuanLyChienDichAppService
    {
        private readonly IRepository<tbBaiDang, Guid> _baiDangRepo;
        private readonly IRepository<tbChienDich, Guid> _chienDichRepo;
        private readonly IRepository<tbTepDinhKem, Guid> _tepDinhKemRepo;
        private readonly IRepository<tbBaiDangTepDinhKem, Guid> _baiDangTepDinhKemRepo;

        public QuanLyChienDichAppService(
           IUserContext userContext,
            IUnitOfWork unitOfWork,
            IRepository<tbBaiDang, Guid> baiDangRepo,
            IRepository<tbChienDich, Guid> chienDichRepo,
            IRepository<tbTepDinhKem, Guid> tepDinhKemRepo,
            IRepository<tbBaiDangTepDinhKem, Guid> baiDangTepDinhKemRepo)
            : base(userContext, unitOfWork)
        {
            _baiDangRepo = baiDangRepo;
            _chienDichRepo = chienDichRepo;
            _tepDinhKemRepo = tepDinhKemRepo;
            _baiDangTepDinhKemRepo = baiDangTepDinhKemRepo;
        }
        public async Task<IEnumerable<tbChienDich>> GetChienDichs(
          string loai = "all",
          List<Guid> idChienDichs = null,
          LocThongTinDto locThongTin = null)
        {
            var query = _chienDichRepo.Query()
                .Where(x =>
                    x.TrangThai != 0 &&
                    x.MaDonViSuDung == CurrentDonViId);

            // Áp dụng lọc trước khi join để tối ưu
            if (locThongTin != null)
            {
                if (locThongTin.IdNguoiTao.HasValue)
                    query = query.Where(x => x.IdNguoiTao == locThongTin.IdNguoiTao.Value);

                var ngayTaoRange = DateHelper.ParseThangNam(locThongTin.NgayTao);
                if (ngayTaoRange.Start.HasValue && ngayTaoRange.End.HasValue)
                {
                    query = query.Where(x =>
                        x.NgayTao >= ngayTaoRange.Start.Value &&
                        x.NgayTao <= ngayTaoRange.End.Value);
                }
            }

            if (loai == "single" && idChienDichs != null)
            {
                query = query.Where(x => idChienDichs.Contains(x.IdChienDich));
            }
      ;

            var data = await query
                .OrderByDescending(x => x.IdChienDich)
                .ToListAsync();

            return data;
        }
        public async Task<bool> IsExisted_ChienDich(tbChienDich chienDich)
        {
            // Kiểm tra còn hồ sơ khác có trùng mã không
            var chienDich_OLD = await _chienDichRepo.Query()
                .FirstOrDefaultAsync(x =>
                x.TenChienDich == chienDich.TenChienDich
                && x.IdChienDich != chienDich.IdChienDich
                && x.TrangThai != 0 && x.MaDonViSuDung == CurrentDonViSuDung.MaDonViSuDung);
            return chienDich_OLD != null;
        }
        public async Task Create_ChienDich(tbChienDich chienDich)
        {
            await _unitOfWork.ExecuteInTransaction(async () =>
            {
                var entity = new tbChienDich
                {
                    IdChienDich = Guid.NewGuid(),
                    TenChienDich = chienDich.TenChienDich,
                    GhiChu = chienDich.GhiChu,

                    TrangThaiHoatDong = (int)TrangThaiDangBai_ChienDich.WaitToPost,
                    TrangThai = 1,
                    MaDonViSuDung = CurrentDonViSuDung.MaDonViSuDung,
                    IdNguoiTao = CurrentNguoiDung.IdNguoiDung,
                    NgayTao = DateTime.Now,
                };

                await _unitOfWork.InsertAsync<tbChienDich, Guid>(entity);
            });
        }
        public async Task Delete_ChienDichs(List<Guid> idChienDichs)
        {
            if (idChienDichs == null || !idChienDichs.Any())
                throw new ArgumentException("Danh sách chiến dịch không được để trống.");

            await _unitOfWork.ExecuteInTransaction(async () =>
            {
                foreach (var id in idChienDichs)
                {
                    var chienDich = await _chienDichRepo.GetByIdAsync(id);
                    if (chienDich == null) continue;

                    // Cập nhật trạng thái bài đăng
                    chienDich.TrangThaiHoatDong = (int?)TrangThaiDangBai_ChienDich.WaitToDelete;
                    chienDich.TrangThai = 0;
                    chienDich.IdNguoiSua = CurrentNguoiDung.IdNguoiDung;
                    chienDich.NgaySua = DateTime.Now;
                    _chienDichRepo.Update(chienDich);

                    var baiDangs = _baiDangRepo.Query()
                    .Where(x => x.IdChienDich == chienDich.IdChienDich).ToList();
                    foreach (var baiDang in baiDangs)
                    {
                        baiDang.TrangThaiDangBai = (int)TrangThaiDangBai_ChienDich.WaitToDelete; // Chờ xóa trên nền tảng
                        baiDang.TrangThai = 0;
                        baiDang.IdNguoiSua = CurrentNguoiDung.IdNguoiDung;
                        baiDang.NgaySua = DateTime.Now;

                        // Lấy các bản ghi liên kết Tệp - Bài đăng
                        var baiDangTepDinhKems = await _baiDangTepDinhKemRepo.Query()
                            .Where(x => x.IdBaiDang == baiDang.IdBaiDang)
                            .ToListAsync();

                        if (!baiDangTepDinhKems.Any()) continue;

                        var tepIds = baiDangTepDinhKems.Select(x => x.IdTepDinhKem).Distinct().ToList();

                        var tepDinhKems = await _tepDinhKemRepo.Query()
                            .Where(x => tepIds.Contains(x.IdTep))
                            .ToListAsync();

                        foreach (var tep in tepDinhKems)
                        {
                            tep.TrangThai = 0;
                            tep.IdNguoiSua = CurrentNguoiDung.IdNguoiDung;
                            tep.NgaySua = DateTime.Now;
                            _tepDinhKemRepo.Update(tep);
                        }
                        ;
                    }

                }

                await _unitOfWork.SaveChangesAsync();
            });
        }
    }
}