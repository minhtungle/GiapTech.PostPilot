using EDM_DB;
using Public.Models;
using System.Collections.Generic;

namespace DigitizingSetting.Models {
    public class DigitizingSettingM {
    }
    public class tbBieuMauExtend : tbBieuMau {
        public KiemTraExcel KiemTraExcel { get; set; } = new KiemTraExcel();
        public default_tbLoaiBieuMau LoaiBieuMau { get; set; } = new default_tbLoaiBieuMau();
        public List<tbBieuMau_TruongDuLieu> TruongDuLieus { get; set; } = new List<tbBieuMau_TruongDuLieu>();
    }
}