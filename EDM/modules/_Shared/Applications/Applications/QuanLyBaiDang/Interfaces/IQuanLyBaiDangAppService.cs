using Applications.QuanLyBaiDang.Dtos;
using Applications.QuanLyBaiDang.Models;
using EDM_DB;
using Public.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;
using LocThongTin_BaiDang = Applications.QuanLyBaiDang.Dtos.LocThongTinDto;

namespace Applications.QuanLyBaiDang.Interfaces
{
    public interface IQuanLyBaiDangAppService
    {
        List<ThaoTac> GetThaoTacs(string maChucNang);
        Task<Index_OutPut_Dto> Index_OutPut();
        Task<DisplayModel_CRUD_BaiDang_Output_Dto> DisplayModal_CRUD_BaiDang_Output(
            DisplayModel_CRUD_BaiDang_Input_Dto input);
        Task<FormAddBaiDangDto> AddBanGhi_Modal_CRUD_Output(
            List<tbBaiDangExtend> baiDangs);
        Task<IEnumerable<tbBaiDangExtend>> GetBaiDangs
           (string loai = "all",
           List<Guid> idBaiDangs = null,
           LocThongTin_BaiDang locThongTin = null);

        Task<FreeImageUploadResponse> UploadToFreeImageHost(
            HttpPostedFileBase file);
        Task Create_BaiDang(
            string loai,
            List<tbBaiDangExtend> baiDangs,
            HttpPostedFileBase[] files,
            Guid[] rowNumbers);
        Task Update_BaiDang(
            string loai,
            List<tbBaiDangExtend> baiDangs,
            HttpPostedFileBase[] files,
            Guid[] rowNumbers);
        Task Delete_BaiDangs(
            List<Guid> idBaiDangs);
    }
}
