# Icon
	## Icon facebook: https://icon.fchat.vn/
	## Icon template: https://icons.getbootstrap.com/
# Ký tự
	## Ký tự đặc biệt: https://kytudacbiet.com/
# Quy tắc phân chia và đặt tên

	## FOLDER:
	loaiModule => module (VD: __Home)
	module => manhinhphu (VD: Report) - Nên tách nhỏ nếu màn hình phụ bao gồm các view nhỏ hơn, tiếp tục với các lớp sau đó

	## FILE-NAME:
	module-loaichucnang.chucnang (VD: Organization)
	module.manhinhphu-loaichucnang.chucnang (VD: Report)
	Màn hình chính mỗi của folder: FOLDER-NAME.cshtml (VD: Organization)
### NOTE
	## CSS 
		- hover table: background-color: var(--bs-table-hover-bg); color: var(--bs-table-color);

###### [THƯ VIỆN]
	## DataTables: https://datatables.net/
		- API: https://datatables.net/reference/api/
		- Render Data: 
		- Button:
			+ Thứ tự hiển thị: https://datatables.net/reference/option/dom
			+ Thêm button vào header: https://datatables.net/reference/api/button().add()
			+ Thêm button vào row: https://datatables.net/examples/ajax/null_data_source.html
		- Dropdown bị table-scroll che: https://stackoverflow.com/questions/18138785/bootstraps-dropdown-hidden-by-datatables
		- Lấy dữ liệu trên bảng: https://stackoverflow.com/questions/31932680/jquery-datatables-access-all-rows-data
			+ VD: digitizingsetting.js <save>
		- Truy cập thuộc tính của <table/>: https://datatables.net/reference/api/table()
			+ VD: dataTable.table()
		- Lấy response sau khi gọi ajax:
			+ VD: complete: function(data){
				var data = data.responseJSON.data;
			}
	## TreeView: https://github.com/jonmiles/bootstrap-treeview
		- Cách dùng: http://jonmiles.github.io/bootstrap-treeview/
		- Bắt buộc phải thêm 2 thuộc tính sau để có thể collapse:
			+  expandIcon: 'fa fa-angle-down fa-fw',
			+  collapseIcon: 'fa fa-angle-right fa-fw',
	## Validation HtmlElement (Tự viết)
		- Cách dùng: "Content/_Shared/_shared.js"
			+ Tại input thêm thuộc tính required, name=id
			+ Thêm thông báo là thẻ text bất kỳ

			VD => |	<div class="form-group mb-3">
                  |		<label class="required" for="input-tieudehoso">Tiêu đề hồ sơ</label>
                  |		<input type="text" class="form-control" placeholder="Nhập thông tin ..." 
                  |		       id="input-tieudehoso" 
			      |		       name="input-tieudehoso" required> <= (THÊM)
                  |		<div class="invalid-feedback feedback" for="input-tieudehoso">Không được để trống</div> <= (THÊM)
                  |	</div>
		- Cách kiểm tra:
			+  Gọi: var htmlEl = new HtmlElement(), modalValidtion = htmlEl.activeValidationStates("#documentformation-crud");
			modalValidtion => true/false
###### [FILE I.O]
	## Lấy toàn bộ file trong thư mục: https://www.geeksforgeeks.org/c-sharp-program-for-listing-the-files-in-a-directory/
		VD: 
		 // Get the directory
		DirectoryInfo place = new DirectoryInfo(@"C:\Train");
     
		// Using GetFiles() method to get list of all
		// the files present in the Train directory
		FileInfo[] Files = place.GetFiles();

###### [XỬ LÝ LỖI] 

# Lỗi lưu cache khiến đã xóa rồi nhưng vẫn không chạy => Xóa file bin & obj bằng tool 
	## Nếu clean solution không được:
		- Link tool: https://marketplace.visualstudio.com/items?itemName=dobrynin.cleanbinandobj&ssr=false#overview
		- Link giải đáp: https://stackoverflow.com/questions/1088593/how-to-fully-clean-bin-and-obj-folders-within-visual-studio
# Lệch cột trong datatable
	## Bao table bên trong 1 thẻ overflow : https://stackoverflow.com/questions/34897030/header-columns-misaligned-with-datatable-when-scrollx-is-enabled
	VD: that.table.wrap("<div style='overflow:auto; width:100%;position:relative;'></div>");

# TODO

- Xem xét lại trạng thái học của đơn hàng và trạng thái chăm sóc của khách hàng