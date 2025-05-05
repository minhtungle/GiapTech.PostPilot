'use strict'
/** Auth
 *
 * */
class Auth {
    constructor() {
        this.page = $("#user");
        this.TenDangNhap;
        this.MatKhau;
        this.GhiNho;
    }
    init() {
        var auth = this;
        auth.set();
        let loginM = localStorage.getItem("loginM");
        if (loginM) {
            let _loginM = JSON.parse(loginM)
            $('#input-tendangnhap').val(_loginM.TenDangNhap);
            $('#input-matkhau').val(_loginM.MatKhau);
            $('#input-ghinho').prop("checked", _loginM.GhiNho);
        };
        $(document).on("keypress", function (e) {
            if (e.which == 13 || e.key == "Enter") {
                $(".btn-submit").trigger("click");
            }
        });
    }
    set() {
        this.TenDangNhap = $('#input-tendangnhap').val();
        this.MatKhau = $('#input-matkhau').val();
        this.Email = $('#input-email').val();
        this.GhiNho = $('#input-ghinho').is(":checked");
    }
    login() {
        var auth = this;
        auth.set();
        let loginM = {
            TenDangNhap: auth.TenDangNhap,
            MatKhau: auth.MatKhau,
            GhiNho: auth.GhiNho
        };
        $.ajax({
            ...ajaxDefaultProps({
                url: "/Auth/Login",
                type: "POST",
                data: { loginM },
            }),
            success: function (res) {
                if (res.status == 1) {
                    // Đăng xuất người dùng đang sử dụng
                    var thongTinKiemTra = {
                        NguoiDungs: [
                            {
                                IdNguoiDung: res.nguoiDung.IdNguoiDung
                            }
                        ],
                        LoaiHinhKiemTra: ["idnguoidung"],
                        NoiDungThongBao: "Tài khoản được đăng nhập bởi người dùng khác"
                    };
                    chat.dangXuatNguoiDungHoatDong({ thongTinKiemTra: thongTinKiemTra });

                    auth.GhiNho ? localStorage.setItem("loginM", JSON.stringify(loginM)) : localStorage.removeItem("loginM");
                    sys.alert({ mess: res.mess, status: "success", timeout: 3600 });
                    window.location.href = res.action; // Chuyển tới trang quản trị
                } else {
                    sys.alert({ mess: res.mess, status: "error", timeout: 3600 });
                };
            },
            error: function (ex) {
                sys.alert({ mess: ex.message, status: "error", timeout: 3600 })
            }
        })
    }
    logout() {
        $.ajax({
            ...ajaxDefaultProps({
                url: '/Auth/Logout',
            }),
            success: function (res) {
                sys.alert({ mess: res.mess, status: "success", timeout: 3600 })
                window.location.href = res.action; // Chuyển tới trang người dùng
            }
        })
    }
    register() {

    }
    kiemTraMaXacThuc(e) {
        $("#input-matkhaumoi").val("");
        $("#input-matkhaumoi-xacnhan").val("");
        var trangThai = auth.maXacThuc == $(e).val();
        if (trangThai) {
            $("#matkhaumoi-container").css({
                "display": "block"
            });
            $("#btn-maxacthuc").css({
                "display": "none"
            });
            $("#btn-doimatkhau").css({
                "display": "block"
            });
        } else {
            $("#matkhaumoi-container").css({
                "display": "none"
            });
            $("#btn-maxacthuc").css({
                "display": "block"
            });
            $("#btn-doimatkhau").css({
                "display": "none"
            });
        };
        htmlEl.inputValidationStates(
            $("#input-maxacthuc"),
            "#matkhaumoi-container",
            "Mã xác thực không chính xác",
            {
                status: true,
                isvalid: trangThai
            }
        );
    }
    layMaXacThuc() {
        var auth = this;
        var modalValidtion = htmlEl.activeValidationStates("#thongtin-container");
        if (modalValidtion) {
            auth.set();
            let loginM = {
                TenDangNhap: auth.TenDangNhap,
                Email: auth.Email,
            };
            $.ajax({
                ...ajaxDefaultProps({
                    url: "/Auth/LayMaXacThuc",
                    type: "POST",
                    data: { loginM },
                }),
                success: function (res) {
                    if (res.status == 1) {
                        sys.alert({ mess: res.mess, status: "success", timeout: 3600 });
                        auth.maXacThuc = res.maXacThuc;
                    } else {
                        sys.alert({ mess: res.mess, status: "error", timeout: 3600 });
                    };
                },
                error: function (ex) {
                    sys.alert({ mess: ex.message, status: "error", timeout: 3600 })
                }
            });
        };
    }
};