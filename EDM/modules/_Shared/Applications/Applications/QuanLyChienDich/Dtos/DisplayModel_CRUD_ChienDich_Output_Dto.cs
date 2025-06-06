using Applications.QuanLyChienDich.Models;
using EDM_DB;

namespace Applications.QuanLyChienDich.Dtos
{
    public class DisplayModel_CRUD_ChienDich_Output_Dto
    {
        public tbChienDich ChienDich { get; set; } = new tbChienDich();
        public string Loai { get; set; }
    }
}