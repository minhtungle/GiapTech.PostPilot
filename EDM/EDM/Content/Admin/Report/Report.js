'use strict'
class Report {
    constructor() {
        this.page = $("#page-report");
    }
    init() {
        var report = this;
        sys.activePage({
            page: report.page.attr("id"),
        });
        this.taoChart();
    }
    taoChart() {
        const lineChart = $('#thongkehoso-lineChart');
        const doughnut = $('#thongkehoso-doughnut');
        const polarArea = $('#thongkehoso-polarArea');

        var hoso_thang1 = $("#hoso-thang1").val(),
            hoso_thang2 = $("#hoso-thang2").val(),
            hoso_thang3 = $("#hoso-thang3").val(),
            hoso_thang4 = $("#hoso-thang4").val(),
            hoso_thang5 = $("#hoso-thang5").val(),
            hoso_thang6 = $("#hoso-thang6").val(),
            hoso_thang7 = $("#hoso-thang7").val(),
            hoso_thang8 = $("#hoso-thang8").val(),
            hoso_thang9 = $("#hoso-thang9").val(),
            hoso_thang10 = $("#hoso-thang10").val(),
            hoso_thang11 = $("#hoso-thang11").val(),
            hoso_thang12 = $("#hoso-thang12").val();

        var data = {
            labels: ['T1', 'T2', 'T3', 'T4', 'T5', 'T6', 'T7', 'T8', 'T9', 'T10', 'T11', 'T12'],
            datasets: [{
                label: "Dung lượng hồ sơ",
                data: [hoso_thang1, hoso_thang2, hoso_thang3, hoso_thang4, hoso_thang5, hoso_thang6, hoso_thang7, hoso_thang8, hoso_thang9, hoso_thang10, hoso_thang11, hoso_thang12],
                backgroundColor: [
                    "#003285",
                    "#028391",
                    "#F6DCAC",
                    "#FEAE6F",
                    "#006769",
                    "#40A578",
                    "#9DDE8B",
                    "#E6FF94",
                    "#6DC5D1",
                    "#A91D3A",
                    "#C73659",
                    "#7469B6",
                ],
                hoverOffset: 4,
                borderWidth: 1
            }]
        };
        var options = {
            scales: {
                y: {
                    beginAtZero: true
                }
            }
        };
        new Chart(lineChart, {
            type: 'line',
            data,
            options
        });
        new Chart(doughnut, {
            type: 'doughnut',
            data,
            options
        });
        new Chart(polarArea, {
            type: 'polarArea',
            data,
            options
        });
    }
}
class ThongKeTheoTieuChi {
    constructor() {
        this.thongTinTimKiem = {};
        this.ketQuaTimKiem = [];
        this.danhMucHoSo = {
            dataTable: null,
            dataTable_BaoCao: null,
        };
        this.viTriLuuTru = {
            dataTable: null,
            dataTable_BaoCao: null,
        };
        this.coCauToChuc = {
            dataTable: null,
            dataTable_BaoCao: null,
        };
    }
    init() {
        var thongKe = this;
        thongKe.page = $("#page-report");
        thongKe.danhMucHoSo.dataTable_BaoCao = new DataTableCustom({
            name: "report-danhmuchoso-getListt",
            table: $("#report-danhmuchoso-getListt"),
            props: {
                ordering: false,
                dom: `
                    <'row'<'col-12 mb-3 d-none'B>>
                    <'row'<'col-sm-12 col-md-6'l><'col-sm-12 col-md-6'f>>
                    <'row'<'col-sm-12'rt>>
                    <'row'<'col-sm-12 col-md-5'i><'col-sm-12 col-md-7 pt-2'p>>`,
                lengthMenu: [
                    [10, 20, 50, -1],
                    [10, 20, 50, 'Tất cả'],
                ],
                rowGroup: {
                    dataSrc: function (row) {
                        let tenNhom = row.DanhMucHoSo.TenDanhMucHoSo.trim();
                        //let tenNhom = row.PhongLuuTru.TenPhongLuuTru.trim();
                        return tenNhom.split(".")[-1];
                    },
                    debug: true,
                    drawGrouping: 'console'
                },
                ajax: {
                    url: '/Report/timKiem',
                    type: "GET",
                    data: function (d) {
                        d.str_thongTinTimKiem = JSON.stringify(thongKe.thongTinTimKiem);
                    },
                    complete: function (data) {
                        thongKe.ketQuaTimKiem = data.responseJSON.data;
                    },
                },
                rowId: 'IdHoSo',
                columns: [
                    { data: "MaHoSo" },
                    { data: "So_KyHieu" },
                    { data: "TieuDeHoSo" },
                    { data: "PhongLuuTru.TenPhongLuuTru" },
                    {
                        data: null,
                        className: "text-center",
                        render: function (data, type, row, meta) {
                            let thoiGianBatDau = moment(data.ThoiGianBatDau).format('DD-MM-YYYY'),
                                thoiGianKetThuc = moment(data.ThoiGianKetThuc).format('DD-MM-YYYY');
                            return [thoiGianBatDau, thoiGianKetThuc].join(" - ");
                        }
                    },
                    {
                        data: null,
                        className: "text-center",
                        render: function (data, type, row, meta) {
                            let soLuongTo = data.SoLuongTo,
                                soLuongTrang = data.SoLuongTrang;
                            return `${soLuongTo} tờ/${soLuongTrang} trang`;
                        }
                    },
                    { data: "ThoiHanBaoQuan" },
                    { data: "ThongTinNguoiTao.TenNguoiDung" },
                    { data: "DonViSuDung.TenDonViSuDung" },
                    { data: "GhiChu" },
                ],
            }
        }).dataTable;
        // Gọi mặc định
        thongKe.getList();
        sys.activePage({
            page: thongKe.page.attr("id"),
        });
    }
    getList() {
        var thongKe = this;
        var formData = new FormData();
        formData.append("thongTinTimKiem", JSON.stringify(thongKe.thongTinTimKiem));
        $.ajax({
            ...ajaxDefaultProps({
                url: "/Report/timKiem",
                type: "POST",
                data: formData,
            }),
            contentType: false,
            processData: false,
            success: function (res) {
                [{
                    dataTable: "danhMucHoSo",
                    container: $("#danhmuchoso-getList-container"),
                    body: res.danhmuchoso_view,
                    name: "report-danhmuchoso-getList",
                }, {
                    dataTable: "viTriLuuTru",
                    container: $("#vitriluutru-getList-container"),
                    body: res.vitriluutru_view,
                    name: "report-vitriluutru-getList",
                }, {
                    dataTable: "coCauToChuc",
                    container: $("#cocautochuc-getList-container"),
                    body: res.cocautochuc_view,
                    name: "report-cocautochuc-getList",
                }].map(x => {
                    x.container.html(x.body);
                    thongKe[x.dataTable].dataTable_BaoCao = new DataTableCustom({
                        name: x.name,
                        table: $("#" + x.name),
                        props: {
                            ordering: false,
                            dom: `
                                <'row'<'col-12 mb-3 d-none'B>>
                                <'row'<'col-sm-12 col-md-6'l><'col-sm-12 col-md-6'f>>
                                <'row'<'col-sm-12'rt>>
                                <'row'<'col-sm-12 col-md-5'i><'col-sm-12 col-md-7 pt-2'p>>`,
                            lengthMenu: [
                                [10, 20, 50, -1],
                                [10, 20, 50, 'Tất cả'],
                            ],
                            buttons: [
                                {
                                    extend: 'excelHtml5',
                                    title: ``,
                                }
                            ],
                            //rowGroup: {
                            //    dataSrc: function (row) {
                            //        let tenNhom = row.DanhMucHoSo.TenDanhMucHoSo.trim();
                            //        //let tenNhom = row.PhongLuuTru.TenPhongLuuTru.trim();
                            //        return tenNhom.split(".")[-1];
                            //    },
                            //},
                        }
                    }).dataTable;
                });
            }
        })
    }
    xuatTep_(btn) {
        var thongKe = this;
        //thongKe.dataTable_BaoCao.buttons(btn).trigger();
        var danhsach = $("#report-danhmuchoso-getList").html().trim();
        var uri = 'data:application/vnd.ms-excel;base64,';
        var template = '<html xmlns:o="urn:schemas-microsoft-com:office:office"' +
            ' xmlns:x="urn:schemas-microsoft-com:office:excel" xmlns="http://www.w3.org/TR/REC-html40">' +
            '<head><!--[if gte mso 9]><xml><x:ExcelWorkbook><x:ExcelWorksheets><x:ExcelWorksheet><x:Name>{worksheet}</x:Name>' +
            '<x:WorksheetOptions><x:DisplayGridlines/></x:WorksheetOptions></x:ExcelWorksheet></x:ExcelWorksheets></x:ExcelWorkbook>' +
            '</xml><![endif]--></head><body><table>{table}</table></body></html>';
        var base64 = function (s) {
            return window.btoa(unescape(encodeURIComponent(s)));
        };
        var format = function (s, c) {
            return s.replace(/{(\w+)}/g, function (m, p) {
                return c[p];
            })
        };
        var ctx = {
            worksheet: 'Worksheet',
            table: danhsach
        };
        var link = document.getElementById("xuatTep");
        link.download = "EDM-BaoCao.xlsx";
        link.href = uri + base64(format(template, ctx));
        link.click();
    }
    xuatTep() {
        var tableID = "report-danhmuchoso-getList";
        var filename = "EDM-BaoCao"
        // Lấy bảng HTML
        var table = document.getElementById(tableID);
        var workbook = XLSX.utils.table_to_book(table, { sheet: "Sheet1" });

        // Xuất file Excel
        XLSX.writeFile(workbook, filename ? filename + '.xlsx' : 'export.xlsx');
    }
    timKiemThongTin() {
        var thongKe = this;
        var thongTinTimKiem = {
            MaHoSo: $("#input-mahoso", $("#thongketheotieuchi-timkiem")).val().trim(),
            TieuDeHoSo: $("#input-tieudehoso", $("#thongketheotieuchi-timkiem")).val().trim(),
            So_KyHieu: $("#input-sokyhieu", $("#thongketheotieuchi-timkiem")).val().trim(),
            IdDanhMucHoSos: [],
            IdViTriLuuTrus: [],
            //IdCoCauToChucs: [],
            IdPhongLuuTru: $("#select-phongluutru", $("#thongketheotieuchi-timkiem")).val(),
            PhongLuuTru: {
                IdPhongLuuTru: $("#select-phongluutru", $("#thongketheotieuchi-timkiem")).val(),
            },
            ThoiHanBaoQuan: $("#input-thoihanbaoquan", $("#thongketheotieuchi-timkiem")).val().trim(),
            ThoiGianBatDau: $("#input-thoigianbatdau", $("#thongketheotieuchi-timkiem")).val().trim(),
            ThoiGianKetThuc: $("#input-thoigianketthuc", $("#thongketheotieuchi-timkiem")).val().trim(),
            SoLuongTo: $("#input-soluongto", $("#thongketheotieuchi-timkiem")).val().trim(),
            SoLuongTrang: $("#input-soluongtrang", $("#thongketheotieuchi-timkiem")).val().trim(),
            _NguoiTao: {
                TenNguoiDung: $("#input-nguoitao", $("#thongketheotieuchi-timkiem")).val().trim(),
            },
            _DonViSuDung: {
                TenDonViSuDung: $("#input-donvisudung", $("#thongketheotieuchi-timkiem")).val().trim(),
            },
            GhiChu: $("#input-ghichu", $("#thongketheotieuchi-timkiem")).val().trim(),
        };
        if (thongTinTimKiem.ThoiGianBatDau != "")
            thongTinTimKiem.ThoiGianBatDau = moment(thongTinTimKiem.ThoiGianBatDau, 'YYYY-MM-DD').format('DD/MM/YYYY');
        if (thongTinTimKiem.ThoiGianKetThuc != "")
            thongTinTimKiem.ThoiGianKetThuc = moment(thongTinTimKiem.ThoiGianKetThuc, 'YYYY-MM-DD').format('DD/MM/YYYY');
        thongKe.danhMucHoSo.dataTable.rows().iterator('row', function (context, index) {
            var $row = $(this.row(index).node());
            if ($row.has("input:checked").length > 0) {
                thongTinTimKiem.IdDanhMucHoSos.push($row.attr('id'));
            };
        });
        thongKe.viTriLuuTru.dataTable.rows().iterator('row', function (context, index) {
            var $row = $(this.row(index).node());
            if ($row.has("input:checked").length > 0) {
                thongTinTimKiem.IdViTriLuuTrus.push($row.attr('id'));
            };
        });
        thongKe.coCauToChuc.dataTable.rows().iterator('row', function (context, index) {
            var $row = $(this.row(index).node());
            if ($row.has("input:checked").length > 0) {
                thongTinTimKiem.IdCoCauToChucs.push($row.attr('id'));
            };
        });
        thongKe.thongTinTimKiem = thongTinTimKiem;
        //thongKe.dataTable_BaoCao.ajax.reload();

        thongKe.getList();
        sys.displayModal({
            name: "#thongketheotieuchi-timkiem",
            displayStatus: "hide",
        });
    }
    chonThoiHanBaoQuan(checkBox, input) {
        var thongKe = this;
        var checked = $(checkBox).is(":checked");
        input.attr("disabled", checked);
        if (checked) {
            input.val("Vĩnh viễn");
        } else {
            input.val("")
        };
    }
    displayModal_TimKiem_TheoTieuChi() {
        var thongKe = this;
        $.ajax({
            ...ajaxDefaultProps({
                url: "/Report/displayModal_TimKiem_TheoTieuChi",
                type: "GET",
            }),
            success: function (res) {
                $("#thongketheotieuchi-timkiem").html(res);
                [{
                    dataTable: "danhMucHoSo",
                    name: "danhmuchoso-getList",
                    table: $("#danhmuchoso-getList", $("#thongketheotieuchi-timkiem"))
                }, {
                    dataTable: "viTriLuuTru",
                    name: "vitriluutru-getList",
                    table: $("#vitriluutru-getList", $("#thongketheotieuchi-timkiem"))
                }, {
                    dataTable: "coCauToChuc",
                    name: "cocautochuc-getList",
                    table: $("#cocautochuc-getList", $("#thongketheotieuchi-timkiem"))
                },].map(x => {
                    thongKe[x.dataTable].dataTable = new DataTableCustom({
                        name: x.name,
                        table: x.table,
                        props: {
                            ordering: false,
                            maxHeight: 500,
                            dom: `<'row'<'col-sm-12'rt>>
                                  <'row'<'col-sm-12 col-md-5'i><'col-sm-12 col-md-7 pt-2'p>>`,
                            lengthMenu: [
                                [-1],
                                ['Tất cả'],
                            ],
                        }
                    }).dataTable;
                    /**
                     * Table-checkbox
                     **/
                    treeView({ $table: x.table });
                });
                sys.displayModal({
                    name: "#thongketheotieuchi-timkiem",
                });
            }
        });
    }
}