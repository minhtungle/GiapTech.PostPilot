'use strict'
/**
 * main
 * */
class QuanLyDoanhThu {
    constructor() {
        this.page;
        this.pageGroup;
        this.nguoiDung = {
            data: [],
            dataTable: null,
            save: function () { },
            delete: function () { },
            displayModal_CRUD: function () { },
        };
        this.danhMuc = {
            getList: function () { },
            data: [],
            selected: [],
            treeSelect: null,
        };
        this.locThongTin = {
            $container: {
                tongQuan: $("#tongquan-getList"),
                chiTiet: $("#chitiet-getList"),
                soSanh: $("#sosanh-getList"),
            },
            chiTiet: {
                LoaiThongKe: $("#select-loaithongke", $("#chitiet-getList")).val(),
                ThoiGian: $("#input-thoigian", $("#chitiet-getList")).val(),
                IdDanhMucChas: [],
            },
            tongQuan: {
                LoaiThongKe: $("#select-loaithongke", $("#tongquan-getList")).val(),
                ThoiGian: $("#input-thoigian", $("#tongquan-getList")).val(),
            },
            soSanh: {
                LoaiThongKe: $("#select-loaithongke", $("#sosanh-getList")).val(),
                ThoiGian1: $("#input-thoigian", $("#sosanh-getList")).val(),
                ThoiGian2: $("#input-thoigian", $("#sosanh-getList")).val(),
            }
        }
    }
    init() {
        var quanLyDoanhThu = this;
        var idNguoiDung_DangSuDung = $("#input-idnguoidung-dangsudung").val();
        quanLyDoanhThu.page = $("#page-quanlydoanhthu");
        htmlEl = new HtmlElement();

        quanLyDoanhThu.main = {
            displayModal_CRUD: function () {
                $.ajax({
                    ...ajaxDefaultProps({
                        url: "/QuanLyDoanhThu/displayModal_CRUD",
                        type: "GET",
                    }),
                    success: function (res) {
                        $("#quanlydoanhthu-crud").html(res);
                        sys.displayModal({
                            name: '#quanlydoanhthu-crud'
                        });
                    }
                })
            },
            save: function () {
                // Lấy dữ liệu
                var mucTieus = [];
                $.each($("table tbody tr", $("#quanlydoanhthu-crud")), function () {
                    mucTieus.push({
                        Thang: $(this).data("thang"),
                        DoanhThu: {
                            IdCoCauToChuc_DoanhThu: $(this).data("id"),
                            DoanhThuMucTieu: $("input", $(this)).val() ? parseInt($("input", $(this)).val().replaceAll(' ', '')) : 0,
                        }
                    });
                });
                // Kiểm tra tháng hiện tại
                var f = new FormData();
                f.append("str_mucTieus", JSON.stringify(mucTieus));
                $.ajax({
                    ...ajaxDefaultProps({
                        url: "/QuanLyDoanhThu/create_MucTieuDoanhThu_CoCau",
                        type: "POST",
                        data: f
                    }),
                    contentType: false,
                    processData: false,
                    success: function (res) {
                        if (res.status == "success") {
                            quanLyDoanhThu.locThongTin.timKiem();
                            sys.displayModal({
                                name: '#quanlydoanhthu-crud',
                                displayStatus: "hide"
                            });
                        };
                        sys.alert({ status: res.status, mess: res.mess })
                    }
                })
            }
        };
        quanLyDoanhThu.locThongTin = {
            ...quanLyDoanhThu.locThongTin,
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
            reload: function () {
                quanLyDoanhThu.locThongTin.chiTiet = {
                    LoaiThongKe: $("#select-loaithongke", quanLyDoanhThu.locThongTin.$container.chiTiet).val(),
                    IdDanhMucChas: quanLyDoanhThu.danhMuc.selected,
                    IdThongTin: $("#select-thongtin", quanLyDoanhThu.locThongTin.$container.chiTiet).val(),
                    ThoiGian: $("#input-thoigian", quanLyDoanhThu.locThongTin.$container.chiTiet).val()
                };
                quanLyDoanhThu.locThongTin.tongQuan = {
                    LoaiThongKe: $("#select-loaithongke", quanLyDoanhThu.locThongTin.$container.tongQuan).val(),
                    ThoiGian: $("#input-thoigian", quanLyDoanhThu.locThongTin.$container.tongQuan).val()
                };
            },
            timKiem: function () {
                var $loai = $("#tab-loaithongke");
                if ($("#tongquan-getList-list", $loai).hasClass("active")) {
                    quanLyDoanhThu.thongKe.tongQuan.timKiem();
                } else if ($("#chitiet-getList-list", $loai).hasClass("active")) {
                    quanLyDoanhThu.thongKe.chiTiet.timKiem();
                } else {
                    quanLyDoanhThu.thongKe.soSanh.timKiem();
                };
                quanLyDoanhThu.locThongTin.displayModal_TimKiem("hide")
            }
        };
        quanLyDoanhThu.thongKe = {
            ...quanLyDoanhThu.thongKe,
            tongQuan: {
                chartTheoNam: null,
                chonLoaiThongKe: function () {
                    quanLyDoanhThu.locThongTin.reload();
                },
                timKiem: function () {
                    quanLyDoanhThu.locThongTin.reload();
                    $.ajax({
                        ...ajaxDefaultProps({
                            url: "/QuanLyDoanhThu/thongKeTongQuan",
                            type: "POST",
                            contentType: "application/json; charset=utf-8", // Định nghĩa kiểu nội dung là JSON
                            data: {
                                locThongTin: quanLyDoanhThu.locThongTin.tongQuan
                            }, // Truyền dữ liệu trực tiếp
                        }),
                        success: function (res) {
                            $("#tongquan-getList-container").html(res.view);

                            //#region Tạo dữ liệu chart
                            // Nếu đã có biểu đồ, hủy nó
                            if (quanLyDoanhThu.thongKe.tongQuan.chartTheoNam) {
                                quanLyDoanhThu.thongKe.tongQuan.chartTheoNam.destroy();
                            };

                            var thongKeTongQuan = res.ThongKeTongQuan;
                            var monthNames = []; // Danh sách tên tháng

                            var chartData = {}

                            // Duyệt qua từng phần tử trong `thongKeTongQuan`
                            thongKeTongQuan.forEach(x => {
                                /**
                                  * Chuyển giá trị các loại thống kê thành dạng arr
                                     var arr = [
                                        {
                                            Name: "Tung",
                                            Age: 20
                                        }, {
                                            Name: "HaiAnh",
                                            Age: 10
                                        }
                                    ];
                                    rs = {
                                      Name: ["Tung", "HaiAnh"],
                                      Age: [20, 10]
                                    }
                                 */
                                monthNames.push(`Tháng ${x.Thang}`); // Thêm tháng

                                for (let key in x) {
                                    if (!chartData[key]) {
                                        chartData[key] = []; // Nếu chưa có mảng, khởi tạo
                                    };
                                    chartData[key].push(x[key] || 0); // Thêm giá trị vào mảng
                                }
                            });
                            delete chartData.Thang;
                            // Tạo chart
                            {
                                // Mảng nhãn tương ứng với từng key trong chartData
                                const labels = {
                                    DoanhThuMucTieu: "Doanh thu mục tiêu",
                                    DoanhThuThucTe: "Doanh thu thực tế",
                                    SoLuongKhachHang: "Số lượng khách hàng",
                                    SoLuongDonHang: "Số lượng đơn hàng",
                                    SoLuongThanhToan: "Số lượng thanh toán",
                                    SoLuongSanPham_TiengAnh: "Số lượng sản phẩm Tiếng Anh",
                                    SoLuongSanPham_TiengDuc: "Số lượng sản phẩm Tiếng Đức",
                                };

                                // Màu sắc dễ nhìn
                                const colors = [
                                    'rgb(255, 99, 132)', // Đỏ
                                    'rgb(54, 162, 235)', // Xanh dương
                                    'rgb(255, 206, 86)', // Vàng
                                    'rgb(75, 192, 192)', // Xanh ngọc
                                    'rgb(153, 102, 255)', // Tím
                                    'rgb(255, 159, 64)', // Cam
                                    'rgb(201, 203, 207)', // Xám
                                ];

                                // Tạo datasets
                                const datasets = Object.keys(chartData).map((key, index) => ({
                                    type: 'line',
                                    label: labels[key] || key, // Gán nhãn từ labels hoặc giữ nguyên nếu không có nhãn
                                    data: chartData[key],
                                    fill: false,
                                    borderColor: colors[index % colors.length], // Lặp lại màu khi hết danh sách
                                }));

                                const config = {
                                    type: 'scatter',
                                    data: {
                                        labels: monthNames,
                                        datasets: datasets,
                                    },
                                    options: { scales: { y: { beginAtZero: true } } }
                                };

                                quanLyDoanhThu.thongKe.tongQuan.chartTheoNam = new Chart($("#chart-theonam-container", $("#tongquan-getList"))[0], config);
                            }
                            //#endregion
                        }
                    });
                }
            },
            chiTiet: {
                chartTheoNam: null,
                chonLoaiThongKe: function () {
                    quanLyDoanhThu.locThongTin.reload();
                    $.ajax({
                        ...ajaxDefaultProps({
                            url: "/QuanLyDoanhThu/layDanhMucThongKe",
                            type: "POST",
                            data: {
                                loaiThongKe: quanLyDoanhThu.locThongTin.chiTiet.LoaiThongKe
                            }
                        }),
                        success: function (res) {
                            if (res.length > 0) {
                                var MAKETREEDATA = (thongTins) => {
                                    var nodes = [];
                                    $.each(thongTins, function (iThongTin, thongTin) {
                                        let _thongTin = quanLyDoanhThu.locThongTin.chiTiet.LoaiThongKe == "NVKD"
                                            ? {
                                                name: `${thongTin.root.TenCoCauToChuc}`,
                                                value: thongTin.root.IdCoCauToChuc,
                                                //children: [] // Nếu có thì mới thêm vì sẽ mặc định hiện icon collapse nếu có thuộc tính này
                                            } : {
                                                name: `${thongTin.root.TenLoaiSanPham}`,
                                                value: thongTin.root.IdLoaiSanPham,
                                            };
                                        if (thongTin.nodes.length > 0) _thongTin.children = MAKETREEDATA(thongTin.nodes);
                                        nodes.push(_thongTin);
                                    });
                                    return nodes;
                                };
                                quanLyDoanhThu.danhMuc.data = res;
                                $("#treeSelect_danhmuc_container").html("");
                                quanLyDoanhThu.danhMuc.treeSelect = new TreeSelectCustom({
                                    props: {
                                        parentHtmlContainer: document.getElementById("treeSelect_danhmuc_container"),
                                        isIndependentNodes: true, // Không gộp phần tử con thành phần tử cha khi chọn tất cả
                                        listClassName: 'treeselect-list-item',
                                        placeholder: 'Nhập thông tin tìm kiếm ...',
                                        options: MAKETREEDATA(quanLyDoanhThu.danhMuc.data),
                                        id: 'treeSelect_danhmuc',
                                        alwaysOpen: false,
                                        //value: options,
                                        inputCallback: function (value) {
                                            quanLyDoanhThu.danhMuc.selected = value; // Thêm vào danh sách
                                            //quanLyDoanhThu.thongKe.chiTiet.chonDanhMucThongTin();
                                        },
                                    }
                                }).init();
                            }
                        }
                    })
                },
                chonDanhMucThongTin: function () { // Khoong dung
                    quanLyDoanhThu.locThongTin.reload();
                    $.ajax({
                        ...ajaxDefaultProps({
                            url: "/QuanLyDoanhThu/chonDanhMucThongTin",
                            type: "POST",
                            contentType: "application/json; charset=utf-8", // Định nghĩa kiểu nội dung là JSON
                            data: {
                                locThongTin: quanLyDoanhThu.locThongTin.chiTiet
                            }, // Truyền dữ liệu trực tiếp
                        }),
                        success: function (res) {
                            $("#danhsachtheodanhmuc-container").html(res);
                            //htmlEl = new HtmlElement();
                            quanLyDoanhThu.locThongTin.dataTable = new DataTableCustom({
                                name: "thongtin-getList",
                                table: $("#thongtin-getList"),
                                props: {
                                    dom: `
                                <'row'<'col-sm-12'rt>>
                                <'row'<'col-sm-12 pt-2 text-center'p>>`,
                                }
                            }).dataTable;
                        }
                    });
                },
                thongKeChiTiet: function (idThongTin = '00000000-0000-0000-0000-000000000000') {
                    quanLyDoanhThu.locThongTin.reload();
                    quanLyDoanhThu.locThongTin.chiTiet.IdThongTin = idThongTin;
                    if (quanLyDoanhThu.locThongTin.chiTiet.IdThongTin != '00000000-0000-0000-0000-000000000000') {
                        $.ajax({
                            ...ajaxDefaultProps({
                                url: quanLyDoanhThu.locThongTin.chiTiet.LoaiThongKe == "NVKD" ? "/QuanLyDoanhThu/thongKeChiTietTheoNVKD" : "/QuanLyDoanhThu/thongKeChiTietTheoSP",
                                type: "POST",
                                contentType: "application/json; charset=utf-8", // Định nghĩa kiểu nội dung là JSON
                                data: {
                                    locThongTin: quanLyDoanhThu.locThongTin.chiTiet
                                }, // Truyền dữ liệu trực tiếp
                            }),
                            success: function (res) {
                                $("#chitiet-container").html(res.view);
                                quanLyDoanhThu.thongKe.chiTiet.chonDanhMucThongTin();

                                //#region Tạo dữ liệu chart
                                // Nếu đã có biểu đồ, hủy nó
                                if (quanLyDoanhThu.thongKe.chiTiet.chartTheoNam) {
                                    quanLyDoanhThu.thongKe.chiTiet.chartTheoNam.destroy();
                                };

                                var thongKeTongQuan = res.ThongKeTongQuan;
                                var monthNames = []; // Danh sách tên tháng

                                var chartData = {}

                                // Duyệt qua từng phần tử trong `thongKeTongQuan`
                                thongKeTongQuan.forEach(x => {
                                    /**
                                      * Chuyển giá trị các loại thống kê thành dạng arr
                                         var arr = [
                                            {
                                                Name: "Tung",
                                                Age: 20
                                            }, {
                                                Name: "HaiAnh",
                                                Age: 10
                                            }
                                        ];
                                        rs = {
                                          Name: ["Tung", "HaiAnh"],
                                          Age: [20, 10]
                                        }
                                     */
                                    monthNames.push(`Tháng ${x.Thang}`); // Thêm tháng

                                    for (let key in x) {
                                        if (!chartData[key]) {
                                            chartData[key] = []; // Nếu chưa có mảng, khởi tạo
                                        };
                                        chartData[key].push(x[key] || 0); // Thêm giá trị vào mảng
                                    }
                                });
                                delete chartData.Thang;
                                // Tạo chart
                                {
                                    // Mảng nhãn tương ứng với từng key trong chartData
                                    const labels = {
                                        DoanhThuMucTieu: "Doanh thu mục tiêu",
                                        DoanhThuThucTe: "Doanh thu thực tế",
                                        SoLuongKhachHang: "Số lượng khách hàng",
                                        SoLuongDonHang: "Số lượng đơn hàng",
                                        SoLuongThanhToan: "Số lượng thanh toán",
                                        SoLuongSanPham_TiengAnh: "Số lượng sản phẩm Tiếng Anh",
                                        SoLuongSanPham_TiengDuc: "Số lượng sản phẩm Tiếng Đức",
                                    };

                                    // Màu sắc dễ nhìn
                                    const colors = [
                                        'rgb(255, 99, 132)', // Đỏ
                                        'rgb(54, 162, 235)', // Xanh dương
                                        'rgb(255, 206, 86)', // Vàng
                                        'rgb(75, 192, 192)', // Xanh ngọc
                                        'rgb(153, 102, 255)', // Tím
                                        'rgb(255, 159, 64)', // Cam
                                        'rgb(201, 203, 207)', // Xám
                                    ];

                                    // Tạo datasets
                                    const datasets = Object.keys(chartData).map((key, index) => ({
                                        type: 'line',
                                        label: labels[key] || key, // Gán nhãn từ labels hoặc giữ nguyên nếu không có nhãn
                                        data: chartData[key],
                                        fill: false,
                                        borderColor: colors[index % colors.length], // Lặp lại màu khi hết danh sách
                                    }));

                                    const config = {
                                        type: 'scatter',
                                        data: {
                                            labels: monthNames,
                                            datasets: datasets,
                                        },
                                        options: { scales: { y: { beginAtZero: true } } }
                                    };

                                    quanLyDoanhThu.thongKe.chiTiet.chartTheoNam = new Chart($("#chart-theonam-container", $("#chitiet-getList"))[0], config);
                                }
                                //#endregion
                            }
                        });
                    };
                },
                hienThiXemThongKeChiTiet: function (idThongTin = '00000000-0000-0000-0000-000000000000') {
                    if (idThongTin == '00000000-0000-0000-0000-000000000000') {
                        $("#danhsachtheodanhmuc-container").show();
                        $("#chitiet-container").hide();
                    } else {
                        $("#danhsachtheodanhmuc-container").hide();
                        $("#chitiet-container").show();
                        quanLyDoanhThu.thongKe.chiTiet.thongKeChiTiet(idThongTin);
                    };
                },
                timKiem: function () {
                    quanLyDoanhThu.thongKe.chiTiet.chonDanhMucThongTin();
                    //quanLyDoanhThu.thongKe.chiTiet.thongKeChiTiet();
                }
            },
            soSanh: {
                timKiem: function () { }
            },
        };
        quanLyDoanhThu.setEventListener();
        quanLyDoanhThu.thongKe.tongQuan.timKiem();
        quanLyDoanhThu.thongKe.chiTiet.chonLoaiThongKe();
        sys.activePage({
            page: quanLyDoanhThu.page.attr("id"),
            pageGroup: quanLyDoanhThu.pageGroup
        });
    };
    setEventListener() {
        var quanLyDoanhThu = this;
        $(".button-timkiem").off().on("click", function () {
            quanLyDoanhThu.locThongTin.timKiem();
        });
        $("#select-loaithongke", quanLyDoanhThu.locThongTin.$container.tongQuan).off().on("change", function () {
            quanLyDoanhThu.thongKe.tongQuan.chonLoaiThongKe();
        });
        $("#select-loaithongke", quanLyDoanhThu.locThongTin.$container.chiTiet).off().on("change", function () {
            quanLyDoanhThu.thongKe.chiTiet.chonLoaiThongKe();
        });
    }
};