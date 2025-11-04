using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LeLeStore
{
    public partial class FormLogin : Form
    {
       
        

        public FormLogin()
        {
            InitializeComponent();
            bunifuElipse1.ElipseRadius = 90;
            bunifuElipse1.TargetControl = this;
            this.FormBorderStyle = FormBorderStyle.None;

        }
        
        private void FormLogin_Load(object sender, EventArgs e)
        {
            txtTenDN.Focus();
        }

        private void bunifuThinButton21_Click(object sender, EventArgs e)
        {
            AttemptLogin();
        }
        private void AttemptLogin()
        {
            string username = txtTenDN.Text.Trim();
            string password = txtMatKhau.Text;

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ tên đăng nhập và mật khẩu.", "Thiếu thông tin", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (var adapter = new GStoreDataSetTableAdapters.NguoiDungTableAdapter())
                {
                    var users = adapter.GetData();
                    var matchedUser = users.FirstOrDefault(row =>
                        string.Equals(row.TenDangNhap, username, StringComparison.OrdinalIgnoreCase) &&
                        string.Equals(row.MatKhau, password));

                    if (matchedUser == null)
                    {
                        MessageBox.Show("Tên đăng nhập hoặc mật khẩu không đúng.", "Đăng nhập thất bại", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        txtMatKhau.Focus();
                        txtMatKhau.SelectAll();
                        return;
                    }

                    if (!UserRoleExtensions.TryParse(matchedUser.VaiTro, out var role))
                    {
                        MessageBox.Show("Tài khoản chưa được gán vai trò hợp lệ. Vui lòng liên hệ quản trị viên.", "Lỗi vai trò", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    OpenMainForm(role, matchedUser.TenDangNhap);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không thể kết nối tới hệ thống. Vui lòng kiểm tra lại cấu hình và thử lại.\n\n" + ex.Message, "Lỗi hệ thống", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void OpenMainForm(UserRole role, string username)
        {
            var mainForm = new Form1(role, username);
            mainForm.FormClosed += (s, args) =>
            {
                Show();
                txtMatKhau.Clear();

                if (checkBox1.Checked)
                {
                    checkBox1.Checked = false;
                }
                else
                {
                    txtMatKhau.PasswordChar = '*';
                }

                txtTenDN.Focus();
                txtTenDN.SelectAll();
            };

            Hide();
            mainForm.Show();
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void pbClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            txtMatKhau.PasswordChar = checkBox1.Checked ? '\0' : '*';
        }
    }
}
