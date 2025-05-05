/**
* PDFAnnotate v1.0.1
* Author: Ravisha Heshan
*/
const { PDFDocument } = PDFLib
var PDFAnnotate = function (container_id, url, options = {}) {
    this.arr_pages_number = [];
    this.number_of_pages = 0;
    this.pages_rendered = 0;
    this.active_tool = 1; // 1 - Free hand, 2 - Text, 3 - Arrow, 4 - Rectangle
    this.fabricObjects = [];
    this.fabricObjectsData = [];
    this.color = '#212121';
    this.borderColor = '#000000';
    this.borderSize = 1;
    this.font_size = 16;
    this.active_canvas = 0;
    this.container_id = container_id;
    this.url = url;
    this.pageImageCompression = options.pageImageCompression
        ? options.pageImageCompression.toUpperCase()
        : "NONE";
    var inst = this;

    /**
     * Render all pdf file 
     */
    var loadingTask = pdfjsLib.getDocument(this.url);
    loadingTask.promise.then(function (pdf) {
        var scale = options.scale ? options.scale : 1.3;
        // Nếu truyền pages - [<Trang lấy>, <Trang không lấy>]
        if (options.pages) {
            inst.arr_pages_number = Array.from({ length: pdf.numPages }, (_, index) => index + 1); // Danh sách tất cả trang
            var pageGets = options.pages[0] ?? [], // Trang lấy
                pageNotGets = options.pages[1] ?? []; // Trang không lấy
            inst.arr_pages_number = inst.arr_pages_number.filter(x => pageGets.includes(x) && !pageNotGets.includes(x)); // Lọc trang lấy
            if (inst.arr_pages_number.length == 0) inst.arr_pages_number = [1]; // mặc định trang 1
        };
        //inst.number_of_pages = pdf.numPages;
        inst.number_of_pages = inst.arr_pages_number.length; // Gán tổng trang
        // Duyệt qua tất cả các trang của pdf rồi render từng trang
        var getPages = (numPage) => {
            pdf.getPage(numPage).then(function (page) {
                var viewport = page.getViewport({ scale });
                var canvas = document.createElement('canvas');
                document.getElementById(inst.container_id).appendChild(canvas);
                canvas.className = 'pdf-canvas';
                canvas.height = viewport.height;
                canvas.width = viewport.width;
                context = canvas.getContext('2d');

                var renderContext = {
                    canvasContext: context,
                    viewport: viewport
                };
                var renderTask = page.render(renderContext);
                renderTask.promise.then(function () {
                    $('.pdf-canvas').each(function (index, el) {
                        $(el).attr('id', 'page-' + (index + 1) + '-canvas');
                    });
                    inst.pages_rendered++;
                    // Chạy hàm này ở trang cuối cùng
                    if (inst.pages_rendered == inst.number_of_pages)
                        inst.initFabric();
                });
            });
        };
        inst.arr_pages_number.forEach(page => {
            getPages(page);
        });
    }, function (reason) {
        console.error(reason);
    });
    //tạo 1 canvas lên trên pdf để vẽ 
    this.initFabric = function () {
        var inst = this;
        let canvases = $('#' + inst.container_id + ' canvas')
        canvases.each(function (index, el) {
            var background = el.toDataURL("image/png", 1);
            var fabricObj = new fabric.Canvas(el.id, {
                freeDrawingBrush: {
                    width: 1,
                    color: inst.color
                }
            });
            inst.fabricObjects.push(fabricObj);
            if (typeof options.onPageUpdated == 'function') {
                fabricObj.on('object:added', function () {
                    var oldValue = Object.assign({}, inst.fabricObjectsData[index]);
                    inst.fabricObjectsData[index] = fabricObj.toJSON()
                    options.onPageUpdated(index + 1, oldValue, inst.fabricObjectsData[index])
                })
            }
            fabricObj.setBackgroundImage(background, fabricObj.renderAll.bind(fabricObj));
            $(fabricObj.upperCanvasEl).click(function (event) {
                inst.active_canvas = index;
                inst.fabricClickHandler(event, fabricObj);
            });
            fabricObj.on('after:render', function () {
                inst.fabricObjectsData[index] = fabricObj.toJSON()
                fabricObj.off('after:render')
            })

            if (index === canvases.length - 1 && typeof options.ready === 'function') {
                options.ready()
            }
        });
    }

    this.fabricClickHandler = function (event, fabricObj) {
        var inst = this;
        if (inst.active_tool == 2) {
            var text = new fabric.IText('Sample text', {
                left: event.clientX - fabricObj.upperCanvasEl.getBoundingClientRect().left,
                top: event.clientY - fabricObj.upperCanvasEl.getBoundingClientRect().top,
                fill: inst.color,
                fontSize: inst.font_size,
                selectable: true
            });
            fabricObj.add(text);
            inst.active_tool = 0;
        }
    }
}

