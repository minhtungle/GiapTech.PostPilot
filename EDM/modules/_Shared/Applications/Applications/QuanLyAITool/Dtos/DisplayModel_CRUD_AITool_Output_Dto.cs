using EDM_DB;

namespace Applications.QuanLyAITool.Dtos
{
    public class DisplayModel_CRUD_AITool_Output_Dto
    {
        public tbAITool AITool { get; set; } = new tbAITool();
        public string Loai { get; set; }
    }
}