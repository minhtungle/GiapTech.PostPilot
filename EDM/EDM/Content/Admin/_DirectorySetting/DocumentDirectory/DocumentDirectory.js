'use strict'
class DanhMucHoSo {
    constructor({ tableName, $table }) {
        this.tableName = tableName;
        this.$table = $table;
        this.dataTable;
        this.init();
    }
    init() {
        var danhMuc = this;
        danhMuc.dataTable = new DataTableCustom({
            name: danhMuc.tableName,
            table: danhMuc.$table,
            props: {
                ordering: false,
                maxHeight: 500,
                dom: `
                <'row'<'col-12 mb-3 d-none'B>>
                <'row'<'col-sm-12'rt>>
                <'row'<'col-sm-12 col-md-5'i><'col-sm-12 col-md-7 pt-2 d-none'p>>`,
                lengthMenu: [
                    [-1],
                    ['Tất cả'],
                ],
                //columnDefs: [
                //    { targets: [0], visible: false, searchable: false } // Ẩn cột nhóm (Group column)
                //],
                //order: [[0, 'asc']], // Sắp xếp theo cột nhóm
                //rowGroup: {
                //    dataSrc: 0 // Sử dụng cột nhóm để nhóm dòng
                //},
            }
        }).dataTable;
    }
}
class DocumentDirectory {
    constructor() {
        this.page;
        this.pageGroup;
        this.danhMucHoSo = {
            data: [],
            dataTable: null,
            dataTable_THAYTHE: null,
            getList: function () { },
            displayModal_CRUD: function () { },
            displayModal_Delete: function () { },
            save: function () { },
            delete: function () { },
        };
    }
    init() {
        var dd = this;
        dd.page = $("#page-documentdirectory");
        dd.pageGroup = dd.page.attr("page-group");
        sys.activePage({
            page: dd.page.attr("id"),
            pageGroup: dd.pageGroup
        })
        dd.danhMucHoSo = {
            ...dd.danhMucHoSo,
            getList: function () {
                $.ajax({
                    ...ajaxDefaultProps({
                        url: "/DocumentDirectory/getList",
                    }),
                    success: function (res) {
                        $("#documentdirectory-getList-container").html(res);
                        dd.danhMucHoSo.dataTable = new DanhMucHoSo({
                            tableName: "documentdirectory-getList",
                            $table: $("#documentdirectory-getList", $("#documentdirectory-getList-container"))
                        }).dataTable;
                        /**
                         * Table-checkbox
                         * */
                        treeView({ $table: $("#documentdirectory-getList", $("#documentdirectory-getList-container")) });
                    }
                })
            },
            displayModal_CRUD: function (loai = "", idDanhMuc = 0, idDanhMucCha = 0) {
                $.ajax({
                    ...ajaxDefaultProps({
                        url: "/DocumentDirectory/displayModal_CRUD",
                        type: "POST",
                        data: { loai, idDanhMuc, idDanhMucCha },
                    }),
                    success: function (res) {
                        $("#documentdirectory-crud").html(res);
                        sys.displayModal({
                            name: '#documentdirectory-crud'
                        });
                    }
                });
            },
            save: function (loai) {
                var modalValidtion = htmlEl.activeValidationStates("#documentdirectory-crud");
                if (modalValidtion) {
                    var danhMuc = {
                        IdDanhMucHoSo: $("#input-iddanhmuc", $("#documentdirectory-crud")).val(),
                        IdCha: $("#input-iddanhmuccha", $("#documentdirectory-crud")).val(),
                        CapDo: $("#input-capdo", $("#documentdirectory-crud")).val(),
                        TenDanhMucHoSo: $("#input-tendanhmuc", $("#documentdirectory-crud")).val().trim(),
                        IdPhongLuuTru: $("#select-phongluutru", $("#documentdirectory-crud")).val(),
                        GhiChu: $("#input-ghichu", $("#documentdirectory-crud")).val().trim(),
                    }
                    $.ajax({
                        ...ajaxDefaultProps({
                            url: loai == "create" ? "/DocumentDirectory/create_DanhMuc" : "/DocumentDirectory/update_DanhMuc",
                            type: "POST",
                            data: {
                                str_danhMuc: JSON.stringify(danhMuc),
                            }
                        }),
                        success: function (res) {
                            if (res.status == "success") {
                                sys.alert({ status: res.status, mess: res.mess })
                                dd.danhMucHoSo.getList();
                                sys.displayModal({
                                    name: "#documentdirectory-crud",
                                    displayStatus: "hide"
                                });
                            } else {
                                if (res.mess == "Danh mục hồ sơ đã tồn tại") {
                                    htmlEl.inputValidationStates(
                                        $("#input-tendanhmuc"),
                                        "#documentdirectory-crud",
                                        res.mess,
                                        {
                                            status: true,
                                            isvalid: false
                                        }
                                    )
                                }
                                sys.alert({ status: res.status, mess: res.mess });
                            };
                        }
                    });
                };
            },
            displayModal_Delete: function (deleteChilds = false, idDanhMuc = 0) {
                $.ajax({
                    ...ajaxDefaultProps({
                        url: "/DocumentDirectory/displayModal_Delete",
                        type: "POST",
                        data: {
                            deleteChilds, idDanhMuc
                        }
                    }),
                    success: function (res) {
                        $("#documentdirectory-delete").html(res);
                        dd.danhMucHoSo.dataTable_THAYTHE = new DataTableCustom({
                            name: "danhmuchoso-thaythe-getList",
                            table: $("#danhmuchoso-thaythe-getList"),
                            props: {
                                ordering: false,
                                dom: `<'row'<'col-12 mb-3 d-none'B>>
                                <'row'<'col-sm-12'rt>>
                                <'row'<'col-sm-12 col-md-5'i><'col-sm-12 col-md-7 pt-2'p>>`,
                                lengthMenu: [
                                    [-1],
                                    ['Tất cả'],
                                ],
                            },
                            initCompleteProps: function () { }
                        }).dataTable;
                        // Gán check cho bảng thay thế
                        var rows_THAYTHE = dd.danhMucHoSo.dataTable_THAYTHE.rows().nodes().toArray(); // Tất cả dòng của bảng thay thế
                        singleCheck({
                            name: "danhmuchoso-thaythe-getList",
                            parent: $(rows_THAYTHE)
                        });
                        sys.displayModal({
                            name: '#documentdirectory-delete'
                        });
                    }
                });
            },
            delete: function (deleteChilds = 0, idDanhMuc = 0) {
                var rows_THAYTHE = dd.danhMucHoSo.dataTable_THAYTHE.rows().nodes().toArray(), // Tất cả dòng của bảng thay thế
                    idDanhMuc_THAYTHE = $(`input[type='checkbox']:checked`, $(rows_THAYTHE)).data("id"); // Dòng đang chọn ở bảng thay thế
                deleteChilds = deleteChilds == 1 ? true : false;
                if (idDanhMuc_THAYTHE == null) {
                    sys.alert({ status: "warning", mess: "Bạn chưa chọn danh mục thay thế" })
                } else {
                    $.ajax({
                        ...ajaxDefaultProps({
                            url: "/DocumentDirectory/delete_DanhMucs",
                            type: "POST",
                            data: {
                                deleteChilds, idDanhMuc, idDanhMuc_THAYTHE
                            }
                        }),
                        success: function (res) {
                            sys.alert({ status: res.status, mess: res.mess })
                            dd.danhMucHoSo.getList();
                            sys.displayModal({
                                name: '#documentdirectory-delete',
                                displayStatus: "hide"
                            });
                        }
                    });
                };
            },
        };
        // getList
        dd.danhMucHoSo.getList();
    }
    displayModal_Excel_DanhMuc() {
        var dd = this;
        dd.getList_Excel_DanhMuc("reload");
        sys.displayModal({
            name: '#excel-danhmuc'
        });
    }
    getList_Excel_DanhMuc(loai) {
        var dd = this;
        $.ajax({
            ...ajaxDefaultProps({
                url: "/DocumentDirectory/getList_Excel_DanhMuc",
                type: "POST",
                data: {
                    loai
                }
            }),
            success: function (res) {
                $("#excel-danhmuc-getList-container").html(res);
                /**
                 * ExcelHoSo 
                 */
                dd.create_Excel_DanhMuc();
                /**
                 * Gán các thuộc tính
                 */
                var rows_NEW = dd.excelDanhMuc.dataTable.rows().nodes().toArray(); // Chọn phần thử đầu tiên của bảng
                dd.excelDanhMuc.readRow($(rows_NEW[0]));
                $.each($(".excel-danhmuc-read", $("#excel-danhmuc")), function () {
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
    create_Excel_DanhMuc() {
        var dd = this;
        var containerHeight = $("#excel-danhmuc-getList-container").height() - 10;
        $("#excel-danhmuc-read-container", $("#excel-danhmuc")).height(containerHeight);
        dd.excelDanhMuc = {
            ...dd.excelDanhMuc,
            danhMucDataTables: [],
            dataTable: new DataTableCustom({
                name: "excel-danhmuc-getList",
                table: $("#excel-danhmuc-getList"),
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
                    name: '#excel-danhmuc-capnhattruong',
                    level: 2
                });
            },
            createRow: function () { },
            deleteRow: function () {
                var rows = dd.excelDanhMuc.dataTable.rows().nodes().toArray(),
                    $rowChecks = $(`.checkRow-excel-danhmuc-getList:checked`, rows);
                if ($rowChecks.length == 0) {
                    sys.alert({ status: "warning", mess: "Bạn chưa chọn bản ghi nào" })
                } else {
                    $.each($rowChecks, function () {
                        var $rowCheck = $(this).closest("tr"),
                            rowNumber = $rowCheck.attr("row"),
                            $div = $(`.excel-danhmuc-read[row=${rowNumber}]`, $("#excel-danhmuc"));
                        dd.excelDanhMuc.dataTable.row($rowCheck).remove().draw(); // Xóa dòng đó
                        $div.remove();
                    });
                    // Chọn phần thử đầu tiên của bảng
                    var rows_NEW = dd.excelDanhMuc.dataTable.rows().nodes().toArray();
                    dd.excelDanhMuc.readRow($(rows_NEW[0]));
                };
            },
            readRow: function (el) {
                var rowNumber = $(el).attr("row"),
                    rows = dd.excelDanhMuc.dataTable.rows().nodes().toArray(),
                    $divs = $(".excel-danhmuc-read", $("#excel-danhmuc")),
                    $div = $(`.excel-danhmuc-read[row=${rowNumber}]`, $("#excel-danhmuc"));
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
                var rows = dd.excelDanhMuc.dataTable.rows().nodes().toArray(),
                    $div = $(el).closest(".excel-danhmuc-read"),
                    rowNumber = $div.attr("row");
                var name = $(el).attr("name"),
                    val = $(el).val();
                $.each(rows, function () {
                    if ($(this).attr("row") == rowNumber)
                        $(`span[for="${name}"]`, $(this)).text(val);
                });
            },
            updateMultipleCell: function () {
                var tenDanhMucCha = $("#input-tendanhmuccha", $("#excel-danhmuc-capnhattruong")).val(),
                    rows = dd.excelDanhMuc.dataTable.rows().nodes().toArray(),
                    $rowChecks = $(`.checkRow-excel-danhmuc-getList:checked`, rows);
                if ($rowChecks.length == 0) {
                    sys.alert({ status: "warning", mess: "Bạn chưa chọn bản ghi nào" })
                } else {
                    $.each($rowChecks, function () {
                        var $rowCheck = $(this).closest("tr"),
                            rowNumber = $rowCheck.attr("row"),
                            $div = $(`.excel-danhmuc-read[row=${rowNumber}]`, $("#excel-danhmuc"));
                        // Thay đổi value cho những dòng được chọn
                        $("#input-tendanhmuccha", $div).val(tenDanhMucCha); $("#input-tendanhmuccha", $div).trigger("keyup");
                    });

                    sys.alert({ status: "success", mess: "Cập nhật trường dữ liệu thành công" })
                    sys.displayModal({
                        name: '#excel-danhmuc-capnhattruong',
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
                        url: "/DocumentDirectory/upload_Excel_DanhMuc",
                        type: "POST",
                        data: formData
                    }),
                    contentType: false,
                    processData: false,
                    success: function (res) {
                        dd.getList_Excel_DanhMuc("upload");
                        dd.excelDanhMuc.dataTable.search('').draw();
                        sys.alert({ status: res.status, mess: res.mess });
                    }
                })
            },
            download: function (loaiTaiXuong) {
                var formData = new FormData(),
                    danhMucs = [];
                if (loaiTaiXuong == "data") {
                    var rows = dd.excelDanhMuc.dataTable.rows().nodes().toArray(),
                        $rowChecks = $(`.checkRow-excel-danhmuc-getList:checked`, rows);
                    $.each($rowChecks, function () {
                        var $rowCheck = $(this).closest("tr"),
                            rowNumber = $rowCheck.attr("row"),
                            $div = $(`.excel-danhmuc-read[row=${rowNumber}]`, $("#excel-danhmuc")),
                            danhMuc = {
                                IdDanhMucHoSo: 0,
                                IdCha: 0,
                                DanhMucCha: {
                                    IdDanhMucHoSo: 0,
                                    TenDanhMucHoSo: $("#input-tendanhmuccha", $div).val(),
                                },
                                CapDo: $("#input-capdo", $div).val(),
                                TenDanhMucHoSo: $("#input-tendanhmuc", $div).val().trim(),
                                IdPhongLuuTru: $("#select-phongluutru", $div).val(),
                                GhiChu: $("#input-ghichu", $div).val().trim(),
                            };
                        danhMucs.push(danhMuc);
                    });
                    formData.append("str_danhMucs", JSON.stringify(danhMucs));
                }
                formData.append("loaiTaiXuong", loaiTaiXuong);
                $.ajax({
                    ...ajaxDefaultProps({
                        url: "/DocumentDirectory/get_DanhMucs_download",
                        type: "POST",
                        data: formData
                    }),
                    contentType: false,
                    processData: false,
                    success: function () {
                        sys.alert({ status: "success", mess: "Đã tải xuống thành công" })
                        window.location = "/DocumentDirectory/download_Excel_DanhMuc";
                    }
                })
            },
            reload: function () {
                dd.getList_Excel_DanhMuc("reload");
                dd.excelDanhMuc.dataTable.search('').draw();
                sys.alert({ status: "success", mess: "Đã làm mới dữ liệu" });
            },
            saveByGroup: function () {
                var formData = new FormData(),
                    danhMucs = [];
                $.each($(".excel-danhmuc-read", $("#excel-danhmuc")), function () {
                    var $div = $(this),
                        rowNumber = $div.attr("row"),
                        danhMuc = {
                            IdDanhMucHoSo: 0,
                            IdCha: 0,
                            DanhMucCha: {
                                IdDanhMucHoSo: 0,
                                TenDanhMucHoSo: $("#input-tendanhmuccha", $div).val(),
                            },
                            CapDo: $("#input-capdo", $div).val(),
                            TenDanhMucHoSo: $("#input-tendanhmuc", $div).val().trim(),
                            IdPhongLuuTru: $("#select-phongluutru", $div).val(),
                            GhiChu: $("#input-ghichu", $div).val().trim(),
                        };
                    danhMucs.push(danhMuc);
                });
                formData.append("str_danhMucs", JSON.stringify(danhMucs));
                formData.append("loaiTaiXuong", "data");
                $.ajax({
                    ...ajaxDefaultProps({
                        url: "/DocumentDirectory/get_DanhMucs_download",
                        type: "POST",
                        data: formData
                    }),
                    contentType: false,
                    processData: false,
                    success: function () {
                        dd.excelDanhMuc.save();
                    }
                })
            },
            save: function () {
                $.ajax({
                    ...ajaxDefaultProps({
                        url: "/DocumentDirectory/save_Excel_DanhMuc",
                    }),
                    contentType: false,
                    processData: false,
                    success: function (res) {
                        if (res.status == "success") {
                            sys.displayModal({
                                name: '#excel-danhmuc',
                                displayStatus: "hide"
                            });
                            sys.alert({ status: res.status, mess: res.mess });
                            dd.danhMucHoSo.getList();
                        } else if (res.status == "warning") {
                            // Đẩy lại danh sách dữ liệu chưa hợp lệ
                            dd.getList_Excel_DanhMuc("upload");
                            dd.excelDanhMuc.dataTable.search('').draw();
                            sys.alert({ status: "success", mess: res.mess });
                            dd.danhMucHoSo.getList();
                        } else if (res.status == "error-0") {
                            sys.alert({ status: "error", mess: res.mess })
                        } else {
                            // Đẩy lại danh sách dữ liệu chưa hợp lệ
                            dd.getList_Excel_DanhMuc("upload");
                            dd.excelDanhMuc.dataTable.search('').draw();
                            sys.alert({ status: "error", mess: res.mess })
                        }
                    }
                })
            },
        };
    }
};