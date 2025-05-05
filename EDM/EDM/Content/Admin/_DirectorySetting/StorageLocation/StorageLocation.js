'use strict'
class ViTriLuuTru {
    constructor({ tableName, $table }) {
        this.tableName = tableName;
        this.$table = $table;
        this.dataTable;
        this.init();
    }
    init() {
        var viTri = this;
        viTri.dataTable = new DataTableCustom({
            name: viTri.tableName,
            table: viTri.$table,
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
class StorageLocation {
    constructor() {
        this.page;
        this.pageGroup;
        this.viTriLuuTru = {
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
        var sl = this;
        sl.page = $("#page-storagelocation");
        sl.pageGroup = sl.page.attr("page-group");
        sys.activePage({
            page: sl.page.attr("id"),
            pageGroup: sl.pageGroup
        })
        sl.viTriLuuTru = {
            ...sl.viTriLuuTru,
            getList: function () {
                $.ajax({
                    ...ajaxDefaultProps({
                        url: "/StorageLocation/getList",
                    }),
                    success: function (res) {
                        $("#storagelocation-getList-container").html(res);
                        sl.viTriLuuTru.dataTable = new ViTriLuuTru({
                            tableName: "storagelocation-getList",
                            $table: $("#storagelocation-getList", $("#storagelocation-getList-container"))
                        }).dataTable;
                        /**
                         * Table-checkbox
                         * */
                        treeView({ $table: $("#storagelocation-getList", $("#storagelocation-getList-container")) });
                    }
                })
            },
            timKiem: function (el) {
                var noiDung = $(el).val();
                $.ajax({
                    ...ajaxDefaultProps({
                        url: "/StorageLocation/timKiem",
                        type: "POST",
                        data: {
                            noiDung
                        }
                    }),
                    success: function (res) {
                        $("#storagelocation-getList-container").html(res);
                        sl.viTriLuuTru.dataTable = new ViTriLuuTru({
                            tableName: "storagelocation-getList",
                            $table: $("#storagelocation-getList", $("#storagelocation-getList-container"))
                        }).dataTable;
                        /**
                         * Table-checkbox
                         * */
                        treeView({ $table: $("#storagelocation-getList", $("#storagelocation-getList-container")) });
                    }
                })
            },
            displayModal_CRUD: function (loai = "", idViTri = 0, idViTriCha = 0) {
                $.ajax({
                    ...ajaxDefaultProps({
                        url: "/StorageLocation/displayModal_CRUD",
                        type: "POST",
                        data: { loai, idViTri, idViTriCha },
                    }),
                    success: function (res) {
                        $("#storagelocation-crud").html(res);
                        sys.displayModal({
                            name: '#storagelocation-crud'
                        });
                    }
                });
            },
            save: function (loai) {
                var modalValidtion = htmlEl.activeValidationStates("#storagelocation-crud");
                if (modalValidtion) {
                    var viTri = {
                        IdViTriLuuTru: $("#input-idvitri", $("#storagelocation-crud")).val(),
                        IdCha: $("#input-idvitricha", $("#storagelocation-crud")).val(),
                        CapDo: $("#input-capdo", $("#storagelocation-crud")).val(),
                        TenViTriLuuTru: $("#input-tenvitri", $("#storagelocation-crud")).val().trim(),
                        IdPhongLuuTru: $("#select-phongluutru", $("#storagelocation-crud")).val(),
                        GhiChu: $("#input-ghichu", $("#storagelocation-crud")).val().trim(),
                    }
                    $.ajax({
                        ...ajaxDefaultProps({
                            url: loai == "create" ? "/StorageLocation/create_ViTri" : "/StorageLocation/update_ViTri",
                            type: "POST",
                            data: {
                                str_viTris: JSON.stringify(viTri),
                            }
                        }),
                        success: function (res) {
                            sys.alert({ status: res.status, mess: res.mess })
                            sl.viTriLuuTru.getList();
                            sys.displayModal({
                                name: "#storagelocation-crud",
                                displayStatus: "hide"
                            });
                        }
                    });
                };
            },
            displayModal_Delete: function (deleteChilds = false, idViTri = 0) {
                $.ajax({
                    ...ajaxDefaultProps({
                        url: "/StorageLocation/displayModal_Delete",
                        type: "POST",
                        data: {
                            deleteChilds, idViTri
                        }
                    }),
                    success: function (res) {
                        $("#storagelocation-delete").html(res);
                        sl.viTriLuuTru.dataTable_THAYTHE = new ViTriLuuTru({
                            tableName: "vitriluutru-thaythe-getList",
                            $table: $("#vitriluutru-thaythe-getList", $("#storagelocation-delete"))
                        }).dataTable;
                        // Gán check cho bảng thay thế
                        var rows_THAYTHE = sl.viTriLuuTru.dataTable_THAYTHE.rows().nodes().toArray(); // Tất cả dòng của bảng thay thế
                        singleCheck({
                            name: "vitriluutru-thaythe-getList",
                            parent: $(rows_THAYTHE)
                        });
                        sys.displayModal({
                            name: '#storagelocation-delete'
                        });
                    }
                });
            },
            delete: function (deleteChilds = 0, idViTri = 0) {
                var rows_THAYTHE = sl.viTriLuuTru.dataTable_THAYTHE.rows().nodes().toArray(), // Tất cả dòng của bảng thay thế
                    idViTri_THAYTHE = $(`input[type='checkbox']:checked`, $(rows_THAYTHE)).data("id"); // Dòng đang chọn ở bảng thay thế
                deleteChilds = deleteChilds == 1 ? true : false;
                if (idViTri_THAYTHE == null) {
                    sys.alert({ status: "warning", mess: "Bạn chưa chọn vị trí thay thế" })
                } else {
                    $.ajax({
                        ...ajaxDefaultProps({
                            url: "/StorageLocation/delete_ViTris",
                            type: "POST",
                            data: {
                                deleteChilds, idViTri, idViTri_THAYTHE
                            }
                        }),
                        success: function (res) {
                            sys.alert({ status: res.status, mess: res.mess })
                            sl.viTriLuuTru.getList();
                            sys.displayModal({
                                name: '#storagelocation-delete',
                                displayStatus: "hide"
                            });
                        }
                    });
                };
            },
        };
        // getList
        sl.viTriLuuTru.getList();
    }
    displayModal_Excel_ViTri() {
        var sl = this;
        sl.getList_Excel_ViTri("reload");
        sys.displayModal({
            name: '#excel-vitri'
        });
    }
    getList_Excel_ViTri(loai) {
        var sl = this;
        $.ajax({
            ...ajaxDefaultProps({
                url: "/StorageLocation/getList_Excel_ViTri",
                type: "POST",
                data: {
                    loai
                }
            }),
            success: function (res) {
                $("#excel-vitri-getList-container").html(res);
                /**
                 * ExcelHoSo 
                 */
                sl.create_Excel_ViTri();
                /**
                 * Gán các thuộc tính
                 */
                var rows_NEW = sl.excelViTri.dataTable.rows().nodes().toArray(); // Chọn phần thử đầu tiên của bảng
                sl.excelViTri.readRow($(rows_NEW[0]));
                $.each($(".excel-vitri-read", $("#excel-vitri")), function () {
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
    create_Excel_ViTri() {
        var sl = this;
        var containerHeight = $("#excel-vitri-getList-container").height() - 10;
        $("#excel-vitri-read-container", $("#excel-vitri")).height(containerHeight);
        sl.excelViTri = {
            ...sl.excelViTri,
            viTriDataTables: [],
            dataTable: new DataTableCustom({
                name: "excel-vitri-getList",
                table: $("#excel-vitri-getList"),
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
                    name: '#excel-vitri-capnhattruong',
                    level: 2
                });
            },
            createRow: function () { },
            deleteRow: function () {
                var rows = sl.excelViTri.dataTable.rows().nodes().toArray(),
                    $rowChecks = $(`.checkRow-excel-vitri-getList:checked`, rows);
                if ($rowChecks.length == 0) {
                    sys.alert({ status: "warning", mess: "Bạn chưa chọn bản ghi nào" })
                } else {
                    $.each($rowChecks, function () {
                        var $rowCheck = $(this).closest("tr"),
                            rowNumber = $rowCheck.attr("row"),
                            $div = $(`.excel-vitri-read[row=${rowNumber}]`, $("#excel-vitri"));
                        sl.excelViTri.dataTable.row($rowCheck).remove().draw(); // Xóa dòng đó
                        $div.remove();
                    });
                    // Chọn phần thử đầu tiên của bảng
                    var rows_NEW = sl.excelViTri.dataTable.rows().nodes().toArray();
                    sl.excelViTri.readRow($(rows_NEW[0]));
                };
            },
            readRow: function (el) {
                var rowNumber = $(el).attr("row"),
                    rows = sl.excelViTri.dataTable.rows().nodes().toArray(),
                    $divs = $(".excel-vitri-read", $("#excel-vitri")),
                    $div = $(`.excel-vitri-read[row=${rowNumber}]`, $("#excel-vitri"));
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
                var rows = sl.excelViTri.dataTable.rows().nodes().toArray(),
                    $div = $(el).closest(".excel-vitri-read"),
                    rowNumber = $div.attr("row");
                var name = $(el).attr("name"),
                    val = $(el).val();
                $.each(rows, function () {
                    if ($(this).attr("row") == rowNumber)
                        $(`span[for="${name}"]`, $(this)).text(val);
                });
            },
            updateMultipleCell: function () {
                var idLoaiBieuMau = $("#select-loaibieumau", $("#excel-vitri-capnhattruong")).val(),
                    tenViTriCha = $("#input-tenvitricha", $("#excel-vitri-capnhattruong")).val(),
                    rows = sl.excelViTri.dataTable.rows().nodes().toArray(),
                    $rowChecks = $(`.checkRow-excel-vitri-getList:checked`, rows);
                if ($rowChecks.length == 0) {
                    sys.alert({ status: "warning", mess: "Bạn chưa chọn bản ghi nào" })
                } else {
                    $.each($rowChecks, function () {
                        var $rowCheck = $(this).closest("tr"),
                            rowNumber = $rowCheck.attr("row"),
                            $div = $(`.excel-vitri-read[row=${rowNumber}]`, $("#excel-vitri"));
                        // Thay đổi value cho những dòng được chọn
                        $("#select-loaibieumau", $div).val(idLoaiBieuMau); $("#select-loaibieumau", $div).trigger("change");
                        $("#input-tenvitricha", $div).val(tenViTriCha); $("#input-tenvitricha", $div).trigger("keyup");
                    });

                    sys.alert({ status: "success", mess: "Cập nhật trường dữ liệu thành công" })
                    sys.displayModal({
                        name: '#excel-vitri-capnhattruong',
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
                        url: "/StorageLocation/upload_Excel_ViTri",
                        type: "POST",
                        data: formData
                    }),
                    contentType: false,
                    processData: false,
                    success: function (res) {
                        sl.getList_Excel_ViTri("upload");
                        sl.excelViTri.dataTable.search('').draw();
                        sys.alert({ status: res.status, mess: res.mess });
                    }
                })
            },
            download: function (loaiTaiXuong) {
                var formData = new FormData(),
                    viTris = [];
                if (loaiTaiXuong == "data") {
                    var rows = sl.excelViTri.dataTable.rows().nodes().toArray(),
                        $rowChecks = $(`.checkRow-excel-vitri-getList:checked`, rows);
                    $.each($rowChecks, function () {
                        var $rowCheck = $(this).closest("tr"),
                            rowNumber = $rowCheck.attr("row"),
                            $div = $(`.excel-vitri-read[row=${rowNumber}]`, $("#excel-vitri")),
                            viTri = {
                                IdViTriLuuTru: 0,
                                IdCha: 0,
                                ViTriCha: {
                                    IdViTriLuuTru: 0,
                                    TenViTriLuuTru: $("#input-tenvitricha", $div).val(),
                                },
                                CapDo: $("#input-capdo", $div).val(),
                                TenViTriLuuTru: $("#input-tenvitri", $div).val().trim(),
                                IdPhongLuuTru: $("#select-phongluutru", $div).val(),
                                GhiChu: $("#input-ghichu", $div).val().trim(),
                            };
                        /*sl.excelViTri.viTriDataTables[rowNumber].dataTable.rows().iterator('row', function (context, index) {
                            var $row = $(this.row(index).node());
                            if ($row.has("input[type='checkbox']:checked").length > 0) {
                                viTri.IdCha = $("input[type='checkbox']:checked", $row).data("idvitri");
                                viTri.ViTriCha = {
                                    IdViTriLuuTru: $("input[type='checkbox']:checked", $row).data("idvitri"),
                                    TenViTriLuuTru: $("input[type='checkbox']:checked", $row).data("tenvitri"),
                                };
                            };
                        });*/
                        viTris.push(viTri);
                    });
                    formData.append("str_viTris", JSON.stringify(viTris));
                }
                formData.append("loaiTaiXuong", loaiTaiXuong);
                $.ajax({
                    ...ajaxDefaultProps({
                        url: "/StorageLocation/get_ViTris_download",
                        type: "POST",
                        data: formData
                    }),
                    contentType: false,
                    processData: false,
                    success: function () {
                        sys.alert({ status: "success", mess: "Đã tải xuống thành công" })
                        window.location = "/StorageLocation/download_Excel_ViTri";
                    }
                })
            },
            reload: function () {
                sl.getList_Excel_ViTri("reload");
                sl.excelViTri.dataTable.search('').draw();
                sys.alert({ status: "success", mess: "Đã làm mới dữ liệu" });
            },
            saveByGroup: function () {
                var formData = new FormData(),
                    viTris = [];
                $.each($(".excel-vitri-read", $("#excel-vitri")), function () {
                    var $div = $(this),
                        rowNumber = $div.attr("row"),
                        viTri = {
                            IdViTriLuuTru: 0,
                            IdCha: 0,
                            ViTriCha: {
                                IdViTriLuuTru: 0,
                                TenViTriLuuTru: $("#input-tenvitricha", $div).val(),
                            },
                            CapDo: $("#input-capdo", $div).val(),
                            TenViTriLuuTru: $("#input-tenvitri", $div).val().trim(),
                            IdPhongLuuTru: $("#select-phongluutru", $div).val(),
                            GhiChu: $("#input-ghichu", $div).val().trim(),
                        };
                    /*sl.excelViTri.viTriDataTables[rowNumber].dataTable.rows().iterator('row', function (context, index) {
                        var $row = $(this.row(index).node());
                        if ($row.has("input[type='checkbox']:checked").length > 0) {
                            viTri.IdCha = $("input[type='checkbox']:checked", $row).data("idvitri");
                            viTri.ViTriCha = {
                                IdViTriLuuTru: $("input[type='checkbox']:checked", $row).data("idvitri"),
                                TenViTriLuuTru: $("input[type='checkbox']:checked", $row).data("tenvitri"),
                            };
                        };
                    });*/
                    viTris.push(viTri);
                });
                formData.append("str_viTris", JSON.stringify(viTris));
                formData.append("loaiTaiXuong", "data");
                $.ajax({
                    ...ajaxDefaultProps({
                        url: "/StorageLocation/get_ViTris_download",
                        type: "POST",
                        data: formData
                    }),
                    contentType: false,
                    processData: false,
                    success: function () {
                        sl.excelViTri.save();
                    }
                })
            },
            save: function () {
                $.ajax({
                    ...ajaxDefaultProps({
                        url: "/StorageLocation/save_Excel_ViTri",
                    }),
                    contentType: false,
                    processData: false,
                    success: function (res) {
                        if (res.status == "success") {
                            sys.displayModal({
                                name: '#excel-vitri',
                                displayStatus: "hide"
                            });
                            sys.alert({ status: res.status, mess: res.mess });
                            sl.viTriLuuTru.getList();
                        } else if (res.status == "warning") {
                            // Đẩy lại danh sách dữ liệu chưa hợp lệ
                            sl.getList_Excel_ViTri("upload");
                            sl.excelViTri.dataTable.search('').draw();
                            sys.alert({ status: "success", mess: res.mess });
                            sl.viTriLuuTru.getList();
                        } else if (res.status == "error-0") {
                            sys.alert({ status: "error", mess: res.mess })
                        } else {
                            // Đẩy lại danh sách dữ liệu chưa hợp lệ
                            sl.getList_Excel_ViTri("upload");
                            sl.excelViTri.dataTable.search('').draw();
                            sys.alert({ status: "error", mess: res.mess })
                        }
                    }
                })
            },
        };
    }
};