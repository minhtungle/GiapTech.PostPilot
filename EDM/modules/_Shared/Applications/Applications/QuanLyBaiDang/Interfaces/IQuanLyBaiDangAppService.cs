using Applications.QuanLyBaiDang.Dtos;
using Applications.QuanLyBaiDang.Models;
using EDM_DB;
using Public.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;
using LocThongTin_BaiDang = Applications.QuanLyBaiDang.Dtos.LocThongTinDto;
using LocThongTin_ChienDich = Applications.QuanLyChienDich.Dtos.LocThongTinDto;

namespace Applications.QuanLyBaiDang.Interfaces
{
    public interface IQuanLyBaiDangAppService
    {
        List<ThaoTac> GetThaoTacs(string maChucNang);
        Task<Index_OutPut_Dto> Index_OutPut();
        Task<IEnumerable<tbBaiDangExtend>> GetBaiDangs
           (string loai = "all",
           List<Guid> idBaiDangs = null,
           LocThongTin_BaiDang locThongTin = null);
     
        Task<FreeImageUploadResponse> UploadToFreeImageHost(
            HttpPostedFileBase file);
        Task Create_BaiDang(
            List<tbBaiDangExtend> baiDangs,
            HttpPostedFileBase[] files, 
            Guid[] rowNumbers);
        Task Delete_BaiDangs(
            List<Guid> idBaiDangs);


        Task<IEnumerable<tbChienDich>> GetChienDichs
        (string loai = "all",
        List<Guid> idChienDichs = null,
        LocThongTin_ChienDich locThongTin = null);
        Task Create_ChienDich(
            List<tbChienDich> chienDichs);
        Task Delete_ChienDichs(
            List<Guid> idChienDichs);
    }
}
