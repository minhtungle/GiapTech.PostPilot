'use strict'
/**
 * main
 * */
class UserType {
    constructor() {
        this.page;
        this.pageGroup;
        this.kieuNguoiDung = {
            data: [],
            dataTable: null,
            idKieuNguoiDungs_XOA: []
        };
    }
    init() {
        var ut = this,
            idKieuNguoiDung_DangSuDung = $("#input-idkieunguoidung-dangsudung").val();
        ut.page = $("#page-usertype");
        ut.pageGroup = ut.page.attr("page-group");
        ut.kieuNguoiDung = {
            ...ut.kieuNguoiDung,
            dataTable: new DataTableCustom({
                name: "usertype-getList",
                table: $("#usertype-getList"),
                props: {
                    ajax: {
                        url: '/UserType/getList',
                        type: "GET",
                    },
                    rowId: 'IdKieuNguoiDung',
                    columns: [
                        {
                            data: null,
                            className: "text-center",
                            searchable: false,
                            orderable: false,
                            render: function (data, type, row, meta) {
                                if (idKieuNguoiDung_DangSuDung == data.IdKieuNguoiDung) {
                                    return ``;
                                }
                                return `<input class="form-check-input checkRow-usertype-getList" type="checkbox"/>`;
                            }
                        },
                        {
                            data: null,
                            render: function (data, type, row, meta) {
                                if (idKieuNguoiDung_DangSuDung == data.IdKieuNguoiDung) {
                                    return `${data.TenKieuNguoiDung} <span class="text-danger fst-italic"> (Đang sử dụng)</span>`;
                                }
                                return data.TenKieuNguoiDung;
                            }
                        },
                        { data: "GhiChu" },
                        {
                            data: null,
                            className: "text-center",
                            searchable: false,
                            orderable: false,
                            render: function (data, type, row, meta) {
                                if (idKieuNguoiDung_DangSuDung == data.IdKieuNguoiDung) {
                                    return `<a href="#" class="btn btn-sm btn-primary" title="Cập nhật" onclick="ut.kieuNguoiDung.displayModal_CRUD('update', '${data.IdKieuNguoiDung}')"><i class="bi bi-pencil-square"></i></a>`;
                                }
                                return `
                                <a href="#" class="btn btn-sm btn-primary" title="Cập nhật" onclick="ut.kieuNguoiDung.displayModal_CRUD('update', '${data.IdKieuNguoiDung}')"><i class="bi bi-pencil-square"></i></a>
                                <a href="#" class="btn btn-sm btn-danger" title="Xóa bỏ" onclick="ut.kieuNguoiDung.displayModal_Delete('single','${data.IdKieuNguoiDung}')"><i class="bi bi-trash3-fill"></i></a>
                                `;
                            }
                        }
                    ],
                }
            }).dataTable,
            displayModal_CRUD: function (loai = "", idKieuNguoiDung = '00000000-0000-0000-0000-000000000000') {
                $.ajax({
                    ...ajaxDefaultProps({
                        url: "/UserType/displayModal_CRUD",
                        type: "POST",
                        data: { loai, idKieuNguoiDung }
                        ,
                    }),
                    success: function (res) {
                        $("#usertype-crud").html(res);
                        treeView({ $table: $(".table-treeview") });
                        var kieuNguoiDung_IdChucNang = $("#input-idchucnang").val() != '' ? JSON.parse($("#input-idchucnang").val()) : [],
                            idChucNangs = kieuNguoiDung_IdChucNang.map(x => x.ChucNang).map(x => x.IdChucNang),
                            $chucNangs = $(".table-chucnang tr .checkbox-chucnang");
                        $.each($chucNangs, function () {
                            var idChucNang = $(this).attr("data-id"),
                                capDo = $(this).attr("data-capdo"),
                                $thaoTac = $(this).closest("tr").find(".checkbox-thaotac"); // Cấp độ phải khác 0 => chỉ check con
                            if (capDo != 0 && idChucNangs.includes(idChucNang)) {
                                // Chọn chức năng
                                $(this).trigger("click");
                                // Chọn thao tác
                                var idThaoTacs = kieuNguoiDung_IdChucNang.filter(x => x.ChucNang.IdChucNang == idChucNang)[0].ThaoTacs.map(x => x.IdThaoTac);
                                $.each($thaoTac, function () {
                                    var idThaoTac = $(this).attr("data-idthaotac");
                                    if (idThaoTacs.includes(idThaoTac)) {
                                        $(this).trigger("click");
                                    };
                                });
                            };
                        });
                        sys.displayModal({
                            name: '#usertype-crud'
                        });
                    }
                })
            },
            save: function (loai) {
                var modalValidtion = htmlEl.activeValidationStates("#usertype-crud");
                if (modalValidtion) {
                    var kieuNguoiDung = {
                        IdKieuNguoiDung: $("#input-idkieunguoidung", $("#usertype-crud")).val(),
                        TenKieuNguoiDung: $("#input-tenkieunguoidung", $("#usertype-crud")).val().trim(),
                        GhiChu: $("#input-ghichu", $("#usertype-crud")).val().trim(),
                    }
                    var chucNangs = [];
                    var $chucNangs = $(".table-chucnang tbody tr .checkbox-chucnang:checked");
                    $.each($chucNangs, function () {
                        var idChucNang = $(this).attr("data-id"),
                            maChucNang = $(this).attr("data-machucnang"),
                            capDo = $(this).attr("data-capdo"),
                            $thaoTacs = $(this).closest("tr").find(".checkbox-thaotac:checked"),
                            thaoTacs = [];
                        $.each($thaoTacs, function () {
                            var idThaoTac = $(this).attr("data-idthaotac"),
                                maThaoTac = $(this).attr("data-mathaotac");
                            thaoTacs.push({
                                IdThaoTac: idThaoTac,
                                MaThaoTac: maThaoTac,
                            });
                        });
                        //capDo != 0 &&
                        //chucNangs.push(idChucNang);
                        chucNangs.push({
                            ChucNang: {
                                IdChucNang: idChucNang,
                                MaChucNang: maChucNang,
                            },
                            ThaoTacs: thaoTacs
                        });
                    });
                    //kieuNguoiDung.IdChucNang = chucNangs.toString();
                    kieuNguoiDung.IdChucNang = JSON.stringify(chucNangs);

                    if (chucNangs.length == 0) {
                        sys.alert({ status: "warning", mess: "Bạn chưa chọn chức năng" })
                    } else {
                        $.ajax({
                            ...ajaxDefaultProps({
                                url: loai == "create" ? "/UserType/create_KieuNguoiDung" : "/UserType/update_KieuNguoiDung",
                                type: "POST",
                                data: {
                                    str_kieuNguoiDung: JSON.stringify(kieuNguoiDung),
                                }
                            }),
                            success: function (res) {
                                if (res.status == "success") {
                                    ut.kieuNguoiDung.dataTable.ajax.reload(function () {
                                        sys.displayModal({
                                            name: "#usertype-crud",
                                            displayStatus: "hide"
                                        });
                                        sys.alert({ status: res.status, mess: res.mess });
                                        var thongTinKiemTra = {
                                            NguoiDungs: [
                                                {
                                                    IdKieuNguoiDung: kieuNguoiDung.IdKieuNguoiDung,
                                                }
                                            ],
                                            LoaiHinhKiemTra: ["idkieunguoidung"],
                                            NoiDungThongBao: "Kiểu người dùng đã được thay đổi bởi [quản trị viên]"
                                        };
                                        chat.dangXuatNguoiDungHoatDong({ thongTinKiemTra: thongTinKiemTra });
                                    }, false);
                                } else if (res.status == "datontai") {
                                    htmlEl.inputValidationStates(
                                        $("#input-tenkieunguoidung"),
                                        "#usertype-crud",
                                        res.mess,
                                        {
                                            status: true,
                                            isvalid: false
                                        }
                                    );
                                    sys.alert({ status: "warning", mess: res.mess })
                                } else if (res.status == "logout") {
                                    sys.displayModal({
                                        name: "#usertype-crud",
                                        displayStatus: "hide"
                                    });
                                    sys.logoutDialog({
                                        mess: res.mess,
                                    });
                                } else {
                                    sys.alert({ status: res.status, mess: res.mess })
                                };
                            }
                        });
                    };
                };
            },
            displayModal_Delete: function (loai, idKieuNguoiDung = '00000000-0000-0000-0000-000000000000') {
                //#region Kiểm tra điều kiện
                ut.kieuNguoiDung.idKieuNguoiDungs_XOA.length = 0;
                if (loai == "single") {
                    ut.kieuNguoiDung.idKieuNguoiDungs_XOA.push(idKieuNguoiDung)
                } else {
                    var rows = ut.kieuNguoiDung.dataTable.rows().nodes().toArray(),
                        $rowChecks = $(`.checkRow-usertype-getList:checked`, rows);
                    if ($rowChecks.length == 0) {
                        sys.alert({ status: "warning", mess: "Bạn chưa chọn bản ghi nào" });
                        return 0;
                    } else if ($rowChecks.length == rows.length) {
                        sys.alert({ status: "warning", mess: "Không thể xóa toàn bộ bởi cần giữ lại 1 bản ghi thay thế cho các kiểu người dùng đang sử dụng" });
                        return 0;
                    } else {
                        $.each($rowChecks, function () {
                            var $rowCheck = $(this).closest("tr");
                            ut.kieuNguoiDung.idKieuNguoiDungs_XOA.push($rowCheck.attr("id"));
                        });
                    };
                };
                //#endregion
                if (ut.kieuNguoiDung.idKieuNguoiDungs_XOA.length > 0) {
                    var f = new FormData();
                    f.append("str_idKieuNguoiDungs_XOA", JSON.stringify(ut.kieuNguoiDung.idKieuNguoiDungs_XOA));
                    $.ajax({
                        ...ajaxDefaultProps({
                            url: "/UserType/displayModal_Delete",
                            type: "POST",
                            data: f,
                        }),
                        contentType: false,
                        processData: false,
                        success: function (res) {
                            $("#usertype-delete").html(res);
                            ut.kieuNguoiDung.dataTable_THAYTHE = new DataTableCustom({
                                name: "usertype-thaythe-getList",
                                table: $("#usertype-thaythe-getList"),
                                props: {
                                    dom: `
                                <'row'<'col-sm-12'rt>>
                                <'row'<'col-sm-12 col-md-4 pt-2'l><'col-sm-12 col-md-4 text-center'i><'col-sm-12 col-md-4 pt-2'p>>`,
                                },
                                initCompleteProps: function () { }
                            }).dataTable;
                            // Gán check cho bảng thay thế
                            var rows_THAYTHE = ut.kieuNguoiDung.dataTable_THAYTHE.rows().nodes().toArray(); // Tất cả dòng của bảng thay thế
                            singleCheck({
                                name: "usertype-thaythe-getList",
                                parent: $(rows_THAYTHE)
                            });
                            sys.displayModal({
                                name: "#usertype-delete"
                            });
                        }
                    });
                };
            },
            delete: function () {
                var rows_THAYTHE = ut.kieuNguoiDung.dataTable_THAYTHE.rows().nodes().toArray(), // Tất cả dòng của bảng thay thế
                    rowChecks_THAYTHE = $(`.checkRow-usertype-thaythe-getList:checked`, rows_THAYTHE); // Dòng đang chọn ở bảng thay thế
                if (rowChecks_THAYTHE.length == 0) {
                    sys.alert({ status: "warning", mess: "Bạn chưa chọn kiểu người dùng thay thế" })
                } else {
                    var f = new FormData();
                    f.append("str_idKieuNguoiDungs_XOA", JSON.stringify(ut.kieuNguoiDung.idKieuNguoiDungs_XOA));
                    f.append("idKieuNguoiDung_THAYTHE", $(rowChecks_THAYTHE[0]).data("id"));
                    $.ajax({
                        ...ajaxDefaultProps({
                            url: "/UserType/delete_KieuNguoiDungs",
                            type: "POST",
                            data: f,
                        }),
                        contentType: false,
                        processData: false,
                        success: function (res) {
                            if (res.status == "success") {
                                ut.kieuNguoiDung.dataTable.ajax.reload(function () {
                                    sys.displayModal({
                                        name: "#usertype-delete",
                                        displayStatus: "hide"
                                    });
                                    sys.alert({ status: res.status, mess: res.mess });
                                    var thongTinKiemTra = {
                                        NguoiDungs: ut.kieuNguoiDung.idKieuNguoiDungs_XOA.map(idKieuNguoiDung_XOA => ({
                                            IdKieuNguoiDung: idKieuNguoiDung_XOA
                                        })),
                                        LoaiHinhKiemTra: ["idkieunguoidung"],
                                        NoiDungThongBao: "Kiểu người dùng đã được thay đổi bởi [quản trị viên]"
                                    };
                                    chat.dangXuatNguoiDungHoatDong({ thongTinKiemTra: thongTinKiemTra });
                                }, false);
                            } else if (res.status == "logout") {
                                sys.displayModal({
                                    name: "#usertype-delete",
                                    displayStatus: "hide"
                                });
                                sys.logoutDialog({
                                    mess: res.mess,
                                });
                            } else {
                                sys.alert({ status: res.status, mess: res.mess });
                            };

                        }
                    })
                };
            },
        };
        sys.activePage({
            page: ut.page.attr("id"),
            pageGroup: ut.pageGroup
        });
    }
};