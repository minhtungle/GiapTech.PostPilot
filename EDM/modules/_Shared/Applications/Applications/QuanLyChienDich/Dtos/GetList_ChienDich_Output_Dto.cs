using EDM_DB;
using Public.Models;
using System.Collections.Generic;

namespace Applications.QuanLyChienDich.Dtos
{
    public class GetList_ChienDich_Output_Dto
    {
        public List<ThaoTac> ThaoTacs { get; set; }
        public IEnumerable<tbChienDich> ChienDichs { get; set; }
    }
}