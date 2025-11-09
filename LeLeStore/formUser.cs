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
        public formUser()
        {
            InitializeComponent();
            btnSua.Click += btnSua_Click_1;
            btnLuu.Click += btnLuu_Click;
            btnXoa.Click += btnXoa_Click;

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
                DialogResult result = MessageBox.Show("Bạn có chắc chắn muốn xóa người dùng này?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (result == DialogResult.Yes)
                {
                    try
                    {
                        currentRow.Delete();
                        MessageBox.Show("Đã xóa người dùng khỏi danh sách. Nhấn LƯU để cập nhật vào cơ sở dữ liệu.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
            if (nguoiDungBindingSource.Current is DataRowView currentRow)
            {
                try
                {
                    currentRow.BeginEdit();
                    currentRow["TenDangNhap"] = txtTenDN.Text.Trim();
                    currentRow["MatKhau"] = txtMK.Text;
                    currentRow["VaiTro"] = txtVaiTro.Text.Trim();
                    currentRow.EndEdit();
                    MessageBox.Show("Cập nhật thông tin người dùng thành công.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    currentRow.CancelEdit();
                    MessageBox.Show($"Không thể cập nhật thông tin người dùng. Lỗi: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnLuu_Click(object sender, EventArgs e)
        {
            try
            {
                this.Validate();
                nguoiDungBindingSource.EndEdit();
                int affectedRows = this.nguoiDungTableAdapter.Update(this.gStoreDataSet.NguoiDung);
                if (affectedRows > 0)
                {
                    MessageBox.Show("Đã lưu thay đổi vào cơ sở dữ liệu.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.nguoiDungTableAdapter.Fill(this.gStoreDataSet.NguoiDung);
                }
              
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Không thể lưu thay đổi. Lỗi: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
