'use strict'
/**
 * digitizingsetting-table
 * */
class TruongDuLieu {
    constructor({ tableName, $table }) {
        this.tableName = tableName;
        this.$table = $table;
        this.dataTable;
    }
    init() {
        var truongDuLieu = this;
        truongDuLieu.dataTable = new DataTableCustom({
            name: truongDuLieu.tableName,
            table: truongDuLieu.$table,
            props: {
                dom: `
                <'row'<'col-sm-12 col-md-6'l>>
                <'row'<'col-sm-12'rt>>
                <'row'<'col-sm-12 col-md-5'i><'col-sm-12 col-md-7 pt-2'p>>`,
                stateSave: false,
                lengthMenu: [
                    [5, 10, 15, -1],
                    [5, 10, 15, 'Tất cả'],
                ],
                createdRow: function (row, data, dataIndex) {
                    $(row).addClass("r");
                    var dataId = $(row).attr("data-id");
                    if (dataId == undefined)
                        $(row).attr("data-id", 0);
                },
                columnDefs: [{
                    className: 'text-center',
                    target: [0, 1]
                }]
            },
        }).dataTable;
    }
    create(loai = "", stt = 0) {
        var truongDuLieu = this,
            $tr = [];
        if (loai == "excel") {
            $tr = [
                `<input type="text" class="form-control input-tentruong" />`,
                `<a href="#" class="btn btn-sm btn-danger" title="Xóa bỏ" onclick="ds.excelBieuMau.truongDuLieuDataTables[${stt}].delete(this)"><i class="bi bi-trash3-fill"></i></a>`
            ]
        } else {
            $tr = [
                //`<input type="text" class="form-control input-matruong" />`,
                `<input type="text" class="form-control input-tentruong" />`,
                `<a href="#" class="btn btn-sm btn-danger" title="Xóa bỏ" onclick="ds.truongDuLieu.delete(this)"><i class="bi bi-trash3-fill"></i></a>`
            ]
        };
        truongDuLieu.dataTable.row.add($tr).draw(false);
    }
    delete(e) {
        var truongDuLieu = this,
            $tr = $(e).closest("tr");
        truongDuLieu.dataTable.row($tr).remove().draw();
    }
}
/**
 * main
 * */
