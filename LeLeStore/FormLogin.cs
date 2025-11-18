using Bunifu.Framework.UI;
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
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

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

            // bo góc như cũ
            bunifuElipse1.ElipseRadius = 90;
            bunifuElipse1.TargetControl = this;
            this.FormBorderStyle = FormBorderStyle.None;

            

            // Sự kiện để chắc chắn focus đúng khi form hiển thị
            this.Shown += FormLogin_Shown;

            // Lắng nghe thay đổi 2 textbox để tự nhảy xuống nút
            txtTenDN.TextChanged += Credentials_TextChanged;
            txtMatKhau.TextChanged += Credentials_TextChanged;

            // Gắn event như bạn đang làm:
            txtTenDN.TextChanged += Credentials_TextChanged;
            txtMatKhau.TextChanged += Credentials_TextChanged;


        }
        
        private void FormLogin_Load(object sender, EventArgs e)
        {
            txtTenDN.Focus();
        }
        private void Credentials_TextChanged(object sender, EventArgs e)
        {
           // Chỉ auto - focus nút khi đang đứng ở ô USERNAME,
    // tức là người dùng vừa nhập xong username và password đã có sẵn.
    if (ActiveControl == txtTenDN)
            {
                MaybeFocusLoginIfReady();
            }
        }

        private void MaybeFocusLoginIfReady()
        {
            bool filled = !string.IsNullOrWhiteSpace(txtTenDN.Text)
                       && !string.IsNullOrWhiteSpace(txtMatKhau.Text);

            if (filled && ActiveControl == txtTenDN && !btnDangNhap.Focused)
            {
                // Tự chuyển focus xuống nút Đăng nhập khi đủ dữ liệu
               btnDangNhap.Focus();
            }
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

        private void FormLogin_Shown(object sender, EventArgs e)
        {
            // Vào form là focus username
            txtTenDN.Focus();
            txtTenDN.SelectAll();
        }

        private void txtTenDN_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.Handled = true; e.SuppressKeyPress = true;
                if (string.IsNullOrWhiteSpace(txtMatKhau.Text))
                {
                    // Chưa nhập mật khẩu thì nhảy xuống mật khẩu
                    txtMatKhau.Focus();
                    txtMatKhau.SelectAll();
                }
                else
                {
                    // Đã đủ -> nhảy nút
                    btnDangNhap.Focus();
                }
            }
        }

        private void txtMatKhau_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.Handled = true; e.SuppressKeyPress = true;

                if (!string.IsNullOrWhiteSpace(txtTenDN.Text) &&
                    !string.IsNullOrWhiteSpace(txtMatKhau.Text))
                {
                    // Đủ 2 ô -> đăng nhập luôn
                    AttemptLogin();
                }
                else
                {
                    // Thiếu user thì nhảy lên user
                    if (string.IsNullOrWhiteSpace(txtTenDN.Text))
                    {
                        txtTenDN.Focus();
                        txtTenDN.SelectAll();
                    }
                }
            }
        }
    }
}
