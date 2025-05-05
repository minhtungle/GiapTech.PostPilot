'use strict'
class Messenger {
    constructor() {
        this.page;
        this.pageGroup;
    }
    init() {
        var mess = this;
        mess.page = $("#page-messenger");
        mess.pageGroup = mess.page.attr("page-group");
        // Ẩn khung chat
        $("#khungchat-active").css({
            "display": "none"
        })
        // side-bar
        $('.sidebar-toggle').on('click', () => {
            $('.email-app-sidebar').toggleClass('show')
        })
        $('.sidebar-close-icon').on('click', () => {
            $('.email-app-sidebar').removeClass('show')
        });

        chat = new Chat("#page-messenger");

        sys.activePage({
            page: mess.page.attr("id"),
            pageGroup: mess.pageGroup
        });
    }
    activeChat() {
        chat.tinNhanChuaXem = 0
        $("#khungchat-tinnhanmoi").text(chat.tinNhanChuaXem);
        $("#khungchat-container").toggleClass("active");
        $("#khungchat-tinnhanmoi").removeClass("active");
    }
}
class Chat {
    constructor(parentName = "") {
        this.chatService = $.connection.chatService;

        this.$tenNguoiGui = htmlEl.findByParent("#input-tennguoidung-dangsudung");
        this.$idNguoiGui = htmlEl.findByParent("#input-idnguoidung-dangsudung");
        this.$idKieuNguoiDung = htmlEl.findByParent("#input-idkieunguoidung-dangsudung");
        this.$scrollBottom = htmlEl.findByParent(".noidung-xuongcuoi", parentName);
        this.$messInput = htmlEl.findByParent(".noidung-gui", parentName);
        this.$chatContainer = htmlEl.findByParent(".noidung-hoithoai", parentName);
        this.$sendButton = htmlEl.findByParent(".guitinnhan", parentName);

        this.hoiThoai = [];
        this.tinNhanChuaXem = 0;
        this.gioiHanTinNhanCache = 500;
        this.soLuongTinNhanHienThi1Lan = 500;
        this.init();
    }
    init() {
        var chat = this;
        chat.chatService.client.dangXuatNguoiDungHoatDong = function (res) {
            // Gửi tin nhắn cho toàn bộ người dùng kiểm tra xem nếu trùng thì thông báo đăng xuất
            if (res.LoaiHinhKiemTra.includes("idnguoidung")) {
                var idNguoiDungs_CanKiemTra = res.NguoiDungs.map(nguoiDung => nguoiDung.IdNguoiDung),
                    idNguoiDung_HienTai = chat.$idNguoiGui.val();
                if (idNguoiDungs_CanKiemTra.includes(idNguoiDung_HienTai))
                    sys.logoutDialog({
                        mess: res.NoiDungThongBao,
                    });
            };
            if (res.LoaiHinhKiemTra.includes("idkieunguoidung")) {
                // Gửi tin nhắn cho toàn bộ người dùng kiểm tra xem nếu trùng thì thông báo đăng xuất
                var idKieuNguoiDungs_CanKiemTra = res.NguoiDungs.map(nguoiDung => nguoiDung.IdKieuNguoiDung),
                    idKieuNguoiDung_HienTai = chat.$idKieuNguoiDung.val();
                if (idKieuNguoiDungs_CanKiemTra.includes(idKieuNguoiDung_HienTai))
                    sys.logoutDialog({
                        mess: res.NoiDungThongBao,
                    });
            };
        };
        /**
         * LOGIC:
         * B1: Lấy cache hội thoại
         * B2: Hiển thị tin nhắn cũ (gán scroll thì hiển thị tin nhắn cũ)
         * B3: Hiển thị tin nhắn mới (luôn giữ cache chỉ có 1000 tin nhắn)
         */
        // B1
        var hoiThoai_Cache = localStorage.getItem("hoiThoai");
        chat.hoiThoai = hoiThoai_Cache ? JSON.parse(hoiThoai_Cache) : chat.hoiThoai;
        var tinNhanGanNhat = chat.layTinNhanCu(chat.hoiThoai, chat.soLuongTinNhanHienThi1Lan);
        // B2
        chat.$chatContainer.scroll(function () {
            var chatHeight = chat.$chatContainer.height();
            var scrollHeight = $(this).prop("scrollHeight");
            var topPosition = $(this).scrollTop();
            var bottomPosition = topPosition + chatHeight;
            // Vị trí hiện tại đã tiến tới khung chat cuối
            if (bottomPosition >= (scrollHeight - chatHeight)) {
                chat.$scrollBottom.css({
                    "display": "none"
                });
            } else {
                chat.$scrollBottom.css({
                    "display": "block"
                });
            };
            if (topPosition === 0) {
                //var tinNhanCu = chat.layTinNhanCu(chat.hoiThoai, chat.soLuongTinNhanHienThi1Lan);
                //chat.hienThiTinNhanCu(tinNhanCu);
            };
        });
        chat.hienThiTinNhanCu({
            hoiThoai: tinNhanGanNhat
        });
        // B3
        chat.hienThiTinNhanMoi();
        /**
        * Kích hoạt sự kiện gửi tin nhắn
        */
        chat.$sendButton.on("click", function () {
            chat.guiTinNhan();
        });
        chat.$messInput.on("keypress", function (e) {
            if (e.which == 13 || e.key == "Enter") {
                e.preventDefault();
                chat.guiTinNhan();
            };
        });
    }
    taoHTML_TinNhan(tinNhan) {
        var chat = this,
            tenNguoiDung = chat.$tenNguoiGui.val(), // Lấy tên người dùng của tài khoản
            dangSuDung = false; // Trạng thái là người dùng tài khoản
        // Kiểm tra tin nhắn trước đó có cùng 1 người dùng không
        var tinNhan_Truoc = chat.hoiThoai[tinNhan.stt - 2];
        var tenNguoiGui_Truoc, ngayGui_Truoc, gioGui_Truoc = "";
        if (tinNhan_Truoc) {
            tenNguoiGui_Truoc = tinNhan_Truoc.tenNguoiGui;
            ngayGui_Truoc = tinNhan_Truoc.thoiGian.ngayGui;
            gioGui_Truoc = tinNhan_Truoc.thoiGian.gioGui;
        };
        // Kiểm tra là tin nhắn đến từ ai
        if (tinNhan.tenNguoiGui == tenNguoiDung) {
            dangSuDung = true;
        };
        // Mã hóa tin nhắn thành văn bản thuần túy
        var tinNhanNoiDung = htmlEl.changeHTMLtoString(tinNhan.noiDungGui);
        return `
        ${ngayGui_Truoc == tinNhan.thoiGian.ngayGui ? "" : `
        <div class="form-group">
            <div class="row">
                <div class="col-12 text-center">
                    <div class="divider divider-center">
                        <div class="divider-text text-uppercase">
                            <small class="">${tinNhan.thoiGian.ngayGui}</small>
                        </div>
                    </div>
                </div>
            </div>
        </div>`}
        <div class="form-group tinnhan" data-stt="${tinNhan.stt}">
            <div class="row">
                <div class="col-12 my-1 bold ${dangSuDung ? 'justify-content-end' : ''} ${tenNguoiGui_Truoc == tinNhan.tenNguoiGui ? "d-none" : "d-flex"}">
                    <i class="bi bi-person-circle ${dangSuDung ? 'order-2' : 'order-1'}"></i>
                    <span class="${dangSuDung ? 'order-1' : 'order-2'}">&nbsp;${dangSuDung ? 'Bạn' : tinNhan.tenNguoiGui}&nbsp;</span>
                </div>
                <div class="col-12 d-flex ${dangSuDung ? 'justify-content-end' : ''}">
                    <div class="tinnhan-noidung ${dangSuDung ? 'order-2' : 'order-1'}">${tinNhanNoiDung}</div>
                    <small class="tinnhan-thoigian ${dangSuDung ? 'order-1' : 'order-2'}">&nbsp;${gioGui_Truoc == tinNhan.thoiGian.gioGui ? "" : tinNhan.thoiGian.gioGui}&nbsp;</small>
                </div>
            </div>
        </div>`;
    }
    layTinNhanCu(hoiThoai, soLuong) {
        var chat = this;
        if (hoiThoai.length < soLuong) {
            return hoiThoai;
        };
        if (chat.$chatContainer) {
            // Số lượng tin nhắn đang hiển thị
            var $tinNhanDangHienThi = $(".tinnhan", chat.$chatContainer),
                soLuongTinNhanDangHienThi = $tinNhanDangHienThi.length;
            /**
             * Danh sách tin nhắn cần hiển thị tiếp theo
             * LOGIC
             * Mảng: [1, 2, 3, ... , 98, 99, 100]
             * B1: Đảo ngược mảng
             * B2: Lấy danh sách cần và đảo ngược
             */
            var hoiThoai_reverse = [...hoiThoai].reverse();
            var tinNhanCanLay = hoiThoai_reverse.slice(soLuongTinNhanDangHienThi, soLuong).reverse();
            return tinNhanCanLay;
        };
        return hoiThoai.slice(-soLuong);
    }
    hienThiTinNhanCu({ hoiThoai, isCache = false }) {
        if (hoiThoai.length > 0) {
            var chat = this,
                hoiThoai_HTML = hoiThoai.map(tinNhan => chat.taoHTML_TinNhan(tinNhan)).reduce((before, later) => before + later);
            if (isCache) {
                chat.$chatContainer.html(hoiThoai_HTML);
            } else {
                chat.$chatContainer.html(hoiThoai_HTML);
            };
            // Kéo xuống cuối
            //chat.cuonXuongCuoi();
        };
    }
    hienThiTinNhanMoi() {
        var chat = this;
        chat.chatService.client.nhanTinNhan = function (
            res
            //idNguoiGui, tenNguoiGui, noiDungGui, tenNhom
        ) {
            var padZero = (number) => {
                return number < 10 ? '0' + number : number;
            };
            //#region Lấy thời gian hiện tại
            var date = new Date(),
                options = { weekday: 'long', year: 'numeric', month: '2-digit', day: '2-digit' },
                formatter = new Intl.DateTimeFormat('vi-VN', options),
                formattedDate = formatter.format(date),
                hour = date.getHours(),
                minute = date.getMinutes(),
                ampm = hour >= 12 ? 'chiều' : 'sáng',
                formattedTime = `${padZero(hour)}:${padZero(minute)} ${ampm}`,
                // Định dạng chuỗi giờ và phút
                thoiGian = {
                    ngayGui: formattedDate,
                    gioGui: formattedTime
                };
            //#endregion

            //#region Thêm mới cache
            var tinNhan_NEW = {
                stt: chat.hoiThoai.length + 1,
                tenNguoiGui: res.NguoiGui.TenNguoiDung,
                noiDungGui: res.NoiDungGui,
                thoiGian
            }
            if (chat.hoiThoai.length == chat.gioiHanTinNhanCache) chat.hoiThoai.shift();
            chat.hoiThoai.push(tinNhan_NEW);
            localStorage.setItem("hoiThoai", JSON.stringify(chat.hoiThoai))
            //#endregion

            //#reigon Hiển thị tin nhắn
            var hoiThoai_HTML = chat.taoHTML_TinNhan(tinNhan_NEW)
            chat.$chatContainer.append(hoiThoai_HTML);
            //#endregion
            //#region Kiểm tra khung chat đang mở thì không hiển thị thông báo tin nhắn mới
            var trangThaiXem = $("#khungchat-container").hasClass("active");
            $("#khungchat-tinnhanmoi").removeClass("active");
            if (trangThaiXem) {
                chat.tinNhanChuaXem = 0;
                $("#khungchat-tinnhanmoi").text(chat.tinNhanChuaXem);
            } else {
                chat.tinNhanChuaXem++;
                $("#khungchat-tinnhanmoi").text(chat.tinNhanChuaXem);
                $("#khungchat-tinnhanmoi").addClass("active");
            };
            //#endregion
            // Kéo xuống cuối
            chat.cuonXuongCuoi();
        };
    }
    guiTinNhan() {
        var chat = this,
            idNguoiGui = chat.$idNguoiGui.val(),
            tenNguoiGui = chat.$tenNguoiGui.val(),
            noiDungGui = chat.$messInput.val(),
            tenNhom = "nhomchung";
        if (noiDungGui != "") {
            //chat.chatService.server.thamGiaNhomChat(tenNhom);
            // Call the Send method on the hub.
            chat.chatService.server.guiTinNhan({
                NguoiGui: {
                    IdNguoiDung: idNguoiGui,
                    TenNguoiDung: tenNguoiGui,
                },
                NoiDungGui: noiDungGui,
                NhomChat: {
                    TenNhom: tenNhom,
                },
            });
            // Clear text box and reset focus for next comment.
            chat.$messInput.val('').focus();
        };
    }
    cuonXuongCuoi() {
        var chat = this;
        chat.$chatContainer.scrollTop(chat.$chatContainer.prop("scrollHeight"));
    }
    test() {
        var chat = this;
        chat.chatService.server.test({
            idNguoiDungs_CanKiemTra: 1,
            noiDungGui: "test",
            tenNhom: "kiemtra"
        });
    }

    dangXuatNguoiDungHoatDong({ thongTinKiemTra }) {
        var chat = this;
        chat.chatService.server.dangXuatNguoiDungHoatDong(thongTinKiemTra);
    }
}
var chat = new Chat();