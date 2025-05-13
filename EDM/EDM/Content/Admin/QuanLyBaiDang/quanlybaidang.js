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
                idAnh: 0,
                arrAnh: [],
                add: function (loaiAnh = 'anhdaidien') {
                    var $imgContainer = $(`#${loaiAnh}-container`),
                        soAnhDangCo = $("tr", $imgContainer).length;

                    var addTr = function (files) {
                        var kiemTra = true,
                            mess = "Thêm ảnh thành công",
                            arrAnh = [];
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
                            quanLyBaiDang.baiDang.handleAnhMoTa.idAnh++;
                            var data = {
                                id: quanLyBaiDang.baiDang.handleAnhMoTa.idAnh,
                                file: f,
                                LaAnhDaiDien: loaiAnh == 'anhdaidien' ? true : false,
                                html: `<tr data-id="00000000-0000-0000-0000-000000000000" data-idtamthoi='${quanLyBaiDang.baiDang.handleAnhMoTa.idAnh}'>
                                    <td class="text-center">0</td>
                                    <td class="w-90">${f.name}</td>
                                    <td class="text-center">
                                        <a class="btn c-pointer" onclick="quanLyBaiDang.baiDang.handleAnhMoTa.delete('${loaiAnh}', this)">
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
                        $fileInput = $("#image-anhdaidien").get(0);
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
                        $fileInput = $("#image-anhmota").get(0);
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
                delete: function (loaiAnh, $this) {
                    var $tr = $($this).closest("tr"),
                        id = $tr.attr("data-id"),
                        idTamThoi = $tr.attr("data-idtamthoi");
                    // Xóa ảnh trên giao diện
                    $tr.remove();
                    // Xóa ảnh trong mảng
                    quanLyBaiDang.baiDang.handleAnhMoTa.arrAnh = quanLyBaiDang.baiDang.handleAnhMoTa.arrAnh
                        .filter(function (anh) {
                            return anh.id != idTamThoi;
                        });
                },
            },
            handleAI: {
                taoNoiDungAI: function () {
                    var noiDung = $("#input-noidung").val().trim();

                    noiDung && $.ajax({
                        ...ajaxDefaultProps({
                            url: "/QuanLyBaiDang/taoNoiDungAI",
                            type: "POST",
                            contentType: "application/json; charset=utf-8",
                            data: {
                                input: $("#input-noidung").val().trim()
                            },
                        }),
                        success: function (res) {
                            if (res.status == "success") {
                                $("#input-noidung-ai").text(res.NoiDung);
                            };
                            sys.alert({ mess: res.mess, status: res.status, timeout: 1500 });
                        }
                    })
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
                        /**
                          * Gán các thuộc tính
                          */
                        sys.displayModal({
                            name: '#baidang-crud'
                        });
                    }
                })
            },
            save: function (loai) {
                var $modal = $("#baidang-crud");
                var modalValidtion = htmlEl.activeValidationStates($modal);
                if (modalValidtion) {
                    var baiDangs = [];
                    var baiDang = {
                        BaiDang: {
                            IdBaiDang: $("#input-idbaidang", $modal).val(),
                            NoiDung: $("#input-noidung-ai", $modal).val().trim(),
                            ThoiGian: $("#input-thoigian", $modal).val(),
                        },
                        TuTaoAnh: $("#checkbox-sudunganh-ai").is(":checked"),
                        TepDinhKems: []
                    }
                    $.each($(`#anhmota-container tbody tr`), function () {
                        let idTep = $(this).data("id");
                        baiDang.TepDinhKems.push({
                            IdTep: idTep,
                        });
                    });
                    baiDangs.push(baiDang);
                    sys.confirmDialog({
                        mess: `<p>Bạn có thực sự muốn thêm bản ghi này hay không ?</p>`,
                        callback: function () {
                            var formData = new FormData();
                            formData.append("baiDangs", JSON.stringify(baiDangs));
                            formData.append("loai", loai);

                            $.each(quanLyBaiDang.baiDang.handleAnhMoTa.arrAnh, function (idx, anh) {
                                formData.append("files", anh.file);
                            });

                            $.ajax({
                                ...ajaxDefaultProps({
                                    url: loai == "create" ? "/QuanLyBaiDang/create_BaiDang" : "/QuanLyBaiDang/update_BaiDang",
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
                                        sys.alert({ status: res.status, mess: res.mess });
                                    } else {
                                        if (res.status == "warning") {
                                            htmlEl.inputValidationStates(
                                                $("#input-tenbaidang"),
                                                "#baidang-crud",
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
};