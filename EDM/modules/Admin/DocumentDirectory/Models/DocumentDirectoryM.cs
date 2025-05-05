using EDM_DB;
using Public.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DocumentDirectory.Models {
    public class DocumentDirectoryM {
    }
    public class tbDanhMucHoSoExtend : tbDanhMucHoSo{
        public KiemTraExcel KiemTraExcel { get; set; } = new KiemTraExcel();
        public tbDanhMucHoSo DanhMucCha { get; set; } = new tbDanhMucHoSo();
        public tbDonViSuDung_PhongLuuTru PhongLuuTru { get; set; } = new tbDonViSuDung_PhongLuuTru();

    }
}