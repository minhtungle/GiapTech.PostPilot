using Applications.QuanLyBaiDang.Dtos;
using Applications.QuanLyBaiDang.Enums;
using Applications.QuanLyBaiDang.Interfaces;
using Applications.QuanLyBaiDang.Models;
using EDM_DB;
using Infrastructure.Interfaces;
using Newtonsoft.Json;
using Public.AppServices;
using Public.Helpers;
using Public.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using TrangThaiDangBai_BaiDang = Applications.QuanLyBaiDang.Enums.TrangThaiDangBaiEnum;

namespace Applications.QuanLyBaiDang.Serivices
{
    public class QuanLyBaiDangAppService : BaseAppService, IQuanLyBaiDangAppService
    {
        private readonly IRepository<tbBaiDang, Guid> _baiDangRepo;
        private readonly IRepository<tbChienDich, Guid> _chienDichRepo;
        private readonly IRepository<tbTepDinhKem, Guid> _tepDinhKemRepo;
        private readonly IRepository<tbBaiDangTepDinhKem, Guid> _baiDangTepDinhKemRepo;
        private readonly IRepository<tbNenTang, Guid> _nenTangRepo;
        private readonly IRepository<tbNguoiDung, Guid> _nguoiDungRepo;
        private readonly IRepository<tbAIBot, Guid> _aiBotRepo;
        private readonly IRepository<tbAITool, Guid> _aiToolRepo;

        private readonly IRepository<tbApiCredential, Guid> _apiCredentialRepo;

        public QuanLyBaiDangAppService(
            IUserContext userContext,
            IUnitOfWork unitOfWork,
            IRepository<tbBaiDang, Guid> baiDangRepo,
            IRepository<tbChienDich, Guid> chienDichRepo,
            IRepository<tbTepDinhKem, Guid> tepDinhKemRepo,
            IRepository<tbBaiDangTepDinhKem, Guid> baiDangTepDinhKemRepo,
            IRepository<tbNenTang, Guid> nenTangRepo,
            IRepository<tbNguoiDung, Guid> nguoiDungRepo,
            IRepository<tbAIBot, Guid> aiBotRepo,
            IRepository<tbAITool, Guid> aiToolRepo,
            IRepository<tbApiCredential, Guid> apiCredentialRepo)
            : base(userContext, unitOfWork)
        {
            _baiDangRepo = baiDangRepo;
            _chienDichRepo = chienDichRepo;
            _tepDinhKemRepo = tepDinhKemRepo;
            _baiDangTepDinhKemRepo = baiDangTepDinhKemRepo;
            _nenTangRepo = nenTangRepo;
            _nguoiDungRepo = nguoiDungRepo;
            _aiBotRepo = aiBotRepo;
            _aiToolRepo = aiToolRepo;
            _apiCredentialRepo = apiCredentialRepo;
        }
        public List<ThaoTac> GetThaoTacs(string maChucNang) => GetThaoTacByIdChucNang(maChucNang);

