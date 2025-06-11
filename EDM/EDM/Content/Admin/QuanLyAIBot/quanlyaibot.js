'use strict'
/**
 * main
 * */
class QuanLyAIBot {
    constructor() {
        this.page;
        this.pageGroup;
        this.aiBot = {}
    }
    init() {
        var quanLyAIBot = this;
        var idNguoiDung_DangSuDung = $("#input-idnguoidung-dangsudung").val();
        quanLyAIBot.page = $("#page-quanlyaibot");

        quanLyAIBot.aiBot = {
            ...quanLyAIBot.aiBot,
            dataTable: null,
            handleAI: {
                taoNoiDungAI: function () {
                    var $modal = $("#aibot-crud");
                    var input = {
                        IdAITool: $(`#select-aitool`, $modal).val(),
                        Prompt: $(`#input-prompt`, $modal).val().trim(),
                        Keywords: $(`#input-keywords`, $modal).val().trim(),
                    };

                    (input.IdAITool && input.Prompt)
                        && $.ajax({
                            ...ajaxDefaultProps({
                                url: "/QuanLyAIBot/taoNoiDungAI",
                                type: "POST",
                                contentType: "application/json; charset=utf-8",
                                data: { input },
                            }),
                            success: function (res) {
                                if (res.status == "success") {
                                    $(`#input-noidung-ai`, $modal).text(res.NoiDung);
                                    $(`#input-noidung-ai`, $modal).val(res.NoiDung);
                                    $(`#input-noidung-ai`, $modal).change();
                                };
                                sys.alert({ mess: res.mess, status: res.status, timeout: 1500 });
                            }
                        })
                },
            },
            getList: function () {
                $.ajax({
                    ...ajaxDefaultProps({
                        url: "/QuanLyAIBot/getList_AIBot",
                        type: "GET", // Phải là POST để gửi JSON
                        //contentType: "application/json; charset=utf-8",  // Định dạng JSON
                        //dataType: "json",
                    }),
                    //contentType: false,
                    //processData: false,
                    success: function (res) {
                        $("#aibot-getList-container").html(res);
                        quanLyAIBot.aiBot.dataTable = new DataTableCustom({
                            name: "aibot-getList",
                            table: $("#aibot-getList"),
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

            displayModal_CRUD: function (loai = "", idAIBot = '00000000-0000-0000-0000-000000000000') {
                var idAIBots = [];
                if (loai == "create") idAIBots.push(idAIBot);
                else {
                    if (idAIBot != '00000000-0000-0000-0000-000000000000')
                        idAIBots.push(idAIBot);
                    else {
                        quanLyAIBot.aiBot.dataTable.rows().iterator('row', function (context, index) {
                            var $row = $(this.row(index).node());
                            if ($row.has("input.checkRow-aibot-getList:checked").length > 0) {
                                idAIBots.push($row.attr('id'));
                            };
                        });
                        if (idAIBots.length != 1) {
                            sys.alert({ mess: "Yêu cầu chọn 1 bản ghi", status: "warning", timeout: 1500 });
                            return;
                        }
                    }
                }
                var input = {
                    Loai: loai,
                    IdAIBot: idAIBots[0],
                };
                $.ajax({
                    ...ajaxDefaultProps({
                        url: "/QuanLyAIBot/displayModal_CRUD_AIBot",
                        type: "POST",
                        contentType: "application/json; charset=utf-8",
                        data: {
                            input
                        },
                    }),
                    success: function (res) {
                        $("#aibot-crud").html(res);
                        /**
                          * Gán các thuộc tính
                          */
                        sys.displayModal({
                            name: '#aibot-crud'
                        });
                    }
                })
            },
            save: function (loai) {
                var modalValidtion = htmlEl.activeValidationStates("#aibot-crud");
                if (modalValidtion) {
                    let $modal = $("#aibot-crud");
                    let LoaiAIBots = $("#select-loaiaibot", $modal).val().map(x => ({
                        IdLoaiAIBot: x,
                    }));
                    let aiBot = {
                        AIBot: {
                            IdAIBot: $("#input-idaibot", $modal).val(),
                            TenAIBot: $("#input-tenaibot", $modal).val().trim(),
                            Prompt: $("#input-prompt", $modal).val().trim(),
                            Keywords: $("#input-keywords", $modal).val().trim(),
                            GhiChu: $("#input-ghichu", $modal).val().trim(),
                        },
                        LoaiAIBots: LoaiAIBots,
                    };
                 
                    sys.confirmDialog({
                        mess: `<p>Bạn có thực sự muốn thêm bản ghi này hay không ?</p>`,
                        callback: function () {
                            var formData = new FormData();
                            formData.append("aiBot", JSON.stringify(aiBot));

                            $.ajax({
                                ...ajaxDefaultProps({
                                    url: loai == "create" ? "/QuanLyAIBot/create_AIBot" : "/QuanLyAIBot/update_AIBot",
                                    type: "POST",
                                    data: formData,
                                }),
                                //contentType: "application/json; charset=utf-8",  // Chỉ định kiểu nội dung là JSON
                                contentType: false,
                                processData: false,
                                success: function (res) {
                                    if (res.status == "success") {
                                        quanLyAIBot.aiBot.getList();
                                        sys.displayModal({
                                            name: '#aibot-crud',
                                            displayStatus: "hide"
                                        });
                                        sys.alert({ status: res.status, mess: res.mess });
                                    } else {
                                        if (res.status == "warning") {
                                            htmlEl.inputValidationStates(
                                                $("#input-tenaibot"),
                                                "#aibot-crud",
                                                res.mess,
                                                {
                                                    status: true,
                                                    isvalid: false
                                                }
                                            )
                                        };
                                        sys.alert({ status: res.status, mess: res.mess });
                                    };
                                }
                            });
                        }
                    });

                };
            },
            delete: function (loai, idAIBot = '00000000-0000-0000-0000-000000000000') {
                var idAIBots = [];
                // Lấy id
                if (loai == "single") {
                    idAIBots.push(idAIBot)
                } else {
                    quanLyAIBot.aiBot.dataTable.rows().iterator('row', function (context, index) {
                        var $row = $(this.row(index).node());
                        if ($row.has("input.checkRow-aibot-getList:checked").length > 0) {
                            idAIBots.push($row.attr('id'));
                        };
                    });
                };
                // Kiểm tra id
                if (idAIBots.length > 0) {
                    var f = new FormData();
                    f.append("idAIBots", JSON.stringify(idAIBots));
                    sys.confirmDialog({
                        mess: `Bạn có thực sự muốn xóa bản ghi này hay không ?`,
                        callback: function () {
                            $.ajax({
                                ...ajaxDefaultProps({
                                    url: "/QuanLyAIBot/delete_AIBots",
                                    type: "POST",
                                    data: f,
                                }),
                                contentType: false,
                                processData: false,
                                success: function (res) {
                                    sys.alert({ status: res.status, mess: res.mess })
                                    quanLyAIBot.aiBot.getList();
                                }
                            })
                        }
                    })
                } else {
                    sys.alert({ mess: "Bạn chưa chọn bản ghi nào", status: "warning", timeout: 1500 })
                }
            },
        };
        quanLyAIBot.loaiAiBot = {
            ...quanLyAIBot.loaiAiBot,
            dataTable: null,
            getList: function () {
                $.ajax({
                    ...ajaxDefaultProps({
                        url: "/QuanLyAIBot/getList_LoaiAIBot",
                        type: "GET", // Phải là POST để gửi JSON
                        //contentType: "application/json; charset=utf-8",  // Định dạng JSON
                        //dataType: "json",
                    }),
                    //contentType: false,
                    //processData: false,
                    success: function (res) {
                        $("#loaiaibot-getList-container").html(res);
                        quanLyAIBot.loaiAiBot.dataTable = new DataTableCustom({
                            name: "loaiaibot-getList",
                            table: $("#loaiaibot-getList"),
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

            displayModal_CRUD: function (loai = "", idLoaiAIBot = '00000000-0000-0000-0000-000000000000') {
                if (loai == "update") {
                    var idLoaiAIBots = [];
                    quanLyAIBot.loaiAiBot.dataTable.rows().iterator('row', function (context, index) {
                        var $row = $(this.row(index).node());
                        if ($row.has("input.checkRow-aibot-getList:checked").length > 0) {
                            idLoaiAIBots.push($row.attr('id'));
                        };
                    });
                    if (idLoaiAIBots.length != 1) {
                        sys.alert({ mess: "Yêu cầu chọn 1 bản ghi", status: "warning", timeout: 1500 });
                        return;
                    }
                    else idLoaiAIBot = idLoaiAIBots[0];
                };
                var input = {
                    Loai: loai,
                    IdLoaiAIBot: idLoaiAIBot,
                };
                $.ajax({
                    ...ajaxDefaultProps({
                        url: "/QuanLyAIBot/displayModal_CRUD_LoaiAIBot",
                        type: "POST",
                        contentType: "application/json; charset=utf-8",
                        data: {
                            input
                        },
                    }),
                    success: function (res) {
                        $("#loaiaibot-crud").html(res);
                        /**
                          * Gán các thuộc tính
                          */
                        sys.displayModal({
                            name: '#loaiaibot-crud'
                        });
                    }
                })
            },
            save: function (loai) {
                var modalValidtion = htmlEl.activeValidationStates("#loaiaibot-crud");
                if (modalValidtion) {
                    let $modal = $("#loaiaibot-crud");
                    let loaiAiBot = {
                        IdLoaiAIBot: $("#input-idloaiaibot", $modal).val(),
                        TenLoaiAIBot: $("#input-tenloaiaibot", $modal).val().trim(),
                        GhiChu: $("#input-ghichu", $modal).val().trim(),
                    };
                    sys.confirmDialog({
                        mess: `<p>Bạn có thực sự muốn thêm bản ghi này hay không ?</p>`,
                        callback: function () {
                            var formData = new FormData();
                            formData.append("loaiAiBot", JSON.stringify(loaiAiBot));

                            $.ajax({
                                ...ajaxDefaultProps({
                                    url: loai == "create" ? "/QuanLyAIBot/create_LoaiAIBot" : "/QuanLyAIBot/update_LoaiAIBot",
                                    type: "POST",
                                    data: formData,
                                }),
                                //contentType: "application/json; charset=utf-8",  // Chỉ định kiểu nội dung là JSON
                                contentType: false,
                                processData: false,
                                success: function (res) {
                                    if (res.status == "success") {
                                        quanLyAIBot.loaiAiBot.getList();
                                        sys.displayModal({
                                            name: '#loaiaibot-crud',
                                            displayStatus: "hide"
                                        });
                                    } else if (res.status == "warning") {
                                        htmlEl.inputValidationStates(
                                            $("#input-tenloaiaibot"),
                                            "#loaiaibot-crud",
                                            res.mess,
                                            {
                                                status: true,
                                                isvalid: false
                                            }
                                        )
                                    };
                                    sys.alert({ status: res.status, mess: res.mess });
                                }
                            });
                        }
                    });

                };
            },
            delete: function (loai, idLoaiAIBot = '00000000-0000-0000-0000-000000000000') {
                var idLoaiAIBots = [];
                // Lấy id
                if (loai == "single") {
                    idLoaiAIBots.push(idLoaiAIBot)
                } else {
                    quanLyAIBot.aiBot.dataTable.rows().iterator('row', function (context, index) {
                        var $row = $(this.row(index).node());
                        if ($row.has("input.checkRow-aibot-getList:checked").length > 0) {
                            idLoaiAIBots.push($row.attr('id'));
                        };
                    });
                };
                // Kiểm tra id
                if (idLoaiAIBots.length > 0) {
                    var f = new FormData();
                    f.append("idAIBots", JSON.stringify(idLoaiAIBots));
                    sys.confirmDialog({
                        mess: `Bạn có thực sự muốn xóa bản ghi này hay không ?`,
                        callback: function () {
                            $.ajax({
                                ...ajaxDefaultProps({
                                    url: "/QuanLyAIBot/delete_LoaiAIBots",
                                    type: "POST",
                                    data: f,
                                }),
                                contentType: false,
                                processData: false,
                                success: function (res) {
                                    sys.alert({ status: res.status, mess: res.mess })
                                    quanLyAIBot.loaiAiBot.getList();
                                }
                            })
                        }
                    })
                } else {
                    sys.alert({ mess: "Bạn chưa chọn bản ghi nào", status: "warning", timeout: 1500 })
                }
            },
        };
        quanLyAIBot.aiBot.getList();
        quanLyAIBot.loaiAiBot.getList();
        sys.activePage({
            page: quanLyAIBot.page.attr("id"),
            pageGroup: quanLyAIBot.pageGroup
        });
    }
    createModalCRUD_AIBot() {
        var quanLyAIBot = this;
        var idChienDich = $("#input-idchiendich").val();

        quanLyAIBot.handleModal_CRUD = {
            dataTable: new DataTableCustom({
                name: "aibot-getList",
                table: $("#aibot-getList", $("#aibot-crud")),
                props: {
                    dom: `
                    <'row'<'col-sm-12'rt>>
                    <'row'<'col-sm-12 col-md-6 text-left'i><'col-sm-12 col-md-6 pt-2'p>>`,
                    lengthMenu: [
                        [10, 50, -1],
                        [10, 50, 'Tất cả'],
                    ],
                }
            }).dataTable,
            displayModal_UpdateMultipleCell: function () {
                var rows = quanLyAIBot.handleModal_CRUD.dataTable.rows().nodes().toArray(),
                    $rowChecks = $(`.checkRow-aibot-getList:checked`, rows);
                if ($rowChecks.length == 0) {
                    sys.alert({ status: "warning", mess: "Bạn chưa chọn bản ghi nào" })
                } else {
                    sys.displayModal({
                        name: '#aibot-crud-capnhattruong',
                        level: 2
                    });
                };
            },
            updateMultipleCell: function () {
                var $modal = $("#aibot-crud");
                var $modal_CapNhatTruong = $("#aibot-crud-capnhattruong");

                var idNenTang = $("#select-nentang", $modal_CapNhatTruong).val(),
                    prompt = $("#input-prompt", $modal_CapNhatTruong).val();
                var rows = quanLyAIBot.handleModal_CRUD.dataTable.rows().nodes().toArray(),
                    $rowChecks = $(`.checkRow-aibot-getList:checked`, rows);
                if ($rowChecks.length == 0) {
                    sys.alert({ status: "warning", mess: "Bạn chưa chọn bản ghi nào" })
                } else {
                    $.each($rowChecks, function () {
                        var $rowCheck = $(this).closest("tr"),
                            rowNumber = $rowCheck.attr("row"),
                            $div = $(`.aibot-read[row=${rowNumber}]`, $modal);
                        // Thay đổi value cho những dòng được chọn
                        $("#select-nentang", $div).val(idNenTang); $("#select-nentang", $div).trigger("change");
                        $("#input-prompt", $div).val(prompt); $("#input-prompt", $div).trigger("change");
                        $("#input-prompt", $div).text(prompt); $("#input-prompt", $div).trigger("change");
                    });

                    sys.alert({ status: "success", mess: "Cập nhật trường dữ liệu thành công" })
                    sys.displayModal({
                        name: '#aibot-crud-capnhattruong',
                        displayStatus: "hide",
                        level: 2,
                    });
                };
            },
            addBanGhi: function () {
                // Tạo mã guid cho bản ghi
                //var guid = sys.generateGUID();
                //#region Thêm bản ghi
                var $modal = $("#aibot-crud");
                $.ajax({
                    ...ajaxDefaultProps({
                        url: "/QuanLyAIBot/addBanGhi_Modal_CRUD",
                        type: "GET",
                        //contentType: "application/json; charset=utf-8",
                    }),
                    success: function (res) {
                        quanLyAIBot.handleModal_CRUD.dataTable.destroy();
                        // Tạo bản ghi mới
                        $("#aibot-getList tbody", $modal).prepend(res.html_aibot_row);
                        $("#aibot-read-container", $modal).prepend(res.html_aibot_read);
                        // Tạo dataTable
                        //if (!quanLyAIBot.handleModal_CRUD.dataTable) {
                        quanLyAIBot.handleModal_CRUD.dataTable = new DataTableCustom({
                            name: "aibot-getList",
                            table: $("#aibot-getList", $modal),
                            props: {
                                dom: `
                                <'row'<'col-sm-12'rt>>
                                <'row'<'col-sm-12 col-md-6 text-left'i><'col-sm-12 col-md-6 pt-2'p>>`,
                                lengthMenu: [
                                    [10, 50, -1],
                                    [10, 50, 'Tất cả'],
                                ],
                            }
                        }).dataTable;
                        //    // Nếu đã khởi tạo, chỉ cần thêm dòng mới (đã prepend bên trên rồi) và vẽ lại bảng
                        //}
                        //quanLyAIBot.handleModal_CRUD.dataTable.row($(res.html_aibot_row)).invalidate().draw(false);
                        // Chọn bản ghi đó
                        var rows_NEW = quanLyAIBot.handleModal_CRUD.dataTable.rows().nodes().toArray(); // Chọn phần thử đầu tiên của bảng
                        quanLyAIBot.handleModal_CRUD.readRow($(rows_NEW[0]));
                        $.each($(".aibot-read", $modal), function () {
                            var $div = $(this),
                                rowNumber = $div.attr("row");
                            // Gán validation
                            htmlEl.validationStates($div);
                            htmlEl.inputMask();
                            var modalValidtion = htmlEl.activeValidationStates($div);
                        });
                        //quanLyAIBot.handleModal_CRUD.readRow(res.idAIBot);
                        /**
                          * Gán các thuộc tính
                          */
                        sys.displayModal({
                            name: '#aibot-crud'
                        });
                    }
                })
                //#endregion
            },
            deleteBanGhi: function () {
                var $modal = $("#aibot-crud");
                var rows = quanLyAIBot.handleModal_CRUD.dataTable.rows().nodes().toArray(),
                    $rowChecks = $(`.checkRow-aibot-getList:checked`, rows);

                if ($rowChecks.length === 0) {
                    sys.alert({ status: "warning", mess: "Bạn chưa chọn bản ghi nào" });
                    return;
                }

                // Nếu số bản ghi được chọn >= tổng số dòng hiện có, cảnh báo và không xóa
                if ($rowChecks.length >= rows.length) {
                    sys.alert({ status: "warning", mess: "Phải giữ lại ít nhất một bản ghi!" });
                    return;
                }

                // Tiến hành xóa các bản ghi được chọn
                $.each($rowChecks, function () {
                    var $rowCheck = $(this).closest("tr"),
                        rowNumber = $rowCheck.attr("row"),
                        $div = $(`.aibot-read[row=${rowNumber}]`, $modal);
                    // Xóa bản ghi trong div
                    quanLyAIBot.handleModal_CRUD.dataTable.row($rowCheck).remove().draw();
                    $div.remove();
                    // Xóa ảnh trong mảng
                    quanLyAIBot.aiBot.handleAnhMoTa.arrAnh = quanLyAIBot.aiBot.handleAnhMoTa.arrAnh
                        .filter(function (anh) {
                            return anh.rowNumber != rowNumber;
                        });
                });

                // Lấy lại các dòng mới sau khi xóa
                var rows_NEW = quanLyAIBot.handleModal_CRUD.dataTable.rows().nodes().toArray();

                if (rows_NEW.length > 0) {
                    quanLyAIBot.handleModal_CRUD.readRow($(rows_NEW[0]));
                } else {
                    $modal.find(".aibot-read").empty();
                }
            },
            readRow: function (el) {
                var $modal = $("#aibot-crud");
                var rowNumber = $(el).attr("row"),
                    rows = quanLyAIBot.handleModal_CRUD.dataTable.rows().nodes().toArray(),
                    $divs = $(".aibot-read", $modal),
                    $div = $(`.aibot-read[row=${rowNumber}]`, $modal);

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
            save: function () {
                var aiBots = [];
                $.each($(".aibot-read", $("#aibot-crud")), function () {
                    var $div = $(this),
                        rowNumber = $div.attr("row");
                    var aiBot = {
                        RowNumber: rowNumber,
                        AIBot: {
                            IdChienDich: idChienDich,
                            IdAIBot: $("#input-idaibot", $div).val(),
                            IdNenTang: $("#select-nentang", $div).val(),
                            Prompt: $("#input-prompt", $div).val().trim(),
                            NoiDung: $("#input-noidung-ai", $div).val().trim(),
                            ThoiGian: $("#input-thoigian", $div).val(),
                            TuTaoAnhAI: $("#checkbox-sudunganh-ai", $div).is(":checked"),
                        },
                        TepDinhKems: []
                    };
                    aiBots.push(aiBot);
                });

                //$.each($(`#tbody-anhmota-container tbody tr`), function () {
                //    let idTep = $(this).data("id");
                //    aiBot.TepDinhKems.push({
                //        IdTep: idTep,
                //    });
                //});
                sys.confirmDialog({
                    mess: `<p>Bạn có thực sự muốn thêm bản ghi này hay không ?</p>`,
                    callback: function () {
                        var formData = new FormData();
                        formData.append("aiBots", JSON.stringify(aiBots));

                        $.each(quanLyAIBot.aiBot.handleAnhMoTa.arrAnh, function (idx, anh) {
                            formData.append("files", anh.file);
                            formData.append("rowNumbers", anh.rowNumber);
                        });

                        $.ajax({
                            ...ajaxDefaultProps({
                                url: "/QuanLyAIBot/create_AIBot",
                                type: "POST",
                                data: formData,
                            }),
                            //contentType: "application/json; charset=utf-8",  // Chỉ định kiểu nội dung là JSON
                            contentType: false,
                            processData: false,
                            success: function (res) {
                                if (res.status == "success") {
                                    quanLyAIBot.aiBot.getList();

                                    sys.displayModal({
                                        name: '#aibot-crud',
                                        displayStatus: "hide"
                                    });
                                };
                                sys.alert({ status: res.status, mess: res.mess });
                            }
                        });
                    }
                });
            },
        };
    }
};