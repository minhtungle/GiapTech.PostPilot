'use strict'
/**
 * main
 * */
class QuanLyBaiDang {
    constructor() {
        this.page;
        this.pageGroup;
        this.baiDang = {}
    }
    init() {
        var quanLyBaiDang = this;
        var idChienDich = $("#input-idchiendich").val();
        var idNguoiDung_DangSuDung = $("#input-idnguoidung-dangsudung").val();
        quanLyBaiDang.page = $("#page-quanlybaidang");

        quanLyBaiDang.baiDang = {
            ...quanLyBaiDang.baiDang,
            dataTable: null,
            suDungAnhAI: false,
            handleAnhMoTa: {
                maxDungLuongAnh: 1024 * 1024 * 30, // 500Mb,
                maxAnhDaiDien: 1,
                maxAnhMoTa: 6,
                arrAnh: [],
                add: function (e, loaiAnh = 'anhdaidien', rowNumber = '00000000-0000-0000-0000-000000000000') {
                    var $modal = $("#baidang-crud");

                    var $imgContainer = $(`.baidang-read[row='${rowNumber}'] #tbody-${loaiAnh}-container`, $modal),
                        soAnhDangCo = $("tr", $imgContainer).length;

                    var addTr = function (files) {
                        let kiemTra = true,
                            mess = "Thêm ảnh thành công";

                        let arrAnh = [];

                        $.each(files, function (idx, f) {
                            // Kiểm tra tệp
                            if (!(/\.(png|jpg|jpeg)$/i.test(f.name))) {
                                mess = `Tồn tại tệp không thuộc định dạng cho phép [png|jpg|jpeg]`;
                                kiemTra = false;
                                return false;
                            };
                            // Kiểm tra dung lượng
                            if (f.size > quanLyBaiDang.baiDang.handleAnhMoTa.maxDungLuongAnh) {
                                mess = `Tồn tại tệp có kích thước tệp vượt quá giới hạn ${quanLyBaiDang.baiDang.handleAnhMoTa.maxDungLuongAnh} Mb`;
                                kiemTra = false;
                                return false;
                            };
                            // Kiểm tra tên
                            if (f.name.length > 80) {
                                mess = `Tồn tại tệp có tên vượt quá giới hạn 80 ký tự`;
                                kiemTra = false;
                                return false;
                            };

                            // Thêm ảnh vào mảng
                            let idTamThoi = sys.generateGUID();
                            var data = {
                                rowNumber,
                                idTamThoi,
                                file: f,
                                LaAnhDaiDien: loaiAnh == 'anhdaidien' ? true : false,
                                html: `<tr data-id="00000000-0000-0000-0000-000000000000" data-idtamthoi='${idTamThoi}'>
                                    <td class="text-center">#</td>
                                    <td class="w-90">${f.name}</td>
                                    <td class="text-center">
                                        <a class="btn c-pointer" onclick="quanLyBaiDang.baiDang.handleAnhMoTa.delete('${loaiAnh}', this, ${rowNumber})">
                                            <i class="bi bi-trash3-fill"></i>
                                        </a>
                                    </td>
                                </tr>`,
                            };
                            quanLyBaiDang.baiDang.handleAnhMoTa.arrAnh.push(data);
                            arrAnh.push(data);
                        });

                        if (!kiemTra) {
                            sys.alert({
                                status: "error",
                                mess,
                                timeout: 5000
                            });
                        } else {
                            $.each(arrAnh, function (idx, anh) {
                                //formData.append("files", anh.file); // Dùng khi save()
                                $imgContainer.prepend(anh.html);
                            });

                            sys.alert({
                                status: "success",
                                mess: "Đã thêm ảnh thành công",
                                timeout: 5000
                            });
                        };
                    };

                    var $fileInput = null;
                    if (loaiAnh == 'anhdaidien') {
                        $fileInput = $(`.baidang-read[row='${rowNumber}'] #image-anhdaidien-${rowNumber}`, $modal).get(0);
                        if (soAnhDangCo >= quanLyBaiDang.baiDang.handleAnhMoTa.maxAnhDaiDien) {
                            sys.alert({
                                status: "warning",
                                mess: `Chỉ cho phép tối đa ${quanLyBaiDang.baiDang.handleAnhMoTa.maxAnhDaiDien} ảnh`,
                                timeout: 5000
                            });
                        } else {
                            // Chỉ lấy đủ số ảnh quy định
                            let files = Array.from($fileInput.files)
                                .slice(0, (quanLyBaiDang.baiDang.handleAnhMoTa.maxAnhDaiDien - soAnhDangCo));
                            addTr(files);
                        };
                    } else {
                        $fileInput = $(`.baidang-read[row='${rowNumber}'] #image-anhmota-${rowNumber}`, $modal).get(0);
                        if (soAnhDangCo >= quanLyBaiDang.baiDang.handleAnhMoTa.maxAnhMoTa) {
                            sys.alert({
                                status: "warning",
                                mess: `Chỉ cho phép tối đa ${quanLyBaiDang.baiDang.handleAnhMoTa.maxAnhMoTa} ảnh`,
                                timeout: 5000
                            });
                        } else {
                            // Chỉ lấy đủ số ảnh quy định
                            let files = Array.from($fileInput.files)
                                .slice(0, (quanLyBaiDang.baiDang.handleAnhMoTa.maxAnhMoTa - soAnhDangCo));
                            addTr(files);
                        };
                    };
                    $fileInput.value = ''; // xóa giá trị của input file
                },
                delete: function (loaiAnh, e) {
                    var $tr = $(e).closest("tr"),
                        id = $tr.attr("data-id"),
                        idTamThoi = $tr.attr("data-idtamthoi");
                    // Xóa ảnh trên giao diện
                    $tr.remove();
                    // Xóa ảnh trong mảng
                    quanLyBaiDang.baiDang.handleAnhMoTa.arrAnh = quanLyBaiDang.baiDang.handleAnhMoTa.arrAnh
                        .filter(function (anh) {
                            return anh.idTamThoi != idTamThoi;
                        });
                },
            },
            handleAI: {
                taoNoiDungAI: function (rowNumber) {
                    var $modal = $("#baidang-crud");

                    var prompt = $(`.baidang-read[row='${rowNumber}'] #input-prompt`, $modal).val().trim();

                    prompt && $.ajax({
                        ...ajaxDefaultProps({
                            url: "/QuanLyBaiDang/taoNoiDungAI",
                            type: "POST",
                            contentType: "application/json; charset=utf-8",
                            data: {
                                prompt
                            },
                        }),
                        success: function (res) {
                            if (res.status == "success") {
                                $(`.baidang-read[row='${rowNumber}'] #input-noidung-ai`, $modal).text(res.NoiDung);
                                $(`.baidang-read[row='${rowNumber}'] #input-noidung-ai`, $modal).val(res.NoiDung);
                                $(`.baidang-read[row='${rowNumber}'] #input-noidung-ai`, $modal).change();
                            };
                            sys.alert({ mess: res.mess, status: res.status, timeout: 1500 });
                        }
                    })
                },
                taoNoiDungAI_Multiple: function () {
                    var $modal = $("#baidang-crud");

                    var rows = quanLyBaiDang.handleModal_CRUD.dataTable.rows().nodes().toArray(),
                        $rowChecks = $(`.checkRow-baidang-getList:checked`, rows);
                    if ($rowChecks.length == 0) {
                        sys.alert({ status: "warning", mess: "Bạn chưa chọn bản ghi nào" })
                    } else {
                        $.each($rowChecks, function () {
                            var $rowCheck = $(this).closest("tr"),
                                rowNumber = $rowCheck.attr("row"),
                                $div = $(`.baidang-read[row=${rowNumber}]`, $modal);

                            var prompt = $(`.baidang-read[row='${rowNumber}'] #input-prompt`, $modal).val().trim();

                            prompt && $.ajax({
                                ...ajaxDefaultProps({
                                    url: "/QuanLyBaiDang/taoNoiDungAI",
                                    type: "POST",
                                    contentType: "application/json; charset=utf-8",
                                    data: {
                                        prompt
                                    },
                                }),
                                success: function (res) {
                                    if (res.status == "success") {
                                        $(`#input-noidung-ai`, $div).text(res.NoiDung);
                                        $(`#input-noidung-ai`, $div).val(res.NoiDung);
                                        $(`#input-noidung-ai`, $div).change();
                                    };
                                    sys.alert({ mess: res.mess, status: res.status, timeout: 1500 });
                                }
                            })
                        });
                    };
                },

                kichHoatSuDungAnhAI: function () {
                    var trangThai = $("#checkbox-sudunganh-ai").is(":checked");
                    var $anhMoTaContainer = $("#anhmota-container");
                    if (trangThai) {
                        $anhMoTaContainer.hide();
                        quanLyBaiDang.baiDang.suDungAnhAI = true;
                    }
                    else {
                        $anhMoTaContainer.show();
                        quanLyBaiDang.baiDang.suDungAnhAI = false;
                    };
                }
            },
            lichDangBai: {
                add: function (e) {
                    var $table_LichDangBai = $(e).closest("table#table-lichdangbai"),
                        $tbody_LichDangBai = $("tbody.thongtin-lichdangbai", $table_LichDangBai),
                        $banMau = $($tbody_LichDangBai[0]);
                    // Thêm 1 buổi
                    $("thead", $table_LichDangBai).after(`<tbody class="thongtin-lichdangbai">${$banMau.html()}<tbody>`);
                },
                delete: function (e) {
                    var $table_LichDangBai = $(e).closest("table#table-lichdangbai"),
                        $tbody_LichDangBai = $("tbody.thongtin-lichdangbai", $table_LichDangBai),
                        $_this = $(e).closest("tbody.thongtin-lichdangbai");
                    if ($tbody_LichDangBai.length > 1) {
                        $_this.remove();
                    }
                },
            },
            switchCarouselItems(itemId) {
                var $modal = $("#baidang-crud");
                $(".carousel-item", $modal).removeClass("active");
                $(`.carousel-item${itemId}`, $modal).addClass('active');
            },
            getList: function () {
                $.ajax({
                    ...ajaxDefaultProps({
                        url: "/QuanLyBaiDang/getList_BaiDang",
                        type: "GET", // Phải là POST để gửi JSON
                        contentType: "application/json; charset=utf-8",  // Định dạng JSON
                        data: { idChienDich }
                        //dataType: "json",
                    }),
                    //contentType: false,
                    //processData: false,
                    success: function (res) {
                        $("#baidang-getList-container").html(res);
                        quanLyBaiDang.baiDang.dataTable = new DataTableCustom({
                            name: "baidang-getList",
                            table: $("#baidang-getList"),
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
                    }
                });
            },

            displayModal_CRUD: function (loai = "", idBaiDang = '00000000-0000-0000-0000-000000000000') {
                // Reset lại các thuộc tính
                quanLyBaiDang.baiDang.handleAnhMoTa.arrAnh = [];

                if (loai == "update") {
                    var idBaiDangs = [];
                    quanLyBaiDang.baiDang.dataTable.rows().iterator('row', function (context, index) {
                        var $row = $(this.row(index).node());
                        if ($row.has("input.checkRow-baidang-getList:checked").length > 0) {
                            idBaiDangs.push($row.attr('id'));
                        };
                    });
                    if (idBaiDangs.length != 1) {
                        sys.alert({ mess: "Yêu cầu chọn 1 bản ghi", status: "warning", timeout: 1500 });
                        return;
                    }
                    else idBaiDang = idBaiDangs[0];
                };
                var input = {
                    Loai: loai,
                    IdBaiDang: idBaiDang,
                };
                $.ajax({
                    ...ajaxDefaultProps({
                        url: "/QuanLyBaiDang/displayModal_CRUD_BaiDang",
                        type: "POST",
                        contentType: "application/json; charset=utf-8",
                        data: {
                            input
                        },
                    }),
                    success: function (res) {
                        $("#baidang-crud").html(res);
                        quanLyBaiDang.createModalCRUD_BaiDang();
                        quanLyBaiDang.handleModal_CRUD.addBanGhi();
                        /**
                          * Gán các thuộc tính
                          */
                        sys.displayModal({
                            name: '#baidang-crud'
                        });
                    }
                })
            },
            delete: function (loai, id) {
                var idBaiDangs = [];
                // Lấy id
                if (loai == "single") {
                    idBaiDangs.push(id)
                } else {
                    quanLyBaiDang.baiDang.dataTable.rows().iterator('row', function (context, index) {
                        var $row = $(this.row(index).node());
                        if ($row.has("input.checkRow-vanban-getList:checked").length > 0) {
                            idBaiDangs.push($row.attr('id'));
                        };
                    });
                };
                // Kiểm tra id
                if (idBaiDangs.length > 0) {
                    var f = new FormData();
                    f.append("idBaiDangs", idBaiDangs.toString());
                    sys.confirmDialog({
                        mess: `Bạn có thực sự muốn xóa bản ghi này hay không ?`,
                        callback: function () {
                            $.ajax({
                                ...ajaxDefaultProps({
                                    url: "/QuanLyBaiDang/delete_BaiDang",
                                    type: "POST",
                                    data: f,
                                }),
                                contentType: false,
                                processData: false,
                                success: function (res) {
                                    sys.alert({ status: res.status, mess: res.mess })
                                    quanLyBaiDang.baiDang.getList();
                                }
                            })
                        }
                    })
                } else {
                    sys.alert({ mess: "Bạn chưa chọn bản ghi nào", status: "warning", timeout: 1500 })
                }
            },

        };
        quanLyBaiDang.baiDang.getList();
        sys.activePage({
            page: quanLyBaiDang.page.attr("id"),
            pageGroup: quanLyBaiDang.pageGroup
        });
    }
    createModalCRUD_BaiDang() {
        var quanLyBaiDang = this;
        var idChienDich = $("#input-idchiendich").val();

        quanLyBaiDang.handleModal_CRUD = {
            dataTable: new DataTableCustom({
                name: "baidang-getList",
                table: $("#baidang-getList", $("#baidang-crud")),
                props: {
                    dom: `
                    <'row'<'col-sm-12'rt>>
                    <'row'<'col-sm-12 col-md-6 text-left'i><'col-sm-12 col-md-6 pt-2'p>>`,
                    lengthMenu: [
                        [10, 50, -1],
                        [10, 50, 'Tất cả'],
                    ],
                }
            }).dataTable,
            displayModal_UpdateMultipleCell: function () {
                var rows = quanLyBaiDang.handleModal_CRUD.dataTable.rows().nodes().toArray(),
                    $rowChecks = $(`.checkRow-baidang-getList:checked`, rows);
                if ($rowChecks.length == 0) {
                    sys.alert({ status: "warning", mess: "Bạn chưa chọn bản ghi nào" })
                } else {
                    sys.displayModal({
                        name: '#baidang-crud-capnhattruong',
                        level: 2
                    });
                };
            },
            updateMultipleCell: function () {
                var $modal = $("#baidang-crud");
                var $modal_CapNhatTruong = $("#baidang-crud-capnhattruong");

                var idNenTang = $("#select-nentang", $modal_CapNhatTruong).val(),
                    prompt = $("#input-prompt", $modal_CapNhatTruong).val();
                var rows = quanLyBaiDang.handleModal_CRUD.dataTable.rows().nodes().toArray(),
                    $rowChecks = $(`.checkRow-baidang-getList:checked`, rows);
                if ($rowChecks.length == 0) {
                    sys.alert({ status: "warning", mess: "Bạn chưa chọn bản ghi nào" })
                } else {
                    $.each($rowChecks, function () {
                        var $rowCheck = $(this).closest("tr"),
                            rowNumber = $rowCheck.attr("row"),
                            $div = $(`.baidang-read[row=${rowNumber}]`, $modal);
                        // Thay đổi value cho những dòng được chọn
                        $("#select-nentang", $div).val(idNenTang); $("#select-nentang", $div).trigger("change");
                        $("#input-prompt", $div).val(prompt); $("#input-prompt", $div).trigger("change");
                        $("#input-prompt", $div).text(prompt); $("#input-prompt", $div).trigger("change");
                    });

                    sys.alert({ status: "success", mess: "Cập nhật trường dữ liệu thành công" })
                    sys.displayModal({
                        name: '#baidang-crud-capnhattruong',
                        displayStatus: "hide",
                        level: 2,
                    });
                };
            },
            addBanGhi: function () {
                // Tạo mã guid cho bản ghi
                //var guid = sys.generateGUID();
                //#region Thêm bản ghi
                var $modal = $("#baidang-crud");
                $.ajax({
                    ...ajaxDefaultProps({
                        url: "/QuanLyBaiDang/addBanGhi_Modal_CRUD",
                        type: "GET",
                        //contentType: "application/json; charset=utf-8",
                    }),
                    success: function (res) {
                        quanLyBaiDang.handleModal_CRUD.dataTable.destroy();
                        // Tạo bản ghi mới
                        $("#baidang-getList tbody", $modal).prepend(res.html_baidang_row);
                        $("#baidang-read-container", $modal).prepend(res.html_baidang_read);
                        // Tạo dataTable
                        //if (!quanLyBaiDang.handleModal_CRUD.dataTable) {
                        quanLyBaiDang.handleModal_CRUD.dataTable = new DataTableCustom({
                            name: "baidang-getList",
                            table: $("#baidang-getList", $modal),
                            props: {
                                dom: `
                                <'row'<'col-sm-12'rt>>
                                <'row'<'col-sm-12 col-md-6 text-left'i><'col-sm-12 col-md-6 pt-2'p>>`,
                                lengthMenu: [
                                    [10, 50, -1],
                                    [10, 50, 'Tất cả'],
                                ],
                            }
                        }).dataTable;
                        //    // Nếu đã khởi tạo, chỉ cần thêm dòng mới (đã prepend bên trên rồi) và vẽ lại bảng
                        //}
                        //quanLyBaiDang.handleModal_CRUD.dataTable.row($(res.html_baidang_row)).invalidate().draw(false);
                        // Chọn bản ghi đó
                        var rows_NEW = quanLyBaiDang.handleModal_CRUD.dataTable.rows().nodes().toArray(); // Chọn phần thử đầu tiên của bảng
                        quanLyBaiDang.handleModal_CRUD.readRow($(rows_NEW[0]));
                        $.each($(".baidang-read", $modal), function () {
                            var $div = $(this),
                                rowNumber = $div.attr("row");
                            // Gán validation
                            htmlEl.validationStates($div);
                            htmlEl.inputMask();
                            var modalValidtion = htmlEl.activeValidationStates($div);
                        });
                        //quanLyBaiDang.handleModal_CRUD.readRow(res.idBaiDang);
                        /**
                          * Gán các thuộc tính
                          */
                        sys.displayModal({
                            name: '#baidang-crud'
                        });
                    }
                })
                //#endregion
            },
            deleteBanGhi: function () {
                var $modal = $("#baidang-crud");
                var rows = quanLyBaiDang.handleModal_CRUD.dataTable.rows().nodes().toArray(),
                    $rowChecks = $(`.checkRow-baidang-getList:checked`, rows);

                if ($rowChecks.length === 0) {
                    sys.alert({ status: "warning", mess: "Bạn chưa chọn bản ghi nào" });
                    return;
                }

                // Nếu số bản ghi được chọn >= tổng số dòng hiện có, cảnh báo và không xóa
                if ($rowChecks.length >= rows.length) {
                    sys.alert({ status: "warning", mess: "Phải giữ lại ít nhất một bản ghi!" });
                    return;
                }

                // Tiến hành xóa các bản ghi được chọn
                $.each($rowChecks, function () {
                    var $rowCheck = $(this).closest("tr"),
                        rowNumber = $rowCheck.attr("row"),
                        $div = $(`.baidang-read[row=${rowNumber}]`, $modal);
                    // Xóa bản ghi trong div
                    quanLyBaiDang.handleModal_CRUD.dataTable.row($rowCheck).remove().draw();
                    $div.remove();
                    // Xóa ảnh trong mảng
                    quanLyBaiDang.baiDang.handleAnhMoTa.arrAnh = quanLyBaiDang.baiDang.handleAnhMoTa.arrAnh
                        .filter(function (anh) {
                            return anh.rowNumber != rowNumber;
                        });
                });

                // Lấy lại các dòng mới sau khi xóa
                var rows_NEW = quanLyBaiDang.handleModal_CRUD.dataTable.rows().nodes().toArray();

                if (rows_NEW.length > 0) {
                    quanLyBaiDang.handleModal_CRUD.readRow($(rows_NEW[0]));
                } else {
                    $modal.find(".baidang-read").empty();
                }
            },
            readRow: function (el) {
                var $modal = $("#baidang-crud");
                var rowNumber = $(el).attr("row"),
                    rows = quanLyBaiDang.handleModal_CRUD.dataTable.rows().nodes().toArray(),
                    $divs = $(".baidang-read", $modal),
                    $div = $(`.baidang-read[row=${rowNumber}]`, $modal);

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
            save: function () {
                var baiDangs = [];
                $.each($(".baidang-read", $("#baidang-crud")), function () {
                    var $div = $(this),
                        rowNumber = $div.attr("row");
                    var baiDang = {
                        RowNumber: rowNumber,
                        BaiDang: {
                            IdChienDich: idChienDich,
                            IdBaiDang: $("#input-idbaidang", $div).val(),
                            IdNenTang: $("#select-nentang", $div).val(),
                            Prompt: $("#input-prompt", $div).val().trim(),
                            NoiDung: $("#input-noidung-ai", $div).val().trim(),
                            ThoiGian: $("#input-thoigian", $div).val(),
                            TuTaoAnhAI: $("#checkbox-sudunganh-ai", $div).is(":checked"),
                        },
                        TepDinhKems: []
                    };
                    baiDangs.push(baiDang);
                });

                //$.each($(`#tbody-anhmota-container tbody tr`), function () {
                //    let idTep = $(this).data("id");
                //    baiDang.TepDinhKems.push({
                //        IdTep: idTep,
                //    });
                //});
                sys.confirmDialog({
                    mess: `<p>Bạn có thực sự muốn thêm bản ghi này hay không ?</p>`,
                    callback: function () {
                        var formData = new FormData();
                        formData.append("baiDangs", JSON.stringify(baiDangs));

                        $.each(quanLyBaiDang.baiDang.handleAnhMoTa.arrAnh, function (idx, anh) {
                            formData.append("files", anh.file);
                            formData.append("rowNumbers", anh.rowNumber);
                        });

                        $.ajax({
                            ...ajaxDefaultProps({
                                url: "/QuanLyBaiDang/create_BaiDang",
                                type: "POST",
                                data: formData,
                            }),
                            //contentType: "application/json; charset=utf-8",  // Chỉ định kiểu nội dung là JSON
                            contentType: false,
                            processData: false,
                            success: function (res) {
                                if (res.status == "success") {
                                    quanLyBaiDang.baiDang.getList();

                                    sys.displayModal({
                                        name: '#baidang-crud',
                                        displayStatus: "hide"
                                    });
                                };
                                sys.alert({ status: res.status, mess: res.mess });
                            }
                        });
                    }
                });
            },
        };
    }
};