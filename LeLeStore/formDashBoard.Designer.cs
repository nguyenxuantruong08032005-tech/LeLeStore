namespace LeLeStore
{
    partial class formDashBoard
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.gbFilterBanHang = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.dtpFromBH = new System.Windows.Forms.DateTimePicker();
            this.dtpToBH = new System.Windows.Forms.DateTimePicker();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.cboLoaiThongKeBH = new System.Windows.Forms.ComboBox();
            this.btnThongKeBH = new ReaLTaiizor.Controls.Button();
            this.tabpage = new System.Windows.Forms.TabControl();
            this.tabPageBanHang = new System.Windows.Forms.TabPage();
            this.tabPageKho = new System.Windows.Forms.TabPage();
            this.gbSummaryBH = new System.Windows.Forms.GroupBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txtTongDoanhThu = new System.Windows.Forms.TextBox();
            this.txtTongSoHoaDon = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.txtTongSoSPbanra = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.dgvBanHang = new System.Windows.Forms.DataGridView();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.btnThongKeKho = new ReaLTaiizor.Controls.Button();
            this.cboLoaiThongKeKho = new System.Windows.Forms.ComboBox();
            this.label7 = new System.Windows.Forms.Label();
            this.dtpToKho = new System.Windows.Forms.DateTimePicker();
            this.label8 = new System.Windows.Forms.Label();
            this.dtpFromKho = new System.Windows.Forms.DateTimePicker();
            this.label9 = new System.Windows.Forms.Label();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.txtTongSLTon = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.txtSLNhap = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.txtTongSoSP = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.txtSLXuat = new System.Windows.Forms.TextBox();
            this.label13 = new System.Windows.Forms.Label();
            this.dgvKho = new System.Windows.Forms.DataGridView();
            this.gbFilterBanHang.SuspendLayout();
            this.tabpage.SuspendLayout();
            this.tabPageBanHang.SuspendLayout();
            this.tabPageKho.SuspendLayout();
            this.gbSummaryBH.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvBanHang)).BeginInit();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvKho)).BeginInit();
            this.SuspendLayout();
            // 
            // gbFilterBanHang
            // 
            this.gbFilterBanHang.Controls.Add(this.btnThongKeBH);
            this.gbFilterBanHang.Controls.Add(this.cboLoaiThongKeBH);
            this.gbFilterBanHang.Controls.Add(this.label3);
            this.gbFilterBanHang.Controls.Add(this.dtpToBH);
            this.gbFilterBanHang.Controls.Add(this.label2);
            this.gbFilterBanHang.Controls.Add(this.dtpFromBH);
            this.gbFilterBanHang.Controls.Add(this.label1);
            this.gbFilterBanHang.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            this.gbFilterBanHang.Location = new System.Drawing.Point(19, 34);
            this.gbFilterBanHang.Name = "gbFilterBanHang";
            this.gbFilterBanHang.Size = new System.Drawing.Size(734, 212);
            this.gbFilterBanHang.TabIndex = 0;
            this.gbFilterBanHang.TabStop = false;
            this.gbFilterBanHang.Text = "Điều kiện lọc";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(19, 41);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(96, 24);
            this.label1.TabIndex = 0;
            this.label1.Text = "Từ ngày:";
            // 
            // dtpFromBH
            // 
            this.dtpFromBH.Location = new System.Drawing.Point(23, 68);
            this.dtpFromBH.Name = "dtpFromBH";
            this.dtpFromBH.Size = new System.Drawing.Size(347, 30);
            this.dtpFromBH.TabIndex = 1;
            // 
            // dtpToBH
            // 
            this.dtpToBH.Location = new System.Drawing.Point(23, 158);
            this.dtpToBH.Name = "dtpToBH";
            this.dtpToBH.Size = new System.Drawing.Size(347, 30);
            this.dtpToBH.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(19, 131);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(106, 24);
            this.label2.TabIndex = 2;
            this.label2.Text = "Đến ngày:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(444, 41);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(146, 24);
            this.label3.TabIndex = 4;
            this.label3.Text = "Loại thống kê:";
            // 
            // cboLoaiThongKeBH
            // 
            this.cboLoaiThongKeBH.FormattingEnabled = true;
            this.cboLoaiThongKeBH.Location = new System.Drawing.Point(448, 68);
            this.cboLoaiThongKeBH.Name = "cboLoaiThongKeBH";
            this.cboLoaiThongKeBH.Size = new System.Drawing.Size(255, 32);
            this.cboLoaiThongKeBH.TabIndex = 5;
            // 
            // btnThongKeBH
            // 
            this.btnThongKeBH.BackColor = System.Drawing.Color.Transparent;
            this.btnThongKeBH.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(34)))), ((int)(((byte)(37)))));
            this.btnThongKeBH.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnThongKeBH.EnteredBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(165)))), ((int)(((byte)(37)))), ((int)(((byte)(37)))));
            this.btnThongKeBH.EnteredColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(34)))), ((int)(((byte)(37)))));
            this.btnThongKeBH.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnThongKeBH.Image = null;
            this.btnThongKeBH.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnThongKeBH.InactiveColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(34)))), ((int)(((byte)(37)))));
            this.btnThongKeBH.Location = new System.Drawing.Point(499, 126);
            this.btnThongKeBH.Name = "btnThongKeBH";
            this.btnThongKeBH.PressedBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(165)))), ((int)(((byte)(37)))), ((int)(((byte)(37)))));
            this.btnThongKeBH.PressedColor = System.Drawing.Color.FromArgb(((int)(((byte)(165)))), ((int)(((byte)(37)))), ((int)(((byte)(37)))));
            this.btnThongKeBH.Size = new System.Drawing.Size(111, 28);
            this.btnThongKeBH.TabIndex = 1;
            this.btnThongKeBH.Text = "Thống Kê";
            this.btnThongKeBH.TextAlignment = System.Drawing.StringAlignment.Center;
            this.btnThongKeBH.Click += new System.EventHandler(this.btnThongKeBH_Click);
            // 
            // tabpage
            // 
            this.tabpage.Controls.Add(this.tabPageBanHang);
            this.tabpage.Controls.Add(this.tabPageKho);
            this.tabpage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabpage.Location = new System.Drawing.Point(0, 0);
            this.tabpage.Name = "tabpage";
            this.tabpage.SelectedIndex = 0;
            this.tabpage.Size = new System.Drawing.Size(1355, 807);
            this.tabpage.TabIndex = 1;
            // 
            // tabPageBanHang
            // 
            this.tabPageBanHang.Controls.Add(this.dgvBanHang);
            this.tabPageBanHang.Controls.Add(this.gbSummaryBH);
            this.tabPageBanHang.Controls.Add(this.gbFilterBanHang);
            this.tabPageBanHang.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tabPageBanHang.Location = new System.Drawing.Point(4, 29);
            this.tabPageBanHang.Name = "tabPageBanHang";
            this.tabPageBanHang.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageBanHang.Size = new System.Drawing.Size(1347, 774);
            this.tabPageBanHang.TabIndex = 0;
            this.tabPageBanHang.Text = "Thống kê bán hàng";
            this.tabPageBanHang.UseVisualStyleBackColor = true;
            // 
            // tabPageKho
            // 
            this.tabPageKho.Controls.Add(this.dgvKho);
            this.tabPageKho.Controls.Add(this.groupBox4);
            this.tabPageKho.Controls.Add(this.groupBox3);
            this.tabPageKho.Location = new System.Drawing.Point(4, 29);
            this.tabPageKho.Name = "tabPageKho";
            this.tabPageKho.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageKho.Size = new System.Drawing.Size(1112, 695);
            this.tabPageKho.TabIndex = 1;
            this.tabPageKho.Text = "Thống kê kho";
            this.tabPageKho.UseVisualStyleBackColor = true;
            // 
            // gbSummaryBH
            // 
            this.gbSummaryBH.Controls.Add(this.txtTongSoSPbanra);
            this.gbSummaryBH.Controls.Add(this.label6);
            this.gbSummaryBH.Controls.Add(this.txtTongSoHoaDon);
            this.gbSummaryBH.Controls.Add(this.label5);
            this.gbSummaryBH.Controls.Add(this.txtTongDoanhThu);
            this.gbSummaryBH.Controls.Add(this.label4);
            this.gbSummaryBH.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            this.gbSummaryBH.Location = new System.Drawing.Point(795, 17);
            this.gbSummaryBH.Name = "gbSummaryBH";
            this.gbSummaryBH.Size = new System.Drawing.Size(392, 328);
            this.gbSummaryBH.TabIndex = 1;
            this.gbSummaryBH.TabStop = false;
            this.gbSummaryBH.Text = "Khu vực tổng quan";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(19, 45);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(168, 24);
            this.label4.TabIndex = 0;
            this.label4.Text = "Tổng doanh thu:";
            // 
            // txtTongDoanhThu
            // 
            this.txtTongDoanhThu.Location = new System.Drawing.Point(25, 90);
            this.txtTongDoanhThu.Name = "txtTongDoanhThu";
            this.txtTongDoanhThu.Size = new System.Drawing.Size(259, 30);
            this.txtTongDoanhThu.TabIndex = 1;
            // 
            // txtTongSoHoaDon
            // 
            this.txtTongSoHoaDon.Location = new System.Drawing.Point(34, 284);
            this.txtTongSoHoaDon.Name = "txtTongSoHoaDon";
            this.txtTongSoHoaDon.Size = new System.Drawing.Size(142, 30);
            this.txtTongSoHoaDon.TabIndex = 3;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(28, 239);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(180, 24);
            this.label5.TabIndex = 2;
            this.label5.Text = "Tổng số hóa đơn:";
            // 
            // txtTongSoSPbanra
            // 
            this.txtTongSoSPbanra.Location = new System.Drawing.Point(25, 192);
            this.txtTongSoSPbanra.Name = "txtTongSoSPbanra";
            this.txtTongSoSPbanra.Size = new System.Drawing.Size(259, 30);
            this.txtTongSoSPbanra.TabIndex = 5;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(19, 147);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(259, 24);
            this.label6.TabIndex = 4;
            this.label6.Text = "Tổng số sản phẩm bán ra:";
            // 
            // dgvBanHang
            // 
            this.dgvBanHang.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvBanHang.Location = new System.Drawing.Point(19, 381);
            this.dgvBanHang.Name = "dgvBanHang";
            this.dgvBanHang.RowHeadersWidth = 62;
            this.dgvBanHang.RowTemplate.Height = 28;
            this.dgvBanHang.Size = new System.Drawing.Size(1299, 320);
            this.dgvBanHang.TabIndex = 2;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.btnThongKeKho);
            this.groupBox3.Controls.Add(this.cboLoaiThongKeKho);
            this.groupBox3.Controls.Add(this.label7);
            this.groupBox3.Controls.Add(this.dtpToKho);
            this.groupBox3.Controls.Add(this.label8);
            this.groupBox3.Controls.Add(this.dtpFromKho);
            this.groupBox3.Controls.Add(this.label9);
            this.groupBox3.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            this.groupBox3.Location = new System.Drawing.Point(26, 26);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(637, 212);
            this.groupBox3.TabIndex = 1;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Điều kiện lọc";
            // 
            // btnThongKeKho
            // 
            this.btnThongKeKho.BackColor = System.Drawing.Color.Transparent;
            this.btnThongKeKho.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(34)))), ((int)(((byte)(37)))));
            this.btnThongKeKho.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnThongKeKho.EnteredBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(165)))), ((int)(((byte)(37)))), ((int)(((byte)(37)))));
            this.btnThongKeKho.EnteredColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(34)))), ((int)(((byte)(37)))));
            this.btnThongKeKho.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnThongKeKho.Image = null;
            this.btnThongKeKho.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnThongKeKho.InactiveColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(34)))), ((int)(((byte)(37)))));
            this.btnThongKeKho.Location = new System.Drawing.Point(441, 128);
            this.btnThongKeKho.Name = "btnThongKeKho";
            this.btnThongKeKho.PressedBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(165)))), ((int)(((byte)(37)))), ((int)(((byte)(37)))));
            this.btnThongKeKho.PressedColor = System.Drawing.Color.FromArgb(((int)(((byte)(165)))), ((int)(((byte)(37)))), ((int)(((byte)(37)))));
            this.btnThongKeKho.Size = new System.Drawing.Size(111, 28);
            this.btnThongKeKho.TabIndex = 1;
            this.btnThongKeKho.Text = "Thống Kê";
            this.btnThongKeKho.TextAlignment = System.Drawing.StringAlignment.Center;
            // 
            // cboLoaiThongKeKho
            // 
            this.cboLoaiThongKeKho.FormattingEnabled = true;
            this.cboLoaiThongKeKho.Location = new System.Drawing.Point(390, 70);
            this.cboLoaiThongKeKho.Name = "cboLoaiThongKeKho";
            this.cboLoaiThongKeKho.Size = new System.Drawing.Size(238, 32);
            this.cboLoaiThongKeKho.TabIndex = 5;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(386, 43);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(146, 24);
            this.label7.TabIndex = 4;
            this.label7.Text = "Loại thống kê:";
            // 
            // dtpToKho
            // 
            this.dtpToKho.Location = new System.Drawing.Point(23, 158);
            this.dtpToKho.Name = "dtpToKho";
            this.dtpToKho.Size = new System.Drawing.Size(347, 30);
            this.dtpToKho.TabIndex = 3;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(19, 131);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(106, 24);
            this.label8.TabIndex = 2;
            this.label8.Text = "Đến ngày:";
            // 
            // dtpFromKho
            // 
            this.dtpFromKho.Location = new System.Drawing.Point(23, 68);
            this.dtpFromKho.Name = "dtpFromKho";
            this.dtpFromKho.Size = new System.Drawing.Size(347, 30);
            this.dtpFromKho.TabIndex = 1;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(19, 41);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(96, 24);
            this.label9.TabIndex = 0;
            this.label9.Text = "Từ ngày:";
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.txtSLXuat);
            this.groupBox4.Controls.Add(this.label13);
            this.groupBox4.Controls.Add(this.txtTongSLTon);
            this.groupBox4.Controls.Add(this.label10);
            this.groupBox4.Controls.Add(this.txtSLNhap);
            this.groupBox4.Controls.Add(this.label11);
            this.groupBox4.Controls.Add(this.txtTongSoSP);
            this.groupBox4.Controls.Add(this.label12);
            this.groupBox4.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            this.groupBox4.Location = new System.Drawing.Point(724, 26);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(353, 343);
            this.groupBox4.TabIndex = 2;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Khu vực tổng quan";
            // 
            // txtTongSLTon
            // 
            this.txtTongSLTon.Location = new System.Drawing.Point(23, 145);
            this.txtTongSLTon.Name = "txtTongSLTon";
            this.txtTongSLTon.Size = new System.Drawing.Size(259, 30);
            this.txtTongSLTon.TabIndex = 5;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(21, 118);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(195, 24);
            this.label10.TabIndex = 4;
            this.label10.Text = "Tổng số lượng tồn:";
            // 
            // txtSLNhap
            // 
            this.txtSLNhap.Location = new System.Drawing.Point(25, 215);
            this.txtSLNhap.Name = "txtSLNhap";
            this.txtSLNhap.Size = new System.Drawing.Size(142, 30);
            this.txtSLNhap.TabIndex = 3;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(21, 188);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(211, 24);
            this.label11.TabIndex = 2;
            this.label11.Text = "Tổng số lượng nhập:";
            // 
            // txtTongSoSP
            // 
            this.txtTongSoSP.Location = new System.Drawing.Point(25, 72);
            this.txtTongSoSP.Name = "txtTongSoSP";
            this.txtTongSoSP.Size = new System.Drawing.Size(259, 30);
            this.txtTongSoSP.TabIndex = 1;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(19, 45);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(193, 24);
            this.label12.TabIndex = 0;
            this.label12.Text = "Tổng số sản phẩm:";
            // 
            // txtSLXuat
            // 
            this.txtSLXuat.Location = new System.Drawing.Point(25, 285);
            this.txtSLXuat.Name = "txtSLXuat";
            this.txtSLXuat.Size = new System.Drawing.Size(142, 30);
            this.txtSLXuat.TabIndex = 7;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(21, 258);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(205, 24);
            this.label13.TabIndex = 6;
            this.label13.Text = "Tổng số lượng xuất:";
            // 
            // dgvKho
            // 
            this.dgvKho.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvKho.Location = new System.Drawing.Point(26, 391);
            this.dgvKho.Name = "dgvKho";
            this.dgvKho.RowHeadersWidth = 62;
            this.dgvKho.RowTemplate.Height = 28;
            this.dgvKho.Size = new System.Drawing.Size(1191, 332);
            this.dgvKho.TabIndex = 3;
            // 
            // formDashBoard
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1355, 807);
            this.Controls.Add(this.tabpage);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "formDashBoard";
            this.Text = "Dashboard";
            this.Load += new System.EventHandler(this.formDashBoard_Load);
            this.gbFilterBanHang.ResumeLayout(false);
            this.gbFilterBanHang.PerformLayout();
            this.tabpage.ResumeLayout(false);
            this.tabPageBanHang.ResumeLayout(false);
            this.tabPageKho.ResumeLayout(false);
            this.gbSummaryBH.ResumeLayout(false);
            this.gbSummaryBH.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvBanHang)).EndInit();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvKho)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox gbFilterBanHang;
        private System.Windows.Forms.DateTimePicker dtpFromBH;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DateTimePicker dtpToBH;
        private System.Windows.Forms.Label label2;
        private ReaLTaiizor.Controls.Button btnThongKeBH;
        private System.Windows.Forms.ComboBox cboLoaiThongKeBH;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TabControl tabpage;
        private System.Windows.Forms.TabPage tabPageBanHang;
        private System.Windows.Forms.GroupBox gbSummaryBH;
        private System.Windows.Forms.TabPage tabPageKho;
        private System.Windows.Forms.TextBox txtTongSoSPbanra;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txtTongSoHoaDon;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtTongDoanhThu;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.DataGridView dgvBanHang;
        private System.Windows.Forms.GroupBox groupBox3;
        private ReaLTaiizor.Controls.Button btnThongKeKho;
        private System.Windows.Forms.ComboBox cboLoaiThongKeKho;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.DateTimePicker dtpToKho;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.DateTimePicker dtpFromKho;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.TextBox txtTongSLTon;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox txtSLNhap;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.TextBox txtTongSoSP;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.TextBox txtSLXuat;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.DataGridView dgvKho;
    }
}