class DigitizingSetting {
    constructor() {
        this.page;
        this.pageGroup;
        this.bieuMau = {
            data: [],
            idBieuMaus_XOA: [],
            dataTable: null,
        };
    }
    init() {
        var ds = this;
        ds.page = $("#page-digitizingsetting");
        ds.pageGroup = ds.page.attr("page-group");
        ds.bieuMau = {
            ...ds.bieuMau,
            dataTable: new DataTableCustom({
                name: "digitizingsetting-getList",
                table: $("#digitizingsetting-getList"),
                props: {
                    ajax: {
                        url: '/DigitizingSetting/getList',
                        type: "GET",
                    },
                    rowId: 'IdBieuMau',
                    columns: [
                        {
                            className: "text-center",
                            defaultContent: `<input class="form-check-input checkRow-digitizingsetting-getList" type="checkbox"/>`,
                            searchable: false,
                            orderable: false,
                        },
                        {
                            data: "TenBieuMau",
                        },
                        {
                            data: "LoaiBieuMau.TenLoaiBieuMau",
                            className: "text-center",
                        },
                        {
                            data: "IdBieuMau",
                            className: "text-center",
                            searchable: false,
                            orderable: false,
                            render: function (data, type, row, meta) {
                                return type == "display" ?
                                    `
                                <a href="#" class="btn btn-sm btn-primary" title="Cập nhật" onclick="ds.bieuMau.displayModal_CRUD('update', ${data})"><i class="bi bi-pencil-square"></i></a>
                                <a href="#" class="btn btn-sm btn-danger" title="Xóa bỏ" onclick="ds.bieuMau.delete('single',${data})"><i class="bi bi-trash3-fill"></i></a>
                                ` : data;
                                //<a href="#" class="btn btn-sm btn-danger" title="Xóa bỏ" onclick="ds.bieuMau.displayModal_Delete('single',${data})"><i class="bi bi-trash3-fill"></i></a>
                            }
                        }
                    ],
                }
            }).dataTable,
            btnDownload(btn) {
                ds.bieuMau.dataTable.buttons(btn).trigger();
            },
            displayModal_CRUD: function (loai = "", idBieuMau = 0) {
                $.ajax({
                    ...ajaxDefaultProps({
                        url: "/DigitizingSetting/displayModal_CRUD",
                        type: "POST",
                        data: { loai, idBieuMau }
                    }),
                    success: function (res) {
                        $("#digitizingsetting-crud").html(res);
                        sys.displayModal({
                            name: '#digitizingsetting-crud'
                        });
                        /**
                         * table-truongdulieu
                         * */
                        ds.truongDuLieu = new TruongDuLieu({
                            tableName: "table-truongdulieu",
                            $table: $("#table-truongdulieu", $("#digitizingsetting-crud"))
                        });
                        ds.truongDuLieu.init();
                    }
                })
            },
            save: function (loai) {
                var modalValidtion = htmlEl.activeValidationStates("#digitizingsetting-crud");
                if (modalValidtion) {
                    var bieuMau = {
                        IdBieuMau: $("#input-idbieumau", $("#digitizingsetting-crud")).val(),
                        TenBieuMau: $("#input-tenbieumau", $("#digitizingsetting-crud")).val().trim(),
                        IdLoaiBieuMau: $("#select-loaibieumau", $("#digitizingsetting-crud")).val(),
                        TruongDuLieus: []
                    };
                    ds.truongDuLieu.dataTable.rows().iterator('row', function (context, index) {
                        var node = $(this.row(index).node());
                        let truongDuLieu = {
                            IdTruongDuLieu: $(node).attr("data-id"),
                            //MaTruong: $(".input-matruong", node).val(),
                            TenTruong: $(".input-tentruong", node).val()
                        };
                        if (truongDuLieu.TenTruong != "") bieuMau.TruongDuLieus.push(truongDuLieu);
                    });
                    if (bieuMau.TruongDuLieus.length == 0) {
                        sys.alert({ status: "warning", mess: "Bạn chưa tạo trường dữ liệu" })
                    } else {
                        $.ajax({
                            ...ajaxDefaultProps({
                                url: loai == "create" ? "/DigitizingSetting/create_BieuMau" : "/DigitizingSetting/update_BieuMau",
                                type: "POST",
                                data: {
                                    str_bieuMau: JSON.stringify(bieuMau),
                                }
                            }),
                            success: function (res) {
                                if (res.status == "success") {
                                    ds.bieuMau.dataTable.ajax.reload(function () {
                                        sys.displayModal({
                                            name: "#digitizingsetting-crud",
                                            displayStatus: "hide"
                                        });
                                        sys.alert({ status: res.status, mess: res.mess })
                                    }, false);
                                } else {
                                    if (res.mess == "Biểu mẫu đã tồn tại") {
                                        htmlEl.inputValidationStates(
                                            $("#input-tenbieumau"),
                                            "#digitizingsetting-crud",
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
                };
            },

            delete: function (loai, idBieuMau = 0) {
                //#region Kiểm tra điều kiện
                ds.bieuMau.idBieuMaus_XOA.length = 0;
                if (loai == "single") {
                    ds.bieuMau.idBieuMaus_XOA.push(idBieuMau)
                } else {
                    var rows = ds.bieuMau.dataTable.rows().nodes().toArray(),
                        $rowChecks = $(`.checkRow-digitizingsetting-getList:checked`, rows);
                    if ($rowChecks.length == 0) {
                        sys.alert({ status: "warning", mess: "Bạn chưa chọn bản ghi nào" });
                        return 0;
                    } else if ($rowChecks.length == rows.length) {
                        sys.alert({ status: "warning", mess: "Không thể xóa toàn bộ bởi cần giữ lại 1 bản ghi thay thế cho các văn bản đang sử dụng" });
                        return 0;
                    } else {
                        $.each($rowChecks, function () {
                            var $rowCheck = $(this).closest("tr");
                            ds.bieuMau.idBieuMaus_XOA.push($rowCheck.attr("id"));
                        });
                    };
                };
                //#endregion
                if (ds.bieuMau.idBieuMaus_XOA.length > 0) {
                    var f = new FormData();
                    f.append("str_idBieuMaus_XOA", ds.bieuMau.idBieuMaus_XOA.toString());
                    f.append("idBieuMau_THAYTHE", 0);
                    sys.confirmDialog({
                        mess: `
                        <p class="font-bold">Dữ liệu của biểu mẫu đã được gán sẽ bị xóa
                        </p>
                        <p>Bạn có thực sự muốn xóa bản ghi này hay không ?</p>
                        `,
                        callback: function () {
                            $.ajax({
                                ...ajaxDefaultProps({
                                    url: "/DigitizingSetting/delete_BieuMaus",
                                    type: "POST",
                                    data: f
                                }),
                                contentType: false,
                                processData: false,
                                success: function (res) {
                                    ds.bieuMau.dataTable.ajax.reload(function () {
                                        sys.displayModal({
                                            name: "#digitizingsetting-delete",
                                            displayStatus: "hide"
                                        });
                                        sys.alert({ status: res.status, mess: res.mess })
                                    }, false);
                                }
                            });
                        }
                    });
                };
            },
            //#region Không dùng nữa
            _displayModal_Delete: function (loai, idBieuMau = 0) {
                //#region Kiểm tra điều kiện
                ds.bieuMau.idBieuMaus_XOA.length = 0;
                if (loai == "single") {
                    ds.bieuMau.idBieuMaus_XOA.push(idBieuMau)
                } else {
                    var rows = ds.bieuMau.dataTable.rows().nodes().toArray(),
                        $rowChecks = $(`.checkRow-digitizingsetting-getList:checked`, rows);
                    if ($rowChecks.length == 0) {
                        sys.alert({ status: "warning", mess: "Bạn chưa chọn bản ghi nào" });
                        return 0;
                    } else if ($rowChecks.length == rows.length) {
                        sys.alert({ status: "warning", mess: "Không thể xóa toàn bộ bởi cần giữ lại 1 bản ghi thay thế cho các văn bản đang sử dụng" });
                        return 0;
                    } else {
                        $.each($rowChecks, function () {
                            var $rowCheck = $(this).closest("tr");
                            ds.bieuMau.idBieuMaus_XOA.push($rowCheck.attr("id"));
                        });
                    };
                };
                //#endregion
                if (ds.bieuMau.idBieuMaus_XOA.length > 0) {
                    var f = new FormData();
                    f.append("str_idBieuMaus_XOA", ds.bieuMau.idBieuMaus_XOA.toString());
                    $.ajax({
                        ...ajaxDefaultProps({
                            url: "/DigitizingSetting/displayModal_Delete",
                            type: "POST",
                            data: f,
                        }),
                        contentType: false,
                        processData: false,
                        success: function (res) {
                            $("#digitizingsetting-delete").html(res);
                            ds.bieuMau.dataTable_THAYTHE = new DataTableCustom({
                                name: "digitizingsetting-thaythe-getList",
                                table: $("#digitizingsetting-thaythe-getList"),
                                props: {
                                    dom: `
                                <'row'<'col-sm-12'rt>>
                                <'row'<'col-sm-12 col-md-4 pt-2'l><'col-sm-12 col-md-4 text-center'i><'col-sm-12 col-md-4 pt-2'p>>`,
                                },
                                initCompleteProps: function () { }
                            }).dataTable;
                            // Gán check cho bảng thay thế
                            var rows_THAYTHE = ds.bieuMau.dataTable_THAYTHE.rows().nodes().toArray(); // Tất cả dòng của bảng thay thế
                            singleCheck({
                                name: "digitizingsetting-thaythe-getList",
                                parent: $(rows_THAYTHE)
                            });
                            sys.displayModal({
                                name: "#digitizingsetting-delete"
                            });
                        }
                    });
                };
            },
            _delete: function () {
                var rows_THAYTHE = ds.bieuMau.dataTable_THAYTHE.rows().nodes().toArray(), // Tất cả dòng của bảng thay thế
                    rowChecks_THAYTHE = $(`.checkRow-digitizingsetting-thaythe-getList:checked`, $(rows_THAYTHE)); // Dòng đang chọn ở bảng thay thế
                if (rowChecks_THAYTHE.length == 0) {
                    sys.alert({ status: "warning", mess: "Bạn chưa chọn biểu mẫu thay thế" })
                } else {
                    var f = new FormData();
                    f.append("str_idBieuMaus_XOA", ds.bieuMau.idBieuMaus_XOA.toString());
                    f.append("idBieuMau_THAYTHE", $(rowChecks_THAYTHE[0]).data("id"));
                    $.ajax({
                        ...ajaxDefaultProps({
                            url: "/DigitizingSetting/delete_BieuMaus",
                            type: "POST",
                            data: f
                        }),
                        contentType: false,
                        processData: false,
                        success: function (res) {
                            ds.bieuMau.dataTable.ajax.reload(function () {
                                sys.displayModal({
                                    name: "#digitizingsetting-delete",
                                    displayStatus: "hide"
                                });
                                sys.alert({ status: res.status, mess: res.mess })
                            }, false);
                        }
                    });
                };
            }
            //#endregion 
        };
        sys.activePage({
            page: ds.page.attr("id"),
            pageGroup: ds.pageGroup
        });
    }
    displayModal_Excel_BieuMau() {
        var ds = this;
        ds.getList_Excel_BieuMau("reload");
        sys.displayModal({
            name: '#excel-bieumau'
        });
    }
    getList_Excel_BieuMau(loai) {
        var ds = this;
        $.ajax({
            ...ajaxDefaultProps({
                url: "/DigitizingSetting/getList_Excel_BieuMau",
                type: "POST",
                data: {
                    loai
                }
            }),
            success: function (res) {
                $("#excel-bieumau-getList-container").html(res);
                /**
                 * ExcelHoSo 
                 */
                ds.create_Excel_BieuMau();
                /**
                 * Gán các thuộc tính
                 */
                var rows_NEW = ds.excelBieuMau.dataTable.rows().nodes().toArray(); // Chọn phần thử đầu tiên của bảng
                ds.excelBieuMau.readRow($(rows_NEW[0]));
                $.each($(".excel-bieumau-read", $("#excel-bieumau")), function () {
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
    create_Excel_BieuMau() {
        var ds = this;
        var containerHeight = $("#excel-bieumau-getList-container").height() - 10;
        $("#excel-bieumau-read-container", $("#excel-bieumau")).height(containerHeight);
        ds.excelBieuMau = {
            truongDuLieuDataTables: [],
            dataTable: new DataTableCustom({
                name: "excel-bieumau-getList",
                table: $("#excel-bieumau-getList"),
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
                    name: '#excel-bieumau-capnhattruong',
                    level: 2
                });
            },
            createRow: function () { },
            deleteRow: function () {
                var rows = ds.excelBieuMau.dataTable.rows().nodes().toArray(),
                    $rowChecks = $(`.checkRow-excel-bieumau-getList:checked`, rows);
                if ($rowChecks.length == 0) {
                    sys.alert({ status: "warning", mess: "Bạn chưa chọn bản ghi nào" })
                } else {
                    $.each($rowChecks, function () {
                        var $rowCheck = $(this).closest("tr"),
                            rowNumber = $rowCheck.attr("row"),
                            $div = $(`.excel-bieumau-read[row=${rowNumber}]`, $("#excel-bieumau"));
                        ds.excelBieuMau.dataTable.row($rowCheck).remove().draw(); // Xóa dòng đó
                        $div.remove();
                    });
                    // Chọn phần thử đầu tiên của bảng
                    var rows_NEW = ds.excelBieuMau.dataTable.rows().nodes().toArray();
                    ds.excelBieuMau.readRow($(rows_NEW[0]));
                };
            },
            readRow: function (el) {
                var rowNumber = $(el).attr("row"),
                    rows = ds.excelBieuMau.dataTable.rows().nodes().toArray(),
                    $divs = $(".excel-bieumau-read", $("#excel-bieumau")),
                    $div = $(`.excel-bieumau-read[row=${rowNumber}]`, $("#excel-bieumau"));
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
                var rows = ds.excelBieuMau.dataTable.rows().nodes().toArray(),
                    $div = $(el).closest(".excel-bieumau-read"),
                    rowNumber = $div.attr("row");
                $.each(rows, function () {
                    if ($(this).attr("row") == rowNumber)
                        $('span[data-tentruong="TenBieuMau"]', $(this)).text($(el).val());
                });
            },
            updateMultipleCell: function () {
                var idLoaiBieuMau = $("#select-loaibieumau", $("#excel-bieumau-capnhattruong")).val(),
                    rows = ds.excelBieuMau.dataTable.rows().nodes().toArray(),
                    $rowChecks = $(`.checkRow-excel-bieumau-getList:checked`, rows);
                if ($rowChecks.length == 0) {
                    sys.alert({ status: "warning", mess: "Bạn chưa chọn bản ghi nào" })
                } else {
                    $.each($rowChecks, function () {
                        var $rowCheck = $(this).closest("tr"),
                            rowNumber = $rowCheck.attr("row"),
                            $div = $(`.excel-bieumau-read[row=${rowNumber}]`, $("#excel-bieumau"));
                        // Thay đổi value cho những dòng được chọn
                        $("#select-loaibieumau", $div).val(idLoaiBieuMau); $("#select-loaibieumau", $div).trigger("change");
                    });

                    sys.alert({ status: "success", mess: "Cập nhật trường dữ liệu thành công" })
                    sys.displayModal({
                        name: '#excel-bieumau-capnhattruong',
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
                        url: "/DigitizingSetting/upload_Excel_BieuMau",
                        type: "POST",
                        data: formData
                    }),
                    contentType: false,
                    processData: false,
                    success: function (res) {
                        ds.getList_Excel_BieuMau("upload");
                        ds.excelBieuMau.dataTable.search('').draw();
                        sys.alert({ status: res.status, mess: res.mess });
                    }
                })
            },
            download: function (loaiTaiXuong) {
                var formData = new FormData(),
                    bieuMaus = [];
                if (loaiTaiXuong == "data") {
                    var rows = ds.excelBieuMau.dataTable.rows().nodes().toArray(),
                        $rowChecks = $(`.checkRow-excel-bieumau-getList:checked`, rows);
                    $.each($rowChecks, function () {
                        var $rowCheck = $(this).closest("tr"),
                            rowNumber = $rowCheck.attr("row"),
                            $div = $(`.excel-bieumau-read[row=${rowNumber}]`, $("#excel-bieumau")),
                            bieuMau = {
                                IdBieuMau: 0,
                                TenBieuMau: $("#input-tenbieumau", $div).val().trim(),
                                IdLoaiBieuMau: $("#select-loaibieumau", $div).val(),
                                LoaiBieuMau: {
                                    IdLoaiBieuMau: $("#select-loaibieumau", $div).val(),
                                    TenLoaiBieuMau: $("#select-loaibieumau option:selected", $div).text(),
                                },
                                TruongDuLieus: []
                            };
                        ds.excelBieuMau.truongDuLieuDataTables[rowNumber].dataTable.rows().iterator('row', function (context, index) {
                            var node = $(this.row(index).node());
                            let truongDuLieu = {
                                IdTruongDuLieu: $(node).attr("data-id"),
                                TenTruong: $(".input-tentruong", node).val()
                            };
                            if (truongDuLieu.TenTruong != "") bieuMau.TruongDuLieus.push(truongDuLieu);
                        });
                        bieuMaus.push(bieuMau);
                    });
                    formData.append("str_bieuMau", JSON.stringify(bieuMaus));
                }
                formData.append("loaiTaiXuong", loaiTaiXuong);
                $.ajax({
                    ...ajaxDefaultProps({
                        url: "/DigitizingSetting/get_BieuMaus_download",
                        type: "POST",
                        data: formData
                    }),
                    contentType: false,
                    processData: false,
                    success: function () {
                        sys.alert({ status: "success", mess: "Đã tải xuống thành công" })
                        window.location = "/DigitizingSetting/download_Excel_BieuMau";
                    }
                })
            },
            reload: function () {
                ds.getList_Excel_BieuMau("reload");
                ds.excelBieuMau.dataTable.search('').draw();
                sys.alert({ status: "success", mess: "Đã làm mới dữ liệu" });
            },
            saveByGroup: function () {
                var formData = new FormData(),
                    bieuMaus = [];
                $.each($(".excel-bieumau-read", $("#excel-bieumau")), function () {
                    var $div = $(this),
                        rowNumber = $div.attr("row"),
                        bieuMau = {
                            IdBieuMau: 0,
                            TenBieuMau: $("#input-tenbieumau", $div).val().trim(),
                            IdLoaiBieuMau: $("#select-loaibieumau", $div).val(),
                            LoaiBieuMau: {
                                IdLoaiBieuMau: $("#select-loaibieumau", $div).val(),
                                TenLoaiBieuMau: $("#select-loaibieumau option:selected", $div).text(),
                            },
                            TruongDuLieus: []
                        };
                    ds.excelBieuMau.truongDuLieuDataTables[rowNumber].dataTable.rows().iterator('row', function (context, index) {
                        var node = $(this.row(index).node());
                        let truongDuLieu = {
                            IdTruongDuLieu: $(node).attr("data-id"),
                            TenTruong: $(".input-tentruong", node).val()
                        };
                        if (truongDuLieu.TenTruong != "") bieuMau.TruongDuLieus.push(truongDuLieu);
                    });
                    bieuMaus.push(bieuMau);
                });
                formData.append("str_bieuMau", JSON.stringify(bieuMaus));
                formData.append("loaiTaiXuong", "data");
                $.ajax({
                    ...ajaxDefaultProps({
                        url: "/DigitizingSetting/get_BieuMaus_download",
                        type: "POST",
                        data: formData
                    }),
                    contentType: false,
                    processData: false,
                    success: function () {
                        ds.excelBieuMau.save();
                    }
                })
            },
            save: function () {
                $.ajax({
                    ...ajaxDefaultProps({
                        url: "/DigitizingSetting/save_Excel_BieuMau",
                    }),
                    contentType: false,
                    processData: false,
                    success: function (res) {
                        if (res.status == "success") {
                            ds.bieuMau.dataTable.ajax.reload(function () {
                                sys.displayModal({
                                    name: '#excel-bieumau',
                                    displayStatus: "hide"
                                });
                                sys.alert({ status: res.status, mess: res.mess })
                            }, false);
                        } else if (res.status == "warning") {
                            ds.bieuMau.dataTable.ajax.reload(function () {
                                // Đẩy lại danh sách dữ liệu chưa hợp lệ
                                ds.getList_Excel_BieuMau("upload");
                                ds.excelBieuMau.dataTable.search('').draw();
                                sys.alert({ status: "success", mess: res.mess });
                            }, false);
                        } else if (res.status == "error-0") {
                            sys.alert({ status: "error", mess: res.mess })
                        } else {
                            // Đẩy lại danh sách dữ liệu chưa hợp lệ
                            ds.getList_Excel_BieuMau("upload");
                            ds.excelBieuMau.dataTable.search('').draw();
                            sys.alert({ status: "error", mess: res.mess })
                        }
                    }
                })
            },
        };
        /**
         * table-truongdulieu
        * */
        var $truongDuLieuTables = $(".table-truongdulieu", $("#excel-bieumau-read-container"));
        $.each($truongDuLieuTables, function () {
            var truongDuLieuDataTable = new TruongDuLieu({
                tableName: "table-truongdulieu",
                $table: $(this)
            });
            truongDuLieuDataTable.init()
            ds.excelBieuMau.truongDuLieuDataTables.push(truongDuLieuDataTable);
        });
    }
};