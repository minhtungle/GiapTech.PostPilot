'use strict'
class Admin {
    constructor() {
        this.doanhThu = {}
    }
    init() {
        var admin = this;
        var maChucVu_DangSuDung = $("#input-machucvu-dangsudung").val();
        admin.doanhThu = {
            hienThiThongBaoNoiBat: function () {
                $.ajax({
                    ...ajaxDefaultProps({
                        url: "/QuanLyDoanhThu/hienThiThongBaoNoiBat",
                        type: "GET",
                    }),
                    success: function (res) {
                        let { topDoanhThu_Thang, topDoanhThu_Tong, tienDoDoanhThu } = res;
                        if (topDoanhThu_Thang) {
                            let HTML_rank = `
                            <span class="badge bg-danger font-bold">${moment().subtract(1, 'months').format("MM/YYYY") }</span>
                            `;
                            if (topDoanhThu_Thang.length > 0) {
                                HTML_rank += `&nbsp;&nbsp;&nbsp;&nbsp;<span class="text-success font-bold">🥇️: ${topDoanhThu_Thang[0].TenNguoiDung}</span>`;
                            };
                            if (topDoanhThu_Thang.length > 1) {
                                HTML_rank += `&nbsp;&nbsp;&nbsp;&nbsp;<span class="text-warning font-bold">🥈️: ${topDoanhThu_Thang[1].TenNguoiDung}</span>`;
                            };
                            if (topDoanhThu_Thang.length > 2) {
                                HTML_rank += `&nbsp;&nbsp;&nbsp;&nbsp;<span class="text-primary font-bold">🥉: ${topDoanhThu_Thang[2].TenNguoiDung}</span>`;
                            };

                            $(".scrolling-content#thongbao-dong-1", $(".navbar")).html(HTML_rank);
                        };
                        if (topDoanhThu_Tong) {
                            let HTML_rank = `
                            <span class="badge bg-danger font-bold">Tích lũy</span>
                            `;
                            if (topDoanhThu_Tong.length > 0) {
                                HTML_rank += `&nbsp;&nbsp;&nbsp;&nbsp;<span class="text-success font-bold">🥇️: ${topDoanhThu_Tong[0].TenNguoiDung}</span>`;
                            };
                            if (topDoanhThu_Tong.length > 1) {
                                HTML_rank += `&nbsp;&nbsp;&nbsp;&nbsp;<span class="text-warning font-bold">🥈️: ${topDoanhThu_Tong[1].TenNguoiDung}</span>`;
                            };
                            if (topDoanhThu_Tong.length > 2) {
                                HTML_rank += `&nbsp;&nbsp;&nbsp;&nbsp;<span class="text-primary font-bold">🥉: ${topDoanhThu_Tong[2].TenNguoiDung}</span>`;
                            };

                            $(".scrolling-content#thongbao-dong-2", $(".navbar")).html(HTML_rank);
                        };
                    }
                });
            },
            get_MucTieuDoanhThu_CaNhan: function () {
                if (maChucVu_DangSuDung == "NVKD") {
                    $.ajax({
                        ...ajaxDefaultProps({
                            url: "/QuanLyDoanhThu/kiemTra_MucTieuDoanhThu_CaNhan",
                            type: "POST",
                            data: {
                                ngayLenMucTieu: moment().format("MM/YYYY")
                            }
                        }),
                        success: function (res) {
                            if (res == "False") {
                                sys.displayModal({
                                    name: '#muctieudoanhthu-canhan-crud'
                                });
                            };
                        }
                    });
                }
            },
            thietLapMucTieuDoanhThu_CaNhan: function () {
                var modalValidtion = htmlEl.activeValidationStates("#muctieudoanhthu-canhan-crud");
                if (modalValidtion) {
                    sys.confirmDialog({
                        mess: `
                        <p class="font-bold">
                            Có thể chinh sửa tại <span class="text-danger fst-italic">[Thông tin cá nhân]</span>
                        </p>
                        <p>Bạn có thực sự muốn thêm bản ghi này hay không ?</p>
                        `,
                        callback: function () {
                            var nguoiDung_DoanhThu = {
                                DoanhThuMucTieu: $("#input-doanhthumuctieu", $("#muctieudoanhthu-canhan-crud")).val().replaceAll(' ', ''),
                                DoanhThuThucTe: 0,
                                PhanTramHoanThien: 0,
                            };
                            $.ajax({
                                ...ajaxDefaultProps({
                                    url: "/QuanLyDoanhThu/thietLapMucTieuDoanhThu_CaNhan",
                                    type: "POST",
                                    data: {
                                        str_nguoiDung_DoanhThu: JSON.stringify(nguoiDung_DoanhThu),
                                    }
                                }),
                                success: function (res) {
                                    if (res.status == "success") {
                                        sys.displayModal({
                                            name: "#muctieudoanhthu-canhan-crud",
                                            displayStatus: "hide"
                                        });
                                        location.reload();
                                    };
                                    sys.alert({ status: res.status, mess: res.mess, timeout: 5000 });
                                }
                            });
                        }
                    });
                };
            },
            get_MucTieuDoanhThu_CoCau: function () {
                if (maChucVu_DangSuDung == "GD") {
                    $.ajax({
                        ...ajaxDefaultProps({
                            url: "/QuanLyDoanhThu/kiemTra_MucTieuDoanhThu_CoCau",
                            type: "POST",
                            data: {
                                ngayLenMucTieu: moment().format("MM/YYYY")
                            }
                        }),
                        success: function (res) {
                            if (res == "False") {
                                sys.displayModal({
                                    name: '#muctieudoanhthu-cocau-crud'
                                });
                            };
                        }
                    });
                }
            },
            thietLapMucTieuDoanhThu_CoCau: function () {
                var modalValidtion = htmlEl.activeValidationStates("#muctieudoanhthu-cocau-crud");
                if (modalValidtion) {
                    sys.confirmDialog({
                        mess: `
                        <p class="font-bold">
                            Có thể chinh sửa tại <span class="text-danger fst-italic">[Thông tin cá nhân]</span>
                        </p>
                        <p>Bạn có thực sự muốn thêm bản ghi này hay không ?</p>
                        `,
                        callback: function () {
                            var coCauToChuc_DoanhThu = {
                                DoanhThuMucTieu: $("#input-doanhthumuctieu", $("#muctieudoanhthu-cocau-crud")).val().replaceAll(' ', ''),
                                DoanhThuThucTe: 0,
                                PhanTramHoanThien: 0,
                            };
                            $.ajax({
                                ...ajaxDefaultProps({
                                    url: "/QuanLyDoanhThu/thietLapMucTieuDoanhThu_CoCau",
                                    type: "POST",
                                    data: {
                                        str_coCau_DoanhThu: JSON.stringify(coCauToChuc_DoanhThu),
                                    }
                                }),
                                success: function (res) {
                                    if (res.status == "success") {
                                        sys.displayModal({
                                            name: "#muctieudoanhthu-cocau-crud",
                                            displayStatus: "hide"
                                        });
                                    };
                                    sys.alert({ status: res.status, mess: res.mess, timeout: 5000 });
                                }
                            });
                        }
                    });
                };
            },
        };
        //admin.doanhThu.hienThiThongBaoNoiBat();
        //admin.doanhThu.get_MucTieuDoanhThu_CaNhan();
        //admin.doanhThu.get_MucTieuDoanhThu_CoCau();
    }
}