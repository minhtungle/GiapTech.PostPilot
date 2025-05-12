'use strict'
/**
 * main
 * */
class QuanLyChienDich {
    constructor() {
        this.page;
        this.pageGroup;
        this.chienDich = {}
    }
    init() {
        var quanLyChienDich = this;
        var idNguoiDung_DangSuDung = $("#input-idnguoidung-dangsudung").val();
        quanLyChienDich.page = $("#page-quanlychiendich");

        quanLyChienDich.chienDich = {
            ...quanLyChienDich.chienDich,
            dataTable: null,
            getList: function () {
                $.ajax({
                    ...ajaxDefaultProps({
                        url: "/QuanLyChienDich/getList_ChienDich",
                        type: "GET", // Phải là POST để gửi JSON
                        //contentType: "application/json; charset=utf-8",  // Định dạng JSON
                        //data: { locThongTin: quanLyChienDich.baiDang.locThongTin.data }
                        //dataType: "json",
                    }),
                    //contentType: false,
                    //processData: false,
                    success: function (res) {
                        $("#chiendich-getList-container").html(res);
                        quanLyChienDich.chienDich.dataTable = new DataTableCustom({
                            name: "chiendich-getList",
                            table: $("#chiendich-getList"),
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
            xemChiTiet: function (idChienDich = '00000000-0000-0000-0000-000000000000') {
                $.ajax({
                    ...ajaxDefaultProps({
                        url: "/QuanLyChienDich/displayModal_CRUD_BaiDang",
                        type: "POST",
                        contentType: "application/json; charset=utf-8",
                        data: {
                            idChienDich
                        },
                    }),
                    success: function (res) {
                        $("#chiendich-crud").html(res);
                        /**
                          * Gán các thuộc tính
                          */
                        sys.displayModal({
                            name: '#chiendich-crud'
                        });
                    }
                })
            }
        };

        quanLyChienDich.chienDich.getList();
        sys.activePage({
            page: quanLyChienDich.page.attr("id"),
            pageGroup: quanLyChienDich.pageGroup
        });
    }
};