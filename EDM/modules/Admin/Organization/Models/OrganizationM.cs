using EDM_DB;
using Public.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Organization.Models {
    public class OrganizationM {
    }
    public class tbCoCauToChucExtend : tbCoCauToChuc {
        public KiemTraExcel KiemTraExcel { get; set; } = new KiemTraExcel();
        public tbCoCauToChuc CoCauCha { get; set; } = new tbCoCauToChuc();
    }
}