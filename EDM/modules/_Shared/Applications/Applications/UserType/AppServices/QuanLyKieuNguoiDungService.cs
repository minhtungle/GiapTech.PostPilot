using Applications.UserType.Interfaces;
using EDM_DB;
using Infrastructure.Interfaces;
using Newtonsoft.Json;
using Public.AppServices;
using Public.Interfaces;
using Public.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Applications.UserType.AppServices
{
    public class QuanLyKieuNguoiDungService : BaseAppService, IQuanLyKieuNguoiDungAppService
    {
        private readonly IRepository<tbKieuNguoiDung, Guid> _kieuNguoiDungRepo;
        public QuanLyKieuNguoiDungService(IUserContext userContext, IRepository<tbKieuNguoiDung, Guid> kieuNguoiDungRepo)
          : base(userContext)
        {
            _kieuNguoiDungRepo = kieuNguoiDungRepo;
        }
    }
}