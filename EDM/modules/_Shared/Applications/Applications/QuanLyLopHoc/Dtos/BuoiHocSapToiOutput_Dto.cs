using EDM_DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Applications.QuanLyLopHoc.Dtos
{
    public class BuoiHocSapToiOutput_Dto
    {
        public tbLopHoc_BuoiHoc BuoiHoc { get; set; }
        public tbLopHoc LopHoc { get; set; }
        public int SoNgayConLai { get; set; }
    }
}