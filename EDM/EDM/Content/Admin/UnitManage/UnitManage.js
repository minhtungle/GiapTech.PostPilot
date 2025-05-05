'use strict'
class UnitManage {
    constructor() {
        this.page;
        this.donViSuDung = {
            data: [],
            thietLapGiaoDien: {
                logo: null,
                banner: null,
                giaoDien: "",
            },
            getList: function () { },
            getFile: function () { },
            deleteFile: function () { },
            save: function () { }
        }
    }
    init() {
        var um = this;
        um.page = $("#page-unitmanage");
        /***
         * Đơn vị sử dụng
         */
        um.donViSuDung = {
            ...um.donViSuDung,
            getList: function () {
                $.ajax({
                    ...ajaxDefaultProps({
                        url: "/UnitManage/getList_DonViSuDung",
                        type: "GET",
                    }),
                    success: function (res) {
                        $("#donvisudung-getList").html(res);
                        htmlEl = new HtmlElement(); // Chạy lại htmlEL
                    }
                });
            },
            save: function () {
                //htmlEl.validationStates("#donvisudung-getList"); // Gán sự kiện kiểm tra cho các thẻ required trong form
                var f = new FormData(),
                    modalValidtion = htmlEl.activeValidationStates("#donvisudung-getList");
                if (modalValidtion) {
                    var donViSuDung = {
                        TenDonViSuDung: $("#input-tendonvisudung", $("#donvisudung-getList")).val(),
                        TieuDeTrangChu: $("#input-tieudetrangchu", $("#donvisudung-getList")).val(),
                        Logo: um.donViSuDung.thietLapGiaoDien.logo == null ? "/Assets/images/logo-img.png" : "",
                        Banner: um.donViSuDung.thietLapGiaoDien.banner == null ? "/Assets/images/banner-img.png" : "",
                        GiaoDien: um.donViSuDung.thietLapGiaoDien.giaoDien,
                        SoDienThoai: $("#input-sodienthoai", $("#donvisudung-getList")).val().trim(),
                        Email: $("#input-email", $("#donvisudung-getList")).val().trim(),
                        DiaChi: $("#input-diachi", $("#donvisudung-getList")).val().trim(),
                        SuDungTrangNguoiDung: $("#select-sudungtrangnguoidung", $("#donvisudung-getList")).val() == 0 ? false : true,
                    };
                    f.append("logo", um.donViSuDung.thietLapGiaoDien.logo);
                    f.append("banner", um.donViSuDung.thietLapGiaoDien.banner);
                    f.append("str_donViSuDung", JSON.stringify(donViSuDung));
                    $.ajax({
                        ...ajaxDefaultProps({
                            url: "/UnitManage/update_DonViSuDung",
                            type: "POST",
                            data: f
                        }),
                        contentType: false,
                        processData: false,
                        success: function (res) {
                            sys.alert({ status: res.status, mess: res.mess })
                            um.donViSuDung.getList();
                        }
                    });
                };
            },
            thayDoiGiaoDien(trangThai = "thaydoi") {
                var bgColor = trangThai == "xoa" ? "" : $("#input-giaodien").val();
                $(".demo-giaodien").css({
                    "background": bgColor
                });
                um.donViSuDung.thietLapGiaoDien.giaoDien = bgColor;
            },
            thayDoiTieuDe() {
                var tieuDe = $("#input-tieudetrangchu").val();
                $("#demo-tieudetrangchu").text(tieuDe);
            },
            thayDoiTenDonViSuDung() {
                var tenDonViSuDung = $("#input-tendonvisudung").val();
                $("#demo-tendonvisudung").text(tenDonViSuDung);
            },
            xoaLogo: function () {
                $(".demo-logo").attr("src", "/Assets/images/logo-img.png");
                $("label[for='input-logo']").text("Chọn tệp đính kèm");
                um.donViSuDung.thietLapGiaoDien.logo = null;
            },
            xoaBanner: function () {
                $(".demo-banner").attr("src", "/Assets/images/banner-img.png");
                $("label[for='input-banner']").text("Chọn tệp đính kèm");
                um.donViSuDung.thietLapGiaoDien.banner = null;
            },
            thayDoiLogo: function () {
                var $selectLogo = $("#input-logo").get(0),
                    kiemTra = true;
                $.each($selectLogo.files, function (idx, f) {
                    var extension = f.type;
                    if (/\.(png|jpg|jpeg|ico)$/i.test(f.name)) {
                        um.donViSuDung.thietLapGiaoDien.logo = f;
                        $("label[for='input-logo']").text(f.name);
                    } else {
                        kiemTra = false;
                    };
                });
                // Xóa bộ nhớ đệm để upload file trong lần tiếp theo
                $selectLogo.value = ''; // xóa giá trị của input file
                if (!kiemTra) {
                    sys.alert({
                        status: "error",
                        mess: `Tồn tại tệp không thuộc định dạng cho phép [pdf|png|jpg|jpeg|ico]`,
                        timeout: 5000
                    });
                } else {
                    // Tạo URL đại diện cho đối tượng dữ liệu
                    var imageUrl = URL.createObjectURL(um.donViSuDung.thietLapGiaoDien.logo);
                    $(".demo-logo").attr("src", imageUrl);
                };
            },
            thayDoiBanner: function () {
                var $selectBanner = $("#input-banner").get(0),
                    kiemTra = true;
                $.each($selectBanner.files, function (idx, f) {
                    var extension = f.type;
                    if (/\.(png|jpg|jpeg|ico)$/i.test(f.name)) {
                        um.donViSuDung.thietLapGiaoDien.banner = f;
                        $("label[for='input-banner']").text(f.name);
                    } else {
                        kiemTra = false;
                    };
                });
                // Xóa bộ nhớ đệm để upload file trong lần tiếp theo
                $selectBanner.value = ''; // xóa giá trị của input file
                if (!kiemTra) {
                    sys.alert({
                        status: "error",
                        mess: `Tồn tại tệp không thuộc định dạng cho phép [pdf|png|jpg|jpeg|ico]`,
                        timeout: 5000
                    });
                } else {
                    // Tạo URL đại diện cho đối tượng dữ liệu
                    var imageUrl = URL.createObjectURL(um.donViSuDung.thietLapGiaoDien.banner);
                    $(".demo-banner").attr("src", imageUrl);
                };
            }
        }
        sys.activePage({
            page: um.page.attr("id"),
            pageGroup: um.pageGroup
        });
        um.donViSuDung.getList();
    }
}