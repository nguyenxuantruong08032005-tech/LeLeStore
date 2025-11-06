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
    public partial class Form1 : Form
    {
        private readonly UserRole _role;
        private readonly string _username;
        formDashBoard dashBoard;
        formPoint point;
        formUpdateClient updateclient;
        formUser user;
        formProduct product;
        formStaff staff;
        formSupplier supplier;
        formEmployeeSalary salary;
        formPayMent payment;
        private readonly Dictionary<Button, string> _sidebarButtonTexts = new Dictionary<Button, string>();
        public Form1() : this(UserRole.QuanLy, string.Empty)
        {
        }

        public Form1(UserRole role, string username)
        {
            _role = role;
            _username = username ?? string.Empty;

            InitializeComponent();
            mdiProp();
            InitializeSidebarButtons();
            SetSidebarButtonState(true);
            ApplyRolePermissions();
            UpdateTitle();
        }
        private void ApplyRolePermissions()
        {
            panel8.Visible = _role != UserRole.Kho;
            panel6.Visible = _role != UserRole.BanHang;
            pnProduct.Visible = _role == UserRole.QuanLy;
        }

        private void UpdateTitle()
        {
            string roleName = _role.ToDisplayName();
            string header = $"LeLeStore - {roleName}";

            if (!string.IsNullOrWhiteSpace(_username))
            {
                header += $" ({_username})";
            }

            label1.Text = header;
            Text = header;
        }


        private void panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {

        }

     
        bool menuExpand = false;
        private void mdiProp()
        {
            this.SetBevel(false);
            Controls.OfType<MdiClient>().FirstOrDefault().BackColor = Color.FromArgb(232, 234, 237);
        }
        private void menuTransition_Tick(object sender, EventArgs e)
        {
            const int minHeight = 55;
            const int maxHeight = 168;
            const int step = 25;
            if (menuExpand == false)
            {
                menuContainer.Height = Math.Min(menuContainer.Height + step, maxHeight);
                if (menuContainer.Height >= maxHeight)
                {
                    menuTransition.Stop();
                    menuExpand = true;
                }

            }
            else
            {
                menuContainer.Height = Math.Max(menuContainer.Height - step, minHeight); ;
                if (menuContainer.Height <= minHeight)
                {
                    menuTransition.Stop();
                    menuExpand = false;
                }
            }
        }
        bool sidebarExpand = true;
        private void sidebarTransition_Tick(object sender, EventArgs e)
        {
            const int minWidth = 47;
            const int maxWidth = 251;
            const int step = 5;
            if (sidebarExpand)
            {
                int newWidth = Math.Max(minWidth, flowLayoutPanel1.Width - step);
                flowLayoutPanel1.Width = newWidth;

                if (newWidth <= minWidth)
                {
                    sidebarExpand = false;
                    sidebarTransition.Stop();
                    pnDashDoard.Width = flowLayoutPanel1.Width;
                    pUser.Width = flowLayoutPanel1.Width;
                    pnProduct.Width = flowLayoutPanel1.Width;
                    pnClient.Width = flowLayoutPanel1.Width;
                    pnEmployeeSalary.Width = flowLayoutPanel1.Width;
                    pnPayMent.Width = flowLayoutPanel1.Width;
                    pnSupplier.Width = flowLayoutPanel1.Width;
                    pnLogout.Width = flowLayoutPanel1.Width;
                    menuContainer.Width = flowLayoutPanel1.Width;
                    SetSidebarButtonState(false);

                }
            }
            else
            {
                int newWidth = Math.Min(maxWidth, flowLayoutPanel1.Width + step);
                flowLayoutPanel1.Width = newWidth;

                if (newWidth >= maxWidth)
                {
                    sidebarExpand = true;
                    sidebarTransition.Stop();

                    pnDashDoard.Width = flowLayoutPanel1.Width;
                    pUser.Width = flowLayoutPanel1.Width;
                    pnProduct.Width = flowLayoutPanel1.Width;
                    pnClient.Width = flowLayoutPanel1.Width;
                    pnEmployeeSalary.Width = flowLayoutPanel1.Width;
                    pnPayMent.Width = flowLayoutPanel1.Width;
                    pnSupplier.Width = flowLayoutPanel1.Width;
                    pnLogout.Width = flowLayoutPanel1.Width;
                    menuContainer.Width = flowLayoutPanel1.Width;
                    SetSidebarButtonState(true);
                }
            }
        }

        private void InitializeSidebarButtons()
        {
            foreach (var button in EnumerateSidebarButtons(flowLayoutPanel1))
            {
                if (!_sidebarButtonTexts.ContainsKey(button))
                {
                    _sidebarButtonTexts.Add(button, button.Text);
                }
            }
        }

        private static IEnumerable<Button> EnumerateSidebarButtons(Control parent)
        {
            foreach (Control control in parent.Controls)
            {
                if (control is Button button)
                {
                    yield return button;
                }
                else
                {
                    foreach (var child in EnumerateSidebarButtons(control))
                    {
                        yield return child;
                    }
                }
            }
        }

        private void SetSidebarButtonState(bool expanded)
        {
            foreach (var entry in _sidebarButtonTexts)
            {
                var button = entry.Key;
                if (expanded)
                {
                    button.Text = entry.Value;
                    button.ImageAlign = ContentAlignment.MiddleLeft;
                    button.TextAlign = ContentAlignment.MiddleLeft;
                    button.Padding = Padding.Empty;
                }
                else
                {
                    button.Text = string.Empty;
                    button.ImageAlign = ContentAlignment.MiddleCenter;
                    button.TextAlign = ContentAlignment.MiddleCenter;
                    button.Padding = Padding.Empty;
                }
            }
        }


        private void menu_Click(object sender, EventArgs e)
        {
            menuTransition.Start();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void btnHam_Click(object sender, EventArgs e)
        {
            sidebarTransition.Start();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnUpdateClient_Click(object sender, EventArgs e)
        {
            if(updateclient == null)
            {
                updateclient = new formUpdateClient();
                updateclient.FormClosed += UpdateClient_FormClosed;
                updateclient.MdiParent = this;
                updateclient.Dock = DockStyle.Fill;
                updateclient.Show();
            }
            else
            {
                updateclient.Activate();
            }
        }
        private void UpdateClient_FormClosed(object sender, EventArgs e)
        {
            updateclient = null;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (user == null)
            {
                user = new formUser();
                user.FormClosed += About_FormClosed;
                user.MdiParent = this;
                user.Dock = DockStyle.Fill;
                user.Show();
            }
            else
            {
                user.Activate();
            }
        }
        private void About_FormClosed(object sender, EventArgs e)
        {
            user = null;
        }

        private void sbmenu2_Click(object sender, EventArgs e)
        {
            if (point == null)
            {
                point = new formPoint();
                point.FormClosed += Point_FormClosed;
                point.MdiParent = this;
                point.Dock = DockStyle.Fill;
                point.Show();
            }
            else
            {
                point.Activate();
            }
        }
        private void Point_FormClosed(object sender, EventArgs e)
        {
            point = null;
        }

        private void button4_Click(object sender, EventArgs e)
        {

            if (product == null)
            {
                product = new formProduct();
                product.FormClosed += Product_FormClosed;
                product.MdiParent = this;
                product.Dock = DockStyle.Fill;
                product.Show();
            }
            else
            {
                product.Activate();
            }
        }
        private void Product_FormClosed(object sender, EventArgs e)
        {
            product = null;
        }

        private void btnStaff_Click(object sender, EventArgs e)
        {
            if (staff == null)
            {
                staff = new formStaff();
                staff.FormClosed += Staff_FormClosed;
                staff.MdiParent = this;
                staff.Dock = DockStyle.Fill;
                staff.Show();
            }
            else
            {
                staff.Activate();
            }
        }
        private void Staff_FormClosed(object sender, EventArgs e)
        {
            staff = null;
        }

        private void btnSupplier_Click(object sender, EventArgs e)
        {
            if (supplier == null)
            {
                supplier = new formSupplier();
                supplier.FormClosed += Supplier_FormClosed;
                supplier.MdiParent = this;
                supplier.Dock = DockStyle.Fill;
                supplier.Show();
            }
            else
            {
                supplier.Activate();
            }
        }
        private void Supplier_FormClosed(object sender, EventArgs e)
        {
            supplier = null;
        }

        private void btnEmployeeSalary_Click(object sender, EventArgs e)
        {
            if (salary == null)
            {
                salary = new formEmployeeSalary();
                salary.FormClosed += EmployeeSalary_FormClosed;
                salary.MdiParent = this;
                salary.Dock = DockStyle.Fill;
                salary.Show();
            }
            else
            {
                salary.Activate();
            }
        }
        private void EmployeeSalary_FormClosed(object sender, EventArgs e)
        {
            salary = null;
        }

        private void btnDashBoard_Click(object sender, EventArgs e)
        {
            if (dashBoard == null)
            {
                dashBoard = new formDashBoard();
                dashBoard.FormClosed += Dashboard_FormClosed;
                dashBoard.MdiParent = this;
                dashBoard.Show();
            }
            else
            {
                dashBoard.Activate();
            }
        }
        private void Dashboard_FormClosed(object sender, EventArgs e)
        {
            dashBoard = null;
        }

        private void btnPayMent_Click(object sender, EventArgs e)
        {
            if (payment == null)
            {
                payment = new formPayMent();
                payment.FormClosed += PayMent_FormClosed;
                payment.MdiParent = this;
                payment.Show();
            }
            else
            {
                payment.Activate();
            }
        }
        private void PayMent_FormClosed(object sender, EventArgs e)
        {
            payment = null;
        }
        bool menuExpand2 = false;
        private void menuTransition2_Tick(object sender, EventArgs e)
        {
            const int minHeight = 55;
            const int maxHeight = 210;
            const int step = 15;
            if (menuExpand2 == false)
            {
                menuContainer2.Height = Math.Min(menuContainer2.Height + step, maxHeight);
                if (menuContainer2.Height >= maxHeight)
                {
                    menuTransition2.Stop();
                    menuExpand2 = true;
                }

            }
            else
            {
                menuContainer2.Height = Math.Max(menuContainer2.Height - step, minHeight); ;
                if (menuContainer2.Height <= minHeight)
                {
                    menuTransition2.Stop();
                    menuExpand2 = false;
                }
            }
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            menuTransition2.Start();
        }
    }
}
