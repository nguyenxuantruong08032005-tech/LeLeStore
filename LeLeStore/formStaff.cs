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
    public partial class formStaff : Form
    {
        private enum OperationMode
        {
            None,
            Add,
            Edit,
            Delete
        }

        private class NhanVienSnapshot
        {
            public string HoTen { get; set; }
            public string ChucVu { get; set; }
            public string SoDienThoai { get; set; }
            public string DiaChi { get; set; }
            public int? MaNguoiDung { get; set; }
        }

        private OperationMode currentMode = OperationMode.None;
        private GStoreDataSet.NhanVienRow currentRow;
        private NhanVienSnapshot originalValues;
        public formStaff()
        {
            InitializeComponent();
          

            txtMaNV.ReadOnly = true;
            SetTextBoxesReadOnly(true);
        }

        private void formStaff_Load(object sender, EventArgs e)
        {
            // TODO: This line of code loads data into the 'gStoreDataSet.NhanVien' table. You can move, or remove it, as needed.
            this.nhanVienTableAdapter.Fill(this.gStoreDataSet.NhanVien);
            LoadRowFromCurrentSelection();

        }

        private void btnThem_Click(object sender, EventArgs e)
        {
            currentMode = OperationMode.Add;
            currentRow = null;
            originalValues = null;

            SetTextBoxesReadOnly(false);
            ClearTextBoxes();
            txtTenNV.Focus();
        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            var row = GetSelectedRow();
            if (row == null)
            {
                MessageBox.Show("Vui lòng chọn nhân viên cần sửa.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            currentMode = OperationMode.Edit;
            currentRow = row;
            originalValues = CreateSnapshot(row);

            SetTextBoxesReadOnly(false);
            PopulateTextBoxes(row);
            txtTenNV.Focus();
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            var row = GetSelectedRow();
            if (row == null)
            {
                MessageBox.Show("Vui lòng chọn nhân viên cần xóa.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            currentMode = OperationMode.Delete;
            currentRow = row;
            originalValues = null;

            SetTextBoxesReadOnly(true);
            PopulateTextBoxes(row);
        }

        private void btnLuu_Click(object sender, EventArgs e)
        {
            switch (currentMode)
            {
                case OperationMode.Add:
                    SaveNewNhanVien();
                    break;
                case OperationMode.Edit:
                    SaveEditedNhanVien();
                    break;
                case OperationMode.Delete:
                    DeleteNhanVien();
                    break;
                default:
                    MessageBox.Show("Vui lòng chọn chức năng Thêm, Sửa hoặc Xóa trước khi lưu.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    break;
            }
        }

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            if (currentMode != OperationMode.None)
            {
                return;
            }

            LoadRowFromCurrentSelection();
        }
        private void LoadRowFromCurrentSelection()
        {
            var row = GetSelectedRow();
            currentRow = row;

            if (row != null)
            {
                PopulateTextBoxes(row);
            }
            else
            {
                ClearTextBoxes();
            }
        }

        private GStoreDataSet.NhanVienRow GetSelectedRow()
        {
            if (dataGridView1.CurrentRow == null)
            {
                return null;
            }

            var rowView = dataGridView1.CurrentRow.DataBoundItem as DataRowView;
            return rowView?.Row as GStoreDataSet.NhanVienRow;
        }
        private void PopulateTextBoxes(GStoreDataSet.NhanVienRow row)
        {
            
            txtTenNV.Text = row.HoTen;
            txtCV.Text = row.IsChucVuNull() ? string.Empty : row.ChucVu;
            txtSDT.Text = row.IsSoDienThoaiNull() ? string.Empty : row.SoDienThoai;
            txtDC.Text = row.IsDiaChiNull() ? string.Empty : row.DiaChi;
            txtMaND.Text = row.IsMaNguoiDungNull() ? string.Empty : row.MaNguoiDung.ToString();
        }

        private void ClearTextBoxes()
        {
            txtMaNV.Text = string.Empty;
            txtTenNV.Text = string.Empty;
            txtCV.Text = string.Empty;
            txtSDT.Text = string.Empty;
            txtDC.Text = string.Empty;
            txtMaND.Text = string.Empty;
        }
        private void SetTextBoxesReadOnly(bool readOnly)
        {
            txtTenNV.ReadOnly = readOnly;
            txtCV.ReadOnly = readOnly;
            txtSDT.ReadOnly = readOnly;
            txtDC.ReadOnly = readOnly;
            txtMaND.ReadOnly = readOnly;
        }

        private NhanVienSnapshot CreateSnapshot(GStoreDataSet.NhanVienRow row)
        {
            return new NhanVienSnapshot
            {
                HoTen = row.HoTen,
                ChucVu = row.IsChucVuNull() ? null : row.ChucVu,
                SoDienThoai = row.IsSoDienThoaiNull() ? null : row.SoDienThoai,
                DiaChi = row.IsDiaChiNull() ? null : row.DiaChi,
                MaNguoiDung = row.IsMaNguoiDungNull() ? (int?)null : row.MaNguoiDung
            };
        }
        private void SaveNewNhanVien()
        {
            string hoTen = NormalizeRequiredText(txtTenNV.Text);
            if (string.IsNullOrEmpty(hoTen))
            {
                MessageBox.Show("Họ tên không được để trống.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!TryParseMaNguoiDung(out int? maNguoiDung))
            {
                return;
            }

            string chucVu = NormalizeOptionalText(txtCV.Text);
            string soDienThoai = NormalizeOptionalText(txtSDT.Text);
            string diaChi = NormalizeOptionalText(txtDC.Text);

            var newRow = gStoreDataSet.NhanVien.NewNhanVienRow();
            newRow.HoTen = hoTen;
            if (string.IsNullOrEmpty(chucVu))
            {
                newRow.SetChucVuNull();
            }
            else
            {
                newRow.ChucVu = chucVu;
            }

            if (string.IsNullOrEmpty(soDienThoai))
            {
                newRow.SetSoDienThoaiNull();
            }
            else
            {
                newRow.SoDienThoai = soDienThoai;
            }

            if (string.IsNullOrEmpty(diaChi))
            {
                newRow.SetDiaChiNull();
            }
            else
            {
                newRow.DiaChi = diaChi;
            }

            if (maNguoiDung.HasValue)
            {
                newRow.MaNguoiDung = maNguoiDung.Value;
            }
            else
            {
                newRow.SetMaNguoiDungNull();
            }

            gStoreDataSet.NhanVien.AddNhanVienRow(newRow);
            currentRow = newRow;

            CommitChanges(() => newRow.MaNhanVien);
            MessageBox.Show("Đã thêm nhân viên thành công.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        private void SaveEditedNhanVien()
        {
            if (currentRow == null)
            {
                MessageBox.Show("Không có nhân viên được chọn để sửa.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string hoTen = NormalizeRequiredText(txtTenNV.Text);
            if (string.IsNullOrEmpty(hoTen))
            {
                MessageBox.Show("Họ tên không được để trống.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!TryParseMaNguoiDung(out int? maNguoiDung))
            {
                return;
            }

            string chucVu = NormalizeOptionalText(txtCV.Text);
            string soDienThoai = NormalizeOptionalText(txtSDT.Text);
            string diaChi = NormalizeOptionalText(txtDC.Text);

            string originalHoTen = NormalizeRequiredText(originalValues != null ? originalValues.HoTen : null);
            string originalChucVu = NormalizeOptionalText(originalValues != null ? originalValues.ChucVu : null);
            string originalSoDienThoai = NormalizeOptionalText(originalValues != null ? originalValues.SoDienThoai : null);
            string originalDiaChi = NormalizeOptionalText(originalValues != null ? originalValues.DiaChi : null);
            int? originalMaNguoiDung = originalValues != null ? originalValues.MaNguoiDung : null;

            bool hasChanges = false;
            if (!string.Equals(hoTen, originalHoTen, StringComparison.Ordinal))
            {
                hasChanges = true;
            }
            else if (!string.Equals(chucVu, originalChucVu, StringComparison.Ordinal))
            {
                hasChanges = true;
            }
            else if (!string.Equals(soDienThoai, originalSoDienThoai, StringComparison.Ordinal))
            {
                hasChanges = true;
            }
            else if (!string.Equals(diaChi, originalDiaChi, StringComparison.Ordinal))
            {
                hasChanges = true;
            }
            else if (maNguoiDung != originalMaNguoiDung)
            {
                hasChanges = true;
            }

            if (!hasChanges)
            {
                MessageBox.Show("Không có thay đổi.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            currentRow.HoTen = hoTen;

            if (string.IsNullOrEmpty(chucVu))
            {
                currentRow.SetChucVuNull();
            }
            else
            {
                currentRow.ChucVu = chucVu;
            }

            if (string.IsNullOrEmpty(soDienThoai))
            {
                currentRow.SetSoDienThoaiNull();
            }
            else
            {
                currentRow.SoDienThoai = soDienThoai;
            }

            if (string.IsNullOrEmpty(diaChi))
            {
                currentRow.SetDiaChiNull();
            }
            else
            {
                currentRow.DiaChi = diaChi;
            }

            if (maNguoiDung.HasValue)
            {
                currentRow.MaNguoiDung = maNguoiDung.Value;
            }
            else
            {
                currentRow.SetMaNguoiDungNull();
            }

            CommitChanges(() => currentRow.MaNhanVien);
            MessageBox.Show("Đã cập nhật thông tin nhân viên.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        private void DeleteNhanVien()
        {
            if (currentRow == null)
            {
                MessageBox.Show("Không có nhân viên được chọn để xóa.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var dialogResult = MessageBox.Show("Bạn có chắc chắn muốn xóa nhân viên này không?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dialogResult != DialogResult.Yes)
            {
                return;
            }

            int? nextSelectionId = null;
            if (dataGridView1.CurrentRow != null && dataGridView1.CurrentRow.Index > 0)
            {
                var previousRow = dataGridView1.Rows[dataGridView1.CurrentRow.Index - 1];
                var rowView = previousRow.DataBoundItem as DataRowView;
                if (rowView != null)
                {
                    var nvRow = rowView.Row as GStoreDataSet.NhanVienRow;
                    if (nvRow != null)
                    {
                        nextSelectionId = nvRow.MaNhanVien;
                    }
                }
            }

            currentRow.Delete();
            CommitChanges(() => nextSelectionId);
            MessageBox.Show("Đã xóa nhân viên.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void CommitChanges(Func<int?> selectIdProvider)
        {
            try
            {
                Validate();
                nhanVienBindingSource.EndEdit();
                dataGridView1.EndEdit();
                nhanVienTableAdapter.Update(gStoreDataSet.NhanVien);
                int? selectId = selectIdProvider != null ? selectIdProvider() : (int?)null;
                ReloadData(selectId);
                ResetState();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không thể lưu dữ liệu. Chi tiết: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void ReloadData(int? selectId)
        {
            nhanVienTableAdapter.Fill(gStoreDataSet.NhanVien);

            if (selectId.HasValue)
            {
                bool selected = false;
                foreach (DataGridViewRow gridRow in dataGridView1.Rows)
                {
                    var rowView = gridRow.DataBoundItem as DataRowView;
                    if (rowView == null)
                    {
                        continue;
                    }

                    var nvRow = rowView.Row as GStoreDataSet.NhanVienRow;
                    if (nvRow != null && nvRow.MaNhanVien == selectId.Value)
                    {
                        dataGridView1.ClearSelection();
                        gridRow.Selected = true;
                        dataGridView1.CurrentCell = gridRow.Cells[0];
                        currentRow = nvRow;
                        PopulateTextBoxes(nvRow);
                        selected = true;
                        break;
                    }
                }

                if (!selected)
                {
                    LoadRowFromCurrentSelection();
                }
            }
            else
            {
                LoadRowFromCurrentSelection();
            }
        }

        private void ResetState()
        {
            currentMode = OperationMode.None;
            originalValues = null;
            SetTextBoxesReadOnly(true);
        }
        private string NormalizeOptionalText(string value)
        {
            return string.IsNullOrWhiteSpace(value) ? string.Empty : value.Trim();
        }

        private string NormalizeRequiredText(string value)
        {
            return string.IsNullOrWhiteSpace(value) ? string.Empty : value.Trim();
        }

        private bool TryParseMaNguoiDung(out int? maNguoiDung)
        {
            string value = txtMaND.Text.Trim();
            if (string.IsNullOrEmpty(value))
            {
                maNguoiDung = null;
                return true;
            }

            int parsed;
            if (int.TryParse(value, out parsed))
            {
                maNguoiDung = parsed;
                return true;
            }

            MessageBox.Show("Mã người dùng phải là số.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            maNguoiDung = null;
            return false;
        }
    }
}
