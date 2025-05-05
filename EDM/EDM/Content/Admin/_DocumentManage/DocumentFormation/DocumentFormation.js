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
        if (chucNang == "trangchu") {
            options = {
                steps: [{
                    element: $('[intro-container="thanhtacvu"]')[0],
                    title: 'Thanh thao tác',
                    intro: `<p>🤖: Thanh thao tác chung.</p>
                            <p>🤖: <span class='text-danger font-bold'>Bạn chỉ được phép sử dụng những chức năng do quản trị viên đã cung cấp</span>.</p>`
                }, {
                    element: $('[intro-container="dulieu-container"]')[0],
                    title: 'Bảng dữ liệu',
                    intro: `<p>🤖: Dữ liệu bản ghi sẽ hiển thị tại đây.</p>`,
                    position: 'top'
                }, {
                    element: $('[intro-container="dulieu-hienthicot"]')[0],
                    title: 'Bảng dữ liệu',
                    intro: `<p>🤖: Nhấn chọn để ẩn/hiện cột dữ liệu mong muốn.</p>`
                }, {
                    element: $('[intro-container="dulieu-tencot"]')[0],
                    title: 'Bảng dữ liệu',
                    intro: `<p>🤖: Nhấn chọn để sắp xếp dữ liệu.</p>`
                }, {
                    element: $('[intro-container="dulieu-tacvu"]')[0],
                    title: 'Bảng dữ liệu',
                    intro: `<p>🤖: Các thao tác với từng bản ghi.</p>`,
                    position: 'left'
                }, {
                    element: $('[intro-container="dulieu-timkiem"]')[0],
                    title: 'Bảng dữ liệu',
                    intro: `<p>🤖: Nhập thông tin cần tìm kiếm.</p>`
                },]
            };
        } else if (chucNang == "hoso-themmoi") {
            options = {
                steps: [{
                    element: $('[intro-container="thongtin-thieu"]')[0],
                    title: 'Thêm mới hồ sơ',
                    intro: '<p>🤖: Nếu cần bổ sung thông tin còn thiếu, nhấn vào đây để di chuyển tới chức năng tương ứng.</p>'
                }, {
                    element: $('[intro-container="thongtin-chung"]')[0],
                    title: 'Thêm mới hồ sơ',
                    intro: '<p>🤖: Các trường thông tin của hồ sơ.</p>'
                }, {
                    element: $('[intro-container="thongtin-batbuoc"]')[0],
                    title: 'Thêm mới hồ sơ',
                    intro: '<p>🤖: Trường thông tin có dấu <span class="text-danger">*</span> là bắt buộc nhập.</p>'
                }, {
                    element: $('[intro-container="thongtin-thietlapquyen"]')[0],
                    title: 'Thêm mới hồ sơ',
                    intro: '<p>🤖: Nhấn vào đây để thiết lập quyền truy cập hồ sơ.</p>'
                },]
            };
        } else if (chucNang == "hoso-thietlapquyen") {
            options = {
                steps: [{
                    element: $('[intro-container="thietlapquyen-cocautochuc"]')[0],
                    title: 'Thiết lập quyền truy cập hồ sơ',
                    intro: '<p>🤖: Lọc theo nhóm cơ cấu tổ chức.</p>'
                }, {
                    element: $('[intro-container="thietlapquyen-tennguoidung"]')[0],
                    title: 'Thiết lập quyền truy cập hồ sơ',
                    intro: '<p>🤖: Lọc theo tên người dùng.</p>'
                },]
            };
        } else if (chucNang == "hoso-themmoihosoexcel") {
            options = {
                steps: [{
                    element: $('[intro-container="excel-thaotaccoban"]')[0],
                    title: 'Thêm mới hồ sơ bằng excel',
                    intro: '<p>🤖: Các thao tác với tệp.</p>'
                }, {
                    element: $('[intro-container="excel-thaotacnangcao"]')[0],
                    title: 'Thêm mới hồ sơ bằng excel',
                    intro: '<p>🤖: Cập nhật dữ liệu các bản ghi.</p>'
                }, {
                    element: $('[intro-container="excel-kiemtra"]')[0],
                    title: 'Thêm mới hồ sơ bằng excel',
                    intro: '<p>🤖: Trạng thái hợp lệ của bản ghi trước khi lưu.</p>'
                },]
            };
        } else if (chucNang == "hoso-themmoivanbanzip") {
            options = {
                steps: [{
                    element: $('[intro-container="file-vanban-thaotaccoban"]')[0],
                    title: 'Thêm mới văn bản bằng zip/rar',
                    intro: '<p>🤖: Các thao tác với tệp.</p>'
                }, {
                    element: $('[intro-container="file-vanban-kiemtra"]')[0],
                    title: 'Thêm mới văn bản bằng zip/rar',
                    intro: '<p>🤖: Trạng thái hợp lệ của bản ghi trước khi lưu.</p>'
                },]
            };
        } else {

        };

        introJsCustom.start({ options });
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
/**
 * main
 * */
