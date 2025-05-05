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
                dom: `
                <'row'<'col-sm-12 col-md-6'>>
                <'row'<'col-sm-12'rt>>
                <'row'<'col-sm-12 col-md-4 pt-2'l><'col-sm-12 col-md-4 text-center'i><'col-sm-12 col-md-4 pt-2'p>>`,
                lengthMenu: [
                    [5, 10, 15, -1],
                    [5, 10, 15, 'Tất cả'],
                ],
                columnDefs: [{
                    className: 'text-center',
                    target: '_all'
                }]
            },
        }).dataTable;
    }
    create() {
        var tdl = this,
            $tr = [];
        /**
         * Tạo từng cell theo header
         * */
        tdl.dataTable.columns().header().map(col => {
            let idDuLieuSo = 0,
                idTruongDuLieu = col.dataset.idtruongdulieu;
            if (idTruongDuLieu == 0) {
                $tr.push(`<a href="#" class="btn btn-sm btn-danger" title="Xóa bỏ" onclick="dd.truongDuLieu.delete(this)"><i class="bi bi-trash3-fill"></i></a>`)
            } else if (idTruongDuLieu == -1) {
                $tr.push(`<input type="number" class="form-control text-success trangso" data-idtruongdulieu="-1" data-iddulieuso="-1"/>`);
            }
            else {
                $tr.push(`<input type="text" class="form-control truongdulieu" data-idtruongdulieu="${idTruongDuLieu}" data-iddulieuso="${idDuLieuSo}"/>`);
            }
        });
        tdl.dataTable.row.add($tr).draw(false);

    }
    delete(e) {
        var tdl = this,
            $tr = $(e).closest("tr");
        tdl.dataTable.row($tr).remove().draw();
    }
}
/**
 * main
 * */
