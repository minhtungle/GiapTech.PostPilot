using Applications.QuanLyLopHoc.Models;
using EDM_DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Applications.QuanLyLopHoc.Dtos
{
    public class DisplayModal_XemLichHoc_Output_Dto
    {
        public string Loai { get; set; } = string.Empty;
        public tbLopHocExtend LopHoc { get; set; } = new tbLopHocExtend();
        public List<tbNguoiDung> GiaoViens { get; set; } = new List<tbNguoiDung>();
        public List<tbDonHangExtend> DonHangs { get; set; } = new List<tbDonHangExtend>();
    }
}