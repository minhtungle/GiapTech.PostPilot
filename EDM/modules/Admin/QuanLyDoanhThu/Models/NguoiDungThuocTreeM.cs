using Public.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using UserAccount.Models;

namespace QuanLyDoanhThu.Models
{
    public class NguoiDungThuocTreeM<T> : Tree<T>
    {
        public List<tbNguoiDungExtend> NguoiDungs { get; set; } = new List<tbNguoiDungExtend>();
    }
}