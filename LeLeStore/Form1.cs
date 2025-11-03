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
        public Form1()
        {
            InitializeComponent();
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
        private void menuTransition_Tick(object sender, EventArgs e)
        {
            if (menuExpand == false)
            {
                menuContainer.Height += 10;
                if (menuContainer.Height >= 160)
                {
                    menuTransition.Stop();
                    menuExpand = true;
                }

            }
            else
            {
                menuContainer.Height -= 10;
                if (menuContainer.Height <= 53)
                {
                    menuTransition.Stop();
                    menuExpand = false;
                }
            }
        }
        bool sidebarExpand = true;
        private void sidebarTransition_Tick(object sender, EventArgs e)
        {
            if (sidebarExpand)
            {
                sidebar.Width -= 10;
                if (sidebar.Width <= 52)
                {
                    sidebarExpand = false;
                    sidebarTransition.Stop();
                    
                }
            }
            else
            {
                sidebar.Width += 10;
                if (sidebar.Width >= 251)
                {
                    sidebarExpand = true;
                    sidebarTransition.Stop();
                    
                }
            }
        }

        private void button1_Click_1(object sender, EventArgs e)
        {

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
    }
}
