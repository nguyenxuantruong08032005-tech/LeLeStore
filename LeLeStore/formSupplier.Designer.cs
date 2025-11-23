namespace LeLeStore
{
    partial class formSupplier
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
            this.components = new System.ComponentModel.Container();
            this.label1 = new System.Windows.Forms.Label();
            this.txtDiaChiNCC = new System.Windows.Forms.TextBox();
            this.txtSdtNCC = new System.Windows.Forms.TextBox();
            this.txtTenNCC = new System.Windows.Forms.TextBox();
            this.txtMaNCC = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.btnThem = new ReaLTaiizor.Controls.Button();
            this.btnXoa = new ReaLTaiizor.Controls.Button();
            this.btnSua = new ReaLTaiizor.Controls.Button();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.maNCCDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tenNCCDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.soDienThoaiDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.diaChiDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.maNhanVienDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.nhaCungCapBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.gStoreDataSet = new LeLeStore.GStoreDataSet();
            this.nhaCungCapTableAdapter = new LeLeStore.GStoreDataSetTableAdapters.NhaCungCapTableAdapter();
            this.btnHuy = new ReaLTaiizor.Controls.Button();
            this.cboMaNV = new System.Windows.Forms.ComboBox();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nhaCungCapBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gStoreDataSet)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 26F, System.Drawing.FontStyle.Bold);
            this.label1.Location = new System.Drawing.Point(379, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(664, 70);
            this.label1.TabIndex = 0;
            this.label1.Text = "QUẢN LÝ NHÀ CUNG CẤP";
            // 
            // txtDiaChiNCC
            // 
            this.txtDiaChiNCC.Location = new System.Drawing.Point(590, 195);
            this.txtDiaChiNCC.Name = "txtDiaChiNCC";
            this.txtDiaChiNCC.Size = new System.Drawing.Size(214, 26);
            this.txtDiaChiNCC.TabIndex = 24;
            // 
            // txtSdtNCC
            // 
            this.txtSdtNCC.Location = new System.Drawing.Point(590, 127);
            this.txtSdtNCC.Name = "txtSdtNCC";
            this.txtSdtNCC.Size = new System.Drawing.Size(197, 26);
            this.txtSdtNCC.TabIndex = 23;
            // 
            // txtTenNCC
            // 
            this.txtTenNCC.Location = new System.Drawing.Point(275, 201);
            this.txtTenNCC.Name = "txtTenNCC";
            this.txtTenNCC.Size = new System.Drawing.Size(229, 26);
            this.txtTenNCC.TabIndex = 22;
            // 
            // txtMaNCC
            // 
            this.txtMaNCC.Location = new System.Drawing.Point(275, 127);
            this.txtMaNCC.Name = "txtMaNCC";
            this.txtMaNCC.Size = new System.Drawing.Size(95, 26);
            this.txtMaNCC.TabIndex = 21;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            this.label5.Location = new System.Drawing.Point(586, 168);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(84, 24);
            this.label5.TabIndex = 20;
            this.label5.Text = "Địa Chỉ:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            this.label4.Location = new System.Drawing.Point(586, 100);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(149, 24);
            this.label4.TabIndex = 19;
            this.label4.Text = "Số Điện Thoại:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            this.label3.Location = new System.Drawing.Point(269, 171);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(195, 24);
            this.label3.TabIndex = 18;
            this.label3.Text = "Tên Nhà Cung Cấp:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(271, 100);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(180, 24);
            this.label2.TabIndex = 17;
            this.label2.Text = "Mã Nhà Cung Cấp";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            this.label6.Location = new System.Drawing.Point(932, 141);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(147, 24);
            this.label6.TabIndex = 25;
            this.label6.Text = "Mã Nhân Viên:";
            // 
            // btnThem
            // 
            this.btnThem.BackColor = System.Drawing.Color.Transparent;
            this.btnThem.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(34)))), ((int)(((byte)(37)))));
            this.btnThem.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnThem.EnteredBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(165)))), ((int)(((byte)(37)))), ((int)(((byte)(37)))));
            this.btnThem.EnteredColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(34)))), ((int)(((byte)(37)))));
            this.btnThem.Font = new System.Drawing.Font("Arial", 16F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnThem.Image = null;
            this.btnThem.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnThem.InactiveColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(34)))), ((int)(((byte)(37)))));
            this.btnThem.Location = new System.Drawing.Point(212, 702);
            this.btnThem.Name = "btnThem";
            this.btnThem.PressedBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(165)))), ((int)(((byte)(37)))), ((int)(((byte)(37)))));
            this.btnThem.PressedColor = System.Drawing.Color.FromArgb(((int)(((byte)(165)))), ((int)(((byte)(37)))), ((int)(((byte)(37)))));
            this.btnThem.Size = new System.Drawing.Size(192, 97);
            this.btnThem.TabIndex = 30;
            this.btnThem.Text = "THÊM";
            this.btnThem.TextAlignment = System.Drawing.StringAlignment.Center;
            this.btnThem.Click += new System.EventHandler(this.btnThem_Click);
            // 
            // btnXoa
            // 
            this.btnXoa.BackColor = System.Drawing.Color.Transparent;
            this.btnXoa.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(34)))), ((int)(((byte)(37)))));
            this.btnXoa.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnXoa.EnteredBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(165)))), ((int)(((byte)(37)))), ((int)(((byte)(37)))));
            this.btnXoa.EnteredColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(34)))), ((int)(((byte)(37)))));
            this.btnXoa.Font = new System.Drawing.Font("Arial", 16F, System.Drawing.FontStyle.Bold);
            this.btnXoa.Image = null;
            this.btnXoa.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnXoa.InactiveColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(34)))), ((int)(((byte)(37)))));
            this.btnXoa.Location = new System.Drawing.Point(813, 693);
            this.btnXoa.Name = "btnXoa";
            this.btnXoa.PressedBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(165)))), ((int)(((byte)(37)))), ((int)(((byte)(37)))));
            this.btnXoa.PressedColor = System.Drawing.Color.FromArgb(((int)(((byte)(165)))), ((int)(((byte)(37)))), ((int)(((byte)(37)))));
            this.btnXoa.Size = new System.Drawing.Size(192, 97);
            this.btnXoa.TabIndex = 28;
            this.btnXoa.Text = "XÓA";
            this.btnXoa.TextAlignment = System.Drawing.StringAlignment.Center;
            this.btnXoa.Click += new System.EventHandler(this.btnXoa_Click);
            // 
            // btnSua
            // 
            this.btnSua.BackColor = System.Drawing.Color.Transparent;
            this.btnSua.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(34)))), ((int)(((byte)(37)))));
            this.btnSua.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSua.EnteredBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(165)))), ((int)(((byte)(37)))), ((int)(((byte)(37)))));
            this.btnSua.EnteredColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(34)))), ((int)(((byte)(37)))));
            this.btnSua.Font = new System.Drawing.Font("Arial", 16F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSua.Image = null;
            this.btnSua.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnSua.InactiveColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(34)))), ((int)(((byte)(37)))));
            this.btnSua.Location = new System.Drawing.Point(516, 693);
            this.btnSua.Name = "btnSua";
            this.btnSua.PressedBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(165)))), ((int)(((byte)(37)))), ((int)(((byte)(37)))));
            this.btnSua.PressedColor = System.Drawing.Color.FromArgb(((int)(((byte)(165)))), ((int)(((byte)(37)))), ((int)(((byte)(37)))));
            this.btnSua.Size = new System.Drawing.Size(192, 97);
            this.btnSua.TabIndex = 27;
            this.btnSua.Text = "SỬA";
            this.btnSua.TextAlignment = System.Drawing.StringAlignment.Center;
            this.btnSua.Click += new System.EventHandler(this.btnSua_Click);
            // 
            // dataGridView1
            // 
            this.dataGridView1.AutoGenerateColumns = false;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.maNCCDataGridViewTextBoxColumn,
            this.tenNCCDataGridViewTextBoxColumn,
            this.soDienThoaiDataGridViewTextBoxColumn,
            this.diaChiDataGridViewTextBoxColumn,
            this.maNhanVienDataGridViewTextBoxColumn});
            this.dataGridView1.DataSource = this.nhaCungCapBindingSource;
            this.dataGridView1.Location = new System.Drawing.Point(156, 251);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowHeadersWidth = 62;
            this.dataGridView1.RowTemplate.Height = 28;
            this.dataGridView1.Size = new System.Drawing.Size(1145, 379);
            this.dataGridView1.TabIndex = 31;
            this.dataGridView1.SelectionChanged += new System.EventHandler(this.dataGridView1_SelectionChanged);
            // 
            // maNCCDataGridViewTextBoxColumn
            // 
            this.maNCCDataGridViewTextBoxColumn.DataPropertyName = "MaNCC";
            this.maNCCDataGridViewTextBoxColumn.HeaderText = "MaNCC";
            this.maNCCDataGridViewTextBoxColumn.MinimumWidth = 8;
            this.maNCCDataGridViewTextBoxColumn.Name = "maNCCDataGridViewTextBoxColumn";
            this.maNCCDataGridViewTextBoxColumn.ReadOnly = true;
            this.maNCCDataGridViewTextBoxColumn.Width = 150;
            // 
            // tenNCCDataGridViewTextBoxColumn
            // 
            this.tenNCCDataGridViewTextBoxColumn.DataPropertyName = "TenNCC";
            this.tenNCCDataGridViewTextBoxColumn.HeaderText = "TenNCC";
            this.tenNCCDataGridViewTextBoxColumn.MinimumWidth = 8;
            this.tenNCCDataGridViewTextBoxColumn.Name = "tenNCCDataGridViewTextBoxColumn";
            this.tenNCCDataGridViewTextBoxColumn.Width = 150;
            // 
            // soDienThoaiDataGridViewTextBoxColumn
            // 
            this.soDienThoaiDataGridViewTextBoxColumn.DataPropertyName = "SoDienThoai";
            this.soDienThoaiDataGridViewTextBoxColumn.HeaderText = "SoDienThoai";
            this.soDienThoaiDataGridViewTextBoxColumn.MinimumWidth = 8;
            this.soDienThoaiDataGridViewTextBoxColumn.Name = "soDienThoaiDataGridViewTextBoxColumn";
            this.soDienThoaiDataGridViewTextBoxColumn.Width = 150;
            // 
            // diaChiDataGridViewTextBoxColumn
            // 
            this.diaChiDataGridViewTextBoxColumn.DataPropertyName = "DiaChi";
            this.diaChiDataGridViewTextBoxColumn.HeaderText = "DiaChi";
            this.diaChiDataGridViewTextBoxColumn.MinimumWidth = 8;
            this.diaChiDataGridViewTextBoxColumn.Name = "diaChiDataGridViewTextBoxColumn";
            this.diaChiDataGridViewTextBoxColumn.Width = 150;
            // 
            // maNhanVienDataGridViewTextBoxColumn
            // 
            this.maNhanVienDataGridViewTextBoxColumn.DataPropertyName = "MaNhanVien";
            this.maNhanVienDataGridViewTextBoxColumn.HeaderText = "MaNhanVien";
            this.maNhanVienDataGridViewTextBoxColumn.MinimumWidth = 8;
            this.maNhanVienDataGridViewTextBoxColumn.Name = "maNhanVienDataGridViewTextBoxColumn";
            this.maNhanVienDataGridViewTextBoxColumn.Width = 150;
            // 
            // nhaCungCapBindingSource
            // 
            this.nhaCungCapBindingSource.DataMember = "NhaCungCap";
            this.nhaCungCapBindingSource.DataSource = this.gStoreDataSet;
            // 
            // gStoreDataSet
            // 
            this.gStoreDataSet.DataSetName = "GStoreDataSet";
            this.gStoreDataSet.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema;
            // 
            // nhaCungCapTableAdapter
            // 
            this.nhaCungCapTableAdapter.ClearBeforeFill = true;
            // 
            // btnHuy
            // 
            this.btnHuy.BackColor = System.Drawing.Color.Transparent;
            this.btnHuy.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(34)))), ((int)(((byte)(37)))));
            this.btnHuy.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnHuy.EnteredBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(165)))), ((int)(((byte)(37)))), ((int)(((byte)(37)))));
            this.btnHuy.EnteredColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(34)))), ((int)(((byte)(37)))));
            this.btnHuy.Font = new System.Drawing.Font("Arial", 16F, System.Drawing.FontStyle.Bold);
            this.btnHuy.Image = null;
            this.btnHuy.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnHuy.InactiveColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(34)))), ((int)(((byte)(37)))));
            this.btnHuy.Location = new System.Drawing.Point(1094, 693);
            this.btnHuy.Name = "btnHuy";
            this.btnHuy.PressedBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(165)))), ((int)(((byte)(37)))), ((int)(((byte)(37)))));
            this.btnHuy.PressedColor = System.Drawing.Color.FromArgb(((int)(((byte)(165)))), ((int)(((byte)(37)))), ((int)(((byte)(37)))));
            this.btnHuy.Size = new System.Drawing.Size(192, 97);
            this.btnHuy.TabIndex = 32;
            this.btnHuy.Text = "HỦY";
            this.btnHuy.TextAlignment = System.Drawing.StringAlignment.Center;
            this.btnHuy.Click += new System.EventHandler(this.btnHuy_Click);
            // 
            // cboMaNV
            // 
            this.cboMaNV.FormattingEnabled = true;
            this.cboMaNV.Location = new System.Drawing.Point(936, 171);
            this.cboMaNV.Name = "cboMaNV";
            this.cboMaNV.Size = new System.Drawing.Size(295, 28);
            this.cboMaNV.TabIndex = 33;
            // 
            // formSupplier
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1486, 779);
            this.Controls.Add(this.cboMaNV);
            this.Controls.Add(this.btnHuy);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.btnThem);
            this.Controls.Add(this.btnXoa);
            this.Controls.Add(this.btnSua);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.txtDiaChiNCC);
            this.Controls.Add(this.txtSdtNCC);
            this.Controls.Add(this.txtTenNCC);
            this.Controls.Add(this.txtMaNCC);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "formSupplier";
            this.Text = "formSupplier";
            this.Load += new System.EventHandler(this.formSupplier_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nhaCungCapBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gStoreDataSet)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtDiaChiNCC;
        private System.Windows.Forms.TextBox txtSdtNCC;
        private System.Windows.Forms.TextBox txtTenNCC;
        private System.Windows.Forms.TextBox txtMaNCC;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label6;
        private ReaLTaiizor.Controls.Button btnThem;
      
        private ReaLTaiizor.Controls.Button btnXoa;
        private ReaLTaiizor.Controls.Button btnSua;
        private System.Windows.Forms.DataGridView dataGridView1;
        private GStoreDataSet gStoreDataSet;
        private System.Windows.Forms.BindingSource nhaCungCapBindingSource;
        private GStoreDataSetTableAdapters.NhaCungCapTableAdapter nhaCungCapTableAdapter;
        private System.Windows.Forms.DataGridViewTextBoxColumn maNCCDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn tenNCCDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn soDienThoaiDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn diaChiDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn maNhanVienDataGridViewTextBoxColumn;
        private ReaLTaiizor.Controls.Button btnHuy;
        private System.Windows.Forms.ComboBox cboMaNV;
    }
}