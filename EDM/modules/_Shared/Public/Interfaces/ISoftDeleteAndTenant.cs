using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Public.Interfaces
{
    public interface ISoftDeleteAndTenant
    {
        int TrangThai { get; }
        Guid MaDonViSuDung { get; }
    }
}