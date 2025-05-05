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
        this.locThongTin = {
            displayModal_TimKiem: function () { },
            reload: function () { },
            timKiem: function () { },
        }
        this.khachHang = {
            displayModal_KhachHang_XemChiTiet: function () { },
            chonSanPham: function () { },
            capNhatSoTienDaDong: function () { },
        }
        this.lopHoc = {
            choXepLop: {
                dataTable: null,
                getList: function () { },
            },
            daXepLop: {
                dataTable: null,
                getList: function () { },
            },
            lichHoc: {
                dataTable: null,
                buoiHoc: {
                    dataTable: null,
                }
            }
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
        htmlEl.select2Mask("#choxeplop-container");
        htmlEl.select2Mask("#daxeplop-container");

        quanLyLopHoc.khachHang = {
            ...quanLyLopHoc.khachHang,
            displayModal_KhachHang_XemChiTiet: function (idKhachHang = '00000000-0000-0000-0000-000000000000') {
                if (idKhachHang == '00000000-0000-0000-0000-000000000000') {
                    var idKhachHangs = [];
                    quanLyLopHoc.lopHoc.lichHoc.donHang.dataTable.rows().iterator('row', function (context, index) {
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
        };
        quanLyLopHoc.lopHoc = {
            ...quanLyLopHoc.lopHoc,
            dataInUsed: {
                LopHoc: { IdLopHoc: '00000000-0000-0000-0000-000000000000', }
            },
            choXepLop: {
                locThongTin: {
                    data: null,
                    reload: function () {
                        quanLyLopHoc.lopHoc.choXepLop.locThongTin.data = {
                            ThoiGian: $("#input-thoigian", $("#choxeplop-container")).val(),
                            IdLoaiSanPham: $("#select-loaisanpham", $("#choxeplop-container")).val(),
                            IdSanPham: $("#select-sanpham", $("#choxeplop-container")).val(),

                            IdGiaoVien: $("#select-giaovien", $("#choxeplop-container")).val(),
                            IdKhachHang: $("#select-khachhang", $("#choxeplop-container")).val(),
                            IdTrinhDoDauVao: $("#select-trinhdodauvao", $("#choxeplop-container")).val(),
                            IdTrinhDoDauRa: $("#select-trinhdodaura", $("#choxeplop-container")).val(),
                        };
                    },
                    timKiem: function () {
                        quanLyLopHoc.lopHoc.choXepLop.getList();
                    },
                },
                dataTable: null,
                getList: function () {
                    quanLyLopHoc.lopHoc.choXepLop.locThongTin.reload();

                    $.ajax({
                        ...ajaxDefaultProps({
                            url: "/QuanLyLopHoc/getList_ChoXepLop",
                            type: "POST", // Phải là POST để gửi JSON
                            //contentType: "application/json; charset=utf-8",  // Định dạng JSON
                            data: { locThongTin: quanLyLopHoc.lopHoc.choXepLop.locThongTin.data }
                            //dataType: "json",
                        }),
                        //contentType: false,
                        //processData: false,
                        success: function (res) {
                            $("#choxeplop-getlist-container").html(res);
                            quanLyLopHoc.lopHoc.choXepLop.dataTable = new DataTableCustom({
                                name: "choxeplop-getList",
                                table: $("#choxeplop-getList"),
                                props: {
                                    lengthMenu: [
                                        [2, 5],
                                        [2, 5],
                                    ],
                                }
                            }).dataTable;
                        }
                    });
                },
            },
            daXepLop: {
                locThongTin: {
                    data: null,
                    reload: function () {
                        quanLyLopHoc.lopHoc.daXepLop.locThongTin.data = {
                            ThoiGian: $("#input-thoigian", $("#daxeplop-container")).val(),
                            TenLopHoc: $("#input-tenlophoc", $("#daxeplop-container")).val(),
                            IdLopHoc_TrangThaiHoc: $("#select-lophoc-trangthaihoc", $("#daxeplop-container")).val(),
                            IdLoaiSanPham: $("#select-loaisanpham", $("#daxeplop-container")).val(),
                            IdSanPham: $("#select-sanpham", $("#daxeplop-container")).val(),

                            IdGiaoVien: $("#select-giaovien", $("#daxeplop-container")).val(),
                            IdKhachHang: $("#select-khachhang", $("#daxeplop-container")).val(),
                            IdTrinhDoDauVao: $("#select-trinhdodauvao", $("#daxeplop-container")).val(),
                            IdTrinhDoDauRa: $("#select-trinhdodaura", $("#daxeplop-container")).val(),
                        };
                    },
                    timKiem: function () {
                        quanLyLopHoc.lopHoc.daXepLop.getList();
                    },
                },
                dataTable: null,
                getList: function () {
                    quanLyLopHoc.lopHoc.daXepLop.locThongTin.reload();

                    $.ajax({
                        ...ajaxDefaultProps({
                            url: "/QuanLyLopHoc/getList_DaXepLop",
                            type: "POST", // Phải là POST để gửi JSON
                            //contentType: "application/json; charset=utf-8",  // Định dạng JSON
                            data: { locThongTin: quanLyLopHoc.lopHoc.daXepLop.locThongTin.data }
                            //dataType: "json",
                        }),
                        success: function (res) {
                            $("#daxeplop-getlist-container").html(res);
                            quanLyLopHoc.lopHoc.daXepLop.dataTable = new DataTableCustom({
                                name: "daxeplop-getList",
                                table: $("#daxeplop-getList"),
                                props: {
                                    lengthMenu: [
                                        [2, 5],
                                        [2, 5],
                                    ],
                                }
                            }).dataTable;
                        }
                    });
                },
            },

            giaoVien: {
                dataTable: null
            },
            donHang: {
                dataTable: null
            },
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

            tachLopHoc: function () { },
            dungLopHoc: function () { },
            lichHoc: {
                dataTable: null,
                donHang: {
                    dataTable: null,
                    chonDonHang: function () {
                        let tenKhachHangs = [],
                            idDonHangs = [],
                            idKhachHangs = [];
                        let soBuois = [];
                        quanLyLopHoc.lopHoc.lichHoc.donHang.dataTable.rows().iterator('row', function (context, index) {
                            let $row = $(this.row(index).node());
                            let $cb = $("input.checkRow-donhang-getList", $row);
                            if ($cb.is(":checked")) {
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
                        let tenKhachHang_DeXuat = tenKhachHangs.join(", "),
                            idDonHang_DeXuat = idDonHangs.join(","),
                            idKhachHang_DeXuat = idKhachHangs.join(",");
                        let soBuoi_DeXuat = soBuois.reduce((p, c) => p + c, 0);
                        $("#input-tenkhachhang", $("#quanlylophoc-lichhoc-taolichhoc")).val(tenKhachHang_DeXuat);
                        $("#input-tenkhachhang", $("#quanlylophoc-lichhoc-taolichhoc")).data("iddonhang", idDonHang_DeXuat);
                        $("#input-tenkhachhang", $("#quanlylophoc-lichhoc-taolichhoc")).data("idkhachhang", idKhachHang_DeXuat);
                        $("#input-tenkhachhang", $("#quanlylophoc-lichhoc-taolichhoc")).trigger("change");

                        $("#input-sobuoi-dexuat", $("#quanlylophoc-lichhoc-taolichhoc")).text(soBuoi_DeXuat);
                        $("#input-sobuoi-dexuat", $("#quanlylophoc-lichhoc-taolichhoc")).text(soBuoi_DeXuat);
                    },
                },
                buoiHoc: {
                    dataInUsed: {
                        BuoiHoc: { IdLopHoc_BuoiHoc: '00000000-0000-0000-0000-000000000000', },
                        HinhAnhBuoiHocs: [],
                    },
                    anhBuoiHocViewerJS: null,
                    displayModal_XemBuoiHoc: function (loai, idBuoiHoc = '00000000-0000-0000-0000-000000000000') {
                        $.ajax({
                            ...ajaxDefaultProps({
                                url: "/QuanLyLopHoc/displayModal_XemBuoiHoc",
                                type: "POST",
                                data: {
                                    loai, idBuoiHoc
                                },
                            }),
                            //contentType: false,
                            //processData: false,
                            success: function (res) {
                                $("#quanlylophoc-buoihoc-crud").html(res);
                                quanLyLopHoc.lopHoc.lichHoc.buoiHoc.dataInUsed = {
                                    BuoiHoc: {
                                        IdLopHoc_BuoiHoc: $("#input-idbuoihoc").val(),
                                    },
                                    HinhAnhBuoiHocs: [],
                                };

                                quanLyLopHoc.lopHoc.lichHoc.buoiHoc.taoAnhBuoiHocViewerJS();

                                sys.displayModal({
                                    name: '#quanlylophoc-buoihoc-crud',
                                    level: 3
                                });
                            }
                        });
                    },
                    themAnhBuoiHoc: function () {
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
                                                       onclick="quanLyLopHoc.lopHoc.lichHoc.buoiHoc.xoaAnhBuoiHoc(this)">
                                                        <i class="bi bi-trash3-fill"></i>
                                                    </a>
                                                </td>
                                            </tr>`;
                                            $tbodyAnhBuoiHoc.prepend(html); // Thêm danh sách ảnh
                                            // Lưu thông tin ảnh vào buổi học
                                            quanLyLopHoc.lopHoc.lichHoc.buoiHoc.dataInUsed.HinhAnhBuoiHocs.push(hinhAnhBuoiHoc);

                                        } else {
                                            status = "warning";
                                            mess = `Chỉ nhận tối đa ${soAnhToiDa} ảnh`;
                                        };
                                    });
                                }).then(function () {
                                    quanLyLopHoc.lopHoc.lichHoc.buoiHoc.taoAnhBuoiHocViewerJS();
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
                    xoaAnhBuoiHoc: function (e) {
                        var $tr = $(e).closest("tr");
                        $tr.remove(); // Xóa phần tử hiện tại
                        quanLyLopHoc.lopHoc.lichHoc.buoiHoc.anhBuoiHocViewerJS.update(); // Cập nhật viewer
                    },
                    taoAnhBuoiHocViewerJS: function () {
                        // Destroy viewer cũ rồi mới gán lại được để không bị trùng
                        if (quanLyLopHoc.lopHoc.lichHoc.buoiHoc.anhBuoiHocViewerJS != null)
                            quanLyLopHoc.lopHoc.lichHoc.buoiHoc.anhBuoiHocViewerJS.destroy();

                        quanLyLopHoc.lopHoc.lichHoc.buoiHoc.anhBuoiHocViewerJS = new Viewer($("#tbody-anhbuoihoc")[0], {
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
                                quanLyLopHoc.lopHoc.lichHoc.buoiHoc.taoAnhBuoiHocViewerJS();
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
                            quanLyLopHoc.lopHoc.lichHoc.buoiHoc.anhBuoiHocViewerJS.view($tr.index()); // Hiển thị ảnh tương ứng trong Viewer.js
                        });
                    },

                    diemDanh: function () {
                        quanLyLopHoc.lopHoc.lichHoc.buoiHoc.dataInUsed.BuoiHoc = {
                            IdLopHoc_BuoiHoc: $("#input-idbuoihoc", $("#quanlylophoc-buoihoc-crud")).val(),
                            GhiChu: $("#input-ghichu", $("#quanlylophoc-buoihoc-crud")).val().trim(),
                            DiemDanh: $("#select-diemdanh", $("#quanlylophoc-buoihoc-crud")).val(),
                        }; // HinhAnhBuoiHocs đã thuộc dataInUsed

                        var modalValidtion = htmlEl.activeValidationStates("#quanlylophoc-buoihoc-crud");
                        if (modalValidtion) {
                            var f = new FormData();
                            f.append("buoiHoc", JSON.stringify(quanLyLopHoc.lopHoc.lichHoc.buoiHoc.dataInUsed));

                            sys.confirmDialog({
                                mess: `<p>Bạn xác nhận muốn lưu bản ghi này không ?</p>`,
                                callback: function () {
                                    $.ajax({
                                        ...ajaxDefaultProps({
                                            url: "/QuanLyLopHoc/diemDanh",
                                            type: "POST",
                                            data: f,
                                        }),
                                        contentType: false,
                                        processData: false,
                                        success: function (res) {
                                            if (res.status == "success") {
                                                quanLyLopHoc.buoiHocSapToi.getList();
                                                quanLyLopHoc.lopHocThamGia.getList();
                                                sys.displayModal({
                                                    name: '#quanlylophoc-buoihoc-crud',
                                                    displayStatus: "hide"
                                                });
                                            };
                                            sys.alert({ status: res.status, mess: res.mess, timeout: 6000 });
                                        }
                                    })
                                }
                            });
                        };
                    },
                    capNhat: function () {
                        quanLyLopHoc.lopHoc.lichHoc.buoiHoc.dataInUsed.BuoiHoc = {
                            IdLopHoc_BuoiHoc: $("#input-idbuoihoc", $("#quanlylophoc-buoihoc-crud")).val(),
                            GhiChu: $("#input-ghichu", $("#quanlylophoc-buoihoc-crud")).val().trim(),
                            DiemDanh: $("#select-diemdanh", $("#quanlylophoc-buoihoc-crud")).val(),
                        }; // HinhAnhBuoiHocs đã thuộc dataInUsed

                        var modalValidtion = htmlEl.activeValidationStates("#quanlylophoc-buoihoc-crud");
                        if (modalValidtion) {
                            var f = new FormData();
                            f.append("buoiHocs", JSON.stringify(quanLyLopHoc.lopHoc.lichHoc.buoiHoc.dataInUsed));

                            sys.confirmDialog({
                                mess: `<p>Bạn xác nhận muốn lưu bản ghi này không ?</p>`,
                                callback: function () {
                                    $.ajax({
                                        ...ajaxDefaultProps({
                                            url: "/QuanLyLopHoc/capNhatBuoiHoc",
                                            type: "POST",
                                            data: f,
                                        }),
                                        contentType: false,
                                        processData: false,
                                        success: function (res) {
                                            if (res.status == "success") {
                                                quanLyLopHoc.lopHocThamGia.dataTable.ajax.reload();
                                                quanLyLopHoc.buoiHocSapToi.dataTable.ajax.reload(function () {
                                                    sys.displayModal({
                                                        name: '#quanlylophoc-buoihoc-crud',
                                                        displayStatus: "hide"
                                                    });
                                                }, false);
                                            };
                                            sys.alert({ status: res.status, mess: res.mess, timeout: 6000 });
                                        }
                                    })
                                }
                            });
                        };
                    },
                    save: function (loai) {
                        if (loai == 'diemdanh') {
                            quanLyLopHoc.lopHoc.lichHoc.buoiHoc.diemDanh();
                        } else {
                            quanLyLopHoc.lopHoc.lichHoc.buoiHoc.capNhat();
                        }
                    },
                    capNhat: function (idBuoiHoc = '00000000-0000-0000-0000-000000000000') {
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


                        sys.alert({ mess: "Cập nhật thành công thông tin buổi học", status: "success", timeout: 1500 });
                        sys.displayModal({
                            name: '#quanlylophoc-buoihoc-crud',
                            level: 2,
                            displayStatus: "hide"
                        });
                    }
                },
                displayModal_XemLichHoc: function (loai = "", idLopHoc = '00000000-0000-0000-0000-000000000000') {
                    // create - lấy idLopHoc theo param
                    if (idLopHoc == '00000000-0000-0000-0000-000000000000') {
                        var idLopHocs = [];
                        quanLyLopHoc.lopHoc.daXepLop.dataTable.rows().iterator('row', function (context, index) {
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
                            url: "/QuanLyLopHoc/displayModal_XemLichHoc",
                            type: "POST",
                            data: { loai, idLopHoc }
                        }),
                        success: function (res) {
                            $("#quanlylophoc-lichhoc-xemlichhoc").html(res);
                            quanLyLopHoc.lopHoc.dataInUsed.LopHoc.IdLopHoc = $("#input-idlophoc").val();

                            // Tạo dataTable cho lịch học
                            quanLyLopHoc.lopHoc.lichHoc.dataTable = new DataTableCustom({
                                name: "lichhoc-getList",
                                table: $("#lichhoc-getList", $("#quanlylophoc-lichhoc-xemlichhoc")),
                                props: {
                                    //maxHeight: 300,
                                    dom: `
                                    <'row'<'col-sm-12'rt>>
                                    <'row'<'col-sm-12 col-md-6 text-left pt-2'l><'col-sm-12 col-md-6 pt-2'p>>`,
                                    lengthMenu: [
                                        [5],
                                        [5],
                                    ],
                                }
                            }).dataTable;

                            sys.displayModal({
                                name: "#quanlylophoc-lichhoc-xemlichhoc",
                            });
                        }
                    })
                },
                displayModal_TaoLichHoc: function (idLopHoc = '00000000-0000-0000-0000-000000000000') {
                    $.ajax({
                        ...ajaxDefaultProps({
                            url: "/QuanLyLopHoc/displayModal_TaoLichHoc",
                            type: "GET",
                            data: {
                                idLopHoc: idLopHoc
                            }
                        }),
                        success: function (res) {
                            $("#quanlylophoc-lichhoc-taolichhoc").html(res);

                            var ngayHienTai = new Date();

                            var $table_LichHoc = $("#table-lichhoc", $("#quanlylophoc-lichhoc-taolichhoc")),
                                $tbody_LichHoc = $("tbody.thongtin-buoihoc", $table_LichHoc);

                            $.each($tbody_LichHoc, function (iBuoiHoc, $buoiHoc) {
                                $(".input-thoigianbatdau", $buoiHoc).attr("min", ngayHienTai.toISOString().split('.')[0]);
                            });
                            // Tạo dataTable cho đơn hàng
                            quanLyLopHoc.lopHoc.lichHoc.donHang.dataTable = new DataTableCustom({
                                name: "donhang-getList",
                                table: $("#donhang-getList", $("#quanlylophoc-lichhoc-taolichhoc")),
                                props: {
                                    dom: `
                                <'row'<'col-sm-12'rt>>
                                <'row'<'col-sm-12 col-md-6 text-left'i><'col-sm-12 col-md-6 pt-2'p>>`,
                                    lengthMenu: [
                                        [5, 10],
                                        [5, 10],
                                    ],
                                }
                            }).dataTable;

                            sys.displayModal({
                                name: "#quanlylophoc-lichhoc-taolichhoc",
                                level: 2
                            });
                        }
                    })
                },

                checkTaoLichHocTuDong: function () {
                    var trangThai = $("#checkbox-chedocapnhatlichhoc", $("#quanlylophoc-lichhoc-taolichhoc")).is(":checked");
                    return trangThai;
                },
                switchTaoLichHocTuDong: function () {
                    $("#input-sobuoi-container", $("#quanlylophoc-lichhoc-taolichhoc")).hide();

                    if (quanLyLopHoc.lopHoc.lichHoc.checkTaoLichHocTuDong()) {
                        $("#input-sobuoi-container", $("#quanlylophoc-lichhoc-taolichhoc")).show();
                    };
                },

                themDongBuoiHoc: function (e) {
                    var $table_LichHoc = $(e).closest("table#table-lichhoc"),
                        $tbody_LichHoc = $("tbody.thongtin-buoihoc", $table_LichHoc),
                        $banMau = $($tbody_LichHoc[0]);
                    // Thêm 1 buổi
                    $("thead", $table_LichHoc).after(`<tbody class="thongtin-buoihoc">${$banMau.html()}<tbody>`);
                },
                xoaDongBuoiHoc: function (e) {
                    var $table_LichHoc = $(e).closest("table#table-lichhoc"),
                        $tbody_LichHoc = $("tbody.thongtin-buoihoc", $table_LichHoc),
                        $_this = $(e).closest("tbody.thongtin-buoihoc");
                    if ($tbody_LichHoc.length > 1) {
                        $_this.remove();
                    }
                },

                layLichHoc: function (idLopHoc = '00000000-0000-0000-0000-000000000000') {
                    $.ajax({
                        ...ajaxDefaultProps({
                            url: "/QuanLyLopHoc/layLichHoc",
                            type: "GET",
                            data: {
                                idLopHoc: idLopHoc
                            },
                        }),
                        success: function (res) {
                            $("#lichhoc-container").html(res);
                        }
                    });
                },

                taoBuoiHoc: function (idLopHoc = '00000000-0000-0000-0000-000000000000') {
                    function taoTuDong({ buoiHocs = [], tongSoBuoi = 0 }) {
                        var thuTuTuan = 0; // Đếm số tuần đã thêm
                        var thuTuBuoiHoc = 0;
                        var buoiHocs_NEW = [];
                        while ((buoiHocs_NEW.length < tongSoBuoi) && buoiHocs.length > 0) {
                            buoiHocs.forEach((_buoiHoc, iBuoiHoc) => {
                                let buoiHoc = _buoiHoc.BuoiHoc;
                                thuTuBuoiHoc++; // Tăng thứ tự lên
                                // Thêm số tuần
                                let thoiGianBatDau = new Date(buoiHoc.ThoiGianBatDau);
                                thoiGianBatDau.setDate(thoiGianBatDau.getDate() + (7 * thuTuTuan));
                                thoiGianBatDau = sys.chuyenThoiGianDungLocalVN(thoiGianBatDau);
                                let thoiGianKetThuc = new Date(thoiGianBatDau.getTime() + buoiHoc.ThoiLuong * 60000); // ThoiLuong tính bằng phút

                                let buoiHoc_NEW = {
                                    BuoiHoc: {
                                        ...buoiHoc,
                                        ThuTuBuoiHoc: thuTuBuoiHoc,
                                        ThoiGianBatDau: thoiGianBatDau.toISOString(), // ISO string theo giờ địa phương
                                        ThoiGianKetThuc: thoiGianKetThuc.toISOString(), // ISO string theo giờ địa phương
                                    },
                                    HinhAnhBuoiHocs: []
                                };
                                buoiHocs_NEW.length < tongSoBuoi && buoiHocs_NEW.push(buoiHoc_NEW);
                            });

                            thuTuTuan++;
                        };
                        //return buoiHocs_NEW.slice(0, tongSoBuoi); // Chỉ lấy đúng tổng số buổi
                        return buoiHocs_NEW; // Chỉ lấy đúng tổng số buổi
                    };
                    function layThongTin() {
                        let buoiHocs = [];
                        var tongSoBuoi = 0;

                        var $table_LichHoc = $("#table-lichhoc", $("#quanlylophoc-lichhoc-taolichhoc")),
                            $tbody_LichHoc = $("tbody.thongtin-buoihoc", $table_LichHoc);
                        var idGiaoViens = $("#select-giaovien", $("#quanlylophoc-lichhoc-taolichhoc")).val().join(","),
                            idKhachHangs = $("#input-tenkhachhang", $("#quanlylophoc-lichhoc-taolichhoc")).data("idkhachhang"),
                            idDonHangs = $("#input-tenkhachhang", $("#quanlylophoc-lichhoc-taolichhoc")).data("iddonhang"),
                            luongTheoBuoi = $("#input-luongtheobuoi", $("#quanlylophoc-lichhoc-taolichhoc")).val().replaceAll(' ', '');
                        // Dữ liệu ban đầu
                        $.each($tbody_LichHoc, function (iBuoiHoc, $buoiHoc) {
                            let buoiHoc = {
                                BuoiHoc: {
                                    IdLopHoc_BuoiHoc: '00000000-0000-0000-0000-000000000000',
                                    IdLopHoc: idLopHoc,
                                    LuongTheoBuoi: luongTheoBuoi,
                                    IdGiaoVien: `,${idGiaoViens}`,
                                    IdKhachHang: `,${idKhachHangs}`,
                                    IdDonHang: `,${idDonHangs}`,
                                    ThuTuBuoiHoc: 0, // Sẽ thay đổi khi tạo events

                                    ThoiGianBatDau: $(".input-thoigianbatdau", $buoiHoc).val() ?? "",
                                    ThoiLuong: $(".input-thoiluong", $buoiHoc).val() ?? "",
                                    DiemDanh: 0,
                                    GhiChu: "",
                                },
                                HinhAnhBuoiHocs: []
                            };
                            if (buoiHoc.ThoiGianBatDau != "" && buoiHoc.ThoiLuong != "") buoiHocs.push(buoiHoc);
                        });

                        if (buoiHocs.length > 0) {
                            // Sắp xếp các sự kiện theo thời gian bắt đầu (ThoiGianBatDau)
                            buoiHocs.sort((a, b) => new Date(a.ThoiGianBatDau) - new Date(b.ThoiGianBatDau));
                            tongSoBuoi = buoiHocs.length;

                            // Lựa chọn chế độ cập nhật lịch học
                            if (quanLyLopHoc.lopHoc.lichHoc.checkTaoLichHocTuDong()) {
                                let _tongSoBuoi = $("#input-sobuoi", $("#quanlylophoc-lichhoc-taolichhoc")).val();
                                if (_tongSoBuoi == "") {
                                    sys.alert({ status: "warning", mess: "Nhập tổng số buổi" });
                                    return;
                                };
                                tongSoBuoi = parseInt(_tongSoBuoi);
                            };

                            // Tạo tự động cho đến khi đủ buổi
                            buoiHocs = taoTuDong({ buoiHocs, tongSoBuoi: tongSoBuoi })
                        };

                        return buoiHocs;
                    };

                    var modalValidtion = htmlEl.activeValidationStates("#quanlylophoc-lichhoc-taolichhoc");
                    if (modalValidtion) {
                        var buoiHocs = layThongTin();

                        var f = new FormData();
                        f.append("idLopHoc", idLopHoc);
                        f.append("buoiHocs", JSON.stringify(buoiHocs));

                        sys.confirmDialog({
                            mess: `Bạn có thực sự muốn thêm bản ghi này hay không ?`,
                            callback: function () {
                                $.ajax({
                                    ...ajaxDefaultProps({
                                        url: "/QuanLyLopHoc/taoBuoiHoc",
                                        type: "POST",
                                        data: f,
                                    }),
                                    contentType: false,
                                    processData: false,
                                    success: function (res) {
                                        if (res.status == "success") {
                                            quanLyLopHoc.lopHoc.lichHoc.displayModal_XemLichHoc('update', idLopHoc);

                                            sys.displayModal({
                                                name: "#quanlylophoc-lichhoc-taolichhoc",
                                                displayStatus: "hide"
                                            });
                                        };
                                        sys.alert({ status: res.status, mess: res.mess, timeout: 6000 });
                                    }
                                })
                            }
                        });
                    };
                },
                capNhatBuoiHoc: function () { },
                xoaBuoiHoc: function (loai, idBuoiHoc = '00000000-0000-0000-0000-000000000000') {
                    var idLopHoc = $("#input-idlophoc").val();
                    var idBuoiHocs = [];
                    if (loai == "single") {
                        idBuoiHocs.push(idBuoiHoc);
                    } else {
                        quanLyLopHoc.lopHoc.lichHoc.dataTable.rows().iterator('row', function (context, index) {
                            var $row = $(this.row(index).node());
                            if ($row.has("input.checkRow-lichhoc-getList:checked").length > 0) {
                                idBuoiHocs.push($row.attr('id'));
                            };
                        });
                    };
                    // Kiểm tra idLopHoc
                    if (idBuoiHocs.length > 0) {
                        var f = new FormData();
                        f.append("idLopHoc", idLopHoc);
                        f.append("idBuoiHocs", JSON.stringify(idBuoiHocs));
                        sys.confirmDialog({
                            mess: `Bạn có thực sự muốn xóa bản ghi này hay không ?`,
                            callback: function () {
                                $.ajax({
                                    ...ajaxDefaultProps({
                                        url: "/QuanLyLopHoc/xoaBuoiHoc",
                                        type: "POST",
                                        data: f,
                                    }),
                                    contentType: false,
                                    processData: false,
                                    success: function (res) {
                                        if (res.status == "success") {
                                            quanLyLopHoc.lopHoc.lichHoc.displayModal_XemLichHoc('update', idLopHoc);
                                        };
                                        sys.alert({ status: res.status, mess: res.mess, timeout: 6000 });
                                    }
                                })
                            }
                        })
                    } else {
                        sys.alert({ mess: "Bạn chưa chọn bản ghi nào", status: "warning", timeout: 1500 });
                    };
                },
            },
            displayModal_CRUD_LopHoc: function (loai = "", idLopHoc = '00000000-0000-0000-0000-000000000000') {
                if (loai == "update") {
                    var idLopHocs = [];
                    quanLyLopHoc.lopHoc.daXepLop.dataTable.rows().iterator('row', function (context, index) {
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
                        $("#quanlylophoc-crud").html(res);

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
                        quanLyLopHoc.lopHoc.giaoVien.dataTable = new DataTableCustom({
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
                        quanLyLopHoc.lopHoc.donHang.dataTable = new DataTableCustom({
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

                    sys.confirmDialog({
                        mess: `
                                <p class="font-bold">
                                    Kiểm tra kỹ <span class="text-danger fst-italic"> thông tin lớp học</span> <br />
                                </p>
                                <p class="font-bold">Không thể chỉnh sửa sau khi lưu <br />
                                    <span class="text-danger fst-italic"> Hãy liên hệ quản trị viên nếu cần hỗ trợ cập nhật</span>
                                </p>
                                <p>Bạn có thực sự muốn thêm bản ghi này hay không ?</p>
                                `,
                        callback: function () {
                            var f = new FormData();
                            f.append("lopHoc", JSON.stringify(lopHoc));
                            f.append("loai", loai);

                            $.ajax({
                                ...ajaxDefaultProps({
                                    url: loai == "create" ? "/QuanLyLopHoc/create_LopHoc" : "/QuanLyLopHoc/update_LopHoc",
                                    type: "POST",
                                    data: f,
                                }),
                                //contentType: "application/json; charset=utf-8",  // Chỉ định kiểu nội dung là JSON
                                contentType: false,
                                processData: false,
                                success: function (res) {
                                    if (res.status == "success") {
                                        quanLyLopHoc.lopHoc.choXepLop.getList();
                                        quanLyLopHoc.lopHoc.daXepLop.getList();

                                        // Mở modal tạo lịch học
                                        quanLyLopHoc.lopHoc.lichHoc.displayModal_XemLichHoc('update', res.IdLopHoc);

                                        sys.displayModal({
                                            name: '#quanlylophoc-crud',
                                            displayStatus: "hide"
                                        });
                                        sys.alert({ status: res.status, mess: res.mess });
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

                                }
                            });
                        }
                    });
                };
            },
            delete: function (loai, idLopHoc = '00000000-0000-0000-0000-000000000000') {
                var idLopHocs = [];
                if (loai == "single") {
                    idLopHocs.push(idLopHoc);
                } else {
                    quanLyLopHoc.lopHoc.daXepLop.dataTable.rows().iterator('row', function (context, index) {
                        var $row = $(this.row(index).node());
                        if ($row.has("input.checkRow-daxeplop-getList:checked").length > 0) {
                            idLopHocs.push($row.attr('id'));
                        };
                    });
                };
                // Kiểm tra idLopHoc
                if (idLopHocs.length > 0) {
                    var f = new FormData();
                    f.append("idLopHocs", JSON.stringify(idLopHocs));
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
                                    quanLyLopHoc.lopHoc.daXepLop.getList();
                                    sys.alert({ status: res.status, mess: res.mess })
                                }
                            })
                        }
                    })
                } else {
                    sys.alert({ mess: "Bạn chưa chọn bản ghi nào", status: "warning", timeout: 1500 });
                };
            },
            guiMail: function () {
                var idLopHocs = [];
                quanLyLopHoc.lopHoc.daXepLop.dataTable.rows().iterator('row', function (context, index) {
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
                sys.confirmDialog({
                    mess: `Bạn có muốn gửi mail về thông tin lớp cho học viên ?`,
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
            },
        };

        quanLyLopHoc.lopHoc.choXepLop.getList();
        quanLyLopHoc.lopHoc.daXepLop.getList();
    }
    funcs_GiaoDienGiaoVien() {
        var quanLyLopHoc = this;
        quanLyLopHoc.buoiHocSapToi = {
            dataTable: null,
            getList: function () {
                $.ajax({
                    ...ajaxDefaultProps({
                        url: "/QuanLyLopHoc/buoiHocSapToi",
                        type: "GET", // Phải là POST để gửi JSON
                        data: {
                            idGiaoVien: quanLyLopHoc.nguoiDung.idNguoiDung_DangSuDung
                        },  // Chắc chắn đã dùng JSON.stringify()
                    }),
                    success: function (res) {
                            $("#buoihocsaptoi-getlist-container").html(res);
                        quanLyLopHoc.buoiHocSapToi.dataTable = new DataTableCustom({
                            name: "buoihocsaptoi-getList",
                            table: $("#buoihocsaptoi-getList"),
                            props: {
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
        quanLyLopHoc.lopHocThamGia = {
            dataTable: null,
            getList: function () {
                $.ajax({
                    ...ajaxDefaultProps({
                        url: "/QuanLyLopHoc/lopHocThamGia",
                        type: "GET", // Phải là POST để gửi JSON
                        data: {
                            idGiaoVien: quanLyLopHoc.nguoiDung.idNguoiDung_DangSuDung
                        },  // Chắc chắn đã dùng JSON.stringify()
                    }),
                    success: function (res) {
                            $("#lophocthamgia-getlist-container").html(res);
                        quanLyLopHoc.lopHocThamGia.dataTable = new DataTableCustom({
                            name: "lophocthamgia-getList",
                            table: $("#lophocthamgia-getList"),
                            props: {
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
        quanLyLopHoc.buoiHocSapToi.getList();
        quanLyLopHoc.lopHocThamGia.getList();
    }
};