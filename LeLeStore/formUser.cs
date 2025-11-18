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
        public formUser()
        {
            InitializeComponent();


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
            if (!isEditingUser)
            {
                if (nguoiDungBindingSource.Current is DataRowView currentRow)
                {
                    DialogResult confirm = MessageBox.Show("Bạn muốn sửa thông tin người dùng ?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (confirm == DialogResult.Yes)
                    {
                        isEditingUser = true;
                        selectedEditRow = currentRow;
                        ClearUserInputs();
                    }
                }
                return;
            }

            if (selectedEditRow == null)
            {
                MessageBox.Show("Vui lòng chọn người dùng cần sửa từ danh sách.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                isEditingUser = false;
                return;
            }

            if (string.IsNullOrWhiteSpace(txtTenDN.Text) || string.IsNullOrWhiteSpace(txtMK.Text) || string.IsNullOrWhiteSpace(txtVaiTro.Text))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin trước khi cập nhật.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                selectedEditRow.BeginEdit();
                selectedEditRow["TenDangNhap"] = txtTenDN.Text.Trim();
                selectedEditRow["MatKhau"] = txtMK.Text;
                selectedEditRow["VaiTro"] = txtVaiTro.Text.Trim();
                selectedEditRow.EndEdit();
                nguoiDungBindingSource.EndEdit();
                nguoiDungTableAdapter.Update(gStoreDataSet.NguoiDung);
                nguoiDungTableAdapter.Fill(gStoreDataSet.NguoiDung);
                MessageBox.Show("Đã cập nhật thông tin người dùng thành công !", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                selectedEditRow.CancelEdit();
                MessageBox.Show($"Không thể cập nhật thông tin người dùng. Lỗi: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                isEditingUser = false;
                selectedEditRow = null;
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
            if (!isAddingUser)
            {
                DialogResult confirm = MessageBox.Show("Bạn muốn thêm người dùng mới ?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (confirm == DialogResult.Yes)
                {
                    isAddingUser = true;
                    ClearUserInputs();
                }
                return;
            }

            if (string.IsNullOrWhiteSpace(txtTenDN.Text) || string.IsNullOrWhiteSpace(txtMK.Text) || string.IsNullOrWhiteSpace(txtVaiTro.Text))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin người dùng mới.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                var newRow = gStoreDataSet.NguoiDung.NewNguoiDungRow();
                newRow.TenDangNhap = txtTenDN.Text.Trim();
                newRow.MatKhau = txtMK.Text;
                newRow.VaiTro = txtVaiTro.Text.Trim();
                gStoreDataSet.NguoiDung.AddNguoiDungRow(newRow);

                nguoiDungBindingSource.EndEdit();
                nguoiDungTableAdapter.Update(gStoreDataSet.NguoiDung);
                nguoiDungTableAdapter.Fill(gStoreDataSet.NguoiDung);

                MessageBox.Show("Đã thêm người dùng mới thành công !", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Không thể thêm người dùng mới. Lỗi: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                isAddingUser = false;
            }
        }
    }
}


