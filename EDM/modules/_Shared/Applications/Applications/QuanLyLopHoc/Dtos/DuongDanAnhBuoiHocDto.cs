using Public.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Applications.QuanLyLopHoc.Dtos
{
    public struct DuongDanAnhBuoiHocDto
    {
        public string DuongDanThuMuc_LOPHOC {  get; set; }
        public string DuongDanThuMuc_LOPHOC_SERVER {  get; set; }
        public string DuongDanThuMuc_BUOIHOC {  get; set; }
        public string DuongDanThuMuc_BUOIHOC_SERVER {  get; set; }
        public DuongDanVanBan DuongDan_HINHANH {  get; set; }
    }
}