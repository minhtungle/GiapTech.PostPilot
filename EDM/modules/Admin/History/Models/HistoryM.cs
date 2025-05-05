using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EDM_DB;
namespace History.Models
{
    public class HistoryM
    {
    }
    public class tbLichSuTruyCapExtend : tbLichSuTruyCap
    {
        public string TenNguoiDung { get; set; } = string.Empty;
        public string TenDonViSuDung { get; set; } = string.Empty;
    }
}