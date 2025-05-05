'use strict'
/**
 * main
 * */
class QuanLyTaiLieu {
    constructor() {
        this.page;
        this.pageGroup;
        this.taiLieu = {}
    }
    init() {
        var quanLyTaiLieu = this;
        var idNguoiDung_DangSuDung = $("#input-idnguoidung-dangsudung").val();
        quanLyTaiLieu.page = $("#page-quanlysanpham");

        quanLyTaiLieu.taiLieu = {
            ...quanLyTaiLieu.taiLieu,
            dataTable: null,
            getList: function () {
                $.ajax({
                    ...ajaxDefaultProps({
                        url: "/QuanLyTaiLieu/getList_TaiLieu",
                        type: "GET", // Phải là POST để gửi JSON
                        //contentType: "application/json; charset=utf-8",  // Định dạng JSON
                        //data: { locThongTin: quanLyTaiLieu.taiLieu.locThongTin.data }
                        //dataType: "json",
                    }),
                    //contentType: false,
                    //processData: false,
                    success: function (res) {
                        $("#tailieu-getList-container").html(res);
                        quanLyTaiLieu.taiLieu.dataTable = new DataTableCustom({
                            name: "tailieu-getList",
                            table: $("#tailieu-getList"),
                            props: {
                                lengthMenu: [
                                    [5, 10],
                                    [5, 10],
                                ],
                            }
                        }).dataTable;
                    }
                });
            },
            displayModal_XemChiTiet: function (idTaiLieu = '00000000-0000-0000-0000-000000000000') {
                $.ajax({
                    ...ajaxDefaultProps({
                        url: "/QuanLyTaiLieu/displayModal_XemChiTiet",
                        type: "POST",
                        data: { idTaiLieu }
                    }),
                    success: function (res) {
                        $("#tailieu-xemchitiet").html(res);

                        //var $imgContainer = $("#img-container")[0];
                        //var viewer = new Viewer($imgContainer);

                        sys.displayModal({
                            name: '#tailieu-xemchitiet'
                        });
                    }
                })
            },

            save: function () {
                var $fileInput = $("#input-file-tailieu").get(0),
                    kiemTra = true,
                    mess = `Tổng kích thước tệp gửi lên vượt quá giới hạn 1GB`,
                    maxSizeInBytes = 1024 * 1024 * 1024, // 1 GB,
                    real_maxSizeInBytes = 0,
                    formData = new FormData();
                $.each($fileInput.files, function (idx, f) {
                    var extension = f.type;
                    if (/\.(pdf|png|jpg|jpeg|mp3|mp4)$/i.test(f.name)) {
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
                        mess = `Tồn tại tệp không thuộc định dạng cho phép [pdf|png|jpg|jpeg|mp3|mp4]`;
                        kiemTra = false;
                    };
                });
                // Xóa bộ nhớ đệm để upload file trong lần tiếp theo
                $fileInput.value = ''; // xóa giá trị của input file
                if (!kiemTra || real_maxSizeInBytes >= maxSizeInBytes) {
                    sys.alert({
                        status: "error",
                        mess,
                        timeout: 5000
                    });
                } else {
                    $.ajax({
                        ...ajaxDefaultProps({
                            url: "/QuanLyTaiLieu/create_TaiLieu",
                            type: "POST",
                            data: formData
                        }),
                        contentType: false,
                        processData: false,
                        success: function (res) {
                            sys.displayModal({
                                name: '#tailieu-crud',
                                displayStatus: "hide"
                            });
                            sys.alert({ status: res.status, mess: res.mess })
                            quanLyTaiLieu.taiLieu.getList();
                        }
                    });
                };
            },
            delete: function (loai, id) {
                var idTaiLieus = [];
                // Lấy id
                if (loai == "single") {
                    idTaiLieus.push(id)
                } else {
                    quanLyTaiLieu.taiLieu.dataTable.rows().iterator('row', function (context, index) {
                        var $row = $(this.row(index).node());
                        if ($row.has("input.checkRow-vanban-getList:checked").length > 0) {
                            idTaiLieus.push($row.attr('id'));
                        };
                    });
                };
                // Kiểm tra id
                if (idTaiLieus.length > 0) {
                    var f = new FormData();
                    f.append("idTaiLieus", idTaiLieus.toString());
                    sys.confirmDialog({
                        mess: `Bạn có thực sự muốn xóa bản ghi này hay không ?`,
                        callback: function () {
                            $.ajax({
                                ...ajaxDefaultProps({
                                    url: "/QuanLyTaiLieu/delete_TaiLieu",
                                    type: "POST",
                                    data: f,
                                }),
                                contentType: false,
                                processData: false,
                                success: function (res) {
                                    sys.alert({ status: res.status, mess: res.mess })
                                    quanLyTaiLieu.taiLieu.getList();
                                }
                            })
                        }
                    })
                } else {
                    sys.alert({ mess: "Bạn chưa chọn bản ghi nào", status: "warning", timeout: 1500 })
                }
            },
        };
        quanLyTaiLieu.taiLieu.getList();
        sys.activePage({
            page: quanLyTaiLieu.page.attr("id"),
            pageGroup: quanLyTaiLieu.pageGroup
        });
    }
};