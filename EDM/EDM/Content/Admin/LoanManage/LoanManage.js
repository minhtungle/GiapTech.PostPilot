'use strict'
/**
 * main
 * */
class LoanManage {
    constructor() {
        this.page;
        this.pageGroup;
        this.phieuMuon = {
            data: [],
            dataTable: null,
            save: function () { },
            delete: function () { },
            lyDoHuyDuyet: function () { },
            update: function () { },
            hienThiFile: function () { },
            displayModal_duyetPhieu: function () { },
        };
        this.chungThuc = {
            $vanBanChungThuc: null,
            displayModal_ChungThuc: function () { },
            luuMauChungThuc: function () { },
            xacNhanChungThuc: function () { },
        }
        this.kySo = {
            duongDanVanBan_BANDAU: "",
            signFileCallBack: function () { },
            exc_sign_files: function () { },
        };
        this.vanBanMuon = {
            data: [],
            dataTable: null,
            readRow: function () { }
        }
    }
    init() {
        var loan = this;
        loan.page = $("#page-loanmanage");
        loan.pageGroup = this.page.attr("page-group");
        loan.phieuMuon = {
            ...loan.phieuMuon,
            dataTable: new DataTableCustom({
                name: "loanmanage-getList",
                table: $("#loanmanage-getList"),
                props: {
                    ajax: {
                        url: '/LoanManage/getList',
                        type: "GET",
                    },
                    rowId: 'IdPhieuMuon',
                    columns: [
                        {
                            className: "text-center",
                            defaultContent: `<input class="form-check-input checkRow-loanmanage-getList" type="checkbox"/>`,
                            searchable: false,
                            orderable: false,
                        },
                        {
                            data: "SoPhieu",
                            class: "text-center",
                        },
                        {
                            data: "NguoiMuon_HoTen",
                        },
                        {
                            data: "HinhThucMuon.TenHinhThucMuon",
                            class: "text-center",
                        },
                        {
                            data: "TrangThai",
                            class: "text-center",
                            render: function (data, type, row, meta) {
                                let className = "",
                                    val = "";
                                if (data == 1) {
                                    className = "badge bg-primary";
                                    val = "Chờ xử lý";
                                } else if (data == 2) {
                                    className = "badge bg-success";
                                    val = "Đã duyệt";
                                } else if (data == 3) {
                                    className = "badge bg-danger";
                                    val = "Không duyệt";
                                } else {
                                    className = "badge bg-danger";
                                    val = "Hết hạn";
                                };
                                return `<span class="${className}">${val}</span>`;
                            }
                        },
                        {
                            data: "IdPhieuMuon",
                            className: "text-center",
                            searchable: false,
                            orderable: false,
                            render: function (data, type, row, meta) {
                                return `<a class="btn btn-sm btn-primary" title="Cập nhật" onclick="loan.phieuMuon.displayModal_duyetPhieu(${data})"><i class="bi bi-pencil-square"></i></a>`
                            }
                        }
                    ],
                }
            }).dataTable,
            displayModal_duyetPhieu: function (idPhieuMuon = 0) {
                $.ajax({
                    ...ajaxDefaultProps({
                        url: "/LoanManage/displayModal_Update_PhieuMuon",
                        type: "POST",
                        data: { idPhieuMuon }
                    }),
                    success: function (res) {
                        $("#phieumuon-update").html(res);
                        sys.displayModal({
                            name: "#phieumuon-update"
                        });
                        loan.create_VanBanMuon();
                    }
                })
            },
            lyDoHuyDuyet: function (loai, idPhieuMuon = 0) {
                var idPhieuMuons = [];
                if (loai == "single") {
                    idPhieuMuons.push(idPhieuMuon);
                } else {
                    loan.phieuMuon.dataTable.rows().iterator('row', function (context, index) {
                        var $row = $(this.row(index).node());
                        if ($row.has("input.checkRow-loanmanage-getList:checked").length > 0) {
                            idPhieuMuons.push($row.attr('id'));
                        };
                    });
                };
                if (idPhieuMuons.length == 0) {
                    sys.alert({ mess: "Bạn chưa chọn bản ghi nào", status: "warning", timeout: 1500 });
                } else {
                    sys.displayModal({
                        name: '#phieumuon-lydohuyduyet',
                        level: 2
                    });
                    $("#phieumuon-lydohuyduyet").find("button[name='yes']").off().on("click", function (e) {
                        e.preventDefault();
                        loan.phieuMuon.update(loai, 'huyduyet', idPhieuMuon)
                        sys.displayModal({
                            name: "#phieumuon-lydohuyduyet",
                            displayStatus: "hide"
                        })
                    });
                };
            },
            update: function (loai, trangThaiCapNhat, idPhieuMuon = 0) {
                var idPhieuMuons = [],
                    lyDoHuyDuyet = $("#phieumuon-lydohuyduyet textarea").val().trim();
                // Lấy id
                if (loai == "single") {
                    idPhieuMuons.push(idPhieuMuon);
                } else {
                    loan.phieuMuon.dataTable.rows().iterator('row', function (context, index) {
                        var $row = $(this.row(index).node());
                        if ($row.has("input.checkRow-loanmanage-getList:checked").length > 0) {
                            idPhieuMuons.push($row.attr('id'));
                        };
                    });
                };
                // Kiểm tra id
                if (idPhieuMuons.length == 0) {
                    sys.alert({ mess: "Bạn chưa chọn bản ghi nào", status: "warning", timeout: 1500 });
                } else {
                    var f = new FormData();
                    f.append("str_idPhieuMuons", idPhieuMuons.toString());
                    f.append("trangThaiCapNhat", trangThaiCapNhat);
                    f.append("lyDoHuyDuyet", lyDoHuyDuyet);
                    var callback = () => ({
                        ...ajaxDefaultProps({
                            url: "/LoanManage/update_PhieuMuons",
                            type: "POST",
                            data: f,
                        }),
                        contentType: false,
                        processData: false,
                        success: function (res) {
                            loan.phieuMuon.dataTable.ajax.reload(function () {
                                sys.displayModal({
                                    name: '#phieumuon-update',
                                    displayStatus: "hide"
                                });
                                sys.alert({ status: res.status, mess: res.mess })
                            }, false);
                        }
                    });
                    if (trangThaiCapNhat == "huyduyet") {
                        $.ajax({ ...callback() });
                    } else {
                        sys.confirmDialog({
                            mess: `Bạn có thực sự muốn ${trangThaiCapNhat == "duyet" ? "duyệt" : "xóa bỏ"} phiếu này hay không ?`,
                            callback: function () {
                                $.ajax({ ...callback() })
                            },
                        });
                    };
                };
            },
            hienThiFile: function (duongDan) {
                $("iframe", $("#phieumuon-update")).attr("src", `${duongDan}`);
                loan.chungThuc.hienThiChinhSuaChungThuc();
            },
        };
        loan.chungThuc = {
            ...loan.chungThuc,
            displayModal_ChungThuc: function (loai = "", idPhieuMuon = 0, idVanBan = 0) {
                $.ajax({
                    ...ajaxDefaultProps({
                        url: "/LoanManage/displayModal_ChungThuc",
                        type: "POST",
                        data: { idPhieuMuon, idVanBan, loai }
                    }),
                    success: function (res) {
                        loan.chungThuc.hienThiChinhSuaChungThuc();
                        $("#phieumuon-chungthuc").html(res);
                        // Hiển thị số trang đang trỏ tới của PDF
                        {
                            //var $iframe = document.getElementById("vanBanMuon-xemtep").contentDocument;
                            //trangDangSuDung = $iframe.
                            //$("#input-vitritrangchungthuc").val();
                        };
                        // Hiển thị textarea
                        loan.chungThuc.toggleTextArea2Span(false);
                        sys.displayModal({
                            name: "#phieumuon-chungthuc",
                            level: 2,
                        });
                    }
                })
            },
            thayDoiTextArea: function (tenNoiDung) {
                var $container = $(tenNoiDung, $("#phieumuon-chungthuc")),
                    $textArea = $("textarea", $container),
                    $span = $("span", $container);
                // Gán nội dung textarea cho span
                var noiDung = $textArea.val().replace(/\n/g, "<br />");
                $span.html(noiDung);
            },
            toggleTextArea2Span: function (trangThaiHienThi = false) {
                var $container = $(".input-span-container")
                if (trangThaiHienThi) {
                    $("textarea", $container).css({
                        "display": "none"
                    });
                    $("span", $container).css({
                        "display": "block"
                    });
                } else {
                    $("textarea", $container).css({
                        "display": "block"
                    });
                    $("span", $container).css({
                        "display": "none"
                    });
                };
            },
            // Đang dùng hàm này
            luuMauChungThuc: function (idPhieuMuon = 0, idVanBan = 0) {
                sys.loading(true); // Hiển thị màn hình chờ
                $("#chinhSuaTepChungThuc").empty();
                var duongdanvanbanmuon_dangchungthuc = $("#duongdanvanbanmuon-dangchungthuc").val(),
                    viTriTrangChungThuc = parseInt($("#input-vitritrangchungthuc").val()),
                    // Đè mẫu chứng thực
                    ganMauChungThuc = () => {
                        /**
                         * B1: Tải xuống mẫu chứng thực
                         * B2: Khởi tạo view chỉnh sửa chứng thực
                         * B4: Xác nhận - loan.chungThuc.xacNhanChungThuc()
                         */
                        // B1: Tải xuống mẫu chứng thực
                        var base64_MAUCHUNGTHUCs = [];
                        var $mauchungthucs = ["mauchungthuc-1-container", "mauchungthuc-2-container"];
                        // Hiển thị span
                        loan.chungThuc.toggleTextArea2Span(true);
                        $.each($mauchungthucs, function (i, $mauchungthuc) {
                            html2canvas(document.getElementById($mauchungthuc), {
                                allowTaint: true,
                                useCORS: true,
                                backgroundColor: 'rgba(0, 0, 0, 0)',
                                removeContainer: true,
                            }).then(function (canvas) {
                                let base64_MAUCHUNGTHUC = canvas.toDataURL("image/png");
                                base64_MAUCHUNGTHUCs.push(base64_MAUCHUNGTHUC);
                                // Gọi ajax khi lấy đủ số lượng base64
                                if (base64_MAUCHUNGTHUCs.length == $mauchungthucs.length) {
                                    var f = new FormData();
                                    f.append("idPhieuMuon", idPhieuMuon);
                                    f.append("idVanBan", idVanBan);
                                    f.append("base64_MAUCHUNGTHUCs", JSON.stringify(base64_MAUCHUNGTHUCs));
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
                                                // Chèn mẫu chứng thực đã lưu lên văn bản
                                                $.each($mauchungthucs, function (i) {
                                                    loan.$vanBanChungThuc.addImageToCanvas(`${res.duongDanVanBan.duongDanThuMuc_MAUCHUNGTHUC}/mau-chung-thuc-${i}.png`);
                                                });
                                                // Hiển thị view chỉnh sửa chứng thực và ẩn tạm view xem văn bản
                                                loan.chungThuc.hienThiChinhSuaChungThuc("true");
                                                // Gán hàm xác thực cho văn bản này
                                                $("#btn-xacnhan-chungthuc").off().on("click", function () {
                                                    loan.chungThuc.xacNhanChungThuc("xacnhan", res.duongDanVanBan);
                                                });
                                                $("#btn-huy-chungthuc").off().on("click", function () {
                                                    loan.chungThuc.xacNhanChungThuc("huy");
                                                });
                                            };
                                            sys.displayModal({
                                                name: "#phieumuon-chungthuc",
                                                level: 2,
                                                displayStatus: "hide"
                                            });
                                            sys.loading(false); // Ẩn màn hình chờ
                                        }
                                    });
                                };
                            })
                        });
                    };

                // Tải trang cần chứng thực rồi mới đè mẫu chứng thực lên
                loan.$vanBanChungThuc = new PDFAnnotate(
                    "chinhSuaTepChungThuc", duongdanvanbanmuon_dangchungthuc, {
                    onPageUpdated(page, oldData, newData) { },
                    ready() {
                        //console.log(this.pages);
                        viTriTrangChungThuc = loan.$vanBanChungThuc.arr_pages_number[0] // Gán lại vị trí sau xử lý
                        $("#input-vitritrangchungthuc").val(viTriTrangChungThuc);
                        ganMauChungThuc();
                    },
                    scale: 1,
                    pageImageCompression: "FAST", // FAST, MEDIUM, SLOW (Helps to control the new PDF file size)
                    pages: [
                        [viTriTrangChungThuc], // Trang lấy
                        [] // Trang không lấy
                    ],
                });
            },
            xacNhanChungThuc: async function (trangThaiXacThuc = "huy", duongDanVanBan = null) {
                if (trangThaiXacThuc == "xacnhan") {
                    sys.loading(true); // Hiển thị màn hình chờ
                    var base64_VANBAN_CHUNGTHUC = await loan.$vanBanChungThuc.savePdf("test.pdf"),
                        viTriTrangChungThuc = parseInt($("#input-vitritrangchungthuc").val()),
                        f = new FormData();
                    sys.loading(false); // Ẩn màn hình chờ
                    f.append("base64_VANBAN_CHUNGTHUC", JSON.stringify(base64_VANBAN_CHUNGTHUC));
                    f.append("duongDanVanBan", JSON.stringify(duongDanVanBan));
                    f.append("viTriTrangChungThuc", viTriTrangChungThuc);
                    $.ajax({
                        ...ajaxDefaultProps({
                            url: "/LoanManage/xacNhanChungThuc",
                            type: "POST",
                            data: f,
                        }),
                        contentType: false,
                        processData: false,
                        success: function (res) {
                            window.document.getElementById("vanBanMuon-xemtep").contentWindow.location.reload(true);
                            sys.alert({ mess: res.mess, status: res.status, timeout: 1500 });
                        }
                    });
                };
                loan.chungThuc.hienThiChinhSuaChungThuc();
            },
            hienThiChinhSuaChungThuc: function (trangThaiHienThi = "false") {
                $("#phieumuon-update .modal-footer .btn-thaotac").prop("disabled", false); // Kích hoạt nút thao tác
                $("#vanBanMuon-xemtep-chungthuc").css({
                    "display": "none"
                });
                $("#vanBanMuon-xemtep").css({
                    "display": "block"
                });
                if (trangThaiHienThi == "true") {
                    $("#phieumuon-update .modal-footer .btn-thaotac").prop("disabled", true); // Vô hiệu hóa nút thao tác
                    $("#vanBanMuon-xemtep").css({
                        "display": "none"
                    });
                    $("#vanBanMuon-xemtep-chungthuc").css({
                        "display": "block"
                    });
                };
            }
        }

        var protocol = window.location.protocol, // "https:"
            hostname = window.location.hostname, // "localhost"
            port = window.location.port, // "44345"
            base_url = `${protocol}//${hostname}${port != '' ? `:${port}` : ""}`;

        loan.kySo = {
            ...loan.kySo,
            // 1 văn bản
            signFileCallBack: function (res) {
                res = JSON.parse(res);
                if (res.Status == 0) {
                    let tenVanBan_KYSO = res.FileName,
                        duongDanThuMuc_KYSO = res.FileServer;
                    let form = new FormData();
                    form.append("duongDanVanBan_BANDAU", loan.kySo.duongDanVanBan_BANDAU);
                    form.append("tenVanBan_KYSO", tenVanBan_KYSO);
                    form.append("duongDanThuMuc_KYSO", duongDanThuMuc_KYSO);
                    $.ajax({
                        ...ajaxDefaultProps({
                            url: "/DocumentDigitizing/chuyenFileKySoSang",
                            type: "POST",
                            data: form
                        }),
                        contentType: false,
                        processData: false,
                        success: function (res) {
                            sys.alert({ mess: res.mess, status: res.status, timeout: 1500 });
                        }
                    });
                } else {
                    sys.alert({ mess: `Ký số thất bại: ${res.Message}`, status: "error", timeout: 1500 });
                };
            },
            // Nhiều văn bản
            signFilesCallBack: function (res) {
                res = JSON.parse(res);
                var mess = res.Message;
                if (res.Status == 0) {
                    var tepVanBans = res.Files.filter(x => x.Status == 0).map(x => ({
                        ...x,
                        FileName: x.FileName.replace(base_url, "", 1) // Lấy lại đường dẫn thư mục ban đầu
                    })); // Lấy tệp ký thành công
                    var form = new FormData();
                    form.append("str_tepVanBans", JSON.stringify(tepVanBans));
                    $.ajax({
                        ...ajaxDefaultProps({
                            url: "/DocumentDigitizing/chuyenFileKySoHangLoatSang",
                            type: "POST",
                            data: form
                        }),
                        contentType: false,
                        processData: false,
                        success: function (res) {
                            sys.alert({ mess, status: res.status, timeout: 1500 });
                        }
                    });
                } else {
                    sys.alert({ mess: `Ký số thất bại: ${mess}`, status: "error", timeout: 1500 });
                };
            },
            // Ký nhiều văn bản
            exc_sign_files: function () {
                var duongDanVanBans = [];
                loan.vanBanMuon.dataTable.rows().iterator('row', function (context, index) {
                    var $row = $(this.row(index).node());
                    if ($row.has("input.checkRow-vanBanMuon-getList:checked").length > 0) {
                        var duongDanVanBan = $row.data("duongdanvanban");
                        duongDanVanBans.push(duongDanVanBan);
                    };
                });
                if (duongDanVanBans.length == 0) {
                    sys.alert({ mess: "Bạn chưa chọn bản ghi nào", status: "warning", timeout: 1500 });
                } else {
                    var prms = {
                        FileUploadHandler: `${base_url}/FileUploadHandler.aspx`,
                        SessionId: "",
                        JWTToken: "",
                        Files: duongDanVanBans.map((duongDanTep, i) => ({
                            "FileID": i,
                            "FileName": `${base_url}/${duongDanTep}`,
                            "URL": `${base_url}/${duongDanTep}`
                        }))
                    };
                    vgca_sign_files(JSON.stringify(prms), loan.kySo.signFilesCallBack);
                };
            },
            // Ký từng văn bản
            exc_sign_approved: function (duongDanTep) {
                loan.kySo.duongDanVanBan_BANDAU = duongDanTep;
                var prms = {
                    FileUploadHandler: `${base_url}/FileUploadHandler.aspx`,
                    SessionId: "",
                    JWTToken: "",
                    FileName: `${base_url}/${duongDanTep}`,
                };
                vgca_sign_approved(JSON.stringify(prms), loan.kySo.signFileCallBack);
            },
        };
        sys.activePage({
            page: loan.page.attr("id"),
            pageGroup: loan.pageGroup
        });
    }
    create_VanBanMuon() {
        var loan = this;
        //var containerHeight = $("#truongdulieu-nav", $("#phieumuon-update")).height() - 10;
        //$("iframe", $("#phieumuon-update")).height(containerHeight);
        $("#vanBanMuon-getList tbody tr:first td a[title='Xem']").trigger("click");
        loan.vanBanMuon = {
            ...loan.vanBanMuon,
            dataTable: new DataTableCustom({
                name: "vanBanMuon-getList",
                table: $("#vanBanMuon-getList"),
                props: {
                    //maxHeight: 300,
                    dom: `
                    <'row'<'col-sm-12 col-md-6'>>
                    <'row'<'col-sm-12'rt>>
                    <'row'<'col-sm-12 col-md-4 pt-2'l><'col-sm-12 col-md-4 text-center'i><'col-sm-12 col-md-4 pt-2'p>>`,
                    lengthMenu: [
                        [5, 10, -1],
                        [5, 10, "Tất cả"],
                    ],
                }
            }).dataTable,

            readRow: function (el) {
                var rows = loan.vanBanMuon.dataTable.rows().nodes().toArray();
                $.each(rows, function () {
                    $(this).css({
                        "background-color": "transparent",
                    })
                });
                $(el).closest("tr").css({
                    "background-color": "var(--bs-table-hover-bg)",
                })
            },
        }
    }
};