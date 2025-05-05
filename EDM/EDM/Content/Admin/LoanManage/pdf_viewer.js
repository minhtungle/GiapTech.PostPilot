class PDFViewerCustom {
    constructor() {
        this.pdfContainer = "";
        this.init();
    }
    init() {
        var pdfViewer = this;
        pdfViewer.chungThuc = {
            ...pdfViewer.chungThuc,
            displayModal_ChungThuc: function (loai = "", idPhieuMuon = 0, idVanBan = 0) {
                $.ajax({
                    ...ajaxDefaultProps({
                        url: "/LoanManage/displayModal_ChungThuc",
                        type: "POST",
                        data: { idPhieuMuon, idVanBan, loai }
                    }),
                    success: function (res) {
                        $("#phieumuon-chungthuc").html(res);
                        sys.displayModal({
                            name: "#phieumuon-chungthuc",
                            level: 2,
                        });
                    }
                })
            },
            luuMauChungThuc: function (idPhieuMuon = 0, idVanBan = 0) {
                /**
                 * B1: Tải xuống mẫu chứng thực
                 * B2: Khởi tạo view chỉnh sửa chứng thực
                 * B4: Xác nhận - pdfViewer.chungThuc.xacNhanChungThuc()
                 */

                // B1: Tải xuống mẫu chứng thực
                html2canvas(document.getElementById("mauchungthuc-container"),
                    {
                        allowTaint: true,
                        useCORS: true,
                        backgroundColor: 'rgba(0, 0, 0, 0)',
                        removeContainer: true,
                    }).then(function (canvas) {
                        let base64_MAUCHUNGTHUC = canvas.toDataURL("image/png");
                        var f = new FormData();
                        f.append("base64_MAUCHUNGTHUC", JSON.stringify(base64_MAUCHUNGTHUC));
                        f.append("idPhieuMuon", idPhieuMuon);
                        f.append("idVanBan", idVanBan);
                        $.ajax({
                            ...ajaxDefaultProps({
                                url: "/LoanManage/luuMauChungThuc",
                                type: "POST",
                                data: f,
                            }),
                            contentType: false,
                            processData: false,
                            success: function (res) {
                                // B2: Khởi tạo view chỉnh sửa chứng thực
                                {
                                    // Tạo view hiển thị văn bản
                                    $("#chinhSuaTepChungThuc").empty();
                                    pdfViewer.$vanBanChungThuc = new PDFAnnotate(
                                        "chinhSuaTepChungThuc", res.duongDanVanBan.duongDanVanBan_PHIEUMUON, {
                                        onPageUpdated(page, oldData, newData) {
                                        },
                                        ready() {
                                            // Chèn mẫu chứng thực đã lưu lên văn bản
                                            pdfViewer.$vanBanChungThuc.addImageToCanvas(res.duongDanVanBan.duongDanVanBan_MAUCHUNGTHUC);
                                        },
                                        scale: 1,
                                        pageImageCompression: "FAST", // FAST, MEDIUM, SLOW(Helps to control the new PDF file size)
                                    });
                                    // Hiển thị view chỉnh sửa chứng thực và ẩn tạm view xem văn bản
                                    pdfViewer.chungThuc.hienThiChinhSuaChungThuc("true");
                                    // Gán hàm xác thực cho văn bản này
                                    $("#btn-xacnhan-chungthuc").off().on("click", function () {
                                        pdfViewer.chungThuc.xacNhanChungThuc("xacnhan", res.duongDanVanBan.duongDanVanBan_PHIEUMUON_SERVER);
                                    });
                                    $("#btn-huy-chungthuc").off().on("click", function () {
                                        pdfViewer.chungThuc.xacNhanChungThuc("huy");
                                    });
                                };
                                sys.displayModal({
                                    name: "#phieumuon-chungthuc",
                                    level: 2,
                                    displayStatus: "hide"
                                });
                            }
                        });
                    });
            },
            xacNhanChungThuc: function (trangThaiXacThuc = "huy", duongDanVanBan = null) {
                if (trangThaiXacThuc == "xacnhan") {
                    var base64_VANBAN_CHUNGTHUC = pdfViewer.$vanBanChungThuc.savePdf("test.pdf");
                    var f = new FormData();
                    f.append("base64_VANBAN_CHUNGTHUC", JSON.stringify(base64_VANBAN_CHUNGTHUC));
                    f.append("duongDanVanBan", JSON.stringify(duongDanVanBan));
                    $.ajax({
                        ...ajaxDefaultProps({
                            url: "/LoanManage/xacNhanChungThuc",
                            type: "POST",
                            data: f,
                        }),
                        contentType: false,
                        processData: false,
                        success: function (res) {

                        }
                    });
                };
                pdfViewer.chungThuc.hienThiChinhSuaChungThuc("false");
            },
            hienThiChinhSuaChungThuc: function (trangThaiHienThi = "false") {
                $("#vanBanMuon-xemtep-chungthuc").css({
                    "display": "none"
                });
                $("#outerContainer").css({
                    "display": "block"
                });
                if (trangThaiHienThi == "true") {
                    $("#outerContainer").css({
                        "display": "none"
                    });
                    $("#vanBanMuon-xemtep-chungthuc").css({
                        "display": "block"
                    });
                };
            }
        }
    }
    themMauChungThuc(duongDanVanBan_MAUCHUNGTHUC = "") {
        var pdfViewer = this;
        duongDanVanBan_MAUCHUNGTHUC = "/Assets/uploads/1/HOSO//HOSO_1/Loi-thu-toi-cua-mot-sat-thu-kinh-te[.pdf]/PHIEUMUON//MAUCHUNGTHUC/mau-chung-thuc.png";
        pdfViewer.pdfContainer.addImageToCanvas(duongDanVanBan_MAUCHUNGTHUC);
    }
    displayModal_ChungThuc(loai = "", idPhieuMuon = 0, idVanBan = 0) {
        $.ajax({
            ...ajaxDefaultProps({
                url: "/LoanManage/displayModal_ChungThuc",
                type: "POST",
                data: { idPhieuMuon, idVanBan, loai }
            }),
            success: function (res) {
                $("#phieumuon-chungthuc").html(res);
                sys.displayModal({
                    name: "#phieumuon-chungthuc",
                    level: 2,
                });
            }
        })
    }
    luuMauChungThuc(idPhieuMuon = 0, idVanBan = 0) {
        var pdfViewer = this;
        // Xuất form chứng thực ra dạng pdf và lưu xuống thư mục phiếu mượn
        html2canvas(document.getElementById("chungthuc-container"),
            {
                allowTaint: true,
                useCORS: true,
                backgroundColor: 'rgba(0, 0, 0, 0)',
                removeContainer: true,
            }).then(function (canvas) {
                let base64_MAUCHUNGTHUC = canvas.toDataURL("image/png");
                var f = new FormData();
                f.append("base64_MAUCHUNGTHUC", JSON.stringify(base64_MAUCHUNGTHUC));
                f.append("idVanBan", JSON.stringify(idVanBan));
                $.ajax({
                    ...ajaxDefaultProps({
                        url: "/LoanManage/luuMauChungThuc",
                        type: "POST",
                        data: f,
                    }),
                    contentType: false,
                    processData: false,
                    success: function (res) {
                        // Ghi đè tệp mẫu chứng thực lên view PDF
                        pdfViewer.themMauChungThuc(res.duongDanVanBan_MAUCHUNGTHUC);
                        sys.displayModal({
                            name: "#phieumuon-chungthuc",
                            level: 2,
                            displayStatus: "hide"
                        });
                    }
                });
            });
    }
}
var pdfViewer = new PDFViewerCustom();