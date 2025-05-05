'use strict'
class AccountInfo {
    constructor() {
        this.page;
        this.pageGroup;
        this.nguoiDung = {
            data: [],
            dataTable: null,
            save: function () { },
        };
    }
    init() {
        var ai = this;
        ai.page = $("#page-accountinfo");
        ai.pageGroup = ai.page.attr("page-group");
        sys.activePage({
            page: ai.page.attr("id"),
            pageGroup: ai.pageGroup
        });
        ai.nguoiDung = {
            ...ai.nguoiDung,
            getList: function () {
                $.ajax({
                    ...ajaxDefaultProps({
                        url: "/AccountInfo/getList",
                        type: "GET",
                        complete: function (data) {
                            ai.nguoiDung.data = data.responseJSON.data;
                        },
                    }),
                    success: function (res) {
                        $("#accountinfo-getList").html(res);
                        htmlEl.inputMask();
                    }
                })
            },
            save: function () {
                var $hinhthuccapnhat = $("#hinhthuccapnhat", $("#accountinfo-getList"));
                // Chọn hình thức cập nhật
                if ($("#thongtin-list", $hinhthuccapnhat).hasClass("active")) {
                    htmlEl.validationStates("#accountinfo-getList #thongtin");
                    var modalValidtion = htmlEl.activeValidationStates("#accountinfo-getList #thongtin");
                    if (modalValidtion) {
                        var nguoiDung = {
                            IdNguoiDung: $("#input-idnguoidung", $("#accountinfo-getList")).val(),

                            GioiTinh: $("#select-gioitinh", $("#accountinfo-getList")).val() == 1 ? true : false,
                            KichHoat: $("#select-kichhoat", $("#accountinfo-getList")).val() == 1 ? true : false,
                            IdKieuNguoiDung: $("#select-kieunguoidung", $("#accountinfo-getList")).val(),
                            IdCoCauToChuc: $("#select-cocautochuc", $("#accountinfo-getList")).val(),

                            TenNguoiDung: $("#input-tennguoidung", $("#accountinfo-getList")).val().trim(),
                            TenDangNhap: $("#input-tendangnhap", $("#accountinfo-getList")).val().trim(),
                            Email: $("#input-email", $("#accountinfo-getList")).val().trim(),
                            SoDienThoai: $("#input-sodienthoai", $("#accountinfo-getList")).val().trim(),
                            NgaySinh: $("#input-ngaysinh", $("#accountinfo-getList")).val().trim(),
                            ChucVu: $("#input-chucvu", $("#accountinfo-getList")).val().trim(),
                        };
                        if (nguoiDung.NgaySinh != "")
                            nguoiDung.NgaySinh = moment(nguoiDung.NgaySinh, 'YYYY-MM-DD').format('DD/MM/YYYY');
                        $.ajax({
                            ...ajaxDefaultProps({
                                url: "/AccountInfo/update_NguoiDung",
                                type: "POST",
                                data: {
                                    str_nguoiDung: JSON.stringify(nguoiDung),
                                }
                            }),
                            success: function (res) {
                                if (res.status == "success") {
                                    ai.nguoiDung.getList();
                                    sys.logoutDialog({
                                        mess: res.mess,
                                    });
                                } else if (res.status == "datontai") {
                                    if (res.mess == "Tên đăng nhập đã tồn tại") {
                                        htmlEl.inputValidationStates(
                                            $("#input-tendangnhap"),
                                            "#accountinfo-getList",
                                            res.mess,
                                            {
                                                status: true,
                                                isvalid: false
                                            }
                                        );
                                    }
                                    sys.alert({ status: "warning", mess: res.mess });
                                } else {
                                    sys.alert({ status: res.status, mess: res.mess });
                                };
                            }
                        });
                    }
                } else {
                    htmlEl.validationStates("#accountinfo-getList #doimatkhau");
                    var modalValidtion = htmlEl.activeValidationStates("#accountinfo-getList #doimatkhau");
                    if (modalValidtion) {
                        var nguoiDung = {
                            IdNguoiDung: $("#input-idnguoidung", $("#accountinfo-getList")).val(),

                            MatKhauCu: $("#input-matkhau-cu", $("#accountinfo-getList")).val().trim(),
                            MatKhauMoi: $("#input-matkhau-moi", $("#accountinfo-getList")).val().trim(),
                            MatKhauMoi_XacNhan: $("#input-matkhau-moi-xacnhan", $("#accountinfo-getList")).val().trim(),
                        };
                        if (nguoiDung.MatKhauMoi != nguoiDung.MatKhauMoi_XacNhan) {
                            htmlEl.inputValidationStates(
                                $("#input-matkhau-moi-xacnhan"),
                                "#accountinfo-getList",
                                "Mật khẩu mới chưa chính xác",
                                {
                                    status: true,
                                    isvalid: false
                                }
                            );
                        } else {
                            $.ajax({
                                ...ajaxDefaultProps({
                                    url: "/AccountInfo/capNhat_MatKhau",
                                    type: "POST",
                                    data: {
                                        str_nguoiDung: JSON.stringify(nguoiDung),
                                    }
                                }),
                                success: function (res) {
                                    if (res.status == "success") {
                                        ai.nguoiDung.getList();
                                        sys.logoutDialog({
                                            mess: res.mess,
                                        });
                                    } else if (res.status == "matkhaucuchuachinhxac") {
                                        htmlEl.inputValidationStates(
                                            $("#input-matkhau-cu"),
                                            "#accountinfo-getList",
                                            "Mật khẩu cũ chưa chính xác",
                                            {
                                                status: true,
                                                isvalid: false
                                            }
                                        );
                                        sys.alert({ status: "warning", mess: res.mess });
                                    } else {
                                        sys.alert({ status: res.status, mess: res.mess });
                                    };
                                }
                            });
                        };
                    };
                };
            },
        }
    }
}