        public async Task<Index_OutPut_Dto> Index_OutPut()
        {
            var thaoTacs = GetThaoTacs(maChucNang: "QuanLyBaiDang");
            var nenTangs = await _nenTangRepo.Query().Where(x =>
                    x.TrangThai != 0 &&
                    x.MaDonViSuDung == CurrentDonViId).ToListAsync();
            var nguoiTaos = await _nguoiDungRepo.Query().Where(x =>
                    x.TrangThai != 0 &&
                    x.MaDonViSuDung == CurrentDonViId).ToListAsync();
            var chienDichs = await _chienDichRepo.Query().Where(x =>
                    x.TrangThai != 0 &&
                    x.MaDonViSuDung == CurrentDonViId).ToListAsync();
            var aiTools = await _aiToolRepo.Query().Where(x =>
                    x.TrangThai != 0 &&
                    x.MaDonViSuDung == CurrentDonViId).ToListAsync();
            var aiBots = await _aiBotRepo.Query().Where(x =>
                    x.TrangThai != 0 &&
                    x.MaDonViSuDung == CurrentDonViId).ToListAsync();
            var trangThaiDangBaiEnums = EnumHelper.GetEnumInfoList<TrangThaiDangBaiEnum>();

            return new Index_OutPut_Dto
            {
                ThaoTacs = thaoTacs,
                NenTangs = nenTangs,
                NguoiTaos = nguoiTaos,
                ChienDichs = chienDichs,
                AIBots = aiBots,
                AITools = aiTools,
                TrangThaiDangBais = trangThaiDangBaiEnums,
            };
        }
        public async Task<DisplayModel_CRUD_BaiDang_Output_Dto> DisplayModal_CRUD_BaiDang_Output(
            DisplayModel_CRUD_BaiDang_Input_Dto input)
        {
            var output = new DisplayModel_CRUD_BaiDang_Output_Dto
            {
                Loai = input.Loai,
            };
            if (input.Loai == "create")
            {
                output.BaiDangs = new List<tbBaiDangExtend>
                {
                    new tbBaiDangExtend()
                };
            }
            else
            {
                var baiDang = await GetBaiDangs(
                    loai: "single",
                    idBaiDangs: input.IdBaiDangs);
                // Chỉ lấy những bài đăng có trạng thái (nháp, chờ đăng)
                output.BaiDangs = baiDang
                    .Where(x => x.BaiDang.TrangThaiDangBai == (int?)TrangThaiDangBai_BaiDang.Draft
                        || x.BaiDang.TrangThaiDangBai == (int?)TrangThaiDangBai_BaiDang.WaitToPost)
                    .ToList();
            }
            return output;
        }
        public async Task<FormAddBaiDangDto> AddBanGhi_Modal_CRUD_Output(List<tbBaiDangExtend> baiDangs)
        {
            //var baiDang = new tbBaiDangExtend { BaiDang = new tbBaiDang() };
            var nenTangs = await _nenTangRepo.Query().Where(x =>
                    x.TrangThai != 0 &&
                    x.MaDonViSuDung == CurrentDonViId).ToListAsync();
            var chienDichs = await _chienDichRepo.Query().Where(x =>
                x.TrangThai != 0 &&
                x.MaDonViSuDung == CurrentDonViId).ToListAsync();
            var aiTools = await _aiToolRepo.Query().Where(x =>
                    x.TrangThai != 0 &&
                    x.MaDonViSuDung == CurrentDonViId).ToListAsync();
            var aiBots = await _aiBotRepo.Query().Where(x =>
                    x.TrangThai != 0 &&
                    x.MaDonViSuDung == CurrentDonViId).ToListAsync();

            return new FormAddBaiDangDto
            {
                BaiDangs = baiDangs,
                ChienDichs = chienDichs,
                NenTangs = nenTangs,
                AIBots = aiBots,
                AITools = aiTools,
            };
        }
        public async Task<IEnumerable<tbBaiDangExtend>> GetBaiDangs(
            string loai = "all",
            List<Guid> idBaiDangs = null,
            LocThongTinDto locThongTin = null)
        {
            var query = _baiDangRepo.Query()
                .Where(x =>
                    x.TrangThai != 0 &&
                    x.MaDonViSuDung == CurrentDonViId);

            // Áp dụng lọc trước khi join để tối ưu
            if (locThongTin != null)
            {
                if (!string.IsNullOrWhiteSpace(locThongTin.NoiDung))
                    query = query.Where(x => x.NoiDung.Contains(locThongTin.NoiDung));

                if (locThongTin.IdChienDich.HasValue)
                    query = query.Where(x => x.IdChienDich == locThongTin.IdChienDich.Value);

                if (locThongTin.TrangThaiDangBai.HasValue)
                    query = query.Where(x => x.TrangThaiDangBai == locThongTin.TrangThaiDangBai.Value);

                if (locThongTin.IdNguoiTao.HasValue)
                    query = query.Where(x => x.IdNguoiTao == locThongTin.IdNguoiTao.Value);

                if (locThongTin.IdNenTang.HasValue)
                    query = query.Where(x => x.IdNenTang == locThongTin.IdNenTang.Value);

                var ngayTaoRange = DateHelper.ParseThangNam(locThongTin.NgayTao);
                if (ngayTaoRange.Start.HasValue && ngayTaoRange.End.HasValue)
                {
                    query = query.Where(x =>
                        x.NgayTao >= ngayTaoRange.Start.Value &&
                        x.NgayTao <= ngayTaoRange.End.Value);
                }

                var ngayDangRange = DateHelper.ParseThangNam(locThongTin.NgayDangBai);
                if (ngayDangRange.Start.HasValue && ngayDangRange.End.HasValue)
                {
                    query = query.Where(x =>
                        x.ThoiGian.HasValue &&
                        x.ThoiGian.Value >= ngayDangRange.Start.Value &&
                        x.ThoiGian.Value <= ngayDangRange.End.Value);
                }

            }

            if (loai == "single" && idBaiDangs != null && idBaiDangs.Any())
            {
                query = query.Where(x => idBaiDangs.Contains(x.IdBaiDang));
            }

            var result = await query
                .GroupJoin(
                    _nguoiDungRepo.Query(),
                    bd => bd.IdNguoiTao,
                    nd => nd.IdNguoiDung,
                    (bd, nds) => new { BaiDang = bd, NguoiTao = nds.DefaultIfEmpty() }
                )
                .SelectMany(
                    x => x.NguoiTao,
                    (x, nd) => new { x.BaiDang, NguoiTao = nd }
                )
                .GroupJoin(
                    _chienDichRepo.Query(),
                    x => x.BaiDang.IdChienDich,
                    cd => cd.IdChienDich,
                    (x, cds) => new { x.BaiDang, x.NguoiTao, ChienDich = cds.DefaultIfEmpty() }
                )
                .SelectMany(
                    x => x.ChienDich,
                    (x, cd) => new { x.BaiDang, x.NguoiTao, ChienDich = cd }
                )
                .GroupJoin(
                    _nenTangRepo.Query(),
                    x => x.BaiDang.IdNenTang,
                    nt => nt.IdNenTang,
                    (x, nts) => new { x.BaiDang, x.NguoiTao, x.ChienDich, NenTang = nts.DefaultIfEmpty() }
                )
                .SelectMany(
                    x => x.NenTang,
                    (x, nt) => new tbBaiDangExtend
                    {
                        BaiDang = x.BaiDang,
                        NguoiTao = x.NguoiTao,
                        ChienDich = x.ChienDich,
                        NenTang = nt
                    }
                )
                .OrderByDescending(x => x.BaiDang.ThoiGian)
                .ToListAsync();

            return result;
        }

