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
    public partial class formPayMent : Form
    {
        public formPayMent()
        {
            InitializeComponent();
        }
        private Control CreateProductCard(GStoreDataSet.SanPhamRow sp)
        {
            var card = new Panel
            {
                Width = 140,
                Height = 210,
                Margin = new Padding(8),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            var pic = new PictureBox
            {
                Width = 120,
                Height = 120,
                Top = 8,
                Left = 10,
                SizeMode = PictureBoxSizeMode.Zoom
            };
            try
            {
                if (!sp.IsHinhAnhNull() && File.Exists(sp.HinhAnh))
                    pic.Image = Image.FromFile(sp.HinhAnh);
                else
                    pic.Image = Properties.Resources.no_image;
            }
            catch { }

            var lblName = new Label
            {
                AutoSize = false,
                Width = 120,
                Height = 32,
                Left = 10,
                Top = pic.Bottom + 4,
                TextAlign = ContentAlignment.MiddleCenter,
                Text = sp.TenSP
            };

            var lblPrice = new Label
            {
                AutoSize = false,
                Width = 120,
                Height = 18,
                Left = 10,
                Top = lblName.Bottom,
                ForeColor = Color.Firebrick,
                TextAlign = ContentAlignment.MiddleCenter,
                Text = sp.DonGia.ToString("N0") + " đ"
            };

            var lblStock = new Label
            {
                AutoSize = false,
                Width = 120,
                Height = 16,
                Left = 10,
                Top = lblPrice.Bottom,
                ForeColor = Color.DimGray,
                TextAlign = ContentAlignment.MiddleCenter,
                Text = $"Tồn: {sp.SoLuong}"
            };

            var btnAdd = new Button
            {
                Width = 120,
                Height = 28,
                Left = 10,
                Top = lblStock.Bottom + 4,
                Text = sp.SoLuong > 0 ? "Thêm" : "Hết hàng",
                BackColor = Color.Black,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Tag = sp,
                Enabled = sp.SoLuong > 0
            };
            btnAdd.FlatAppearance.BorderSize = 0;
            btnAdd.Click += BtnAdd_Click;

            card.Controls.Add(pic);
            card.Controls.Add(lblName);
            card.Controls.Add(lblPrice);
            card.Controls.Add(lblStock);
            card.Controls.Add(btnAdd);
            return card;
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void btnThanhToan_Click(object sender, EventArgs e)
        {

        }

        private void splitContainer1_Panel1_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
