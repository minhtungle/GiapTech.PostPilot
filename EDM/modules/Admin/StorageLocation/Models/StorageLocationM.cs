using EDM_DB;
using Public.Models;

namespace StorageLocation.Models {
    public class StorageLocationM {
    }
    public class tbViTriLuuTruExtend : tbViTriLuuTru {
        public KiemTraExcel KiemTraExcel { get; set; } = new KiemTraExcel();
        public tbViTriLuuTru ViTriCha { get; set; } = new tbViTriLuuTru();
        public tbDonViSuDung_PhongLuuTru PhongLuuTru { get; set; } = new tbDonViSuDung_PhongLuuTru();
    }
}