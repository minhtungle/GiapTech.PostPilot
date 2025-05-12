using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Applications.QuanLyChienDich.Dtos
{
    public class DisplayModel_CRUD_ChienDich_Input_Dto
    {
        public Guid IdChienDich { get; set; }
        public string Loai { get; set; }
    }
}