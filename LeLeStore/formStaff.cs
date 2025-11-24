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
        private enum OperationMode { None, Add, Edit, Delete }
        private OperationMode mode = OperationMode.None;

        private readonly GStoreDataSetTableAdapters.NguoiDungTableAdapter nguoiDungTableAdapter
           = new GStoreDataSetTableAdapters.NguoiDungTableAdapter();


        private GStoreDataSet.NhanVienRow currentRow;
        private void SetMode(OperationMode m)
        {
            mode = m;
            bool canEdit = (m == OperationMode.Add || m == OperationMode.Edit);

            // Textboxes
            txtMaNV.ReadOnly = true;
            SetTextBoxesReadOnly(!canEdit); // ô nhập mở khi Add/Edit

            // Buttons
            btnThem.Enabled = (m != OperationMode.Edit);      // đang Edit thì khóa Thêm
            btnXoa.Enabled = (m == OperationMode.None);      // chỉ cho xóa ở xem thường
            btnSua.Enabled = (m != OperationMode.Add);       // đang Add thì khóa Sửa

            btnThem.Text = (m == OperationMode.Add) ? "Lưu" : "Thêm";
            btnSua.Text = (m == OperationMode.Edit) ? "Lưu" : "Sửa";

            // 👉 Hủy chỉ bật khi đang Add/Edit
            btnHuy.Enabled = (m != OperationMode.None);
        }
        private void CancelPendingEdits()
        {
            try { this.Validate(); } catch { }
            try { dataGridView1.CancelEdit(); } catch { }
            try { nhanVienBindingSource.CancelEdit(); } catch { }

            // Nếu currentRow đang ở trạng thái sửa thêm trong DataSet thì rollback
            if (currentRow != null &&
                (currentRow.RowState == DataRowState.Modified || currentRow.RowState == DataRowState.Added))
            {
                currentRow.RejectChanges();
            }
            else
            {
                // Thử lấy row hiện chọn rồi rollback nếu cần
                if (dataGridView1.CurrentRow?.DataBoundItem is DataRowView rv &&
                    rv.Row is GStoreDataSet.NhanVienRow r &&
                    (r.RowState == DataRowState.Modified || r.RowState == DataRowState.Added))
                {
                    r.RejectChanges();
                }
            }
        }

        private void ReloadInputs()
        {
            // Nạp lại textbox theo selection hiện tại
            LoadRowFromCurrentSelection();
            try { nhanVienBindingSource.ResetCurrentItem(); } catch { }
        }

        private bool AskConfirm(string msg, MessageBoxIcon icon = MessageBoxIcon.Question)
        {
            return MessageBox.Show(msg, "Xác nhận", MessageBoxButtons.YesNo, icon) == DialogResult.Yes;
        }

        private void CommitUI()
        {
            Validate();
            try { dataGridView1.EndEdit(); } catch { }
            nhanVienBindingSource.EndEdit();
        }

        public formStaff()
        {
            InitializeComponent();

            

            dataGridView1.SelectionChanged += dataGridView1_SelectionChanged;
            txtMaNV.ReadOnly = true;
            SetTextBoxesReadOnly(false);

            cbMaNgDung.DropDownStyle = ComboBoxStyle.DropDownList;
            InitializeChucVuCombo();
            txtSDT.Validating += TxtSDT_Validating;
        }

        private void InitializeChucVuCombo()
        {
            cboChucVu.DropDownStyle = ComboBoxStyle.DropDownList;
            cboChucVu.Items.Clear();
            cboChucVu.Items.AddRange(new object[]
            {
                "Nhân viên bán hàng",
                "Quản lý cửa hàng",
                "Nhân viên kho"
            });
        }

        private void formStaff_Load(object sender, EventArgs e)
        {
            // TODO: This line of code loads data into the 'gStoreDataSet.NhanVien' table. You can move, or remove it, as needed.
            this.nhanVienTableAdapter.Fill(this.gStoreDataSet.NhanVien);
            LoadRowFromCurrentSelection();

            LoadNguoiDungCombo();
        }
        // ===================== HELPER =====================

        private void LoadNguoiDungCombo()
        {
            int? previousSelection = cbMaNgDung.SelectedValue is int value ? value : (int?)null;

            nguoiDungTableAdapter.Fill(gStoreDataSet.NguoiDung);

            var users = gStoreDataSet.NguoiDung
                .Select(row => new
                {
                    MaNguoiDung = row.MaNguoiDung,
                    DisplayText = $"{row.MaNguoiDung} - {row.VaiTro}"
                })
                .OrderBy(item => item.MaNguoiDung)
                .ToList();

            cbMaNgDung.DataSource = users;
            cbMaNgDung.DisplayMember = "DisplayText";
            cbMaNgDung.ValueMember = "MaNguoiDung";

            if (previousSelection.HasValue && users.Any(u => u.MaNguoiDung == previousSelection.Value))
            {
                cbMaNgDung.SelectedValue = previousSelection.Value;
            }
            else
            {
                cbMaNgDung.SelectedIndex = -1;
            }
        }

        private string GetSelectedChucVu()
        {
            return cboChucVu.SelectedItem as string ?? cboChucVu.Text;
        }

        private void SetSelectedChucVu(string chucVu)
        {
            if (string.IsNullOrWhiteSpace(chucVu))
            {
                cboChucVu.SelectedIndex = -1;
                return;
            }

            int index = cboChucVu.Items.IndexOf(chucVu);
            if (index >= 0)
            {
                cboChucVu.SelectedIndex = index;
            }
            else
            {
                cboChucVu.Items.Add(chucVu);
                cboChucVu.SelectedItem = chucVu;
            }
        }
        private void ShowSuccess(string message)
        {
            MessageBox.Show(message, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        // Trạng thái 2-bước
        
       

        
       
       

      

        private void btnThem_Click(object sender, EventArgs e)
        {
            CommitUI();

            if (mode != OperationMode.Add)
            {
                if (!AskConfirm("Bạn muốn thêm Nhân viên mới?")) return;

                SetMode(OperationMode.Add);
                ClearTextBoxes();
                txtTenNV.Focus();
                return;
            }

            // Pha lưu
            if (!ValidateRequiredFields()) return;
            if (!TryParseMaNguoiDung(out int maNguoiDung)) return;
            if (!AskConfirm("Xác nhận lưu nhân viên mới?")) return;

            string hoTen = NormalizeRequiredText(txtTenNV.Text);
            string chucVu = GetSelectedChucVu();
            string soDienThoai = NormalizeOptionalText(txtSDT.Text);
            string diaChi = NormalizeOptionalText(txtDC.Text);

            var newRow = gStoreDataSet.NhanVien.NewNhanVienRow();
            newRow.HoTen = hoTen;
            if (string.IsNullOrEmpty(chucVu)) newRow.SetChucVuNull(); else newRow.ChucVu = chucVu;
            if (string.IsNullOrEmpty(soDienThoai)) newRow.SetSoDienThoaiNull(); else newRow.SoDienThoai = soDienThoai;
            if (string.IsNullOrEmpty(diaChi)) newRow.SetDiaChiNull(); else newRow.DiaChi = diaChi;
            newRow.MaNguoiDung = maNguoiDung;

            gStoreDataSet.NhanVien.AddNhanVienRow(newRow);

            // Lưu + reload và focus về bản ghi mới (theo ID sinh ra)
            if (CommitChanges(() => newRow.MaNhanVien))
            {
                MessageBox.Show("Đã thêm Nhân viên.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                SetMode(OperationMode.None);
            }
        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            CommitUI();

            var row = GetSelectedRow();
            if (row == null)
            {
                MessageBox.Show("Vui lòng chọn nhân viên cần sửa.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (mode != OperationMode.Edit)
            {
                if (!AskConfirm("Bạn muốn sửa thông tin nhân viên này?")) return;

                // Vào chế độ Edit: đổ sẵn dữ liệu (đã có PopulateTextBoxes khi chọn)
                SetMode(OperationMode.Edit);
                txtTenNV.Focus();
                return;
            }

            // Pha lưu
            if (!ValidateRequiredFields()) return;
            if (!TryParseMaNguoiDung(out int maNguoiDung)) return;
            if (!AskConfirm("Xác nhận lưu thay đổi thông tin nhân viên?")) return;

            string hoTen = NormalizeRequiredText(txtTenNV.Text);
            string chucVu = GetSelectedChucVu();
            string soDienThoai = NormalizeOptionalText(txtSDT.Text);
            string diaChi = NormalizeOptionalText(txtDC.Text);

            // So sánh thay đổi (tuỳ thích giữ đoạn của bạn); ở đây cập nhật trực tiếp
            row.HoTen = hoTen;
            if (string.IsNullOrEmpty(chucVu)) row.SetChucVuNull(); else row.ChucVu = chucVu;
            if (string.IsNullOrEmpty(soDienThoai)) row.SetSoDienThoaiNull(); else row.SoDienThoai = soDienThoai;
            if (string.IsNullOrEmpty(diaChi)) row.SetDiaChiNull(); else row.DiaChi = diaChi;
            row.MaNguoiDung = maNguoiDung;

            // Lưu + reload và quay lại đúng bản ghi đang sửa
            if (CommitChanges(() => row.MaNhanVien))
            {
                MessageBox.Show("Đã cập nhật nhân viên thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                SetMode(OperationMode.None);
            }
        }
        

        private void btnXoa_Click(object sender, EventArgs e)
        {
            if (mode != OperationMode.None) return; // tránh xóa khi đang Add/Edit

            var row = GetSelectedRow();
            if (row == null)
            {
                MessageBox.Show("Vui lòng chọn nhân viên cần xóa.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (!AskConfirm("Bạn có chắc chắn muốn xóa nhân viên này?", MessageBoxIcon.Warning)) return;

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
            SetSelectedChucVu(row.IsChucVuNull() ? string.Empty : row.ChucVu);
            txtSDT.Text = row.IsSoDienThoaiNull() ? string.Empty : row.SoDienThoai;
            txtDC.Text = row.IsDiaChiNull() ? string.Empty : row.DiaChi;
            SetNguoiDungSelection(row.IsMaNguoiDungNull() ? (int?)null : row.MaNguoiDung);
        }

        private void ClearTextBoxes()
        {
            txtMaNV.Text = string.Empty;
            txtTenNV.Text = string.Empty;
            cboChucVu.SelectedIndex = -1;
            txtSDT.Text = string.Empty;
            txtDC.Text = string.Empty;
            cbMaNgDung.SelectedIndex = -1;
        }

        private void SetNguoiDungSelection(int? maNguoiDung)
        {
            if (maNguoiDung.HasValue)
            {
                cbMaNgDung.SelectedValue = maNguoiDung.Value;

                if (cbMaNgDung.SelectedIndex == -1)
                {
                    cbMaNgDung.SelectedIndex = -1;
                }
            }
            else
            {
                cbMaNgDung.SelectedIndex = -1;
            }
        }
        private void SetTextBoxesReadOnly(bool readOnly)
        {
            txtTenNV.ReadOnly = readOnly;
            
            txtSDT.ReadOnly = readOnly;
            txtDC.ReadOnly = readOnly;
            cbMaNgDung.Enabled = !readOnly;
            cboChucVu.Enabled = !readOnly;
        }

        private bool ValidatePhoneNumber()
        {
            string soDienThoai = NormalizeOptionalText(txtSDT.Text);

            if (!IsValidPhoneFormat(soDienThoai, out string message))
            {
                MessageBox.Show(message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            if (!IsPhoneNumberUnique(soDienThoai, GetCurrentEditingId()))
            {
                MessageBox.Show("Số điện thoại đã tồn tại.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            return true;
        }

        private bool IsValidPhoneFormat(string soDienThoai, out string message)
        {
            if (string.IsNullOrWhiteSpace(soDienThoai))
            {
                message = "Số điện thoại phải đủ 10 số.";
                return false;
            }

            if (!soDienThoai.All(char.IsDigit))
            {
                message = "Số điện thoại chỉ được chứa chữ số.";
                return false;
            }

            if (soDienThoai.Length < 10)
            {
                message = "Số điện thoại phải đủ 10 số.";
                return false;
            }

            if (soDienThoai.Length > 10)
            {
                message = "Số điện thoại chỉ được phép 10 số.";
                return false;
            }

            message = string.Empty;
            return true;
        }

        private bool IsPhoneNumberUnique(string soDienThoai, int? currentId)
        {
            return !gStoreDataSet.NhanVien.Any(row =>
                !row.IsSoDienThoaiNull() &&
                row.SoDienThoai == soDienThoai &&
                (!currentId.HasValue || row.MaNhanVien != currentId.Value));
        }

        private int? GetCurrentEditingId()
        {
            if (mode == OperationMode.Edit && currentRow != null && currentRow.RowState != DataRowState.Added)
            {
                return currentRow.MaNhanVien;
            }

            if (int.TryParse(txtMaNV.Text, out int id))
            {
                return id;
            }

            return null;
        }

        private void TxtSDT_Validating(object sender, CancelEventArgs e)
        {
            if (mode == OperationMode.None)
            {
                return;
            }

            if (!ValidatePhoneNumber())
            {
                e.Cancel = true;
            }
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
            LoadNguoiDungCombo();
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

            if (cboChucVu.SelectedIndex < 0)
            {
                MessageBox.Show("Không được để trống Chức vụ.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            if (!ValidatePhoneNumber())
            {
               
                return false;
            }

                return true;
            }

        private bool TryParseMaNguoiDung(out int maNguoiDung)
        {
            if (cbMaNgDung.SelectedValue is int selectedId)
            {
                maNguoiDung = selectedId;
                return true;
            }

            MessageBox.Show("Vui lòng chọn Mã người dùng hợp lệ từ danh sách.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            maNguoiDung = 0;
            return false;
        }

        private void btnHuy_Click(object sender, EventArgs e)
        {
            // Nếu không ở chế độ Add/Edit thì chỉ đồng bộ lại UI rồi thoát
            if (mode == OperationMode.None)
            {
                ReloadInputs();
                return;
            }

            if (MessageBox.Show("Hủy các thay đổi đang thực hiện?", "Xác nhận",
                                MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;

            // Hủy mọi thay đổi treo
            CancelPendingEdits();

            // Trả UI về trạng thái bình thường
            SetMode(OperationMode.None);

            // Làm tươi phần nhập theo dòng đang chọn
            ReloadInputs();
        }
    }
}
