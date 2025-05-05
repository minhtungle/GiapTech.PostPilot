'use strict'
class CoCauToChuc {
    constructor({ tableName, $table }) {
        this.tableName = tableName;
        this.$table = $table;
        this.dataTable;
        this.init();
    }
    init() {
        var coCau = this;
        coCau.dataTable = new DataTableCustom({
            name: coCau.tableName,
            table: coCau.$table,
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
class Organization {
    constructor() {
        this.page;
        this.pageGroup;
        this.coCau = {
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
        var org = this;
        org.page = $("#page-organization");
        org.pageGroup = org.page.attr("page-group");
        sys.activePage({
            page: org.page.attr("id"),
            pageGroup: org.pageGroup
        })
        org.coCau = {
            ...org.coCau,
            getList: function () {
                $.ajax({
                    ...ajaxDefaultProps({
                        url: "/Organization/getList",
                    }),
                    success: function (res) {
                        $("#organization-getList-container").html(res);
                        org.coCau.dataTable = new CoCauToChuc({
                            tableName: "organization-getList",
                            $table: $("#organization-getList", $("#organization-getList-container"))
                        }).dataTable;
                        /**
                         * Table-checkbox
                         * */
                        treeView({ $table: $("#organization-getList", $("#organization-getList-container")) });
                    }
                })
            },
            displayModal_CRUD: function (loai = "", idCoCau = '00000000-0000-0000-0000-000000000000', idCoCauCha = '00000000-0000-0000-0000-000000000000') {
                $.ajax({
                    ...ajaxDefaultProps({
                        url: "/Organization/displayModal_CRUD",
                        type: "POST",
                        data: { loai, idCoCau, idCoCauCha },
                    }),
                    success: function (res) {
                        $("#organization-crud").html(res);
                        sys.displayModal({
                            name: '#organization-crud'
                        });
                    }
                });
            },
            save: function (loai) {
                var modalValidtion = htmlEl.activeValidationStates("#organization-crud");
                if (modalValidtion) {
                    var coCau = {
                        IdCoCauToChuc: $("#input-idcocau", $("#organization-crud")).val(),
                        IdCha: $("#input-idcocaucha", $("#organization-crud")).val(),
                        CapDo: $("#input-capdo", $("#organization-crud")).val(),
                        TenCoCauToChuc: $("#input-tencocau", $("#organization-crud")).val().trim(),
                        IdQuanLy: $("#select-quanly", $("#organization-crud")).val().join(','),
                        GhiChu: $("#input-ghichu", $("#organization-crud")).val().trim(),
                    };
                    if (coCau.IdQuanLy != "") coCau.IdQuanLy = `,${coCau.IdQuanLy}`;
                    $.ajax({
                        ...ajaxDefaultProps({
                            url: loai == "create" ? "/Organization/create_CoCau" : "/Organization/update_CoCau",
                            type: "POST",
                            data: {
                                str_coCau: JSON.stringify(coCau),
                            }
                        }),
                        success: function (res) {
                            if (res.status == "success") {
                                org.coCau.getList();
                                sys.displayModal({
                                    name: "#organization-crud",
                                    displayStatus: "hide"
                                });
                                sys.alert({ status: res.status, mess: res.mess });
                            } else if (res.status == "datontai") {
                                htmlEl.inputValidationStates(
                                    $("#input-tencocau"),
                                    "#organization-crud",
                                    res.mess,
                                    {
                                        status: true,
                                        isvalid: false
                                    }
                                );
                                sys.alert({ status: "warning", mess: res.mess });
                            } else if (res.status == "logout") {
                                sys.displayModal({
                                    name: "#organization-crud",
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
                };
            },
            displayModal_Delete: function (deleteChilds = false, idCoCau = '00000000-0000-0000-0000-000000000000') {
                $.ajax({
                    ...ajaxDefaultProps({
                        url: "/Organization/displayModal_Delete",
                        type: "POST",
                        data: {
                            deleteChilds, idCoCau
                        }
                    }),
                    success: function (res) {
                        $("#organization-delete").html(res);
                        org.coCau.dataTable_THAYTHE = new DataTableCustom({
                            name: "cocautochuc-thaythe-getList",
                            table: $("#cocautochuc-thaythe-getList"),
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
                        var rows_THAYTHE = org.coCau.dataTable_THAYTHE.rows().nodes().toArray(); // Tất cả dòng của bảng thay thế
                        singleCheck({
                            name: "cocautochuc-thaythe-getList",
                            parent: $(rows_THAYTHE)
                        });
                        sys.displayModal({
                            name: '#organization-delete'
                        });
                    }
                });
            },
            delete: function (deleteChilds = 0, idCoCau = '00000000-0000-0000-0000-000000000000') {
                var rows_THAYTHE = org.coCau.dataTable_THAYTHE.rows().nodes().toArray(), // Tất cả dòng của bảng thay thế
                    rowChecks_THAYTHE = $(`input[type='checkbox']:checked`, $(rows_THAYTHE)); // Dòng đang chọn ở bảng thay thế
                deleteChilds = deleteChilds == 1 ? true : false;
                if (rowChecks_THAYTHE.length == 0) {
                    sys.alert({ status: "warning", mess: "Bạn chưa chọn cơ cấu thay thế" });
                } else {
                    $.ajax({
                        ...ajaxDefaultProps({
                            url: "/Organization/delete_CoCaus",
                            type: "POST",
                            data: {
                                deleteChilds, idCoCau,
                                idCoCau_THAYTHE: $(rowChecks_THAYTHE[0]).data("id")
                            }
                        }),
                        success: function (res) {
                            if (res.status == "success") {
                                org.coCau.getList();
                                sys.alert({ status: res.status, mess: res.mess });
                                sys.displayModal({
                                    name: '#organization-delete',
                                    displayStatus: "hide"
                                });
                            } else if (res.status == "logout") {
                                sys.displayModal({
                                    name: '#organization-delete',
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
                };
            },
        };
        // getList
        org.coCau.getList();
    }
    displayModal_Excel_CoCau() {
        var org = this;
        org.getList_Excel_CoCau("reload");
        sys.displayModal({
            name: '#excel-cocau'
        });
    }
    getList_Excel_CoCau(loai) {
        var org = this;
        $.ajax({
            ...ajaxDefaultProps({
                url: "/Organization/getList_Excel_CoCau",
                type: "POST",
                data: {
                    loai
                }
            }),
            success: function (res) {
                $("#excel-cocau-getList-container").html(res);
                /**
                 * ExcelHoSo 
                 */
                org.create_Excel_CoCau();
                /**
                 * Gán các thuộc tính
                 */
                var rows_NEW = org.excelCoCau.dataTable.rows().nodes().toArray(); // Chọn phần thử đầu tiên của bảng
                org.excelCoCau.readRow($(rows_NEW[0]));
                $.each($(".excel-cocau-read", $("#excel-cocau")), function () {
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
    create_Excel_CoCau() {
        var org = this;
        var containerHeight = $("#excel-cocau-getList-container").height() - 10;
        $("#excel-cocau-read-container", $("#excel-cocau")).height(containerHeight);
        org.excelCoCau = {
            ...org.excelCoCau,
            coCauDataTables: [],
            dataTable: new DataTableCustom({
                name: "excel-cocau-getList",
                table: $("#excel-cocau-getList"),
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
                    name: '#excel-cocau-capnhattruong',
                    level: 2
                });
            },
            createRow: function () { },
            deleteRow: function () {
                var rows = org.excelCoCau.dataTable.rows().nodes().toArray(),
                    $rowChecks = $(`.checkRow-excel-cocau-getList:checked`, rows);
                if ($rowChecks.length == 0) {
                    sys.alert({ status: "warning", mess: "Bạn chưa chọn bản ghi nào" })
                } else {
                    $.each($rowChecks, function () {
                        var $rowCheck = $(this).closest("tr"),
                            rowNumber = $rowCheck.attr("row"),
                            $div = $(`.excel-cocau-read[row=${rowNumber}]`, $("#excel-cocau"));
                        org.excelCoCau.dataTable.row($rowCheck).remove().draw(); // Xóa dòng đó
                        $div.remove();
                    });
                    // Chọn phần thử đầu tiên của bảng
                    var rows_NEW = org.excelCoCau.dataTable.rows().nodes().toArray();
                    org.excelCoCau.readRow($(rows_NEW[0]));
                };
            },
            readRow: function (el) {
                var rowNumber = $(el).attr("row"),
                    rows = org.excelCoCau.dataTable.rows().nodes().toArray(),
                    $divs = $(".excel-cocau-read", $("#excel-cocau")),
                    $div = $(`.excel-cocau-read[row=${rowNumber}]`, $("#excel-cocau"));
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
                var rows = org.excelCoCau.dataTable.rows().nodes().toArray(),
                    $div = $(el).closest(".excel-cocau-read"),
                    rowNumber = $div.attr("row");
                var name = $(el).attr("name"),
                    val = $(el).val();
                $.each(rows, function () {
                    if ($(this).attr("row") == rowNumber)
                        $(`span[for="${name}"]`, $(this)).text(val);
                });
            },
            updateMultipleCell: function () {
                var tenCoCauCha = $("#input-tencocaucha", $("#excel-cocau-capnhattruong")).val(),
                    rows = org.excelCoCau.dataTable.rows().nodes().toArray(),
                    $rowChecks = $(`.checkRow-excel-cocau-getList:checked`, rows);
                if ($rowChecks.length == 0) {
                    sys.alert({ status: "warning", mess: "Bạn chưa chọn bản ghi nào" })
                } else {
                    $.each($rowChecks, function () {
                        var $rowCheck = $(this).closest("tr"),
                            rowNumber = $rowCheck.attr("row"),
                            $div = $(`.excel-cocau-read[row=${rowNumber}]`, $("#excel-cocau"));
                        // Thay đổi value cho những dòng được chọn
                        $("#input-tencocaucha", $div).val(tenCoCauCha); $("#input-tencocaucha", $div).trigger("keyup");
                    });

                    sys.alert({ status: "success", mess: "Cập nhật trường dữ liệu thành công" })
                    sys.displayModal({
                        name: '#excel-cocau-capnhattruong',
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
                        url: "/Organization/upload_Excel_CoCau",
                        type: "POST",
                        data: formData
                    }),
                    contentType: false,
                    processData: false,
                    success: function (res) {
                        org.getList_Excel_CoCau("upload");
                        org.excelCoCau.dataTable.search('').draw();
                        sys.alert({ status: res.status, mess: res.mess });
                    }
                })
            },
            download: function (loaiTaiXuong) {
                var formData = new FormData(),
                    coCaus = [];
                if (loaiTaiXuong == "data") {
                    var rows = org.excelCoCau.dataTable.rows().nodes().toArray(),
                        $rowChecks = $(`.checkRow-excel-cocau-getList:checked`, rows);
                    $.each($rowChecks, function () {
                        var $rowCheck = $(this).closest("tr"),
                            rowNumber = $rowCheck.attr("row"),
                            $div = $(`.excel-cocau-read[row=${rowNumber}]`, $("#excel-cocau")),
                            coCau = {
                                IdCoCauToChuc: 0,
                                IdCha: 0,
                                CoCauCha: {
                                    IdCoCauToChuc: 0,
                                    TenCoCauToChuc: $("#input-tencocaucha", $div).val(),
                                },
                                CapDo: $("#input-capdo", $div).val(),
                                TenCoCauToChuc: $("#input-tencocau", $div).val().trim(),
                                GhiChu: $("#input-ghichu", $div).val().trim(),
                            };
                        coCaus.push(coCau);
                    });
                    formData.append("str_coCaus", JSON.stringify(coCaus));
                }
                formData.append("loaiTaiXuong", loaiTaiXuong);
                $.ajax({
                    ...ajaxDefaultProps({
                        url: "/Organization/get_CoCaus_download",
                        type: "POST",
                        data: formData
                    }),
                    contentType: false,
                    processData: false,
                    success: function () {
                        sys.alert({ status: "success", mess: "Đã tải xuống thành công" })
                        window.location = "/Organization/download_Excel_CoCau";
                    }
                })
            },
            reload: function () {
                org.getList_Excel_CoCau("reload");
                org.excelCoCau.dataTable.search('').draw();
                sys.alert({ status: "success", mess: "Đã làm mới dữ liệu" });
            },
            saveByGroup: function () {
                var formData = new FormData(),
                    coCaus = [];
                $.each($(".excel-cocau-read", $("#excel-cocau")), function () {
                    var $div = $(this),
                        rowNumber = $div.attr("row"),
                        coCau = {
                            IdCoCauToChuc: 0,
                            IdCha: 0,
                            CoCauCha: {
                                IdCoCauToChuc: 0,
                                TenCoCauToChuc: $("#input-tencocaucha", $div).val(),
                            },
                            CapDo: $("#input-capdo", $div).val(),
                            TenCoCauToChuc: $("#input-tencocau", $div).val().trim(),
                            GhiChu: $("#input-ghichu", $div).val().trim(),
                        };
                    coCaus.push(coCau);
                });
                formData.append("str_coCaus", JSON.stringify(coCaus));
                formData.append("loaiTaiXuong", "data");
                $.ajax({
                    ...ajaxDefaultProps({
                        url: "/Organization/get_CoCaus_download",
                        type: "POST",
                        data: formData
                    }),
                    contentType: false,
                    processData: false,
                    success: function () {
                        org.excelCoCau.save();
                    }
                })
            },
            save: function () {
                $.ajax({
                    ...ajaxDefaultProps({
                        url: "/Organization/save_Excel_CoCau",
                    }),
                    contentType: false,
                    processData: false,
                    success: function (res) {
                        if (res.status == "success") {
                            sys.displayModal({
                                name: '#excel-cocau',
                                displayStatus: "hide"
                            });
                            sys.alert({ status: res.status, mess: res.mess });
                            org.coCau.getList();
                        } else if (res.status == "warning") {
                            // Đẩy lại danh sách dữ liệu chưa hợp lệ
                            org.getList_Excel_CoCau("upload");
                            org.excelCoCau.dataTable.search('').draw();
                            sys.alert({ status: "success", mess: res.mess });
                            org.coCau.getList();
                        } else if (res.status == "error-0") {
                            sys.alert({ status: "error", mess: res.mess })
                        } else {
                            // Đẩy lại danh sách dữ liệu chưa hợp lệ
                            org.getList_Excel_CoCau("upload");
                            org.excelCoCau.dataTable.search('').draw();
                            sys.alert({ status: "error", mess: res.mess })
                        }
                    }
                })
            },
        };
    }
};