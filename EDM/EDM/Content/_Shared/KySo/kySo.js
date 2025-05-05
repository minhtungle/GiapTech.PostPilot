//class KySo {
//    constructor() {
//        this.duongDanVanBan_BANDAU;
//    }
//    init() { }
//    signFileCallBack(rv) {
//        var kySo = this,
//            received_msg = JSON.parse(rv);
//        if (received_msg.Status == 0) {
//            let tenVanBan_KYSO = received_msg.FileName,
//                duongDanThuMuc_KYSO = received_msg.FileServer;
//            let form = new FormData();
//            form.append("duongDanVanBan_BANDAU", kySo.duongDanVanBan_BANDAU);
//            form.append("tenVanBan_KYSO", tenVanBan_KYSO);
//            form.append("duongDanThuMuc_KYSO", duongDanThuMuc_KYSO);
//            $.ajax({
//                ...ajaxDefaultProps({
//                    url: "/LoanManage/chuyenFileKySoSang",
//                    type: "POST",
//                    data: form
//                }),
//                contentType: false,
//                processData: false,
//                success: function (res) {
//                    if (res.status == "success") {
//                        //viewnoidung(staticidfile, staticidhstl, statictenfile);
//                    }
//                    sys.alert({ mess: res.mess, status: res.status, timeout: 1500 });
//                }
//            });
//        } else {
//            sys.alert({ mess: `Ký số thất bại: ${received_msg.Message}`, status: "error", timeout: 1500 });
//        };
//    }
//    exc_sign_files(filePath) {
//        var kySo = this,
//            protocol = window.location.protocol, // "https:"
//            hostname = window.location.hostname, // "localhost"
//            port = window.location.port, // "44345"
//            base_url = protocol + "//" + hostname + ":" + port;
//        kySo.duongDanVanBan_BANDAU = filePath;
//        var prms = {};
//        prms["FileUploadHandler"] = `${base_url}/FileUploadHandler.aspx`;
//        prms["SessionId"] = "";
//        prms["FileName"] = `${base_url}/${filePath}`;
//        prms["MetaData"] = "";
//        vgca_sign_file(JSON.stringify(prms), kySo.signFileCallBack);
//    }
//}