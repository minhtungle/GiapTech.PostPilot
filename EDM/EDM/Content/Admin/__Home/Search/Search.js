'use strict'

class HuongDanSuDung {
    constructor() {

    }
    kichHoat(chucNang) {
        var huongDanSuDung = this;
        var soLanDangNhap = $("#input-solandangnhap").val(),
            tenNguoiDung_DangSuDung = $("#input-tennguoidung-dangsudung").val(),
            introJsCustom = new IntroJSCustom(),
            options = {};
        if (chucNang == "timkiem-coban") {
            options = {
                steps: [{
                    element: $('[intro-container="navbar-menu-nguoidung"]')[0],
                    intro: `<p>Xin chàoo !!! <span class='text-danger'>${tenNguoiDung_DangSuDung}</span></p>
                            <p>🤖: Tôi là <span class='text-primary font-bold'>Bot</span> - hướng dẫn viên hệ thống, có vẻ đây là lần đầu chúng ta gặp nhau.</p>
                            <p>🤖: Bây giờ, hãy thử nhấn nút <span class='text-danger font-bold'>Tiếp tục</span> hoặc các phím mũi tên ◀️ ▶️ trên bàn phím.</p>`
                }, {
                    element: $('[intro-container="btn-huongdansudung-tongquat"]')[0],
                    intro: `<p>🤖: Đó là cách di chuyển giữa các bước hướng dẫn.</p>
                            <p>🤖: Đừng lo nếu quá trình hướng dẫn không may bị ngắt quãng.</p>
                            <p>🤖: Bạn có thể mở lại tại đây bất cứ lúc nào <small class='font-bold'>không mở được là do đang ngủ nha 😴</small>.</p>`
                }, {
                    element: $('[intro-container="sidebar-menu"]')[0],
                    intro: `<p>🤖: Phía bên trái là danh mục chức năng bạn được phép truy cập do <span class='text-danger fst-italic'>quản trị viên</span> phân quyền .</p>`,
                    position: 'right'
                }, {
                    element: $('[intro-container="navbar-hienthi-sidebar"]')[0],
                    intro: `<p>🤖: Nút đóng mở danh mục chức năng.</p>
                            <p>🤖: <span class='text-danger font-bold'>[MẸO]</span> khi đóng lại bạn sẽ có nhiều không gian làm việc hơn đó.</p>`
                }, {
                    element: $('[intro-container="navbar-tinnhan"]')[0],
                    intro: `<p>🤖: Đây là mục tin nhắn chung.</p>
                            <p>🤖: Cuộc hội thoại của bạn và người dùng khác sẽ được cập nhật tại đây, tuy không giống <span class='text-primary font-bold'>facebook</span> lắm.</>`
                }, {
                    element: $('[intro-container="navbar-homthu"]')[0],
                    intro: `<p>🤖: Bên cạnh là hòm thư cá nhân.</p>
                            <p>🤖: Nơi quản lý các mail thông báo công việc.</p>`
                }, {
                    element: $('[intro-container="navbar-menu-nguoidung"]')[0],
                    intro: `<p>🤖: Lối tắt tới quản lý thông tin tài khoản.</p>`
                }, {
                    element: $('[intro-container="body"]')[0],
                    intro: `<p>🤖: Đây là màn hình công việc chính.</p>
                            <p>🤖: Tất cả thông tin về dữ liệu đã lưu trữ, thao tác với dữ liệu đều được thực hiện phía bên trong màn hình này.</p>`
                }, {
                    element: $('[intro-container="navbar-menu-nguoidung"]')[0],
                    intro: `<p>🤖: Và đó là tổng quan về toàn bộ không gian làm việc.</p>
                            <p>🤖: Nếu bạn đã nắm rõ thì chúng ta sẽ đi tới hướng dẫn chi tiết về luồng công việc.</p>
                            <p>🤖: Còn nếu chưa, bạn có thể vào mục <span class='text-primary font-bold text-decoration-underline c-pointer'><a href='/AccountInfo/Index'>Thông tin tài khoản</a></span> để kích hoạt lại trạng thái <span class='text-danger font-bold'>Lần đầu đăng nhập</span> và làm quen lại với <span class='text-primary font-bold'>Bảnh</span> hehehe.</p>`
                }, {
                    element: $('[intro-container="timkiem-noidung-container"]')[0],
                    intro: `<p>🤖: Thanh tìm kiếm gồm 3 mục chính.</p>`
                }, {
                    element: $('[intro-container="timkiem-nangcao"]')[0],
                    intro: `<p>🤖: Mục tìm kiếm nâng cao.</p>
                            <p>🤖: Sử dụng để tìm kiếm nhiều trường thông tin cùng lúc.</p>`
                }, {
                    element: $('[intro-container="timkiem-noidung"]')[0],
                    intro: '<p>🤖: Nhập nội dung tìm kiếm.</p>'
                }, {
                    element: $('[intro-container="timkiem-chontieuchi"]')[0],
                    intro: `<p>🤖: Tìm kiếm cơ bản sẽ dựa vào các tiêu chí dưới đây.</p>
                            <p>🤖: Bạn có thể chọn tất cả hoặc chỉ chọn 1 tiêu chí bất kỳ.</p>`
                }, {
                    element: $('[intro-container="timkiem-ketqua-solieu"]')[0],
                    intro: `<p>🤖: Số liệu về kết quả tìm kiếm.</p>`
                }, {
                    element: $('[intro-container="search-timkiemhoso-ketquatimkiem"]')[0],
                    intro: `<p>🤖: Chuyển đổi màn hình xem kết quả và phiếu mượn.</p>`
                }]
            };
        } else if (chucNang == "timkiem-nangcao") {
            options = {
                steps: [{
                    element: $('[intro-container="timkiem-vitriluutru"]')[0],
                    intro: '<p>🤖: Lựa chọn (vị trí, danh mục, phông) - nơi lưu giữ hồ sơ.</p>'
                }, {
                    element: $('[intro-container="timkiem-nangcao-container"]')[0],
                    intro: '<p>🤖: Nhập nội dung các trường dữ liệu của văn bản.</p>'
                }, {
                    element: $('[intro-container="timkiem-reset"]')[0],
                    intro: '<p>🤖: Khôi phục về trạng thái ban đầu.</p>'
                },]
            };
        } else {

        };

        introJsCustom.start({ options });
        // Lần dầu đăng nhập
        if (soLanDangNhap != 0 && chucNang == "timkiem-coban") {
            introJsCustom.init.goToStepNumber(10);
        };
        //introJsCustom.init.onbeforechange(function (targetElement) {
        //    if ($(targetElement).attr('intro-container') == "next-step") {
        //        sys.displayModal({
        //            name: '#timkiemhoso-nangcao',
        //        });
        //    };
        //    if ($(targetElement).attr('intro-container') == "timkiem-noidung")
        //        sys.displayModal({
        //            name: '#timkiemhoso-nangcao',
        //            displayStatus: "hide"
        //        });
        //});
        //introJsCustom.init.onafterchange(function (targetElement) {
        //    if ($(targetElement).attr('intro-container') == "next-step") {
        //        introJsCustom.init.nextStep();
        //    };
        //});
    }
    lanDauDangNhap(chucNang) {
        var huongDanSuDung = this;
        /**
         * 1. Gán sự kiện hướng dẫn theo chức năng
         * 2. Nếu là lần đầu đăng nhập thì tự đổng mở hướng dẫn
         */
        var soLanDangNhap = $("#input-solandangnhap").val();
        soLanDangNhap == 0 && huongDanSuDung.kichHoat(chucNang);
    }
}
var huongDanSuDung = new HuongDanSuDung();
class TruongDuLieu {
    constructor() {
        this.table;
        this.dataTable;
    }
    init() {
        var tdl = this;
        tdl.table = $("#table-truongdulieu");
        tdl.dataTable = new DataTableCustom({
            name: "table-truongdulieu",
            table: tdl.table,
            props: {
                lengthMenu: [
                    [5, 10, 15, -1],
                    [5, 10, 15, 'Tất cả'],
                ],
                columnDefs: [{
                    target: [-1],
                    orderSequence: ["asc"],
                    className: 'text-center',
                }],
                initCompleteProps: function () { }
            },
        }).dataTable;
    }
}
class Search {
    constructor() {
        this.page;
        this.thongTinTimKiem = {
            loai: "coban",
            data: [],
        }
        this.truongDuLieu = {
            data: [],
            dataTable: null,
        }
        this.ketQua = {
            data: [],
            dataTable: null
        }
        this.phieuMuon = {
            data: [],
            $dongDuocChons: [],
            dataTable: null,
            createRow: function () { },
            deleteRow: function () { },
            themDanhSachMuon: function () { },
            displayModal_CRUD: function () { },
            save: function () { },

        }
        this.vanBan = {
            data: [],
            dataTable: null,
            truongDuLieu: null,
            getList: function () { },
            activePDF: function () { },
            displayModal_Read_VanBan: function () { },
            displayPartial_DuLieuSos: function () { },
            redirectVanBan: function () { },
        }
    }
    init() {
        var s = this;
        // Tạo thanh lọc tiêu chí tìm kiếm
        {
            /*var selec2Custom = new Select2Custom({
                $select: $("#timkiem-chontieuchi"),
                props: {
                    placeholder: "Chọn tiêu chí tìm kiếm",
                    //allowClear: true
                }
            }).init();
            $("#timkiem-chontieuchi").on("select2:opening select2:closing", function (event) {
                var $searchfield = $(this).parent().find('.select2-search__field'),
                    $searchinline = $(this).parent().find('.select2-search--inline');
                $searchinline.css({
                    "display": "none"
                });
                $searchfield.prop('disabled', true);
            });
            $("#timkiem-chontieuchi").on("change", function (e) {
                if ($(this).val().length == 0) {
                    e.preventDefault();
                };
            });*/
            var tieuChiTimKiem_Cache = [
                {
                    ten: "MaHoSo",
                    trangThai: false,
                },
                {
                    ten: "TieuDeHoSo",
                    trangThai: false,
                },
                {
                    ten: "TenVanBan",
                    trangThai: false,
                },
                {
                    ten: "DuLieuSo",
                    trangThai: true,
                },
            ];
            // Lấy dữ liệu từ cache
            tieuChiTimKiem_Cache = localStorage.getItem("tieuChiTimKiem") ? JSON.parse(localStorage.getItem("tieuChiTimKiem")) : tieuChiTimKiem_Cache;
            var $tags = $("#timkiem-chontieuchi-container .tag");
            $.each($tags, function () {
                var ten = $(this).attr("data-ten"),
                    trangThai_Cache = tieuChiTimKiem_Cache.filter(x => x.ten == ten)[0].trangThai;
                $(this).attr("data-trangthai", trangThai_Cache);
            });
            // Gán sự kiện chọn
            $("#timkiem-chontieuchi-container .tag").on("click", function () {
                var $tag = $(this),
                    ten = $tag.attr("data-ten"),
                    trangThai = $tag.attr("data-trangthai"),
                    soLuongTieuChi = $tags.length,
                    soLuongTieuChi_Checked = $("#timkiem-chontieuchi-container .tag[data-trangthai='true']").length;
                if (trangThai == "true") {
                    if ((soLuongTieuChi - soLuongTieuChi_Checked) != (soLuongTieuChi - 1)) {
                        $tag.attr("data-trangthai", false);
                        tieuChiTimKiem_Cache.filter(x => x.ten == ten)[0].trangThai = false;
                    };
                } else {
                    $tag.attr("data-trangthai", true);
                    tieuChiTimKiem_Cache.filter(x => x.ten == ten)[0].trangThai = true;
                };
                // Gán lại cache
                localStorage.setItem("tieuChiTimKiem", JSON.stringify(tieuChiTimKiem_Cache))
            });
        };
        s.page = $("#page-home");
        s.truongDuLieu = {
            ...s.truongDuLieu,
            dataTable: new DataTableCustom({
                name: "timkiem-truongdulieu-getList",
                table: $("#timkiem-truongdulieu-getList"),
                props: {
                    maxHeight: 500,
                    dom: `
                    <'row'<'col-sm-12 col-md-6'>>
                    <'row'<'col-sm-12'rt>>
                    <'row'<'col-sm-12 col-md-4 pt-2'l><'col-sm-12 col-md-4 text-center'i><'col-sm-12 col-md-4 pt-2'p>>`,
                    lengthMenu: [
                        [10, 20, 50, -1],
                        [10, 20, 50, 'Tất cả'],
                    ],
                    ajax: {
                        url: '/Search/getList_TruongDuLieus',
                        type: "GET",
                        complete: function (data) {
                            s.ketQua.data = data.responseJSON.data;
                        },
                    },
                    rowId: 'str_IdTruongDuLieus',
                    columns: [
                        {
                            data: "TenTruong",
                            className: "text-left w-45 tentruong",
                        },
                        {
                            data: null,
                            className: "text-center",
                            searchable: false,
                            //orderable: false,
                            render: function (data, type, row, meta) {
                                return type == "display" ?
                                    `<input type="text" class="form-control dulieuso" placeholder="Tìm kiếm ..." />` : data.str_IdTruongDuLieus;
                            }
                        }
                    ],
                }
            }).dataTable
        }
        s.ketQua = {
            ...s.ketQua,
            locDuLieu: function (duration = 100) {
                setTimeout(function () {
                    var filter = {
                        cheDoSuDung: [],
                        timKiem: $("#filter-noidung").val().trim()
                    };
                    $("#filter-congkhai").is(":checked") && filter.cheDoSuDung.push($("#filter-congkhai").siblings("label[for='filter-congkhai']").text().trim());
                    $("#filter-hanche").is(":checked") && filter.cheDoSuDung.push($("#filter-hanche").siblings("label[for='filter-hanche']").text().trim());
                    $("#filter-baomat").is(":checked") && filter.cheDoSuDung.push($("#filter-baomat").siblings("label[for='filter-baomat']").text().trim());
                    var $ketQua = $(".card-ketqua-container", $("#ketquatimkiem-getList"));
                    $.each($ketQua, function () {
                        // Hiển thị bản ghi
                        $(this).show();
                        // Lấy thông tin
                        var tenCheDoSuDung = $(this).find("span[name='TenCheDoSuDung']").text(),
                            maHoSo = $(this).find("span[name='MaHoSo']").text(),
                            tenVanBan = $(this).find("span[name='TenVanBan']").text(),
                            trangSo = $(this).find("span[name='TrangSo']").text();
                        var thongTin = `${maHoSo}|${tenVanBan}|${trangSo}`;
                        // Lọc dữ liệu
                        var trangThai = true;
                        if (filter.timKiem != "") {
                            var noiDung = filter.timKiem.split(",");
                            trangThai = noiDung.some(x => thongTin.includes(noiDung));
                        };
                        if (filter.cheDoSuDung.length > 0) {
                            trangThai = filter.cheDoSuDung.includes(tenCheDoSuDung);
                        };
                        // Ẩn hiện bản ghi
                        if (trangThai) {
                            $(this).show();
                        } else {
                            $(this).hide();
                        };
                    });
                }, duration);
            }
        };
        s.phieuMuon = {
            ...s.phieuMuon,
            dataTable: new DataTableCustom({
                name: "danhsachmuon-getList",
                table: $("#danhsachmuon-getList"),
                props: {
                    dom: `
                    <'row'<'col-sm-12 col-md-6'>>
                    <'row'<'col-sm-12'rt>>
                    <'row'<'col-sm-12 col-md-4 pt-2'l><'col-sm-12 col-md-4 text-center'i><'col-sm-12 col-md-4 pt-2'p>>`,
                    lengthMenu: [
                        [10, 50, -1],
                        [10, 50, 'Tất cả'],
                    ],
                    columnDefs: [
                        {
                            className: "text-center",
                            targets: [0, 2, 3]
                        }
                    ]
                }
            }).dataTable,
            createRow: function (vanBanThems) {
                s.phieuMuon.data.push(...vanBanThems);
                $.each(vanBanThems, function (i, data) {
                    var trangSo = parseInt(data.TrangSo.split(",")[0], 10);
                    var tenVanBan = data.TenVanBan_BanDau != null ? data.TenVanBan_BanDau : data.TenVanBan;
                    var $tds = [
                        `<input class="form-check-input checkRow-danhsachmuon-getList" data-idvanban="${data.IdVanBan}" data-tenvanban="${tenVanBan}" data-tieudehoso="${data.TieuDeHoSo}" type="checkbox"/>`,

                        `<span hidden>${data.MaHoSo}-${data.TenCheDoSuDung}-${data.TieuDeHoSo}-${tenVanBan}-${data.GhiChu}</span>
                        <div class="list-group-item list-group-item-action position-relative">
                            <div class="d-flex w-100 justify-content-between">
                                <p class="mb-1 long-text"><span class="text-danger">${data.MaHoSo} | ${data.TenCheDoSuDung}</span></p>
                            </div>
                            <p class="mb-1 long-text">
                                <span class="font-bold fs-5 text-uppercase" title="${tenVanBan}">${sys.truncateString(tenVanBan, 60)}</span><br />
                                <span class="text-primary" title="${data.TieuDeHoSo}">${sys.truncateString(data.TieuDeHoSo, 20)} | Trang số: ${data.TrangSo}</span><br />
                                ${((data.GhiChu == null || data.GhiChu == "") ? "" : data.GhiChu)}
                            </p>
                        </div>`,

                        `<div class="row">
                            <div class="col-sm-12 col-md-6">
                                <input type="number" class="form-control numberInt32format tutrang" limit="0-max" placeholder="Bắt đầu" value="1"/>
                            </div>
                            <div class="col-sm-12 col-md-6">
                                <input type="number" class="form-control numberInt32format dentrang" limit="0-max" placeholder="Kết thúc" value="2"/>
                            </div>
                        </div>`,

                        `
                        <a class="btn btn-sm btn-primary" onclick="s.vanBan.displayModal_Read_VanBan(${data.IdHoSo}, ${data.IdVanBan}, ${trangSo})" title="Xem văn bản"><i class="bi bi-images"></i></a>
                        <a href="#" class="btn btn-sm btn-danger" title="Xóa bỏ" onclick="s.phieuMuon.deleteRow(this)"><i class="bi bi-trash3-fill"></i></a>
                        `
                    ];
                    var newRows = s.phieuMuon.dataTable.row.add($tds).draw(false);
                    //$(newRows).attr("id", data.IdVanBan);
                    htmlEl.inputMask()
                });
                let phieuMuonSoluong = s.phieuMuon.dataTable.rows().count();
                $(".phieumuon-soluong").text(phieuMuonSoluong);
            },
            deleteRow(e) {
                var $tr = $(e).closest("tr"),
                    idVanBan = $tr.find("input.checkRow-danhsachmuon-getList").data("idvanban");
                s.phieuMuon.data = s.phieuMuon.data.filter(x => x.IdVanBan != idVanBan);
                s.phieuMuon.dataTable.row($tr).remove().draw();
                let phieuMuonSoluong = s.phieuMuon.dataTable.rows().count();
                $(".phieumuon-soluong").text(phieuMuonSoluong);
            },
            themDanhSachMuon: function () {
                var idVanBanThems = [], vanBanThems = [];
                // Lấy danh sách hồ sơ mượn
                /*s.ketQua.dataTable.rows().iterator('row', function (context, index) {
                    var $row = $(this.row(index).node());
                    if ($row.has("input.checkRow-ketquatimkiem-getList:checked").length > 0) {
                        idVanBanThems.push($row.attr('id'));
                    };
                });*/
                var $ketqua = $("input.checkRow-ketquatimkiem-getList:checked", $("#ketquatimkiem-getList"));
                $.each($ketqua, function () {
                    let idVanbanThem = $(this).data("idvanban");
                    idVanBanThems.push(idVanbanThem);
                });
                // Kiểm tra bản ghi
                if (idVanBanThems.length == 0) {
                    sys.alert({ mess: "Bạn chưa chọn bản ghi nào", status: "warning", timeout: 1500 })
                } else {
                    $.each(idVanBanThems, function (i, idVanBanThem) {
                        var vanBan = s.ketQua.data.find(x => x.IdVanBan == idVanBanThem);
                        if (s.phieuMuon.data.length > 0) {
                            if (!s.phieuMuon.data.some(x => x.IdVanBan == idVanBanThem)) {
                                vanBanThems.push(vanBan)
                            }
                        } else {
                            vanBanThems.push(vanBan)
                        }
                    });
                    vanBanThems.length > 0 && s.phieuMuon.createRow(vanBanThems);
                    sys.alert({ mess: "Thêm danh sách mượn thành công", status: "success", timeout: 1500 })
                }
            },
            displayModal_CRUD: function (loai = "", id = 0) {
                s.phieuMuon.$dongDuocChons.length = 0;
                s.phieuMuon.dataTable.rows().iterator('row', function (context, index) {
                    var $row = $(this.row(index).node());
                    if ($row.has("input.checkRow-danhsachmuon-getList:checked").length > 0) {
                        s.phieuMuon.$dongDuocChons.push($row);
                    };
                });
                // Kiểm tra bản ghi
                if (s.phieuMuon.$dongDuocChons.length == 0) {
                    sys.alert({ mess: "Bạn chưa chọn bản ghi nào", status: "warning", timeout: 1500 })
                } else {
                    sys.displayModal({
                        name: '#phieumuon-create'
                    });
                }
            },
            save: function () {
                var modalValidtion = htmlEl.activeValidationStates("#phieumuon-create");
                if (modalValidtion) {
                    var phieuMuon = {
                        NguoiMuon_HoTen: $("#input-nguoimuon_hoten", $("#phieumuon-create")).val().trim(),
                        NguoiMuon_CCCD: $("#input-nguoimuon_cccd", $("#phieumuon-create")).val().trim(),
                        NguoiMuon_SoDienThoai: $("#input-nguoimuon_sodienthoai", $("#phieumuon-create")).val().trim(),
                        NguoiMuon_DonViCongTac: $("#input-nguoimuon_donvicongtac", $("#phieumuon-create")).val().trim(),
                        NguoiMuon_Email: $("#input-nguoimuon_email", $("#phieumuon-create")).val().trim(),
                        NguoiMuon_LyDoMuon: $("#input-nguoimuon_lydomuon", $("#phieumuon-create")).val().trim(),
                        NgayYeuCau: $("#input-ngayyeucau", $("#phieumuon-create")).val().trim(),
                        NgayHenTra: $("#input-ngayhentra", $("#phieumuon-create")).val().trim(),
                        NguoiMuon_LyDoMuon: $("#input-nguoimuon_lydomuon", $("#phieumuon-create")).val().trim(),
                        IdHinhThucMuon: $("#select-hinhthucmuon", $("#phieumuon-create")).val(),
                        PhieuMuon_VanBans: []
                    };
                    if (phieuMuon.NgayYeuCau != "")
                        phieuMuon.NgayYeuCau = moment(phieuMuon.NgayYeuCau, 'YYYY-MM-DD');
                    if (phieuMuon.NgayHenTra != "")
                        phieuMuon.NgayHenTra = moment(phieuMuon.NgayHenTra, 'YYYY-MM-DD');
                    // Ngày yêu cầu < Ngày hẹn trả
                    if (phieuMuon.NgayYeuCau.isAfter(phieuMuon.NgayHenTra) || phieuMuon.NgayHenTra.isBefore(phieuMuon.NgayYeuCau)) {
                        var mess = "Ngày yêu cầu không được lớn hơn ngày hẹn trả";
                        htmlEl.inputValidationStates(
                            $("#input-ngayyeucau"),
                            "#phieumuon-create",
                            mess,
                            {
                                status: true,
                                isvalid: false
                            }
                        );
                        htmlEl.inputValidationStates(
                            $("#input-ngayhentra"),
                            "#phieumuon-create",
                            mess,
                            {
                                status: true,
                                isvalid: false
                            }
                        );
                    } else {
                        phieuMuon.NgayYeuCau = phieuMuon.NgayYeuCau.format('DD/MM/YYYY');
                        phieuMuon.NgayHenTra = phieuMuon.NgayHenTra.format('DD/MM/YYYY');
                        $.each(s.phieuMuon.$dongDuocChons, function (i, $row) {
                            phieuMuon.PhieuMuon_VanBans.push({
                                IdVanBan: $row.find("input.checkRow-danhsachmuon-getList").data("idvanban"),
                                HoSo: {
                                    TieuDeHoSo: $row.find("input.checkRow-danhsachmuon-getList").data("tieudehoso"),
                                },
                                VanBan: {
                                    IdVanBan: $row.find("input.checkRow-danhsachmuon-getList").data("idvanban"),
                                    TenVanBan: $row.find("input.checkRow-danhsachmuon-getList").data("tenvanban"),
                                    TenVanBan_BanDau: $row.find("input.checkRow-danhsachmuon-getList").data("tenvanban"),
                                },
                                TuTrang: $("input.tutrang", $($row)).val().trim(),
                                DenTrang: $("input.dentrang", $($row)).val().trim(),
                            })
                        });
                        $.ajax({
                            ...ajaxDefaultProps({
                                url: "/LoanManage/create_PhieuMuon",
                                type: "POST",
                                data: {
                                    str_phieuMuon: JSON.stringify(phieuMuon),
                                }
                            }),
                            success: function (res) {
                                if (res.status == "success") {
                                    $.each(s.phieuMuon.$dongDuocChons, function (i, $row) {
                                        var idVanBan = $row.find("input.checkRow-danhsachmuon-getList").data("idvanban");
                                        s.phieuMuon.data = s.phieuMuon.data.filter(x => x.IdVanBan != idVanBan);
                                        s.phieuMuon.dataTable.row($row).remove().draw();
                                        let phieuMuonSoluong = s.phieuMuon.dataTable.rows().count();
                                        $(".phieumuon-soluong").text(phieuMuonSoluong);
                                    });
                                    sys.displayModal({
                                        name: '#phieumuon-create',
                                        displayStatus: "hide"
                                    });
                                    sys.alert({ status: res.status, mess: res.mess, timeout: 6000 });
                                } else {
                                    sys.alert({ status: res.status, mess: res.mess });
                                }
                            }
                        });
                    }
                };
            }
        };
        s.vanBan = {
            ...s.vanBan,
            getList: function () {
                s.vanBan.dataTable = new DataTableCustom({
                    name: "vanban-getList",
                    table: $("#vanban-getList"),
                    props: {
                    }
                }).dataTable;
            },
            activePDF: function (href) {
                //if (window.innerWidth < 768) {
                //    $("#file-nav").toggleClass("active")
                //} else {
                window.open(href)
                //}
            },
            displayModal_Read_VanBan: function (idHoSo, idVanBan, trangSo = 1) {
                $.ajax({
                    ...ajaxDefaultProps({
                        url: '/Search/displayModal_Read_VanBan',
                        type: "POST",
                        data: { idHoSo, idVanBan, trangSo }
                    }),
                    success: function (res) {
                        $("#vanban-detail").html(res);
                        s.vanBan.displayPartial_DuLieuSos();
                        sys.displayModal({
                            name: '#vanban-detail'
                        });
                    }
                })
            },
            displayPartial_DuLieuSos: function () {
                var idVanBan = $('#input-idvanban').val() || 0,
                    idBieuMau = $('#select-bieumau').val() || 0;
                $.ajax({
                    ...ajaxDefaultProps({
                        url: '/Search/get_DuLieuSos',
                        type: "POST",
                        data: { idVanBan, idBieuMau }
                    }),
                    success: function (res) {
                        $("#truongdulieus-container").html(res);
                        /**
                        * table-truongdulieu
                        * */
                        s.vanBan.truongDuLieu = new TruongDuLieu();
                        s.vanBan.truongDuLieu.init();
                    }
                })
            },
            redirectVanBan: function (idHoSo) {
                let url = window.location.origin + `/Search/VanBan?idHoSo=${idHoSo}`;
                window.open(url);
            }
        };

        sys.activePage({
            page: s.page.attr("id"),
        })
        $("#timkiem-noidung").off().on("keypress", function (e) {
            if (e.which == 13 || e.key == "Enter") {
                s.timKiem('coban');
            }
        })

        huongDanSuDung.lanDauDangNhap("timkiem-coban");
        $("#btn-huongdansudung-tongquat").off().on("click", () => huongDanSuDung.kichHoat("timkiem-coban"));
    }
    getThongTinTimKiem(loaiTimKiem) {
        var s = this;
        s.thongTinTimKiem.loai = loaiTimKiem;
        s.thongTinTimKiem.data = [];
        if (loaiTimKiem == "coban") {
            s.thongTinTimKiem.data = [{
                str_IdTruongDuLieus: "",
                TenTruong: "",
                IdViTriLuuTru: 0,
                IdDanhMucHoSo: 0,
                IdPhongLuuTru: 0,
                NoiDungTimKiem: $("#timkiem-noidung").val().trim(),
                TieuChiTimKiem: ""
            }];
            var $tags = $("#timkiem-chontieuchi-container .tag[data-trangthai='true']"),
                TieuChiTimKiem = [];
            $.each($tags, function () {
                var ten = $(this).attr("data-ten");
                TieuChiTimKiem.push(ten);
            });
            s.thongTinTimKiem.data[0].TieuChiTimKiem = TieuChiTimKiem.join(",");

        } else {
            var $timKiemNangCao = $("#timkiemhoso-nangcao");
            s.truongDuLieu.dataTable.rows().iterator('row', function (context, index) {
                var $row = $(this.row(index).node());
                //$checkBox = $("input[type='checkbox']:checked", $row); // Lấy dòng được chọn
                $.each($row, function () {
                    var $tr = $(this).closest('tr'),
                        o = {
                            str_IdTruongDuLieus: $tr.attr("id"),
                            TenTruong: $tr.find(".tentruong").text().trim(),
                            IdViTriLuuTru: $("#timkiem-vitriluutru", $timKiemNangCao).val(),
                            IdDanhMucHoSo: $("#timkiem-danhmuchoso", $timKiemNangCao).val(),
                            IdPhongLuuTru: $("#timkiem-phongluutru", $timKiemNangCao).val(),
                            NoiDungTimKiem: $tr.find(".dulieuso").val().trim(),
                        }
                    if (o.DuLieuSo != "") s.thongTinTimKiem.data.push(o);
                });
            });
        };
    }
    reset() {
        s.truongDuLieu.dataTable.rows().iterator('row', function (context, index) {
            var $row = $(this.row(index).node());
            //$checkBox = $("input[type='checkbox']:checked", $row); // Lấy dòng được chọn
            $.each($row, function () {
                var $tr = $(this).closest('tr');
                $tr.find(".dulieuso").val("");
            });
        });
    }
    timKiem(loaiTimKiem) {
        var s = this;
        $("#ketquatimkiem-container-tab").trigger("click");

        s.getThongTinTimKiem(loaiTimKiem);
        //s.ketQua.dataTable.ajax.reload();
        $.ajax({
            ...ajaxDefaultProps({
                url: "/Search/getList",
                type: "POST",
                data: {
                    str_thongTinTimKiem: JSON.stringify(s.thongTinTimKiem)
                }
            }),
            success: function (res) {
                // Xóa thông tin lọc
                $("#filter-noidung").val("");
                // Gán dữ liệu
                s.ketQua.data = res.vanBans;
                $(".ketqua-soluong").text(res.vanBans.length);
                $(".ketqua-congkhai-soluong").text(res.vanBans.filter(x => x.IdCheDoSuDung == 1).length);
                $(".ketqua-hanche-soluong").text(res.vanBans.filter(x => x.IdCheDoSuDung == 2).length);
                $(".ketqua-baomat-soluong").text(res.vanBans.filter(x => x.IdCheDoSuDung == 3).length);
                // Thêm view
                $("#ketquatimkiem-getList").html(res.view);
                var windowHeight = $(window).height(), // Chiều cao của cửa sổ trình duyệt
                    elementHeight = $("#search-timkiemhoso-ketquatimkiem").height(), // Chiều cao của phần tử
                    scrollTop = $("#search-timkiemhoso-ketquatimkiem").offset().top - (windowHeight / 2) + (elementHeight);

                $("html, #main").animate({
                    scrollTop: scrollTop
                }, 1000, "swing");
                // Gán sự kiện
                $(".card-ketqua-content").off().on("click", function () { // Chọn từng thẻ
                    let that = $(this).siblings(".card-ketqua-checkbox");
                    let checked = $(".checkRow-ketquatimkiem-getList", that).is(":checked");
                    $(".checkRow-ketquatimkiem-getList", that).trigger("click");
                });
                multipleCheck({  // Chọn nhiều thẻ
                    name: "ketquatimkiem-getList",
                    parent: $("#ketquatimkiem-getList")
                });
                if (loaiTimKiem == "nangcao") {
                    sys.displayModal({
                        name: '#timkiemhoso-nangcao',
                        displayStatus: "hide"
                    });
                };
            }
        });
    }
    chonLoaiTimKiem() {
        var s = this;
        huongDanSuDung.lanDauDangNhap("timkiem-nangcao");
        sys.displayModal({
            name: '#timkiemhoso-nangcao',
        });
    }
};