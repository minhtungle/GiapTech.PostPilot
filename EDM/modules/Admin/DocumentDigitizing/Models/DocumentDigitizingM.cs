using DocumentFormation.Models;
using EDM_DB;

namespace DocumentDigitizing.Models {
    public class DocumentDigitizingM {
    }
    public class EXCEL_DULIEUSOsM : tbBieuMauExtend {
        public tbHoSoExtend HoSo { get; set; } = new tbHoSoExtend();
    }
    public class TepVanBanKySoHangLoatM
    {
        public string FileID { get; set;}
        public string FileName { get; set;}
        public string FileSignedURL { get; set;}
        public string Message { get; set;}
        public string URL { get; set;}
        public int Status { get; set; }
    }
}