class DocumentFormation {
    constructor() {
        this.page;
        this.pageGroup;
        this.hoSo = {
            data: [],
            dataTable: null,
            save: function () { },
            delete: function () { },
            nopLuu: function () { },
            kiemTra_MaHoSo: function () { },
            displayModal_CRUD: function () { },
            btnDownload: function () { },
            createMaHoSo: function () { },
        }
        this.viTriLuuTru = {
            data: [],
            treeView: null,
            nodeId: 0,
            str_idViTriLuuTrus: "",
            treeSelect: null,
            getList: function () { },
        }
        this.danhMucHoSo = {
            data: [],
            treeView: null,
            nodeId: 0,
            str_idDanhMucHoSos: "",
            treeSelect: null,
            getList: function () { },
        }
        this.coCauToChuc = {
            data: [],
            dataTable: null,
            getList: function () { },
            timKiem: function () { }
        }
        this.nguoiDung = {
            data: [],
            dataTable: null,
            getList: function () { }
        }
    }
    init() {
        var df = this;
        var idNguoiDung_DangSuDung = $("#input-idnguoidung-dangsudung").val();
        df.page = $("#page-documentformation");

        df.hoSo = {
            ...df.hoSo,
            dataTable: new DataTableCustom({
                name: "documentformation-getList",
                table: $("#documentformation-getList"),
                props: {
                    ajax: {
                        url: '/DocumentFormation/getList',
                        type: "GET",
                        data: function (arg) {
                            arg.str_idViTriLuuTrus = df.viTriLuuTru.str_idViTriLuuTrus;
                            arg.str_idDanhMucHoSos = df.danhMucHoSo.str_idDanhMucHoSos;
                        }
                    },
                    rowId: 'IdHoSo',
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
                                    return `<input class="form-check-input checkRow-documentformation-getList" type="checkbox"/>`;
                                };
                                return ``;
                            }
                        },
                        {
                            data: "TrangThai",
                            className: "text-center",
                            render: function (data, type, row, meta) {
                                if (data == 1)
                                    return `<span class="font-bold fst-italic text-danger">Chưa nộp</span>`;
                                return `<span class="font-bold fst-italic text-success">Đã nộp</span>`;
                            }
                        },
                        {
                            data: null,
                            className: "text-center",
                            render: function (data, type, row, meta) {
                                var quyenTruyCap = data.QuyenTruyCap.split(",");
                                // Nếu có quyền thì cho thao tác
                                if (quyenTruyCap.some(x => x == idNguoiDung_DangSuDung)) {
                                    return `<span title="${data.MaHoSo}">${sys.truncateString(data.MaHoSo, 60)}</span>`;
                                };
                                return `<span class="font-bold fst-italic text-danger" title="Không có quyền"><i class="bi bi-file-earmark-lock2 fs-4"></i></span>`;
                            }
                        },
                        {
                            data: "TieuDeHoSo",
                            className: "text-justify",
                            render: function (data, type, row, meta) {
                                return `<span title="${data}">${sys.truncateString(data, 60)}</span>`;
                            }
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
                            data: "ThoiHanBaoQuan",
                            className: "text-center",
                        },
                        {
                            data: "CheDoSuDung.TenCheDoSuDung",
                            className: "text-center",
                        },
                        { data: "NgonNgu" },
                        {
                            data: "ThoiGianBatDau",
                            className: "text-center",
                            render: function (data, type, row, meta) {
                                return data == null ? "" : moment(data).format('DD-MM-YYYY')
                            }
                        },
                        {
                            data: "ThoiGianKetThuc",
                            className: "text-center",
                            render: function (data, type, row, meta) {
                                return data == null ? "" : moment(data).format('DD-MM-YYYY')
                            }
                        },
                        {
                            data: null,
                            className: "text-center",
                            render: function (data, type, row, meta) {
                                var soLuongVanBan_YeuCau = data.TongSoVanBan;
                                var soLuongVanBan_ThucTe = data.VanBans.length;
                                var soLuongVanBan_SoHoa = 0;
                                $.each(data.VanBans, function (i, vanBan) {
                                    if (vanBan.IdBieuMau != null && vanBan.IdBieuMau != 0) {
                                        soLuongVanBan_SoHoa += 1;
                                    };
                                });
                                return `<span class="" title="Số lượng đã số hóa / Số lượng thực tế / Số lượng yêu cầu">${soLuongVanBan_SoHoa} / ${soLuongVanBan_ThucTe} / ${soLuongVanBan_YeuCau}</span>`;
                            }
                        },
                        {
                            data: "SoLuongTo",
                            className: "text-center",
                        },
                        {
                            data: "SoLuongTrang",
                            className: "text-center",
                        },
                        { data: "TinhTrangVatLy" },
                        {
                            data: null,
                            className: "text-center",
                            searchable: false,
                            orderable: false,
                            render: function (data, type, row, meta) {
                                var quyenTruyCap = data.QuyenTruyCap.split(",");
                                // Nếu có quyền thì cho thao tác
                                if (quyenTruyCap.some(x => x == idNguoiDung_DangSuDung)) {
                                    return `
                                    <div style="white-space: nowrap" intro-container="thanhtacvu-bangdulieu">
                                        <a class="btn btn-sm btn-success" title="Thêm văn bản" onclick="df.DocumentDigitizing(${data.IdHoSo})"><i class="bi bi-file-pdf-fill"></i></a>
                                        <a class="btn btn-sm btn-primary" title="Cập nhật" onclick="df.hoSo.displayModal_CRUD('update', ${data.IdHoSo})"><i class="bi bi-pencil-square"></i></a>
                                        <a class="btn btn-sm btn-danger" title="Xóa bỏ" onclick="df.hoSo.delete('single',${data.IdHoSo})"><i class="bi bi-trash3-fill"></i></a>
                                    </div>`;
                                };
                                return `<span class="font-bold fst-italic text-danger">Không có quyền</span>`;
                            }
                        }
                    ],
                }
            }).dataTable,
            btnDownload: function (btn) {
                df.hoSo.dataTable.buttons(btn).trigger();
            },
            createMaHoSo: function (item) {
                setTimeout(function () {
                    var maHoSo = $(item).val().trim();
                    // Bước 1: Biến chữ có dấu thành chữ không dấu
                    maHoSo = maHoSo.normalize('NFD').replace(/[\u0300-\u036f]/g, '');
                    // Bước 2: Xóa ký tự đặc biệt trừ dấu '_'
                    maHoSo = maHoSo.replace(/[^\w\s-_]+/g, '');
                    // Bước 3: Thay thế khoảng cách bằng duy nhất 1 ký tự '_'
                    maHoSo = maHoSo.replace(/\s+/g, '_');
                    // Bước 4: Xóa ký tự '-' ở đầu và cuối câu
                    maHoSo = maHoSo.replace(/^-+|-+$/g, '');
                    // Bước 5: Viết hoa
                    maHoSo = maHoSo.toUpperCase();
                    $(item).val(maHoSo);
                }, 500);
                //var maPhong = $("#select-phongluutru option:selected", $("#documentformation-crud")).data("ma").trim(),
                //    maViTri = $("#select-vitriluutru option:selected", $("#documentformation-crud")).data("ma").trim(),
                //    mucLuc = $("#select-danhmuchoso option:selected", $("#documentformation-crud")).data("ma").trim(),
                //    soKyHieu = $("#input-sokyhieu").val().trim(),
                //    maHoSo = [maPhong, mucLuc, soKyHieu].filter(x => x != "").join(".");
                //$("#input-mahoso").val(maHoSo.toUpperCase());
            },
            displayModal_CRUD: function (loai = "", idHoSo = 0) {
                $.ajax({
                    ...ajaxDefaultProps({
                        url: "/DocumentFormation/displayModal_CRUD",
                        type: "POST",
                        data: { loai, idHoSo }
                    }),
                    success: function (res) {
                        $("#documentformation-crud").html(res);
                        df.create_LichSu();
                        /**
                          * Gán các thuộc tính
                          */
                        var rows_NEW = df.hoSo_lichSu.dataTable.rows().nodes().toArray(); // Chọn phần thử đầu tiên của bảng
                        df.hoSo_lichSu.readRow($(rows_NEW[0]));
                        sys.displayModal({
                            name: '#documentformation-crud'
                        });
                    }
                })
            },
            save: function (loai) {
                var modalValidtion = htmlEl.activeValidationStates("#documentformation-crud");
                if (modalValidtion) {
                    var hoSo = {
                        IdHoSo: $("#input-idhoso", $("#documentformation-crud")).val(),
                        TieuDeHoSo: $("#input-tieudehoso", $("#documentformation-crud")).val().trim(),
                        QuyenTruyCap: $("#input-quyentruycap", $("#documentformation-crud")).val().trim(),
                        IdViTriLuuTru: $("#select-vitriluutru", $("#documentformation-crud")).val(),
                        ViTriLuuTru: {
                            IdViTriLuuTru: $("#select-vitriluutru", $("#documentformation-crud")).val(),
                        },
                        IdDanhMucHoSo: $("#select-danhmuchoso", $("#documentformation-crud")).val(),
                        DanhMucHoSo: {
                            IdDanhMucHoSo: $("#select-danhmuchoso", $("#documentformation-crud")).val(),
                        },
                        IdPhongLuuTru: $("#select-phongluutru", $("#documentformation-crud")).val(),
                        PhongLuuTru: {
                            IdPhongLuuTru: $("#select-phongluutru", $("#documentformation-crud")).val(),
                        },
                        IdCheDoSuDung: $("#select-chedosudung", $("#documentformation-crud")).val(),
                        CheDoSuDung: {
                            IdCheDoSuDung: $("#select-chedosudung", $("#documentformation-crud")).val(),
                        },
                        MucLucSo_NamHinhThanh: $("#input-muclucso", $("#documentformation-crud")).val().trim(),
                        So_KyHieu: $("#input-sokyhieu", $("#documentformation-crud")).val().trim(),
                        MaHoSo: $("#input-mahoso", $("#documentformation-crud")).val().trim(),
                        ThoiHanBaoQuan: $("#input-thoihanbaoquan", $("#documentformation-crud")).val().trim(),
                        ThoiGianBatDau: $("#input-thoigianbatdau", $("#documentformation-crud")).val().trim(),
                        ThoiGianKetThuc: $("#input-thoigianketthuc", $("#documentformation-crud")).val().trim(),
                        TinhTrangVatLy: $("#input-tinhtrangvatly", $("#documentformation-crud")).val().trim(),
                        NgonNgu: $("#input-ngonngu", $("#documentformation-crud")).val().trim(),
                        TuKhoa: $("#input-tukhoa", $("#documentformation-crud")).val().trim(),
                        KyHieuThongTin: $("#input-kyhieuthongtin", $("#documentformation-crud")).val().trim(),
                        TongSoVanBan: $("#input-tongsovanban", $("#documentformation-crud")).val().trim(),
                        SoLuongTo: $("#input-soluongto", $("#documentformation-crud")).val().trim(),
                        SoLuongTrang: $("#input-soluongtrang", $("#documentformation-crud")).val().trim(),
                        GhiChu: $("#input-ghichu", $("#documentformation-crud")).val().trim(),
                    };
                    if (hoSo.ThoiGianBatDau != "")
                        hoSo.ThoiGianBatDau = moment(hoSo.ThoiGianBatDau, 'YYYY-MM-DD').format('DD/MM/YYYY');
                    if (hoSo.ThoiGianKetThuc != "")
                        hoSo.ThoiGianKetThuc = moment(hoSo.ThoiGianKetThuc, 'YYYY-MM-DD').format('DD/MM/YYYY');
                    $.ajax({
                        ...ajaxDefaultProps({
                            url: loai == "create" ? "/DocumentFormation/create_HoSo" : "/DocumentFormation/update_HoSo",
                            type: "POST",
                            data: {
                                str_hoSo: JSON.stringify(hoSo),
                            }
                        }),
                        success: function (res) {
                            if (res.status == "success") {
                                df.hoSo.dataTable.ajax.reload(function () {
                                    sys.displayModal({
                                        name: '#documentformation-crud',
                                        displayStatus: "hide"
                                    });
                                    sys.alert({ status: res.status, mess: res.mess });
                                }, false);
                            } else {
                                if (res.status == "warning") {
                                    htmlEl.inputValidationStates(
                                        $("#input-mahoso"),
                                        "#documentformation-crud",
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
                };
            },
            delete: function (loai, idHoSo = 0) {
                var idHoSos = [];
                if (loai == "single") {
                    idHoSos.push(idHoSo);
                } else {
                    df.hoSo.dataTable.rows().iterator('row', function (context, index) {
                        var $row = $(this.row(index).node());
                        if ($row.has("input.checkRow-documentformation-getList:checked").length > 0) {
                            idHoSos.push($row.attr('id'));
                        };
                    });
                };
                // Kiểm tra idHoSo
                if (idHoSos.length > 0) {
                    var f = new FormData();
                    f.append("str_idHoSos", idHoSos.toString());
                    sys.confirmDialog({
                        mess: `
                        <p class="font-bold">Hồ sơ có liên kết với các
                            <span class="text-danger fst-italic"> [Văn bản]</span> và
                            <span class="text-danger fst-italic"> [Phiếu mượn]</span> 
                        </p>
                        <p>Bạn có thực sự muốn xóa bản ghi này hay không ?</p>
                        `,
                        callback: function () {
                            $.ajax({
                                ...ajaxDefaultProps({
                                    url: "/DocumentFormation/delete_HoSos",
                                    type: "POST",
                                    data: f,
                                }),
                                contentType: false,
                                processData: false,
                                success: function (res) {
                                    df.hoSo.dataTable.ajax.reload(function () {
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
            nopLuu: function (loai, idHoSo = 0) {
                var idHoSos = [];
                if (loai == "single") {
                    idHoSos.push(idHoSo)
                } else {
                    df.hoSo.dataTable.rows().iterator('row', function (context, index) {
                        var $row = $(this.row(index).node()),
                            $checkBox = $("input[type='checkbox']:checked", $row);
                        $.each($checkBox, function () {
                            idHoSos.push($(this).closest('tr').attr('id'));
                        })
                    });
                };
                if (idHoSos.length > 0) {
                    var f = new FormData();
                    f.append("str_idHoSos", idHoSos.toString());
                    sys.confirmDialog({
                        mess: "Bạn chắn chắn muốn nộp lưu cho hồ sơ này ?",
                        callback: function () {
                            $.ajax({
                                ...ajaxDefaultProps({
                                    url: "/DocumentFormation/nopLuu",
                                    type: "POST",
                                    data: f,
                                }),
                                contentType: false,
                                processData: false,
                                success: function (res) {
                                    df.hoSo.dataTable.ajax.reload(function () {
                                        sys.alert({ status: res.status, mess: res.mess });
                                        //if (res.status == "success") {
                                        //    setTimeout(function () {
                                        //        sys.confirmDialog({
                                        //            mess: "Bạn có muốn chuyển tới trang hồ sơ nộp lưu ?",
                                        //            callback: function () {
                                        //                window.location.href = `/DocumentStorage/Index`
                                        //            }
                                        //        });
                                        //    }, 1000);
                                        //};
                                    }, false);
                                }
                            })
                        }
                    });
                } else {
                    sys.alert({ mess: "Bạn chưa chọn bản ghi nào", status: "warning", timeout: 1500 })
                };
            },
            huyNopLuu: function (loai, idHoSo = 0) {
                var idHoSos = [];
                if (loai == "single") {
                    idHoSos.push(idHoSo)
                } else {
                    df.hoSo.dataTable.rows().iterator('row', function (context, index) {
                        var $row = $(this.row(index).node()),
                            $checkBox = $("input[type='checkbox']:checked", $row);
                        $.each($checkBox, function () {
                            idHoSos.push($(this).closest('tr').attr('id'));
                        })
                    });
                };
                if (idHoSos.length > 0) {
                    var f = new FormData();
                    f.append("str_idHoSos", idHoSos.toString());
                    sys.confirmDialog({
                        mess: "Bạn chắn chắn muốn hủy nộp lưu cho hồ sơ này ?",
                        callback: function () {
                            $.ajax({
                                ...ajaxDefaultProps({
                                    url: "/DocumentFormation/huyNopLuu",
                                    type: "POST",
                                    data: f,
                                }),
                                contentType: false,
                                processData: false,
                                success: function (res) {
                                    df.hoSo.dataTable.ajax.reload(function () {
                                        sys.alert({ status: res.status, mess: res.mess });
                                    }, false);
                                }
                            })
                        }
                    });
                } else {
                    sys.alert({ mess: "Bạn chưa chọn bản ghi nào", status: "warning", timeout: 1500 })
                };
            },
            saoLuu: function () {
                df.hoSo.xacNhanSaoLuu({
                    mess: `
                        <p class="font-bold text-danger">Sau khi lựa chọn hình thức, hệ thống sẽ sao lưu tất cả dữ liệu thuộc loại hình đã chọn</p>
                        <p>Quá trình sao lưu sẽ mất vài phút, bạn có muốn tiếp tục thao tác ?</p>
                        `,
                    saoLuuHoSo: function () {
                        var formData = new FormData(),
                            loaiTaiXuong = "saoluu";
                        formData.append("loaiTaiXuong", loaiTaiXuong);
                        $.ajax({
                            ...ajaxDefaultProps({
                                url: "/DocumentFormation/get_HoSos_download",
                                type: "POST",
                                data: formData
                            }),
                            contentType: false,
                            processData: false,
                            success: function () {
                                sys.alert({ status: "success", mess: "Sao lưu dữ liệu thành công" })
                                window.location = "/DocumentFormation/download_Excel_HoSo";
                            }
                        })
                    },
                    saoLuuVanBan: function () {
                        $.ajax({
                            ...ajaxDefaultProps({
                                url: `/DocumentFormation/saoLuu`,
                            }),
                            contentType: false,
                            processData: false,
                            success: function (res) {
                                sys.alert({ status: res.status, mess: res.mess });
                                if (res.status == "success") {
                                    window.location = res.duongDan_TepSaoLuu;
                                };
                            }
                        })
                    },
                });
            },
            xacNhanSaoLuu({
                mess = "",
                saoLuuHoSo = function () { },
                saoLuuVanBan = function () { },
                callback_no = function () {
                    sys.displayModal({
                        name: "#xacnhansaoluu",
                        displayStatus: "hide",
                        level: 100
                    })
                }
            }) {
                $("#xacnhansaoluu-content").html(mess);
                sys.displayModal({
                    name: "#xacnhansaoluu",
                    level: 100
                });
                // Phải off() rồi mới on() để không bị lặp lại sự kiện nhiều lần
                $("#xacnhansaoluu").find("button[name='hoso']").off().on("click", function (e) {
                    e.preventDefault();
                    saoLuuHoSo();
                    sys.displayModal({
                        name: "#xacnhansaoluu",
                        displayStatus: "hide",
                        level: 100
                    })
                });
                $("#xacnhansaoluu").find("button[name='vanban']").off().on("click", function (e) {
                    e.preventDefault();
                    saoLuuVanBan();
                    sys.displayModal({
                        name: "#xacnhansaoluu",
                        displayStatus: "hide",
                        level: 100
                    })
                });
                $("#xacnhansaoluu").find("button[name='no']").off().on("click", function (e) {
                    e.preventDefault();
                    callback_no();
                });
            },
            thietLapQuyenHangLoat() {
                var idHoSos = [],
                    quyenTruyCap = $("#input-quyentruycap-hangloat").val();;
                // Lấy danh sách hồ sơ
                df.hoSo.dataTable.rows().iterator('row', function (context, index) {
                    var $row = $(this.row(index).node());
                    if ($row.has("input.checkRow-documentformation-getList:checked").length > 0) {
                        idHoSos.push($row.attr('id'));
                    };
                });
                // Kiểm tra idHoSo
                if (idHoSos.length > 0) {
                    var f = new FormData();
                    f.append("str_idHoSos", idHoSos.toString());
                    f.append("quyenTruyCap", quyenTruyCap);
                    $.ajax({
                        ...ajaxDefaultProps({
                            url: "/DocumentFormation/thietLapQuyenHangLoat",
                            type: "POST",
                            data: f,
                        }),
                        contentType: false,
                        processData: false,
                        success: function (res) {
                            df.hoSo.dataTable.ajax.reload(function () {
                                sys.alert({ status: res.status, mess: res.mess })
                            }, false);
                        }
                    });
                } else {
                    sys.alert({ mess: "Bạn chưa chọn hồ sơ nào", status: "warning", timeout: 1500 });
                };
            },
        };
        df.viTriLuuTru = {
            ...df.viTriLuuTru,
            getList: function () {
                $.ajax({
                    ...ajaxDefaultProps({
                        url: "/DocumentFormation/get_ViTriLuuTrus",
                        type: "GET",
                    }),
                    success: function (res) {
                        if (res.length > 0) {
                            var MAKETREEDATA = (vts) => {
                                var nodes = [];
                                $.each(vts, function (i, vt) {
                                    let _vt = {
                                        name: `${vt.root.TenViTriLuuTru} (${vt.hoSos.length}📦)`,
                                        value: vt.root.IdViTriLuuTru,
                                        //children: [] // Nếu có thì mới thêm vì sẽ mặc định hiện icon collapse nếu có thuộc tính này
                                    }
                                    if (vt.nodes.length > 0) _vt.children = MAKETREEDATA(vt.nodes);
                                    nodes.push(_vt);
                                });
                                return nodes;
                            };
                            df.viTriLuuTru.data = res;
                            df.viTriLuuTru.treeSelect = new TreeSelectCustom({
                                props: {
                                    parentHtmlContainer: document.getElementById("treeSelect_vitriluutru_container"),
                                    isIndependentNodes: true, // Không gộp phần tử con thành phần tử cha khi chọn tất cả
                                    listClassName: 'treeselect-list-item',
                                    placeholder: 'Tìm kiếm theo vị trí lưu trữ...',
                                    options: MAKETREEDATA(df.viTriLuuTru.data),
                                    id: 'treeSelect_vitriluutru',
                                    //value: options,
                                    inputCallback: function (value) {
                                        df.viTriLuuTru.str_idViTriLuuTrus = value.join(","); // Thêm vào danh sách
                                        df.hoSo.dataTable.ajax.reload();
                                    },
                                }
                            }).init();
                        }
                    }
                })
            },
        };
        df.danhMucHoSo = {
            ...df.danhMucHoSo,
            getList: function () {
                $.ajax({
                    ...ajaxDefaultProps({
                        url: "/DocumentFormation/get_DanhMucHoSos",
                        type: "GET",
                    }),
                    success: function (res) {
                        if (res.length > 0) {
                            var MAKETREEDATA = (vts) => {
                                var nodes = [];
                                $.each(vts, function (i, vt) {
                                    let _vt = {
                                        name: `${vt.root.TenDanhMucHoSo} (${vt.hoSos.length}📦)`,
                                        value: vt.root.IdDanhMucHoSo,
                                        //children: [] // Nếu có thì mới thêm vì sẽ mặc định hiện icon collapse nếu có thuộc tính này
                                    }
                                    if (vt.nodes.length > 0) _vt.children = MAKETREEDATA(vt.nodes);
                                    nodes.push(_vt);
                                });
                                return nodes;
                            };
                            df.danhMucHoSo.data = res;
                            df.danhMucHoSo.treeSelect = new TreeSelectCustom({
                                props: {
                                    parentHtmlContainer: document.getElementById("treeSelect_danhmuchoso_container"),
                                    isIndependentNodes: true, // Không gộp phần tử con thành phần tử cha khi chọn tất cả
                                    listClassName: 'treeselect-list-item',
                                    placeholder: 'Tìm kiếm theo danh mục hố sơ...',
                                    options: MAKETREEDATA(df.danhMucHoSo.data),
                                    id: 'treeSelect_danhmuchoso',
                                    //value: options,
                                    inputCallback: function (value) {
                                        df.danhMucHoSo.str_idDanhMucHoSos = value.join(","); // Thêm vào danh sách
                                        df.hoSo.dataTable.ajax.reload();
                                    },
                                }
                            }).init();
                        }
                    }
                })
            },
        };
        huongDanSuDung.lanDauDangNhap("trangchu");
        $("#btn-huongdansudung-tongquat").off().on("click", () => huongDanSuDung.kichHoat("trangchu"));
        sys.activePage({
            page: df.page.attr("id"),
            pageGroup: df.pageGroup
        });
    }
    displayModal_ThietLapQuyen(viTriLuu, loai = "single") {
        $.ajax({
            ...ajaxDefaultProps({
                url: "/DocumentFormation/displayModal_ThietLapQuyen",
                type: "GET",
            }),
            success: function (res) {
                $("#documentformation-thietlapquyen").html(res);
                var idCoCau_DangSuDung = $("#input-idcocautochuc-dangsudung").val();
                var idNguoiDung_DangSuDung = $("#input-idnguoidung-dangsudung").val();
                var kiemTraChonNhomQuyen = function () {
                    // Số bản ghi
                    var soLuongBanGhi = $("._checkRow-nguoidung-getList").length;
                    // Số bản ghi đã chọn
                    var soLuongBanGhi_DaChon = $("._checkRow-nguoidung-getList[type='checkbox']:checked").length;
                    if (soLuongBanGhi == soLuongBanGhi_DaChon) {
                        $("._checkAll-nguoidung-getList").prop("checked", true);
                    } else {
                        $("._checkAll-nguoidung-getList").prop("checked", false);
                    };
                };
                //#region Tạo datatable
                {
                    df.nguoiDung = {
                        dataTable: new DataTableCustom({
                            name: "nguoidung-getList",
                            table: $("#nguoidung-getList"),
                            props: {
                                dom: `
                                <'row'<'col-sm-12 col-md-6'>>
                                <'row'<'col-sm-12'rt>>
                                <'row'<'col-sm-12 col-md-6'i><'col-sm-12 col-md-6 pt-2'p>>`,
                                maxHeight: 600,
                                order: [[1, 'asc']],
                                //orderable: false,
                                lengthMenu: [
                                    [-1],
                                    ['Tất cả'],
                                ],
                            }
                        }).dataTable,
                    };
                    // Tạo sự kiện chọn cơ cấu
                    $("#select-cocautochuc", "#documentformation-thietlapquyen").on("change", function () {
                        // Lọc nhóm
                        var idCoCau = $(this).val();
                        if (idCoCau == 0) {
                            $(this).siblings("input.dt-search-col").val("").trigger("change");
                        } else {
                            var tenCoCau = $(this).find("option:selected").text().split(".").reverse()[0].trim();
                            $(this).siblings("input.dt-search-col").val(tenCoCau).trigger("change");
                        };
                        // Trạng thái chọn nhóm
                        kiemTraChonNhomQuyen();
                    });
                    // Mặc định chọn cơ cấu của tài khoản
                    //$(`#select-cocautochuc option[value="${idCoCau_DangSuDung}"]`, "#documentformation-thietlapquyen").trigger("click");
                    //$(`#select-cocautochuc`, "#documentformation-thietlapquyen").val(idCoCau_DangSuDung).trigger("change");
                };
                //#endregion
                //#region Chọn nhóm quyền
                {
                    $("._checkAll-nguoidung-getList").off().on("click", function () {
                        $("._checkRow-nguoidung-getList").prop("checked", $(this).is(":checked"));
                    });
                    $("._checkRow-nguoidung-getList").off().on("click", function () { kiemTraChonNhomQuyen() });
                };
                //#endregion
                //#region Gán quyền
                {
                    var rows = df.nguoiDung.dataTable.rows().nodes().toArray();
                    // Nếu là cập nhật thì sẽ gán danh sách idNguoiDung vào bảng
                    //if (loai == "update") {
                    var idNguoiDungs = $(viTriLuu).val().split(",");
                    if (idNguoiDungs) {
                        $.each(rows, function () {
                            var idNguoiDung = $(this).data("idnguoidung");
                            var $checkBox = $(`._checkRow-nguoidung-getList`, $(this));
                            $checkBox.is(":checked") && $checkBox.trigger("click"); // Đã check thì hủy
                            if (idNguoiDungs.some(x => x == idNguoiDung)) { // Nếu thuộc danh sách quyền thì check
                                $checkBox.trigger("click");
                            };
                        });
                    };
                    //};
                    // Gán sự kiện lưu cho nút lưu, danh sách idNguoiDung sẽ lưu vào vị trí được gọi
                    $("#thietLapQuyen_Luu", "#documentformation-thietlapquyen").off().on("click", function () {
                        // Lấy danh sách idNguoiDung
                        var idNguoiDungs = [];
                        var $rowChecks = $(`._checkRow-nguoidung-getList:checked`, rows);
                        $.each($rowChecks, function () {
                            var $rowCheck = $(this).closest("tr"),
                                idNguoiDung = $rowCheck.data("idnguoidung");
                            idNguoiDungs.push(idNguoiDung);
                        });
                        // Kiểm tra danh sách quyền
                        if (idNguoiDungs.length == 0) {
                            sys.alert({ status: "warning", mess: "Bạn chưa chọn quyền nào" });
                        } else {
                            // Danh sách quyền không chứa tài khoản đang sử dụng
                            if (!idNguoiDungs.some(x => x == idNguoiDung_DangSuDung)) {
                                sys.confirmDialog({
                                    mess: `
                                    <p class="font-bold">Bạn hiện không thiết lập quyền cho chính mình, hồ sơ sẽ chỉ
                                        <span class="text-danger fst-italic"> [hiển thị] </span> 
                                        với các tài khoản có quyền được thiết lập bên dưới
                                    </p>
                                    <p>Bạn đã chắc chắn với quyết định của mình ?</p>
                                    `,
                                    callback: function () {
                                        //if (!idNguoiDungs.some(x => x == idNguoiDung_DangSuDung)) idNguoiDungs.push(idNguoiDung_DangSuDung); // Tự chọn chính mình
                                        // Lưu vào vị trí
                                        $(viTriLuu).val(idNguoiDungs.join(","));
                                        sys.displayModal({
                                            name: "#documentformation-thietlapquyen",
                                            displayStatus: "hide",
                                            level: 3
                                        });
                                    }
                                });
                            } else {
                                $(viTriLuu).val(idNguoiDungs.join(","));
                                sys.displayModal({
                                    name: "#documentformation-thietlapquyen",
                                    displayStatus: "hide",
                                    level: 3
                                });
                                // Gọi hàm thiết lập hàng loạt
                                if (loai == "multiple") {
                                    df.hoSo.thietLapQuyenHangLoat();
                                };
                            };
                        };
                    });
                };
                //#endregion
                sys.displayModal({
                    name: "#documentformation-thietlapquyen",
                    level: 3
                });
            }
        })
    }
    chonThoiHanBaoQuan(checkBox, input) {
        var checked = $(checkBox).is(":checked");
        input.attr("disabled", checked);
        if (checked) {
            input.val("Vĩnh viễn");
        } else {
            input.val("");
        };
    }
    DocumentDigitizing(idHoSo) {
        window.location.href = `/DocumentDigitizing?idHoSo=${idHoSo}`;
        //$.ajax({
        //    ...ajaxDefaultProps({
        //        url: `/DocumentDigitizing/Index?idHoSo=${idHoSo}`,
        //        type: "GET",
        //    }),
        //    success: function (res) {
        //        //$('#page-documentformation').animate({ opacity: 0 }, 1000);
        //        //$('#page-documentdigitizing').delay(1000).animate({ opacity: 1 }, 1000);
        //        $('#page-documentformation').hide();
        //        $('#page-documentdigitizing').html(res);

        //        dd = new DocumentDigitizing();
        //        dd.init();
        //        dd.vanBan.getList();
        //    }
        //})
    }

    //#region Thêm excel - Hồ sơ
    displayModal_Excel_HoSo() {
        var df = this;
        df.getList_Excel_HoSo("reload");
        sys.displayModal({
            name: '#excel-hoso'
        });
    }
    getList_Excel_HoSo(loai) {
        var df = this;
        $.ajax({
            ...ajaxDefaultProps({
                url: "/DocumentFormation/getList_Excel_HoSo",
                type: "POST",
                data: {
                    loai
                }
            }),
            success: function (res) {
                $("#excel-hoso-getList-container").html(res);
                /**
                 * ExcelHoSo 
                 */
                df.create_Excel_HoSo();
                /**
                 * Gán các thuộc tính
                 */
                var rows_NEW = df.excelHoSo.dataTable.rows().nodes().toArray(); // Chọn phần thử đầu tiên của bảng
                df.excelHoSo.readRow($(rows_NEW[0]));
                $.each($(".excel-hoso-read", $("#excel-hoso")), function () {
                    var $div = $(this),
                        rowNumber = $div.attr("row");
                    // Gán validation
                    htmlEl.validationStates($div);
                    htmlEl.inputMask();
                    htmlEl.select2Mask($div);

                    var modalValidtion = htmlEl.activeValidationStates($div);
                });
            }
        })
    }
    create_Excel_HoSo() {
        var df = this;
        var containerHeight = $("#excel-hoso-getList-container").height() - 10;
        $("#excel-hoso-read-container", $("#excel-hoso")).height(containerHeight);
        df.excelHoSo = {
            dataTable: new DataTableCustom({
                name: "excel-hoso-getList",
                table: $("#excel-hoso-getList"),
                props: {
                    maxHeight: containerHeight,
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
            displayModal_UpdateMultipleCell: function () {
                sys.displayModal({
                    name: '#excel-hoso-capnhattruong',
                    level: 2
                });
            },
            createRow: function () { },
            deleteRow: function () {
                var rows = df.excelHoSo.dataTable.rows().nodes().toArray(),
                    $rowChecks = $(`.checkRow-excel-hoso-getList:checked`, rows);
                if ($rowChecks.length == 0) {
                    sys.alert({ status: "warning", mess: "Bạn chưa chọn bản ghi nào" })
                } else {
                    $.each($rowChecks, function () {
                        var $rowCheck = $(this).closest("tr"),
                            rowNumber = $rowCheck.attr("row"),
                            $div = $(`.excel-hoso-read[row=${rowNumber}]`, $("#excel-hoso"));
                        df.excelHoSo.dataTable.row($rowCheck).remove().draw(); // Xóa dòng đó
                        $div.remove();
                    });
                    // Chọn phần thử đầu tiên của bảng
                    df.excelHoSo.readRow($(rows[0]));
                };
            },
            readRow: function (el) {
                var rowNumber = $(el).attr("row"),
                    rows = df.excelHoSo.dataTable.rows().nodes().toArray(),
                    $divs = $(".excel-hoso-read", $("#excel-hoso")),
                    $div = $(`.excel-hoso-read[row=${rowNumber}]`, $("#excel-hoso"));
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
            updateSingleCell: function (el) {
                var rows = df.excelHoSo.dataTable.rows().nodes().toArray(),
                    $div = $(el).closest(".excel-hoso-read"),
                    rowNumber = $div.attr("row");
                $.each(rows, function () {
                    if ($(this).attr("row") == rowNumber) {
                        //var val = $(el).val().replace(/[.-]/g, '');
                        var val = $(el).val();
                        $(el).val(val);
                        $('span[data-tentruong="MaHoSo"]', $(this)).text(val);
                    };
                });
            },
            updateMultipleCell: function () {
                var soDongChon = 0,
                    idPhongLuuTru = $("#select-phongluutru-capnhattruong", $("#excel-hoso-capnhattruong")).val(),
                    idViTriLuuTru = $("#select-vitriluutru-capnhattruong", $("#excel-hoso-capnhattruong")).val(),
                    idDanhMucHoSo = $("#select-danhmuchoso-capnhattruong", $("#excel-hoso-capnhattruong")).val(),
                    idCheDoSuDung = $("#select-chedosudung-capnhattruong", $("#excel-hoso-capnhattruong")).val(),
                    quyenTruyCap = $("#input-quyentruycap-capnhattruong", $("#excel-hoso-capnhattruong")).val();

                var rows = df.excelHoSo.dataTable.rows().nodes().toArray(),
                    $rowChecks = $(`.checkRow-excel-hoso-getList:checked`, rows);
                if ($rowChecks.length == 0) {
                    sys.alert({ status: "warning", mess: "Bạn chưa chọn bản ghi nào" })
                } else {
                    $.each($rowChecks, function () {
                        var $rowCheck = $(this).closest("tr"),
                            rowNumber = $rowCheck.attr("row"),
                            $div = $(`.excel-hoso-read[row=${rowNumber}]`, $("#excel-hoso"));
                        // Thay đổi value cho những dòng được chọn
                        $(`#select-phongluutru-${rowNumber}`, $div).val(idPhongLuuTru); $(`#select-phongluutru-${rowNumber}`, $div).trigger("change");
                        $(`#select-vitriluutru-${rowNumber}`, $div).val(idViTriLuuTru); $(`#select-vitriluutru-${rowNumber}`, $div).trigger("change");
                        $(`#select-danhmuchoso-${rowNumber}`, $div).val(idDanhMucHoSo); $(`#select-danhmuchoso-${rowNumber}`, $div).trigger("change");
                        $(`#input-quyentruycap-${rowNumber}`, $div).val(quyenTruyCap); $(`#input-quyentruycap-${rowNumber}`, $div).trigger("change");
                        $(`#select-chedosudung`, $div).val(idCheDoSuDung); $(`#select-chedosudung`, $div).trigger("change");
                    });

                    sys.alert({ status: "success", mess: "Cập nhật trường dữ liệu thành công" })
                    sys.displayModal({
                        name: '#excel-hoso-capnhattruong',
                        displayStatus: "hide",
                        level: 2,
                    });
                };
            },
            upload: function () {
                var $select = $("#select-file").get(0),
                    formData = new FormData();
                $.each($select.files, function (idx, f) {
                    var extension = f.type;
                    if (extension.includes("sheet")) {
                        formData.append("files", f);
                    };
                });
                // Xóa bộ nhớ đệm để upload file trong lần tiếp theo
                $select.value = ''; // xóa giá trị của input file
                $.ajax({
                    ...ajaxDefaultProps({
                        url: "/DocumentFormation/upload_Excel_HoSo",
                        type: "POST",
                        data: formData
                    }),
                    contentType: false,
                    processData: false,
                    success: function (res) {
                        df.getList_Excel_HoSo("upload");
                        df.excelHoSo.dataTable.search('').draw();
                        sys.alert({ status: res.status, mess: res.mess });
                    }
                })
            },
            download: function (loaiTaiXuong) {
                var formData = new FormData(),
                    hoSos = [];
                if (loaiTaiXuong == "data") {
                    var rows = df.excelHoSo.dataTable.rows().nodes().toArray(),
                        $rowChecks = $(`.checkRow-excel-hoso-getList:checked`, rows);
                    $.each($rowChecks, function () {
                        var $rowCheck = $(this).closest("tr"),
                            rowNumber = $rowCheck.attr("row"),
                            $div = $(`.excel-hoso-read[row=${rowNumber}]`, $("#excel-hoso")),
                            hoSo = {
                                IdHoSo: 0,
                                TieuDeHoSo: $("#input-tieudehoso", $div).val().trim(),
                                QuyenTruyCap: $(`#input-quyentruycap-${rowNumber}`, $div).val().trim(),
                                IdViTriLuuTru: $(`#select-vitriluutru-${rowNumber}`, $div).val(),
                                ViTriLuuTru: {
                                    IdViTriLuuTru: $(`#select-vitriluutru-${rowNumber}`, $div).val(),
                                },
                                IdDanhMucHoSo: $(`#select-danhmuchoso-${rowNumber}`, $div).val(),
                                DanhMucHoSo: {
                                    IdDanhMucHoSo: $(`#select-danhmuchoso-${rowNumber}`, $div).val(),
                                },
                                IdPhongLuuTru: $(`#select-phongluutru-${rowNumber}`, $div).val(),
                                PhongLuuTru: {
                                    IdPhongLuuTru: $(`#select-phongluutru-${rowNumber}`, $div).val(),
                                },
                                IdCheDoSuDung: $("#select-chedosudung", $div).val(),
                                CheDoSuDung: {
                                    IdCheDoSuDung: $("#select-chedosudung", $div).val(),
                                },
                                MucLucSo_NamHinhThanh: $("#input-muclucso", $div).val().trim(),
                                So_KyHieu: $("#input-sokyhieu", $div).val().trim(),
                                MaHoSo: $("#input-mahoso", $div).val().trim(),
                                ThoiHanBaoQuan: $("#input-thoihanbaoquan", $div).val().trim(),
                                ThoiGianBatDau: $("#input-thoigianbatdau", $div).val().trim(),
                                ThoiGianKetThuc: $("#input-thoigianketthuc", $div).val().trim(),
                                TinhTrangVatLy: $("#input-tinhtrangvatly", $div).val().trim(),
                                NgonNgu: $("#input-ngonngu", $div).val().trim(),
                                TuKhoa: $("#input-tukhoa", $div).val().trim(),
                                KyHieuThongTin: $("#input-kyhieuthongtin", $div).val().trim(),
                                TongSoVanBan: $("#input-tongsovanban", $div).val().trim(),
                                SoLuongTo: $("#input-soluongto", $div).val().trim(),
                                SoLuongTrang: $("#input-soluongtrang", $div).val().trim(),
                                GhiChu: $("#input-ghichu", $div).val().trim(),
                            };
                        if (hoSo.ThoiGianBatDau != "")
                            hoSo.ThoiGianBatDau = moment(hoSo.ThoiGianBatDau, 'YYYY-MM-DD').format('DD/MM/YYYY');
                        if (hoSo.ThoiGianKetThuc != "")
                            hoSo.ThoiGianKetThuc = moment(hoSo.ThoiGianKetThuc, 'YYYY-MM-DD').format('DD/MM/YYYY');
                        hoSos.push(hoSo);
                    });
                    formData.append("str_hoSos", JSON.stringify(hoSos));
                }
                formData.append("loaiTaiXuong", loaiTaiXuong);
                $.ajax({
                    ...ajaxDefaultProps({
                        url: "/DocumentFormation/get_HoSos_download",
                        type: "POST",
                        data: formData
                    }),
                    contentType: false,
                    processData: false,
                    success: function () {
                        sys.alert({ status: "success", mess: "Đã tải xuống thành công" })
                        window.location = "/DocumentFormation/download_Excel_HoSo";
                    }
                })
            },
            reload: function () {
                df.getList_Excel_HoSo("reload");
                df.excelHoSo.dataTable.search('').draw();
                sys.alert({ status: "success", mess: "Đã làm mới dữ liệu" });
            },
            saveByGroup: function () {
                var formData = new FormData(),
                    hoSos = [];
                $.each($(".excel-hoso-read", $("#excel-hoso")), function () {
                    var $div = $(this),
                        rowNumber = $div.attr("row"),
                        hoSo = {
                            IdHoSo: 0,
                            TieuDeHoSo: $("#input-tieudehoso", $div).val().trim(),
                            QuyenTruyCap: $(`#input-quyentruycap-${rowNumber}`, $div).val().trim(),
                            IdViTriLuuTru: $(`#select-vitriluutru-${rowNumber}`, $div).val(),
                            ViTriLuuTru: {
                                IdViTriLuuTru: $(`#select-vitriluutru-${rowNumber}`, $div).val(),
                            },
                            IdDanhMucHoSo: $(`#select-danhmuchoso-${rowNumber}`, $div).val(),
                            DanhMucHoSo: {
                                IdDanhMucHoSo: $(`#select-danhmuchoso-${rowNumber}`, $div).val(),
                            },
                            IdPhongLuuTru: $(`#select-phongluutru-${rowNumber}`, $div).val(),
                            PhongLuuTru: {
                                IdPhongLuuTru: $(`#select-phongluutru-${rowNumber}`, $div).val(),
                            },
                            IdCheDoSuDung: $("#select-chedosudung", $div).val(),
                            CheDoSuDung: {
                                IdCheDoSuDung: $("#select-chedosudung", $div).val(),
                            },
                            MucLucSo_NamHinhThanh: $("#input-muclucso", $div).val().trim(),
                            So_KyHieu: $("#input-sokyhieu", $div).val().trim(),
                            MaHoSo: $("#input-mahoso", $div).val().trim(),
                            ThoiHanBaoQuan: $("#input-thoihanbaoquan", $div).val().trim(),
                            ThoiGianBatDau: $("#input-thoigianbatdau", $div).val().trim(),
                            ThoiGianKetThuc: $("#input-thoigianketthuc", $div).val().trim(),
                            TinhTrangVatLy: $("#input-tinhtrangvatly", $div).val().trim(),
                            NgonNgu: $("#input-ngonngu", $div).val().trim(),
                            TuKhoa: $("#input-tukhoa", $div).val().trim(),
                            KyHieuThongTin: $("#input-kyhieuthongtin", $div).val().trim(),
                            TongSoVanBan: $("#input-tongsovanban", $div).val().trim(),
                            SoLuongTo: $("#input-soluongto", $div).val().trim(),
                            SoLuongTrang: $("#input-soluongtrang", $div).val().trim(),
                            GhiChu: $("#input-ghichu", $div).val().trim(),
                        };
                    if (hoSo.ThoiGianBatDau != "")
                        hoSo.ThoiGianBatDau = moment(hoSo.ThoiGianBatDau, 'YYYY-MM-DD').format('DD/MM/YYYY');
                    if (hoSo.ThoiGianKetThuc != "")
                        hoSo.ThoiGianKetThuc = moment(hoSo.ThoiGianKetThuc, 'YYYY-MM-DD').format('DD/MM/YYYY');
                    hoSos.push(hoSo);
                });
                formData.append("str_hoSos", JSON.stringify(hoSos));
                formData.append("loaiTaiXuong", "data");
                $.ajax({
                    ...ajaxDefaultProps({
                        url: "/DocumentFormation/get_HoSos_download",
                        type: "POST",
                        data: formData
                    }),
                    contentType: false,
                    processData: false,
                    success: function () {
                        df.excelHoSo.save();
                    }
                })
            },
            save: function () {
                $.ajax({
                    ...ajaxDefaultProps({
                        url: "/DocumentFormation/save_Excel_HoSo",
                    }),
                    contentType: false,
                    processData: false,
                    success: function (res) {
                        if (res.status == "success") {
                            df.hoSo.dataTable.ajax.reload(function () {
                                sys.displayModal({
                                    name: '#excel-hoso',
                                    displayStatus: "hide"
                                });
                                sys.alert({ status: res.status, mess: res.mess })
                            }, false);
                        } else if (res.status == "warning") {
                            df.hoSo.dataTable.ajax.reload(function () {
                                // Đẩy lại danh sách dữ liệu chưa hợp lệ
                                df.getList_Excel_HoSo("upload");
                                df.excelHoSo.dataTable.search('').draw();
                                sys.alert({ status: "success", mess: res.mess });
                            }, false);
                        } else if (res.status == "error-0") {
                            sys.alert({ status: "error", mess: res.mess })
                        } else {
                            // Đẩy lại danh sách dữ liệu chưa hợp lệ
                            df.getList_Excel_HoSo("upload");
                            df.excelHoSo.dataTable.search('').draw();
                            sys.alert({ status: "error", mess: res.mess })
                        }
                    }
                })
            },
        };
    }
    //#endregion

    //#region Xem lịch sử hồ sơ

    //displayModal_LichSu(idHoSo = 0) {
    //    $.ajax({
    //        ...ajaxDefaultProps({
    //            url: "/DocumentFormation/displayModal_LichSu",
    //            type: "POST",
    //            data: { idHoSo }
    //        }),
    //        success: function (res) {
    //            $("#documentformation-lichsu").html(res);
    //            df.create_LichSu();
    //            /**
    //              * Gán các thuộc tính
    //              */
    //            var rows_NEW = df.hoSo_lichSu.dataTable.rows().nodes().toArray(); // Chọn phần thử đầu tiên của bảng
    //            df.hoSo_lichSu.readRow($(rows_NEW[0]));
    //            sys.displayModal({
    //                name: '#documentformation-lichsu'
    //            });
    //        }
    //    })
    //}
    create_LichSu() {
        var df = this;
        //var containerHeight = $("#lichsu-getList-container").height() - 10;
        //$("#lichsu-read-container", $("#documentformation-lichsu")).height(containerHeight);
        df.hoSo_lichSu = {
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
                    rows = df.hoSo_lichSu.dataTable.rows().nodes().toArray(),
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
    //#endregion

    //#region Ký số hàng loạt
    displayModal_KySoHangLoat() {
        var df = this;
        var idHoSos = [];
        df.hoSo.dataTable.rows().iterator('row', function (context, index) {
            var $row = $(this.row(index).node());
            if ($row.has("input.checkRow-documentformation-getList:checked").length > 0) {
                idHoSos.push($row.attr('id'));
            };
        });
        // Kiểm tra idHoSo
        if (idHoSos.length > 0) {
            var f = new FormData();
            f.append("str_idHoSos", idHoSos.toString());
            $.ajax({
                ...ajaxDefaultProps({
                    url: "/DocumentFormation/displayModal_KySoHangLoat",
                    type: "POST",
                    data: f
                }),
                contentType: false,
                processData: false,
                success: function (res) {
                    $("#documentformation-kysohangloat").html(res);
                    df.create_KySoHangLoat();
                    /**
                    * Gán các thuộc tính
                    */
                    var rows_NEW = df.kySoHangLoat.dataTable.rows().nodes().toArray(); // Chọn phần thử đầu tiên của bảng
                    df.kySoHangLoat.readRow($(rows_NEW[0]));
                    sys.displayModal({
                        name: "#documentformation-kysohangloat",
                        level: 1
                    });
                }
            })
        } else {
            sys.alert({ mess: "Bạn chưa chọn bản ghi nào", status: "warning", timeout: 1500 });
        };

    }
    create_KySoHangLoat() {
        var df = this;
        var protocol = window.location.protocol, // "https:"
            hostname = window.location.hostname, // "localhost"
            port = window.location.port, // "44345"
            base_url = `${protocol}//${hostname}${port != '' ? `:${port}` : ""}`;
        //var containerHeight = $("#hoso-getList-container", $("#documentformation-kysohangloat")).height() - 10;
        //$("#vanban-read-container", $("#documentformation-kysohangloat")).height(containerHeight);
        df.kySoHangLoat = {
            vanBanDataTables: [],
            dataTable: new DataTableCustom({
                name: "hoso-getList",
                table: $("#hoso-getList", $("#documentformation-kysohangloat")),
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
            createRow: function () { },
            deleteRow: function () {
                var rows = df.kySoHangLoat.dataTable.rows().nodes().toArray(),
                    $rowChecks = $(`.checkRow-hoso-getList:checked`, rows);
                if ($rowChecks.length == 0) {
                    sys.alert({ status: "warning", mess: "Bạn chưa chọn bản ghi nào" })
                } else {
                    $.each($rowChecks, function () {
                        var $rowCheck = $(this).closest("tr"),
                            rowNumber = $rowCheck.attr("row"),
                            $div = $(`.vanban-read[row=${rowNumber}]`, $("#documentformation-kysohangloat"));
                        df.kySoHangLoat.dataTable.row($rowCheck).remove().draw(); // Xóa dòng đó
                        $div.remove();
                    });
                    // Chọn phần thử đầu tiên của bảng
                    df.kySoHangLoat.readRow($(rows[0]));
                };
            },
            readRow: function (el) {
                var rowNumber = $(el).attr("row"),
                    rows = df.kySoHangLoat.dataTable.rows().nodes().toArray(),
                    $divs = $(".vanban-read", $("#documentformation-kysohangloat")),
                    $div = $(`.vanban-read[row=${rowNumber}]`, $("#documentformation-kysohangloat"));
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
            signFilesCallBack: function (res) {
                res = JSON.parse(res);
                var mess = res.Message;
                if (res.Status == 0) {
                    var tepVanBans = res.Files.filter(x => x.Status == 0).map(x => ({
                        ...x,
                        FileName: x.FileName.replace(base_url, "", 1) // Lấy lại đường dẫn thư mục ban đầu
                    })); // Lấy tệp ký thành công
                    var form = new FormData();
                    form.append("str_tepVanBans", JSON.stringify(tepVanBans));
                    $.ajax({
                        ...ajaxDefaultProps({
                            url: "/DocumentDigitizing/chuyenFileKySoHangLoatSang",
                            type: "POST",
                            data: form
                        }),
                        contentType: false,
                        processData: false,
                        success: function (res) {
                            sys.alert({ mess, status: res.status, timeout: 1500 });
                            sys.displayModal({
                                name: "#documentformation-kysohangloat",
                                displayStatus: "hide",
                                level: 1
                            });
                        }
                    });
                } else {
                    sys.alert({ mess: `Ký số thất bại: ${mess}`, status: "error", timeout: 1500 });
                };
            },
            // Ký nhiều văn bản
            exc_sign_files: function () {
                // Lấy từng bản ghi văn bản được chọn
                var duongDanVanBans = [];
                $.each(df.kySoHangLoat.vanBanDataTables, function (i, vanBanDataTable) {
                    vanBanDataTable.rows().iterator('row', function (context, index) {
                        var $row = $(this.row(index).node());
                        if ($row.has("input[type='checkbox']:checked").length > 0) {
                            var duongDanVanBan = $row.data("duongdanvanban");
                            duongDanVanBans.push(duongDanVanBan);
                        };
                    });
                });
                //// Ký số
                //var protocol = window.location.protocol, // "https:"
                //    hostname = window.location.hostname, // "localhost"
                //    port = window.location.port, // "44345"
                //    base_url = `${protocol}//${hostname}${port != '' ? `:${port}` : ""}`;
                var prms = {
                    FileUploadHandler: `${base_url}/FileUploadHandler.aspx`,
                    SessionId: "",
                    JWTToken: "",
                    Files: duongDanVanBans.map((duongDanTep, i) => ({
                        "FileID": i,
                        "FileName": `${base_url}/${duongDanTep}`,
                        "URL": `${base_url}/${duongDanTep}`
                    }))
                };
                vgca_sign_files(JSON.stringify(prms), df.kySoHangLoat.signFilesCallBack);
            },
        };

        var $vanBanDataTables = $(".vanban-getList", $("#documentformation-kysohangloat"));
        $.each($vanBanDataTables, function () {
            var $table = $(this),
                vanBanDataTable = new DataTableCustom({
                    name: $table.attr("id"),
                    table: $table,
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
                }).dataTable;
            // Gán check cho bảng thay thế
            //var rowCoCaus = coCauToChucDataTable.dataTable.rows().nodes().toArray(); // Tất cả dòng của bảng thay thế
            //singleCheck({
            //    name: "cocautochuc-getList",
            //    parent: $(rowCoCaus)
            //});
            //// Check dòng có idcocautochuc
            //coCauToChucDataTable.dataTable.rows().iterator('row', function (context, index) {
            //    var $row = $(this.row(index).node());
            //    $(`input[type='checkbox'][data-idcocau='${idCoCauToChuc}']`, $row).prop("checked", true);
            //});
            df.kySoHangLoat.vanBanDataTables.push(vanBanDataTable);
        });
    }
    //#endregion

    //#region Thêm zip/rar - văn bản
    displayModal_File_VanBan() {
        var df = this;
        df.getList_File_VanBan("reload");
        sys.displayModal({
            name: '#file-vanban'
        });
    }
    getList_File_VanBan(loai) {
        var df = this;
        $.ajax({
            ...ajaxDefaultProps({
                url: "/DocumentFormation/getList_File_VanBan",
                type: "POST",
                data: {
                    loai
                }
            }),
            success: function (res) {
                $("#file-vanban-getList-container").html(res);
                /**
                 * ExcelHoSo 
                 */
                df.create_File_VanBan();
                /**
                 * Gán các thuộc tính
                 */
                var rows_NEW = df.fileVanBan.dataTable.rows().nodes().toArray(); // Chọn phần thử đầu tiên của bảng
                df.fileVanBan.readRow($(rows_NEW[0]));
                $.each($(".file-vanban-read", $("#file-vanban")), function () {
                    var $div = $(this),
                        rowNumber = $div.attr("row");
                    // Gán validation
                    htmlEl.validationStates($div);
                    htmlEl.inputMask();
                    htmlEl.select2Mask($div);

                    var modalValidtion = htmlEl.activeValidationStates($div);
                });
            }
        })
    }
    create_File_VanBan() {
        var df = this;
        var containerHeight = $("#file-vanban-getList-container").height() - 10;
        $("#file-vanban-read-container", $("#file-vanban")).height(containerHeight);
        df.fileVanBan = {
            dataTable: new DataTableCustom({
                name: "file-vanban-getList",
                table: $("#file-vanban-getList"),
                props: {
                    maxHeight: containerHeight,
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

            displayModal_HinhThucNapDuLieu() {
                sys.displayModal({
                    name: '#file-vanban-hinhthucnapdulieu',
                    level: 2
                });
            },
            createRow: function () { },
            deleteRow: function () {
                var rows = df.fileVanBan.dataTable.rows().nodes().toArray(),
                    $rowChecks = $(`.checkRow-file-vanban-getList:checked`, rows);
                if ($rowChecks.length == 0) {
                    sys.alert({ status: "warning", mess: "Bạn chưa chọn bản ghi nào" })
                } else {
                    $.each($rowChecks, function () {
                        var $rowCheck = $(this).closest("tr");
                        df.fileVanBan.dataTable.row($rowCheck).remove().draw(); // Xóa dòng đó
                    });
                };
            },
            readRow: function (el) {
                var rowNumber = $(el).attr("row"),
                    rows = df.fileVanBan.dataTable.rows().nodes().toArray(),
                    $divs = $(".file-vanban-read", $("#file-vanban")),
                    $div = $(`.file-vanban-read[row=${rowNumber}]`, $("#file-vanban"));
                $divs.hide(); $div.show();
                // Chọn 1
                $(`#select-vitriluutru-${rowNumber}`).change();
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
            readFile: function (el) {
                var rowNumber = $(el).attr("name").split("-")[2],
                    val = $(el).val();
                if (val) {
                    var thongTinVanBan = val.split("~"),
                        duongDan = thongTinVanBan[0],
                        loai = thongTinVanBan[1],
                        frame = "";
                    if (loai.includes("mp4")) {
                        frame = `<video src="${duongDan}" controls style="width: 100%; height: 70vh; border: 1px solid var(--bs-body-color)"></video>`;
                    } else {
                        frame = `<iframe src="${duongDan}" style="width: 100%; height: 70vh;"></iframe>`;
                    };
                    $(`#file-${rowNumber}`, $("#file-vanban")).html(frame);
                };
            },
            updateSingleCell: function (item) {
                var text = "";
                if ($(item).is("select")) {
                    text = $('option:selected', $(item)).text();
                };
                if ($(item).is("input")) {
                    text = $(item).val();
                };
                // Gán cho span tương ứng
                var tenTruong = $(item).data("tentruong");
                $(item).siblings(`span[data-tentruong="${tenTruong}"]`).text(text);
            },
            upload: function () {
                var $select = $("#select-file-vanban").get(0),
                    kiemTra = true,
                    mess = "Tệp có kích thước vượt quá giới hạn 1GB",
                    maxSizeInBytes = 1024 * 1024 * 1024, // 1 GB,
                    real_maxSizeInBytes = 0,
                    formData = new FormData();
                $.each($select.files, function (idx, f) {
                    var extension = f.type;
                    if (/\.(zip|rar)$/i.test(f.name)) {
                        real_maxSizeInBytes += f.size;
                        if (f.size > maxSizeInBytes) {
                            kiemTra = false;
                            mess = `Tổng kích thước tệp gửi lên vượt quá giới hạn 1GB`;
                            return false; // Dừng vòng lặp khi gặp file vượt quá giới hạn
                        };
                        formData.append("files", f);
                    };
                });
                // Xóa bộ nhớ đệm để upload file trong lần tiếp theo
                $select.value = ''; // xóa giá trị của input file
                if (!kiemTra || real_maxSizeInBytes >= maxSizeInBytes) {
                    sys.alert({
                        status: "error",
                        mess,
                        timeout: 5000
                    });
                } else {
                    $.ajax({
                        ...ajaxDefaultProps({
                            url: "/DocumentFormation/upload_File_VanBan",
                            type: "POST",
                            data: formData
                        }),
                        contentType: false,
                        processData: false,
                        success: function (res) {
                            df.getList_File_VanBan("upload");
                            df.fileVanBan.dataTable.search('').draw();
                            sys.alert({ status: res.status, mess: res.mess });
                        }
                    });
                };
            },
            reload: function () {
                df.getList_File_VanBan("reload");
                df.fileVanBan.dataTable.search('').draw();
                sys.alert({ status: "success", mess: "Đã làm mới dữ liệu" });
            },
            saveByGroup: function () {
                var formData = new FormData(),
                    fileVanBans = [];
                df.fileVanBan.dataTable.rows().iterator('row', function (context, index) {
                    var $row = $(this.row(index).node()),
                        fileVanBan = {
                            MaHoSo: $("span[data-tentruong='MaHoSo'", $row).text(),
                            VanBans: [
                                {
                                    TenVanBan: $("span[data-tentruong='TenVanBan'", $row).text(),
                                    DuongDan: $("span[data-tentruong='DuongDan'", $row).text(),
                                }
                            ]
                        };
                    fileVanBans.push(fileVanBan);
                });
                formData.append("str_file_vanBans", JSON.stringify(fileVanBans));
                formData.append("loaiTaiXuong", "data");
                $.ajax({
                    ...ajaxDefaultProps({
                        url: "/DocumentFormation/get_File_VanBan_download",
                        type: "POST",
                        data: formData
                    }),
                    contentType: false,
                    processData: false,
                    success: function () {
                        df.fileVanBan.save();
                    }
                })
            },
            save: function () {
                var f = new FormData(),
                    hinhThucNapDuLieu = $("#select-hinhthucnapdulieu").val();
                f.append("hinhthucnapdulieu", hinhThucNapDuLieu);
                $.ajax({
                    ...ajaxDefaultProps({
                        url: "/DocumentFormation/save_File_VanBan",
                        type: "POST",
                        data: f
                    }),
                    contentType: false,
                    processData: false,
                    success: function (res) {
                        if (res.status == "success") {
                            df.hoSo.dataTable.ajax.reload(function () {
                                sys.displayModal({
                                    name: '#file-vanban',
                                    displayStatus: "hide"
                                });
                                sys.alert({ status: res.status, mess: res.mess })
                            }, false);
                        } else if (res.status == "warning") {
                            df.hoSo.dataTable.ajax.reload(function () {
                                // Đẩy lại danh sách dữ liệu chưa hợp lệ
                                df.getList_File_VanBan("upload");
                                df.fileVanBan.dataTable.search('').draw();
                                sys.alert({ status: "success", mess: res.mess });
                            }, false);
                        } else if (res.status == "error-0") {
                            sys.alert({ status: "error", mess: res.mess })
                        } else {
                            // Đẩy lại danh sách dữ liệu chưa hợp lệ
                            df.getList_File_VanBan("upload");
                            df.fileVanBan.dataTable.search('').draw();
                            sys.alert({ status: "error", mess: res.mess })
                        };
                        sys.displayModal({
                            name: '#file-vanban-hinhthucnapdulieu',
                            displayStatus: "hide"
                        });
                    }
                })
            },
        };
    }
    //#endregion
};