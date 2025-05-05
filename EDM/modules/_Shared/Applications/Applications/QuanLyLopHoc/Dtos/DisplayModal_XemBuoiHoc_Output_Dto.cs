using Applications.QuanLyLopHoc.Models;
using EDM_DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Applications.QuanLyLopHoc.Dtos
{
    public class DisplayModal_XemBuoiHoc_Output_Dto
    {
        public string Loai { get; set; } = string.Empty;
        public tbLopHoc_BuoiHocExtend BuoiHoc { get; set; } = new tbLopHoc_BuoiHocExtend();
    }
}