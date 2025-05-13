'use strict'
/**
 * main
 * */
class QuanLyLopHoc {
    constructor() {
        this.page;
        this.pageGroup;
        this.nguoiDung = {
            idNguoiDung_DangSuDung: $("#input-idnguoidung-dangsudung").val(),
            maChucVu_DangSuDung: $("#input-machucvu-dangsudung").val()
        }
        this.lopHoc = {
            data: [],
            dataTable: null,
            save: function () { },
            delete: function () { },
            displayModal_CRUD: function () { },
        }
        this.lichHoc = {
            anhBuoiHocViewerJS: null,
        }
    }
    init() {
        var quanLyLopHoc = this;
        quanLyLopHoc.page = $("#page-quanlylophoc");
        sys.activePage({
            page: quanLyLopHoc.page.attr("id"),
            pageGroup: quanLyLopHoc.pageGroup
        });
    }
    funcs_GiaoDienQuanLy() {
        var quanLyLopHoc = this;
        quanLyLopHoc.khachHang = {
            ...quanLyLopHoc.khachHang,
            displayModal_KhachHang_XemChiTiet: function (idKhachHang = '00000000-0000-0000-0000-000000000000') {
                if (idKhachHang == '00000000-0000-0000-0000-000000000000') {
                    var idKhachHangs = [];
                    quanLyLopHoc.donHang.dataTable.rows().iterator('row', function (context, index) {
                        var $row = $(this.row(index).node());
                        if ($row.has("input.checkRow-quanlykhachhang-getList:checked").length > 0) {
                            idKhachHangs.push($row.attr('id'));
                        };
                    });
                    if (idKhachHangs.length != 1) {
                        sys.alert({ mess: "Yêu cầu chọn 1 bản ghi", status: "warning", timeout: 1500 });
                        return;
                    }
                    else idKhachHang = idKhachHangs[0];
                };
                $.ajax({
                    ...ajaxDefaultProps({
                        url: "/QuanLyLopHoc/displayModal_KhachHang_XemChiTiet",
                        type: "POST",
                        data: { idKhachHang }
                    }),
                    success: function (res) {
                        $("#quanlylophoc-khachhang-xemchitiet").html(res);
                        sys.displayModal({
                            name: '#quanlylophoc-khachhang-xemchitiet',
                            level: 4
                        });
                        // Cập nhật giá trị thẻ sản phẩm
                        $(".select-sanpham").trigger("change");
                    }
                })
            },
            chonSanPham: function (e) {
                var $tbDonHang = $(e).closest("table.table-donhang"),
                    $donHang = $("tr.donhang", $tbDonHang),
                    $inputTongSoTien = $(".input-tongsotien", $donHang),
                    giaTien = $("option:selected", $(e)).data("giatien");
                $inputTongSoTien.val(giaTien); // Cập nhật giá tiền
                quanLyLopHoc.khachHang.capNhatSoTienDaDong($tbDonHang); // Cập nhật số tiền đã đống cho đơn hàng đó
            },
            capNhatSoTienDaDong: function (e) {
                var $tbDonHang = $(e), // Lấy đơn hàng
                    $donHang = $("tr.donhang", $tbDonHang),
                    $thanhToans = $("tr.thanhtoan", $tbDonHang);
                var tongSoTien_YeuCau = $(".input-tongsotien", $donHang).val() ?? 1,
                    tongSoTien_DaDong = 0;

                // Cập nhật số tiền đóng cho từng lần thanh toán
                $.each($thanhToans, function (iThanhToan, $thanhToan) {
                    let soTienDaDong = $(".input-sotiendadong", $thanhToan).val() ?? 0,
                        giaTriQuyDoi = $(".select-donvitien", $thanhToan).val() ?? 1;
                    // Tính số tiền sau quy đổi và hiển thị
                    let soTienDaDong_SauQuyDoi = parseInt(soTienDaDong) * parseInt(giaTriQuyDoi);
                    $(".input-sotiendadong-sauquydoi", $thanhToan).val(soTienDaDong_SauQuyDoi);
                    // Tính % đã đóng của lần thanh toán này
                    var phanTramDaDong = parseFloat((soTienDaDong_SauQuyDoi / tongSoTien_YeuCau) * 100).toFixed(2);
                    $(".input-phantramdadong", $thanhToan).val(`${phanTramDaDong}%`);

                    tongSoTien_DaDong += soTienDaDong_SauQuyDoi;
                });
                // Tính tổng % đã đóng
                var tongPhanTramDaDong = parseFloat((tongSoTien_DaDong / tongSoTien_YeuCau) * 100).toFixed(2);
                $(".input-phantramtong", $donHang).val(`${tongPhanTramDaDong}%`);
            },
        }
        quanLyLopHoc.donHang = {
            ...quanLyLopHoc.donHang,
            dataTable: new DataTableCustom({
                name: "choxeplop-getList",
                table: $("#choxeplop-getList"),
                props: {
                    lengthMenu: [
                        [2, 5],
                        [2, 5],
                    ],
                    ajax: {
                        url: '/QuanLyLopHoc/getList',
                        type: "GET",
                        data: function (arg) {
                            arg.loaiDanhSach = "choxeplop";
                        }
                    },
                    rowId: 'DonHang.IdDonHang',
                    columns: [
                        {
                            data: null,
                            className: "text-center",
                            searchable: false,
                            orderable: false,
                            render: function (data, type, row, meta) {
                                return `<input class="form-check-input checkRow-choxeplop-getList" type="checkbox"/>`;
                            }
                        },
                        {
                            data: "ThongTinNguoiTao.TenNguoiDung",
                            className: "text-center",
                        },
                        {
                            data: null,
                            className: "text-left",
                            render: function (data, type, row, meta) {
                                return `
                                <table class="table table-bordered table-responsive w-100">
                                    <tbody>
                                        <tr>
                                            <td class="">Khóa học</td>
                                            <td class="">
                                                <span class="font-bold text-primary">${data.SanPham.TenSanPham}</span> <br />
                                                <small class="fst-italic">(${data.SanPham.ThoiGianBuoiHoc} phút/buổi - tổng ${data.SanPham.SoBuoi} buổi)</small>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="">Khách hàng</td>
                                            <td class="">
                                                <span class="font-bold text-danger">${data.KhachHang.TenKhachHang}</span> <br />
                                                <small class="fst-italic">(đăng ký lần ${data.DonHang.ThuTuDonHang})</small>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="">Trình độ đầu vào</td>
                                            <td class="">${data.TrinhDoDauVao.TenTrinhDo}</td>
                                        </tr>
                                        <tr>
                                            <td class="">Trình độ đầu ra</td>
                                            <td class="">${data.TrinhDoDauRa.TenTrinhDo}</td>
                                        </tr>
                                        <tr>
                                            <td class="">Ngày đóng học phí</td>
                                            <td class="">${moment(data.DonHang.NgayTao).format('DD-MM-YYYY')}</td>
                                        </tr>
                                    </tbody>
                                </table>
                                `;
                            }
                        },
                        {
                            data: "DonHang.GhiChu",
                            className: "text-left",
                            render: function (data, type, row, meta) {
                                //return `<span title="${data}">${sys.truncateString(data, 60)}</span>`;
                                return data.replaceAll("\n", "<br />");
                            }
                        },
                        {
                            data: null,
                            className: "text-center",
                            searchable: false,
                            orderable: false,
                            render: function (data, type, row, meta) {
                                return `
                                    <div style="white-space: nowrap">
                                        <a class="btn btn-sm btn-light-secondary" title="Xem chi tiết" onclick="quanLyLopHoc.khachHang.displayModal_KhachHang_XemChiTiet('${data.KhachHang.IdKhachHang}')"><i class="bi bi-eye-fill"></i></a>
                                    </div>`;
                            }
                        }
                    ],
                }
            }).dataTable,
            dataTable_CRUD: null,
            chonDonHang: function () {
                var tenKhachHangs = [],
                    idDonHangs = [],
                    idKhachHangs = [];
                var soBuois = [];
                quanLyLopHoc.donHang.dataTable_CRUD.rows().iterator('row', function (context, index) {
                    var $row = $(this.row(index).node());
                    if ($row.has("input.checkRow-choxeplop-getList:checked").length > 0) {
                        let $donHang = $row,
                            idDonHang = $donHang.data('iddonhang'),
                            idKhachHang = $donHang.data('idkhachhang'),
                            tenKhachHang = $donHang.data("tenkhachhang"),
                            soBuoi = parseInt($donHang.data("sobuoi"));

                        idDonHangs.push(idDonHang);
                        idKhachHangs.push(idKhachHang);
                        tenKhachHangs.push(tenKhachHang);
                        soBuois.push(soBuoi);
                    };
                });
                idDonHangs = [...new Set(idDonHangs)];
                tenKhachHangs = [...new Set(tenKhachHangs)];
                idKhachHangs = [...new Set(idKhachHangs)];
                var tenKhachHang_DeXuat = tenKhachHangs.join(", "),
                    idDonHang_DeXuat = idDonHangs.join(","),
                    idKhachHang_DeXuat = idKhachHangs.join(",");
                var soBuoi_DeXuat = soBuois.reduce((p, c) => p + c, 0);
                $("#input-tenkhachhang", $("#quanlylophoc-lichhoc-crud")).text(tenKhachHang_DeXuat);
                $("#input-tenkhachhang", $("#quanlylophoc-lichhoc-crud")).data("iddonhang", idDonHang_DeXuat);
                $("#input-tenkhachhang", $("#quanlylophoc-lichhoc-crud")).data("idkhachhang", idKhachHang_DeXuat);
                $("#input-tenkhachhang", $("#quanlylophoc-lichhoc-crud")).trigger("change");

                $("#input-sobuoi-dexuat", $("#quanlylophoc-lichhoc-crud")).text(soBuoi_DeXuat);
                $("#input-sobuoi-dexuat", $("#quanlylophoc-lichhoc-crud")).text(soBuoi_DeXuat);
            },
        };
        quanLyLopHoc.lichHoc = {
            ...quanLyLopHoc.lichHoc,
            //events: [],
            calendar: null,
            displayModal_CRUD_BuoiHoc: function (loai, id) {
                let buoiHoc;
                //if (loai == "diemdanh") {
                //    buoiHoc = {
                //        BuoiHoc: {
                //            IdLopHoc_BuoiHoc: id
                //        }
                //    }
                //} else {
                let event = quanLyLopHoc.lichHoc.layDuLieu_LichHoc(id)[0];
                buoiHoc = {
                    BuoiHoc: {
                        ...event.extendedProps,
                    },
                    //GiaoViens: [{
                    //    IdNguoiDung: 'af565a26-e775-433f-a782-d3ab236c0e0c',
                    //    TenNguoiDung: "Lê Minh Tùng"
                    //}]
                };
                //}
                var f = new FormData();
                f.append("str_buoiHoc", JSON.stringify(buoiHoc));
                f.append("loai", loai);
                $.ajax({
                    ...ajaxDefaultProps({
                        url: "/QuanLyLopHoc/displayModal_CRUD_BuoiHoc",
                        type: "POST",
                        data: f,
                        //data: JSON.stringify(buoiHoc),
                    }),
                    //contentType: "application/json; charset=utf-8",  // Chỉ định kiểu nội dung là JSON
                    //dataType: "json",
                    contentType: false,
                    processData: false,
                    success: function (res) {
                        $("#quanlylophoc-buoihoc-crud").html(res);

                        // Gán sự kiện cập nhật ảnh
                        $("#btn-themanhbuoihoc", $("#quanlylophoc-buoihoc-crud")).off().on("change", function () {
                            quanLyLopHoc.lichHoc.themAnhBuoiHoc(id);
                        });
                        // Gán sự kiện lưu
                        $("#btn-capnhatbuoihoc", $("#quanlylophoc-buoihoc-crud")).off().on("click", function () {
                            quanLyLopHoc.lichHoc.save(loai, id);
                        });
                        quanLyLopHoc.lichHoc.taoAnhBuoiHocViewerJS();

                        sys.displayModal({
                            name: '#quanlylophoc-buoihoc-crud',
                            level: 3
                        });
                    }
                });
            },
            displayModal_CRUD_LichHoc: function (loai = "", loaiCapNhat = 'capnhat-lichhoc', idLopHoc = '00000000-0000-0000-0000-000000000000') {
                $.ajax({
                    ...ajaxDefaultProps({
                        url: "/QuanLyLopHoc/displayModal_CRUD_LichHoc",
                        type: "POST",
                        data: { loai, loaiCapNhat, idLopHoc }
                    }),
                    success: function (res) {

                        $("#quanlylophoc-lichhoc-crud").html(res);

                        var ngayHienTai = new Date();

                        var $table_LichHoc = $("#table-lichhoc", $("#quanlylophoc-lichhoc-crud")),
                            $tbody_LichHoc = $("tbody.thongtin-buoihoc", $table_LichHoc);

                        $.each($tbody_LichHoc, function (iBuoiHoc, $buoiHoc) {
                            $(".input-thoigianbatdau", $buoiHoc).attr("min", ngayHienTai.toISOString().split('.')[0]);
                        });
                        $("#btn-capnhatlichhoc", $("#quanlylophoc-lichhoc-crud")).off().on("click", function () {
                            quanLyLopHoc.lichHoc.capNhat_LichHoc(loai, loaiCapNhat);
                        });
                        $("#input-tenkhachhang", $("#quanlylophoc-lichhoc-crud")).off().on("click", function () {
                            if ($(this).val() == "") { // Nếu rỗng thì kéo xuống danh sách cần xếp lớp
                                quanLyLopHoc.lichHoc.cuonToi_DanhSachCanXepLop();
                            };
                        });
                        // Tạo dataTable cho đơn hàng
                        quanLyLopHoc.donHang.dataTable_CRUD = new DataTableCustom({
                            name: "choxeplop-getList",
                            table: $("#choxeplop-getList", $("#quanlylophoc-lichhoc-crud")),
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

                        sys.displayModal({
                            name: "#quanlylophoc-lichhoc-crud",
                            level: 2
                        });
                    }
                })
            },
            cuonToi_DanhSachCanXepLop: function () {
                // Cuộn tới mục calender
                var modalContent = $("#quanlylophoc-crud").find('.modal-content');
                var targetElement = $("#calendar-lichhoc", $("#quanlylophoc-crud"));

                // Tính toán vị trí cần cuộn tới dựa trên khoảng cách của phần tử tới modal-content
                var scrollPosition = targetElement.offset().top - modalContent.offset().top + modalContent.scrollTop();

                // Cuộn modal-content tới vị trí tính toán
                modalContent.animate({
                    scrollTop: scrollPosition
                }, 500); // 500ms để cuộn mượt
            },
            capNhat_LichHoc: function (loai, loaiCapNhat = "xemchitiet") {
                var modalValidtion = htmlEl.activeValidationStates("#quanlylophoc-lichhoc-crud");
                if (modalValidtion) {
                    quanLyLopHoc.lichHoc.refeshLichHocCalendar(loai, loaiCapNhat);
                    quanLyLopHoc.lichHoc.cuonToi_DanhSachCanXepLop();
                    sys.displayModal({
                        name: "#quanlylophoc-lichhoc-crud",
                        displayStatus: "hide",
                        level: 2
                    });
                };
            },
            refeshLichHocCalendar: function (loai = "", loaiCapNhat = "xemchitiet", buoiHocs = []) {
                var TongSoBuoi = buoiHocs.length;
                function taoLichHoc_TuDong({ buoiHocs = [], tongSoBuoi = 0 }) {
                    var thuTuTuan = 0; // Đếm số tuần đã thêm
                    var thuTuBuoiHoc = 0;
                    var buoiHocs_NEW = [];
                    while ((buoiHocs_NEW.length < tongSoBuoi) && buoiHocs.length > 0) {
                        buoiHocs.forEach((buoiHoc, iBuoiHoc) => {
                            thuTuBuoiHoc++; // Tăng thứ tự lên
                            // Thêm số tuần
                            let thoiGianBatDau = new Date(buoiHoc.ThoiGianBatDau);
                            thoiGianBatDau.setDate(thoiGianBatDau.getDate() + (7 * thuTuTuan));
                            thoiGianBatDau = sys.chuyenThoiGianDungLocalVN(thoiGianBatDau);
                            let thoiGianKetThuc = new Date(thoiGianBatDau.getTime() + buoiHoc.ThoiLuong * 60000); // ThoiLuong tính bằng phút

                            let buoiHoc_NEW = {
                                ...buoiHoc,
                                ThuTuBuoiHoc: thuTuBuoiHoc,
                                ThoiGianBatDau: thoiGianBatDau.toISOString(), // ISO string theo giờ địa phương
                                ThoiGianKetThuc: thoiGianKetThuc.toISOString(), // ISO string theo giờ địa phương
                            };
                            buoiHocs_NEW.length < tongSoBuoi && buoiHocs_NEW.push(buoiHoc_NEW);
                        });

                        thuTuTuan++;
                    };
                    //return buoiHocs_NEW.slice(0, tongSoBuoi); // Chỉ lấy đúng tổng số buổi
                    return buoiHocs_NEW; // Chỉ lấy đúng tổng số buổi
                };

                function layBuoiHocs() {
                    var $table_LichHoc = $("#table-lichhoc", $("#quanlylophoc-lichhoc-crud")),
                        $tbody_LichHoc = $("tbody.thongtin-buoihoc", $table_LichHoc);
                    var idGiaoViens = $("#select-giaovien", $("#quanlylophoc-lichhoc-crud")).val().join(","),
                        idKhachHangs = $("#input-tenkhachhang", $("#quanlylophoc-lichhoc-crud")).data("idkhachhang"),
                        idDonHangs = $("#input-tenkhachhang", $("#quanlylophoc-lichhoc-crud")).data("iddonhang"),
                        luongTheoBuoi = $("#input-luongtheobuoi", $("#quanlylophoc-lichhoc-crud")).val().replaceAll(' ', '');
                    // Dữ liệu ban đầu
                    $.each($tbody_LichHoc, function (iBuoiHoc, $buoiHoc) {
                        let buoiHoc = {
                            IdLopHoc_BuoiHoc: '00000000-0000-0000-0000-000000000000',
                            IdLopHoc: '00000000-0000-0000-0000-000000000000',
                            LuongTheoBuoi: luongTheoBuoi,
                            IdGiaoVien: `,${idGiaoViens}`,
                            IdKhachHang: `,${idKhachHangs}`,
                            IdDonHang: `,${idDonHangs}`,
                            ThuTuBuoiHoc: 0, // Sẽ thay đổi khi tạo events

                            ThoiGianBatDau: $(".input-thoigianbatdau", $buoiHoc).val() ?? "",
                            ThoiLuong: $(".input-thoiluong", $buoiHoc).val() ?? "",
                            DiemDanh: 0,
                            GhiChu: "",
                            HinhAnhBuoiHocs: []
                        };
                        if (buoiHoc.ThoiGianBatDau != "" && buoiHoc.ThoiLuong != "") buoiHocs.push(buoiHoc);
                    });

                    if (buoiHocs.length > 0) {
                        // Sắp xếp các sự kiện theo thời gian bắt đầu (ThoiGianBatDau)
                        buoiHocs.sort((a, b) => new Date(a.ThoiGianBatDau) - new Date(b.ThoiGianBatDau));
                        TongSoBuoi = buoiHocs.length;

                        // Lựa chọn chế độ cập nhật lịch học
                        if (quanLyLopHoc.lichHoc.kiemTra_CheDoCapNhatLichHoc()) {
                            let tongSoBuoi = $("#input-sobuoi", $("#quanlylophoc-lichhoc-crud")).val();
                            if (tongSoBuoi == "") {
                                sys.alert({ status: "warning", mess: "Nhập tổng số buổi" });
                                return;
                            };
                            TongSoBuoi = parseInt(tongSoBuoi);
                        };

                        // Tạo tự động cho đến khi đủ buổi
                        buoiHocs = taoLichHoc_TuDong({ buoiHocs, tongSoBuoi: TongSoBuoi })
                    };
                    return buoiHocs;
                };

                function taoEvents(buoiHocs = []) {
                    // Thêm các sự kiện ban đầu vào danh sách sự kiện
                    let events = buoiHocs.map((event, iEvent) => ({
                        id: parseInt(event.ThuTuBuoiHoc),
                        title: `B${event.ThuTuBuoiHoc}`,
                        start: event.ThoiGianBatDau,
                        end: event.ThoiGianKetThuc,
                        allDay: false,
                        ...event // Chỗ này sẽ rơi vào extendedProps
                        //backgroundColor: '#FF5733', // Màu nền
                        //borderColor: '#C70039',     // Màu viền
                        //textColor: '#FFFFFF',       // Màu chữ
                    }))
                    return events;
                };

                function taoLichHoc(events = []) {
                    //quanLyLopHoc.lichHoc.events = events;
                    quanLyLopHoc.lichHoc.calendar = new CalendarCustom({
                        name: $("#calendar-lichhoc", $("#quanlylophoc-crud")),
                        props: {
                            contentHeight: 400,
                            initialView: 'multiMonth',
                            views: {
                                multiMonth: {
                                    type: 'multiMonth',
                                    duration: { months: 2 }
                                }
                            },
                            events: events,
                            eventDrop: function (info) {
                                let ngayHienTai = new Date();
                                let ngayCu = new Date(info.event.extendedProps.ThoiGianBatDau);
                                let ngayMoi = new Date(info.event.start);

                                let mess = `Buổi học "${info.event.title}" đã được di chuyển đến ${info.event.start.toISOString().split('T')[0]}`;

                                if (loai == "diemdanh") {
                                    mess = "Bạn không có quyền chỉnh sửa ngày học";
                                    info.revert(); // Quay lại vị trí cũ
                                } else {
                                    // Kiểm tra nếu sự kiện bị kéo đến quá khứ
                                    if (ngayMoi < ngayHienTai) {
                                        mess = "Không thể di chuyển sự kiện đến quá khứ";
                                        info.revert(); // Quay lại vị trí cũ
                                    } else if (info.event.extendedProps.ThoiGianBatDau) {
                                        // Kiểm tra nếu ngày cũ thuộc tháng trước
                                        let thangHienTai = ngayHienTai.getMonth();
                                        let thangNgayCu = ngayCu.getMonth();
                                        let namHienTai = ngayHienTai.getFullYear();
                                        let namNgayCu = ngayCu.getFullYear();

                                        if (
                                            (namNgayCu < namHienTai) ||
                                            (namNgayCu === namHienTai && thangNgayCu < thangHienTai) ||
                                            ((ngayHienTai - ngayCu) / (1000 * 60 * 60 * 24) <= 10)
                                        ) {
                                            mess = "Không thể di chuyển sự kiện vì ngày cũ thuộc tháng trước hoặc ít hơn 10 ngày so với hiện tại";
                                            info.revert(); // Quay lại vị trí cũ
                                        }
                                    }
                                };

                                sys.alert({ mess: mess, status: "warning", timeout: 1500 });
                            },

                            eventResize: function (info) {
                                // Kiểm tra nếu sự kiện kéo dài đến quá khứ
                                let mess = `Buổi học "${info.event.title}" đã được di chuyển đến ${info.event.start.toISOString().split('T')[0]}`;
                                if (loai == "diemdanh") {
                                    mess = "Bạn không có quyền chỉnh sửa ngày học";
                                    info.revert(); // Quay lại vị trí cũ
                                } else {
                                    // Kiểm tra nếu sự kiện bị kéo đến quá khứ
                                    if (info.event.start < new Date()) {
                                        mess = "Không thể di chuyển sự kiện đến quá khứ";
                                        info.revert(); // Quay lại vị trí cũ
                                    };
                                }
                                sys.alert({ mess: mess, status: "warning", timeout: 1500 });
                            },

                            eventAllow: function (dropInfo, draggedEvent) {
                                var today = new Date();
                                today.setHours(0, 0, 0, 0); // Đặt giờ về 0 để so sánh chính xác

                                // Không cho phép kéo sự kiện từ quá khứ
                                return dropInfo.start >= today;
                            },
                            eventClick: function (info) {
                                //quanLyLopHoc.lichHoc.displayModal_CRUD_BuoiHoc(loai, info);
                            },
                            eventContent: function (info) {
                                // Chỉ hiển thị tiêu đề của sự kiện
                                let buoiHoc = info.event.extendedProps;
                                let cusTomeEl = {
                                    class: "",
                                    hoverTitle: "",
                                };
                                if (buoiHoc.DiemDanh == 1) cusTomeEl = {
                                    class: `success`, hoverTitle: "Đã điểm danh"
                                };
                                else if (buoiHoc.DiemDanh == 0) cusTomeEl = {
                                    class: `danger`, hoverTitle: "Chưa điểm danh"
                                };
                                else cusTomeEl = {
                                    class: `warning`, hoverTitle: "Khác"
                                };
                                let eventId = parseInt(info.event.id);
                                return {
                                    _html: `<div class="w-100 font-bold badge bg-${cusTomeEl.class}" title="${cusTomeEl.hoverTitle}">${info.event.title}</div>`,
                                    html: `
                                        <div class="dropdown w-100">
                                            <button type="button" class="w-100 btn btn-sm btn-${cusTomeEl.class} dropdown-toggle" title="${cusTomeEl.hoverTitle}" dropdown-toggle" data-bs-toggle="dropdown">
                                                ${info.event.title}
                                            </button>
                                             <div class="dropdown-menu" style="border: var(--bs-modal-footer-border-width) solid var(--bs-modal-footer-border-color);">
                                                <a class="dropdown-item c-pointer xem-chi-tiet" data-event-id="${eventId}" onclick="quanLyLopHoc.lichHoc.displayModal_CRUD_BuoiHoc('${loai}', ${eventId})">Cập nhật</a>
                                                <a class="dropdown-item c-pointer xoa-bo" data-event-id="${eventId}" onclick="quanLyLopHoc.lichHoc.displayModal_CRUD_BuoiHoc('${loai}', ${eventId})">Xóa bỏ</a>
                                            </div>
                                        </div>
                                    `
                                };
                            },
                            //validRange: function (nowDate) {
                            //    return {
                            //        start: nowDate,
                            //    };
                            //},
                        }
                    }).init();
                    quanLyLopHoc.lichHoc.calendar.render();
                };

                function themBuoiHoc(buoiHocs = []) { // Có thể sử dụng để reload lịch khi di chuyển, xóa ...
                    // Lấy danh sách buổi học cũ để sắp xếp lại
                    var events_OLD = quanLyLopHoc.lichHoc.layDuLieu_LichHoc();
                    var buoiHocs_OLD = events_OLD.map(event => ({
                        ...event.extendedProps
                    }));
                    var buoiHocs_NEW = [...buoiHocs_OLD, ...buoiHocs];
                    buoiHocs_NEW = buoiHocs_NEW
                        .sort((a, b) => new Date(a.ThoiGianBatDau) - new Date(b.ThoiGianBatDau))
                        .map((buoiHoc, iBuoiHoc) => ({
                            ...buoiHoc,
                            ThuTuBuoiHoc: iBuoiHoc + 1,
                        }));
                    return buoiHocs_NEW;
                };

                function xoaBuoiHoc() {

                }

                let events_NEW = [];
                if (loaiCapNhat == "taomoi-lichhoc") {
                    let buoiHocs = layBuoiHocs();
                    events_NEW = taoEvents(buoiHocs);
                } else if (loaiCapNhat == "bosung-buoihoc") {
                    // Lấy buổi học cũ
                    // Lấy buổi học mới + lần lặp
                    // Tạo lịch
                    // Không bổ sung vào quá khứ, xếp lại thứ tự buổi
                    let buoiHocs = layBuoiHocs();
                    let buoiHocs_NEW = themBuoiHoc(buoiHocs);
                    events_NEW = taoEvents(buoiHocs_NEW);
                } else {
                    buoiHocs = buoiHocs.map(buoiHoc => {
                        // Chuỗi có thể là "/Date(1728474380000)/" hoặc đã là ISO
                        //var dateString = "/Date(1728474380000)/";

                        // Kiểm tra nếu chuỗi bắt đầu bằng "/Date("
                        if (buoiHoc.ThoiGianBatDau.startsWith("/Date(")) {
                            // Trích xuất số mili giây từ chuỗi
                            var timestamp = parseInt(buoiHoc.ThoiGianBatDau.match(/\d+/)[0], 10);

                            // Chuyển số mili giây thành đối tượng Date
                            let date = new Date(timestamp);
                            date = sys.chuyenThoiGianDungLocalVN(date); // Chuyển đúng thời gian của local

                            // Chuyển đổi sang định dạng ISO
                            buoiHoc.ThoiGianBatDau = date.toISOString();
                        };
                        return buoiHoc;
                    });
                    events_NEW = taoEvents(buoiHocs);
                };
                taoLichHoc(events_NEW);
            },
            capNhat_EventsBuoiHoc: function ({ info, buoiHoc = {} }) {
                function updateExtendedProps(obj, updates) {
                    Object.keys(updates).forEach(key => {
                        obj.setExtendedProp(key, updates[key]);
                    });
                }

                // Sử dụng hàm để cập nhật nhiều trường
                //updateExtendedProps(info.event, { ...buoiHoc });
                updateExtendedProps(info, { ...buoiHoc });
            },
            layDuLieu_LichHoc: function (eventId = 0) {
                // Tạo lại cấu trúc dữ liệu như ban đầu
                let originalFormatEvents = quanLyLopHoc.lichHoc.calendar.getEvents().map((event, i) => ({
                    id: event.id,
                    title: event.title,
                    start: event.start.toISOString(), // Chuyển về định dạng ISO string
                    end: event.end ? event.end.toISOString() : null,
                    extendedProps: {
                        ...event.extendedProps,
                        ThoiGianBatDau: event.start.toISOString(), // Chuyển về định dạng ISO string
                    },
                }))
                if (eventId != 0) originalFormatEvents = originalFormatEvents.filter(x => x.id == eventId);
                return originalFormatEvents;
            },
            them_BuoiHoc: function (e) {
                var $table_LichHoc = $(e).closest("table#table-lichhoc"),
                    $tbody_LichHoc = $("tbody.thongtin-buoihoc", $table_LichHoc),
                    $banMau = $($tbody_LichHoc[0]);
                // Thêm 1 buổi
                $("thead", $table_LichHoc).after(`<tbody class="thongtin-buoihoc">${$banMau.html()}<tbody>`);
            },
            xoa_BuoiHoc: function (e) {
                var $table_LichHoc = $(e).closest("table#table-lichhoc"),
                    $tbody_LichHoc = $("tbody.thongtin-buoihoc", $table_LichHoc),
                    $_this = $(e).closest("tbody.thongtin-buoihoc");
                if ($tbody_LichHoc.length > 1) {
                    $_this.remove();
                }
            },
            kiemTra_CheDoCapNhatLichHoc: function () {
                var trangThai = $("#checkbox-chedocapnhatlichhoc", $("#quanlylophoc-lichhoc-crud")).is(":checked");
                return trangThai;
            },
            chonCheDoCapNhatLichHoc: function () {
                $("#input-sobuoi-container", $("#quanlylophoc-lichhoc-crud")).hide();

                if (quanLyLopHoc.lichHoc.kiemTra_CheDoCapNhatLichHoc()) {
                    $("#input-sobuoi-container", $("#quanlylophoc-lichhoc-crud")).show();
                };
            },
            themAnhBuoiHoc: function (eventId) {
                let event = quanLyLopHoc.lichHoc.layDuLieu_LichHoc(eventId)[0];

                var soAnhToiDa = 1;
                var $inputFile = $("#btn-themanhbuoihoc", $("#quanlylophoc-buoihoc-crud")).get(0),
                    kiemTra = true,
                    status = "success",
                    mess = "Thêm ảnh thành công",
                    maxMb = 1,
                    maxSizeInBytes = 1024 * 1024 * maxMb,
                    real_maxSizeInBytes = 0;
                var hinhAnhBuoiHocs = [];

                // Dùng Promises để đợi FileReader hoàn thành
                $.each($inputFile.files, function (idx, f) {
                    var extension = f.type;
                    real_maxSizeInBytes += f.size;
                    if (!(/\.(png|jpg|jpeg)$/i.test(f.name))) {
                        kiemTra = false;
                        status = "error";
                        mess = `Tồn tại tệp không thuộc định dạng cho phép [png|jpg|jpeg]`;
                    };
                    if (f.size > maxSizeInBytes) {
                        kiemTra = false;
                        status = "error";
                        mess = `Tồn tại tệp có kích thước tệp vượt quá giới hạn ${maxMb}Mb`;
                    };
                    if (f.name.length > 80) {
                        kiemTra = false;
                        status = "error";
                        mess = `Tồn tại tệp có tên vượt quá giới hạn 80 ký tự`;
                    };

                    if (!kiemTra || real_maxSizeInBytes >= maxSizeInBytes) {
                        sys.alert({ status, mess, timeout: 5000 });
                        return false;
                    } else {
                        // Đọc file và đợi kết quả Base64
                        sys.readFileAsBase64(f, "all").then(function (base64Data) {
                            hinhAnhBuoiHocs.push({
                                IdHinhAnh: '00000000-0000-0000-0000-000000000000',
                                TenHinhAnh: f.name,
                                DuongDanHinhAnh: URL.createObjectURL(f), // Chỉ sử dụng cho hiển thị tạm thời
                                FileBase64: base64Data // Gán Base64 vào thuộc tính File
                            });

                            // Kiểm tra nếu là file cuối cùng đã được xử lý
                            //if (idx === $inputFile.files.length - 1) {
                            //}
                            // Xử lý tiếp sau khi tất cả các file đã được đọc
                            let $tbodyAnhBuoiHoc = $("#tbody-anhbuoihoc", $("#quanlylophoc-buoihoc-crud"));
                            let soAnhDangCo = $("tr", $tbodyAnhBuoiHoc).length;
                            $.each(hinhAnhBuoiHocs, function (ihinhAnhBuoiHoc, hinhAnhBuoiHoc) {
                                // Xóa ảnh cũ trùng tên
                                let anhDaTonTai = $(`tr[data-tenhinhanh='${hinhAnhBuoiHoc.TenHinhAnh}']`, $tbodyAnhBuoiHoc);
                                if (anhDaTonTai.length != 0) {
                                    soAnhDangCo = soAnhDangCo - 1;
                                    anhDaTonTai.remove();
                                };

                                // Kiểm tra lượng ảnh
                                soAnhDangCo = soAnhDangCo + 1;
                                if (soAnhDangCo <= soAnhToiDa) {
                                    // Thêm ảnh mới
                                    let html = `
                                            <tr data-idhinhanh="${hinhAnhBuoiHoc.IdHinhAnh}"
                                                data-duongdananh="${hinhAnhBuoiHoc.DuongDanHinhAnh}"
                                                data-tenhinhanh="${hinhAnhBuoiHoc.TenHinhAnh}">
                                                <td class="text-start">
                                                    📸 <a class="image-link c-pointer"
                                                       data-bs-toggle="tooltip" data-bs-placement="right" title="${hinhAnhBuoiHoc.TenHinhAnh}"
                                                       data-original="${hinhAnhBuoiHoc.DuongDanHinhAnh}">
                                                        ${sys.truncateString(hinhAnhBuoiHoc.TenHinhAnh, 30)}</a >
                                                    <img src="${hinhAnhBuoiHoc.DuongDanHinhAnh}" alt="Picture ${ihinhAnhBuoiHoc}" hidden/>
                                                </td>
                                                <td class='text-center'>
                                                    <a class="btn btn-light-secondary c-pointer"
                                                       onclick="quanLyLopHoc.lichHoc.xoaAnhBuoiHoc(this)">
                                                        <i class="bi bi-trash3-fill"></i>
                                                    </a>
                                                </td>
                                            </tr>`;
                                    $tbodyAnhBuoiHoc.prepend(html); // Thêm danh sách ảnh
                                    // Lưu thông tin ảnh vào buổi học
                                    quanLyLopHoc.lichHoc.capNhat_EventsBuoiHoc({
                                        event, buoiHoc: {
                                            HinhAnhBuoiHocs: [hinhAnhBuoiHoc]
                                        }
                                    });
                                } else {
                                    status = "warning";
                                    mess = `Chỉ nhận tối đa ${soAnhToiDa} ảnh`;
                                };
                            });
                        }).then(function () {
                            quanLyLopHoc.lichHoc.taoAnhBuoiHocViewerJS();
                            sys.alert({ status, mess });
                        }).catch(function (error) {
                            console.log("Lỗi đọc tệp:", error);
                            kiemTra = false;
                            status = "error";
                            mess = `Lỗi khi đọc tệp`;
                        });
                    };
                });
                // Xóa bộ nhớ đệm để upload file trong lần tiếp theo
                $inputFile.value = '';
            },
            taoAnhBuoiHocViewerJS: function () {
                // Destroy viewer cũ rồi mới gán lại được để không bị trùng
                if (quanLyLopHoc.lichHoc.anhBuoiHocViewerJS != null) quanLyLopHoc.lichHoc.anhBuoiHocViewerJS.destroy();
                quanLyLopHoc.lichHoc.anhBuoiHocViewerJS = new Viewer($("#tbody-anhbuoihoc")[0], {
                    backdrop: "static", // Ngăn việc đóng Viewer khi click vào backdrop
                    zIndex: 21050, // Đảm bảo rằng Viewer.js có zIndex cao hơn modal
                    show: function () {
                        // Lấy nội dung của Viewer.js body và thay thế vào modal
                        let viewerBody = $("#tbody-anhbuoihoc").closest("#table-anhbuoihoc-container").html(); // Thay thế bằng nội dung bạn cần
                        $("#modal-bait-viewer .modal-body").html(viewerBody);
                        $("#table-anhbuoihoc-container", $("#quanlylophoc-buoihoc-crud")).html("");

                        // Hiển thị modal chứa Viewer.js
                        sys.displayModal({
                            name: '#viewer-container-modal',
                            level: 19999,
                        });
                    },
                    hide: function () {
                        // Ẩn modal nhử cho viewer
                        sys.displayModal({
                            name: '#viewer-container-modal',
                            level: 19999,
                            displayStatus: "hide"
                        });
                        // Xóa nội dung trong modal để tránh xung đột
                        let $anhBuoiHocContainer = $("#modal-bait-viewer .modal-body");
                        $("#table-anhbuoihoc-container", $("#quanlylophoc-buoihoc-crud")).html($anhBuoiHocContainer.html());
                        $anhBuoiHocContainer.html("");
                        quanLyLopHoc.lichHoc.taoAnhBuoiHocViewerJS();
                    }
                    //url: 'data-original', // Sử dụng thuộc tính 'data-duongdananh' để lấy URL ảnh
                    //url(image) {
                    //    let src = image.src.replace('?size=160', '');
                    //    return src;
                    //},
                });
                $(".image-link", $("#quanlylophoc-buoihoc-crud")).off().on("click", function (e) {
                    e.preventDefault(); // Ngăn hành động mặc định của thẻ <a>
                    const $tr = $(this).closest('tr'); // Lấy thẻ <tr> tương ứng
                    quanLyLopHoc.lichHoc.anhBuoiHocViewerJS.view($tr.index()); // Hiển thị ảnh tương ứng trong Viewer.js
                });
            },
            xoaAnhBuoiHoc: function (e) {
                var $tr = $(e).closest("tr");
                $tr.remove(); // Xóa phần tử hiện tại
                quanLyLopHoc.lichHoc.anhBuoiHocViewerJS.update(); // Cập nhật viewer
            },
            save: function (loai, eventId) {
                let event = quanLyLopHoc.lichHoc.layDuLieu_LichHoc(eventId)[0];

                var choPhepChinhSua = true;
                var mess = "";
                //if (loai == "update" || loai == "diemdanh") {
                //    // Không cho sửa buổi trước ngày hôm nay
                //    if (info.event.start < new Date()) {
                //        mess = "Không thể cập nhật buổi học ở quá khứ";
                //        choPhepChinhSua = false;
                //    };
                //};

                if (!choPhepChinhSua) {
                    sys.alert({ mess: mess, status: "warning", timeout: 1500 });
                } else {
                    var buoiHoc = {
                        BuoiHoc: {
                            IdLopHoc_BuoiHoc: $("#input-idlophoc_buoihoc", $("#quanlylophoc-buoihoc-crud")).val(),
                            IdKhachHang: $("#select-khachhang-buoihoc", $("#quanlylophoc-buoihoc-crud")).val().join(","),
                            IdGiaoVien: $("#select-giaovien-buoihoc", $("#quanlylophoc-buoihoc-crud")).val().join(","),
                            ThuTuBuoiHoc: $("#input-thutubuoihoc", $("#quanlylophoc-buoihoc-crud")).val(),
                            GhiChu: $("#input-ghichu", $("#quanlylophoc-buoihoc-crud")).val().trim(),

                            DiemDanh: $("#select-diemdanh", $("#quanlylophoc-buoihoc-crud")).val(),
                        }
                        //HinhAnhBuoiHoc: []
                    };

                    buoiHoc.IdKhachHang = `,${buoiHoc.IdKhachHang}`;
                    buoiHoc.IdGiaoVien = `,${buoiHoc.IdGiaoVien}`;
                    // Cập nhật event
                    quanLyLopHoc.lichHoc.capNhat_EventsBuoiHoc({
                        event, buoiHoc
                    });

                    sys.alert({ mess: "Cập nhật thành công thông tin buổi học", status: "success", timeout: 1500 });
                    sys.displayModal({
                        name: '#quanlylophoc-buoihoc-crud',
                        level: 2,
                        displayStatus: "hide"
                    });
                }
            }
        }
        quanLyLopHoc.lopHoc = {
            ...quanLyLopHoc.lopHoc,
            taiLieu: {
                dataTable: null,

                create: function () {
                    var taiLieuDataTable = quanLyLopHoc.lopHoc.taiLieu.dataTable,
                        $tr = [
                            `<input type="text" class="form-control input-tentailieu" placeholder="Tên tài liệu" />`,
                            `<input type="text" class="form-control input-duongdantailieu" placeholder="Đường dẫn (Drive, Onedrive, ...)" />`,
                            `<a class="btn btn-light-secondary c-pointer" onclick="quanLyLopHoc.lopHoc.taiLieu.delete(this)"><i class="bi bi-trash3-fill"></i></a>`
                        ];
                    taiLieuDataTable.row.add($tr).draw(false);
                },
                delete: function (e) {
                    var taiLieuDataTable = quanLyLopHoc.lopHoc.taiLieu.dataTable,
                        $tr = $(e).closest("tr");
                    taiLieuDataTable.row($tr).remove().draw();
                }
            },
            donHang: {
                dataTable: null
            },
            giaoVien: {
                dataTable: null
            },
            fTaiLieus: [],
            dataTable: new DataTableCustom({
                name: "daxeplop-getList",
                table: $("#daxeplop-getList"),
                props: {
                    ajax: {
                        url: '/QuanLyLopHoc/getList',
                        type: "GET",
                        data: function (arg) {
                            arg.loaiDanhSach = "daxeplop";
                        }
                    },
                    rowId: 'LopHoc.IdLopHoc',
                    columns: [
                        {
                            data: null,
                            className: "text-center",
                            searchable: false,
                            orderable: false,
                            render: function (data, type, row, meta) {
                                return `<input class="form-check-input checkRow-daxeplop-getList" type="checkbox"/>`;
                            }
                        },
                        {
                            data: null,
                            className: "text-center",
                            render: function (data, type, row, meta) {
                                let tenGiaoViens = data.GiaoViens.map(gv => gv.TenNguoiDung).join(', ');
                                let tenKhachHangs = data.KhachHangs.map(gv => gv.TenKhachHang).join(', ');

                                /**
                                 * 0: Xóa, 
                                 * 1: Khởi tạo - Chưa bắt đầu, 
                                 * 2: Đang học, 
                                 * 3: Tạm dừng
                                 * 4: Kết thúc
                                 */
                                let trangThai = "";
                                if (data.LopHoc.TrangThaiLopHoc == 0) trangThai = `<small class="font-bold fst-italic text-danger">Đã xóa</small>`;
                                else if (data.LopHoc.TrangThaiLopHoc == 1) trangThai = `<small class="font-bold fst-italic text-primary">Chưa bắt đầu</small>`;
                                else if (data.LopHoc.TrangThaiLopHoc == 2) trangThai = `<small class="font-bold fst-italic text-success">Đang học</small>`;
                                else if (data.LopHoc.TrangThaiLopHoc == 3) trangThai = `<small class="font-bold fst-italic text-warning">Tạm dừng</small>`;
                                else if (data.LopHoc.TrangThaiLopHoc == 4) trangThai = `<small class="font-bold fst-italic text-danger">Kết thúc</small>`;
                                return `
                                <span title="${data.LopHoc.TenLopHoc}">${sys.truncateString(data.LopHoc.TenLopHoc ?? "", 60)}</span> <br />
                                ${trangThai}
                                <hr />
                                <table class="table table-bordered table-responsive w-100">
                                    <tbody>
                                        <tr>
                                            <td class="text-left font-bold">Giáo viên</td>
                                            <td class="text-left ">${tenGiaoViens}</td>
                                        </tr>
                                        <tr>
                                            <td class="text-left font-bold">Khách hàng</td>
                                            <td class="text-left ">${tenKhachHangs}</td>
                                        </tr>
                                    </tbody>
                                </table>`;
                            }
                        },
                        {
                            data: null,
                            className: "text-left",
                            searchable: true,
                            orderable: true,
                            render: function (data, type, row, meta) {
                                const today = moment();  // Ngày hiện tại

                                let buoiHocHomNay = data.LopHoc.BuoiHocs.filter(buoiHoc => {
                                    // Trích xuất timestamp từ chuỗi "/Date(...)"
                                    const timestamp = parseInt(buoiHoc.ThoiGianBatDau.match(/\d+/)[0]);

                                    // Tạo đối tượng moment từ timestamp
                                    const buoiHocDate = moment(timestamp);

                                    // So sánh ngày theo định dạng dd/mm/yyyy
                                    return buoiHocDate.format('DD/MM/YYYY') === today.format('DD/MM/YYYY');
                                });

                                let soBuoiDaHoc = data.LopHoc.BuoiHocs.filter(buoiHoc => {
                                    // Trích xuất timestamp từ chuỗi "/Date(...)"
                                    const timestamp = parseInt(buoiHoc.ThoiGianBatDau.match(/\d+/)[0]);

                                    // Tạo đối tượng moment từ timestamp
                                    const buoiHocDate = moment(timestamp);

                                    // So sánh xem buổi học đã xảy ra hay chưa
                                    return buoiHocDate.isBefore(today, 'day');
                                });

                                let trangThaiDiemDanh = function (buoiHoc) {
                                    let diemDanhHomNay = "";
                                    if (buoiHoc.DiemDanh == 0) diemDanhHomNay = `<span class="font-bold text-danger">Chưa điểm danh</span>`;
                                    else if (buoiHoc.DiemDanh == 1) diemDanhHomNay = `<span class="font-bold text-success">Đã điểm danh</span>`;
                                    else if (buoiHoc.DiemDanh == 2) diemDanhHomNay = `<span class="font-bold text-warning">HV nghỉ không phép</span>`;
                                    else diemDanhHomNay = `<span class="font-bold text-warning">HV nghỉ có phép</span>`;
                                    return diemDanhHomNay;
                                }

                                let diemDanhHomNay = "";
                                if (buoiHocHomNay.length == 0) diemDanhHomNay = `<span class="font-bold text-danger">Không có lớp</span>`;
                                else diemDanhHomNay = trangThaiDiemDanh(buoiHocHomNay[0]);
                                return `
                                <table class="table table-bordered table-responsive w-100">
                                    <tbody>
                                        <tr>
                                            <td class="text-danger">Buổi đã qua</td>
                                            <td class="text-center">${soBuoiDaHoc.length}</td>
                                        </tr>
                                        <tr>
                                            <td class="text-success">Tổng số buổi</td>
                                            <td class="text-center">${data.LopHoc.BuoiHocs.length}</td>
                                        </tr>
                                    </tbody>
                                </table>
                                <table class="table table-bordered table-responsive w-100">
                                    <tbody>
                                        <tr>
                                            <td class="">Hôm nay</td>
                                            <td class="">${diemDanhHomNay}</td>
                                        </tr>
                                        <tr>
                                            <td class="">Ngày bắt đầu</td>
                                            <td class="">${moment(data.LopHoc.BuoiHocs[0].ThoiGianBatDau).format('DD-MM-YYYY')}</td>
                                        </tr>
                                        <tr>
                                            <td class="">Dự kiến kết thúc</td>
                                            <td class="">${moment(data.LopHoc.BuoiHocs[data.BuoiHocs.length - 1].ThoiGianBatDau).format('DD-MM-YYYY')}</td>
                                        </tr>
                                    </tbody>
                                </table>
                                `;
                            }
                        },
                        {
                            data: "LopHoc.GhiChu",
                            className: "text-left",
                            render: function (data, type, row, meta) {
                                let ghiChu = data.replaceAll("\n", "<br />");
                                return `<span title="${data}">${sys.truncateString(ghiChu, 60)}</span>`;
                            }
                        },
                        {
                            data: null,
                            className: "text-center",
                            searchable: false,
                            orderable: false,
                            render: function (data, type, row, meta) {
                                return `
                                    <div style="white-space: nowrap">
                                        <a class="btn btn-sm btn-light-secondary" title="Gửi mail" onclick="quanLyLopHoc.lopHoc.guiMail('${data.LopHoc.IdLopHoc}')"><i class="bi bi-envelope-paper-fill"></i></a>

                                        <a class="btn btn-sm btn-light-secondary" title="Xem chi tiết" onclick="quanLyLopHoc.lopHoc.displayModal_CRUD_LopHoc('read','${data.LopHoc.IdLopHoc}')"><i class="bi bi-eye-fill"></i></a>
                                    </div>`;
                            }
                        }
                    ],
                }
            }).dataTable,
            displayModal_CRUD_LopHoc: function (loai = "", idLopHoc = '00000000-0000-0000-0000-000000000000') {
                if (loai == "update") {
                    var idLopHocs = [];
                    quanLyLopHoc.lopHoc.dataTable.rows().iterator('row', function (context, index) {
                        var $row = $(this.row(index).node());
                        if ($row.has("input.checkRow-daxeplop-getList:checked").length > 0) {
                            idLopHocs.push($row.attr('id'));
                        };
                    });
                    if (idLopHocs.length != 1) {
                        sys.alert({ mess: "Yêu cầu chọn 1 bản ghi", status: "warning", timeout: 1500 });
                        return;
                    }
                    else idLopHoc = idLopHocs[0];
                };
                $.ajax({
                    ...ajaxDefaultProps({
                        url: "/QuanLyLopHoc/displayModal_CRUD_LopHoc",
                        type: "POST",
                        data: { loai, idLopHoc }
                    }),
                    success: function (res) {
                        $("#quanlylophoc-crud").html(res.view);

                        // Tạo dataTable cho tài liệu
                        quanLyLopHoc.lopHoc.taiLieu.dataTable = new DataTableCustom({
                            name: "table-tailieu",
                            table: $("#table-tailieu", $("#quanlylophoc-crud")),
                            props: {
                                dom: `
                                <'row'<'col-sm-12'rt>>
                                <'row'<'col-sm-12 col-md-6 text-left'i><'col-sm-12 col-md-6 pt-2'p>>`,
                                stateSave: false,
                                lengthMenu: [
                                    [2, 5],
                                    [2, 5],
                                ],
                                createdRow: function (row, data, dataIndex) {
                                    $(row).addClass("tr-tailieu");
                                    var dataId = $(row).attr("data-idtailieu");
                                    if (!dataId) $(row).attr("data-idtailieu", '00000000-0000-0000-0000-000000000000');
                                },
                                columnDefs: [{
                                    className: 'text-center',
                                    target: [0, 1]
                                },
                                {
                                    className: 'text-center w-5',
                                    target: [2]
                                }]
                            },
                        }).dataTable;

                        // Tạo dataTable cho giáo viên
                        quanLyLopHoc.lopHoc.giaoVien.dataTable_CRUD = new DataTableCustom({
                            name: "table-lophoc_giaovien",
                            table: $("#table-lophoc_giaovien", $("#quanlylophoc-crud")),
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

                        // Tạo dataTable cho đơn hàng
                        quanLyLopHoc.lopHoc.donHang.dataTable_CRUD = new DataTableCustom({
                            name: "table-lophoc_donhang",
                            table: $("#table-lophoc_donhang", $("#quanlylophoc-crud")),
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


                        setTimeout(function () { // Đợi load
                            // Tạo lịch học
                            quanLyLopHoc.lichHoc.refeshLichHocCalendar(loai, "xemchitiet", res.lopHoc.BuoiHocs);
                        }, 600);
                        sys.displayModal({
                            name: "#quanlylophoc-crud"
                        });
                    }
                })
            },
            save: function (loai) {
                var modalValidtion = htmlEl.activeValidationStates("#quanlylophoc-crud");
                if (modalValidtion) {
                    var lopHoc = {
                        LopHoc: {
                            IdLopHoc: $("#input-idlophoc", $("#quanlylophoc-crud")).val(),
                            //MaLopHoc: $("#input-masanpham", $("#quanlylophoc-crud")).val().trim(),
                            TenLopHoc: $("#input-tenlophoc", $("#quanlylophoc-crud")).val().trim(),
                            NenTangHoc: $("#select-nentanghoc", $("#quanlylophoc-crud")).val(),
                            //IdGiaoVien: $("#select-giaovien", $("#quanlylophoc-crud")).val().join(","),

                            GhiChu: $("#input-ghichu", $("#quanlylophoc-crud")).val().trim(),
                        },
                        BuoiHocs: quanLyLopHoc.lichHoc.layDuLieu_LichHoc().map(x => x.extendedProps),
                        TaiLieus: []
                    };
                    quanLyLopHoc.lopHoc.taiLieu.dataTable.rows().iterator('row', function (context, index) {
                        var node = $(this.row(index).node());
                        let taiLieu = {
                            IdTaiLieu: $(node).attr("data-idtailieu"),
                            TenTaiLieu: $(".input-tentailieu", node).val() ?? "Tài liệu học",
                            DuongDanTaiLieu: $(".input-duongdantailieu", node).val()
                        };
                        if (taiLieu.DuongDanTaiLieu != "") lopHoc.TaiLieus.push(taiLieu);
                    });

                    if (lopHoc.BuoiHocs.length == 0) {
                        sys.alert({ mess: "Bạn cần xếp lịch học", status: "warning", timeout: 1500 });
                    } else {
                        sys.confirmDialog({
                            mess: `
                                <p class="font-bold">
                                    Kiểm tra kỹ <span class="text-danger fst-italic"> thông tin lịch học</span> <br />
                                </p>
                                <p class="font-bold">Không thể chỉnh sửa sau khi lưu <br />
                                    <span class="text-danger fst-italic"> Hãy liên hệ quản trị viên nếu cần hỗ trợ cập nhật</span>
                                </p>
                                <p>Bạn có thực sự muốn thêm bản ghi này hay không ?</p>
                                `,
                            callback: function () {
                                var f = new FormData();
                                f.append("str_LopHoc", JSON.stringify(lopHoc));
                                f.append("loai", loai);

                                $.ajax({
                                    ...ajaxDefaultProps({
                                        url: loai == "create" ? "/QuanLyLopHoc/create_LopHoc" : loai == "diemdanh" ? "/QuanLyLopHoc/diemDanh" : "/QuanLyLopHoc/update_LopHoc",
                                        type: "POST",
                                        data: f,
                                        //data: JSON.stringify(lopHoc)
                                    }),
                                    //contentType: "application/json; charset=utf-8",  // Chỉ định kiểu nội dung là JSON
                                    contentType: false,
                                    processData: false,
                                    success: function (res) {
                                        if (loai == "diemdanh") {
                                            if (res.status == "success") {
                                                quanLyLopHoc.lopHocThamGia.dataTable.ajax.reload();
                                                quanLyLopHoc.buoiHocSapToi.dataTable.ajax.reload(function () {
                                                    sys.displayModal({
                                                        name: '#quanlylophoc-crud',
                                                        displayStatus: "hide"
                                                    });
                                                }, false);
                                            };
                                            sys.alert({ status: res.status, mess: res.mess });
                                        } else {
                                            if (res.status == "success") {
                                                quanLyLopHoc.donHang.dataTable.ajax.reload();
                                                quanLyLopHoc.lopHoc.dataTable.ajax.reload(function () {
                                                    sys.displayModal({
                                                        name: '#quanlylophoc-crud',
                                                        displayStatus: "hide"
                                                    });
                                                    sys.alert({ status: res.status, mess: res.mess });
                                                }, false);
                                            } else {
                                                if (res.status == "warning") {
                                                    htmlEl.inputValidationStates(
                                                        $("#input-tensanpham"),
                                                        "#quanlylophoc-crud",
                                                        res.mess,
                                                        {
                                                            status: true,
                                                            isvalid: false
                                                        }
                                                    )
                                                };
                                                sys.alert({ status: res.status, mess: res.mess });
                                            };
                                        };
                                    }
                                });
                            }
                        });
                    };
                };
            },
            delete: function (loai, idLopHoc = '00000000-0000-0000-0000-000000000000') {
                var idLopHocs = [];
                if (loai == "single") {
                    idLopHocs.push(idLopHoc);
                } else {
                    quanLyLopHoc.lopHoc.dataTable.rows().iterator('row', function (context, index) {
                        var $row = $(this.row(index).node());
                        if ($row.has("input.checkRow-daxeplop-getList:checked").length > 0) {
                            idLopHocs.push($row.attr('id'));
                        };
                    });
                };
                // Kiểm tra idLopHoc
                if (idLopHocs.length > 0) {
                    var f = new FormData();
                    f.append("str_idLopHocs", JSON.stringify(idLopHocs));
                    sys.confirmDialog({
                        mess: `
                        <p class="font-bold">Sản phẩm có liên kết với các
                            <span class="text-danger fst-italic"> [Khách hàng]</span> và
                            <span class="text-danger fst-italic"> [Lớp học]</span> 
                        </p>
                        <p>Bạn có thực sự muốn xóa bản ghi này hay không ?</p>
                        `,
                        callback: function () {
                            $.ajax({
                                ...ajaxDefaultProps({
                                    url: "/QuanLyLopHoc/delete_LopHocs",
                                    type: "POST",
                                    data: f,
                                }),
                                contentType: false,
                                processData: false,
                                success: function (res) {
                                    quanLyLopHoc.lopHoc.dataTable.ajax.reload(function () {
                                        sys.alert({ status: res.status, mess: res.mess })
                                    }, false);
                                }
                            })
                        }
                    })
                } else {
                    sys.alert({ mess: "Bạn chưa chọn bản ghi nào", status: "warning", timeout: 1500 });
                };
            },
            guiMail: function (idLopHoc = '00000000-0000-0000-0000-000000000000') {
                sys.confirmDialog({
                    mess: `
                        <p class="font-bold">Bạn có muốn gửi mail về thông tin lớp cho học viên ?</p>
                        `,
                    callback: function () {
                        $.ajax({
                            ...ajaxDefaultProps({
                                url: "/QuanLyLopHoc/guiMail",
                                type: "POST",
                                data: { idLopHoc }
                            }),
                            success: function (res) {
                                sys.alert({ mess: res.mess, status: res.status, timeout: 1500 });
                            }
                        })
                    }
                })
            }
        };
        quanLyLopHoc.locThongTin = {
            ...quanLyLopHoc.locThongTin,
            displayModal_TimKiem: function (display = 'show') {
                sys.displayModal({
                    name: '#modal-timkiem',
                    displayStatus: 'hide'
                });
                if (display == 'show') {
                    sys.displayModal({
                        name: '#modal-timkiem'
                    });
                };
            },
            timKiem: async function () {
                quanLyLopHoc.locThongTin.data = {
                    IdLoaiSanPham: $("#select-loaisanpham", $("#modal-timkiem")).val(),
                    IdGiaoVien: $("#select-tengiaovien", $("#modal-timkiem")).val(),
                    IdKhachHang: $("#select-tenkhachhang", $("#modal-timkiem")).val(),
                    TrinhDoDauVao: $("#input-idtrinhdodauvao", $("#modal-timkiem")).val(),
                    TrinhDoDauRa: $("#input-idtrinhdodaura", $("#modal-timkiem")).val(),
                };
                await quanLyLopHoc.khachHang.dataTable.ajax.reload();
                await quanLyLopHoc.locThongTin.displayModal_TimKiem('hide');
            },
        };
    }
    funcs_GiaoDienGiaoVien() {
        var quanLyLopHoc = this;
        quanLyLopHoc.buoiHocSapToi = {
            ...quanLyLopHoc.buoiHocSapToi,
            dataTable: new DataTableCustom({
                name: "buoihocsaptoi-getList",
                table: $("#buoihocsaptoi-getList"),
                props: {
                    ajax: {
                        url: '/QuanLyLopHoc/buoiHocSapToi',
                        type: "GET",
                        data: function (arg) {
                            arg.idGiaoVien = quanLyLopHoc.nguoiDung.idNguoiDung_DangSuDung;
                        }
                    },
                    dom: `
                        <'row'<'col-sm-12'rt>>
                        <'row'<'col-sm-12 col-md-6 text-left'i><'col-sm-12 col-md-6 pt-2'p>>`,
                    lengthMenu: [
                        [5, 10],
                        [5, 10],
                    ],
                    order: [[1, 'asc']],
                    //aaSorting: [],
                    //columnDefs: [{
                    //    'targets': [1], /* column index [0,1,2,3]*/
                    //    'orderable': true, /* true or false */
                    //}],
                    //rowId: 'IdKhachHang_DonHang_ThanhToan',
                    columns: [
                        {
                            data: null,
                            orderable: false,
                            className: "text-center",
                            render: function (data, type, row, meta) {

                                return `
                                <span title="${data.LopHoc.TenLopHoc}">${sys.truncateString(data.LopHoc.TenLopHoc ?? "", 60)}</span>
                                `;
                            }
                        },
                        {
                            data: null,
                            className: "text-center",
                            render: function (data, type, row, meta) {
                                /**
                                * 0: Xóa, 
                                * 1: Khởi tạo - Chưa bắt đầu, 
                                * 2: Đang học, 
                                * 3: Tạm dừng
                                * 4: Kết thúc
                                */
                                let ngayConLai = "";
                                if (data.SoNgayConLai < 0) ngayConLai = `<small class="font-bold fst-italic text-danger">Muộn ${(data.SoNgayConLai * (-1))} ngày</small>`;
                                else if (data.SoNgayConLai == 0) ngayConLai = `<small class="font-bold fst-italic text-danger">Hôm nay</small>`;
                                else ngayConLai = `<small class="fst-italic text-primary">(${moment(data.LopHoc.ThoiGianBatDau).format('DD/MM/yyyy')} - còn ${data.SoNgayConLai} ngày)</small>`;
                                return `<span>${data.BuoiHoc.ThuTuBuoiHoc} / ${data.LopHoc.SoBuoi}</span> <br />
                                ${ngayConLai}`;
                            }
                        },
                        {
                            data: null,
                            className: "text-center",
                            render: function (data, type, row, meta) {
                                let diemDanhHomNay = "";
                                if (data.BuoiHoc.DiemDanh == 0) diemDanhHomNay = `<span class="font-bold text-danger">Chưa điểm danh</span>`;
                                else if (data.BuoiHoc.DiemDanh == 1) diemDanhHomNay = `<span class="font-bold text-success">Đã điểm danh</span>`;
                                else if (data.BuoiHoc.DiemDanh == 2) diemDanhHomNay = `<span class="font-bold text-warning">HV nghỉ không phép</span>`;
                                else diemDanhHomNay = `<span class="font-bold text-warning">HV nghỉ có phép</span>`;
                                return diemDanhHomNay;
                            }
                        },
                        {
                            data: null,
                            className: "text-center",
                            searchable: false,
                            orderable: false,
                            render: function (data, type, row, meta) {
                                let info = {
                                    event: {
                                        extendedProps: {
                                            IdLopHoc_BuoiHoc: data.BuoiHoc.IdLopHoc_BuoiHoc,
                                        }
                                    }
                                };
                                if (data.SoNgayConLai == 0 || data.SoNgayConLai == -1)
                                    return `
                                    <div style="white-space: nowrap">
                                        <a class="btn btn-sm btn-light-secondary" title="Điểm danh" onclick="quanLyLopHoc.lopHoc.displayModal_CRUD_LopHoc('diemdanh','${data.LopHoc.IdLopHoc}')"><i class="bi bi-calendar2-check-fill"></i></a>
                                    </div>`;
                                return "";
                            }
                        }
                    ],
                }
            }).dataTable,
        };
        quanLyLopHoc.lopHocThamGia = {
            ...quanLyLopHoc.lopHocThamGia,
            dataTable: new DataTableCustom({
                name: "lophocthamgia-getList",
                table: $("#lophocthamgia-getList"),
                props: {
                    ajax: {
                        url: '/QuanLyLopHoc/lopHocThamGia',
                        type: "GET",
                        data: function (arg) {
                            arg.idGiaoVien = quanLyLopHoc.nguoiDung.idNguoiDung_DangSuDung;
                        }
                    },
                    dom: `
                        <'row'<'col-sm-12'rt>>
                        <'row'<'col-sm-12 col-md-6 text-left'i><'col-sm-12 col-md-6 pt-2'p>>`,
                    lengthMenu: [
                        [2, 5],
                        [2, 5],
                    ],
                    //rowId: 'IdKhachHang_DonHang_ThanhToan',
                    columns: [
                        {
                            data: null,
                            className: "text-center",
                            render: function (data, type, row, meta) {
                                /**
                                 * 0: Xóa, 
                                 * 1: Khởi tạo - Chưa bắt đầu, 
                                 * 2: Đang học, 
                                 * 3: Tạm dừng
                                 * 4: Kết thúc
                                 */
                                let trangThai = "";
                                if (data.LopHoc.TrangThaiLopHoc == 0) trangThai = `<small class="font-bold fst-italic text-danger">Đã xóa</small>`;
                                else if (data.LopHoc.TrangThaiLopHoc == 1) trangThai = `<small class="font-bold fst-italic text-primary">Chưa bắt đầu</small>`;
                                else if (data.LopHoc.TrangThaiLopHoc == 2) trangThai = `<small class="font-bold fst-italic text-success">Đang học</small>`;
                                else if (data.LopHoc.TrangThaiLopHoc == 3) trangThai = `<small class="font-bold fst-italic text-warning">Tạm dừng</small>`;
                                else if (data.LopHoc.TrangThaiLopHoc == 4) trangThai = `<small class="font-bold fst-italic text-danger">Kết thúc</small>`;
                                return `
                                <span title="${data.LopHoc.TenLopHoc}">${sys.truncateString(data.LopHoc.TenLopHoc ?? "", 60)}</span> <br />
                                ${trangThai}`;
                            }
                        },
                        {
                            data: null,
                            className: "text-left",
                            render: function (data, type, row, meta) {
                                const today = moment();  // Ngày hiện tại

                                let buoiHocHomNay = data.LopHoc.BuoiHocs.filter(buoiHoc => {
                                    // Trích xuất timestamp từ chuỗi "/Date(...)"
                                    const timestamp = parseInt(buoiHoc.ThoiGianBatDau.match(/\d+/)[0]);

                                    // Tạo đối tượng moment từ timestamp
                                    const buoiHocDate = moment(timestamp);

                                    // So sánh ngày theo định dạng dd/mm/yyyy
                                    return buoiHocDate.format('DD/MM/YYYY') === today.format('DD/MM/YYYY');
                                });

                                let soBuoiDaHoc = data.LopHoc.BuoiHocs.filter(buoiHoc => {
                                    // Trích xuất timestamp từ chuỗi "/Date(...)"
                                    const timestamp = parseInt(buoiHoc.ThoiGianBatDau.match(/\d+/)[0]);

                                    // Tạo đối tượng moment từ timestamp
                                    const buoiHocDate = moment(timestamp);

                                    // So sánh xem buổi học đã xảy ra hay chưa
                                    return buoiHocDate.isBefore(today, 'day');
                                });

                                let trangThaiDiemDanh = function (buoiHoc) {
                                    let diemDanhHomNay = "";
                                    if (buoiHoc.DiemDanh == 0) diemDanhHomNay = `<span class="font-bold text-danger">Chưa điểm danh</span>`;
                                    else if (buoiHoc.DiemDanh == 1) diemDanhHomNay = `<span class="font-bold text-success">Đã điểm danh</span>`;
                                    else if (buoiHoc.DiemDanh == 2) diemDanhHomNay = `<span class="font-bold text-warning">HV nghỉ không phép</span>`;
                                    else diemDanhHomNay = `<span class="font-bold text-warning">HV nghỉ có phép</span>`;
                                    return diemDanhHomNay;
                                }

                                let diemDanhHomNay = "";
                                if (buoiHocHomNay.length == 0) diemDanhHomNay = `<span class="font-bold text-danger">Không có lớp</span>`;
                                else diemDanhHomNay = trangThaiDiemDanh(buoiHocHomNay[0]);
                                return `
                                <table class="table table-bordered table-responsive w-100">
                                    <tbody>
                                        <tr>
                                            <td class="text-danger">Buổi đã qua</td>
                                            <td class="text-center">${soBuoiDaHoc.length}</td>
                                        </tr>
                                        <tr>
                                            <td class="text-success">Tổng số buổi</td>
                                            <td class="text-center">${data.LopHoc.BuoiHocs.length}</td>
                                        </tr>
                                    </tbody>
                                </table>
                                <table class="table table-bordered table-responsive w-100">
                                    <tbody>
                                        <tr>
                                            <td class="">Hôm nay</td>
                                            <td class="">${diemDanhHomNay}</td>
                                        </tr>
                                        <tr>
                                            <td class="">Ngày bắt đầu</td>
                                            <td class="">${moment(data.LopHoc.BuoiHocs[0].ThoiGianBatDau).format('DD-MM-YYYY')}</td>
                                        </tr>
                                        <tr>
                                            <td class="">Dự kiến kết thúc</td>
                                            <td class="">${moment(data.LopHoc.BuoiHocs[data.LopHoc.BuoiHocs.length - 1].ThoiGianBatDau).format('DD-MM-YYYY')}</td>
                                        </tr>
                                    </tbody>
                                </table>
                                `;
                            }
                        },
                        {
                            data: null,
                            className: "text-center",
                            searchable: false,
                            orderable: false,
                            render: function (data, type, row, meta) {
                                return `
                                    <div style="white-space: nowrap">
                                        <a class="btn btn-sm btn-light-secondary" title="Xem chi tiết" onclick="quanLyLopHoc.lopHoc.displayModal_CRUD_LopHoc('diemdanh','${data.LopHoc.IdLopHoc}')"><i class="bi bi-eye-fill"></i></a>
                                    </div>`;
                            }
                        }
                    ],
                }
            }).dataTable,
        };
    }
};