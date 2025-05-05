using EDM_DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Applications.QuanLyDoanhThu.Dtos
{
    public class MucTieuDoanhThuCoCauOutput_Dto
    {
        public int Thang {  get; set; }
        public tbCoCauToChuc_DoanhThu DoanhThu {  get; set; }
    }
}