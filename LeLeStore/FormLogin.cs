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

        }

        private void bunifuThinButton21_Click(object sender, EventArgs e)
        {

        }


        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void pbClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
