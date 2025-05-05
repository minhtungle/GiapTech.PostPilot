'use strict'
class KhaiThacHoSo {
    constructor() {
        this.page;
        this.phieuMuon = {
            hienThiFile: function () { }
        };
        this.vanBanMuon = {
            dataTable: null,
        }
    }
    init() {
        var khaiThac = this;
        khaiThac.page = $("#page-khaithachosomuon");
        khaiThac.phieuMuon = {
            ...khaiThac.phieuMuon,
            hienThiFile: function (duongDan) {
                $("iframe", khaiThac.page).attr("src", `${duongDan}`);
            }
        };
        khaiThac.vanBanMuon = {
            ...khaiThac.vanBanMuon,
            dataTable: new DataTableCustom({
                name: "vanBanMuon-getList",
                table: $("#vanBanMuon-getList"),
                props: {
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
        };
        $("#vanBanMuon-getList tbody tr:first").trigger("click");
    }
}