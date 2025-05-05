'use strict'
/**
 * Select2Custom
 * */
class Select2Custom {
    constructor({ $select, props = {} }) {
        this.$select = $select;
        this.props = props;
    }
    init() {
        var select2Custom = this;
        return select2Custom.$select.select2({
            theme: "bootstrap-5",
            language: "vi",
            ...select2Custom.props,
        });
    }
}
/**
 * HtmlElement
 * */
class HandleHtmlElement {
    constructor({
        element = null,
        type = "text",
        maxLength = 9999999999,
        minLength = 0,
        maxValue = 9999999999,
        minValue = 0,
    }) {
        this.element = element;
        this.type = type;
        this.maxLength = maxLength;
        this.minLength = minLength;
        this.maxValue = maxValue;
        this.minValue = minValue;
    }

    getValue() {
        return $(this.element).val();
    }

    upperCase() {
        $(this.element).on('input', function () {
            const upperCaseValue = $(this).val().toUpperCase();
            $(this).val(upperCaseValue);
        });
    }

    lowerCase() {
        $(this.element).on('input', function () {
            const lowerCaseValue = $(this).val().toLowerCase();
            $(this).val(lowerCaseValue);
        });
    }

    limitLength() {
        const maxLength = this.maxLength;
        const minLength = this.minLength;
        $(this.element).on('input', function () {
            const value = $(this).val();
            if (value.length > maxLength) {
                $(this).val(value.substring(0, maxLength));
            } else if (value.length < minLength && value.length > 0) {
                $(this).addClass("error"); // Thêm class thông báo lỗi (tuỳ chỉnh CSS)
            } else {
                $(this).removeClass("error");
            }
        });
    }

    limitValue() {
        const minValue = this.minValue;
        const maxValue = this.maxValue;
        const type = this.type;

        type == "number" && $(this.element).on('input', function () {
            let value = parseFloat($(this).val().replaceAll(' ', ''));

            if (!isNaN(value)) {
                if (value > maxValue) {
                    value = maxValue;
                } else if (value < minValue) {
                    value = minValue;
                }
                $(this).val(value);
            }
        });
    }

    dontAllowCopy() {
        $(this.element).on('copy', function (e) {
            e.preventDefault();
        });
    }

    dontAllowPaste() {
        $(this.element).on('paste', function (e) {
            e.preventDefault();
        });
    }

    dontAllowAlphaKey() {
        $(this.element).on('keypress', function (e) {
            const char = String.fromCharCode(e.which);
            if (/[a-zA-Z]/.test(char)) {
                e.preventDefault();
            }
        });
    }

    dontAllowNumberKey() {
        $(this.element).on('keypress', function (e) {
            const char = String.fromCharCode(e.which);
            if (/[0-9]/.test(char)) {
                e.preventDefault();
            }
        });
    }

    dontAllowSpecialKey() {
        $(this.element).on('keypress', function (e) {
            const char = String.fromCharCode(e.which);
            if (/[^a-zA-Z0-9]/.test(char)) {
                e.preventDefault();
            }
        });
    }

    onlyAllowAlphaKey() {
        $(this.element).on('keypress', function (e) {
            const char = String.fromCharCode(e.which);
            if (!/[a-zA-Z]/.test(char)) {
                e.preventDefault();
            }
        });
    }

    onlyAllowNumberKey() {
        $(this.element).on('keypress', function (e) {
            const char = String.fromCharCode(e.which);
            if (!/[0-9]/.test(char)) {
                e.preventDefault();
            }
        });
    }

    onlyAllowSpecialKey() {
        $(this.element).on('keypress', function (e) {
            const char = String.fromCharCode(e.which);
            if (!/[^a-zA-Z0-9]/.test(char)) {
                e.preventDefault();
            }
        });
    }

    passwordPattern($container) {
        $(this.element).on("change", function () {
            var kiemTraMatKhau = sys.kiemTraMatKhau($(this).val().trim());
            var status = true, error = "";
            $.each([
                {
                    $element: $("#input-check-dodaitoithieu", $container),
                    obj: kiemTraMatKhau.minLength
                }, {
                    $element: $("#input-check-inthuong", $container),
                    obj: kiemTraMatKhau.hasLowerCase
                }, {
                    $element: $("#input-check-inhoa", $container),
                    obj: kiemTraMatKhau.hasUpperCase
                }, {
                    $element: $("#input-check-kytuso", $container),
                    obj: kiemTraMatKhau.hasNumber
                }, {
                    $element: $("#input-check-kytudacbiet", $container),
                    obj: kiemTraMatKhau.hasSpecialChar
                },
            ], function () {
                this.$element.prop("checked", true);
                if (!this.obj.status) {
                    status = this.obj.status;
                    error = this.obj.error;
                    this.$element.prop("checked", false);
                };
            });
            status ? htmlEl.inputValidationStates(
                $(this),
                $container,
                "",
                {
                    status: true,
                    isvalid: true
                }
            ) : htmlEl.inputValidationStates(
                $(this),
                $container,
                error,
                {
                    status: true,
                    isvalid: false
                }
            );
        });
    }
}

