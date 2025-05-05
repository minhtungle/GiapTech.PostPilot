'use strict'
class Guide {
    constructor() {
        this.page;
        this.pageGroup;
    }
    init() {
        var guide = this;
        guide.page = $("#page-guide");
        guide.pageGroup = guide.page.attr("page-group");
        sys.activePage({
            page: this.page.attr("id"),
            pageGroup: this.pageGroup
        })
    }
    getFile() {
        $.ajax({
            ...ajaxDefaultProps({
                url: "/Guide/getFile",
            }),
            success: function (res) {
                $("#getFile").html(res);
            }
        })
    }
    save() {
        var guide = this,
            $selectPDF = $("#select-file").get(0),
            kiemTra = true,
            mess = "",
            maxSizeInBytes = 500 * 1024 * 1024, // 500 MB
            formData = new FormData();
        $.each($selectPDF.files, function (idx, f) {
            var extension = f.type;
            if (/\.(doc|docx|xls|xlsx|pdf|png|jpg|jpeg|mp4)$/i.test(f.name)) {
                if (f.size > maxSizeInBytes) {
                    kiemTra = false;
                    mess = `Tồn tại tệp có kích thước tệp vượt quá giới hạn 500MB`;
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
        if (!kiemTra) {
            sys.alert({
                status: "error",
                mess,
                timeout: 5000
            });
        } else {
            $.ajax({
                ...ajaxDefaultProps({
                    //url: "/Guide/Test",
                    url: "/Guide/create_HuongDan",
                    type: "POST",
                    data: formData
                }),
                contentType: false,
                processData: false,
                success: function (res) {
                    guide.getFile();
                    sys.alert({ status: res.status, mess: res.mess })
                }
            });
        };
    }
    dropHandler(ev) {
        //console.log("File(s) dropped");
        // Prevent default behavior (Prevent file from being opened)
        var formData = new FormData();
        ev.preventDefault();
        if (ev.dataTransfer.items) {
            // Use DataTransferItemList interface to access the file(s)
            [...ev.dataTransfer.items].forEach((item, i) => {
                // If dropped items aren't files, reject them
                if (item.kind === "file") {
                    const f = item.getAsFile();
                    var extension = f.type;
                    if (extension.includes("msword") || extension.includes("pdf")) {
                        formData.append("files", f);
                    } else {
                        sys.alert({ status: "error", mess: "Tệp đính kèm không đúng định dạng, chỉ nhận tệp .pdf và .docx", timeout: 5000 })
                    }
                } else {
                    sys.alert({ status: "error", mess: "Tệp đính kèm không đúng định dạng" })
                }
            });
        } else {
            // Use DataTransfer interface to access the file(s)
            [...ev.dataTransfer.files].forEach((f, i) => {
                var extension = f.type;
                if (extension.includes("msword") || extension.includes("pdf")) {
                    formData.append("files", f);
                }
            });
        }
        $.ajax({
            ...ajaxDefaultProps({
                url: "/Guide/create",
                type: "POST",
                data: formData
            }),
            contentType: false,
            processData: false,
            success: function (res) {
                guide.getFile();
                sys.alert({ status: res.status, mess: res.mess })
            }
        })
    }
    dragOverHandler(ev) {
        //console.log("File(s) in drop zone");
        // Prevent default behavior (Prevent file from being opened)
        ev.preventDefault();
    }
}