PDFAnnotate.prototype.savePdf = async function (fileName) {
    var inst = this;
    if (typeof fileName === 'undefined') {
        fileName = `${new Date().getTime()}.pdf`;
    };
    const existingPdfBytes = await fetch(inst.url).then(res => res.arrayBuffer());
    var pdfDoc = await PDFDocument.load(existingPdfBytes),
        newPdfDoc = await PDFDocument.create(),
        copiedPages = await newPdfDoc.copyPages(pdfDoc, inst.arr_pages_number.map(x => x - 1));
    // Chỉ lấy các trang được chọn
    copiedPages.forEach(newPage => {
        newPdfDoc.addPage(newPage);
    });
    pdfDoc = newPdfDoc; // Thay thế pdfDoc
    const pages = pdfDoc.getPages();
    //const pages = pdfDoc.getPages().filter((page, page_index) => inst.arr_pages_number.includes(page_index + 1)); // (số trang) = (vị trí trong mảng + 1)
    inst.fabricObjects.forEach(function (pdf, index) {
        // Vì trang được chọn và inst.fabricObjects tương đồng về vị trí phần tử mảng
        const page = pages[index];
        pdf._objects.forEach(async function (image, index) {
            //get position of image
            let left = image.aCoords.bl.x;
            let top = image.aCoords.bl.y;
            let scaleX = image.scaleX;
            let scaleY = image.scaleY;
            let rotation = PDFLib.degrees(0);
            /**
             * Mặc đình PDF sẽ là nằm ngang
             * __________________
             * |                | 
             * |                | 
             * |                | 
             * |                | height
             * |                | 
             * |                | 
             * __________________
             *          width        
             */
            const { width, height } = page.getSize();
            let x = 0,
                y = 0,
                pageRotation = page.getRotation().angle;
            rotation = PDFLib.degrees(-pageRotation);
            if (pageRotation == 90) {
                /**
                 *             
                 *     __________________
                 *     |              x |
                 *     |              ^ |
                 *     |              | |
                 *     |              | | width
                 *     |              | |
                 *     | y <----------o |
                 *     ------------------
                 *            height
                 */
                x = width - top;
                y = height - left;
                //rotation = PDFLib.degrees(-90); // Quay ảnh chèn theo chiều quay của PDF
            } else if (pageRotation == 180) {
                /**
                 *            width
                 *     __________________
                 *     | x <----------o |
                 *     |              | |
                 *     |              | |
                 *     |              | | height
                 *     |              | |
                 *     |              y |
                 *     ------------------
                 *             
                 */
                x = width - left;
                y = top;
                //rotation = PDFLib.degrees(-180); // Quay ảnh chèn theo chiều quay của PDF
            } else if (pageRotation == 270) {
                /**
                 *            height
                 *     __________________
                 *     | o ----------> y|
                 *     | |              |
                 *     | |              |
                 *     | |              | width
                 *     | |              | 
                 *     | x              |
                 *     ------------------
                 *             
                 */
                //x = top;
                //y = left;
                x = width - top;
                y = height - left;
                rotation = PDFLib.degrees(-90); // Quay ảnh chèn theo chiều quay của PDF
                // Đéo hiểu sao đúng
            }
            else { // 0 và 360
                /**
                 *             
                 *     __________________
                 *     | y              |
                 *     | ^              |
                 *     | |              |
                 *     | |              | height
                 *     | |              |
                 *     | o----------> x |
                 *     ------------------
                 *            width 
                 */
                x = left;
                y = height - top;
                rotation = PDFLib.degrees(0);
            };

            //get type of image
            let imageSrc = image._element.currentSrc;
            let arraySrc = imageSrc.split(';');
            if (arraySrc[0].search("png") == -1) {
                const jpgImage = await pdfDoc.embedJpg(imageSrc)
                //draw image on pdf
                page.drawImage(jpgImage, {
                    x: x,
                    y: y,
                    width: jpgImage.scale(scaleX).width,
                    height: jpgImage.scale(scaleY).height,
                    rotate: rotation
                })
            } else {
                const pngImage = await pdfDoc.embedPng(imageSrc)
                //draw image on pdf
                page.drawImage(pngImage, {
                    x: x,
                    y: y,
                    width: pngImage.scale(scaleX).width,
                    height: pngImage.scale(scaleY).height,
                    rotate: rotation
                })
            }
        })
    });
    const pdfBytes = await pdfDoc.saveAsBase64({ dataUri: true })
    return pdfBytes;
    ////var binary = '';
    ////var bytes = [].slice.call(pdfBytes);
    ////bytes.forEach((b) => binary += String.fromCharCode(b));
    ////return window.btoa(binary);
    //download(pdfBytes, fileName, "application/pdf");
}

