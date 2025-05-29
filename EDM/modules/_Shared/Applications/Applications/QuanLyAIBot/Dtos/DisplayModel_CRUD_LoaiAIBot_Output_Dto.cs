using EDM_DB;

namespace Applications.QuanLyAIBot.Dtos
{
    public class DisplayModel_CRUD_LoaiAIBot_Output_Dto
    {
        public tbLoaiAIBot LoaiAIBot { get; set; } = new tbLoaiAIBot();
        public string Loai { get; set; }
    }
}