'use strict'
/**
 * main
 * */
class ThongTinCaNhan {
    constructor() {
        this.chatService = $.connection.chatService;
    }
    init() {
        var thongTinCaNhan = this,
            idNguoiDung_DangSuDung = $("#input-idnguoidung-dangsudung").val();
        thongTinCaNhan.nguoiDung = {
            ...thongTinCaNhan.nguoiDung,
            dataTable: new DataTableCustom({
                name: "thongtincanhan-getList",
                table: $("#thongtincanhan-getList"),
                props: {
                    ajax: {
                        url: '/ThongTinCaNhan/getList_DoanhThu',
                        type: "GET",
                    },
                    rowId: 'IdNguoiDung_DoanhThu',
                    columns: [
                        {
                            data: null,
                            className: "text-center",
                            searchable: false,
                            orderable: false,
                            render: function (data, type, row, meta) {
                                return `<input class="form-check-input checkRow-thongtincanhan-getList" type="checkbox"/>`;
                            }
                        },
                        {
                            data: null,
                            className: "text-center",
                            render: function (data, type, row, meta) {
                                return thongTinCaNhan.nguoiDung.hienThiTienDoDoanhThu({
                                    doanhThuMucTieu: data.DoanhThuMucTieu,
                                    doanhThuThucTe: data.DoanhThuThucTe,
                                    phanTramHoanThien: data.PhanTramHoanThien
                                });
                            }
                        },
                        {
                            data: null,
                            className: "text-center",
                            render: function (data, type, row, meta) {
                                return `<span class="">${data.NgayLenMucTieu}</span>`;
                            }
                        },
                        {
                            data: null,
                            className: "text-center",
                            render: function (data, type, row, meta) {
                                if (data.NgayDatMucTieu == null) return `<span class="text-danger font-bold">Chưa đạt</span>`;
                                return `<span class="">${moment(data.NgayDatMucTieu).format('DD-MM-YYYY')}</span>`;
                            }
                        },
                        {
                            data: null,
                            className: "text-center",
                            searchable: false,
                            orderable: false,
                            render: function (data, type, row, meta) {
                                let hienTai = moment().format('MM/YYYY');
                                if (hienTai == data.NgayLenMucTieu) {
                                    return `
                                    <div style="white-space: nowrap">
                                        <a class="btn btn-sm btn-primary" title="Cập nhật" onclick="thongTinCaNhan.nguoiDung.displayModal_thietLapMucTieuDoanhThu_CaNhan()"><i class="bi bi-pencil-square"></i></a>
                                    </div>`;
                                } return '';
                            }
                        }
                    ],
                }
            }).dataTable,
            getThongTinCaNhan: function () {
                $.ajax({
                    ...ajaxDefaultProps({
                        url: "/ThongTinCaNhan/getThongTinCaNhan",
                        type: "POST",
                        data: { idNguoiDung: idNguoiDung_DangSuDung },
                    }),
                    success: function (res) {
                        $("#thongtincanhan-crud").html(res);
                        $("#input-sodienthoai", $("#thongtincanhan-crud")).trigger("keyup"); // Format cho ô này

                        htmlEl = new HtmlElement(); // Chạy lại htmlEL
                        var doiMatKhau = new DoiMatKhau('#doimatkhau', '#input-matkhau-moi', '#input-matkhau-moi-xacnhan');
                        doiMatKhau.kiemTraMatKhau();
                    }
                });
            },
            hienThiTienDoDoanhThu: function ({ doanhThuMucTieu, doanhThuThucTe, phanTramHoanThien }) {
                let _doanhThuMucTieu = sys.convertInt2MoneyFormat(doanhThuMucTieu);
                let _doanhThuThucTe = sys.convertInt2MoneyFormat(doanhThuThucTe);
                let _doanhThuConLai = sys.convertInt2MoneyFormat((doanhThuMucTieu - doanhThuThucTe));
                let trangThai = "";
                let chiSoDoanhThu = `${_doanhThuThucTe} / <span class="font-bold">${_doanhThuMucTieu}</span>`;
                if (phanTramHoanThien < 30) {
                    trangThai = "danger";
                } else if (phanTramHoanThien < 50) {
                    trangThai = "warning";
                } else {
                    trangThai = "success";
                };
                return `
                <div class="progress progress-sm progress-${trangThai} mx-5 mt-4 mb-2">
                    <div class="progress-bar progress-label" role="progressbar"
                        style="width: ${phanTramHoanThien}%" 
                        aria-valuenow="${phanTramHoanThien}"
                        aria-valuemin="0" aria-valuemax="${(phanTramHoanThien > 100 ? phanTramHoanThien : 100)}">
                    </div>
                </div>
                  <div>
                    <span class="text-${trangThai}">${_doanhThuThucTe} / <span class="font-bold">${_doanhThuMucTieu}</span></span>
                    ${(phanTramHoanThien == 100 ? `` : `- <span class="text-danger">còn ${_doanhThuConLai}</span>`)}
                </div>
                `;
            },
            getMucTieuDoanhThu_CaNhan: function () {
                $.ajax({
                    ...ajaxDefaultProps({
                        url: "/ThongTinCaNhan/getMucTieuDoanhThu_CaNhan",
                        type: "GET",
                        data: {
                            ngayLenMucTieu: moment().format("MM/yyy")
                        }
                    }),
                    success: function (res) {
                        var tienDoDoanhThu_VIEW = "";
                        var doanhThu = res.doanhThu;
                        if (doanhThu != null) {
                            tienDoDoanhThu_VIEW = thongTinCaNhan.nguoiDung.hienThiTienDoDoanhThu({
                                doanhThuMucTieu: doanhThu.DoanhThuMucTieu,
                                doanhThuThucTe: doanhThu.DoanhThuThucTe,
                                phanTramHoanThien: doanhThu.PhanTramHoanThien
                            });
                        } else {
                            tienDoDoanhThu_VIEW = "";
                        };
                        $("#process-muctieudoanhthu-canhan").html(tienDoDoanhThu_VIEW);
                    }
                })
            },
            getTongDoanhThu_CaNhan: function () {
                $.ajax({
                    ...ajaxDefaultProps({
                        url: "/ThongTinCaNhan/getTongDoanhThu_CaNhan",
                        type: "GET",
                    }),
                    success: function (res) {
                        var tienDoDoanhThu_VIEW = "";
                        var tongDoanhThu = res.tongDoanhThu,
                            capDo_DoanhThu_HienTai = res.capDo_DoanhThu_HienTai,
                            capDo_DoanhThu_TiepTheo = res.capDo_DoanhThu_TiepTheo;
                        if (res.capDo_DoanhThu_HienTai != null) {
                            $("#tencapdodoanhthu-hientai", $("#process-capdodoanhthu-canhan-container")).text(capDo_DoanhThu_HienTai.TenCapDo_DoanhThu);
                            $("#tencapdodoanhthu-tieptheo", $("#process-capdodoanhthu-canhan-container")).text(capDo_DoanhThu_TiepTheo.TenCapDo_DoanhThu);
                            let phanTramHoanThien = (tongDoanhThu / capDo_DoanhThu_TiepTheo.DoanhThuYeuCau) * 100;
                            tienDoDoanhThu_VIEW = thongTinCaNhan.nguoiDung.hienThiTienDoDoanhThu({
                                doanhThuMucTieu: capDo_DoanhThu_TiepTheo.DoanhThuYeuCau,
                                doanhThuThucTe: tongDoanhThu,
                                phanTramHoanThien: parseFloat(phanTramHoanThien, 2)
                            });
                        } else {
                            tienDoDoanhThu_VIEW = "";
                        };
                        $("#process-capdodoanhthu-canhan").html(tienDoDoanhThu_VIEW);
                    }
                })
            },
            displayModal_thietLapMucTieuDoanhThu_CaNhan: function () {
                var maChucVu = $("#input-machucvu-dangsudung").val();
                if (maChucVu == "NVKD") {
                    //$("#btn-thietLapMucTieuDoanhThu_CaNhan", $("#muctieudoanhthu-canhan-crud")).off().on("click", function () {
                    //    thongTinCaNhan.nguoiDung.thietLapMucTieuDoanhThu_CaNhan();
                    //});
                    sys.displayModal({
                        name: '#muctieudoanhthu-canhan-crud'
                    });
                }
            },
            save: function () {
                var $hinhthuccapnhat = $("#hinhthuccapnhat", $("#thongtincanhan-crud"));
                // Chọn hình thức cập nhật
                if ($("#thongtin-list", $hinhthuccapnhat).hasClass("active")) {
                    var modalValidtion = htmlEl.activeValidationStates("#thongtincanhan-crud #thongtin");
                    if (modalValidtion) {
                        var nguoiDung = {
                            IdNguoiDung: idNguoiDung_DangSuDung,

                            GioiTinh: $("#select-gioitinh", $("#thongtincanhan-crud")).val() == 1 ? true : false,
                            KichHoat: $("#select-kichhoat", $("#thongtincanhan-crud")).val() == 1 ? true : false,

                            TenNguoiDung: $("#input-tennguoidung", $("#thongtincanhan-crud")).val().trim(),

                            Email: $("#input-email", $("#thongtincanhan-crud")).val().trim(),
                            SoDienThoai: $("#input-sodienthoai", $("#thongtincanhan-crud")).val().trim(),
                            SoTaiKhoanNganHang: $("#input-sotaikhoannganhang", $("#thongtincanhan-crud")).val().trim(),
                            NgaySinh: $("#input-ngaysinh", $("#thongtincanhan-crud")).val().trim(),
                            GhiChu: $("#input-ghichu", $("#thongtincanhan-crud")).val().trim(),
                            LinkLienHe: $("#input-linklienhe", $("#thongtincanhan-crud")).val().trim(),
                        };
                        if (nguoiDung.NgaySinh != "")
                            nguoiDung.NgaySinh = moment(nguoiDung.NgaySinh, 'YYYY-MM-DD').format('DD/MM/YYYY');

                        $.ajax({
                            ...ajaxDefaultProps({
                                url: "/ThongTinCaNhan/update_NguoiDung",
                                type: "POST",
                                data: {
                                    str_nguoiDung: JSON.stringify(nguoiDung),
                                }
                            }),
                            success: function (res) {
                                if (res.status == "success") {
                                    thongTinCaNhan.nguoiDung.getThongTinCaNhan();
                                };
                                sys.alert({ status: res.status, mess: res.mess });
                            }
                        });
                    };
                } else {
                    var modalValidtion = htmlEl.activeValidationStates("#thongtincanhan-crud #doimatkhau");
                    if (modalValidtion) {
                        var nguoiDung = {
                            IdNguoiDung: idNguoiDung_DangSuDung,

                            MatKhauCu: $("#input-matkhau-cu", $("#thongtincanhan-crud")).val().trim(),
                            MatKhauMoi: $("#input-matkhau-moi", $("#thongtincanhan-crud")).val().trim(),
                            MatKhauMoi_XacNhan: $("#input-matkhau-moi-xacnhan", $("#thongtincanhan-crud")).val().trim(),
                        };
                        if (nguoiDung.MatKhauMoi != nguoiDung.MatKhauMoi_XacNhan) {
                            htmlEl.inputValidationStates(
                                $("#input-matkhau-moi-xacnhan"),
                                "#thongtincanhan-crud",
                                "Mật khẩu mới chưa chính xác",
                                {
                                    status: true,
                                    isvalid: false
                                }
                            );
                        } else {
                            $.ajax({
                                ...ajaxDefaultProps({
                                    url: "/ThongTinCaNhan/capNhat_MatKhau",
                                    type: "POST",
                                    data: {
                                        str_nguoiDung: JSON.stringify(nguoiDung),
                                    }
                                }),
                                success: function (res) {
                                    if (res.status == "success") {
                                        if (res.status == "success") {
                                            thongTinCaNhan.nguoiDung.getThongTinCaNhan();
                                        };
                                        sys.alert({ status: res.status, mess: res.mess });
                                    } else if (res.status == "matkhaucuchuachinhxac") {
                                        htmlEl.inputValidationStates(
                                            $("#input-matkhau-cu"),
                                            "#thongtincanhan-crud",
                                            "Mật khẩu cũ chưa chính xác",
                                            {
                                                status: true,
                                                isvalid: false
                                            }
                                        );
                                        sys.alert({ status: "warning", mess: res.mess });
                                    } else if (res.status == "logout") {
                                        sys.displayModal({
                                            name: "#useraccount-crud",
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
                    };
                };
            }
        };
        thongTinCaNhan.nguoiDung.getThongTinCaNhan();
        thongTinCaNhan.nguoiDung.getTongDoanhThu_CaNhan();
        thongTinCaNhan.nguoiDung.getMucTieuDoanhThu_CaNhan();
    }
};