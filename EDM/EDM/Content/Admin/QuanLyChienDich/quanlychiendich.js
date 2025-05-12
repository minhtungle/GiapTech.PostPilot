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
                        //data: { locThongTin: quanLyChienDich.baiDang.locThongTin.data }
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
            displayModal_CRUD: function (loai = "", idBaiDang = '00000000-0000-0000-0000-000000000000') {
                // Reset lại các thuộc tính
                quanLyBaiDang.baiDang.handleAnhMoTa.arrAnh = [];

                if (loai == "update") {
                    var idBaiDangs = [];
                    quanLyBaiDang.baiDang.dataTable.rows().iterator('row', function (context, index) {
                        var $row = $(this.row(index).node());
                        if ($row.has("input.checkRow-baidang-getList:checked").length > 0) {
                            idBaiDangs.push($row.attr('id'));
                        };
                    });
                    if (idBaiDangs.length != 1) {
                        sys.alert({ mess: "Yêu cầu chọn 1 bản ghi", status: "warning", timeout: 1500 });
                        return;
                    }
                    else idBaiDang = idBaiDangs[0];
                };
                var input = {
                    Loai: loai,
                    IdBaiDang: idBaiDang,
                };
                $.ajax({
                    ...ajaxDefaultProps({
                        url: "/QuanLyBaiDang/displayModal_CRUD_BaiDang",
                        type: "POST",
                        contentType: "application/json; charset=utf-8",
                        data: {
                            input
                        },
                    }),
                    success: function (res) {
                        $("#baidang-crud").html(res);
                        /**
                          * Gán các thuộc tính
                          */
                        sys.displayModal({
                            name: '#baidang-crud'
                        });
                    }
                })
            },
            save: function (loai) {
                var modalValidtion = htmlEl.activeValidationStates("#baidang-crud");
                if (modalValidtion) {
                    var baiDangs = [];
                    var baiDang = {
                        BaiDang: {
                            IdBaiDang: $("#input-idbaidang").val(),
                            NoiDung: $("#input-noidung").val().trim(),
                            ThoiGian: $("#input-thoigian").val().trim(),
                            IdDonViTien: $("#select-donvitien").val(),
                        },
                        TuTaoAnh: $("#select-tutaoanh").val(),
                        TepDinhKems: []
                    }
                    $.each($(`#anhmota-container tr`), function () {
                        let idTep = $(this).data("id");
                        baiDang.TepDinhKems.push({
                            IdTep: idTep,
                        });
                    });
                    baiDangs.push(baiDang);
                    sys.confirmDialog({
                        mess: `<p>Bạn có thực sự muốn thêm bản ghi này hay không ?</p>`,
                        callback: function () {
                            var formData = new FormData();
                            formData.append("baiDangs", JSON.stringify(baiDangs));
                            formData.append("loai", loai);

                            $.each(quanLyBaiDang.baiDang.handleAnhMoTa.arrAnh, function (idx, anh) {
                                formData.append("files", anh.file);
                            });

                            $.ajax({
                                ...ajaxDefaultProps({
                                    url: loai == "create" ? "/QuanLyBaiDang/create_BaiDang" : "/QuanLyBaiDang/update_BaiDang",
                                    type: "POST",
                                    data: formData,
                                }),
                                //contentType: "application/json; charset=utf-8",  // Chỉ định kiểu nội dung là JSON
                                contentType: false,
                                processData: false,
                                success: function (res) {
                                    if (res.status == "success") {
                                        quanLyBaiDang.baiDang.getList();

                                        sys.displayModal({
                                            name: '#baidang-crud',
                                            displayStatus: "hide"
                                        });
                                        sys.alert({ status: res.status, mess: res.mess });
                                    } else {
                                        if (res.status == "warning") {
                                            htmlEl.inputValidationStates(
                                                $("#input-tenbaidang"),
                                                "#baidang-crud",
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
            delete: function (loai, id) {
                var idBaiDangs = [];
                // Lấy id
                if (loai == "single") {
                    idBaiDangs.push(id)
                } else {
                    quanLyBaiDang.baiDang.dataTable.rows().iterator('row', function (context, index) {
                        var $row = $(this.row(index).node());
                        if ($row.has("input.checkRow-vanban-getList:checked").length > 0) {
                            idBaiDangs.push($row.attr('id'));
                        };
                    });
                };
                // Kiểm tra id
                if (idBaiDangs.length > 0) {
                    var f = new FormData();
                    f.append("idBaiDangs", idBaiDangs.toString());
                    sys.confirmDialog({
                        mess: `Bạn có thực sự muốn xóa bản ghi này hay không ?`,
                        callback: function () {
                            $.ajax({
                                ...ajaxDefaultProps({
                                    url: "/QuanLyBaiDang/delete_BaiDang",
                                    type: "POST",
                                    data: f,
                                }),
                                contentType: false,
                                processData: false,
                                success: function (res) {
                                    sys.alert({ status: res.status, mess: res.mess })
                                    quanLyBaiDang.baiDang.getList();
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
                        url: "/QuanLyChienDich/displayModal_CRUD_BaiDang",
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