'use strict'
/**
 * main
 * */
class UserAccount {
    constructor() {
        this.chatService = $.connection.chatService;

        this.page;
        this.pageGroup;
        this.dataTable;
        this.nguoiDung = {
            data: [],
            dataTable: null,
            idNguoiDungs_XOA: [],
            btnDownload: function () { },
            displayModal_CRUD: function () { },
            save: function () { },
            delete: function () { },
        };
        this.coCauToChuc = null;
    }
    init() {
        var ua = this,
            idNguoiDung_DangSuDung = $("#input-idnguoidung-dangsudung").val();
        ua.excelNguoiDung = {};
        ua.page = $("#page-useraccount");
        ua.pageGroup = ua.page.attr("page-group");
        ua.nguoiDung = {
            ...ua.nguoiDung,
            dataTable: null,
            getList: function () {
                $.ajax({
                    ...ajaxDefaultProps({
                        url: "/UserAccount/getList",
                        type: "GET", // Phải là POST để gửi JSON
                        //contentType: "application/json; charset=utf-8",  // Định dạng JSON
                        //data: { locThongTin: ua.nguoiDung.locThongTin.data }
                        //dataType: "json",
                    }),
                    //contentType: false,
                    //processData: false,
                    success: function (res) {
                        $("#useraccount-getList-container").html(res);
                        ua.nguoiDung.dataTable = new DataTableCustom({
                            name: "useraccount-getList",
                            table: $("#useraccount-getList"),
                            props: {
                                dom: `
                                <'row'<'col-sm-12'rt>>
                                <'row'<'col-sm-12 col-md-6 text-left'i><'col-sm-12 col-md-6 pt-2'p>>`,
                                lengthMenu: [
                                    [5, 10],
                                    [5, 10],
                                ],
                            }
                        }).dataTable;
                    }
                });
            },
            btnDownload: function (btn) {
                ua.nguoiDung.dataTable.buttons(btn).trigger();
            },
            displayModal_CRUD(loai = "", idNguoiDung = '00000000-0000-0000-0000-000000000000') {
                $.ajax({
                    ...ajaxDefaultProps({
                        url: "/UserAccount/displayModal_CRUD",
                        type: "POST",
                        data: { loai, idNguoiDung }
                        ,
                    }),
                    success: function (res) {
                        $("#useraccount-crud").html(res);
                        $("#input-sodienthoai", $("#useraccount-crud")).trigger("keyup"); // Format cho ô này

                        var doiMatKhau = new DoiMatKhau('#doimatkhau', '#input-matkhau-moi', '#input-matkhau-moi-xacnhan');
                        doiMatKhau.kiemTraMatKhau();

                        sys.displayModal({
                            name: '#useraccount-crud'
                        });
                    }
                });
            },
            save: function (loai) {
                var $hinhthuccapnhat = $("#hinhthuccapnhat", $("#useraccount-crud"));
                // Chọn hình thức cập nhật
                if ($("#thongtin-list", $hinhthuccapnhat).hasClass("active")) {
                    var modalValidtion = htmlEl.activeValidationStates("#useraccount-crud #thongtin");
                    if (modalValidtion) {
                        var nguoiDung = {
                            NguoiDung: {
                                IdNguoiDung: $("#input-idnguoidung", $("#useraccount-crud")).val(),

                                GioiTinh: $("#select-gioitinh", $("#useraccount-crud")).val() == 1 ? true : false,
                                KichHoat: $("#select-kichhoat", $("#useraccount-crud")).val() == 1 ? true : false,
                                IdKieuNguoiDung: $("#select-kieunguoidung", $("#useraccount-crud")).val(),
                                IdCoCauToChuc: $("#select-cocautochuc", $("#useraccount-crud")).val(),
                                IdChucVu: $("#select-chucvu", $("#useraccount-crud")).val(),
                                IdCapDo_DoanhThu: $("#select-capdodoanhthu", $("#useraccount-crud")).val(),

                                TenNguoiDung: $("#input-tennguoidung", $("#useraccount-crud")).val().trim(),
                                TenDangNhap: $("#input-tendangnhap", $("#useraccount-crud")).val().trim(),
                                Email: $("#input-email", $("#useraccount-crud")).val().trim(),
                                SoDienThoai: $("#input-sodienthoai", $("#useraccount-crud")).val().trim(),
                                SoTaiKhoanNganHang: $("#input-sotaikhoannganhang", $("#useraccount-crud")).val().trim(),
                                NgaySinh: $("#input-ngaysinh", $("#useraccount-crud")).val().trim(),
                                GhiChu: $("#input-ghichu", $("#useraccount-crud")).val().trim(),
                                LinkLienHe: $("#input-linklienhe", $("#useraccount-crud")).val().trim(),
                            },
                        };
                        // Nếu là thêm mới thì lấy mật khẩu
                        if (loai == "create") nguoiDung.MatKhau = $("#input-matkhau", $("#useraccount-crud")).val().trim();
                        if (nguoiDung.NgaySinh != "")
                            nguoiDung.NgaySinh = moment(nguoiDung.NgaySinh, 'YYYY-MM-DD').format('DD/MM/YYYY');

                        sys.confirmDialog({
                            mess: `<p>Bạn có thực sự muốn thêm bản ghi này hay không ?</p>`,
                            callback: function () {
                                var formData = new FormData();
                                formData.append("nguoiDung", JSON.stringify(nguoiDung));
                                formData.append("loai", loai);

                                $.ajax({
                                    ...ajaxDefaultProps({
                                        url: loai == "create" ? "/UserAccount/create_NguoiDung" : "/UserAccount/update_NguoiDung",
                                        type: "POST",
                                        data: formData,
                                    }),
                                    //contentType: "application/json; charset=utf-8",  // Chỉ định kiểu nội dung là JSON
                                    contentType: false,
                                    processData: false,
                                    success: function (res) {
                                        if (res.status == "success") {
                                            ua.nguoiDung.getList();
                                            sys.displayModal({
                                                name: "#useraccount-crud",
                                                displayStatus: "hide"
                                            });
                                            sys.alert({ status: res.status, mess: res.mess });
                                            var thongTinKiemTra = {
                                                NguoiDungs: [
                                                    {
                                                        IdNguoiDung: nguoiDung.IdNguoiDung
                                                    }
                                                ],
                                                LoaiHinhKiemTra: ["idnguoidung"],
                                                NoiDungThongBao: "Thông tin tài khoản đã được thay đổi bởi [quản trị viên]"
                                            };
                                            chat.dangXuatNguoiDungHoatDong({ thongTinKiemTra: thongTinKiemTra });
                                        } else if (res.status == "datontai") {
                                            htmlEl.inputValidationStates(
                                                $("#input-tendangnhap"),
                                                "#useraccount-crud",
                                                res.mess,
                                                {
                                                    status: true,
                                                    isvalid: false
                                                }
                                            );
                                            sys.alert({ status: "warning", mess: res.mess });
                                        } else if (res.status == "logout") {
                                            sys.displayModal({
                                                name: "#useraccount-crud",
                                                displayStatus: "hide"
                                            });
                                            sys.logoutDialog({
                                                mess: res.mess,
                                            });
                                        } else {
                                            sys.alert({ status: res.status, mess: res.mess });
                                        };
                                    }
                                });
                            }
                        });
                    };
                } else {
                    var modalValidtion = htmlEl.activeValidationStates("#useraccount-crud #doimatkhau");
                    if (modalValidtion) {
                        var nguoiDung = {
                            NguoiDung: {
                                IdNguoiDung: $("#input-idnguoidung", $("#useraccount-crud")).val(),

                                MatKhauCu: $("#input-matkhau-cu", $("#useraccount-crud")).val().trim(),
                                MatKhauMoi: $("#input-matkhau-moi", $("#useraccount-crud")).val().trim(),
                                MatKhauMoi_XacNhan: $("#input-matkhau-moi-xacnhan", $("#useraccount-crud")).val().trim(),
                            }
                        };

                        if (nguoiDung.MatKhauMoi != nguoiDung.MatKhauMoi_XacNhan) {
                            htmlEl.inputValidationStates(
                                $("#input-matkhau-moi-xacnhan"),
                                "#useraccount-crud",
                                "Mật khẩu mới chưa chính xác",
                                {
                                    status: true,
                                    isvalid: false
                                }
                            );
                        } else {
                            sys.confirmDialog({
                                mess: `<p>Bạn có thực sự muốn thêm bản ghi này hay không ?</p>`,
                                callback: function () {
                                    var formData = new FormData();
                                    formData.append("nguoiDung", JSON.stringify(nguoiDung));
                                    formData.append("loai", loai);

                                    $.ajax({
                                        ...ajaxDefaultProps({
                                            url: "/UserAccount/capNhat_MatKhau",
                                            type: "POST",
                                            data: formData,
                                        }),
                                        //contentType: "application/json; charset=utf-8",  // Chỉ định kiểu nội dung là JSON
                                        contentType: false,
                                        processData: false,
                                        success: function (res) {
                                            if (res.status == "success") {
                                                ua.nguoiDung.getList();
                                                sys.displayModal({
                                                    name: "#useraccount-crud",
                                                    displayStatus: "hide"
                                                });
                                                sys.alert({ status: res.status, mess: res.mess });
                                                var thongTinKiemTra = {
                                                    NguoiDungs: [
                                                        {
                                                            IdNguoiDung: nguoiDung.IdNguoiDung
                                                        }
                                                    ],
                                                    LoaiHinhKiemTra: ["idnguoidung"],
                                                    NoiDungThongBao: "Thông tin tài khoản đã được thay đổi bởi [quản trị viên]"
                                                };
                                                chat.dangXuatNguoiDungHoatDong({ thongTinKiemTra: thongTinKiemTra });
                                            } else if (res.status == "matkhaucuchuachinhxac") {
                                                htmlEl.inputValidationStates(
                                                    $("#input-matkhau-cu"),
                                                    "#useraccount-crud",
                                                    "Mật khẩu cũ chưa chính xác",
                                                    {
                                                        status: true,
                                                        isvalid: false
                                                    }
                                                );
                                                sys.alert({ status: "warning", mess: res.mess });
                                            } else if (res.status == "logout") {
                                                sys.displayModal({
                                                    name: "#useraccount-crud",
                                                    displayStatus: "hide"
                                                });
                                                sys.logoutDialog({
                                                    mess: res.mess,
                                                });
                                            } else {
                                                sys.alert({ status: res.status, mess: res.mess });
                                            };
                                        }
                                    });
                                }
                            });
                        };
                    };
                };
            },
            displayModal_Delete: function (loai, idNguoiDung = '00000000-0000-0000-0000-000000000000') {
                //#region Kiểm tra điều kiện
                ua.nguoiDung.idNguoiDungs_XOA.length = 0;
                if (loai == "single") {
                    ua.nguoiDung.idNguoiDungs_XOA.push(idNguoiDung)
                } else {
                    var rows = ua.nguoiDung.dataTable.rows().nodes().toArray(),
                        $rowChecks = $(`.checkRow-useraccount-getList:checked`, rows);
                    if ($rowChecks.length == 0) {
                        sys.alert({ status: "warning", mess: "Bạn chưa chọn bản ghi nào" });
                        return 0;
                    } else if ($rowChecks.length == rows.length) {
                        sys.alert({ status: "warning", mess: "Không thể xóa toàn bộ bởi cần giữ lại 1 bản ghi thay thế cho các người dùng đang sử dụng" });
                        return 0;
                    } else {
                        $.each($rowChecks, function () {
                            var $rowCheck = $(this).closest("tr");
                            ua.nguoiDung.idNguoiDungs_XOA.push($rowCheck.attr("id"));
                        });
                    };
                };
                //#endregion
                if (ua.nguoiDung.idNguoiDungs_XOA.length > 0) {
                    var f = new FormData();
                    f.append("str_idNguoiDungs_XOA", JSON.stringify(ua.nguoiDung.idNguoiDungs_XOA));
                    $.ajax({
                        ...ajaxDefaultProps({
                            url: "/UserAccount/displayModal_Delete",
                            type: "POST",
                            data: f,
                        }),
                        contentType: false,
                        processData: false,
                        success: function (res) {
                            $("#useraccount-delete").html(res);
                            ua.nguoiDung.dataTable_THAYTHE = new DataTableCustom({
                                name: "useraccount-thaythe-getList",
                                table: $("#useraccount-thaythe-getList"),
                                props: {
                                    dom: `
                                <'row'<'col-sm-12'rt>>
                                <'row'<'col-sm-12 col-md-4 pt-2'l><'col-sm-12 col-md-4 text-center'i><'col-sm-12 col-md-4 pt-2'p>>`,
                                },
                                initCompleteProps: function () { }
                            }).dataTable;
                            // Gán check cho bảng thay thế
                            var rows_THAYTHE = ua.nguoiDung.dataTable_THAYTHE.rows().nodes().toArray(); // Tất cả dòng của bảng thay thế
                            singleCheck({
                                name: "useraccount-thaythe-getList",
                                parent: $(rows_THAYTHE)
                            });
                            sys.displayModal({
                                name: "#useraccount-delete"
                            });
                        }
                    });
                };
            },
            delete: function () {
                var rows_THAYTHE = ua.nguoiDung.dataTable_THAYTHE.rows().nodes().toArray(), // Tất cả dòng của bảng thay thế
                    rowChecks_THAYTHE = $(`.checkRow-useraccount-thaythe-getList:checked`, rows_THAYTHE); // Dòng đang chọn ở bảng thay thế
                if (rowChecks_THAYTHE.length == 0) {
                    sys.alert({ status: "warning", mess: "Bạn chưa chọn người dùng thay thế" })
                } else {
                    var f = new FormData();
                    f.append("str_idNguoiDungs_XOA", JSON.stringify(ua.nguoiDung.idNguoiDungs_XOA));
                    f.append("idNguoiDung_THAYTHE", $(rowChecks_THAYTHE[0]).data("id"));
                    $.ajax({
                        ...ajaxDefaultProps({
                            url: "/UserAccount/delete_NguoiDungs",
                            type: "POST",
                            data: f,
                        }),
                        contentType: false,
                        processData: false,
                        success: function (res) {
                            if (res.status == "success") {
                                ua.nguoiDung.getList();
                                sys.displayModal({
                                    name: "#useraccount-delete",
                                    displayStatus: "hide"
                                });
                                sys.alert({ status: res.status, mess: res.mess });
                                var thongTinKiemTra = {
                                    NguoiDungs: ua.nguoiDung.idNguoiDungs_XOA.map(idNguoiDung_XOA => ({
                                        IdNguoiDung: idNguoiDung_XOA
                                    })),
                                    LoaiHinhKiemTra: ["idnguoidung"],
                                    NoiDungThongBao: "Thông tin tài khoản đã được thay đổi bởi [quản trị viên]"
                                };
                                chat.dangXuatNguoiDungHoatDong({ thongTinKiemTra: thongTinKiemTra });
                            } else if (res.status == "logout") {
                                sys.displayModal({
                                    name: "#useraccount-delete",
                                    displayStatus: "hide"
                                });
                                sys.logoutDialog({
                                    mess: res.mess,
                                });
                            } else {
                                sys.alert({ status: res.status, mess: res.mess });
                            };

                        }
                    })
                };
            },
        };
        ua.chatService.client.capNhatNguoiDungHoatDong = function () {
            $.ajax({
                ...ajaxDefaultProps({
                    url: "/UserAccount/capNhatNguoiDungHoatDong",
                    type: "POST",
                    data: {
                        idNguoiDung: parseInt(chat.$idNguoiGui.val()),
                    }
                }),
                success: function (res) {
                    if (res) ua.nguoiDung.getList();
                }
            });
        };
        ua.nguoiDung.getList();
        sys.activePage({
            page: ua.page.attr("id"),
            pageGroup: ua.pageGroup
        });
    }
    displayModal_Excel_NguoiDung() {
        var ua = this;
        ua.getList_Excel_NguoiDung("reload");
        sys.displayModal({
            name: '#excel-nguoidung'
        });
    }
    getList_Excel_NguoiDung(loai) {
        var ua = this;
        $.ajax({
            ...ajaxDefaultProps({
                url: "/UserAccount/getList_Excel_NguoiDung",
                type: "POST",
                data: {
                    loai
                }
            }),
            success: function (res) {
                $("#excel-nguoidung-getList-container").html(res);
                /**
                 * ExcelHoSo 
                 */
                ua.create_Excel_NguoiDung();
                /**
                 * Gán các thuộc tính
                 */
                var rows_NEW = ua.excelNguoiDung.dataTable.rows().nodes().toArray(); // Chọn phần thử đầu tiên của bảng
                ua.excelNguoiDung.readRow($(rows_NEW[0]));
                $.each($(".excel-nguoidung-read", $("#excel-nguoidung")), function () {
                    var $div = $(this),
                        rowNumber = $div.attr("row");
                    // Gán validation
                    htmlEl.validationStates($div);
                    htmlEl.inputMask();
                    var modalValidtion = htmlEl.activeValidationStates($div);
                });
            }
        })
    }
    create_Excel_NguoiDung() {
        var ua = this;
        var containerHeight = $("#excel-nguoidung-getList-container").height() - 10;
        $("#excel-nguoidung-read-container", $("#excel-nguoidung")).height(containerHeight);
        ua.excelNguoiDung = {
            ...ua.excelNguoiDung,
            dataTable: new DataTableCustom({
                name: "excel-nguoidung-getList",
                table: $("#excel-nguoidung-getList"),
                props: {
                    maxHeight: containerHeight,
                    dom: `
                    <'row'<'col-sm-12 col-md-6'>>
                    <'row'<'col-sm-12'rt>>
                    <'row'<'col-sm-12 col-md-4 pt-2'l><'col-sm-12 col-md-4 text-center'i><'col-sm-12 col-md-4 pt-2'p>>`,
                    lengthMenu: [
                        [10, 50, -1],
                        [10, 50, 'Tất cả'],
                    ],
                }
            }).dataTable,
            displayModal_UpdateMultipleCell: function () {
                sys.displayModal({
                    name: '#excel-nguoidung-capnhattruong',
                    level: 2
                });
            },
            createRow: function () { },
            deleteRow: function () {
                var rows = ua.excelNguoiDung.dataTable.rows().nodes().toArray(),
                    $rowChecks = $(`.checkRow-excel-nguoidung-getList:checked`, rows);
                if ($rowChecks.length == 0) {
                    sys.alert({ status: "warning", mess: "Bạn chưa chọn bản ghi nào" })
                } else {
                    $.each($rowChecks, function () {
                        var $rowCheck = $(this).closest("tr"),
                            rowNumber = $rowCheck.attr("row"),
                            $div = $(`.excel-nguoidung-read[row=${rowNumber}]`, $("#excel-nguoidung"));
                        ua.excelNguoiDung.dataTable.row($rowCheck).remove().draw(); // Xóa dòng đó
                        $div.remove();
                    });
                    // Chọn phần thử đầu tiên của bảng
                    var rows_NEW = ua.excelNguoiDung.dataTable.rows().nodes().toArray();
                    ua.excelNguoiDung.readRow($(rows_NEW[0]));
                };
            },
            readRow: function (el) {
                var rowNumber = $(el).attr("row"),
                    rows = ua.excelNguoiDung.dataTable.rows().nodes().toArray(),
                    $divs = $(".excel-nguoidung-read", $("#excel-nguoidung")),
                    $div = $(`.excel-nguoidung-read[row=${rowNumber}]`, $("#excel-nguoidung"));
                $divs.hide(); $div.show();
                $.each(rows, function () {
                    if ($(this).attr("row") == rowNumber) {
                        $(this).css({
                            "background-color": "var(--bs-table-hover-bg)",
                        })
                    } else {
                        $(this).css({
                            "background-color": "transparent",
                        })
                    };
                });
            },
            updateSingleCell: function (el) {
                var rows = ua.excelNguoiDung.dataTable.rows().nodes().toArray(),
                    $div = $(el).closest(".excel-nguoidung-read"),
                    rowNumber = $div.attr("row");
                $.each(rows, function () {
                    if ($(this).attr("row") == rowNumber)
                        $('span[data-tentruong="TenNguoiDung"]', $(this)).text($(el).val());
                });
            },
            updateMultipleCell: function () {
                var kichHoat = $("#select-kichhoat", $("#excel-nguoidung-capnhattruong")).val(),
                    idChucVu = $("#select-chucvu", $("#excel-nguoidung-capnhattruong")).val(),
                    idKieuNguoiDung = $("#select-kieunguoidung", $("#excel-nguoidung-capnhattruong")).val(),
                    idCoCauToChuc = $("#select-cocautochuc", $("#excel-nguoidung-capnhattruong")).val();
                var rows = ua.excelNguoiDung.dataTable.rows().nodes().toArray(),
                    $rowChecks = $(`.checkRow-excel-nguoidung-getList:checked`, rows);
                if ($rowChecks.length == 0) {
                    sys.alert({ status: "warning", mess: "Bạn chưa chọn bản ghi nào" })
                } else {
                    $.each($rowChecks, function () {
                        var $rowCheck = $(this).closest("tr"),
                            rowNumber = $rowCheck.attr("row"),
                            $div = $(`.excel-nguoidung-read[row=${rowNumber}]`, $("#excel-nguoidung"));
                        // Thay đổi value cho những dòng được chọn
                        $("#select-kichhoat", $div).val(kichHoat); $("#select-kichhoat", $div).trigger("change");
                        $("#select-chucvu", $div).val(idChucVu); $("#select-chucvu", $div).trigger("change");
                        $("#select-kieunguoidung", $div).val(idKieuNguoiDung); $("#select-kieunguoidung", $div).trigger("change");
                        $("#select-cocautochuc", $div).val(idCoCauToChuc); $("#select-cocautochuc", $div).trigger("change");
                    });

                    sys.alert({ status: "success", mess: "Cập nhật trường dữ liệu thành công" })
                    sys.displayModal({
                        name: '#excel-nguoidung-capnhattruong',
                        displayStatus: "hide",
                        level: 2,
                    });
                };
            },
            upload: function () {
                var $select = $("#select-file").get(0),
                    formData = new FormData();
                $.each($select.files, function (idx, f) {
                    var extension = f.type;
                    if (extension.includes("sheet")) {
                        formData.append("files", f);
                    };
                });
                // Xóa bộ nhớ đệm để upload file trong lần tiếp theo
                $select.value = ''; // xóa giá trị của input file
                $.ajax({
                    ...ajaxDefaultProps({
                        url: "/UserAccount/upload_Excel_NguoiDung",
                        type: "POST",
                        data: formData
                    }),
                    contentType: false,
                    processData: false,
                    success: function (res) {
                        ua.getList_Excel_NguoiDung("upload");
                        ua.excelNguoiDung.dataTable.search('').draw();
                        sys.alert({ status: res.status, mess: res.mess });
                    }
                })
            },
            download: function (loaiTaiXuong) {
                var formData = new FormData(),
                    nguoiDungs = [];
                if (loaiTaiXuong == "data") {
                    var rows = ua.excelNguoiDung.dataTable.rows().nodes().toArray(),
                        $rowChecks = $(`.checkRow-excel-nguoidung-getList:checked`, rows);
                    $.each($rowChecks, function () {
                        var $rowCheck = $(this).closest("tr"),
                            rowNumber = $rowCheck.attr("row"),
                            $div = $(`.excel-nguoidung-read[row=${rowNumber}]`, $("#excel-nguoidung")),
                            nguoiDung = {
                                NguoiDung: {
                                    IdNguoiDung: '00000000-0000-0000-0000-000000000000',

                                    GioiTinh: $("#select-gioitinh", $div).val() == 1 ? true : false,
                                    KichHoat: $("#select-kichhoat", $div).val() == 1 ? true : false,
                                    IdKieuNguoiDung: $("#select-kieunguoidung", $div).val(),

                                    IdCoCauToChuc: $("#select-cocautochuc", $div).val(),

                                    IdChucVu: $("#select-chucvu", $div).val(),

                                    TenNguoiDung: $("#input-tennguoidung", $div).val().trim(),
                                    TenDangNhap: $("#input-tendangnhap", $div).val().trim(),
                                    MatKhau: $("#input-matkhau", $div).val().trim(),
                                    Email: $("#input-email", $div).val().trim(),
                                    SoDienThoai: $("#input-sodienthoai", $div).val().trim(),
                                    SoTaiKhoanNganHang: $("#input-sotaikhoannganhang", $div).val().trim(),
                                    NgaySinh: $("#input-ngaysinh", $div).val().trim(),
                                    GhiChu: $("#input-ghichu", $div).val().trim(),
                                    LinkLienHe: $("#input-linklienhe", $div).val().trim(),
                                },
                                KieuNguoiDung: {
                                    IdKieuNguoiDung: $("#select-kieunguoidung", $div).val(),
                                    TenKieuNguoiDung: $("#select-kieunguoidung option:selected", $div).text(),
                                },
                                CoCauToChuc: {
                                    IdCoCauToChuc: $("#select-cocautochuc", $div).val(),
                                    TenCoCauToChuc: $("#select-cocautochuc option:selected", $div).text(),
                                },
                                ChucVu: {
                                    IdChucVu: $("#select-chucvu", $div).val(),
                                    TenChucVu: $("#select-chucvu option:selected", $div).text(),
                                },
                            };
                        if (nguoiDung.NgaySinh != "")
                            nguoiDung.NgaySinh = moment(nguoiDung.NgaySinh, 'YYYY-MM-DD').format('DD/MM/YYYY');
                        nguoiDungs.push(nguoiDung);
                    });
                    formData.append("str_nguoiDungs", JSON.stringify(nguoiDungs));
                }
                formData.append("loaiTaiXuong", loaiTaiXuong);
                $.ajax({
                    ...ajaxDefaultProps({
                        url: "/UserAccount/get_NguoiDungs_download",
                        type: "POST",
                        data: formData
                    }),
                    contentType: false,
                    processData: false,
                    success: function () {
                        sys.alert({ status: "success", mess: "Đã tải xuống thành công" })
                        window.location = "/UserAccount/download_Excel_NguoiDung";
                    }
                })
            },
            reload: function () {
                ua.getList_Excel_NguoiDung("reload");
                ua.excelNguoiDung.dataTable.search('').draw();
                sys.alert({ status: "success", mess: "Đã làm mới dữ liệu" });
            },
            saveByGroup: function () {
                var formData = new FormData(),
                    nguoiDungs = [];
                $.each($(".excel-nguoidung-read", $("#excel-nguoidung")), function () {
                    var $div = $(this),
                        rowNumber = $div.attr("row"),
                        nguoiDung = {
                            NguoiDung: {
                                IdNguoiDung: '00000000-0000-0000-0000-000000000000',

                                GioiTinh: $("#select-gioitinh", $div).val() == 1 ? true : false,
                                KichHoat: $("#select-kichhoat", $div).val() == 1 ? true : false,
                                IdKieuNguoiDung: $("#select-kieunguoidung", $div).val(),

                                IdCoCauToChuc: $("#select-cocautochuc", $div).val(),

                                IdChucVu: $("#select-chucvu", $div).val(),


                                TenNguoiDung: $("#input-tennguoidung", $div).val().trim(),
                                TenDangNhap: $("#input-tendangnhap", $div).val().trim(),
                                MatKhau: $("#input-matkhau", $div).val().trim(),
                                Email: $("#input-email", $div).val().trim(),
                                SoDienThoai: $("#input-sodienthoai", $div).val().trim(),
                                SoTaiKhoanNganHang: $("#input-sotaikhoannganhang", $div).val().trim(),
                                NgaySinh: $("#input-ngaysinh", $div).val().trim(),
                                GhiChu: $("#input-ghichu", $div).val().trim(),
                                LinkLienHe: $("#input-linklienhe", $div).val().trim(),
                            },
                            CoCauToChuc: {
                                IdCoCauToChuc: $("#select-cocautochuc", $div).val(),
                                TenCoCauToChuc: $("#select-cocautochuc option:selected", $div).text(),
                            },
                            KieuNguoiDung: {
                                IdKieuNguoiDung: $("#select-kieunguoidung", $div).val(),
                                TenKieuNguoiDung: $("#select-kieunguoidung option:selected", $div).text(),
                            },
                            ChucVu: {
                                IdChucVu: $("#select-chucvu", $div).val(),
                                TenChucVu: $("#select-chucvu option:selected", $div).text(),
                            },
                        };
                    if (nguoiDung.NgaySinh != "")
                        nguoiDung.NgaySinh = moment(nguoiDung.NgaySinh, 'YYYY-MM-DD').format('DD/MM/YYYY');
                    nguoiDungs.push(nguoiDung);
                });
                formData.append("str_nguoiDungs", JSON.stringify(nguoiDungs));
                formData.append("loaiTaiXuong", "data");
                $.ajax({
                    ...ajaxDefaultProps({
                        url: "/UserAccount/get_NguoiDungs_download",
                        type: "POST",
                        data: formData
                    }),
                    contentType: false,
                    processData: false,
                    success: function () {
                        ua.excelNguoiDung.save();
                    }
                })
            },
            save: function () {
                $.ajax({
                    ...ajaxDefaultProps({
                        url: "/UserAccount/save_Excel_NguoiDung",
                    }),
                    contentType: false,
                    processData: false,
                    success: function (res) {
                        if (res.status == "success") {
                            ua.nguoiDung.getList();
                            sys.displayModal({
                                name: '#excel-nguoidung',
                                displayStatus: "hide"
                            });
                            sys.alert({ status: res.status, mess: res.mess })
                        } else if (res.status == "warning") {
                            ua.nguoiDung.getList();
                            // Đẩy lại danh sách dữ liệu chưa hợp lệ
                            ua.getList_Excel_NguoiDung("upload");
                            ua.excelNguoiDung.dataTable.search('').draw();
                            sys.alert({ status: "success", mess: res.mess });
                        } else if (res.status == "error-0") {
                            sys.alert({ status: "error", mess: res.mess })
                        } else {
                            // Đẩy lại danh sách dữ liệu chưa hợp lệ
                            ua.getList_Excel_NguoiDung("upload");
                            ua.excelNguoiDung.dataTable.search('').draw();
                            sys.alert({ status: "error", mess: res.mess })
                        }
                    }
                })
            },
        };
    }
};