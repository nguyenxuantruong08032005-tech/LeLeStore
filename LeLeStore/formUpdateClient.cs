using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LeLeStore
{
    public partial class formUpdateClient : Form
    {
        // ---- đặt trong class ----
        private void CommitUI()
        {
            Validate();
            try { dataGridView1.EndEdit(); } catch { }
            khachHangBindingSource.EndEdit();
        }

        private bool AskConfirm(string msg, MessageBoxIcon icon = MessageBoxIcon.Question)
        {
            return MessageBox.Show(msg, "Xác nhận", MessageBoxButtons.YesNo, icon) == DialogResult.Yes;
        }

        private void RefreshDataAndRestoreByKey(int? keyToFocus)
        {
            gStoreDataSet.KhachHang.Clear();
            khachHangTableAdapter.Fill(gStoreDataSet.KhachHang);

            if (keyToFocus.HasValue)
            {
                var rows = gStoreDataSet.KhachHang.Select($"MaKhachHang = {keyToFocus.Value}");
                if (rows.Length > 0)
                {
                    var row = rows[0];
                    int pos = gStoreDataSet.KhachHang.Rows.IndexOf(row);
                    if (pos >= 0) khachHangBindingSource.Position = pos;
                }
            }

            PopulateInputsFromSelection();
        }

     

        // ---- Cập nhật SetOperation để đổi text/khóa nút ----
        private void SetOperation(ClientOperation operation)
        {
            _currentOperation = operation;

            bool canEditFields = operation == ClientOperation.Add || operation == ClientOperation.Edit;
            bool isDelete = operation == ClientOperation.Delete;

            txtMaKH.ReadOnly = true;
            txtTenKH.ReadOnly = !canEditFields;
            txtSDTKH.ReadOnly = !canEditFields;
            txtDiaChiKH.ReadOnly = !canEditFields;
            cboMaNV.Enabled = canEditFields;

            // Điểm chỉ nhập khi Thêm
            txtDiem.ReadOnly = operation != ClientOperation.Add;

            if (isDelete)
            {
                txtTenKH.ReadOnly = txtSDTKH.ReadOnly = txtDiaChiKH.ReadOnly =
                 cboMaNV.Enabled = false;
                txtDiem.ReadOnly = true;
            }

            // Đồng bộ nút (giả sử bạn có 3 nút: btnThem, btnSua, btnXoa)
            btnThem.Enabled = (operation != ClientOperation.Edit && operation != ClientOperation.Delete);
            btnSua.Enabled = (operation != ClientOperation.Add && operation != ClientOperation.Delete);
            btnXoa.Enabled = (operation == ClientOperation.None);

            btnThem.Text = (operation == ClientOperation.Add) ? "Lưu" : "Thêm";
            btnSua.Text = (operation == ClientOperation.Edit) ? "Lưu" : "Sửa";
        }

        private enum ClientOperation
        {
            None,
            Add,
            Edit,
            Delete
        }
        private readonly string _username;
        private readonly GStoreDataSetTableAdapters.NhanVienTableAdapter _nhanVienTableAdapter = new GStoreDataSetTableAdapters.NhanVienTableAdapter();
        private readonly GStoreDataSetTableAdapters.NguoiDungTableAdapter _nguoiDungTableAdapter = new GStoreDataSetTableAdapters.NguoiDungTableAdapter();
        private ClientOperation _currentOperation = ClientOperation.None;
        public formUpdateClient(string username = "")
        {
            _username = username ?? string.Empty;
            InitializeComponent();
            btnHuy.CausesValidation = false;
            SetOperation(ClientOperation.None);
            PopulateInputsFromSelection();
            LoadEmployeeOptions();
        }

        private void formUpdateClient_Load(object sender, EventArgs e)
        {
            // TODO: This line of code loads data into the 'gStoreDataSet.KhachHang' table. You can move, or remove it, as needed.
            this.khachHangTableAdapter.Fill(this.gStoreDataSet.KhachHang);
            PopulateInputsFromSelection();
            LoadEmployeeOptions();
        }
      

        private void PopulateInputsFromSelection()
        {
            var row = GetCurrentClientRow();
            if (row != null)
            {
                PopulateInputs(row);
            }
            else
            {
                ClearInputFields();
            }
        }

        private void PopulateInputs(GStoreDataSet.KhachHangRow row)
        {
            txtMaKH.Text = row.MaKhachHang.ToString();
            txtTenKH.Text = row.IsNull("HoTen") ? string.Empty : row.HoTen;
            txtSDTKH.Text = row.IsNull("SoDienThoai") ? string.Empty : row.SoDienThoai;
            txtDiaChiKH.Text = row.IsNull("DiaChi") ? string.Empty : row.DiaChi;
            txtDiem.Text = row.DiemTichLuy.ToString();
            SetEmployeeSelection(row.IsNull("MaNhanVien") ? (int?)null : row.MaNhanVien);
        }

        private void ClearInputFields()
        {
            txtMaKH.Text = string.Empty;
            txtTenKH.Text = string.Empty;
            txtSDTKH.Text = string.Empty;
            txtDiaChiKH.Text = string.Empty;
            txtDiem.Text = string.Empty;
            SetEmployeeSelection(null);
        }

        private GStoreDataSet.KhachHangRow GetCurrentClientRow()
        {
            if (dataGridView1.CurrentRow?.DataBoundItem is DataRowView rowView)
            {
                return rowView.Row as GStoreDataSet.KhachHangRow;
            }

            return null;
        }

        private bool TryValidateInputsForAdd(out string hoTen, out string soDienThoai, out string diaChi, out int diemTichLuy, out int maNhanVien)
        {
            hoTen = txtTenKH.Text.Trim();
            soDienThoai = txtSDTKH.Text.Trim();
            diaChi = txtDiaChiKH.Text.Trim();
            diemTichLuy = 0;
            maNhanVien = 0;

            if (string.IsNullOrWhiteSpace(hoTen))
            {
                MessageBox.Show("Tên khách hàng không được để trống.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtTenKH.Focus();
                return false;
            }

            if (!int.TryParse(txtDiem.Text.Trim(), out diemTichLuy))
            {
                MessageBox.Show("Điểm tích lũy phải là số nguyên.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtDiem.Focus();
                return false;
            }

            if (diemTichLuy < 0)
            {
                MessageBox.Show("Điểm tích lũy không được âm.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtDiem.Focus();
                return false;
            }

            if (!TryValidatePhoneNumber(currentClientId: null, showMessage: true, out soDienThoai))
            {
                return false;
            }
            if (string.IsNullOrWhiteSpace(diaChi))
            {
                MessageBox.Show("Địa chỉ khách hàng không được để trống.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtDiaChiKH.Focus();
                return false;
            }

            if (cboMaNV.SelectedValue is int selectedEmployee)
            {
                maNhanVien = selectedEmployee;
            }
            else
            {
                MessageBox.Show("Vui lòng chọn mã nhân viên hợp lệ.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cboMaNV.Focus();
                return false;
            }

            return true;
        }

        private bool TryValidateInputsForEdit(int maKhachHang, out string hoTen, out string soDienThoai, out string diaChi, out int maNhanVien)
        {
            hoTen = txtTenKH.Text.Trim();
            soDienThoai = txtSDTKH.Text.Trim();
            diaChi = txtDiaChiKH.Text.Trim();
            maNhanVien = 0;

            if (string.IsNullOrWhiteSpace(hoTen))
            {
                MessageBox.Show("Tên khách hàng không được để trống.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtTenKH.Focus();
                return false;
            }
            if (!TryValidatePhoneNumber(maKhachHang, showMessage: true, out soDienThoai))
            {
                return false;
            }
            if (string.IsNullOrWhiteSpace(diaChi))
            {
                MessageBox.Show("Địa chỉ khách hàng không được để trống.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtDiaChiKH.Focus();
                return false;
            }
            if (cboMaNV.SelectedValue is int selectedEmployee)
            {
                maNhanVien = selectedEmployee;
            }
            else
            {
                MessageBox.Show("Vui lòng chọn mã nhân viên hợp lệ.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cboMaNV.Focus();
                return false;
            }

            return true;
        }

        private void LoadEmployeeOptions()
        {
            try
            {
                gStoreDataSet.NguoiDung.Clear();
                gStoreDataSet.NhanVien.Clear();

                _nguoiDungTableAdapter.ClearBeforeFill = true;
                _nguoiDungTableAdapter.Fill(gStoreDataSet.NguoiDung);

                _nhanVienTableAdapter.ClearBeforeFill = true;
                _nhanVienTableAdapter.Fill(gStoreDataSet.NhanVien);

                var employees = new List<KeyValuePair<int, string>>();

                if (!string.IsNullOrWhiteSpace(_username))
                {
                    var userRow = gStoreDataSet.NguoiDung
                        .FirstOrDefault(row => !row.IsNull(gStoreDataSet.NguoiDung.TenDangNhapColumn)
                                              && string.Equals(row.TenDangNhap, _username, StringComparison.OrdinalIgnoreCase));

                    if (userRow != null)
                    {
                        employees = gStoreDataSet.NhanVien
                            .Where(row => !row.IsMaNguoiDungNull() && row.MaNguoiDung == userRow.MaNguoiDung)
                            .Select(row => new KeyValuePair<int, string>(row.MaNhanVien, $"{row.MaNhanVien} - {row.HoTen}"))
                            .OrderBy(item => item.Key)
                            .ToList();
                    }
                }

                cboMaNV.DisplayMember = "Value";
                cboMaNV.ValueMember = "Key";
                cboMaNV.DropDownStyle = ComboBoxStyle.DropDownList;
                cboMaNV.DataSource = employees;
                cboMaNV.SelectedIndex = employees.Count > 0 ? 0 : -1;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Không thể tải danh sách nhân viên.\n{ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                cboMaNV.DataSource = new List<KeyValuePair<int, string>>();
                cboMaNV.SelectedIndex = -1;
            }
        }

        private void SetEmployeeSelection(int? maNhanVien)
        {
            if (cboMaNV.DataSource == null)
            {
                cboMaNV.SelectedIndex = -1;
                return;
            }

            if (maNhanVien.HasValue)
            {
                cboMaNV.SelectedValue = maNhanVien.Value;
                if (cboMaNV.SelectedIndex < 0)
                {
                    cboMaNV.SelectedIndex = cboMaNV.Items.Count > 0 ? 0 : -1;
                }
            }
            else
            {
                cboMaNV.SelectedIndex = cboMaNV.Items.Count > 0 ? 0 : -1;
            }
        }





        private void RefreshData(int? focusKey = null)
        {
            RefreshDataAndRestoreByKey(focusKey);
        }

        private void btnThem_Click(object sender, EventArgs e)
        {
            CommitUI();

            if (_currentOperation != ClientOperation.Add)
            {
                if (!AskConfirm("Bạn muốn thêm khách hàng mới?")) return;

                SetOperation(ClientOperation.Add);
                ClearInputFields();
                txtTenKH.Focus();
                return;
            }

            // Pha lưu
            if (!TryValidateInputsForAdd(out string hoTen, out string soDienThoai, out string diaChi, out int diemTichLuy, out int maNhanVien))
                return;

            if (!AskConfirm("Xác nhận lưu khách hàng mới?")) return;

            try
            {
                khachHangTableAdapter.Insert(
                    hoTen,
                    soDienThoai,
                    diaChi,
                    diemTichLuy,
                    maNhanVien
                );

                // Sau Insert: Fill + focus về cuối (hoặc nếu biết ID mới, truyền vào RefreshData(idMoi))
                RefreshData(focusKey: null);
                khachHangBindingSource.Position = Math.Max(0, khachHangBindingSource.Count - 1);

                MessageBox.Show("Thêm khách hàng thành công.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Không thể thêm khách hàng. Lỗi: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                SetOperation(ClientOperation.None);
            }

        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            CommitUI();

            var row = GetCurrentClientRow();
            if (row == null)
            {
                MessageBox.Show("Vui lòng chọn khách hàng cần sửa.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (_currentOperation != ClientOperation.Edit)
            {
                if (!AskConfirm("Bạn muốn sửa thông tin khách hàng này?")) return;

                SetOperation(ClientOperation.Edit);
                PopulateInputs(row);
                txtTenKH.Focus();
                return;
            }

            // Pha lưu
            if (!int.TryParse(txtMaKH.Text, out int maKhachHang))
            {
                MessageBox.Show("Không xác định được khách hàng cần sửa.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!TryValidateInputsForEdit(maKhachHang, out string hoTen, out string soDienThoai, out string diaChi, out int maNhanVien))
                return;

            if (!AskConfirm("Xác nhận lưu thay đổi thông tin khách hàng?")) return;

            var editRow = gStoreDataSet.KhachHang.FindByMaKhachHang(maKhachHang);
            if (editRow == null)
            {
                MessageBox.Show("Không tìm thấy khách hàng cần sửa.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                editRow.HoTen = hoTen;
                editRow.SoDienThoai = soDienThoai;
                editRow.DiaChi = diaChi;
                editRow.MaNhanVien = maNhanVien;

                khachHangBindingSource.EndEdit();
                khachHangTableAdapter.Update(editRow);

                // KHÔNG Fill để tránh reset con trỏ; chỉ cập nhật hiển thị:
                khachHangBindingSource.ResetCurrentItem();

                MessageBox.Show("Cập nhật khách hàng thành công.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Không thể cập nhật khách hàng. Lỗi: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                SetOperation(ClientOperation.None);
            }
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            CommitUI();

            var row = GetCurrentClientRow();
            if (row == null)
            {
                MessageBox.Show("Vui lòng chọn khách hàng cần xóa.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (!AskConfirm("Bạn có chắc chắn muốn xóa khách hàng này?", MessageBoxIcon.Warning)) return;

            // Sau xóa: đưa con trỏ về bản ghi trước
            int desiredPos = Math.Max(0, khachHangBindingSource.Position - 1);

            try
            {
                row.Delete();
                khachHangBindingSource.EndEdit();
                khachHangTableAdapter.Update(gStoreDataSet.KhachHang);

                RefreshData(focusKey: null);
                if (khachHangBindingSource.Count > 0)
                    khachHangBindingSource.Position = Math.Min(desiredPos, khachHangBindingSource.Count - 1);

                MessageBox.Show("Xóa khách hàng thành công.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Không thể xóa khách hàng. Lỗi: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool TryValidatePhoneNumber(int? currentClientId, bool showMessage, out string sanitizedNumber)
        {
            sanitizedNumber = txtSDTKH.Text.Trim();

            if (string.IsNullOrWhiteSpace(sanitizedNumber))
            {
                if (showMessage)
                    MessageBox.Show("Vui lòng nhập số điện thoại.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtSDTKH.Focus();
                return false;
            }

            if (!sanitizedNumber.All(char.IsDigit))
            {
                if (showMessage)
                    MessageBox.Show("Số điện thoại chỉ được chứa số.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtSDTKH.Focus();
                return false;
            }

            if (sanitizedNumber.Length < 10)
            {
                if (showMessage)
                    MessageBox.Show("Số điện thoại phải đủ 10 số.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtSDTKH.Focus();
                return false;
            }

            if (sanitizedNumber.Length > 10)
            {
                if (showMessage)
                    MessageBox.Show("Số điện thoại không được vượt quá 10 số.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtSDTKH.Focus();
                return false;
            }
            var phoneToCheck = sanitizedNumber;
            bool isDuplicate = gStoreDataSet.KhachHang.Any(row =>
                !row.IsSoDienThoaiNull() &&
                 string.Equals(row.SoDienThoai, phoneToCheck, StringComparison.Ordinal) &&
                (!currentClientId.HasValue || row.MaKhachHang != currentClientId.Value));

            if (isDuplicate)
            {
                if (showMessage)
                    MessageBox.Show("Số điện thoại đã tồn tại.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtSDTKH.Focus();
                return false;
            }

            return true;
        }



        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            if (_currentOperation == ClientOperation.Add)
            {
                return;
            }

            PopulateInputsFromSelection();
        }

        private void btnHuy_Click(object sender, EventArgs e)
        {
            if (_currentOperation == ClientOperation.None)
            {
                PopulateInputsFromSelection();
                return;
            }

            if (!AskConfirm("Hủy các thay đổi khách hàng đang thực hiện?")) return;

            // Lưu lại chế độ AutoValidate hiện tại và tắt tạm thời
            var prev = this.AutoValidate;
            this.AutoValidate = AutoValidate.Disable;
            try
            {
                int? focusKey = null;
                if (int.TryParse(txtMaKH.Text.Trim(), out int parsedId))
                    focusKey = parsedId;

                // KHÔNG gọi Validate() ở đây nữa
                try { dataGridView1.CancelEdit(); } catch { }
                try { khachHangBindingSource.CancelEdit(); } catch { }
                try { gStoreDataSet.KhachHang.RejectChanges(); } catch { }

                SetOperation(ClientOperation.None);
                RefreshDataAndRestoreByKey(focusKey);
            }
            finally
            {
                // khôi phục
                this.AutoValidate = prev;
            }
        }

        private void txtSDTKH_Validating(object sender, CancelEventArgs e)
        {
            int? currentClientId = null;
            if (int.TryParse(txtMaKH.Text, out int parsedId))
            {
                currentClientId = parsedId;
            }

            if (!TryValidatePhoneNumber(currentClientId, showMessage: true, out _))
            {
                e.Cancel = true;
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            string phoneNumber = txtSearch.Text.Trim();

            if (string.IsNullOrWhiteSpace(phoneNumber))
            {
                MessageBox.Show("Vui lòng nhập số điện thoại để tìm kiếm.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (gStoreDataSet.KhachHang.Count == 0)
            {
                try
                {
                    khachHangTableAdapter.Fill(gStoreDataSet.KhachHang);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Không thể tải dữ liệu khách hàng. Lỗi: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            var matchedRow = gStoreDataSet.KhachHang
                .FirstOrDefault(row => !row.IsSoDienThoaiNull() && string.Equals(row.SoDienThoai, phoneNumber, StringComparison.Ordinal));

            if (matchedRow != null)
            {
                int position = gStoreDataSet.KhachHang.Rows.IndexOf(matchedRow);
                if (position >= 0)
                {
                    khachHangBindingSource.Position = position;
                    PopulateInputs(matchedRow);

                    if (dataGridView1.Rows.Count > position)
                    {
                        dataGridView1.ClearSelection();
                        dataGridView1.Rows[position].Selected = true;
                        dataGridView1.CurrentCell = dataGridView1.Rows[position].Cells[0];
                    }
                }
            }
            else
            {
                MessageBox.Show("Số điện thoại không tồn tại", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }
}