PDFAnnotate.prototype.enableSelector = function () {
    var inst = this;
    inst.active_tool = 0;
    if (inst.fabricObjects.length > 0) {
        $.each(inst.fabricObjects, function (index, fabricObj) {
            fabricObj.isDrawingMode = false;
        });
    }
}

PDFAnnotate.prototype.enablePencil = function () {
    var inst = this;
    inst.active_tool = 1;
    if (inst.fabricObjects.length > 0) {
        $.each(inst.fabricObjects, function (index, fabricObj) {
            fabricObj.isDrawingMode = true;
        });
    }
}

PDFAnnotate.prototype.enableAddText = function () {
    var inst = this;
    inst.active_tool = 2;
    if (inst.fabricObjects.length > 0) {
        $.each(inst.fabricObjects, function (index, fabricObj) {
            fabricObj.isDrawingMode = false;
        });
    }
}

PDFAnnotate.prototype.enableRectangle = function () {
    var inst = this;
    var fabricObj = inst.fabricObjects[inst.active_canvas];
    inst.active_tool = 4;
    if (inst.fabricObjects.length > 0) {
        $.each(inst.fabricObjects, function (index, fabricObj) {
            fabricObj.isDrawingMode = false;
        });
    }

    var rect = new fabric.Rect({
        width: 100,
        height: 100,
        fill: inst.color,
        stroke: inst.borderColor,
        strokeSize: inst.borderSize
    });
    fabricObj.add(rect);
}

PDFAnnotate.prototype.enableAddArrow = function () {
    var inst = this;
    inst.active_tool = 3;
    if (inst.fabricObjects.length > 0) {
        $.each(inst.fabricObjects, function (index, fabricObj) {
            fabricObj.isDrawingMode = false;
            new Arrow(fabricObj, inst.color, function () {
                inst.active_tool = 0;
            });
        });
    }
}