class DocumentDigitizing {
    constructor() {
        this.page;
        this.pageGroup;
        this.vanBan = {
            data: [],
            dataTable: null,
            activePDF: function () { },
            save: function () { },
            digitizing: function () { },
            delete: function () { },
            displayModal_Digitizing: function () { },
            displayPartial_DuLieuSos: function () { },
        };
        this.kySo = {
            duongDanVanBan_BANDAU: "",
            signFileCallBack: function () { },
            exc_sign_files: function () { },
            exc_sign_approved: function () { },
        };
        this.truongDuLieu;
        this.excel = {};
    }
    init() {
        var dd = this;
        dd.page = $("#page-documentdigitizing");
        dd.pageGroup = dd.page.attr("page-group");

        var protocol = window.location.protocol, // "https:"
            hostname = window.location.hostname, // "localhost"
            port = window.location.port, // "44345"
            base_url = `${protocol}//${hostname}${port != '' ? `:${port}` : ""}`;

        dd.vanBan = {
            ...dd.vanBan,
            getList: function () {
                $.ajax({
                    ...ajaxDefaultProps({
                        url: "/DocumentDigitizing/getList",
                        type: "GET"
                    }),
                    contentType: false,
                    processData: false,
                    success: function (res) {
                        $("#vanBan-container").html(res);
                        dd.vanBan.dataTable = new DataTableCustom({
                            name: "vanban-getList",
                            table: $("#vanban-getList"),
                        }).dataTable;
                    }
                });
            },
            displayModal_Digitizing(idHoSo, loai = '', idVanBan = 0) {
                $.ajax({
                    ...ajaxDefaultProps({
                        url: '/DocumentDigitizing/displayModal_Digitizing',
                        type: "POST",
                        data: { idHoSo, loai, idVanBan }
                    }),
                    success: function (res) {
                        $("#vanban-digitizing").html(res);
                        dd.vanBan.displayPartial_DuLieuSos();
                        sys.displayModal({
                            name: '#vanban-digitizing'
                        });
                    }
                })
            },
            displayPartial_DuLieuSos: function () {
                var idVanBan = $('#input-idvanban').val() || 0,
                    idBieuMau = $('#select-bieumau').val() || 0;
                $.ajax({
                    ...ajaxDefaultProps({
                        url: '/DocumentDigitizing/get_DuLieuSos',
                        type: "POST",
                        data: { idVanBan, idBieuMau }
                    }),
                    success: function (res) {
                        $("#truongdulieus-container").html(res);
                        /**
                        * table-truongdulieu
                        * */
                        dd.truongDuLieu = new TruongDuLieu();
                        dd.truongDuLieu.init();
                    }
                })
            },
            save: function () {
                var $selectPDF = $("#select-pdf").get(0),
                    kiemTra = true,
                    mess = `Tổng kích thước tệp gửi lên vượt quá giới hạn 1GB`,
                    maxSizeInBytes = 1024 * 1024 * 1024, // 1 GB,
                    real_maxSizeInBytes = 0,
                    formData = new FormData();
                formData.append("idHoSo", $("#input-idhoso").val());
                $.each($selectPDF.files, function (idx, f) {
                    var extension = f.type;
                    if (/\.(doc|docx|xls|xlsx|pdf|png|jpg|jpeg|mp4)$/i.test(f.name)) {
                        real_maxSizeInBytes += f.size;
                        if (f.size > maxSizeInBytes) {
                            kiemTra = false;
                            mess = `Tồn tại tệp có kích thước tệp vượt quá giới hạn 1GB`;
                            return false; // Dừng vòng lặp khi gặp file vượt quá giới hạn
                        };
                        if (f.name.length > 80) {
                            kiemTra = false;
                            mess = `Tồn tại tệp có tên vượt quá giới hạn 80 ký tự`;
                            return false;
                        };
                        formData.append("files", f);
                    } else {
                        mess = `Tồn tại tệp không thuộc định dạng cho phép [doc|docx|xls|xlsx|pdf|png|jpg|jpeg|mp4]`;
                        kiemTra = false;
                    };
                });
                // Xóa bộ nhớ đệm để upload file trong lần tiếp theo
                $selectPDF.value = ''; // xóa giá trị của input file
                if (!kiemTra || real_maxSizeInBytes >= maxSizeInBytes) {
                    sys.alert({
                        status: "error",
                        mess,
                        timeout: 5000
                    });
                } else {
                    $.ajax({
                        ...ajaxDefaultProps({
                            url: "/DocumentDigitizing/create_VanBan",
                            type: "POST",
                            data: formData
                        }),
                        contentType: false,
                        processData: false,
                        success: function (res) {
                            sys.displayModal({
                                name: '#vanban-create',
                                displayStatus: "hide"
                            });
                            sys.alert({ status: res.status, mess: res.mess })
                            dd.vanBan.getList();
                            //dd.vanBan.dataTable.ajax.reload(function () {
                            //    sys.displayModal({
                            //        name: '#vanban-create',
                            //        displayStatus: "hide"
                            //    });
                            //    sys.alert({ status: res.status, mess: res.mess })
                            //}, false);
                        }
                    });
                };
            },
            delete: function (loai, id) {
                var idVanBans = [];
                // Lấy id
                if (loai == "single") {
                    idVanBans.push(id)
                } else {
                    dd.vanBan.dataTable.rows().iterator('row', function (context, index) {
                        var $row = $(this.row(index).node());
                        if ($row.has("input.checkRow-vanban-getList:checked").length > 0) {
                            idVanBans.push($row.attr('id'));
                        };
                    });
                };
                // Kiểm tra id
                if (idVanBans.length > 0) {
                    var f = new FormData();
                    f.append("str_idVanBans", idVanBans.toString());
                    f.append("idHoSo", $("#input-idhoso").val());
                    sys.confirmDialog({
                        mess: `
                        <p class="font-bold">Văn bản có liên kết với các
                            <span class="text-danger fst-italic"> [Phiếu mượn]</span> 
                        </p>
                        <p>Bạn có thực sự muốn xóa bản ghi này hay không ?</p>
                        `,
                        callback: function () {
                            $.ajax({
                                ...ajaxDefaultProps({
                                    url: "/DocumentDigitizing/delete_VanBans",
                                    type: "POST",
                                    data: f,
                                }),
                                contentType: false,
                                processData: false,
                                success: function (res) {
                                    sys.alert({ status: res.status, mess: res.mess })
                                    dd.vanBan.getList();
                                }
                            })
                        }
                    })
                } else {
                    sys.alert({ mess: "Bạn chưa chọn bản ghi nào", status: "warning", timeout: 1500 })
                }
            },
            displayModal_GanBieuMau() {
                sys.displayModal({
                    name: '#vanban-ganbieumau'
                });
            },
            ganBieuMau: function () {
                var idVanBans = [],
                    idBieuMau = $("#select-ganbieumau").val();
                // Lấy id
                dd.vanBan.dataTable.rows().iterator('row', function (context, index) {
                    var $row = $(this.row(index).node()),
                        $checkBox = $("input[type='checkbox']:checked", $row);
                    $.each($checkBox, function () {
                        idVanBans.push($(this).closest('tr').attr('id'));
                    })
                });
                // Kiểm tra id
                if (idVanBans.length > 0) {
                    var f = new FormData();
                    f.append("str_idVanBans", idVanBans.toString());
                    f.append("idBieuMau", idBieuMau);
                    sys.confirmDialog({
                        mess: "Bạn có muốn gán biểu mẫu cho bản ghi này hay không ?",
                        callback: function () {
                            $.ajax({
                                ...ajaxDefaultProps({
                                    url: "/DocumentDigitizing/ganBieuMau_VanBans",
                                    type: "POST",
                                    data: f,
                                }),
                                contentType: false,
                                processData: false,
                                success: function (res) {
                                    sys.alert({ status: res.status, mess: res.mess })
                                    dd.vanBan.getList();
                                }
                            })
                        }
                    })
                } else {
                    sys.alert({ mess: "Bạn chưa chọn bản ghi nào", status: "warning", timeout: 1500 })
                }
            },
            digitizing: function () {
                var idLoaiBieuMau = $('#select-bieumau option:selected').data("idloaibieumau"),
                    idHoSo = $("#input-idhoso").val(),
                    vb = {
                        IdHoSo: idHoSo,
                        IdVanBan: $('#input-idhoso').val(),
                        IdVanBan: $('#input-idvanban').val(),
                        IdBieuMau: $('#select-bieumau').val(),
                        BieuMau: {
                            IdBieuMau: $('#select-bieumau').val(),
                            TruongDuLieus: []
                        }
                    };
                if (vb.IdBieuMau == null || vb.IdBieuMau == 0) {
                    sys.alert({ status: "warning", mess: "Không thể thực hiện số hóa khi chưa có thông tin biểu mẫu" });
                } else {
                    if (idLoaiBieuMau == 1) { // Có cấu trúc
                        dd.truongDuLieu.dataTable.rows().iterator('row', function (context, index) {
                            var node = $(this.row(index).node()),
                                $input = $("input.truongdulieu", node), // Lấy input của dòng
                                trangSo = $("input.trangso", node).val(),
                                nhom = [];
                            $.each($input, function (idx, input) {
                                let truongDuLieu = {
                                    IdDuLieuSo: $(input).data("iddulieuso"),
                                    IdTruongDuLieu: $(input).data("idtruongdulieu"),
                                    DuLieuSo: $(input).val(),
                                    TrangSo: trangSo,
                                    Nhom: idx + 1
                                };
                                nhom.push(truongDuLieu);
                            })
                            if (nhom.some(x => x.DuLieuSo != "")) vb.BieuMau.TruongDuLieus.push({ DuLieuSos: nhom });
                        });
                    } else {
                        var nhom = [],
                            trangSo = $("#truongdulieus-container input.trangso").val();
                        $.each($("#truongdulieus-container input.truongdulieu"), function () {
                            let duLieuSo = {
                                idHoSo: idHoSo,
                                IdDuLieuSo: $(this).data("iddulieuso"),
                                IdTruongDuLieu: $(this).data("idtruongdulieu"),
                                DuLieuSo: $(this).val(),
                                TrangSo: trangSo,
                                Nhom: 0
                            }
                            //if (duLieuSo.IdDuLieuSo == 0 && duLieuSo.DuLieuSo == "") { }
                            nhom.push(duLieuSo);
                        });
                        vb.BieuMau.TruongDuLieus.push({ DuLieuSos: nhom });
                    }
                    var f = new FormData();
                    f.append("vb", JSON.stringify(vb))
                    $.ajax({
                        ...ajaxDefaultProps({
                            url: "/DocumentDigitizing/digitizing",
                            type: "POST",
                            data: f
                        }),
                        contentType: false,
                        processData: false,
                        success: function (res) {
                            sys.displayModal({
                                name: '#vanban-digitizing',
                                displayStatus: "hide"
                            });
                            sys.alert({ status: res.status, mess: res.mess })
                            dd.vanBan.getList();
                        }
                    });
                };
            },
            nopLuu: function (idHoSo = 0) {
                var idHoSos = [];
                idHoSos.push(idHoSo);
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
                                sys.alert({ status: res.status, mess: res.mess })
                                dd.vanBan.getList();
                            }
                        })
                    }
                });
            },
            huyNopLuu: function (idHoSo = 0) {
                var idHoSos = [];
                idHoSos.push(idHoSo);
                var f = new FormData();
                f.append("str_idHoSos", idHoSos.toString());
                sys.confirmDialog({
                    mess: "Bạn chắc chắn muốn hủy nộp lưu hồ sơ này ?",
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
                                sys.alert({ status: res.status, mess: res.mess })
                                dd.vanBan.getList();
                            }
                        })
                    }
                })
            },
            activePDF: function (href) {
                //if (window.innerWidth < 768) {
                //    $("#file-nav").toggleClass("active")
                //} else {
                window.open(href)
                //}
            },
            exportToOffice: function (idHoSo, idVanBan, loaiXuat) {
                //var idHoSo = $("#input-idhoso").val();
                window.location = `/DocumentDigitizing/exportToOffice?idHoSo=${idHoSo}&idVanBan=${idVanBan}&loaiXuat=${loaiXuat}`;
            }
        };
        dd.kySo = {
            ...dd.kySo,
            // 1 văn bản
            signFileCallBack: function (res) {
                res = JSON.parse(res);
                if (res.Status == 0) {
                    let tenVanBan_KYSO = res.FileName,
                        duongDanThuMuc_KYSO = res.FileServer;
                    let form = new FormData();
                    form.append("duongDanVanBan_BANDAU", dd.kySo.duongDanVanBan_BANDAU);
                    form.append("tenVanBan_KYSO", tenVanBan_KYSO);
                    form.append("duongDanThuMuc_KYSO", duongDanThuMuc_KYSO);
                    $.ajax({
                        ...ajaxDefaultProps({
                            url: "/DocumentDigitizing/chuyenFileKySoSang",
                            type: "POST",
                            data: form
                        }),
                        contentType: false,
                        processData: false,
                        success: function (res) {
                            sys.alert({ mess: res.mess, status: res.status, timeout: 1500 });
                        }
                    });
                } else {
                    sys.alert({ mess: `Ký số thất bại: ${res.Message}`, status: "error", timeout: 1500 });
                };
            },
            // Nhiều văn bản
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
                        }
                    });
                } else {
                    sys.alert({ mess: `Ký số thất bại: ${mess}`, status: "error", timeout: 1500 });
                };
            },
            // Ký nhiều văn bản
            exc_sign_files: function () {
                var duongDanVanBans = [];
                dd.vanBan.dataTable.rows().iterator('row', function (context, index) {
                    var $row = $(this.row(index).node());
                    if ($row.has("input.checkRow-vanban-getList:checked").length > 0) {
                        var duongDanVanBan = $row.data("duongdanvanban");
                        duongDanVanBans.push(duongDanVanBan);
                    };
                });
                if (duongDanVanBans.length == 0) {
                    sys.alert({ mess: "Bạn chưa chọn bản ghi nào", status: "warning", timeout: 1500 });
                } else {
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
                    vgca_sign_files(JSON.stringify(prms), dd.kySo.signFilesCallBack);
                };
            },
            // Ký từng văn bản
            exc_sign_approved: function (duongDanTep) {
                var protocol = window.location.protocol, // "https:"
                    hostname = window.location.hostname, // "localhost"
                    port = window.location.port, // "44345"
                    base_url = `${protocol}//${hostname}${port != '' ? `:${port}` : ""}`;
                dd.kySo.duongDanVanBan_BANDAU = duongDanTep;
                var prms = {
                    FileUploadHandler: `${base_url}/FileUploadHandler.aspx`,
                    SessionId: "",
                    JWTToken: "",
                    FileName: `${base_url}/${duongDanTep}`,
                };
                vgca_sign_approved(JSON.stringify(prms), dd.kySo.signFileCallBack);
            },

        };
        sys.activePage({
            page: dd.page.attr("id"),
            pageGroup: dd.pageGroup
        });
    }
    displayModal_Excel_DuLieuSo() {
        var dd = this;
        dd.getList_Excel_DuLieuSo("reload");
        sys.displayModal({
            name: '#excel-dulieuso'
        });
    }
    getList_Excel_DuLieuSo(loai) {
        var dd = this;
        $.ajax({
            ...ajaxDefaultProps({
                url: "/DocumentDigitizing/getList_Excel_DuLieuSo",
                type: "POST",
                data: {
                    loai
                }
            }),
            success: function (res) {
                $("#excel-dulieuso-getList-container").html(res);
                /**
                 * ExcelHoSo 
                 */
                dd.create_Excel_DuLieuSo();
            }
        })
    }
    create_Excel_DuLieuSo() {
        var dd = this,
            $tableExcels = $("table", $("#excel-dulieuso-getList-container"));
        dd.excel.dataTables = [];
        $.each($tableExcels, function (i, eTb) {
            var table = {
                idBieuMau: $(eTb).data("idbieumau"),
                idLoaiBieuMau: $(eTb).data("idloaibieumau"),
                tenBieuMau: $(eTb).data("tenbieumau"),
                dataTable: new DataTableCustom({
                    name: $(eTb).attr("id"),
                    table: $(eTb),
                    props: {
                        //scrollX: true,
                        autoWidth: true,
                        responsive: true,
                        //lengthMenu: [
                        //    [10, 50, 100, -1],
                        //    [10, 50, 100, 'Tất cả'],
                        //],
                    }
                }).dataTable
            };
            dd.excel.dataTables.push(table);
        })
        dd.excel = {
            ...dd.excel,
            createRow: function () { },
            deleteRow: function (idBieuMau, e) {
                var $tr = $(e).closest("tr");
                $.each(dd.excel.dataTables, function () {
                    var exTb = this
                    if (exTb.idBieuMau == idBieuMau) {
                        exTb.dataTable.row($tr).remove().draw();
                    }
                });
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
                var $select = $("#select-file").get(0),
                    formData = new FormData();
                $.each($select.files, function (idx, f) {
                    var extension = f.type;
                    if (extension.includes("sheet")) {
                        formData.append("files", f);
                    }
                });
                // Xóa bộ nhớ đệm để upload file trong lần tiếp theo
                $select.value = ''; // xóa giá trị của input file
                $.ajax({
                    ...ajaxDefaultProps({
                        url: "/DocumentDigitizing/upload_Excel_DuLieuSo",
                        type: "POST",
                        data: formData
                    }),
                    contentType: false,
                    processData: false,
                    success: function (res) {
                        dd.getList_Excel_DuLieuSo("upload");
                        $.each(dd.excel.dataTables, function () {
                            this.dataTable.search('').draw();
                        })
                        sys.alert({ status: res.status, mess: res.mess });
                    }
                })
            },
            download: function (loaiTaiXuong) {
                var formData = new FormData(),
                    hoSos = [];
                if (loaiTaiXuong == "data") {
                    $.each(dd.excel.dataTables, function () {
                        var exTb = this,
                            idBieuMau = exTb.idBieuMau,
                            idLoaiBieuMau = exTb.idLoaiBieuMau,
                            tenBieuMau = exTb.tenBieuMau,
                            nhom = 0; // Vì đang xét trên chỉ 1 loại biểu mẫu nên sẽ tạo nhóm bắt đầu từ 0;
                        exTb.dataTable.rows().iterator('row', function (context, index) {
                            var $row = $(this.row(index).node()),
                                hoSo_NEW = {
                                    IdHoSo: 0,
                                    MaHoSo: 0,
                                    VanBans: [
                                        {
                                            IdVanBan: 0,
                                            TenVanBan: "",
                                            BieuMau: {
                                                IdBieuMau: 0,
                                                TenBieuMau: "",
                                                TruongDuLieus: []
                                            }
                                        }
                                    ]
                                };
                            if ($row.has(`input.checkRow-excel-${idBieuMau}-getList:checked`).length > 0) {
                                //#region Tạo hồ sơ
                                var $span = $("span", $row),
                                    trangSo = $("span[data-tentruong='TrangSo']", $row).text(); // Lấy span
                                ; // Lấy span
                                // NẾU LÀ BIỂU MẪU CÓ CẤU TRÚC (BẢNG BIỂU) => THÊM DỮ LIỆU SỐ MỚI VÀO DANH SÁCH TRƯỜNG DỮ LIỆU
                                if (idLoaiBieuMau == 1) nhom++;
                                $.each($span, function (idx, span) {
                                    var text = $(span).text().trim(),
                                        tenTruong = $(span).data("tentruong");
                                    if (tenTruong == "MaHoSo") {
                                        hoSo_NEW.MaHoSo = text;
                                    } else if (tenTruong == "TenVanBan") {
                                        hoSo_NEW.VanBans[0].TenVanBan = text;
                                        hoSo_NEW.VanBans[0].BieuMau.IdBieuMau = idBieuMau;
                                        hoSo_NEW.VanBans[0].BieuMau.IdLoaiBieuMau = idLoaiBieuMau;
                                        hoSo_NEW.VanBans[0].BieuMau.TenBieuMau = tenBieuMau;
                                    } else if (tenTruong == "TrangSo") { }
                                    else {
                                        var idTruongDuLieu = $(span).data("idtruongdulieu");
                                        hoSo_NEW.VanBans[0].BieuMau.TruongDuLieus = [
                                            ...hoSo_NEW.VanBans[0].BieuMau.TruongDuLieus,
                                            {
                                                IdBieuMau: idBieuMau,
                                                IdTruongDuLieu: idTruongDuLieu,
                                                DuLieuSos: [
                                                    {
                                                        IdDuLieuSo: 0,
                                                        DuLieuSo: text,
                                                        Nhom: nhom,
                                                        TrangSo: trangSo
                                                    }
                                                ],
                                            }
                                        ];
                                    };
                                });
                                //#endregion
                                hoSos.push(hoSo_NEW);
                            };
                        });
                    })
                    formData.append("hoSos", JSON.stringify(hoSos));
                }
                formData.append("loaiTaiXuong", loaiTaiXuong);
                formData.append("idHoSo", $("#input-idhoso").val());
                $.ajax({
                    ...ajaxDefaultProps({
                        url: "/DocumentDigitizing/get_DuLieuSos_download",
                        type: "POST",
                        data: formData
                    }),
                    contentType: false,
                    processData: false,
                    success: function () {
                        sys.alert({ status: "success", mess: "Đã tải xuống thành công" })
                        window.location = "/DocumentDigitizing/download_Excel_DuLieuSo";
                    }
                })
            },
            reload: function () {
                dd.getList_Excel_DuLieuSo("reload");
                $.each(dd.excel.dataTables, function () {
                    this.dataTable.search('').draw();
                })
                sys.alert({ status: "success", mess: "Đã làm mới dữ liệu" })
            },
            saveByGroup: function () {
                var formData = new FormData(),
                    hoSos = [];
                $.each(dd.excel.dataTables, function () {
                    var exTb = this,
                        idBieuMau = exTb.idBieuMau,
                        idLoaiBieuMau = exTb.idLoaiBieuMau,
                        tenBieuMau = exTb.tenBieuMau,
                        nhom = 0;
                    exTb.dataTable.rows().iterator('row', function (context, index) {
                        // NẾU LÀ BIỂU MẪU CÓ CẤU TRÚC (BẢNG BIỂU) => THÊM DỮ LIỆU SỐ MỚI VÀO DANH SÁCH TRƯỜNG DỮ LIỆU
                        if (idLoaiBieuMau == 1) nhom++;
                        var $row = $(this.row(index).node()),
                            hoSo_NEW = {
                                IdHoSo: 0,
                                MaHoSo: 0,
                                VanBans: [
                                    {
                                        IdHoSo: 0,
                                        IdVanBan: 0,
                                        TenVanBan: "",
                                        BieuMau: {
                                            IdBieuMau: 0,
                                            TenBieuMau: "",
                                            TruongDuLieus: []
                                        }
                                    }
                                ]
                            };
                        //#region TẠO 1 ĐỐI TƯỢNG HỒ SƠ CHO 1 DÒNG
                        var $span = $("span", $row),
                            trangSo = $("span[data-tentruong='TrangSo']", $row).text(); // Lấy span

                        $.each($span, function (idx, span) {
                            var text = $(span).text().trim(),
                                tenTruong = $(span).data("tentruong");
                            if (tenTruong == "MaHoSo") {
                                hoSo_NEW.MaHoSo = text;
                            } else if (tenTruong == "TenVanBan") {
                                hoSo_NEW.VanBans[0].TenVanBan = text;
                                hoSo_NEW.VanBans[0].BieuMau.IdBieuMau = idBieuMau;
                                hoSo_NEW.VanBans[0].BieuMau.IdLoaiBieuMau = idLoaiBieuMau;
                                hoSo_NEW.VanBans[0].BieuMau.TenBieuMau = tenBieuMau;
                            } else if (tenTruong == "TrangSo") { }
                            else {
                                var idTruongDuLieu = $(span).data("idtruongdulieu");
                                hoSo_NEW.VanBans[0].BieuMau.TruongDuLieus = [
                                    ...hoSo_NEW.VanBans[0].BieuMau.TruongDuLieus,
                                    {
                                        IdBieuMau: idBieuMau,
                                        IdTruongDuLieu: idTruongDuLieu,
                                        DuLieuSos: [
                                            {
                                                IdDuLieuSo: 0,
                                                DuLieuSo: text,
                                                IdTruongDuLieu: idTruongDuLieu,
                                                Nhom: nhom,
                                                TrangSo: trangSo
                                            }
                                        ],
                                    }
                                ];
                            };
                        });
                        //#endregion

                        //#region KIỂM TRA HỒ SƠ ĐÃ TỒN TẠI CHƯA
                        var hoSo_OLD = hoSos.find(x => x.MaHoSo == hoSo_NEW.MaHoSo);
                        if (hoSo_OLD) {
                            // KIỂM TRA VĂN BẢN ĐÃ TỒN TẠI CHƯA
                            var vanBan_OLD = hoSo_OLD.VanBans.find(x => x.TenVanBan == hoSo_NEW.VanBans[0].TenVanBan);
                            if (vanBan_OLD) {
                                // KIỂM TRA BIỂU MẪU ĐÃ TỒN TẠI CHƯA
                                var bieuMau_OLD = vanBan_OLD.BieuMau.TenBieuMau == hoSo_NEW.VanBans[0].BieuMau.TenBieuMau;
                                if (!bieuMau_OLD) {
                                    vanBan_OLD.BieuMau.IdBieuMau = hoSo_NEW.VanBans[0].BieuMau.IdBieuMau;
                                    vanBan_OLD.BieuMau.TenBieuMau = hoSo_NEW.VanBans[0].BieuMau.TenBieuMau;
                                    vanBan_OLD.BieuMau.IdLoaiBieuMau = hoSo_NEW.VanBans[0].BieuMau.IdLoaiBieuMau;
                                    // Nếu là một biểu mẫu khác thì sẽ xóa hết dữ liệu số cũ
                                    vanBan_OLD.BieuMau.TruongDuLieus = [];
                                }
                                if (vanBan_OLD.BieuMau.IdLoaiBieuMau == 1) {
                                    // NẾU LÀ BIỂU MẪU CÓ CẤU TRÚC (BẢNG BIỂU) => THÊM DỮ LIỆU SỐ MỚI VÀO DANH SÁCH TRƯỜNG DỮ LIỆU
                                    $.each(vanBan_OLD.BieuMau.TruongDuLieus, function (truongDuLieu_OLD_index, truongDuLieu_OLD) {
                                        var truongDuLieu_NEW = hoSo_NEW.VanBans[0].BieuMau.TruongDuLieus.find(x => x.IdTruongDuLieu == truongDuLieu_OLD.IdTruongDuLieu);
                                        truongDuLieu_OLD.DuLieuSos.push(...truongDuLieu_NEW.DuLieuSos);
                                    });
                                } else {
                                    // NẾU LÀ BIỂU MẪU KHÔNG CẤU TRÚC (VĂN BẢN) => TẠO LẠI DANH SÁCH TRƯỜNG DỮ LIỆU
                                    vanBan_OLD.BieuMau.TruongDuLieus = hoSo_NEW.VanBans[0].BieuMau.TruongDuLieus;
                                }

                            } else {
                                hoSo_OLD.VanBans.push(...hoSo_NEW.VanBans)
                            }
                        } else {
                            hoSos.push(hoSo_NEW);
                        }
                        //#endregion
                    });
                })
                formData.append("hoSos", JSON.stringify(hoSos));
                formData.append("loaiTaiXuong", "data");
                formData.append("idHoSo", $("#input-idhoso").val());
                $.ajax({
                    ...ajaxDefaultProps({
                        url: "/DocumentDigitizing/get_DuLieuSos_download",
                        type: "POST",
                        data: formData
                    }),
                    contentType: false,
                    processData: false,
                    success: function () {
                        dd.excel.save();
                    }
                })
            },
            save: function () {
                //status = false;
                $.ajax({
                    ...ajaxDefaultProps({
                        url: "/DocumentDigitizing/save_Excel_DuLieuSo",
                        //type: "POST",
                        //data: formData
                    }),
                    contentType: false,
                    processData: false,
                    success: function (res) {
                        //status = res.status
                        sys.displayModal({
                            name: '#excel-dulieuso',
                            displayStatus: "hide"
                        });
                        sys.alert({ status: res.status, mess: res.mess });
                        dd.vanBan.getList();
                    }
                })
                //return status;
            },
        }
    }
};