'use strict'
/**
 * main
 * */
class QuanLyLuong {
    constructor() {
        this.page;
        this.pageGroup;

        this.nguoiDung = {
            idNguoiDung_DangSuDung: $("#input-idnguoidung-dangsudung").val(),
            maChucVu_DangSuDung: $("#input-machucvu-dangsudung").val(),
            chuaTinhLuong: {
                dataTable: null,
            },
            daTinhLuong: {
                dataTable: null,
            },
            dangXemChiTiet: {},
            displayModal_XemChiTiet_NguoiDung: function () { },
            displayModal_TinhTongLuong: function () { },
            tinhTongLuong: function () { },
            guiMail: function () { },
        };
        this.dangTinhTongLuong = {

        }
        this.locThongTin = {
            data: {
                ThoiGian: $("#input-thoigian", $("#modal-timkiem")).val(),
                ChucVu: $("#select-chucvu", $("#modal-timkiem")).val(),
                DaTinhLuong: false,
            },
        }
    }
    init() {
        var quanLyLuong = this;
        quanLyLuong.nguoiDung = {
            ...quanLyLuong.nguoiDung,
            chuaTinhLuong: {
                ...quanLyLuong.nguoiDung.chuaTinhLuong,
                dataTable: new DataTableCustom({
                    name: "nguoidung-chuatinhluong-getList",
                    table: $("#nguoidung-chuatinhluong-getList"),
                    props: {
                        ajax: {
                            url: '/QuanLyLuong/getList',
                            type: "POST",
                            data: function () {
                                return {
                                    locThongTin: {
                                        ...quanLyLuong.locThongTin.data,
                                    },
                                };
                            }
                        },
                        dom: `
                        <'row'<'col-sm-12'rt>>
                        <'row'<'col-sm-12 col-md-6 text-left'i><'col-sm-12 col-md-6 pt-2'p>>`,
                        lengthMenu: [
                            [2, 5],
                            [2, 5],
                        ],
                        rowId: 'IdNguoiDung',
                        columns: [
                            {
                                data: null,
                                className: "text-center",
                                searchable: false,
                                orderable: false,
                                render: function (data, type, row, meta) {
                                    return `<input class="form-check-input checkRow-nguoidung-chuatinhluong-getList" type="checkbox"/>`;
                                }
                            },
                            { data: "TenNguoiDung", },
                            {
                                data: "ChucVu.TenChucVu",
                                className: "text-center",
                            },
                            {
                                data: "TienLuong.TongLuong",
                                className: "text-center",
                                render: function (data, type, row, meta) {
                                    return `${sys.convertInt2MoneyFormat(data)}`;
                                }
                            },
                            {
                                data: "GuiMail",
                                className: "text-center",
                                render: function (data, type, row, meta) {
                                    if (data) return `<span class="font-bold fst-italic text-success">Đã gửi</span>`;
                                    return `<span class="font-bold fst-italic text-danger">Chưa gửi</span>`;
                                }
                            },
                            //{
                            //    data: null,
                            //    className: "text-center",
                            //    searchable: false,
                            //    orderable: false,
                            //    render: function (data, type, row, meta) {
                            //        return `<a href="#" class="btn btn-sm btn-primary" title="Cập nhật" onclick="quanLyLuong.nguoiDung.displayModal_XemChiTiet_NguoiDung('${data.ChucVu.MaChucVu}','${data.IdNguoiDung}')"><i class="bi bi-pencil-fill"></i></a>`;
                            //    }
                            //}
                        ],
                    }
                }).dataTable,
            },
            daTinhLuong: {
                ...quanLyLuong.nguoiDung.daTinhLuong,
                dataTable: new DataTableCustom({
                    name: "nguoidung-datinhluong-getList",
                    table: $("#nguoidung-datinhluong-getList"),
                    props: {
                        ajax: {
                            url: '/QuanLyLuong/getList',
                            type: "POST",
                            data: function () {
                                return {
                                    locThongTin: {
                                        ...quanLyLuong.locThongTin.data,
                                        DaTinhLuong: true
                                    },
                                };
                            }
                        },
                        dom: `
                        <'row'<'col-sm-12'rt>>
                        <'row'<'col-sm-12 col-md-6 text-left'i><'col-sm-12 col-md-6 pt-2'p>>`,
                        lengthMenu: [
                            [2, 5],
                            [2, 5],
                        ],
                        rowId: 'IdNguoiDung',
                        columns: [
                            {
                                data: null,
                                className: "text-center",
                                searchable: false,
                                orderable: false,
                                render: function (data, type, row, meta) {
                                    return `<input class="form-check-input checkRow-nguoidung-datinhluong-getList" type="checkbox"/>`;
                                }
                            },
                            { data: "TenNguoiDung", },
                            {
                                data: "ChucVu.TenChucVu",
                                className: "text-center",
                            },
                            {
                                data: "TienLuong.TongLuong",
                                className: "text-center",
                                render: function (data, type, row, meta) {
                                    return `${sys.convertInt2MoneyFormat(data)}`;
                                }
                            },
                            {
                                data: "GuiMail",
                                className: "text-center",
                                render: function (data, type, row, meta) {
                                    if (data) return `<span class="font-bold fst-italic text-success">Đã gửi</span>`;
                                    return `<span class="font-bold fst-italic text-danger">Chưa gửi</span>`;
                                }
                            },
                            //{
                            //    data: null,
                            //    className: "text-center",
                            //    searchable: false,
                            //    orderable: false,
                            //    render: function (data, type, row, meta) {
                            //        return `<a href="#" class="btn btn-sm btn-primary" title="Cập nhật" onclick="quanLyLuong.nguoiDung.displayModal_XemChiTiet_NguoiDung('${data.ChucVu.MaChucVu}','${data.IdNguoiDung}')"><i class="bi bi-pencil-fill"></i></a>`;
                            //    }
                            //}
                        ],
                    }
                }).dataTable,
            },
            _displayModal_XemChiTiet_NguoiDung: function (chucVu = "GV", idNguoiDung = '00000000-0000-0000-0000-000000000000') {
                $.ajax({
                    ...ajaxDefaultProps({
                        url: "/QuanLyLuong/displayModal_XemChiTiet_NguoiDung",
                        type: "POST",
                        data: {
                            chucVu, idNguoiDung,
                            thoiGian: $("#input-thoigian", $("#modal-timkiem")).val()
                        }
                    }),
                    success: function (res) {
                        $("#quanlyluong-xemchitiet-nguoidung").html(res.view);
                        //#region Gán thông tin lớp học vào object quanLyLuong.nguoiDung.dangXemChiTiet

                        quanLyLuong.nguoiDung.dangXemChiTiet = { ...res.model };
                        quanLyLuong.dangTinhTongLuong.tinhTongLuong();

                        //#endregion

                        //#region Thay đổi công thức và tiền thưởng

                        $.each([
                            {
                                element: $("#input-tienthuongthem", "#quanlyluong-xemchitiet-nguoidung"),
                                type: "number",
                            }, {
                                element: $("#input-heso-dadiemdanh", "#quanlyluong-xemchitiet-nguoidung"),
                                type: "number",
                                maxValue: 10,
                                minValue: 0.25,
                            }, {
                                element: $("#input-heso-hocviennghikhongphep", "#quanlyluong-xemchitiet-nguoidung"),
                                type: "number",
                                maxValue: 10,
                                minValue: 0.25,
                            }, {
                                element: $("#input-heso-hocviennghicophep", "#quanlyluong-xemchitiet-nguoidung"),
                                type: "number",
                                maxValue: 10,
                                minValue: 0.25,
                            },

                        ], function () {
                            var handleHtmlElement = new HandleHtmlElement(this);
                            handleHtmlElement.onlyAllowNumberKey();
                            handleHtmlElement.limitValue();
                            handleHtmlElement.dontAllowPaste();
                            $(this.element).on("change", function () {
                                quanLyLuong.dangTinhTongLuong.tinhTongLuong();
                            });
                        });

                        //#endregion

                        //#region Xử lý công thức lương

                        //".input-hesoluong"

                        //#endregion

                        sys.displayModal({
                            name: '#quanlyluong-xemchitiet-nguoidung'
                        });
                    }
                })
                sys.displayModal({
                    name: '#modal-xemchitiet-nguoidung',
                    displayStatus: 'hide'
                });
            },
            displayModal_TinhTongLuong() {
                //var hienTai = moment().format("MM/YYYY");
                //if (quanLyLuong.locThongTin.data.ThoiGian != hienTai) {
                //    sys.alert({ mess: `Chỉ cho phép cập nhật lương tháng ${hienTai}`, status: "warning", timeout: 1500 });
                //} else {
                //};

                var idNguoiDungs = [];
                quanLyLuong.nguoiDung.chuaTinhLuong.dataTable.rows().iterator('row', function (context, index) {
                    var $row = $(this.row(index).node());
                    if ($row.has("input.checkRow-nguoidung-chuatinhluong-getList:checked").length > 0) {
                        idNguoiDungs.push(`${$row.attr('id')}`);
                    };
                });
                quanLyLuong.nguoiDung.daTinhLuong.dataTable.rows().iterator('row', function (context, index) {
                    var $row = $(this.row(index).node());
                    if ($row.has("input.checkRow-nguoidung-datinhluong-getList:checked").length > 0) {
                        idNguoiDungs.push(`${$row.attr('id')}`);
                    };
                });
                // Kiểm tra idKhachHang
                if (idNguoiDungs.length == 0) {
                    sys.alert({ mess: "Bạn chưa chọn bản ghi nào", status: "warning", timeout: 1500 });
                } else {
                    var f = new FormData();
                    f.append("idNguoiDungs", JSON.stringify(idNguoiDungs));
                    f.append("locThongTin", JSON.stringify(quanLyLuong.locThongTin.data));
                    $.ajax({
                        ...ajaxDefaultProps({
                            url: "/QuanLyLuong/displayModal_TinhTongLuong",
                            type: "POST",
                            data: f,
                        }),
                        contentType: false,
                        processData: false,
                        success: function (res) {
                            $("#tinhtongluong-nguoidung").html(res.view);
                            quanLyLuong.dangTinhTongLuong.nguoiDung_CanTinhTongLuongs = res.model.NguoiDungs;
                            /**
                             * create_TinhTongLuong 
                             */
                            quanLyLuong.create_TinhTongLuong();
                            /**
                             * Gán các thuộc tính
                             */
                            var rows_NEW = quanLyLuong.dangTinhTongLuong.dataTable.rows().nodes().toArray(); // Chọn phần thử đầu tiên của bảng
                            quanLyLuong.dangTinhTongLuong.readRow($(rows_NEW[0]));
                            sys.displayModal({
                                name: "#tinhtongluong-nguoidung"
                            });
                        }
                    })
                };
            },
            guiMail: function () { },
        };
        quanLyLuong.locThongTin = {
            ...quanLyLuong.locThongTin,
            displayModal_TimKiem: function (display = 'show') {
                sys.displayModal({
                    name: '#modal-timkiem',
                    displayStatus: 'hide'
                });
                if (display == 'show') {
                    sys.displayModal({
                        name: '#modal-timkiem'
                    });
                };
            },
            timKiem: async function () {
                quanLyLuong.locThongTin.data = {
                    ...quanLyLuong.locThongTin.data,
                    ChucVu: $("#select-chucvu", $("#modal-timkiem")).val(),
                    ThoiGian: $("#input-thoigian", $("#modal-timkiem")).val(),
                };

                await quanLyLuong.nguoiDung.chuaTinhLuong.dataTable.ajax.reload();
                await quanLyLuong.nguoiDung.daTinhLuong.dataTable.ajax.reload();
                await quanLyLuong.locThongTin.displayModal_TimKiem('hide');
            },
        };

        quanLyLuong.page = $("#page-quanlyluong");
        sys.activePage({
            page: quanLyLuong.page.attr("id"),
            pageGroup: quanLyLuong.pageGroup
        });
    }
    create_TinhTongLuong() {
        var quanLyLuong = this;
        quanLyLuong.dangTinhTongLuong = {
            ...quanLyLuong.dangTinhTongLuong,
            dataTable: new DataTableCustom({
                name: "tinhtongluong-nguoidung-getList",
                table: $("#tinhtongluong-nguoidung-getList"),
                props: {
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
                    name: '#tinhtongluong-nguoidung-capnhattruong',
                    level: 2
                });
            },
            createRow: function () { },
            deleteRow: function () {
                var rows = quanLyLuong.dangTinhTongLuong.dataTable.rows().nodes().toArray(),
                    $rowChecks = $(`.checkRow-tinhtongluong-nguoidung-getList:checked`, rows);
                if ($rowChecks.length == 0) {
                    sys.alert({ status: "warning", mess: "Bạn chưa chọn bản ghi nào" })
                } else {
                    $.each($rowChecks, function () {
                        var $rowCheck = $(this).closest("tr"),
                            rowNumber = $rowCheck.attr("row"),
                            $div = $(`.tinhtongluong-nguoidung-read[row=${rowNumber}]`, $("#tinhtongluong-nguoidung"));
                        quanLyLuong.dangTinhTongLuong.dataTable.row($rowCheck).remove().draw(); // Xóa dòng đó
                        $div.remove();
                    });
                    // Chọn phần thử đầu tiên của bảng
                    var rows_NEW = quanLyLuong.dangTinhTongLuong.dataTable.rows().nodes().toArray();
                    quanLyLuong.dangTinhTongLuong.readRow($(rows_NEW[0]));
                };
            },
            readRow: function (el) {
                var rowNumber = $(el).attr("row"),
                    idNguoiDung = $(el).attr("data-idnguoidung"),
                    rows = quanLyLuong.dangTinhTongLuong.dataTable.rows().nodes().toArray(),
                    $divs = $(".tinhtongluong-nguoidung-read", $("#tinhtongluong-nguoidung")),
                    $div = $(`.tinhtongluong-nguoidung-read[row=${rowNumber}]`, $("#tinhtongluong-nguoidung"));
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

                quanLyLuong.dangTinhTongLuong.tinhTongLuong(idNguoiDung); // TODO: Click vào mới tính

                //#region Thay đổi công thức và tiền thưởng

                $.each([
                    {
                        element: $("#input-tienthuongthem", $div),
                        type: "number",
                    }, {
                        element: $("#input-heso-dadiemdanh", $div),
                        type: "number",
                        maxValue: 10,
                        minValue: 0.25,
                    }, {
                        element: $("#input-heso-hocviennghikhongphep", $div),
                        type: "number",
                        maxValue: 10,
                        minValue: 0.25,
                    }, {
                        element: $("#input-heso-hocviennghicophep", $div),
                        type: "number",
                        maxValue: 10,
                        minValue: 0.25,
                    },

                ], function () {
                    var handleHtmlElement = new HandleHtmlElement(this);
                    handleHtmlElement.onlyAllowNumberKey();
                    handleHtmlElement.limitValue();
                    handleHtmlElement.dontAllowPaste();
                    $(this.element).on("change", function () {
                        quanLyLuong.dangTinhTongLuong.tinhTongLuong(idNguoiDung); // TODO: Click vào mới tính
                    });
                });

                //#endregion
            },
            updateSingleCell: function (el) {
                var rows = quanLyLuong.dangTinhTongLuong.dataTable.rows().nodes().toArray(),
                    $div = $(el).closest(".tinhtongluong-nguoidung-read"),
                    rowNumber = $div.attr("row");
                $.each(rows, function () {
                    if ($(this).attr("row") == rowNumber)
                        $('span[data-tentruong="TongTienLuong"]', $(this)).text($(el).text());
                });
            },
            updateMultipleCell: function () {
                var kichHoat = $("#select-kichhoat", $("#tinhtongluong-nguoidung-capnhattruong")).val(),
                    idChucVu = $("#select-chucvu", $("#tinhtongluong-nguoidung-capnhattruong")).val(),
                    idKieuNguoiDung = $("#select-kieunguoidung", $("#tinhtongluong-nguoidung-capnhattruong")).val(),
                    idCoCauToChuc = $("#select-cocautochuc", $("#tinhtongluong-nguoidung-capnhattruong")).val();
                var rows = quanLyLuong.dangTinhTongLuong.dataTable.rows().nodes().toArray(),
                    $rowChecks = $(`.checkRow-tinhtongluong-nguoidung-getList:checked`, rows);
                if ($rowChecks.length == 0) {
                    sys.alert({ status: "warning", mess: "Bạn chưa chọn bản ghi nào" })
                } else {
                    $.each($rowChecks, function () {
                        var $rowCheck = $(this).closest("tr"),
                            rowNumber = $rowCheck.attr("row"),
                            $div = $(`.tinhtongluong-nguoidung-read[row=${rowNumber}]`, $("#tinhtongluong-nguoidung"));
                        // Thay đổi value cho những dòng được chọn
                        $("#select-kichhoat", $div).val(kichHoat); $("#select-kichhoat", $div).trigger("change");
                        $("#select-chucvu", $div).val(idChucVu); $("#select-chucvu", $div).trigger("change");
                        $("#select-kieunguoidung", $div).val(idKieuNguoiDung); $("#select-kieunguoidung", $div).trigger("change");
                        $("#select-cocautochuc", $div).val(idCoCauToChuc); $("#select-cocautochuc", $div).trigger("change");
                    });

                    sys.alert({ status: "success", mess: "Cập nhật trường dữ liệu thành công" })
                    sys.displayModal({
                        name: '#tinhtongluong-nguoidung-capnhattruong',
                        displayStatus: "hide",
                        level: 2,
                    });
                };
            },
            layThongTinNguoiDungDeTinhLuong: function (idNguoiDung = '00000000-0000-0000-0000-000000000000') {
                var $div = $(`.tinhtongluong-nguoidung-read[data-idnguoidung=${idNguoiDung}]`, $("#tinhtongluong-nguoidung"));

                var lopHocs = quanLyLuong.dangTinhTongLuong.nguoiDung_CanTinhTongLuongs.filter(
                    x => x.IdNguoiDung == idNguoiDung
                )[0].LopHocs;

                var $inputTienThuongThem = $("#input-tienthuongthem", $div);
                var tienThuongThem = $inputTienThuongThem.val() ? parseInt($inputTienThuongThem.val().replaceAll(' ', '')) : 0;
                var congThucTinhLuong = {
                    GhiChu: $("#input-ghichu", $div).val().trim(),
                    TienThuongThem: tienThuongThem,
                    HeSo_ChuaDiemDanh: 0,
                    HeSo_DaDiemDanh: $("#input-heso-dadiemdanh", $div).val() ?? 1,
                    HeSo_HocVienNghiKhongPhep: $("#input-heso-hocviennghikhongphep", $div).val() ?? 1,
                    HeSo_HocVienNghiCoPhep: $("#input-heso-hocviennghicophep", $div).val() ?? 1,
                };
                var nguoiDung = {
                    IdNguoiDung: idNguoiDung,
                    LopHocs: lopHocs,
                    CongThucTinhLuong: congThucTinhLuong
                };
                return nguoiDung;
            },
            tinhTongLuong: function (idNguoiDung = '00000000-0000-0000-0000-000000000000') {
                var $div = $(`.tinhtongluong-nguoidung-read[data-idnguoidung=${idNguoiDung}]`, $("#tinhtongluong-nguoidung"));
                var nguoiDung = quanLyLuong.dangTinhTongLuong.layThongTinNguoiDungDeTinhLuong(idNguoiDung);

                var f = new FormData();
                f.append("nguoiDung", JSON.stringify(nguoiDung));
                $.ajax({
                    ...ajaxDefaultProps({
                        url: "/QuanLyLuong/tinhTongLuong",
                        type: "POST",
                        data: f
                    }),
                    contentType: false,
                    processData: false,
                    success: function (res) {
                        var tienLuong = res.tienLuong;
                        var $pTongTienLuong = $("#p-tongtienluong", $div);
                        // Cập nhật tổng tiền lương
                        $pTongTienLuong.text(sys.convertInt2MoneyFormat(tienLuong.TongTienLuong)); // Tại xem chi tiết
                        quanLyLuong.dangTinhTongLuong.updateSingleCell($pTongTienLuong); // Tại bảng
                        // Tại từng lớp học trong xem chi tiết
                        tienLuong.LuongTheoLops.length > 0 ?? $.each($("#lophoc-container .table-lophoc", $div), function (iTb, tb) {
                            let tongTienLuong = tienLuong.LuongTheoLops[iTb].TongTienLuong;
                            $(this).find("td.tongtienluong_theolop").text(sys.convertInt2MoneyFormat(tongTienLuong));
                        });
                    }
                });
            },
            save: function () {
                var f = new FormData(),
                    nguoiDungs = [];
                $.each($(".tinhtongluong-nguoidung-read", $("#tinhtongluong-nguoidung")), function () {
                    var idNguoiDung = $(this).data("idnguoidung");
                    var nguoiDung = quanLyLuong.dangTinhTongLuong.layThongTinNguoiDungDeTinhLuong(idNguoiDung);
                    nguoiDungs.push(nguoiDung);
                });
                f.append("str_nguoiDungs", JSON.stringify(nguoiDungs));
                f.append("locThongTin", JSON.stringify(quanLyLuong.locThongTin.data));
                $.ajax({
                    ...ajaxDefaultProps({
                        url: "/QuanLyLuong/save",
                        type: "POST",
                        data: f
                    }),
                    contentType: false,
                    processData: false,
                    success: function (res) {
                        if (res.status == "success") {
                            quanLyLuong.nguoiDung.chuaTinhLuong.dataTable.ajax.reload(function () {
                                sys.displayModal({
                                    name: '#tinhtongluong-nguoidung',
                                    displayStatus: "hide"
                                });
                                sys.alert({ status: res.status, mess: res.mess })
                            }, false);
                            quanLyLuong.nguoiDung.daTinhLuong.dataTable.ajax.reload();
                        } else {
                            quanLyLuong.nguoiDung.chuaTinhLuong.dataTable.search('').draw();
                            sys.alert({ status: "error", mess: res.mess })
                        }
                    }
                })
            },
        };
    }
};