'use strict'
class History {
    constructor() {
        this.page;
        this.pageGroup;
        this.history = {
            data: [],
            dataTable: null
        }
    }
    init() {
        var h = this;
        h.page = $("#page-history");
        h.pageGroup = h.page.attr("page-group");
        h.history = {
            ...h.history,
            dataTable: new DataTableCustom({
                name: "history-getList",
                table: $("#history-getList"),
                props: {
                    ajax: {
                        url: '/History/getList',
                        type: "GET",
                        conplete: function (data) {
                            h.history.data = data.responseJSON.data;
                        }
                    },
                    rowId: 'IdLichSuTruyCap',
                    columns: [
                        {
                            className: "text-center",
                            defaultContent: `<input class="form-check-input checkRow-history-getList" type="checkbox"/>`,
                            searchable: false,
                            orderable: false,
                        },
                        {
                            data: "NgayTao",
                            class: "text-center",
                            render: function (data, type, row, meta) {
                                return data == null ? "" : moment(data).format('DD-MM-YYYY')
                            }
                        },
                        {
                            data: "TenModule",
                            class: "text-left",
                        },
                        {
                            data: "NoiDungChiTiet",
                            class: "text-left",
                        },
                        {
                            data: "TenNguoiDung",
                            class: "text-left",
                        },
                        {
                            data: "TenDonViSuDung",
                            className: "text-left",
                        },
                    ],
                }
            }).dataTable
        }
        sys.activePage({
            page: h.page.attr("id"),
            pageGroup: h.pageGroup
        })
    }
}