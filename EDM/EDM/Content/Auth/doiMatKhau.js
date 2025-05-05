'use strict'
class DoiMatKhau {
    constructor(container, $matKhauMoi, $matKhauMoi_XacNhan) {
        this.$container = $(container);
        this.$matKhauMoi = $($matKhauMoi, this.$container);
        this.$matKhauMoi_XacNhan = $($matKhauMoi_XacNhan, this.$container);
    }
    kiemTraMatKhau() {
        var doiMatKhau = this;

        var validMatKhau = new HandleHtmlElement({
            element: doiMatKhau.$matKhauMoi,
        });
        validMatKhau.passwordPattern(doiMatKhau.$container);
    }
    doiMatKhau() {
        var doiMatKhau = this;

        var modalValidtion = htmlEl.activeValidationStates(doiMatKhau.$container);
        if (modalValidtion) {
            var matKhauMoi = doiMatKhau.$matKhauMoi.val().trim(),
                matKhauMoi_XacNhan = doiMatKhau.$matKhauMoi_XacNhan.val().trim();

            if (matKhauMoi != matKhauMoi_XacNhan) {
                sys.alert({ mess: "Mật khẩu xác nhận chưa trùng khớp", status: "error", timeout: 3600 });
                htmlEl.inputValidationStates(
                    doiMatKhau.$matKhauMoi_XacNhan,
                    doiMatKhau.$container,
                    "Mật khẩu mới chưa chính xác",
                    {
                        status: true,
                        isvalid: false
                    }
                );
            } else {
                $.ajax({
                    ...ajaxDefaultProps({
                        url: "/Auth/DoiMatKhau",
                        type: "POST",
                        data: {
                            matKhauMoi: matKhauMoi,
                            matKhauMoi_XacNhan: matKhauMoi_XacNhan,
                        },
                    }),
                    success: function (res) {
                        if (res.status == 1) {
                            sys.alert({ mess: res.mess, status: "success", timeout: 3600 });
                            setTimeout(function () {
                                window.location.href = res.action; // Chuyển tới trang đăng nhập
                            }, 1500);
                        } else {
                            sys.alert({ mess: res.mess, status: "error", timeout: 3600 });
                        };
                    },
                    error: function (ex) {
                        sys.alert({ mess: ex.message, status: "error", timeout: 3600 })
                    }
                });
            };
        };
    }
}