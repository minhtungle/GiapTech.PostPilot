using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace Public.Enums
{
    public enum TrangThaiDiemDanhEnum
    {
        [Description("Chưa điểm danh")]
        ChuaDiemDanh = 0,
        [Description("Đã điểm danh")]
        DaDiemDanh,
        [Description("Học viên nghỉ có phép")]
        HocVienNghiCoPhep,
        [Description("Học viên nghỉ không phép")]
        HocVienNghiKhongPhep,
    }
}