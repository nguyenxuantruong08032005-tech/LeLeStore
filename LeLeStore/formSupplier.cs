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
    public partial class formSupplier : Form
    {
        private enum SupplierOperation
        {
            None,
            Add,
            Edit,
            Delete
        }
        private readonly string _username;
        private readonly GStoreDataSetTableAdapters.NhanVienTableAdapter _nhanVienTableAdapter = new GStoreDataSetTableAdapters.NhanVienTableAdapter();
        private readonly GStoreDataSetTableAdapters.NguoiDungTableAdapter _nguoiDungTableAdapter = new GStoreDataSetTableAdapters.NguoiDungTableAdapter();
        private SupplierOperation _currentOperation = SupplierOperation.None;
        public formSupplier(string username = "")
        {
            _username = username ?? string.Empty;
            InitializeComponent();
            SetOperation(SupplierOperation.None);
            PopulateInputsFromSelection();
            LoadEmployeeOptions();
        }

        private void formSupplier_Load(object sender, EventArgs e)
        {
            // TODO: This line of code loads data into the 'gStoreDataSet.NhaCungCap' table. You can move, or remove it, as needed.
            this.nhaCungCapTableAdapter.Fill(this.gStoreDataSet.NhaCungCap);

        }
        private void SetOperation(SupplierOperation operation)
        {
            _currentOperation = operation;

            bool canEditFields = operation == SupplierOperation.Add || operation == SupplierOperation.Edit;
            bool isDelete = operation == SupplierOperation.Delete;

            txtMaNCC.ReadOnly = true;
            txtTenNCC.ReadOnly = !canEditFields;
            txtSdtNCC.ReadOnly = !canEditFields;
            txtDiaChiNCC.ReadOnly = !canEditFields;
            cboMaNV.Enabled = canEditFields;

            if (isDelete)
            {
                txtTenNCC.ReadOnly = true;
                txtSdtNCC.ReadOnly = true;
                txtDiaChiNCC.ReadOnly = true;
                cboMaNV.Enabled = false;
            }
            // 👉 Hủy chỉ bật khi đang Add/Edit/Delete (khác None)
            btnHuy.Enabled = (operation != SupplierOperation.None);
        }
        private void CancelPendingEdits()
        {
            // Hủy edit ở UI/BindingSource
            try { this.Validate(); } catch { }
            try { dataGridView1.CancelEdit(); } catch { }
            try { nhaCungCapBindingSource.CancelEdit(); } catch { }

            // Hủy thay đổi treo ở DataSet (chỉ bảng NhaCungCap)
            try { gStoreDataSet.NhaCungCap.RejectChanges(); } catch { }
        }

        private void ReloadInputs()
        {
            // Đồng bộ lại textbox theo dòng đang chọn
            PopulateInputsFromSelection();
            try { nhaCungCapBindingSource.ResetCurrentItem(); } catch { }
        }

        private void PopulateInputsFromSelection()
        {
            var row = GetCurrentSupplierRow();
            if (row != null)
            {
                PopulateInputs(row);
            }
            else
            {
                ClearInputFields();
            }
        }

        private void PopulateInputs(GStoreDataSet.NhaCungCapRow row)
        {
            txtMaNCC.Text = row.MaNCC.ToString();
            txtTenNCC.Text = row.IsNull("TenNCC") ? string.Empty : row.TenNCC;

            txtSdtNCC.Text = row.IsNull("SoDienThoai") ? string.Empty : row.SoDienThoai;
            txtDiaChiNCC.Text = row.IsNull("DiaChi") ? string.Empty : row.DiaChi;
            SetEmployeeSelection(row.IsMaNhanVienNull() ? (int?)null : row.MaNhanVien);
        }

        private void ClearInputFields()
        {
            txtMaNCC.Text = string.Empty;
            txtTenNCC.Text = string.Empty;
            txtSdtNCC.Text = string.Empty;
            txtDiaChiNCC.Text = string.Empty;
            SetEmployeeSelection(null);
        }

        private GStoreDataSet.NhaCungCapRow GetCurrentSupplierRow()
        {
            if (dataGridView1.CurrentRow?.DataBoundItem is DataRowView rowView)
            {
                return rowView.Row as GStoreDataSet.NhaCungCapRow;
            }

            return null;
        }

        private bool TryValidateInputs(out string tenNcc, out string soDienThoai, out string diaChi, out int maNhanVien)
        {
            tenNcc = txtTenNCC.Text.Trim();
            soDienThoai = txtSdtNCC.Text.Trim();
            diaChi = txtDiaChiNCC.Text.Trim();
            maNhanVien = 0;

            if (string.IsNullOrWhiteSpace(tenNcc))
            {
                MessageBox.Show("Tên nhà cung cấp không được để trống.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtTenNCC.Focus();
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

        
        

        private void RefreshData(int? focusKey = null)
        {
            RefreshDataAndRestoreByKey(focusKey);
        }


        private void btnThem_Click(object sender, EventArgs e)
        {
            CommitUI();

            if (_currentOperation != SupplierOperation.Add)
            {
                // XÁC NHẬN BƯỚC VÀO CHẾ ĐỘ THÊM
                if (!AskConfirm("Bạn muốn thêm nhà cung cấp mới?")) return;

                SetOperation(SupplierOperation.Add);
                ClearInputFields();
                txtMaNCC.Text = string.Empty;
                txtTenNCC.Focus();

                btnSua.Enabled = false;
                btnXoa.Enabled = false;
                btnThem.Text = "Lưu";
                return;
            }

            // Đang ở chế độ Thêm -> Lưu
            if (!TryValidateInputs(out string tenNcc, out string soDienThoai, out string diaChi, out int maNhanVien))
                return;

            // XÁC NHẬN TRƯỚC KHI LƯU
            if (!AskConfirm("Xác nhận thêm nhà cung cấp này?")) return;

            try
            {
                nhaCungCapTableAdapter.Insert(
                    tenNcc,
                    string.IsNullOrWhiteSpace(soDienThoai) ? null : soDienThoai,
                    string.IsNullOrWhiteSpace(diaChi) ? null : diaChi,
                    maNhanVien
                );

                RefreshData(focusKey: null);
                nhaCungCapBindingSource.Position = nhaCungCapBindingSource.Count - 1;

                MessageBox.Show("Thêm nhà cung cấp thành công.", "Thông báo",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Không thể thêm nhà cung cấp. Lỗi: {ex.Message}", "Lỗi",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                SetOperation(SupplierOperation.None);
                btnSua.Enabled = true;
                btnXoa.Enabled = true;
                btnThem.Text = "Thêm";
            }
        }

        private void btnSua_Click(object sender, EventArgs e)
        {

            CommitUI();

            var row = GetCurrentRow();
            if (row == null)
            {
                MessageBox.Show("Vui lòng chọn nhà cung cấp cần sửa.", "Thông báo",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (_currentOperation != SupplierOperation.Edit)
            {
                // XÁC NHẬN BƯỚC VÀO CHẾ ĐỘ SỬA
                if (!AskConfirm("Bạn muốn sửa thông tin nhà cung cấp này?")) return;

                SetOperation(SupplierOperation.Edit);
                PopulateInputs(row);

                btnThem.Enabled = false;
                btnXoa.Enabled = false;
                btnSua.Text = "Lưu";
                return;
            }

            // Đang ở chế độ Sửa -> Lưu
            if (!int.TryParse(txtMaNCC.Text, out int maNcc))
            {
                MessageBox.Show("Không xác định được nhà cung cấp cần sửa.", "Lỗi",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!TryValidateInputs(out string tenNcc, out string soDienThoai, out string diaChi, out int maNhanVien))
                return;

            // XÁC NHẬN TRƯỚC KHI LƯU
            if (!AskConfirm("Xác nhận lưu thay đổi thông tin nhà cung cấp?")) return;

            var editRow = gStoreDataSet.NhaCungCap.FindByMaNCC(maNcc);
            if (editRow == null)
            {
                MessageBox.Show("Không tìm thấy nhà cung cấp cần sửa.", "Lỗi",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                editRow.TenNCC = tenNcc;
                if (string.IsNullOrWhiteSpace(soDienThoai)) editRow.SetSoDienThoaiNull(); else editRow.SoDienThoai = soDienThoai;
                if (string.IsNullOrWhiteSpace(diaChi)) editRow.SetDiaChiNull(); else editRow.DiaChi = diaChi;
                editRow.MaNhanVien = maNhanVien;

                nhaCungCapBindingSource.EndEdit();
                nhaCungCapTableAdapter.Update(editRow);

                // Không Fill để tránh nhảy con trỏ
                nhaCungCapBindingSource.ResetCurrentItem();

                MessageBox.Show("Cập nhật nhà cung cấp thành công.", "Thông báo",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Không thể cập nhật nhà cung cấp. Lỗi: {ex.Message}", "Lỗi",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                SetOperation(SupplierOperation.None);
                btnThem.Enabled = true;
                btnXoa.Enabled = true;
                btnSua.Text = "Sửa";
            }
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            CommitUI();

            var row = GetCurrentRow();
            if (row == null)
            {
                MessageBox.Show("Vui lòng chọn nhà cung cấp cần xóa.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var confirm = MessageBox.Show("Bạn có chắc chắn muốn xóa nhà cung cấp này?", "Xác nhận",
                                          MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirm != DialogResult.Yes) return;

            int key = row.MaNCC;
            // Sau khi xóa, sẽ đưa con trỏ về bản ghi trước đó
            int desiredPos = Math.Max(0, nhaCungCapBindingSource.Position - 1);

            try
            {
                row.Delete();
                nhaCungCapBindingSource.EndEdit();
                nhaCungCapTableAdapter.Update(gStoreDataSet.NhaCungCap);

                // Fill lại để đồng bộ với DB rồi đưa con trỏ tới vị trí mong muốn
                RefreshData(focusKey: null);
                if (nhaCungCapBindingSource.Count > 0)
                    nhaCungCapBindingSource.Position = Math.Min(desiredPos, nhaCungCapBindingSource.Count - 1);

                MessageBox.Show("Xóa nhà cung cấp thành công.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Không thể xóa nhà cung cấp. Lỗi: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void CommitUI()
        {
            this.Validate();
            try { dataGridView1.EndEdit(); } catch { /* Đảm bảo đúng tên DataGridView */ }
            nhaCungCapBindingSource.EndEdit();
        }

        private GStoreDataSet.NhaCungCapRow GetCurrentRow()
        {
            CommitUI();
            if (dataGridView1.CurrentRow?.DataBoundItem is DataRowView rv)
                return rv.Row as GStoreDataSet.NhaCungCapRow;
            return null;
        }

        private void RefreshDataAndRestoreByKey(int? keyToFocus)
        {
            // Fill lại nhưng không làm mất vị trí bản ghi mong muốn
            gStoreDataSet.NhaCungCap.Clear();
            nhaCungCapTableAdapter.Fill(gStoreDataSet.NhaCungCap);

            if (keyToFocus.HasValue)
            {
                var rows = gStoreDataSet.NhaCungCap.Select($"MaNCC = {keyToFocus.Value}");
                if (rows.Length > 0)
                {
                    var row = rows[0];
                    int pos = gStoreDataSet.NhaCungCap.Rows.IndexOf(row);
                    if (pos >= 0) nhaCungCapBindingSource.Position = pos;
                }
            }

            PopulateInputsFromSelection();
        }
        private bool AskConfirm(string message)
        {
            return MessageBox.Show(message, "Xác nhận",
                                   MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes;
        }

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            if (_currentOperation == SupplierOperation.Add)
            {
                return;
            }

            PopulateInputsFromSelection();
        }

        private void btnHuy_Click(object sender, EventArgs e)
        {
            // Nếu không ở chế độ thao tác thì chỉ làm tươi lại dữ liệu đang hiển thị
            if (_currentOperation == SupplierOperation.None)
            {
                ReloadInputs();
                return;
            }

            var confirm = MessageBox.Show("Hủy các thay đổi đang thực hiện?",
                                          "Xác nhận",
                                          MessageBoxButtons.YesNo,
                                          MessageBoxIcon.Question);
            if (confirm != DialogResult.Yes) return;

            // Rollback mọi chỉnh sửa chưa lưu
            CancelPendingEdits();

            // Trả UI về trạng thái bình thường
            SetOperation(SupplierOperation.None);

            // Mở lại các nút và nhãn nút như cũ
            btnThem.Enabled = true; btnThem.Text = "Thêm";
            btnSua.Enabled = true; btnSua.Text = "Sửa";
            btnXoa.Enabled = true;

            // Làm tươi khu nhập theo selection hiện tại
            ReloadInputs();
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

                ConfigureEmployeeCombo(employees);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Không thể tải danh sách nhân viên.\n{ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                ConfigureEmployeeCombo(new List<KeyValuePair<int, string>>());
            }
        }

        private void ConfigureEmployeeCombo(IList<KeyValuePair<int, string>> employees)
        {
            cboMaNV.DisplayMember = "Value";
            cboMaNV.ValueMember = "Key";
            cboMaNV.DropDownStyle = ComboBoxStyle.DropDownList;
            cboMaNV.DataSource = employees;
            cboMaNV.SelectedIndex = employees.Count > 0 ? 0 : -1;
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
    }
}
