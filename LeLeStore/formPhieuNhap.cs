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
    public partial class formPhieuNhap : Form
    {
        public formPhieuNhap()
        {
            InitializeComponent();
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void label8_Click(object sender, EventArgs e)
        {

        }

        private void formPhieuNhap_Load(object sender, EventArgs e)
        {
            // TODO: This line of code loads data into the 'gStoreDataSet.ChiTietGiaoDichKho' table. You can move, or remove it, as needed.
            this.chiTietGiaoDichKhoTableAdapter.Fill(this.gStoreDataSet.ChiTietGiaoDichKho);

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}
