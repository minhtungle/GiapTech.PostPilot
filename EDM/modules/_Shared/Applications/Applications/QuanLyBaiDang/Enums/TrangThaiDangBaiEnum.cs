using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace Applications.QuanLyBaiDang.Enums
{
    public enum TrangThaiDangBaiEnum
    {
        [Description("Bản nháp")]
        Draft = -1,
        [Description("Chờ đăng")]
        WaitToPost = 0,
        [Description("Đã đăng")]
        Success = 1,
        [Description("Đăng lỗi")]
        Error = 2,
        [Description("Chờ xóa")]
        WaitToDelete = 9,
        [Description("Đã xóa")]
        Deleted = 10,
    }
}