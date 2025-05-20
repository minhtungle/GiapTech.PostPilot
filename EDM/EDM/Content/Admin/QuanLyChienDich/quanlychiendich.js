'use strict'
/**
 * main
 * */
class QuanLyChienDich {
    constructor() {
        this.page;
        this.pageGroup;
        this.chienDich = {}
    }
    init() {
        var quanLyChienDich = this;
        var idNguoiDung_DangSuDung = $("#input-idnguoidung-dangsudung").val();
        quanLyChienDich.page = $("#page-quanlychiendich");

        quanLyChienDich.chienDich = {
            ...quanLyChienDich.chienDich,
            dataTable: null,
            getList: function () {
                $.ajax({
                    ...ajaxDefaultProps({
                        url: "/QuanLyChienDich/getList_ChienDich",
                        type: "GET", // Phải là POST để gửi JSON
                        //contentType: "application/json; charset=utf-8",  // Định dạng JSON
                        //data: { locThongTin: quanLyChienDich.chienDich.locThongTin.data }
                        //dataType: "json",
                    }),
                    //contentType: false,
                    //processData: false,
                    success: function (res) {
                        $("#chiendich-getList-container").html(res);
                        quanLyChienDich.chienDich.dataTable = new DataTableCustom({
                            name: "chiendich-getList",
                            table: $("#chiendich-getList"),
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
            displayModal_CRUD: function (loai = "", idChienDich = '00000000-0000-0000-0000-000000000000') {
                if (loai == "update") {
                    var idChienDichs = [];
                    quanLyChienDich.chienDich.dataTable.rows().iterator('row', function (context, index) {
                        var $row = $(this.row(index).node());
                        if ($row.has("input.checkRow-chiendich-getList:checked").length > 0) {
                            idChienDichs.push($row.attr('id'));
                        };
                    });
                    if (idChienDichs.length != 1) {
                        sys.alert({ mess: "Yêu cầu chọn 1 bản ghi", status: "warning", timeout: 1500 });
                        return;
                    }
                    else idChienDich = idChienDichs[0];
                };
                var input = {
                    Loai: loai,
                    IdChienDich: idChienDich,
                };
                $.ajax({
                    ...ajaxDefaultProps({
                        url: "/QuanLyChienDich/displayModal_CRUD_ChienDich",
                        type: "POST",
                        contentType: "application/json; charset=utf-8",
                        data: {
                            input
                        },
                    }),
                    success: function (res) {
                        $("#chiendich-crud").html(res);
                        /**
                          * Gán các thuộc tính
                          */
                        sys.displayModal({
                            name: '#chiendich-crud'
                        });
                    }
                })
            },
            save: function (loai) {
                var modalValidtion = htmlEl.activeValidationStates("#chiendich-crud");
                if (modalValidtion) {
                    var chienDich = {
                        ChienDich: {
                            IdChienDich: $("#input-idchiendich").val(),
                            TenChienDich: $("#input-tenchiendich").val().trim(),
                        },
                    }
                    sys.confirmDialog({
                        mess: `<p>Bạn có thực sự muốn thêm bản ghi này hay không ?</p>`,
                        callback: function () {
                            var formData = new FormData();
                            formData.append("chienDich", JSON.stringify(chienDich));
                            formData.append("loai", loai);

                            $.ajax({
                                ...ajaxDefaultProps({
                                    url: loai == "create" ? "/QuanLyChienDich/create_ChienDich" : "/QuanLyChienDich/update_ChienDich",
                                    type: "POST",
                                    data: formData,
                                }),
                                //contentType: "application/json; charset=utf-8",  // Chỉ định kiểu nội dung là JSON
                                contentType: false,
                                processData: false,
                                success: function (res) {
                                    if (res.status == "success") {
                                        quanLyChienDich.chienDich.getList();

                                        sys.displayModal({
                                            name: '#chiendich-crud',
                                            displayStatus: "hide"
                                        });
                                        sys.alert({ status: res.status, mess: res.mess });
                                    } else {
                                        if (res.status == "warning") {
                                            htmlEl.inputValidationStates(
                                                $("#input-tenchiendich"),
                                                "#chiendich-crud",
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
            delete: function (loai, idChienDich) {
                var idChienDichs = [];
                // Lấy id
                if (loai == "single") {
                    idChienDichs.push(idChienDich)
                } else {
                    quanLyChienDich.chienDich.dataTable.rows().iterator('row', function (context, index) {
                        var $row = $(this.row(index).node());
                        if ($row.has("input.checkRow-chiendich-getList:checked").length > 0) {
                            idChienDichs.push($row.attr('id'));
                        };
                    });
                };
                // Kiểm tra id
                if (idChienDichs.length > 0) {
                    var f = new FormData();
                    f.append("idChienDichs", JSON.stringify(idChienDichs));
                    sys.confirmDialog({
                        mess: `Bạn có thực sự muốn xóa bản ghi này hay không ?`,
                        callback: function () {
                            $.ajax({
                                ...ajaxDefaultProps({
                                    url: "/QuanLyChienDich/delete_ChienDichs",
                                    type: "POST",
                                    data: f,
                                }),
                                contentType: false,
                                processData: false,
                                success: function (res) {
                                    sys.alert({ status: res.status, mess: res.mess })
                                    quanLyChienDich.chienDich.getList();
                                }
                            })
                        }
                    })
                } else {
                    sys.alert({ mess: "Bạn chưa chọn bản ghi nào", status: "warning", timeout: 1500 })
                }
            },
            xemChiTiet: function (idChienDich = '00000000-0000-0000-0000-000000000000') {
                $.ajax({
                    ...ajaxDefaultProps({
                        url: "/QuanLyChienDich/displayModal_CRUD_ChienDich",
                        type: "POST",
                        contentType: "application/json; charset=utf-8",
                        data: {
                            idChienDich
                        },
                    }),
                    success: function (res) {
                        $("#chiendich-crud").html(res);
                        /**
                          * Gán các thuộc tính
                          */
                        sys.displayModal({
                            name: '#chiendich-crud'
                        });
                    }
                })
            }
        };

        quanLyChienDich.chienDich.getList();
        sys.activePage({
            page: quanLyChienDich.page.attr("id"),
            pageGroup: quanLyChienDich.pageGroup
        });
    }
};