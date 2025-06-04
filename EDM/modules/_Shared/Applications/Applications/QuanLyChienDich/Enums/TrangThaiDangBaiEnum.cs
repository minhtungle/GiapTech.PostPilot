using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace Applications.QuanLyChienDich.Enums
{
    public enum TrangThaiDangBaiEnum
    {
        [Description("Đang triển khai")]
        WaitToPost = 0,
        [Description("Hoàn thành")]
        Success = 1,
        [Description("Chờ xóa")]
        WaitToDelete = 9,
        [Description("Đã xóa")]
        Deleted = 10,
    }
}