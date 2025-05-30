using Applications.UserAccount.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Applications.UserAccount.Dtos
{
    public class DisplayModel_CRUD_NguoiDung_Output_Dto
    {
        public string Loai { get; set; }
        public tbNguoiDungExtend NguoiDung { get; set; }
    }
}