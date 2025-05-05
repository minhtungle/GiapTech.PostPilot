using Applications.QuanLyLopHoc.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Applications.QuanLyLopHoc.Dtos
{
    public class DisplayModal_TaoLichHoc_Output_Dto
    {
        public Guid IdLopHoc {  get; set; }
        public List<tbDonHangExtend> DonHangs { get; set; }
    }
}