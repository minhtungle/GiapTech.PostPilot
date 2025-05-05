'use strict'
class TrangChu {
    constructor() {
        this.page;
        this.pageGroup;
        this.nguoiDung = {
            idNguoiDung_DangSuDung: $("#input-idnguoidung-dangsudung").val(),
            maChucVu_DangSuDung: $("#input-machucvu-dangsudung").val()
        }
        this.doanhThuMoiNhat = {
            data: [],
            dataTable: null,
        }
    }
    init() {
        var trangChu = this;
        trangChu.page = $("#page-home");
        sys.activePage({
            page: trangChu.page.attr("id"),
            pageGroup: trangChu.pageGroup
        });
    }
    funcs_DanhCho_QuyenKinhDoanh() {
        var trangChu = this;
        trangChu.doanhThuMoiNhat = {
            dataTable: null,
            getList: function () {
                $.ajax({
                    ...ajaxDefaultProps({
                        url: '/QuanLyDoanhThu/getDoanhThus_MoiNhat',
                        type: "GET", // Phải là POST để gửi JSON
                    }),
                    success: function (res) {
                        $("#doanhthumoinhat-getList-container").html(res);
                        trangChu.doanhThuMoiNhat.dataTable = new DataTableCustom({
                            name: "doanhthumoinhat-getList",
                            table: $("#doanhthumoinhat-getList"),
                            props: {
                                dom: `
                                <'row'<'col-sm-12'rt>>
                                <'row'<'col-sm-12 col-md-6 text-left'i><'col-sm-12 col-md-6 pt-2'p>>`,  
                                lengthMenu: [
                                    [3],
                                    [3],
                                ],
                            }
                        }).dataTable;
                    }
                });
            },
        };
        trangChu.bangXepHang = {
            dataTable: null,
            getList: function () {
                var ngayLenMucTieu = moment().format("MM/YYYY");
                $.ajax({
                    ...ajaxDefaultProps({
                        url: '/QuanLyDoanhThu/getNguoiDungs_BangXepHang',
                        type: "GET", // Phải là POST để gửi JSON
                        data: {
                            ngayLenMucTieu: ngayLenMucTieu
                        },  // Chắc chắn đã dùng JSON.stringify()
                    }),
                    success: function (res) {
                        $("#bangxephang-getList-container").html(res);
                        trangChu.bangXepHang.dataTable = new DataTableCustom({
                            name: "bangxephang-getList",
                            table: $("#bangxephang-getList"),
                            props: {
                                dom: `
                                <'row'<'col-sm-12'rt>>
                                <'row'<'col-sm-12 col-md-6 text-left'i><'col-sm-12 col-md-6 pt-2'p>>`,  
                                lengthMenu: [
                                    [2, 5],
                                    [2, 5],
                                ],
                            }
                        }).dataTable;
                    }
                });
            },
        };
        trangChu.doanhThuMoiNhat.getList();
        trangChu.bangXepHang.getList();

        trangChu.sinhNhat = {
            ...trangChu.sinhNhat,
            layNguoiDungSinhNhat: function () {
                $.ajax({
                    ...ajaxDefaultProps({
                        url: "/Home/nguoiDungSinhNhats",
                        type: "GET",
                    }),
                    success: function (res) {
                        if (res.model.length > 0) {
                            $("#chucmungsinhnhat-container").html(res.view);
                            ActiveFirework();
                        }
                    }
                });
            },
            chucMungSinhNhat() {
                var audioEl = document.getElementById("audio-chucmungsinhnhat");
                if (audioEl.paused) audioEl.play();
                else audioEl.pause();
            }
        };
    }
}