PDFAnnotate.prototype.addImageToCanvas = function (url) {
    var inst = this;
    var fabricObj = inst.fabricObjects[inst.active_canvas];
    if (fabricObj) {
        var image = new Image();
        toDataURL(url, function (dataUrl) {
            image.src = dataUrl;
        });
        image.onload = function () {
            fabricObj.add(new fabric.Image(image, {
                lockRotation: true,
                scaleX: .5,
                scaleY: .5,
            }))
        }

    }
}
function toDataURL(url, callback) {
    var xhr = new XMLHttpRequest();
    xhr.onload = function () {
        var reader = new FileReader();
        reader.onloadend = function () {
            callback(reader.result);
        }
        reader.readAsDataURL(xhr.response);
    };
    xhr.open('GET', url);
    xhr.responseType = 'blob';
    xhr.send();
}
function MakeBlob(dataURL) {
    var BASE64_MARKER = ';base64,';
    if (dataURL.indexOf(BASE64_MARKER) == -1) {
        var parts = dataURL.split(',');
        var contentType = parts[0].split(':')[1];
        var raw = decodeURIComponent(parts[1]);
        return new Blob([raw], { type: contentType });
    }
    var parts = dataURL.split(BASE64_MARKER);
    var contentType = parts[0].split(':')[1];
    var raw = window.atob(parts[1]);
    var rawLength = raw.length;

    var uInt8Array = new Uint8Array(rawLength);

    for (var i = 0; i < rawLength; ++i) {
        uInt8Array[i] = raw.charCodeAt(i);
    }

    return new Blob([uInt8Array], { type: contentType });
}
PDFAnnotate.prototype.deleteSelectedObject = function () {
    var inst = this;
    var activeObject = inst.fabricObjects[inst.active_canvas].getActiveObject();
    if (activeObject) {
        if (confirm('Are you sure ?')) inst.fabricObjects[inst.active_canvas].remove(activeObject);
    }
}

PDFAnnotate.prototype.setBrushSize = function (size) {
    var inst = this;
    $.each(inst.fabricObjects, function (index, fabricObj) {
        fabricObj.freeDrawingBrush.width = size;
    });
}

PDFAnnotate.prototype.setColor = function (color) {
    var inst = this;
    inst.color = color;
    $.each(inst.fabricObjects, function (index, fabricObj) {
        fabricObj.freeDrawingBrush.color = color;
    });
}

PDFAnnotate.prototype.setBorderColor = function (color) {
    var inst = this;
    inst.borderColor = color;
}

PDFAnnotate.prototype.setFontSize = function (size) {
    this.font_size = size;
}

PDFAnnotate.prototype.setBorderSize = function (size) {
    this.borderSize = size;
}

PDFAnnotate.prototype.clearActivePage = function () {
    var inst = this;
    var fabricObj = inst.fabricObjects[inst.active_canvas];
    var bg = fabricObj.backgroundImage;
    if (confirm('Are you sure?')) {
        fabricObj.clear();
        fabricObj.setBackgroundImage(bg, fabricObj.renderAll.bind(fabricObj));
    }
}

PDFAnnotate.prototype.serializePdf = function () {
    var inst = this;
    return JSON.stringify(inst.fabricObjects, null, 4);
}

PDFAnnotate.prototype.loadFromJSON = function (jsonData) {
    var inst = this;
    $.each(inst.fabricObjects, function (index, fabricObj) {
        if (jsonData.length > index) {
            fabricObj.loadFromJSON(jsonData[index], function () {
                inst.fabricObjectsData[index] = fabricObj.toJSON()
            })
        }
    })
}

// PDFAnnotate.prototype.savePdfTeu = function (fileName) {
//     var inst = this;
//     var doc = new jspdf.jsPDF();
//     if (typeof fileName === 'undefined') {
//         fileName = `${new Date().getTime()}.pdf`;
//     }

//     inst.fabricObjects.forEach(function (fabricObj, index) {
//         if (index != 0) {
//             doc.addPage();
//             doc.setPage(index + 1);
//         }
//         doc.addImage(
//             fabricObj.toDataURL({
//                 format: 'pdf'
//             }),
//             inst.pageImageCompression == "NONE" ? "PNG" : "JPEG",
//             0,
//             0,
//             doc.internal.pageSize.getWidth(),
//             doc.internal.pageSize.getHeight(),
//             `page-${index + 1}`,
//             ["FAST", "MEDIUM", "SLOW"].indexOf(inst.pageImageCompression) >= 0
//                 ? inst.pageImageCompression
//                 : undefined
//         );
//         if (index === inst.fabricObjects.length - 1) {
//             doc.save(fileName);
//         }
//     })
// }