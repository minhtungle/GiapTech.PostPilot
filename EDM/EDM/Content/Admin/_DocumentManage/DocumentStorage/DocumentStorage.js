'use strict'
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
                    orderable: true,
                    target: [0]
                },
                {
                    className: 'text-center',
                    target: [-1]
                }],
            },
            initCompleteProps: function () { }
        }).dataTable;
    }
}
/**
 * main
 * */
class DocumentStorage {
    constructor() {
        this.page;
        this.pageGroup;
        this.viTriLuuTru = {
            data: [],
            treeView: null,
            nodeId: 0,
            idViTriLuuTru: 0,
            getList: function () { },
        };
        this.hoSo = {
            data: [],
            dataTable: null,
            getList: function () { },
            huyNopLuu: function () { }
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
        var ds = this;
        var idNguoiDung_DangSuDung = $("#input-idnguoidung-dangsudung").val();
        ds.page = $("#page-documentstorage");
        ds.pageGroup = this.page.attr("page-group");
        ds.viTriLuuTru = {
            ...ds.viTriLuuTru,
            getList: function () {
                $.ajax({
                    ...ajaxDefaultProps({
                        url: "/DocumentStorage/get_ViTriLuuTrus",
                        type: "GET",
                    }),
                    success: function (res) {
                        if (res.length > 0) {
                            var MAKETREEDATA = (vts) => {
                                var nodes = [];
                                $.each(vts, function (i, vt) {
                                    let _vt = {
                                        text: vt.root.TenViTriLuuTru,
                                        //href: `/DocumentStorage/get_HoSos?idHoSo=${vt.root.IdViTriLuuTru}`,
                                        IdViTriLuuTru: vt.root.IdViTriLuuTru,
                                        //tags: [vt.nodes.length.toString()],
                                        //nodes: [] // Nếu có thì mới thêm vì sẽ mặc định hiện icon collapse nếu có thuộc tính này
                                    }
                                    if (vt.nodes.length > 0) _vt.nodes = MAKETREEDATA(vt.nodes);
                                    nodes.push(_vt);
                                });
                                return nodes;
                            };
                            ds.viTriLuuTru.data = res;
                            ds.viTriLuuTru.treeView = new TreeViewCustom({
                                name: "#vitriluutru-getList",
                                props: {
                                    data: MAKETREEDATA(ds.viTriLuuTru.data),
                                    onNodeSelected: function (event, data) {
                                        ds.viTriLuuTru.idViTriLuuTru = data.IdViTriLuuTru;
                                        ds.viTriLuuTru.nodeId = data.nodeId;
                                        $('#vitriluutru-getList').treeview('expandNode', [data.nodeId, { levels: 0, silent: true }]);
                                        ds.hoSo.dataTable.ajax.reload();
                                    }
                                }
                            }).init();
                        }
                    }
                })
            },
            timKiem: function () {
                // Đóng tất cả
                $('#vitriluutru-getList').treeview('collapseAll', { silent: true });
                // Tìm kiếm
                var tenViTriTimKiem = $('#timkiem-vitriluutru').val();
                var ketQua = $('#vitriluutru-getList').treeview('search', [tenViTriTimKiem, { ignoreCase: true, exactMatch: false }]);
                if (ketQua.length > 0) {
                    // Kéo xuống kết quả đầu tiên
                    var $container = document.getElementById("vitriluutru-getList"),
                        $ketQua = document.querySelector(`#vitriluutru-getList li[data-nodeid="${ketQua[0].nodeId}"]`);
                    $container.scrollTop = $ketQua.offsetTop - $container.offsetTop;
                };
            }
        }
        ds.hoSo = {
            ...ds.hoSo,
            getList: function () {
                ds.hoSo.dataTable = new DataTableCustom({
                    name: "hoso-getList",
                    table: $("#hoso-getList"),
                    props: {
                        ajax: {
                            url: '/DocumentStorage/get_HoSos',
                            type: "GET",
                            data: function (d) {
                                d.idViTriLuuTru = ds.viTriLuuTru.idViTriLuuTru
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
                                        return `<input class="form-check-input checkRow-hoso-getList" type="checkbox"/>`;
                                    };
                                    return ``;
                                }
                            },
                            {
                                data: "MaHoSo",
                                render: function (data, type, row, meta) {
                                    return `<span title="${data}">${sys.truncateString(data, 60)}</span>`;
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
                                data: "ThoiHanBaoQuan",
                                className: "text-center",
                            },
                            {
                                data: "CheDoSuDung.TenCheDoSuDung",
                                className: "text-center",
                            },
                            {
                                data: null,
                                className: "text-center",
                                render: function (data, type, row, meta) {
                                    var quyenTruyCap = data.QuyenTruyCap.split(",");
                                    // Nếu có quyền thì cho thao tác
                                    if (quyenTruyCap.some(x => x == idNguoiDung_DangSuDung)) {
                                        return `<span class="font-bold fst-italic text-success">Được truy cập</span>`;
                                    };
                                    return `<span class="font-bold fst-italic text-danger">Không có quyền</span>`;
                                }
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
                                    <div style="white-space: nowrap">
                                        <a href="#" class="btn btn-sm btn-success" title="Xem văn bản" onclick="ds.vanBan.redirectVanBan(${data.IdHoSo})"><i class="bi bi-folder2-open"></i></a>
                                        <a href="#" class="btn btn-sm btn-danger" title="Hủy nộp lưu" onclick="ds.hoSo.huyNopLuu('single',${data.IdHoSo})"><i class="bi bi-send-x"></i></a>
                                    </div>`;
                                    };
                                    return ``;
                                }
                            }
                        ],
                    }
                }).dataTable;
            },
            huyNopLuu: function (loai, id = 0) {
                var ids = [];
                // Lấy id
                if (loai == "single") {
                    ids.push(id)
                } else {
                    ds.hoSo.dataTable.rows().iterator('row', function (context, index) {
                        var $row = $(this.row(index).node()),
                            $checkBox = $("input[type='checkbox']:checked", $row);
                        $.each($checkBox, function () {
                            ids.push($(this).closest('tr').attr('id'));
                        })
                    });
                };
                // Kiểm tra id
                if (ids.length > 0) {
                    var f = new FormData();
                    f.append("str_ids", ids.toString());
                    sys.confirmDialog({
                        mess: "Bạn chắc chắn muốn hủy nộp lưu bản ghi này ?",
                        callback: function () {
                            $.ajax({
                                ...ajaxDefaultProps({
                                    url: "/DocumentStorage/huyNopLuu",
                                    type: "POST",
                                    data: f,
                                }),
                                contentType: false,
                                processData: false,
                                success: function (res) {
                                    ds.hoSo.dataTable.ajax.reload(function () {
                                        sys.alert({ status: res.status, mess: res.mess })
                                    }, false);
                                }
                            })
                        }
                    })
                } else {
                    sys.alert({ mess: "Bạn chưa chọn bản ghi nào", status: "warning", timeout: 1500 })
                }
            }
        }
        ds.vanBan = {
            ...ds.vanBan,
            getList: function () {
                ds.vanBan.dataTable = new DataTableCustom({
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
            displayModal_Read_VanBan: function (idHoSo, loai, idVanBan) {
                $.ajax({
                    ...ajaxDefaultProps({
                        url: '/DocumentStorage/displayModal_Read_VanBan',
                        type: "POST",
                        data: { idHoSo, loai, idVanBan }
                    }),
                    success: function (res) {
                        $("#vanban-detail").html(res);
                        ds.vanBan.displayPartial_DuLieuSos();
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
                        url: '/DocumentStorage/get_DuLieuSos',
                        type: "POST",
                        data: { idVanBan, idBieuMau }
                    }),
                    success: function (res) {
                        $("#truongdulieus-container").html(res);
                        /**
                        * table-truongdulieu
                        * */
                        ds.vanBan.truongDuLieu = new TruongDuLieu();
                        ds.vanBan.truongDuLieu.init();
                    }
                })
            },
            redirectVanBan: function (idHoSo) {
                let url = window.location.origin + `/DocumentStorage/VanBan?idHoSo=${idHoSo}`;
                window.open(url);
            }
        }
        sys.activePage({
            page: ds.page.attr("id"),
            pageGroup: ds.pageGroup
        });
    }
};