        public async Task<FreeImageUploadResponse> UploadToFreeImageHost(HttpPostedFileBase file)
        {
            if (file == null || file.ContentLength == 0)
                return null;

            var _apiKey = await GetDecryptedCredential("FreeImage", "ApiKey");

            using (var ms = new MemoryStream())
            {
                if (file.InputStream.CanSeek)
                    file.InputStream.Position = 0;

                file.InputStream.CopyTo(ms);
                ms.Position = 0;

                using (var httpClient = new HttpClient())
                using (var formData = new MultipartFormDataContent())
                using (var streamContent = new StreamContent(ms))
                {
                    streamContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);

                    formData.Add(streamContent, "source", file.FileName);
                    formData.Add(new StringContent("upload"), "action");
                    formData.Add(new StringContent(_apiKey), "key");

                    var response = await httpClient.PostAsync("https://freeimage.host/api/1/upload", formData);
                    var json = await response.Content.ReadAsStringAsync();

                    if (response.IsSuccessStatusCode)
                    {
                        dynamic result = JsonConvert.DeserializeObject<FreeImageUploadResponse>(json);
                        //return JObject.Parse(json);
                        return result;
                    }
                    else
                    {
                        dynamic errorResult = JsonConvert.DeserializeObject<FreeImageUploadResponse>(json);
                        //return JObject.Parse(json);
                        return errorResult; // Hoặc trả về null, hoặc ném exception tùy nhu cầu
                    }
                }
            }
        }
        public async Task<string> GetDecryptedCredential(
            string serviceName,
            string credentialType,
            Guid? userId = null)
        {
            var cred = await _apiCredentialRepo.Query()
                .FirstOrDefaultAsync(x => x.ServiceName == serviceName
                                  && x.CredentialType == credentialType
                                  //&& (userId == null || x.IdNguoiDung == userId)
                                  );

            if (cred == null) throw new Exception("Không tìm thấy dữ liệu!");

            return CryptoHelper.Decrypt(cred.KeyJson);
        }
        public async Task Create_BaiDang(
            string loai,
            List<tbBaiDangExtend> baiDangs,
            HttpPostedFileBase[] files,
            Guid[] rowNumbers)
        {
            if (baiDangs == null || !baiDangs.Any())
                throw new ArgumentException("Danh sách bài đăng không được để trống.");

            var tepDinhKemMappings = new List<(tbBaiDang baiDang, List<tbTepDinhKem> teps)>();

            // 1. Upload ảnh trước khi transaction
            foreach (var baiDang_NEW in baiDangs)
            {
                var tepList = new List<tbTepDinhKem>();

                if (files != null && (baiDang_NEW.BaiDang.TuTaoAnhAI == false))
                {
                    for (int i = 0; i < files.Length; i++)
                    {
                        if (baiDang_NEW.RowNumber == rowNumbers[i])
                        {
                            var file = files[i];
                            if (file == null || file.ContentLength <= 0) continue;

                            var result = await UploadToFreeImageHost(file);
                            if (result == null || result.StatusCode != 200)
                                throw new Exception("Upload ảnh thất bại hoặc không có phản hồi từ server.");

                            var tep = new tbTepDinhKem
                            {
                                IdTep = Guid.NewGuid(),
                                FileName = Path.GetFileNameWithoutExtension(file.FileName),
                                DuongDanTepOnline = result.Image.Url,
                                TrangThai = 1,
                                IdNguoiTao = CurrentUserId,
                                NgayTao = DateTime.Now,
                                MaDonViSuDung = CurrentDonViId
                            };

                            tepList.Add(tep);
                        }
                    }
                }


                var baiDang = new tbBaiDang
                {
                    IdBaiDang = Guid.NewGuid(),
                    IdChienDich = baiDang_NEW.BaiDang.IdChienDich,
                    IdNenTang = baiDang_NEW.BaiDang.IdNenTang,
                    NoiDung = baiDang_NEW.BaiDang.NoiDung,
                    ThoiGian = baiDang_NEW.BaiDang.ThoiGian,
                    TuTaoAnhAI = baiDang_NEW.BaiDang.TuTaoAnhAI,
                    TrangThaiDangBai = loai == "create"
                        ? (int?)TrangThaiDangBai_BaiDang.WaitToPost
                        : (int?)TrangThaiDangBai_BaiDang.Draft,
                    TrangThai = 1,
                    IdNguoiTao = CurrentUserId,
                    NgayTao = DateTime.Now,
                    MaDonViSuDung = CurrentDonViId
                };
                tepDinhKemMappings.Add((baiDang, tepList));
            }

            // 2. Transaction: lưu bài đăng, file đính kèm và liên kết
            await _unitOfWork.ExecuteInTransaction(async () =>
            {
                foreach (var (baiDang, tepList) in tepDinhKemMappings)
                {
                    await _unitOfWork.InsertAsync<tbBaiDang, Guid>(baiDang);

                    foreach (var tep in tepList)
                    {
                        await _unitOfWork.InsertAsync<tbTepDinhKem, Guid>(tep);

                        var baiDangTep = new tbBaiDangTepDinhKem
                        {
                            IdBaiDangTepDinhKem = Guid.NewGuid(),
                            IdBaiDang = baiDang.IdBaiDang,
                            IdTepDinhKem = tep.IdTep
                        };

                        await _unitOfWork.InsertAsync<tbBaiDangTepDinhKem, Guid>(baiDangTep);
                    }
                }
            });
        }
        public async Task Delete_BaiDangs(List<Guid> idBaiDangs)
        {
            if (idBaiDangs == null || !idBaiDangs.Any())
                throw new ArgumentException("Danh sách bài đăng không được để trống.");

            await _unitOfWork.ExecuteInTransaction(async () =>
            {
                foreach (var id in idBaiDangs)
                {
                    var baiDang = await _baiDangRepo.GetByIdAsync(id);
                    if (baiDang == null) continue;

                    // Cập nhật trạng thái bài đăng
                    if (baiDang.TrangThaiDangBai == (int?)TrangThaiDangBai_BaiDang.WaitToPost) // Đang chờ đăng thì xóa luôn
                        baiDang.TrangThaiDangBai = (int?)TrangThaiDangBai_BaiDang.Deleted;
                    else baiDang.TrangThaiDangBai = (int?)TrangThaiDangBai_BaiDang.WaitToDelete; // Đã đăng thì chuyển sang trạng thái chờ xóa
                    baiDang.TrangThai = 0;
                    baiDang.IdNguoiSua = CurrentUserId;
                    baiDang.NgaySua = DateTime.Now;
                    _baiDangRepo.Update(baiDang);

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
                        tep.IdNguoiSua = CurrentUserId;
                        tep.NgaySua = DateTime.Now;
                        _tepDinhKemRepo.Update(tep);

                        // Gợi ý nâng cao: Nếu bạn cần xoá file vật lý trên server, gọi FileService tại đây
                        // await _fileService.DeleteFileIfExistsAsync(tep.DuongDanTepVatLy);
                    }
                }

                await _unitOfWork.SaveChangesAsync();
            });
        }
    }
}