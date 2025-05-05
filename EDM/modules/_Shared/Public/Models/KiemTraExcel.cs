using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Public.Models
{
    public class KiemTraExcel
    {
        public string KetQua { get; set; } = "Chờ kiểm tra";
        /**
         * 0: không hợp lệ
         * 1: hợp lệ
         * 2: chờ kiểm tra
         */
        public int TrangThai { get; set; } = 2;
    }
}