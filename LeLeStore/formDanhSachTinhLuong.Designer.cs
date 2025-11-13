namespace LeLeStore
{
    partial class formDanhSachTinhLuong
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
            this.label5 = new System.Windows.Forms.Label();
            this.btnXem = new ReaLTaiizor.Controls.Button();
            this.btnXemTatCa = new ReaLTaiizor.Controls.Button();
            this.dgvBangLuong = new System.Windows.Forms.DataGridView();
            this.txtKyFilter = new System.Windows.Forms.TextBox();
            this.btnExportExcel = new ReaLTaiizor.Controls.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgvBangLuong)).BeginInit();
            this.SuspendLayout();
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            this.label5.Location = new System.Drawing.Point(66, 51);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(105, 24);
            this.label5.TabIndex = 10;
            this.label5.Text = "Kỳ lương:";
            // 
            // btnXem
            // 
            this.btnXem.BackColor = System.Drawing.Color.Transparent;
            this.btnXem.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(34)))), ((int)(((byte)(37)))));
            this.btnXem.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnXem.EnteredBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(165)))), ((int)(((byte)(37)))), ((int)(((byte)(37)))));
            this.btnXem.EnteredColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(34)))), ((int)(((byte)(37)))));
            this.btnXem.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.btnXem.Image = null;
            this.btnXem.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnXem.InactiveColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(34)))), ((int)(((byte)(37)))));
            this.btnXem.Location = new System.Drawing.Point(320, 66);
            this.btnXem.Name = "btnXem";
            this.btnXem.PressedBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(165)))), ((int)(((byte)(37)))), ((int)(((byte)(37)))));
            this.btnXem.PressedColor = System.Drawing.Color.FromArgb(((int)(((byte)(165)))), ((int)(((byte)(37)))), ((int)(((byte)(37)))));
            this.btnXem.Size = new System.Drawing.Size(158, 40);
            this.btnXem.TabIndex = 12;
            this.btnXem.Text = "Xem theo kỳ";
            this.btnXem.TextAlignment = System.Drawing.StringAlignment.Center;
            this.btnXem.Click += new System.EventHandler(this.btnXem_Click);
            // 
            // btnXemTatCa
            // 
            this.btnXemTatCa.BackColor = System.Drawing.Color.Transparent;
            this.btnXemTatCa.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(34)))), ((int)(((byte)(37)))));
            this.btnXemTatCa.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnXemTatCa.EnteredBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(165)))), ((int)(((byte)(37)))), ((int)(((byte)(37)))));
            this.btnXemTatCa.EnteredColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(34)))), ((int)(((byte)(37)))));
            this.btnXemTatCa.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.btnXemTatCa.Image = null;
            this.btnXemTatCa.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnXemTatCa.InactiveColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(34)))), ((int)(((byte)(37)))));
            this.btnXemTatCa.Location = new System.Drawing.Point(538, 66);
            this.btnXemTatCa.Name = "btnXemTatCa";
            this.btnXemTatCa.PressedBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(165)))), ((int)(((byte)(37)))), ((int)(((byte)(37)))));
            this.btnXemTatCa.PressedColor = System.Drawing.Color.FromArgb(((int)(((byte)(165)))), ((int)(((byte)(37)))), ((int)(((byte)(37)))));
            this.btnXemTatCa.Size = new System.Drawing.Size(158, 40);
            this.btnXemTatCa.TabIndex = 13;
            this.btnXemTatCa.Text = "Xem tất cả";
            this.btnXemTatCa.TextAlignment = System.Drawing.StringAlignment.Center;
            this.btnXemTatCa.Click += new System.EventHandler(this.btnXemTatCa_Click);
            // 
            // dgvBangLuong
            // 
            this.dgvBangLuong.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvBangLuong.Location = new System.Drawing.Point(8, 128);
            this.dgvBangLuong.Name = "dgvBangLuong";
            this.dgvBangLuong.ReadOnly = true;
            this.dgvBangLuong.RowHeadersWidth = 62;
            this.dgvBangLuong.RowTemplate.Height = 28;
            this.dgvBangLuong.Size = new System.Drawing.Size(1487, 298);
            this.dgvBangLuong.TabIndex = 14;
            // 
            // txtKyFilter
            // 
            this.txtKyFilter.Location = new System.Drawing.Point(70, 87);
            this.txtKyFilter.Name = "txtKyFilter";
            this.txtKyFilter.Size = new System.Drawing.Size(171, 26);
            this.txtKyFilter.TabIndex = 15;
            // 
            // btnExportExcel
            // 
            this.btnExportExcel.BackColor = System.Drawing.Color.Transparent;
            this.btnExportExcel.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(34)))), ((int)(((byte)(37)))));
            this.btnExportExcel.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnExportExcel.EnteredBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(165)))), ((int)(((byte)(37)))), ((int)(((byte)(37)))));
            this.btnExportExcel.EnteredColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(34)))), ((int)(((byte)(37)))));
            this.btnExportExcel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.btnExportExcel.Image = null;
            this.btnExportExcel.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnExportExcel.InactiveColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(34)))), ((int)(((byte)(37)))));
            this.btnExportExcel.Location = new System.Drawing.Point(561, 461);
            this.btnExportExcel.Name = "btnExportExcel";
            this.btnExportExcel.PressedBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(165)))), ((int)(((byte)(37)))), ((int)(((byte)(37)))));
            this.btnExportExcel.PressedColor = System.Drawing.Color.FromArgb(((int)(((byte)(165)))), ((int)(((byte)(37)))), ((int)(((byte)(37)))));
            this.btnExportExcel.Size = new System.Drawing.Size(184, 40);
            this.btnExportExcel.TabIndex = 16;
            this.btnExportExcel.Text = "Xuất File Excel";
            this.btnExportExcel.TextAlignment = System.Drawing.StringAlignment.Center;
            this.btnExportExcel.Click += new System.EventHandler(this.btnExportExcel_Click);
            // 
            // formDanhSachTinhLuong
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1507, 513);
            this.Controls.Add(this.btnExportExcel);
            this.Controls.Add(this.txtKyFilter);
            this.Controls.Add(this.dgvBangLuong);
            this.Controls.Add(this.btnXemTatCa);
            this.Controls.Add(this.btnXem);
            this.Controls.Add(this.label5);
            this.Name = "formDanhSachTinhLuong";
            this.Text = "formDanhSachTinhLuong";
            this.Load += new System.EventHandler(this.formDanhSachTinhLuong_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvBangLuong)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label5;
        private ReaLTaiizor.Controls.Button btnXem;
        private ReaLTaiizor.Controls.Button btnXemTatCa;
        private System.Windows.Forms.DataGridView dgvBangLuong;
        private System.Windows.Forms.TextBox txtKyFilter;
        private ReaLTaiizor.Controls.Button btnExportExcel;
    }
}