using Applications.QuanLyChienDich.Models;

namespace Applications.QuanLyChienDich.Dtos
{
    public class DisplayModel_CRUD_ChienDich_Output_Dto
    {
        public tbChienDichExtend ChienDich { get; set; } = new tbChienDichExtend();
        public string Loai { get; set; }
    }
}