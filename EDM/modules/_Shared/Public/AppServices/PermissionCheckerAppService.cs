using EDM_DB;
using Infrastructure.Interfaces;
using Newtonsoft.Json;
using Public.Interfaces;
using Public.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Public.AppServices
{
    public class PermissionCheckerAppService : IPermissionCheckerAppService
    {
        private readonly IUserContext _userContext;
        private readonly IRepository<tbNguoiDung, Guid> _nguoiDungRepo;
        private readonly IRepository<tbKieuNguoiDung, Guid> _kieuNguoiDungRepo;

        public PermissionCheckerAppService(
            IUserContext userContext,
            IRepository<tbNguoiDung, Guid> nguoiDungRepo,
            IRepository<tbKieuNguoiDung, Guid> kieuNguoiDungRepo)
        {
            _userContext = userContext;
            _nguoiDungRepo = nguoiDungRepo;
            _kieuNguoiDungRepo = kieuNguoiDungRepo;
        }

        public void CheckAccess(string controllerName, string actionName = "Index")
        {
            if (_userContext.UserId == Guid.Empty)
                throw new UnauthorizedAccessException("Người dùng chưa đăng nhập.");

            var nguoiDung = _nguoiDungRepo.Query()
                .FirstOrDefault(x => x.IdNguoiDung == _userContext.UserId);

            if (nguoiDung == null)
                throw new UnauthorizedAccessException("Không tìm thấy người dùng.");

            var kieuNguoiDung = _kieuNguoiDungRepo.Query()
                .FirstOrDefault(x => x.IdKieuNguoiDung == nguoiDung.IdKieuNguoiDung);

            if (kieuNguoiDung == null || string.IsNullOrEmpty(kieuNguoiDung.IdChucNang))
                throw new UnauthorizedAccessException("Người dùng không có quyền nào.");

            var danhSachChucNang = JsonConvert.DeserializeObject<List<ChucNangs>>(kieuNguoiDung.IdChucNang);

            var hasPermission = danhSachChucNang.Any(cn => cn.ChucNang?.MaChucNang == controllerName);

            if (!hasPermission && actionName == "Index")
                throw new UnauthorizedAccessException("Không có quyền truy cập trang này.");
        }
    }

}