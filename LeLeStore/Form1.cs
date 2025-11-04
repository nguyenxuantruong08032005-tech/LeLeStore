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
        formDashBoard dashBoard;
        formSubmenu1 submenu1;
        formSubmenu2 submenu2;
        formAbout about;
        formSettings settings;
       
        public Form1()
        {
            InitializeComponent();
            mdiProp();
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
            if (menuExpand == false)
            {
                menuContainer.Height += 10;
                if (menuContainer.Height >= 168)
                {
                    menuTransition.Stop();
                    menuExpand = true;
                }

            }
            else
            {
                menuContainer.Height -= 10;
                if (menuContainer.Height <= 55)
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
                    pnDashdoard.Width = flowLayoutPanel1.Width;
                    pnAbout.Width = flowLayoutPanel1.Width;
                    pnSettings.Width = flowLayoutPanel1.Width;
                    pnLogout.Width = flowLayoutPanel1.Width;
                    menuContainer.Width = flowLayoutPanel1.Width;

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

                    pnDashdoard.Width = flowLayoutPanel1.Width;
                    pnAbout.Width = flowLayoutPanel1.Width;
                    pnSettings.Width = flowLayoutPanel1.Width;
                    pnLogout.Width = flowLayoutPanel1.Width;
                    menuContainer.Width = flowLayoutPanel1.Width;
                }
            }
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            if (dashBoard == null)
            {
                dashBoard = new formDashBoard();
                dashBoard.FormClosed += Dashboard_FormClosed;
                dashBoard.MdiParent = this;
                dashBoard.Show();
            }else
            {
                dashBoard.Activate();
            }
        }
        private void Dashboard_FormClosed(object sender, EventArgs e)
        {
            dashBoard = null;
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
           
        }

        private void sbmenu1_Click(object sender, EventArgs e)
        {
            if(submenu1 == null)
            {
                submenu1 = new formSubmenu1();
                submenu1.FormClosed += Submenu1_FormClosed;
                submenu1.MdiParent = this;
                submenu1.Dock = DockStyle.Fill;
                submenu1.Show();
            }
            else
            {
                submenu1.Activate();
            }
        }
        private void Submenu1_FormClosed(object sender, EventArgs e)
        {
            submenu1 = null;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (about == null)
            {
                about = new formAbout();
                about.FormClosed += About_FormClosed;
                about.MdiParent = this;
                about.Dock = DockStyle.Fill;
                about.Show();
            }
            else
            {
                about.Activate();
            }
        }
        private void About_FormClosed(object sender, EventArgs e)
        {
            about = null;
        }

        private void sbmenu2_Click(object sender, EventArgs e)
        {
            if (submenu2 == null)
            {
                submenu2 = new formSubmenu2();
                submenu2.FormClosed += Submenu2_FormClosed;
                submenu2.MdiParent = this;
                submenu2.Dock = DockStyle.Fill;
                submenu2.Show();
            }
            else
            {
                submenu2.Activate();
            }
        }
        private void Submenu2_FormClosed(object sender, EventArgs e)
        {
            submenu2 = null;
        }

        private void button4_Click(object sender, EventArgs e)
        {

            if (settings == null)
            {
                settings = new formSettings();
                settings.FormClosed += Settings_FormClosed;
                settings.MdiParent = this;
                settings.Dock = DockStyle.Fill;
                settings.Show();
            }
            else
            {
                settings.Activate();
            }
        }
        private void Settings_FormClosed(object sender, EventArgs e)
        {
            settings = null;
        }
    }
}
