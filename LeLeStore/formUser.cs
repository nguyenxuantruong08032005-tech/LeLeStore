using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace LeLeStore
{
    public partial class formUser : Form
    {
        private bool isAddingUser = false;
        private bool isEditingUser = false;
        private DataRowView selectedEditRow;
        // ADD
        private enum UserOperation { None, Add, Edit }
        private UserOperation _currentOperation = UserOperation.None;
        public formUser()
        {
            InitializeComponent();


        }
        private void SetOperation(UserOperation op)
        {
            _currentOperation = op;

            bool canEdit = (op == UserOperation.Add || op == UserOperation.Edit);
            txtMaDung.ReadOnly = true;
            txtTenDN.ReadOnly = !canEdit;
            txtMK.ReadOnly = !canEdit;
            txtVaiTro.ReadOnly = !canEdit;

            UpdateButtonsForState();

            // Hủy chỉ bật khi đang ở Add/Edit
            btnHuy.Enabled = (op != UserOperation.None);
        }
        private void CancelPendingUiEdits()
        {
            try { this.Validate(); } catch { }
            try { dataGridView1.CancelEdit(); } catch { }
            try { nguoiDungBindingSource.CancelEdit(); } catch { }

            if (nguoiDungBindingSource.Current is DataRowView rv)
            {
                var row = rv.Row;
                if (row.RowState == DataRowState.Modified || row.RowState == DataRowState.Added)
                {
                    row.RejectChanges();  // rollback về dữ liệu gốc trong DataSet
                }
            }
        }

        private void ReloadInputsFromCurrent()
        {
            if (nguoiDungBindingSource.Current is DataRowView rv)
            {
                txtMaDung.Text = rv["MaNguoiDung"]?.ToString();
                txtTenDN.Text = rv["TenDangNhap"]?.ToString();
                txtMK.Text = rv["MatKhau"]?.ToString();
                txtVaiTro.Text = rv["VaiTro"]?.ToString();
            }
            else
            {
                ClearUserInputs();
            }
        }

        private void UpdateButtonsForState()
        {
            // Giả sử bạn có 3 nút: btnThem, btnSua, btnXoa
            bool editing = _currentOperation == UserOperation.Edit;
            bool adding = _currentOperation == UserOperation.Add;

            btnThem.Enabled = !editing;
            btnXoa.Enabled = !adding && !editing;

            btnThem.Text = adding ? "Lưu" : "Thêm";
            btnSua.Text = editing ? "Lưu" : "Sửa";
        }

        private bool AskConfirm(string message)
        {
            return MessageBox.Show(message, "Xác nhận",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes;
        }

        private void CommitUI()
        {
            this.Validate();
            try { dataGridView1.EndEdit(); } catch { }
            nguoiDungBindingSource.EndEdit();
        }




        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void formUser_Load(object sender, EventArgs e)
        {
            // TODO: This line of code loads data into the 'gStoreDataSet.NguoiDung' table. You can move, or remove it, as needed.
            this.nguoiDungTableAdapter.Fill(this.gStoreDataSet.NguoiDung);
            this.ControlBox = false;
            InitializeBindings();
            SetOperation(UserOperation.None);   // <— thêm dòng này
        }

        private void InitializeBindings()
        {
            txtMaDung.ReadOnly = true;

            txtMaDung.DataBindings.Clear();
            txtTenDN.DataBindings.Clear();
            txtMK.DataBindings.Clear();
            txtVaiTro.DataBindings.Clear();

            txtMaDung.DataBindings.Add("Text", nguoiDungBindingSource, "MaNguoiDung", true, DataSourceUpdateMode.Never);
            txtTenDN.DataBindings.Add("Text", nguoiDungBindingSource, "TenDangNhap", true, DataSourceUpdateMode.Never);
            txtMK.DataBindings.Add("Text", nguoiDungBindingSource, "MatKhau", true, DataSourceUpdateMode.Never);
            txtVaiTro.DataBindings.Add("Text", nguoiDungBindingSource, "VaiTro", true, DataSourceUpdateMode.Never);
        }
        private void btnXoa_Click(object sender, EventArgs e)
        {
            if (nguoiDungBindingSource.Current is DataRowView currentRow)
            {
                DialogResult result = MessageBox.Show("Bạn chắc chắn muốn xóa người dùng này ?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (result == DialogResult.Yes)
                {
                    try
                    {
                        currentRow.Delete();
                        nguoiDungBindingSource.EndEdit();
                        nguoiDungTableAdapter.Update(gStoreDataSet.NguoiDung);
                        nguoiDungTableAdapter.Fill(gStoreDataSet.NguoiDung);
                        MessageBox.Show("Đã xóa người dùng thành công !", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Không thể xóa người dùng. Lỗi: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void btnSua_Click_1(object sender, EventArgs e)
        {
            CommitUI();

            if (_currentOperation != UserOperation.Edit)
            {
                if (nguoiDungBindingSource.Current is DataRowView currentRow)
                {
                    if (!AskConfirm("Bạn muốn sửa thông tin người dùng?")) return;

                    SetOperation(UserOperation.Edit);

                    // Điền sẵn để sửa
                    txtMaDung.Text = currentRow["MaNguoiDung"].ToString();
                    txtTenDN.Text = currentRow["TenDangNhap"].ToString();
                    txtMK.Text = currentRow["MatKhau"].ToString();
                    txtVaiTro.Text = currentRow["VaiTro"].ToString();
                }
                return;
            }

            // Đang ở chế độ Sửa -> lưu
            if (!(nguoiDungBindingSource.Current is DataRowView selectedRow))
            {
                MessageBox.Show("Vui lòng chọn người dùng cần sửa từ danh sách.", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                SetOperation(UserOperation.None);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtTenDN.Text) ||
                string.IsNullOrWhiteSpace(txtMK.Text) ||
                string.IsNullOrWhiteSpace(txtVaiTro.Text))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin trước khi cập nhật.", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!AskConfirm("Xác nhận lưu thay đổi thông tin người dùng?")) return;

            try
            {
                selectedRow.BeginEdit();
                selectedRow["TenDangNhap"] = txtTenDN.Text.Trim();
                selectedRow["MatKhau"] = txtMK.Text;
                selectedRow["VaiTro"] = txtVaiTro.Text.Trim();
                selectedRow.EndEdit();

                nguoiDungBindingSource.EndEdit();
                nguoiDungTableAdapter.Update(gStoreDataSet.NguoiDung);

                // KHÔNG Fill để tránh reset con trỏ
                nguoiDungBindingSource.ResetCurrentItem();

                MessageBox.Show("Đã cập nhật thông tin người dùng thành công !", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                selectedRow.CancelEdit();
                MessageBox.Show($"Không thể cập nhật thông tin người dùng. Lỗi: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                SetOperation(UserOperation.None);
            }
        }
        private void ClearUserInputs()
        {
            txtMaDung.Clear();
            txtTenDN.Clear();
            txtMK.Clear();
            txtVaiTro.Clear();
        }

        private void btnThem_Click(object sender, EventArgs e)
        {
            CommitUI();

            if (_currentOperation != UserOperation.Add)
            {
                if (!AskConfirm("Bạn muốn thêm người dùng mới?")) return;

                SetOperation(UserOperation.Add);
                ClearUserInputs();
                txtTenDN.Focus();
                return;
            }

            // Đang ở chế độ Thêm -> lưu
            if (string.IsNullOrWhiteSpace(txtTenDN.Text) ||
                string.IsNullOrWhiteSpace(txtMK.Text) ||
                string.IsNullOrWhiteSpace(txtVaiTro.Text))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin người dùng mới.", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!AskConfirm("Xác nhận thêm người dùng này?")) return;

            try
            {
                var newRow = gStoreDataSet.NguoiDung.NewNguoiDungRow();
                newRow.TenDangNhap = txtTenDN.Text.Trim();
                newRow.MatKhau = txtMK.Text;
                newRow.VaiTro = txtVaiTro.Text.Trim();
                gStoreDataSet.NguoiDung.AddNguoiDungRow(newRow);

                nguoiDungBindingSource.EndEdit();
                nguoiDungTableAdapter.Update(gStoreDataSet.NguoiDung);

                // Đồng bộ lại và nhảy tới dòng mới (thường là cuối)
                nguoiDungTableAdapter.Fill(gStoreDataSet.NguoiDung);
                nguoiDungBindingSource.Position = Math.Max(0, nguoiDungBindingSource.Count - 1);

                MessageBox.Show("Đã thêm người dùng mới thành công !", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Không thể thêm người dùng mới. Lỗi: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                SetOperation(UserOperation.None);
            }
        }
       

        private void btnHuy_Click(object sender, EventArgs e)
        {
            // Nếu không ở chế độ Add/Edit thì chỉ việc đồng bộ UI rồi thoát
            if (_currentOperation == UserOperation.None)
            {
                ReloadInputsFromCurrent();
                return;
            }

            var confirm = MessageBox.Show(
                "Hủy các thay đổi đang thực hiện?",
                "Xác nhận",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (confirm != DialogResult.Yes) return;

            // Hủy mọi chỉnh sửa treo (UI + BindingSource + DataRow)
            CancelPendingUiEdits();

            // Trả UI về bình thường, nạp lại dữ liệu từ dòng hiện tại
            SetOperation(UserOperation.None);
            ReloadInputsFromCurrent();

            // Làm tươi binding/grid nếu cần
            try { nguoiDungBindingSource.ResetCurrentItem(); } catch { }
        }
    }
}


