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
      
        formUpdateClient updateclient;
        formUser user;
        formProduct product;
        formStaff staff;
        formSupplier supplier;
        formEmployeeSalary salary;
        formPayMent payment;
        formPhieuNhap nhap;
        
        private readonly Dictionary<Button, string> _sidebarButtonTexts = new Dictionary<Button, string>();
        public Form1() : this(UserRole.QuanLy, string.Empty)
        {
        }
        private Form activeForm = null;

        private void OpenChildForm(Form childForm)
        {
            // Đóng form con cũ nếu có
            if (activeForm != null)
                activeForm.Close();

            activeForm = childForm;
            childForm.TopLevel = false;
            childForm.FormBorderStyle = FormBorderStyle.None;
            childForm.Dock = DockStyle.Fill;
            pnlMain.Controls.Add(childForm);
            pnlMain.Tag = childForm;
            childForm.BringToFront();
            childForm.Show();
        }

        public Form1(UserRole role, string username)
        {
            _role = role;
            _username = username ?? string.Empty;

            InitializeComponent();
            flowLayoutPanel1.FlowDirection = FlowDirection.TopDown;
            flowLayoutPanel1.WrapContents = false;   // không quấn sang cột khác
            flowLayoutPanel1.AutoScroll = true;    // tùy chọn: có thể cuộn nếu thiếu chỗ

            mdiProp();
            ApplySidebarWidths();
            InitializeSidebarButtons();
            SetSidebarButtonState(true);
            ApplyRolePermissions();
            UpdateTitle();

        }
        private void ApplySidebarWidths()
        {
            int w = flowLayoutPanel1.ClientSize.Width;

            void Apply(Control parent)
            {
                foreach (Control c in parent.Controls)
                {
                    if (c is Panel) c.Width = w;   // panel chứa button
                    Apply(c);                      // lỡ có panel lồng panel
                }
            }
            Apply(flowLayoutPanel1);
        }
        private void ApplyRolePermissions()
        {
            bool isQuanLy = _role == UserRole.QuanLy;
            bool isBanHang = _role == UserRole.BanHang;
            bool isKho = _role == UserRole.Kho;

            // Top level navigation items
            pUser.Visible = isQuanLy;
            pnClient.Visible = isQuanLy;
            pnProduct.Visible = isQuanLy || isBanHang;
            pnSupplier.Visible = isQuanLy || isKho;
            pnPayMent.Visible = isQuanLy || isBanHang;
            pnEmployeeSalary.Visible = isQuanLy;
            pnDashDoard.Visible = isQuanLy;

            // Customer related submenu (menu)
            bool canAccessCustomerMenu = isQuanLy || isBanHang;
            menuContainer.Visible = canAccessCustomerMenu;
            panel1.Visible = canAccessCustomerMenu;
            panel8.Visible = canAccessCustomerMenu; // formUpdateClient
            

            // Warehouse related submenu (menu2)
            bool canAccessWarehouseMenu = isQuanLy || isBanHang || isKho;
            menuContainer2.Visible = canAccessWarehouseMenu;
            panel2.Visible = canAccessWarehouseMenu;
         
            panel4.Visible = canAccessWarehouseMenu; // formPhieuNhap
         
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
            const int minHeight = 52;
            const int maxHeight = 106;
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
            const int minWidth = 55;
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
                    ApplySidebarWidths();
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
                    ApplySidebarWidths();
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
            OpenChildForm(new formUpdateClient(_username));

        }
        private void UpdateClient_FormClosed(object sender, EventArgs e)
        {
            updateclient = null;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            OpenChildForm(new formUser());
           
        }
     

       
       

        private void button4_Click(object sender, EventArgs e)
        {
            OpenChildForm(new formProduct(_username));


        }
        
        private void btnStaff_Click(object sender, EventArgs e)
        {
            OpenChildForm(new formStaff());
            
        }
        private void Staff_FormClosed(object sender, EventArgs e)
        {
            staff = null;
        }

        private void btnSupplier_Click(object sender, EventArgs e)
        {
            OpenChildForm(new formSupplier(_username));

        }
        private void Supplier_FormClosed(object sender, EventArgs e)
        {
            supplier = null;
        }

        private void btnEmployeeSalary_Click(object sender, EventArgs e)
        {
            OpenChildForm(new formEmployeeSalary());
            
        }
        private void EmployeeSalary_FormClosed(object sender, EventArgs e)
        {
            salary = null;
        }

        private void btnDashBoard_Click(object sender, EventArgs e)
        {
            OpenChildForm(new formDashBoard());
        }
        private void Dashboard_FormClosed(object sender, EventArgs e)
        {
            dashBoard = null;
        }

        private void btnPayMent_Click(object sender, EventArgs e)
        {
            OpenChildForm(new formPayMent(_username));
        }
        private void PayMent_FormClosed(object sender, EventArgs e)
        {
            payment = null;
        }
        bool menuExpand2 = false;
        private void menuTransition2_Tick(object sender, EventArgs e)
        {
            const int minHeight = 52;
            const int maxHeight = 104;
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

        private void menu2_Click_1(object sender, EventArgs e)
        {
            menuTransition2.Start();
        }

        

        private void btnPN_Click(object sender, EventArgs e)
        {
            OpenChildForm(new formPhieuNhap());
        }
        private void PhieuNhap_FormClosed(object sender, EventArgs e)
        {
            nhap = null;
        }

       
       

        private void pnlMain_Paint(object sender, PaintEventArgs e)
        {

        }

        private void flowLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
