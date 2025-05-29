using EDM_DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Interfaces
{
    public interface IUserContext
    {
        Guid UserId { get; }
        Guid DonViId { get; }

        tbNguoiDung NguoiDung { get; }
        tbDonViSuDung DonViSuDung { get; }
        default_tbChucVu ChucVu { get; }
        tbKieuNguoiDung KieuNguoiDung { get; }
        tbCoCauToChuc CoCauToChuc { get; }
    }
}