class HtmlElement {
    constructor() {
        this.init();
        this.inputMask();
        this.validationStates();
        this.select2Mask();
        this.type = [];
    }
    init() {
    }
    dontAllowStringKey() {

    }
    // Định dạng số điện thoại
    phoneMask(max = 11) {
        var num = $(this).val().replace(/\D/g, '');
        $(this).val(
            num.substring(0, 4)
            + (num.length > 4 ? ' ' + num.substring(4, 7) : '')
            + (num.length > 7 ? ' ' + num.substring(7, 9) : '')
            + (num.length > 9 ? ' ' + num.substring(9, 11) : '')
        );
    }
    // Định dạng tiền tệ
    moneyMask() {
        var val = $(this).val();
        val = val.replace(/[^0-9]/g, '').replace(/\B(?=(\d{3})+(?!\d))/g, " ");
        // Loại bỏ tất cả các ký tự không phải là số
        $(this).val(val);
    }
    // Gán mask cho thẻ
    inputMask() {
        $('[type="tel"]').keyup(this.phoneMask);
        $('[type="money"]').keyup(this.moneyMask);
        $('.numberInt32format').on("change", function () {
            var num = $(this).val() == "" ? 0 : parseInt($(this).val(), 10),
                limit = !$(this).attr("limit") ? "0-max" : $(this).attr("limit"),
                min = parseInt(limit.split("-")[0], 10),
                max = limit.split("-")[1] == "max" ? "max" : parseInt(limit.split("-")[1], 10);
            $(this).val(num);
            if (num <= min) $(this).val(min);
            if (max != "max" && num >= max) {
                $(this).val(max);
            };
        });
        $('.monthformat').inputmask('mm/yyyy', { 'placeholder': 'mm/yyyy' });
        $('.dateformat').inputmask('dd/mm/yyyy', { 'placeholder': 'dd/mm/yyyy' });
        $('.datetimeformat').inputmask('datetime', { 'placeholder': 'dd/mm/yyyy hh:mm' });
        //$('.datetimeformat').inputmask('datetime', { 'placeholder': 'dd/mm/yyyy hh:mm' });
    }
    select2Mask(parentName) {
        var htmlEl = this,
            $select2 = htmlEl.findByParent(".form-select2", parentName);
        var selec2Custom = new Select2Custom({
            $select: $select2,
            props: {
                dropdownParent: parentName
            }
        }).init();
    }
    // Tìm thẻ qua thẻ cha (thẻ cha có thể null)
    findByParent(childName, parentName = "") {
        if (parentName != "") {
            var $parent = $(parentName);
            return $(`${childName}`, $parent);
        } else {
            return $(`${childName}`);
        }
    };
    // Nhấn vào thẻ span sẽ biến thành thẻ input và ngược lại
    spanToInput(span, className = '', type = 'text') {
        var htmlEl = this,
            text = $(span).text(),
            input = $(`<input type="${type}" class="${className}" value="${text}" />`);
        $(input).blur(function () {
            var span = $(`<span class="${className}" onclick="${htmlEl.spanToInput(span, className, type)}">${$(this).val()}</span>`);
            $(this).replaceWith(span);
        });
        $(span).replaceWith(input);
        input.focus();
    }
    // Gán sự kiện kiểm tra thẻ required
    validationStates(parentName) {
        var htmlEl = this,
            $inputRequired = htmlEl.findByParent("input[required]", parentName),
            $textareaRequired = htmlEl.findByParent("textarea[required]", parentName),
            $selectRequired = htmlEl.findByParent("select[required]", parentName);
        $inputRequired.on("change", function () {
            htmlEl.inputValidationStates(this, parentName);
        });
        $textareaRequired.on("change", function () {
            htmlEl.inputValidationStates(this, parentName);
        });
        $selectRequired.on("change", function () {
            htmlEl.selectValidationStates(this, parentName);
        });
    }
    // Kiểm tra dữ liệu input trong 1 thẻ cha (thẻ cha có thể null)
    inputValidationStates(el, parentName, feedBack = "Không được để trống",
        activeManual = {
            status: false, isvalid: false
        }) {
        var htmlEl = this;

        var $input = $(el),
            inputName = $input.attr("name"),
            $feedBack = htmlEl.findByParent(`.feedback[for="${inputName}"]`, parentName);
        $feedBack.text(feedBack);
        if (activeManual.status) {
            $input.removeClass("is-invalid").addClass("is-valid");
            $feedBack.hide();

            if (!activeManual.isvalid) {
                $input.removeClass("is-valid").addClass("is-invalid");
                $input.focus();
                $feedBack.show();
            };
        } else {
            $input.removeClass("is-invalid").addClass("is-valid");
            $feedBack.hide();

            var val = $input.val().trim();
            if (val == "") {
                $input.removeClass("is-valid").addClass("is-invalid");
                $input.focus();
                $feedBack.show();
            };

            const re = /^(([^<>()\[\]\\.,;:\s@"]+(\.[^<>()\[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
            //const re = /^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$/;
            var pattern = new RegExp(re);
            if ($input.attr("type") == "email" && !pattern.test(val)) {
                feedBack = "Không đúng định dạng email";
                $feedBack.text(feedBack);
                $feedBack.show();

                $input.removeClass("is-valid").addClass("is-invalid");
                $input.focus();
            };
            //var pattern = new RegExp($input.attr("pattern"));
            //feedBack = $input.attr("pattern-feedback");
            //if (!pattern.test(val)) {
            //    $input.removeClass("is-valid").addClass("is-invalid");
            //    $input.focus();
            //    $feedBack.text(feedBack);
            //    $feedBack.show();
            //};
        };
    }
    // Kiểm tra dữ liệu input trong 1 thẻ cha (thẻ cha có thể null)
    selectValidationStates(el, parentName, feedBack = "Không được để trống",
        activeManual = {
            status: false, isvalid: false
        }) {
        var htmlEl = this;

        var $select = $(el),
            selectName = $select.attr("name"),
            $feedBack = htmlEl.findByParent(`.feedback[for="${selectName}"]`, parentName);
        $feedBack.text(feedBack);
        if (activeManual.status) {
            if (activeManual.isvalid) {
                $select.removeClass("is-invalid").addClass("is-valid");
                $feedBack.hide();
            } else {
                $select.removeClass("is-valid").addClass("is-invalid");
                $select.focus();
                $feedBack.show();
            };
        } else {
            if ($select.val() !== "" && $select.val() !== null && $select.val().length !== 0) {
                $select.removeClass("is-invalid").addClass("is-valid");
                $feedBack.hide();
            } else {
                $select.removeClass("is-valid").addClass("is-invalid");
                $feedBack.show();
            };
        };
    }
    // Gọi sự kiện kiểm tra các thẻ có gán required => true/false
    activeValidationStates(parentName) {
        var htmlEl = this,
            $inputRequired = htmlEl.findByParent("input[required]", parentName),
            $selectRequired = htmlEl.findByParent("select[required]", parentName);
        // Kích hoạt kiểm tra thẻ
        $inputRequired.trigger("change");
        $selectRequired.trigger("change");
        // Kiểm tra có thẻ nào chưa đủ điều kiện
        var $invalidEl = htmlEl.findByParent("[required].is-invalid", parentName);
        if ($invalidEl.length > 0) {
            return false;
        };
        return true;
    }
    // Mã hóa ký hiệu HTML thành chuỗi
    changeSignToCode(text) {
        return text.replace(/</g, '&lt;').replace(/>/g, '&gt;').replace(/&/g, '&amp;').replace(/"/g, '&quot;').replace(/'/g, '&#39;');
    }
    // Mã hóa HTML thành văn bản thuần túy
    changeHTMLtoString(text) {
        return $('<div>').text(text).html();
    }
}
var htmlEl = new HtmlElement();
/**
 * Modal
 * */
class Modal {
    constructor({ name = '', displayStatus = 'show', level = 1, callback = null }) {
        this.name = name;
        this.displayStatus = displayStatus;
        this.level = level;
        this.callback = callback; // ✅ Gán callback đúng cách
        this.init();
    }

    init() {
        var html = new HtmlElement();
        html.init();
    }

    display() {
        const modal = this;
        const $modal = $(modal.name);
        const zIndex_Default = 1050;

        // Xóa các handler cũ để tránh trùng sự kiện
        $modal.off("shown.bs.modal hide.bs.modal hidden.bs.modal");

        // Gộp tất cả xử lý vào một sự kiện shown.bs.modal
        $modal.on("shown.bs.modal", function () {
            const $modalBackdrop = $(".modal-backdrop").last();

            // ✅ Đảm bảo modal-backdrop hiển thị đúng z-index
            if ($modalBackdrop.is(":visible")) {
                $modalBackdrop
                    .css("z-index", zIndex_Default + modal.level)
                    .attr("for", `${modal.name}`);
            }

            // ✅ Cài đặt z-index cho modal
            $modal.css("z-index", zIndex_Default + modal.level + 1);

            // ✅ Thêm validation và các hiệu ứng
            htmlEl.validationStates(modal.name);
            htmlEl.select2Mask(modal.name);

            // ✅ Gọi callback sau khi modal hiển thị hoàn tất
            if (modal.callback && typeof modal.callback === "function") {
                modal.callback();
            }
        });

        $modal.on("hide.bs.modal", function () {
            // 🔔 Có thể thêm logic trước khi modal tắt nếu cần
        });

        $modal.on("hidden.bs.modal", function () {
            // ✅ Xử lý dọn dẹp sau khi modal đã đóng
            if ($modal.attr("removeBody") === "true") {
                $modal.empty();
            }
        });

        // ✅ Hiển thị hoặc ẩn modal dựa trên displayStatus
        if (modal.displayStatus === "show") {
            $modal.modal("show");
        } else {
            $modal.modal("hide");
        }
    }
}

class _Modal {
    constructor({ name = '', displayStatus = 'show', level = 1, callback }) {
        this.name = name;
        this.displayStatus = displayStatus;
        this.level = level;
        this.callback
        this.init();
    }
    init() {
        var html = new HtmlElement();
        html.init();
    }
    display() {
        var modal = this,
            $modal = $(modal.name),
            zIndex_Default = 1050;
        // Làm cho modal-backdrop của modal đè lên modal trước
        $modal.on("shown.bs.modal", function () { // shown.bs.moda: Modal chưa hiển thị
            let $modalBackdrop = $(".modal-backdrop").last();

            if ($modalBackdrop.is(":visible")) {
                $modalBackdrop.css("z-index", zIndex_Default + modal.level)
                $modalBackdrop.attr("for", `${modal.name}`)
            }
        });
        $modal.on("shown.bs.modal", function () { // shown.bs.moda: Modal đã hiển thị
            $modal.css("z-index", zIndex_Default + modal.level + 1);
            // Thêm validation cho thẻ
            htmlEl.validationStates(modal.name);
            htmlEl.select2Mask(modal.name);
            if (modal.callback) modal.callback();
        });
        $modal.on("hide.bs.modal", function () { // hide.bs.modal: Modal chưa tắt
        });
        $modal.on("hidden.bs.modal", function () { // hide.bs.modal: Modal đã tắt
            if ($modal.attr("removeBody") == "true") $modal.empty();
        });

        if (modal.displayStatus == "show") {
            $modal.modal("show");
        } else {
            $modal.modal("hide");
        };
    }
}
/**
 * System
 * */
class System {
    constructor() {
        this.chatService = $.connection.chatService;
    }
    init() {
        var sys = this;
        // Hiển thị khung chat
        $("#khungchat-active").css({
            "display": "flex"
        })
        // select-input
        var selec2Custom = new Select2Custom({
            $select: $(".form-select2"),
            props: {
                dropdownParent: $("document")
            }
        }).init();
        /**
         * Gia hạn session timeout
         */
        sys.keepSessionAlive();
        /**
         * Mặc định tắt sys.loading()
         */
        sys.loading(false);
        /**
         * Kiểm tra danh sách người dùng đang đăng nhập liên tục
         */
        sys.chatService.client.capNhatNguoiDungDangDangNhap = function (res) {
            // Gửi tin nhắn cho toàn bộ người dùng kiểm tra xem nếu trùng thì thông báo đăng xuất
            if (res.LoaiHinhKiemTra.includes("idnguoidung")) {
                var idNguoiDungs_CanKiemTra = res.NguoiDungs.map(nguoiDung => nguoiDung.IdNguoiDung),
                    idNguoiDung_HienTai = parseInt(chat.$idNguoiGui.val());
                if (idNguoiDungs_CanKiemTra.includes(idNguoiDung_HienTai))
                    sys.logoutDialog({
                        mess: res.noiDungGui,
                    });
            };
        };
        //sys.layNguoiDungs_DangDangNhap();
    }
    debounce(func, delay) {
        let timer;
        return function (...args) {
            clearTimeout(timer);
            timer = setTimeout(() => func.apply(this, args), delay);
        };
    }
    // Giữ cho hệ thống không chết session
    keepSessionAlive() {
        var xhttp = new XMLHttpRequest();

        function sendRequest() {
            xhttp.onreadystatechange = function () {
                if (this.readyState == 4 && this.status == 200) {
                    // Xử lý phản hồi từ máy chủ
                    //console.log(this.responseText);
                }
            };
            xhttp.open("GET", "/Home/KeepSessionAlive", true);
            xhttp.send();
        }

        function startPolling() {
            setInterval(function () {
                sendRequest();
            }, 900000);
        }

        startPolling();
    }
    layNguoiDungs_DangDangNhap() {
        var sys = this;
        setInterval(function () {
            sys.chatService.server.layNguoiDungs_DangDangNhap();
        }, 900000);
    }
    // Kích hoạt trạng thái chọn của sideItem (Modul)
    activePage({ page = null, pageGroup = null }) {
        if (page) {
            $("#page-menu li").removeClass("active");
            if (pageGroup) {
                $(`#page-menu .${pageGroup}`).addClass("active");
                $(`#page-menu .${pageGroup} .${page}`).addClass("active");
            } else {
                $(`#page-menu .${page}`).addClass("active");
            }
        }
    }
    // Trạng thái loading của hệ thống
    loading(stt = true) {
        const $loading = $('#sys-loading');
        stt ? $loading.show() : $loading.hide();
    }
    // Modal hỏi trước khi thực hiện hành động
    confirmDialog({
        mess = "",
        callback = function () { },
        callback_no = function () {
            sys.displayModal({
                name: "#confirmdialog",
                displayStatus: "hide",
                level: 100
            })
        }
    }) {
        var sys = this;
        $("#confirmdialog-content").html(mess);
        sys.displayModal({
            name: "#confirmdialog",
            level: 100
        });
        // Phải off() rồi mới on() để không bị lặp lại sự kiện nhiều lần
        $("#confirmdialog").find("button[name='yes']").off().on("click", function (e) {
            e.preventDefault();
            callback();
            sys.displayModal({
                name: "#confirmdialog",
                displayStatus: "hide",
                level: 100
            })
        });
        $("#confirmdialog").find("button[name='no']").off().on("click", function (e) {
            e.preventDefault();
            callback_no();
        });
    }
    // Thông báo đăng xuất hệ thống
    logoutDialog({ mess = "", callback = function () { } }) {
        var sys = this;
        $("#logoutdialog-content").text(mess);
        sys.displayModal({
            name: "#logoutdialog",
            level: 100
        })
        // Phải off() rồi mới on() để không bị lặp lại sự kiện nhiều lần
        $("#logoutdialog").find("button[name='yes']").off().on("click", function (e) {
            e.preventDefault();
            /*callback();*/
            sys.displayModal({
                name: "#logoutdialog",
                displayStatus: "hide",
                level: 100
            });
            $("btn-logout").trigger("click");
        });
    }
    alertDialog({ title = "", content = "", timeout = 3000 }) {
        var sys = this;
        var $alertDialog = $("#alertdialog");
        $(".modal-header", $alertDialog).html(title);
        $(".modal-body", $alertDialog).html(content);
        sys.displayModal({
            name: "#alertdialog",
            level: 100
        });
        //setTimeout(function () {
        //    sys.displayModal({
        //        name: "#alertdialog",
        //        level: 100,
        //        displayStatus: "hide"
        //    });
        //}, timeout);
    }
    // Thông báo
    alert({
        oldestFirst = true,
        mess = "Có chút vấn đề: ",
        status = "",
        timeout = 3000,
        node = undefined,
        selector = undefined,
        callback = function () {
        },
        destination = undefined,
        newWindow = false,
        close = false,
        gravity = "toastify-top",
        positionLeft = false,
        position = '',
        avatar = "",
        className = "",
        stopOnFocus = true,
        onClick = function () {
        },
        offset = { x: 0, y: 0 },
        escapeMarkup = true,
        ariaLive = 'polite',
        //style = { background: '' }
    }) {
        let background = "";
        if (status == "success") {
            background = "#198754";
            mess = "✅ " + mess;
        }
        else if (status == "warning") {
            background = "#fd7e14";
            mess = "⚠️ " + mess;
        }
        else if (status == "error") {
            background = "#dc3545";
            mess = "❌ " + mess;
        }
        else {
            background = "#0dcaf0";
            mess = "🗨️ " + mess;
        }

        var toast = Toastify({
            text: mess, duration: timeout, style: { background },
            oldestFirst, node, selector, callback, destination, newWindow, close, gravity, positionLeft, position, avatar, className, stopOnFocus,
            onClick: function () {
                toast.hideToast();
            }, offset, escapeMarkup, ariaLive
        });
        toast.showToast();
    }
    // Kích hoạt trạng thái hiển thị modal
    displayModal({ name = "", displayStatus = "show", level = 1 }) {
        var sys = this;
        var modal = new Modal({
            name,
            displayStatus,
            level
        });
        modal.display();
    }
    // Cắt chuỗi
    truncateString(str = "", length = 0) {
        if (str.length > length) {
            return str.substring(0, length) + ' ...';
        };
        return str;
    }
    // Xóa dấu
    convertToUnSign(s = "", khoangCach = "", toUpper = false, toLower = false) {
        s = s || "";
        if (toUpper) s = s.toUpperCase();
        if (toLower) s = s.toLowerCase();

        var regex = '/\p{IsCombiningDiacriticalMarks}+/u';
        var temp = s.normalize('NFD').replace(regex, '');
        temp = temp.replace('\u0111', 'd').replace('\u0110', 'D').replace(/\s+/g, khoangCach).replace(/\(\*\)/g, '');

        return temp;
    }
    phanQuyenThaoTac(kichHoat = "true", callback = function () { }) {
        var sys = this;
        if (kichHoat.toLocaleLowerCase() == "false") {
            sys.alert({ status: "warning", mess: "Bạn không có quyền với chức năng này !" });
            return;
        };
        callback();
    }
    convertInt2MoneyFormat(amount = 0, locale = "vi-VN", currency = "VND") {
        return new Intl.NumberFormat(locale, { style: 'currency', currency }).format(amount);
    }
    readFileAsBase64(file, type = "all") {
        return new Promise(function (resolve, reject) {
            var reader = new FileReader();
            reader.onload = function (event) {
                if (type == "all") resolve(event.target.result); // Lấy chuỗi Base64
                else resolve(event.target.result.split(',')[1]); // Lấy chuỗi Base64
            };
            reader.onerror = function (error) {
                reject(error);
            };
            reader.readAsDataURL(file); // Đọc file dưới dạng DataURL
        });
    };
    kiemTraMatKhau(password = "") {
        const minLength = 8; // Độ dài tối thiểu
        const hasLowerCase = /[a-z]/.test(password); // Có chữ thường
        const hasUpperCase = /[A-Z]/.test(password); // Có chữ hoa
        const hasNumber = /[0-9]/.test(password); // Có số
        const hasSpecialChar = /[!@#$%^&*(),.?":{}|<>]/.test(password); // Có ký tự đặc biệt

        return {
            minLength: {
                status: (password.length >= minLength),
                error: `Độ dài tối thiểu ${minLength} ký tự`
            },
            hasLowerCase: {
                status: hasLowerCase,
                error: `Phải chứa ký tự in thường`
            },
            hasUpperCase: {
                status: hasUpperCase,
                error: `Phải chứa ký tự in hoa`
            },
            hasNumber: {
                status: hasNumber,
                error: `Phải chứa ký tự số`
            },
            hasSpecialChar: {
                status: hasSpecialChar,
                error: `Phải chứa ký tự đặc biệt`
            },
        };
    }
    chuyenThoiGianDungLocalVN(date) {
        return new Date(date.getTime() - date.getTimezoneOffset() * 60000);
    }
}
//var sys = new System();
/**
 * Ajax - Thông tin mặc định của hàm ajax
 * */
var ajaxDefaultProps = ({
    url = '', type = 'GET', data = null,
}) => ({
    url, type, data,
    beforeSend: function () { sys.loading(true) },
    complete: function () { sys.loading(false) },
})
/**
 * DataTableCustom - bảng dataTable với các thông tin mặc định
 * */
class DataTableCustom {
    constructor({ name, table, props = {}, initCompleteProps = function () { } }) {
        this.name = name;
        this.table = table;
        this.props = props;
        this.initCompleteProps = initCompleteProps();
        this.dataTable;
        this.init();
    }
    init() {
        var that = this,
            tableId = that.table.attr("id");
        that.dataTable = that.table.DataTable({
            //serverSide: true,
            //responsive: true,
            //colVis: true,
            stateSave: true,
            processing: true,
            deferRender: true,
            search: {
                regex: true,
            },
            scrollCollapse: true,
            //scrollX: true,
            autoWidth: true,
            fixedHeader: true,
            orderClasses: false,
            //ordering: false,  // Tắt tính năng sắp xếp tự động
            dom: `
            <'row'<'col-12 mb-3 d-none'B>>
            <'row'<'col-sm-12 col-md-6'l><'col-sm-12 col-md-6'f>>
            <'row'<'col-sm-12'rt>>
            <'row'<'col-sm-12 col-md-5'i><'col-sm-12 col-md-7 pt-2'p>>`,
            buttons: [
                'copy', 'pdf', 'print',
                {
                    extend: 'excel',
                    title: '', // Bỏ tiêu đề tại đây
                },
            ],
            language: {
                url: `/assets/templates/assets/extensions/datatables/vi.json`,
                decimal: ',',
                thousands: '.',
            },
            ...that.props,
        });
        /**
        * Đăng ký các mã của bạn vào đây để được gọi sau khi DataTable được khởi tạo.
        * */
        that.table.on('init.dt', function () {
            /**
            * Bao bọc table bên trong 1 div overflow
            * */
            that.table.wrap(`<div style='overflow:auto; width:100%; max-height: ${that.props.maxHeight}px; position:relative;'></div>`);
        });
        /**
        * Gọi lại mã của bạn sau mỗi lần DataTable được vẽ lại.
        * */
        that.table.on('draw.dt', function () {
            //$(".dataTables_scrollHeadInner").css("width", "100%");
            //$(".dataTables_scrollHeadInner table").css("width", "100%");
            /**
            * Table-checkbox
            * */
            //let a = []
            //that.dataTable.rows.iterator('row', function (context, index) {
            //    a.push($(this.row(index).node()));
            //});
            multipleCheck({
                name: that.name,
                parent: that.dataTable.rows().nodes().toArray()
            });
            // Từng cột của bảng
            that.dataTable.columns().every(function (i, col) {
                let that = this;
                /**
                * Tìm kiếm theo cột
                * */
                let $searchInputs = $('input.dt-search-col');
                let $searchInput = $('input.dt-search-col', that.footer());
                $searchInput.on('keyup change clear', function () {
                    if (that.search() !== $searchInput.val()) {
                        that.search($searchInput.val(), true, false).draw();
                    };
                });
                $searchInput.val(that.search());
                /**
                * Nhận diện trạng thái ẩn/hiện của cột
                * */
                var $toggleVises = $(`.toggle-vis[data-table="${tableId}"]`);
                var $toggleVis = $(`.toggle-vis[data-table="${tableId}"][data-column="${i}"]`);
                $toggleVis.removeClass("text-decoration-line-through");
                if (!that.visible()) {
                    $toggleVis.addClass("text-decoration-line-through");
                };
            });
            // Custom
            that.initCompleteProps;
        });
        /**
         * Cho footer lên top
         * */
        var r = that.table.find('tfoot tr');
        r.find('th').each(function () {
            $(this).css('padding', 8);
        });
        that.table.find('thead').append(r);
        $('#search_0').css('text-align', 'center');
        /**
         * Ẩn/hiện cột
         * */

        $(`a[data-table="${tableId}"].toggle-vis`).on('click', function (e) {
            e.preventDefault();
            // Lấy cột
            var column = that.dataTable.column($(this).attr('data-column'));
            // Đổi trạng thái
            column.visible(!column.visible());
            $(this).removeClass("text-decoration-line-through");
            if (!column.visible()) {
                $(this).addClass("text-decoration-line-through");
            };
        });
    }
}
/**
 * Table-checkbox
 * CÁCH DÙNG:
 * - Tại checkbox-all thêm class "checkAll-{tenbang}"
 * - Tại checkbox-row thêm class "checkRow-{tenbang}"
 * */
var multipleCheck = ({ name, parent }) => {
    var $check = $(`.checkRow-${name}`, $(parent));
    var $checkAll = $(`.checkAll-${name}`);

    $check.on("click", function (e) {
        var soLuongRow_checked = $(`.checkRow-${name}[type="checkbox"]:checked`, $(parent)).length;
        var soLuongRow = $check.length;
        if (soLuongRow == soLuongRow_checked) {
            $checkAll.prop("checked", true)
        } else {
            $checkAll.prop("checked", false)
        };
    });
    $checkAll.on("click", function (e) {
        $check.prop("checked", $(this).is(":checked"));
    });
}
var singleCheck = ({ name, parent }) => {
    var $check = $(`.checkRow-${name}`, $(parent));
    $check.on("click", function (e) {
        let checked = $(this).is(":checked");
        $check.prop("checked", false)
        $(this).prop("checked", checked)
    });
}
/**
 * FancyTree
 * */
class FancyTreeCustom {
    constructor({ name = "", props = {} }) {
        this.name = name;
        this.props = props;
    }
    init() {
        var fancyTree = this,
            $element = $(fancyTree.name);
        return $element
            .fancytree({

                ...fancyTree.props,
            })
            .on("nodeCommand", function (event, data) {
                // Custom event handler that is triggered by keydown-handler and
                // context menu:
                var refNode,
                    moveMode,
                    tree = $.ui.fancytree.getTree(this),
                    node = tree.getActiveNode();

                switch (data.cmd) {
                    case "addChild":
                    case "addSibling":
                    case "indent":
                    case "moveDown":
                    case "moveUp":
                    case "outdent":
                    case "remove":
                    case "rename":
                        tree.applyCommand(data.cmd, node);
                        break;
                    case "cut":
                        CLIPBOARD = { mode: data.cmd, data: node };
                        break;
                    case "copy":
                        CLIPBOARD = {
                            mode: data.cmd,
                            data: node.toDict(true, function (dict, node) {
                                delete dict.key;
                            }),
                        };
                        break;
                    case "clear":
                        CLIPBOARD = null;
                        break;
                    case "paste":
                        if (CLIPBOARD.mode === "cut") {
                            // refNode = node.getPrevSibling();
                            CLIPBOARD.data.moveTo(node, "child");
                            CLIPBOARD.data.setActive();
                        } else if (CLIPBOARD.mode === "copy") {
                            node.addChildren(
                                CLIPBOARD.data
                            ).setActive();
                        }
                        break;
                    default:
                        alert("Unhandled command: " + data.cmd);
                        return;
                }
            })
            .on("keydown", function (e) {
                var cmd = null;

                // console.log(e.type, $.ui.fancytree.eventToString(e));
                switch ($.ui.fancytree.eventToString(e)) {
                    case "ctrl+shift+n":
                    case "meta+shift+n": // mac: cmd+shift+n
                        cmd = "addChild";
                        break;
                    case "ctrl+c":
                    case "meta+c": // mac
                        cmd = "copy";
                        break;
                    case "ctrl+v":
                    case "meta+v": // mac
                        cmd = "paste";
                        break;
                    case "ctrl+x":
                    case "meta+x": // mac
                        cmd = "cut";
                        break;
                    case "ctrl+n":
                    case "meta+n": // mac
                        cmd = "addSibling";
                        break;
                    case "del":
                    case "meta+backspace": // mac
                        cmd = "remove";
                        break;
                    // case "f2":  // already triggered by ext-edit pluging
                    //   cmd = "rename";
                    //   break;
                    case "ctrl+up":
                    case "ctrl+shift+up": // mac
                        cmd = "moveUp";
                        break;
                    case "ctrl+down":
                    case "ctrl+shift+down": // mac
                        cmd = "moveDown";
                        break;
                    case "ctrl+right":
                    case "ctrl+shift+right": // mac
                        cmd = "indent";
                        break;
                    case "ctrl+left":
                    case "ctrl+shift+left": // mac
                        cmd = "outdent";
                }
                if (cmd) {
                    $(this).trigger("nodeCommand", { cmd: cmd });
                    return false;
                }
            });

        /*
         * Tooltips
         */
        // $("#tree").tooltip({
        //   content: function () {
        //     return $(this).attr("title");
        //   }
        // });

        /*
         * Context menu (https://github.com/mar10/jquery-ui-contextmenu)
         */
        $element.contextmenu({
            delegate: "span.fancytree-node",
            menu: [
                {
                    title: "Edit <kbd>[F2]</kbd>",
                    cmd: "rename",
                    uiIcon: "ui-icon-pencil",
                },
                {
                    title: "Delete <kbd>[Del]</kbd>",
                    cmd: "remove",
                    uiIcon: "ui-icon-trash",
                },
                { title: "----" },
                {
                    title: "New sibling <kbd>[Ctrl+N]</kbd>",
                    cmd: "addSibling",
                    uiIcon: "ui-icon-plus",
                },
                {
                    title: "New child <kbd>[Ctrl+Shift+N]</kbd>",
                    cmd: "addChild",
                    uiIcon: "ui-icon-arrowreturn-1-e",
                },
                { title: "----" },
                {
                    title: "Cut <kbd>Ctrl+X</kbd>",
                    cmd: "cut",
                    uiIcon: "ui-icon-scissors",
                },
                {
                    title: "Copy <kbd>Ctrl-C</kbd>",
                    cmd: "copy",
                    uiIcon: "ui-icon-copy",
                },
                {
                    title: "Paste as child<kbd>Ctrl+V</kbd>",
                    cmd: "paste",
                    uiIcon: "ui-icon-clipboard",
                    disabled: true,
                },
            ],
            beforeOpen: function (event, ui) {
                var node = $.ui.fancytree.getNode(ui.target);
                $("#tree").contextmenu(
                    "enableEntry",
                    "paste",
                    !!CLIPBOARD
                );
                node.setActive();
            },
            select: function (event, ui) {
                var that = this;
                // delay the event, so the menu can close and the click event does
                // not interfere with the edit control
                setTimeout(function () {
                    $(that).trigger("nodeCommand", { cmd: ui.cmd });
                }, 100);
            },
        });
    }
}
/**
 * Treeview <Bảng có checkbox>
 * CÁCH DÙNG:
 * - Tại mỗi dòng thêm 2 props : data-id và data-idcha
 * */
var treeView = ({ $table }) => {
    $table.find("input[type='checkbox']").on("click", function (e, side = "both") {
        var id = $(this).attr("data-id");
        var idCha = $(this).attr("data-idcha");
        var $cha = $table.find(`[data-id="${idCha}"]`);
        var $con = $table.find(`[data-idcha="${id}"]`);
        var trigger = ({ element, checked, side }) => {
            element.prop("checked", !checked) // chuyển trạng thái ngược lại
            element.trigger("click", [side]); // click
        }
        // CHECK CON (Đang xuống) => hết con sẽ dừng
        if (side != "up" && $con.length > 0) {
            (() => {
                trigger({
                    element: $con,
                    checked: $(this).is(":checked"),
                    side: "down"
                })
            })();
        }
        // CHECK CHA (Đang lên) => trạng thái anhEm khác nhau sẽ dừng
        if (side != "down" && $cha.length > 0) {
            (() => {
                let $anhEm = $table.find(`[data-idcha="${idCha}"]`);
                let $anhEm_checked = $table.find(`[data-idcha="${idCha}"][type="checkbox"]:checked`);
                let soLuong_anhEm = $anhEm.length;
                let soLuong_anhEm_checked = $anhEm_checked.length;

                if (soLuong_anhEm == soLuong_anhEm_checked) {
                    trigger({
                        element: $cha,
                        checked: true,
                        side: "up"
                    })
                } else {
                    trigger({
                        element: $cha,
                        checked: false,
                        side: "up"
                    })
                }
            })();
        }
    })
}
//treeView({ $table: $(".table-treeview") }); // Gọi lại dòng này sau câu lệnh nếu dùng js tạo bảng
/**
 * Treeview <Bảng có collapse>
 * */
class TreeViewCustom {
    constructor({ name, props = {} }) {
        this.name = name;
        this.props = props;
    }
    init() {
        var t = this,
            defaultProps = {
                levels: 1,
                indent: 2,
                parentsMarginLeft: '2rem',

                expandIcon: 'bi bi-caret-right', // Bắt buộc
                collapseIcon: 'bi bi-caret-down', // Bắt buộc
                //emptyIcon : 'bi bi-folder-x',
                nodeIcon: 'bi bi-folder2-open',
                selectedIcon: 'bi bi-check',
                checkedIcon: 'bi bi-check',
                uncheckedIcon: 'bi bi-x',
                tagClasses: '',

                color: "var(--bs-text-color)", // '#000000',
                backColor: "var(--bs-body-bg)",
                borderColor: "#dfe3e7 !important", // '#dddddd', underfine
                //borderColor: "var(--bs-border-color)", // '#dddddd', underfine
                onhoverColor: "var(--bs-body-bg-hover-color)",
                selectedColor: 'var(--bs-text-color)',
                selectedBackColor: "var(--bs-body-bg-hover-color)",
                searchResultColor: 'var(--bs-danger)',
                searchResultBackColor: undefined, //'#FFFFFF',
                tagClasses: "badge bg-primary",

                enableLinks: false,
                highlightSelected: true,
                highlightSearchResults: true,
                showBorder: true,
                showIcon: false,
                showCheckbox: false,
                showTags: true,
                multiSelect: false,
                openNodeLinkOnNewTab: true
            };
        return $(t.name).treeview({
            ...defaultProps,
            ...t.props
        })
    }
}
class TreeSelectCustom {
    constructor({ props = {} }) {
        this.props = props;
    }
    init() {
        var t = this,
            defaultProps = {
                // parent container element
                //parentHtmlContainer: t.name,
                // an array of option values
                value: [],
                // define your options here
                options: [],
                // keep the tree select always open
                alwaysOpen: true,
                // open all options to this level
                openLevel: 0,
                // append the Treeselect to the body
                appendToBody: false,
                // shows selected options as tags
                showTags: true,
                // text to show if you use 'showTags'
                tagsCountText: '{count} {tagsCountText}',
                // shows remove icon
                clearable: true,
                // is searchable?
                searchable: true,
                // placeholder text
                placeholder: 'Tìm kiếm...',

                // text to display when no results found
                emptyText: 'Không có kết quả ...',
                // converts multi-select to the single value select
                isSingleSelect: false,
                // returns groups if they selected instead of separate ids
                isGroupedValue: false,
                // All nodes in treeselect work as an independent entity. 
                // Check/uncheck action ignore children/parent updates workflow. 
                // Disabled nodes ignore children/parent workflow as well.
                isIndependentNodes: false,
                // It is impossible to select groups. You can select only leaves.
                disabledBranchNode: false,
                // auto, top, bottom
                direction: 'auto',
                // all groups which have checked values will be expanded on the init.
                expandSelected: false,
                // The list saves the last scroll position before close. If you open the list your scroll will be on the previous position. 
                // If you set the value to false - the scroll will have position 0 and the first item will be focused every time.
                saveScrollPosition: true,
                //  A class name for list. 
                listClassName: '',
                // shows count of children near the group's name.
                showCount: false,
                // group options
                grouped: true,
                // element that appends to the end of the dropdown list
                listSlotHtmlComponent: null,
                // is dropdown list disabled?
                disabled: false,
                // add the list as a static DOM element
                // this prop will be ignored if you use appendToBody.
                staticList: false,
                // id attribute for the accessibility.
                id: '',
                // { arrowUp, arrowDown, arrowRight, attention, clear, cross, check, partialCheck }
                // object contains all svg icons
                iconElements: {},
                srcElement: null,
                // callback and functions
                inputCallback: function (value) { },
                openCallback: function (value) { },
                closeCallback: function (value) { },
                nameChangeCallback: function (value) { },
                searchCallback: function (value) { },
                openCloseGroupCallback: function (groupId, isClosed) { },
                updateValue: function (newValue) { },
                mount: function () { },
                destroy: function () { },
                focus: function () { },
                toggleOpenClose: function () { },
            };
        return new Treeselect({ ...defaultProps, ...t.props });
    }
}
class IntroJSCustom {
    constructor() {
        this.init = introJs();
    }
    defaultOptions(options) {
        return {
            disableInteraction: true,
            showProgress: true,
            showBullets: false,

            nextLabel: "Tiếp tục",
            prevLabel: "Quay lại",
            doneLabel: "Đã hiểu",
            hintButtonLabel: "Đã hiểu",
            ...options
        }
    }
    start({ module, options = {} }) {
        var introCustom = this;
        return introCustom.init.setOptions(introCustom.defaultOptions(options)).start();
    }
    addHints() {
        var introCustom = this;
        return introCustom.init.setOptions(introCustom.defaultOptions()).addHints();
    }
}
class CalendarCustom {
    constructor({ name, props = {} }) {
        this.name = name;
        this.props = props;
    }
    init() {
        var calendarCustom = this,
            defaultProps = {
                themeSystem: 'bootstrap5',
                locale: "vi",
                timeZone: 'UTC',
                headerToolbar: {
                    left: 'prev,next today',
                    //center: 'title',
                    right: 'multiMonth,timeGridWeek,timeGridDay,listWeek'
                },
                buttonText: {
                    prev: 'Trước',
                    next: 'Sau',
                    today: 'Hôm nay',
                    multiMonth: 'Năm',
                    timeGridWeek: 'Tuần',
                    timeGridDay: 'Ngày',
                    listWeek: 'Danh sách tuần',
                    month: 'Tháng',
                    week: 'Tuần',
                    day: 'Ngày',
                    list: 'Danh sách'
                },
                //dayHeaderFormat: {
                //    weekday: 'long', // Hiển thị ngày trong tuần đầy đủ
                //    month: 'short', // Hiển thị tháng ngắn
                //    day: 'numeric' // Hiển thị ngày theo số
                //},
                titleFormat: {
                    year: 'numeric', // Hiển thị năm theo số
                    month: 'long', // Hiển thị tháng đầy đủ
                    day: 'numeric' // Hiển thị ngày theo số
                },
                weekNumberFormat: {
                    week: 'numeric' // Hiển thị tuần theo số
                },
                eventResizableFromStart: true, // Cho phép kéo dài sự kiện từ cả hai đầu
                navLinks: true, // Cho phép nhấp vào ngày/tuần để điều hướng
                selectable: true, // Cho phép chọn nhiều ô trên lịch
                nowIndicator: true, // Hiển thị chỉ báo thời gian hiện tại
                editable: true, // Cho phép chỉnh sửa sự kiện
            };

        return new FullCalendar.Calendar($(calendarCustom.name)[0], {
            ...defaultProps,
            ...calendarCustom.props,
        });
    }
}

