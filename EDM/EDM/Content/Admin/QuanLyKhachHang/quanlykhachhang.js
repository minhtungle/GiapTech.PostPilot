'use strict'
/**
 * main
 * */
class QuanLyKhachHangService {
    constructor() {
        this.page;
        this.pageGroup;
        this.khachHang = {
            data: [],
            dataTable: null,
            save: function () { },
            delete: function () { },
            displayModal_CRUD: function () { },
        };
        this.coCauToChuc = {
            getList: function () { },
            data: [],
            selected: [],
            treeSelect: null,
        }
        this.locThongTin = {
            data: {
                IdCoCauToChucs: [],
                NgayTao: $("#input-ngaytao", $("#modal-timkiem")).val(),
                TenKhachHang: $("#input-tenkhachhang", $("#modal-timkiem")).val(),
                TenNhanVien: $("#input-tennhanvien", $("#modal-timkiem")).val(),
                Email: $("#input-email", $("#modal-timkiem")).val(),
                IdLoaiKhachHang: $("#select-loaikhachhang", $("#modal-timkiem")).val(),
                IdGoiChamSoc: $("#select-goichamsoc", $("#modal-timkiem")).val(),
            }
        }
    }
    init() {
        var quanLyKhachHang = this;
        var idNguoiDung_DangSuDung = $("#input-idnguoidung-dangsudung").val();
        quanLyKhachHang.page = $("#page-quanlykhachhang");

        quanLyKhachHang.khachHang = {
            ...quanLyKhachHang.khachHang,
            dataTable: new DataTableCustom({
                name: "quanlykhachhang-getList",
                table: $("#quanlykhachhang-getList"),
                props: {
                    ajax: {
                        url: '/QuanLyKhachHang/getList',
                        type: "POST",
                        data: function () {
                            return {
                                locThongTin: {
                                    ...quanLyKhachHang.locThongTin.data,
                                },
                            }
                        }
                    },
                    rowId: 'IdKhachHang',
                    columns: [
                        {
                            data: null,
                            className: "text-center",
                            searchable: false,
                            orderable: false,
                            render: function (data, type, row, meta) {
                                var quyenTruyCap = data.QuyenTruyCap.split(",");
                                // Nếu có quyền thì cho thao tác
                                if (quyenTruyCap.some(x => x == idNguoiDung_DangSuDung)) {
                                    return `<input class="form-check-input checkRow-quanlykhachhang-getList" type="checkbox"/>`;
                                };
                                return ``;
                            }
                        },
                        {
                            data: null,
                            className: "text-center",
                            render: function (data, type, row, meta) {
                                let tenLoaiKhachHang = data.LoaiKhachHang.TenLoaiKhachHang;
                                let trangThaiKhachHang = "";
                                if (tenLoaiKhachHang == "Đang tư vấn") trangThaiKhachHang = `<small class="font-bold fst-italic text-warning">${tenLoaiKhachHang}</small>`;
                                else if (tenLoaiKhachHang == "Chờ xếp lớp - học thử") trangThaiKhachHang = `<small class="font-bold fst-italic text-danger">${tenLoaiKhachHang}</small>`;
                                else if (tenLoaiKhachHang == "Chờ xếp lớp - học chính") trangThaiKhachHang = `<small class="font-bold fst-italic text-danger">${tenLoaiKhachHang}</small>`;
                                else if (tenLoaiKhachHang == "Đang học") trangThaiKhachHang = `<small class="font-bold fst-italic text-success">${tenLoaiKhachHang}</small>`; else if (tenLoaiKhachHang == "Ngừng chăm sóc") trangThaiKhachHang = `<small class="font-bold fst-italic text-danger">${tenLoaiKhachHang}</small>`;
                                //else html = `<span class="font-bold text-warning">HV nghỉ có phép</span>`;
                                return `
                                <span>${data.TenKhachHang}</span> <br />
                                ${trangThaiKhachHang}`;
                            }
                        },
                        {
                            data: "Email",
                            className: "text-center",
                        },
                        {
                            data: "ThongTinNguoiTao.TenNguoiDung",
                            className: "text-center",
                        },
                        {
                            data: "NgayTao",
                            className: "text-center",
                            render: function (data, type, row, meta) {
                                return data == null ? "" : moment(data).format('DD-MM-YYYY')
                            }
                        },
                        {
                            data: "NgaySua",
                            className: "text-center",
                            render: function (data, type, row, meta) {
                                return data == null ? "" : moment(data).format('DD-MM-YYYY')
                            }
                        },
                        {
                            data: null,
                            className: "text-center",
                            searchable: false,
                            orderable: false,
                            render: function (data, type, row, meta) {
                                //var quyenTruyCap = data.QuyenTruyCap.split(",");
                                // Nếu có quyền thì cho thao tác
                                //if (quyenTruyCap.some(x => x == idNguoiDung_DangSuDung)) {
                                return `
                                    <div style="white-space: nowrap" intro-container="thanhtacvu-bangdulieu">
                                        <a class="btn btn-sm btn-primary" title="Xem chi tiết" onclick="quanLyKhachHang.khachHang.displayModal_CRUD('read', '${data.IdKhachHang}')"><i class="bi bi-eye-fill"></i></a>
                                    </div>`;
                                //};
                                //return `<span class="font-bold fst-italic text-danger">Không có quyền</span>`;
                            }
                        }
                    ],
                }
            }).dataTable,
            displayModal_CRUD: function (loai = "", idKhachHang = '00000000-0000-0000-0000-000000000000') {
                if (idKhachHang == '00000000-0000-0000-0000-000000000000' && (loai == "update" || loai == "update-donhang")) {
                    var idKhachHangs = [];
                    quanLyKhachHang.khachHang.dataTable.rows().iterator('row', function (context, index) {
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
                        url: "/QuanLyKhachHang/displayModal_CRUD",
                        type: "POST",
                        data: { loai, idKhachHang }
                    }),
                    success: function (res) {
                        $("#quanlykhachhang-crud").html(res);
                        quanLyKhachHang.create_LichSu();
                        /**
                          * Gán các thuộc tính
                          */
                        var rows_NEW = quanLyKhachHang.khachHang_lichSu.dataTable.rows().nodes().toArray(); // Chọn phần thử đầu tiên của bảng
                        quanLyKhachHang.khachHang_lichSu.readRow($(rows_NEW[0]));
                        sys.displayModal({
                            name: '#quanlykhachhang-crud'
                        });
                        // Cập nhật giá trị thẻ sản phẩm
                        $(".select-sanpham").trigger("change");
                    }
                })
            },
            themDonHang: function (e, loaiThemMoi = "donhang", loai = "update") {
                var idKhachHang = $("#input-idkhachhang").val();
                var khachHang_DonHangs = [{
                    IdKhachHang: idKhachHang,
                    IdSanPham: '00000000-0000-0000-0000-000000000000',
                    SanPham: {
                        IdSanPham: '00000000-0000-0000-0000-000000000000'
                    },
                    ThanhToans: [{
                        IdSanPham: '00000000-0000-0000-0000-000000000000',
                        IdKhachHang: idKhachHang,
                        IdDonHang: '00000000-0000-0000-0000-000000000000',
                        PhanTramDaDong: 0,
                    }]
                }];
                $.ajax({
                    ...ajaxDefaultProps({
                        url: "/QuanLyKhachHang/themDonHang",
                        type: "POST",
                        data: {
                            khachHang_DonHangs,
                            loaiThemMoi,
                            loai
                        }
                    }),
                    success: function (res) {
                        if (loaiThemMoi == "donhang") {
                            $("#btn-themdonhang").after(res);
                            quanLyKhachHang.khachHang.capNhatSTT(e, loaiThemMoi);
                        }
                        else {
                            // Thêm vào sau thẻ tr.donhang
                            $(e).closest("tr.donhang").after(res);
                            var $donHang = $(e).closest("table.table-donhang");
                            quanLyKhachHang.khachHang.capNhatSTT($donHang, loaiThemMoi);
                        };
                        // Kích hoạt lại htmlEl
                        htmlEl = new HtmlElement();
                    }
                })
            },
            xoaDonHang: function (e, loaiThemMoi = "donhang") {
                if (loaiThemMoi == "donhang") {
                    var soLuongDonHang = $("#danhsach-donhang table.table-donhang").length;

                    // Chỉ xóa khi > 1 sản phẩm
                    //if (soLuongDonHang > 1) {
                    $(e).closest("table.table-donhang").remove();
                    quanLyKhachHang.khachHang.capNhatSTT(e, loaiThemMoi);
                    //}
                    //else sys.alert({ status: "error", mess: "Phải tồn tại ít nhất 1 sản phẩm" });
                } else {
                    var $tbDonHang = $(e).closest("table.table-donhang");
                    var soLuongDonHang = $("tr.thanhtoan", $tbDonHang).length;

                    // Chỉ xóa khi > 1 đơn hàng
                    if (soLuongDonHang > 1) {
                        $(e).closest("tr.thanhtoan").remove();
                        quanLyKhachHang.khachHang.capNhatSTT($tbDonHang, loaiThemMoi);
                    }
                    else sys.alert({ status: "error", mess: "Phải tồn tại ít nhất 1 lần thanh toán" });
                };
            },
            capNhatSTT: function (e, loaiThemMoi = "donhang") {
                if (loaiThemMoi == "donhang") {
                    var $donHangs = $("#danhsach-donhang table.table-donhang"),
                        soLuongDonHang = $donHangs.length;
                    $.each($donHangs, function (i, $donHang) {
                        var stt = soLuongDonHang - i;
                        $(".stt-donhang", $donHang).text(`Khóa học ${stt}`);
                    });
                } else {
                    var $thanhToans = $("tr.thanhtoan", $(e)),
                        soLuongDonHang = $thanhToans.length;
                    $.each($thanhToans, function (i, $thanhToan) {
                        var stt = soLuongDonHang - i;
                        $(".stt-thanhtoan", $thanhToan).text(`Đóng lần ${stt}`);
                    });
                }
            },
            chonSanPham: function (e) {
                var $tbDonHang = $(e).closest("table.table-donhang"),
                    $donHang = $("tr.donhang", $tbDonHang),
                    $inputTongSoTien = $(".input-tongsotien", $donHang),
                    giaTien = $("option:selected", $(e)).data("giatien");
                $inputTongSoTien.val(giaTien); // Cập nhật giá tiền
                quanLyKhachHang.khachHang.capNhatSoTienDaDong($tbDonHang); // Cập nhật số tiền đã đống cho đơn hàng đó
            },
            nhapSoTienDaDong: function (e) {
                var $tbDonHang = $(e).closest("table.table-donhang"); // Lấy đơn hàng
                quanLyKhachHang.khachHang.capNhatSoTienDaDong($tbDonHang); // Cập nhật số tiền đã đống cho đơn hàng đó
            },
            chonDonViTien: function (e) {
                var $tbDonHang = $(e).closest("table.table-donhang"); // Lấy đơn hàng
                quanLyKhachHang.khachHang.capNhatSoTienDaDong($tbDonHang); // Cập nhật số tiền đã đống cho đơn hàng đó
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
            test: function () {
                quanLyKhachHang.khachHang.thongBaoTienDoDoanhThu({
                    nguoiDung_DoanhThu: {
                        DoanhThuMucTieu: 5 * (10 ^ 7),
                        DoanhThuThucTe: 10 ^ 7,
                        PhanTramHoanThien: ((10 ^ 7) / 5 * (10 ^ 7)) * 100,
                        NgayLenMucTieu: "09/2024"
                    },
                    nguoiDung_DoanhThus_NHOM: [{
                        DoanhThuMucTieu: 6 * (10 ^ 7),
                        DoanhThuThucTe: 2 * (10 ^ 7),
                        PhanTramHoanThien: ((2 * (10 ^ 7)) / 6 * (10 ^ 7)) * 100,
                        NgayLenMucTieu: "09/2024"
                    }, {
                        DoanhThuMucTieu: 7 * (10 ^ 7),
                        DoanhThuThucTe: 10 ^ 7,
                        PhanTramHoanThien: ((10 ^ 7) / 7 * (10 ^ 7)) * 100,
                        NgayLenMucTieu: "09/2024"
                    }, {
                        DoanhThuMucTieu: 3 * (10 ^ 7),
                        DoanhThuThucTe: 3 * (10 ^ 7),
                        PhanTramHoanThien: ((3 * (10 ^ 7)) / 5 * (10 ^ 7)) * 100,
                        NgayLenMucTieu: "09/2024"
                    },]
                })
            },
            thongBaoTienDoDoanhThu: function ({ nguoiDung_DoanhThu, nguoiDung_DoanhThus_NHOM }) {
                let doanhThuMucTieu = sys.convertInt2MoneyFormat(nguoiDung_DoanhThu.DoanhThuMucTieu),
                    doanhThuThucTe = sys.convertInt2MoneyFormat(nguoiDung_DoanhThu.DoanhThuThucTe),
                    doanhThuConLai = sys.convertInt2MoneyFormat((nguoiDung_DoanhThu.DoanhThuMucTieu - nguoiDung_DoanhThu.DoanhThuThucTe)),
                    phanTramHoanThien = nguoiDung_DoanhThu.PhanTramHoanThien,
                    ngayLenMucTieu = nguoiDung_DoanhThu.NgayLenMucTieu;

                let trangThai = "";

                // Đo tiến độ
                if (phanTramHoanThien < 30) {
                    trangThai = "danger";
                } else if (phanTramHoanThien < 50) {
                    trangThai = "warning";
                } else {
                    trangThai = "success";
                };

                let processHTML = `
                <div class="progress progress-sm progress-${trangThai} mx-5 mt-4 mb-2">
                    <div class="progress-bar progress-label" role="progressbar"
                        style="width: ${phanTramHoanThien}%" 
                        aria-valuenow="${phanTramHoanThien}"
                        aria-valuemin="0" aria-valuemax="${(phanTramHoanThien > 100 ? phanTramHoanThien : 100)}">
                    </div>
                </div>
                <div>
                    <span class="text-${trangThai}">${doanhThuThucTe} / <span class="font-bold">${doanhThuMucTieu}</span></span>
                    ${(phanTramHoanThien == 100 ? `` : `- <span class="text-danger">còn ${doanhThuConLai}</span>`)}
                </div>
                `;

                // Hiển thị thông báo
                let messs = [];
                // Tổng kết doanh thu
                messs.push(processHTML);
                // Doanh thu cá nhân
                //if (phanTramHoanThien >= 100) {
                //    messs.push(`
                //    <h3 class="font-bold">Đã hoàn thành mục tiêu cá nhân 🏆</h3>
                //    <h1 class="font-bold" style="font-size: 7rem">️🎉🎉</h1>
                //    `);
                //} else {
                //    messs.push(`
                //    <h4 class="font-bold">✨ Năng lượng vũ trụ cho thấy bạn là chiến thần chốt đơn 🔮</h4>
                //    `);
                //};

                // Doanh thu đội nhóm
                let soLuongNguoi_CoDoanhThuCaoHon = nguoiDung_DoanhThus_NHOM.length;
                if (soLuongNguoi_CoDoanhThuCaoHon == 0) {
                    messs.push(`
                    <h3 class="font-bold">Đắc đạo thành công, <span class="text-danger font-bold" style="font-size: 2rem">TOP 1</span> doanh thu đã thuộc về ngài 👑</h3>
                    <h1 class="font-bold" style="font-size: 7rem">🙇🙇🙇</h1>
                    <h4 class="font-bold">Vạn tuế ... vạn tuế ... vạn thịnh phát</h4>
                    <hr />
                    <small class="font-bold fst-italic text-danger">"Thế gian này rộng lớn, kẻ thù ngoài kia còn nhiều. Hành trình bảo vệ ngôi báu của ngài chỉ mới bắt đầu 🗡"</small>
                    `);
                } else if (soLuongNguoi_CoDoanhThuCaoHon == 1) {
                    let khoangCachDoanhThu = sys.convertInt2MoneyFormat(nguoiDung_DoanhThus_NHOM[0].DoanhThuThucTe - nguoiDung_DoanhThu.DoanhThuThucTe);
                    messs.push(`
                    <h3 class="font-bold">Còn gì khó hơn không ? <span class="text-danger font-bold" style="font-size: 2rem">TOP 2</span> tới chơi</h3>
                    <h1 class="font-bold" style="font-size: 7rem">🥈</h1>
                    <h4 class="font-bold">Thưa nguyên soái 🏇, <span class="text-danger font-bold" style="">TOP 1</span> nợ ngài <span class="text-danger font-bold">${khoangCachDoanhThu}</span> và 1 cái đầu 💀
                    </h4>
                    <hr />
                    <small class="font-bold fst-italic text-danger">"Kẻ về nhì vĩ đại ư ? MUA HAHAHA, thật nực cười 👺. Cái ta cần là ngôi báu kìa. Hãy cẩn thận lúc ngươi ngủ trưa 🛌"</small>
                    `);
                } else if (soLuongNguoi_CoDoanhThuCaoHon == 2) {
                    let khoangCachDoanhThu = sys.convertInt2MoneyFormat(nguoiDung_DoanhThus_NHOM[1].DoanhThuThucTe - nguoiDung_DoanhThu.DoanhThuThucTe);
                    messs.push(`
                    <h3 class="font-bold">Loạn 12 sứ sale 🤺🤺 <span class="text-danger font-bold" style="font-size: 2rem">TOP 3</span> ta không thể đứng nhìn</h3>
                    <h1 class="font-bold" style="font-size: 7rem">🥉</h1>
                    <h4 class="font-bold">Quá căng thẳng 🏊, ngôi <span class="text-danger font-bold">TOP 2</span> chỉ còn <span class="text-danger font-bold">${khoangCachDoanhThu}</span>
                    </h4>
                    <hr />
                    <small class="font-bold fst-italic text-danger">"Phía trước còn gian nan 🕵️ binh đao loạn lạc, đầu rơi máu chảy không kể 💉. Thế gian cần ngài thống nhất ✊"</small>
                    `);
                } else {
                    messs.push(`
                    <h4 class="font-bold">✨ Năng lượng vũ trụ cho thấy ngài là chiến thần chốt đơn 🔮</h4>
                    <h1 class="font-bold" style="font-size: 7rem">️🍾</h1>
                    <h4 class="font-bold">
                        Cố lên nào 👉👈 còn <span class="text-danger font-bold">${soLuongNguoi_CoDoanhThuCaoHon}</span> mạng nữa là <span class="text-danger font-bold">TOP 1</span>
                    </h4>
                    <hr />
                    <small class="font-bold fst-italic text-danger">"Ngoài kia, bá tánh mù chữ còn nhiều 📚. Kẻ làm sale như ta sao dám ngon giấc 🙎"</small>
                    `);
                };

                sys.alertDialog({ title: `<h4>Tiến độ: <span class="font-bold">T${ngayLenMucTieu}<span></h4>`, content: messs.join("<br />"), timeout: 5000 });
            },
            save: function (loai) {
                //that.dataTable.rows({ page: 'all' }).data().toArray();
                var modalValidtion = htmlEl.activeValidationStates("#quanlykhachhang-crud");
                if (modalValidtion) {
                    sys.confirmDialog({
                        mess: `
                        <p class="font-bold">
                            Kiểm tra kỹ <span class="text-danger fst-italic"> phân loại khách hàng</span> <br />
                        </p>
                        <p class="font-bold">Không thể chỉnh sửa sau khi lưu <br />
                            <span class="text-danger fst-italic"> Hãy liên hệ quản trị viên nếu cần hỗ trợ cập nhật</span>
                        </p>
                        <p>Bạn có thực sự muốn thêm bản ghi này hay không ?</p>
                        `,
                        callback: function () {
                            var khachHang = {
                                IdKhachHang: $("#input-idkhachhang", $("#quanlykhachhang-crud")).val(),
                                TenKhachHang: $("#input-tenkhachhang", $("#quanlykhachhang-crud")).val().trim(),
                                Email: $("#input-email", $("#quanlykhachhang-crud")).val().trim(),
                                LienKet: $("#input-lienket", $("#quanlykhachhang-crud")).val().trim(),
                                LienKetSale: $("#input-lienketsale", $("#quanlykhachhang-crud")).val().trim(),
                                SoDienThoai: $("#input-sodienthoai", $("#quanlykhachhang-crud")).val().trim(),
                                DoTuoi: $("#input-dotuoi", $("#quanlykhachhang-crud")).val().trim(),
                                NgheNghiep: $("#input-nghenghiep", $("#quanlykhachhang-crud")).val().trim(),
                                NguonKhachHang: $("#input-nguonkhachhang", $("#quanlykhachhang-crud")).val().trim(),
                                DiaChi: $("#input-diachi", $("#quanlykhachhang-crud")).val().trim(),

                                IdLoaiKhachHang: $("#select-loaikhachhang", $("#quanlykhachhang-crud")).val(),
                                LoaiKhachHang: {
                                    IdLoaiKhachHang: $("#select-loaikhachhang", $("#quanlykhachhang-crud")).val(),
                                },

                                IdGoiChamSoc: $("#select-goichamsoc", $("#quanlykhachhang-crud")).val(),
                                GoiChamSoc: {
                                    IdGoiChamSoc: $("#select-goichamsoc", $("#quanlykhachhang-crud")).val(),
                                },

                                IdPhuongThucThanhToan: $("#select-phuongthucthanhtoan", $("#quanlykhachhang-crud")).val(),
                                PhuongThucThanhToan: {
                                    IdPhuongThucThanhToan: $("#select-phuongthucthanhtoan", $("#quanlykhachhang-crud")).val(),
                                },

                                IdQuocGiaSinhSong: $("#select-quocgiasinhsong", $("#quanlykhachhang-crud")).val(),
                                QuocGiaSinhSong: {
                                    IdQuocGiaSinhSong: $("#select-quocgiasinhsong", $("#quanlykhachhang-crud")).val(),
                                },

                                GhiChu: $("#input-ghichu", $("#quanlykhachhang-crud")).val().trim(),
                                TrangThai: $("#select-loaikhachhang option:selected", $("#quanlykhachhang-crud")).data("stt"),
                                DonHangs: []
                            };
                            // Lấy danh sách đơn hàng
                            {
                                var $tbDonHangs = $(".table-donhang");
                                $.each($tbDonHangs, function (iDonHang, $tbDonHang) {
                                    var idSanPham = $(".select-sanpham", $tbDonHang).val(),
                                        idDonHang = $(this).data("iddonhang"),
                                        tongSoTien = $(".select-sanpham option:selected", $tbDonHang).data("giatien"),
                                        idKhachHang = $("#input-idkhachhang", $("#quanlykhachhang-crud")).val();
                                    // Chỉ lấy các đơn hàng đã chọn sản phẩm
                                    if (idSanPham != 0 && idSanPham != null) {
                                        var $thanhToans = $(".thanhtoan", $tbDonHang);
                                        // Chỉ lấy đơn hàng có thanh toán
                                        if ($thanhToans.length > 0) {
                                            // Tạo thông tin đơn hàng
                                            var idTrinhDoDauVao = $(".select-trinhdodauvao", $tbDonHang).val(),
                                                idTrinhDoDauRa = $(".select-trinhdodaura", $tbDonHang).val(),
                                                thuTuDonHang = $tbDonHangs.length - iDonHang,
                                                ghiChu = $(".input-ghichu", $tbDonHang).val();
                                            var donHang = {
                                                IdDonHang: idDonHang,
                                                IdTrinhDoDauVao: idTrinhDoDauVao,
                                                IdTrinhDoDauRa: idTrinhDoDauRa,
                                                IdKhachHang: idKhachHang,
                                                IdSanPham: idSanPham,
                                                ThuTuDonHang: thuTuDonHang,
                                                TongSoTien: tongSoTien,
                                                GhiChu: ghiChu,
                                                SanPham: {
                                                    IdSanPham: idSanPham
                                                },
                                                ThanhToans: []
                                            };
                                            $.each($thanhToans, function (iThanhToan, $thanhToan) {
                                                let thuTuThanhToan = $thanhToans.length - iThanhToan,
                                                    idThanhToan = $(this).data("idthanhtoan"),
                                                    soTienDaDong_ChuaQuyDoi = $(".input-sotiendadong", $thanhToan).val(),
                                                    idDonViTien = $(".select-donvitien", $thanhToan).data("iddonvitien"),
                                                    soTienDaDong_SauQuyDoi = $(".input-sotiendadong-sauquydoi", $thanhToan).val(),
                                                    phanTramDaDong = $(".input-phantramdadong", $thanhToan).val().replace('%', '');
                                                var thanhToan = {
                                                    IdThanhToan: idThanhToan,
                                                    IdDonHang: idDonHang,
                                                    IdSanPham: idSanPham,
                                                    IdKhachHang: idKhachHang,
                                                    ThuTuThanhToan: thuTuThanhToan,
                                                    SoTienDaDong_ChuaQuyDoi: soTienDaDong_ChuaQuyDoi,
                                                    IdDonViTien: idDonViTien,
                                                    SoTienDaDong: soTienDaDong_SauQuyDoi,
                                                    PhanTramDaDong: phanTramDaDong,
                                                    //NgayTao = moment() // Không cần vì nếu có rồi sẽ không bị cập nhật và nếu chưa có thì sẽ lấy Datetiem.Now bên Controller
                                                };
                                                // Thêm thanh toán vào đơn hàng
                                                donHang.ThanhToans.push(thanhToan);
                                            });
                                            // Thêm đơn hàng vào khách hàng
                                            khachHang.DonHangs.push(donHang);
                                        };
                                    };
                                });
                            };
                            $.ajax({
                                ...ajaxDefaultProps({
                                    url: loai == "create" ? "/QuanLyKhachHang/create_KhachHang" : loai == "update" ? "/QuanLyKhachHang/_update_KhachHang" : "/QuanLyKhachHang/update_KhachHang_DonHang",
                                    type: "POST",
                                    data: {
                                        str_khachHang: JSON.stringify(khachHang),
                                    }
                                }),
                                success: function (res) {
                                    if (res.status == "success") {
                                        quanLyKhachHang.khachHang.dataTable.ajax.reload(function () {
                                            if (res.loaiThemMoi == "daydu") {
                                                quanLyKhachHang.khachHang.thongBaoTienDoDoanhThu({
                                                    nguoiDung_DoanhThu: res.nguoiDung_DoanhThu,
                                                    nguoiDung_DoanhThus_NHOM: res.nguoiDung_DoanhThus_NHOM
                                                });
                                            };

                                            sys.displayModal({
                                                name: '#quanlykhachhang-crud',
                                                displayStatus: "hide"
                                            });
                                        }, false);
                                    } else {
                                        if (res.status == "warning") {
                                            htmlEl.inputValidationStates(
                                                $("#input-tenkhachhang"),
                                                "#quanlykhachhang-crud",
                                                res.mess,
                                                {
                                                    status: true,
                                                    isvalid: false
                                                }
                                            );
                                            htmlEl.inputValidationStates(
                                                $("#input-email"),
                                                "#quanlykhachhang-crud",
                                                res.mess,
                                                {
                                                    status: true,
                                                    isvalid: false
                                                }
                                            );
                                            // Hiển thị và gán hàm cập nhật
                                            $("#update-container", $("#quanlykhachhang-crud")).show();
                                            $("#btn-update", $("#quanlykhachhang-crud")).off().on("click", function () {
                                                quanLyKhachHang.khachHang.displayModal_CRUD('update', res.khachHang_OLD.IdKhachHang);
                                            });
                                            $("#btn-update-donhang", $("#quanlykhachhang-crud")).off().on("click", function () {
                                                quanLyKhachHang.khachHang.displayModal_CRUD('update-donhang', res.khachHang_OLD.IdKhachHang);
                                            });
                                        };
                                    };
                                    sys.alert({ status: res.status, mess: res.mess, timeout: 6000 });
                                }
                            });
                        }
                    });
                };
            },
            delete: function (loai, idKhachHang = 0) {
                var idKhachHangs = [];
                if (loai == "single") {
                    idKhachHangs.push(idKhachHang);
                } else {
                    quanLyKhachHang.khachHang.dataTable.rows().iterator('row', function (context, index) {
                        var $row = $(this.row(index).node());
                        if ($row.has("input.checkRow-quanlykhachhang-getList:checked").length > 0) {
                            idKhachHangs.push($row.attr('id'));
                        };
                    });
                };
                // Kiểm tra idKhachHang
                if (idKhachHangs.length > 0) {
                    var f = new FormData();
                    f.append("str_idKhachHangs", idKhachHangs.toString());
                    sys.confirmDialog({
                        mess: `
                        <p class="font-bold">Khách hàng có liên kết với các
                            <span class="text-danger fst-italic"> [Lớp học]</span> và
                            <span class="text-danger fst-italic"> [Doanh thu]</span> 
                        </p>
                        <p>Bạn có thực sự muốn xóa bản ghi này hay không ?</p>
                        `,
                        callback: function () {
                            $.ajax({
                                ...ajaxDefaultProps({
                                    url: "/QuanLyKhachHang/delete_KhachHangs",
                                    type: "POST",
                                    data: f,
                                }),
                                contentType: false,
                                processData: false,
                                success: function (res) {
                                    quanLyKhachHang.khachHang.dataTable.ajax.reload(function () {
                                        sys.alert({ status: res.status, mess: res.mess })
                                    }, false);
                                }
                            })
                        }
                    });
                } else {
                    sys.alert({ mess: "Bạn chưa chọn bản ghi nào", status: "warning", timeout: 1500 });
                };
            },
        };
        quanLyKhachHang.locThongTin = {
            ...quanLyKhachHang.locThongTin,
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
                quanLyKhachHang.locThongTin.data = {
                    IdCoCauToChucs: quanLyKhachHang.coCauToChuc.selected,
                    NgayTao: $("#input-ngaytao", $("#modal-timkiem")).val(),
                    TenKhachHang: $("#input-tenkhachhang", $("#modal-timkiem")).val(),
                    TenNhanVien: $("#input-tennhanvien", $("#modal-timkiem")).val(),
                    Email: $("#input-email", $("#modal-timkiem")).val(),
                    IdLoaiKhachHang: $("#select-loaikhachhang", $("#modal-timkiem")).val(),
                    IdGoiChamSoc: $("#select-goichamsoc", $("#modal-timkiem")).val(),
                };
                await quanLyKhachHang.khachHang.dataTable.ajax.reload();
                await quanLyKhachHang.locThongTin.displayModal_TimKiem('hide');
            },
        };
        quanLyKhachHang.coCauToChuc = {
            ...quanLyKhachHang.coCauToChuc,
            getList: function () {
                $.ajax({
                    ...ajaxDefaultProps({
                        url: "/QuanLyKhachHang/get_coCauToChucs",
                        type: "GET",
                    }),
                    success: function (res) {
                        if (res.length > 0) {
                            var MAKETREEDATA = (coCaus) => {
                                var nodes = [];
                                $.each(coCaus, function (iCoCau, coCau) {
                                    let _coCau = {
                                        name: `${coCau.root.TenCoCauToChuc}`,
                                        value: coCau.root.IdCoCauToChuc,
                                        //children: [] // Nếu có thì mới thêm vì sẽ mặc định hiện icon collapse nếu có thuộc tính này
                                    }
                                    if (coCau.nodes.length > 0) _coCau.children = MAKETREEDATA(coCau.nodes);
                                    nodes.push(_coCau);
                                });
                                return nodes;
                            };
                            quanLyKhachHang.coCauToChuc.data = res;
                            quanLyKhachHang.coCauToChuc.treeSelect = new TreeSelectCustom({
                                props: {
                                    parentHtmlContainer: document.getElementById("treeSelect_cocautochuc_container"),
                                    isIndependentNodes: true, // Không gộp phần tử con thành phần tử cha khi chọn tất cả
                                    listClassName: 'treeselect-list-item',
                                    placeholder: 'Nhập thông tin tìm kiếm ...',
                                    options: MAKETREEDATA(quanLyKhachHang.coCauToChuc.data),
                                    id: 'treeSelect_cocautochuc',
                                    alwaysOpen: false,
                                    //value: options,
                                    inputCallback: function (value) {
                                        quanLyKhachHang.coCauToChuc.selected = value; // Thêm vào danh sách
                                        /*quanLyKhachHang.nguoiDung.dataTable.ajax.reload();*/
                                    },
                                }
                            }).init();
                        }
                    }
                })
            },
        };
        quanLyKhachHang.coCauToChuc.getList();
        sys.activePage({
            page: quanLyKhachHang.page.attr("id"),
            pageGroup: quanLyKhachHang.pageGroup
        });
    }
    create_LichSu() {
        var quanLyKhachHang = this;
        //var containerHeight = $("#lichsu-getList-container").height() - 10;
        //$("#lichsu-read-container", $("#quanlykhachhang-lichsu")).height(containerHeight);
        quanLyKhachHang.khachHang_lichSu = {
            dataTable: new DataTableCustom({
                name: "lichsu-getList",
                table: $("#lichsu-getList"),
                props: {
                    //maxHeight: containerHeight,
                    dom: `
                    <'row'<'col-sm-12 col-md-6'>>
                    <'row'<'col-sm-12'rt>>
                    <'row'<'col-sm-12 col-md-4 pt-2'l><'col-sm-12 col-md-4 text-center'i><'col-sm-12 col-md-4 pt-2'p>>`,
                    lengthMenu: [
                        [10, 50, -1],
                        [10, 50, 'Tất cả'],
                    ],
                }
            }).dataTable,
            readRow: function (el) {
                var rowNumber = $(el).attr("row"),
                    rows = quanLyKhachHang.khachHang_lichSu.dataTable.rows().nodes().toArray(),
                    $divs = $(".lichsu-read", $("#lichsu-getList-container")),
                    $div = $(`.lichsu-read[row=${rowNumber}]`, $("#lichsu-getList-container"));
                $divs.hide(); $div.show();
                $.each(rows, function () {
                    if ($(this).attr("row") == rowNumber) {
                        $(this).css({
                            "background-color": "var(--bs-table-hover-bg)",
                        })
                    } else {
                        $(this).css({
                            "background-color": "transparent",
                        })
                    };
                });
            },
        };
    }
};

class QuanLyKhachHangController {
    constructor() {
        this.quanLyKhachHangService = new QuanLyKhachHangService;
    }
    init() {
        var quanLyKhachHangController = this;
        quanLyKhachHangController.quanLyKhachHangService.init();
    }
}