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
      

     
        private GStoreDataSet.NhanVienRow currentRow;
     

        public formStaff()
        {
            InitializeComponent();

            

            dataGridView1.SelectionChanged += dataGridView1_SelectionChanged;
            txtMaNV.ReadOnly = true;
            SetTextBoxesReadOnly(false);
           
        }

        private void formStaff_Load(object sender, EventArgs e)
        {
            // TODO: This line of code loads data into the 'gStoreDataSet.NhanVien' table. You can move, or remove it, as needed.
            this.nhanVienTableAdapter.Fill(this.gStoreDataSet.NhanVien);
            LoadRowFromCurrentSelection();

        }
        // ===================== HELPER =====================
     
        

        private void ShowSuccess(string message)
        {
            MessageBox.Show(message, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        // Trạng thái 2-bước
        private enum OperationMode { None, Add, Edit, Delete }
        private OperationMode pendingMode = OperationMode.None;
        private bool awaitingConfirmation = false;

        private void ArmTwoStep(OperationMode mode)
        {
            pendingMode = mode;
            awaitingConfirmation = true;
        }
        private void ResetTwoStep()
        {
            pendingMode = OperationMode.None;
            awaitingConfirmation = false;
        }

        // Confirm OK/Cancel
        private bool Confirm(string message, MessageBoxIcon icon = MessageBoxIcon.Question)
        {
            return MessageBox.Show(message, "Xác nhận", MessageBoxButtons.OKCancel, icon) == DialogResult.OK;
        }

        // Thông báo kết quả
        private void NotifySaved()
        {
            MessageBox.Show("Đã lưu vào SQL và DataGridView.", "Thông báo",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        private void NotifyDeleted()
        {
            MessageBox.Show("Đã xóa khỏi SQL và DataGridView.", "Thông báo",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnThem_Click(object sender, EventArgs e)
        {
            // LẦN 2: thực thi lưu
            if (pendingMode == OperationMode.Add && awaitingConfirmation)
            {
                // validate lúc lưu (sau khi user đã nhập)
                if (!ValidateRequiredFields()) return;
                if (!TryParseMaNguoiDung(out int maNguoiDung)) return;

                string hoTen = NormalizeRequiredText(txtTenNV.Text);
                string chucVu = NormalizeOptionalText(txtCV.Text);
                string soDienThoai = NormalizeOptionalText(txtSDT.Text);
                string diaChi = NormalizeOptionalText(txtDC.Text);

                var newRow = gStoreDataSet.NhanVien.NewNhanVienRow();
                newRow.HoTen = hoTen;
                if (string.IsNullOrEmpty(chucVu)) newRow.SetChucVuNull(); else newRow.ChucVu = chucVu;
                if (string.IsNullOrEmpty(soDienThoai)) newRow.SetSoDienThoaiNull(); else newRow.SoDienThoai = soDienThoai;
                if (string.IsNullOrEmpty(diaChi)) newRow.SetDiaChiNull(); else newRow.DiaChi = diaChi;
                newRow.MaNguoiDung = maNguoiDung;

                gStoreDataSet.NhanVien.AddNhanVienRow(newRow);
                currentRow = newRow;

                if (CommitChanges(() => newRow.MaNhanVien))
                {
                    ResetTwoStep();
                    NotifySaved();
                    // tùy chọn: ClearTextBoxes(); // nếu muốn dọn sau khi đã lưu
                }
                return;
            }

            // LẦN 1: hỏi xác nhận -> OK thì dọn trống để nhập mới
            if (!Confirm("Bạn có chắc chắn muốn thêm Nhân viên mới?")) return;

            ArmTwoStep(OperationMode.Add);          // vào “thế chờ”
            SetTextBoxesReadOnly(false);            // cho phép nhập
            ClearTextBoxes();                       // <<< dọn sạch ngay sau khi OK như bạn yêu cầu
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

            if (!ValidateRequiredFields())
            {
                return;
            }

            if (!TryParseMaNguoiDung(out int maNguoiDung))
            {
                return;
            }

            string hoTen = NormalizeRequiredText(txtTenNV.Text);
            string chucVu = NormalizeOptionalText(txtCV.Text);
            string soDienThoai = NormalizeOptionalText(txtSDT.Text);
            string diaChi = NormalizeOptionalText(txtDC.Text);

            string originalHoTen = NormalizeRequiredText(row.HoTen);
            string originalChucVu = NormalizeOptionalText(row.IsChucVuNull() ? null : row.ChucVu);
            string originalSoDienThoai = NormalizeOptionalText(row.IsSoDienThoaiNull() ? null : row.SoDienThoai);
            string originalDiaChi = NormalizeOptionalText(row.IsDiaChiNull() ? null : row.DiaChi);
            int? originalMaNguoiDung = row.IsMaNguoiDungNull() ? (int?)null : row.MaNguoiDung;

            bool hasChanges =
                !string.Equals(hoTen, originalHoTen, StringComparison.Ordinal) ||
                !string.Equals(chucVu, originalChucVu, StringComparison.Ordinal) ||
                !string.Equals(soDienThoai, originalSoDienThoai, StringComparison.Ordinal) ||
                !string.Equals(diaChi, originalDiaChi, StringComparison.Ordinal) ||
                maNguoiDung != originalMaNguoiDung;
            if (!hasChanges)
            {
                MessageBox.Show("Không có thay đổi.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (!Confirm("Bạn có chắc chắn muốn cập nhật thông tin nhân viên này?"))
            {
                return;
            }

            row.HoTen = hoTen;

            if (string.IsNullOrEmpty(chucVu))
            {
                row.SetChucVuNull();
            }
            else
            {
                row.ChucVu = chucVu;
            }

            if (string.IsNullOrEmpty(soDienThoai))
            {
                row.SetSoDienThoaiNull();
            }
            else
            {
                row.SoDienThoai = soDienThoai;
            }

            if (string.IsNullOrEmpty(diaChi))
            {
                row.SetDiaChiNull();
            }
            else
            {
                row.DiaChi = diaChi;
            }

            row.MaNguoiDung = maNguoiDung;

            if (CommitChanges(() => row.MaNhanVien))
            {
                ShowSuccess("Đã cập nhật nhân viên thành công !");
            }
        }
        

        private void btnXoa_Click(object sender, EventArgs e)
        {
            var row = GetSelectedRow();
            if (row == null)
            {
                MessageBox.Show("Vui lòng chọn nhân viên cần xóa.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (!Confirm("Bạn có chắc chắn muốn xóa nhân viên này?", MessageBoxIcon.Warning))
            {
                return;
            }

            currentRow = row;

            DeleteNhanVien();
        }

        

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
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

        private bool DeleteNhanVien()
        {
            if (currentRow == null)
            {
                MessageBox.Show("Không có nhân viên được chọn để xóa.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }



            int deletedId = currentRow.MaNhanVien;
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
            if (CommitChanges(() => nextSelectionId ?? deletedId))
            {
                ShowSuccess("Đã xóa Nhân viên");
                return true;
            }

            return false;
        }



       

        private bool CommitChanges(Func<int?> selectIdProvider)
        {
            try
            {
                Validate();
                nhanVienBindingSource.EndEdit();
                dataGridView1.EndEdit();
                nhanVienTableAdapter.Update(gStoreDataSet.NhanVien);
                int? selectId = selectIdProvider != null ? selectIdProvider() : (int?)null;
                ReloadData(selectId);
                
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không thể lưu dữ liệu. Chi tiết: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
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
       
      
        private string NormalizeOptionalText(string value)
        {
            return string.IsNullOrWhiteSpace(value) ? string.Empty : value.Trim();
        }

        private string NormalizeRequiredText(string value)
        {
            return string.IsNullOrWhiteSpace(value) ? string.Empty : value.Trim();
        }

        private bool ValidateRequiredFields()
        {
            if (string.IsNullOrWhiteSpace(txtTenNV.Text))
            {
                MessageBox.Show("Không được để trống Họ tên.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtCV.Text))
            {
                MessageBox.Show("Không được để trống Chức vụ.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtSDT.Text))
            {
                MessageBox.Show("Không được để trống Số điện thoại.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

                return true;
            }

        private bool TryParseMaNguoiDung(out int maNguoiDung)
        {
            string value = txtMaND.Text.Trim();
            if (int.TryParse(value, out int parsed))
            {
                maNguoiDung = parsed;
                return true;
            }

            MessageBox.Show("Mã người dùng phải là số.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            maNguoiDung = 0;
            return false;
        }
       
    }
}
