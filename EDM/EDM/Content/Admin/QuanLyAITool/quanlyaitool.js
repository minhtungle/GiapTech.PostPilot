'use strict'
/**
 * main
 * */
class QuanLyAITool {
    constructor() {
        this.page;
        this.pageGroup;
        this.aiTool = {}
    }
    init() {
        var quanLyAITool = this;
        var idNguoiDung_DangSuDung = $("#input-idnguoidung-dangsudung").val();
        quanLyAITool.page = $("#page-quanlyaitool");

        quanLyAITool.aiTool = {
            ...quanLyAITool.aiTool,
            dataTable: null,
            getList: function () {
                $.ajax({
                    ...ajaxDefaultProps({
                        url: "/QuanLyAITool/getList_AITool",
                        type: "GET", // Phải là POST để gửi JSON
                        //contentType: "application/json; charset=utf-8",  // Định dạng JSON
                        //dataType: "json",
                    }),
                    //contentType: false,
                    //processData: false,
                    success: function (res) {
                        $("#aitool-getList-container").html(res);
                        quanLyAITool.aiTool.dataTable = new DataTableCustom({
                            name: "aitool-getList",
                            table: $("#aitool-getList"),
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

            displayModal_CRUD: function (loai = "", idAITool = '00000000-0000-0000-0000-000000000000') {
                if (loai == "update") {
                    var idAITools = [];
                    if (idAITool == '00000000-0000-0000-0000-000000000000') {
                        quanLyAITool.aiTool.dataTable.rows().iterator('row', function (context, index) {
                            var $row = $(this.row(index).node());
                            if ($row.has("input.checkRow-aitool-getList:checked").length > 0) {
                                idAITools.push($row.attr('id'));
                            };
                        });
                        if (idAITools.length != 1) {
                            sys.alert({ mess: "Yêu cầu chọn 1 bản ghi", status: "warning", timeout: 1500 });
                            return;
                        }
                        else idAITool = idAITools[0];
                    }
                };
                var input = {
                    Loai: loai,
                    IdAITool: idAITool,
                };
                $.ajax({
                    ...ajaxDefaultProps({
                        url: "/QuanLyAITool/displayModal_CRUD_AITool",
                        type: "POST",
                        contentType: "application/json; charset=utf-8",
                        data: {
                            input
                        },
                    }),
                    success: function (res) {
                        $("#aitool-crud").html(res);
                        /**
                          * Gán các thuộc tính
                          */
                        sys.displayModal({
                            name: '#aitool-crud'
                        });
                    }
                })
            },
            save: function (loai) {
                var modalValidtion = htmlEl.activeValidationStates("#aitool-crud");
                if (modalValidtion) {
                    let $modal = $("#aitool-crud");
                    let aiTool = {
                        IdAITool: $("#input-idaitool", $modal).val(),
                        ToolCode: $("#input-toolcode", $modal).val().trim(),
                        ToolName: $("#input-toolname", $modal).val().trim(),
                        ApiEndpoint: $("#input-apiendpoint", $modal).val().trim(),
                        Model: $("#input-model", $modal).val().trim(),
                        APIKey: $("#input-apikey", $modal).val().trim(),
                        AdditionalHeaders: $("#input-additionalheaders", $modal).val().trim(),
                        RequestBodyTemplate: $("#input-requestbodytemplate", $modal).val().trim(),
                        GhiChu: $("#input-ghichu", $modal).val().trim(),
                    };
                    sys.confirmDialog({
                        mess: `<p>Bạn có thực sự muốn thêm bản ghi này hay không ?</p>`,
                        callback: function () {
                            var formData = new FormData();
                            formData.append("aiTool", JSON.stringify(aiTool));

                            $.ajax({
                                ...ajaxDefaultProps({
                                    url: loai == "create" ? "/QuanLyAITool/create_AITool" : "/QuanLyAITool/update_AITool",
                                    type: "POST",
                                    data: formData,
                                }),
                                //contentType: "application/json; charset=utf-8",  // Chỉ định kiểu nội dung là JSON
                                contentType: false,
                                processData: false,
                                success: function (res) {
                                    if (res.status == "success") {
                                        quanLyAITool.aiTool.getList();
                                        sys.displayModal({
                                            name: '#aitool-crud',
                                            displayStatus: "hide"
                                        });
                                        sys.alert({ status: res.status, mess: res.mess });
                                    } else {
                                        if (res.status == "warning") {
                                            htmlEl.inputValidationStates(
                                                $("#input-tenaitool"),
                                                "#aitool-crud",
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
            delete: function (loai, idAITool = '00000000-0000-0000-0000-000000000000') {
                var idAITools = [];
                // Lấy id
                if (loai == "single") {
                    idAITools.push(idAITool)
                } else {
                    quanLyAITool.aiTool.dataTable.rows().iterator('row', function (context, index) {
                        var $row = $(this.row(index).node());
                        if ($row.has("input.checkRow-aitool-getList:checked").length > 0) {
                            idAITools.push($row.attr('id'));
                        };
                    });
                };
                // Kiểm tra id
                if (idAITools.length > 0) {
                    var f = new FormData();
                    f.append("idAITools", JSON.stringify(idAITools));
                    sys.confirmDialog({
                        mess: `Bạn có thực sự muốn xóa bản ghi này hay không ?`,
                        callback: function () {
                            $.ajax({
                                ...ajaxDefaultProps({
                                    url: "/QuanLyAITool/delete_AITools",
                                    type: "POST",
                                    data: f,
                                }),
                                contentType: false,
                                processData: false,
                                success: function (res) {
                                    sys.alert({ status: res.status, mess: res.mess })
                                    quanLyAITool.aiTool.getList();
                                }
                            })
                        }
                    })
                } else {
                    sys.alert({ mess: "Bạn chưa chọn bản ghi nào", status: "warning", timeout: 1500 })
                }
            },
        };
        quanLyAITool.aiTool.getList();
        sys.activePage({
            page: quanLyAITool.page.attr("id"),
            pageGroup: quanLyAITool.pageGroup
        });
    }
};