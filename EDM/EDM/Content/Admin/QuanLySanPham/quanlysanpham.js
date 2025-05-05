'use strict'
/**
 * main
 * */
class QuanLySanPham {
    constructor() {
        this.page;
        this.pageGroup;
        this.sanPham = {
            data: [],
            dataTable: null,
            save: function () { },
            delete: function () { },
            displayModal_CRUD: function () { },
        }
    }
    init() {
        var quanLySanPham = this;
        var idNguoiDung_DangSuDung = $("#input-idnguoidung-dangsudung").val();
        quanLySanPham.page = $("#page-quanlysanpham");

        quanLySanPham.sanPham = {
            ...quanLySanPham.sanPham,
            dataInUsed: {
                SanPham: { IdSanPham: '00000000-0000-0000-0000-000000000000', }
            },
            dataTable: null,
            locThongTin: {
                data: null,
                reload: function () {
                    quanLySanPham.sanPham.locThongTin.data = {
                        IdLoaiSanPham: $("#select-loaisanpham").val(),
                        IdLoaiKhoaHoc: $("#select-loaikhoahoc").val(),
                    };
                },
                timKiem: function () {
                    quanLyLopHoc.lopHoc.choXepLop.getList();
                },
            },
            getList: function () {
                //quanLySanPham.sanPham.locThongTin.reload();

                $.ajax({
                    ...ajaxDefaultProps({
                        url: "/QuanLySanPham/getList",
                        type: "GET", // Phải là POST để gửi JSON
                        //contentType: "application/json; charset=utf-8",  // Định dạng JSON
                        //data: { locThongTin: quanLySanPham.sanPham.locThongTin.data }
                        //dataType: "json",
                    }),
                    //contentType: false,
                    //processData: false,
                    success: function (res) {
                        $("#quanlysanpham-getList-container").html(res);
                        quanLySanPham.sanPham.dataTable = new DataTableCustom({
                            name: "quanlysanpham-getList",
                            table: $("#quanlysanpham-getList"),
                            props: {
                                lengthMenu: [
                                    [5, 10],
                                    [5, 10],
                                ],
                            }
                        }).dataTable;
                    }
                });
            },
            displayModal_CRUD: function (loai = "", idSanPham = '00000000-0000-0000-0000-000000000000') {
                if (loai == "update") {
                    var idSanPhams = [];
                    quanLySanPham.sanPham.dataTable.rows().iterator('row', function (context, index) {
                        var $row = $(this.row(index).node());
                        if ($row.has("input.checkRow-quanlysanpham-getList:checked").length > 0) {
                            idSanPhams.push($row.attr('id'));
                        };
                    });
                    if (idSanPhams.length != 1) {
                        sys.alert({ mess: "Yêu cầu chọn 1 bản ghi", status: "warning", timeout: 1500 });
                        return;
                    }
                    else idSanPham = idSanPhams[0];
                };
                $.ajax({
                    ...ajaxDefaultProps({
                        url: "/QuanLySanPham/displayModal_CRUD",
                        type: "POST",
                        data: { loai, idSanPham }
                    }),
                    success: function (res) {
                        $("#quanlysanpham-crud").html(res);
                        /**
                          * Gán các thuộc tính
                          */
                        sys.displayModal({
                            name: '#quanlysanpham-crud'
                        });
                    }
                })
            },
            tinhGiaTien() {
                var giaTienTungBuoi = $("#input-giatientungbuoi", $("#quanlysanpham-crud")).val() ?? 0,
                    soBuoi = $("#input-sobuoi", $("#quanlysanpham-crud")).val() ?? 0,
                    giaTien = parseInt(giaTienTungBuoi.replaceAll(' ', '')) * parseInt(soBuoi);

                $("#input-giatien", $("#quanlysanpham-crud")).val(giaTien).trigger("keyup");
            },
            save: function (loai) {
                var modalValidtion = htmlEl.activeValidationStates("#quanlysanpham-crud");
                if (modalValidtion) {
                    var sanPham = {
                        SanPham: {
                            IdSanPham: $("#input-idsanpham", $("#quanlysanpham-crud")).val(),
                            IdLoaiSanPham: $("#select-loaisanpham", $("#quanlysanpham-crud")).val(),
                            IdLoaiKhoaHoc: $("#select-loaikhoahoc", $("#quanlysanpham-crud")).val(),
                            TenSanPham: $("#input-tensanpham", $("#quanlysanpham-crud")).val().trim(),
                            GiaTienTungBuoi: $("#input-giatientungbuoi", $("#quanlysanpham-crud")).val().replaceAll(' ', ''),
                            SoBuoi: $("#input-sobuoi", $("#quanlysanpham-crud")).val(),
                            ThoiGianBuoiHoc: $("#input-thoigianbuoihoc", $("#quanlysanpham-crud")).val(),
                            GhiChu: $("#input-ghichu", $("#quanlysanpham-crud")).val().trim(),
                        },
                        LoaiSanPham: {
                            IdLoaiSanPham: $("#select-loaisanpham", $("#quanlysanpham-crud")).val(),
                        },
                        LoaiKhoaHoc: {
                            IdLoaiKhoaHoc: $("#select-loaikhoahoc", $("#quanlysanpham-crud")).val(),
                        },
                    };
                    $.ajax({
                        ...ajaxDefaultProps({
                            url: loai == "create" ? "/QuanLySanPham/create_SanPham" : "/QuanLySanPham/update_SanPham",
                            type: "POST",
                            data: {
                                str_sanPham: JSON.stringify(sanPham),
                            }
                        }),
                        success: function (res) {
                            if (res.status == "success") {
                                quanLySanPham.sanPham.getList();
                                sys.displayModal({
                                    name: '#quanlysanpham-crud',
                                    displayStatus: "hide"
                                });
                                sys.alert({ status: res.status, mess: res.mess });
                            } else {
                                if (res.status == "warning") {
                                    htmlEl.inputValidationStates(
                                        $("#input-tensanpham"),
                                        "#quanlysanpham-crud",
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
                };
            },
            delete: function (loai, idSanPham = '00000000-0000-0000-0000-000000000000') {
                var idSanPhams = [];
                if (loai == "single") {
                    idSanPhams.push(idSanPham);
                } else {
                    quanLySanPham.sanPham.dataTable.rows().iterator('row', function (context, index) {
                        var $row = $(this.row(index).node());
                        if ($row.has("input.checkRow-quanlysanpham-getList:checked").length > 0) {
                            idSanPhams.push($row.attr('id'));
                        };
                    });
                };
                // Kiểm tra idSanPham
                if (idSanPhams.length > 0) {
                    var f = new FormData();
                    f.append("str_idSanPhams", JSON.stringify(idSanPhams));
                    sys.confirmDialog({
                        mess: `
                        <p class="font-bold">Sản phẩm có liên kết với các
                            <span class="text-danger fst-italic"> [Khách hàng]</span> và
                            <span class="text-danger fst-italic"> [Lớp học]</span> 
                        </p>
                        <p>Bạn có thực sự muốn xóa bản ghi này hay không ?</p>
                        `,
                        callback: function () {
                            $.ajax({
                                ...ajaxDefaultProps({
                                    url: "/QuanLySanPham/delete_SanPhams",
                                    type: "POST",
                                    data: f,
                                }),
                                contentType: false,
                                processData: false,
                                success: function (res) {
                                    quanLySanPham.sanPham.getList();
                                    sys.alert({ status: res.status, mess: res.mess })
                                }
                            })
                        }
                    })
                } else {
                    sys.alert({ mess: "Bạn chưa chọn bản ghi nào", status: "warning", timeout: 1500 });
                };
            },
        };
        quanLySanPham.sanPham.getList();
        sys.activePage({
            page: quanLySanPham.page.attr("id"),
            pageGroup: quanLySanPham.pageGroup
        });
    }
};