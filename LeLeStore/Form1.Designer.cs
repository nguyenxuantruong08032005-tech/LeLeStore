namespace LeLeStore
{
    partial class Form1
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.sidebar = new System.Windows.Forms.Panel();
            this.btnHam = new System.Windows.Forms.PictureBox();
            this.nightControlBox1 = new ReaLTaiizor.Controls.NightControlBox();
            this.label1 = new System.Windows.Forms.Label();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.pUser = new System.Windows.Forms.Panel();
            this.button3 = new System.Windows.Forms.Button();
            this.pnClient = new System.Windows.Forms.Panel();
            this.btnStaff = new System.Windows.Forms.Button();
            this.pnProduct = new System.Windows.Forms.Panel();
            this.button4 = new System.Windows.Forms.Button();
            this.pnSupplier = new System.Windows.Forms.Panel();
            this.btnSupplier = new System.Windows.Forms.Button();
            this.menuContainer2 = new System.Windows.Forms.FlowLayoutPanel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.menu2 = new System.Windows.Forms.Button();
            this.panel3 = new System.Windows.Forms.Panel();
            this.ptnTonKho = new System.Windows.Forms.Button();
            this.panel4 = new System.Windows.Forms.Panel();
            this.btnPN = new System.Windows.Forms.Button();
            this.panel5 = new System.Windows.Forms.Panel();
            this.btnPX = new System.Windows.Forms.Button();
            this.pnPayMent = new System.Windows.Forms.Panel();
            this.btnPayMent = new System.Windows.Forms.Button();
            this.menuContainer = new System.Windows.Forms.FlowLayoutPanel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.menu = new System.Windows.Forms.Button();
            this.panel8 = new System.Windows.Forms.Panel();
            this.btnUpdate = new System.Windows.Forms.Button();
            this.panel6 = new System.Windows.Forms.Panel();
            this.btnPoint = new System.Windows.Forms.Button();
            this.pnEmployeeSalary = new System.Windows.Forms.Panel();
            this.btnEmployeeSalary = new System.Windows.Forms.Button();
            this.pnDashDoard = new System.Windows.Forms.Panel();
            this.btnDashBoard = new System.Windows.Forms.Button();
            this.pnLogout = new System.Windows.Forms.Panel();
            this.button5 = new System.Windows.Forms.Button();
            this.menuTransition = new System.Windows.Forms.Timer(this.components);
            this.sidebarTransition = new System.Windows.Forms.Timer(this.components);
            this.menuTransition2 = new System.Windows.Forms.Timer(this.components);
            this.sidebar.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.btnHam)).BeginInit();
            this.flowLayoutPanel1.SuspendLayout();
            this.pUser.SuspendLayout();
            this.pnClient.SuspendLayout();
            this.pnProduct.SuspendLayout();
            this.pnSupplier.SuspendLayout();
            this.menuContainer2.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel3.SuspendLayout();
            this.panel4.SuspendLayout();
            this.panel5.SuspendLayout();
            this.pnPayMent.SuspendLayout();
            this.menuContainer.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panel8.SuspendLayout();
            this.panel6.SuspendLayout();
            this.pnEmployeeSalary.SuspendLayout();
            this.pnDashDoard.SuspendLayout();
            this.pnLogout.SuspendLayout();
            this.SuspendLayout();
            // 
            // sidebar
            // 
            this.sidebar.BackColor = System.Drawing.Color.White;
            this.sidebar.Controls.Add(this.btnHam);
            this.sidebar.Controls.Add(this.nightControlBox1);
            this.sidebar.Controls.Add(this.label1);
            this.sidebar.Dock = System.Windows.Forms.DockStyle.Top;
            this.sidebar.Location = new System.Drawing.Point(0, 0);
            this.sidebar.Name = "sidebar";
            this.sidebar.Size = new System.Drawing.Size(1189, 53);
            this.sidebar.TabIndex = 0;
            // 
            // btnHam
            // 
            this.btnHam.Image = ((System.Drawing.Image)(resources.GetObject("btnHam.Image")));
            this.btnHam.Location = new System.Drawing.Point(0, 7);
            this.btnHam.Name = "btnHam";
            this.btnHam.Size = new System.Drawing.Size(65, 40);
            this.btnHam.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.btnHam.TabIndex = 7;
            this.btnHam.TabStop = false;
            this.btnHam.Click += new System.EventHandler(this.btnHam_Click);
            // 
            // nightControlBox1
            // 
            this.nightControlBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.nightControlBox1.BackColor = System.Drawing.Color.Transparent;
            this.nightControlBox1.CloseHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(199)))), ((int)(((byte)(80)))), ((int)(((byte)(80)))));
            this.nightControlBox1.CloseHoverForeColor = System.Drawing.Color.White;
            this.nightControlBox1.Cursor = System.Windows.Forms.Cursors.Hand;
            this.nightControlBox1.DefaultLocation = true;
            this.nightControlBox1.DisableMaximizeColor = System.Drawing.Color.FromArgb(((int)(((byte)(105)))), ((int)(((byte)(105)))), ((int)(((byte)(105)))));
            this.nightControlBox1.DisableMinimizeColor = System.Drawing.Color.FromArgb(((int)(((byte)(105)))), ((int)(((byte)(105)))), ((int)(((byte)(105)))));
            this.nightControlBox1.EnableCloseColor = System.Drawing.Color.FromArgb(((int)(((byte)(160)))), ((int)(((byte)(160)))), ((int)(((byte)(160)))));
            this.nightControlBox1.EnableMaximizeButton = true;
            this.nightControlBox1.EnableMaximizeColor = System.Drawing.Color.FromArgb(((int)(((byte)(160)))), ((int)(((byte)(160)))), ((int)(((byte)(160)))));
            this.nightControlBox1.EnableMinimizeButton = true;
            this.nightControlBox1.EnableMinimizeColor = System.Drawing.Color.FromArgb(((int)(((byte)(160)))), ((int)(((byte)(160)))), ((int)(((byte)(160)))));
            this.nightControlBox1.Location = new System.Drawing.Point(1050, 0);
            this.nightControlBox1.MaximizeHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.nightControlBox1.MaximizeHoverForeColor = System.Drawing.Color.White;
            this.nightControlBox1.MinimizeHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.nightControlBox1.MinimizeHoverForeColor = System.Drawing.Color.White;
            this.nightControlBox1.Name = "nightControlBox1";
            this.nightControlBox1.Size = new System.Drawing.Size(139, 31);
            this.nightControlBox1.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(71, 2);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(158, 45);
            this.label1.TabIndex = 2;
            this.label1.Text = "LeLeStore";
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(24)))), ((int)(((byte)(29)))));
            this.flowLayoutPanel1.Controls.Add(this.pUser);
            this.flowLayoutPanel1.Controls.Add(this.pnClient);
            this.flowLayoutPanel1.Controls.Add(this.pnProduct);
            this.flowLayoutPanel1.Controls.Add(this.pnSupplier);
            this.flowLayoutPanel1.Controls.Add(this.menuContainer2);
            this.flowLayoutPanel1.Controls.Add(this.pnPayMent);
            this.flowLayoutPanel1.Controls.Add(this.menuContainer);
            this.flowLayoutPanel1.Controls.Add(this.pnEmployeeSalary);
            this.flowLayoutPanel1.Controls.Add(this.pnDashDoard);
            this.flowLayoutPanel1.Controls.Add(this.pnLogout);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Left;
            this.flowLayoutPanel1.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(0, 53);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Padding = new System.Windows.Forms.Padding(0, 30, 0, 0);
            this.flowLayoutPanel1.Size = new System.Drawing.Size(251, 638);
            this.flowLayoutPanel1.TabIndex = 1;
            // 
            // pUser
            // 
            this.pUser.Controls.Add(this.button3);
            this.pUser.Location = new System.Drawing.Point(0, 30);
            this.pUser.Margin = new System.Windows.Forms.Padding(0);
            this.pUser.Name = "pUser";
            this.pUser.Size = new System.Drawing.Size(248, 53);
            this.pUser.TabIndex = 5;
            // 
            // button3
            // 
            this.button3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(24)))), ((int)(((byte)(29)))));
            this.button3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.button3.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(24)))), ((int)(((byte)(29)))));
            this.button3.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button3.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button3.ForeColor = System.Drawing.Color.White;
            this.button3.Image = ((System.Drawing.Image)(resources.GetObject("button3.Image")));
            this.button3.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.button3.Location = new System.Drawing.Point(0, 0);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(248, 53);
            this.button3.TabIndex = 4;
            this.button3.Text = "             Người Dùng";
            this.button3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.button3.UseVisualStyleBackColor = false;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // pnClient
            // 
            this.pnClient.Controls.Add(this.btnStaff);
            this.pnClient.Location = new System.Drawing.Point(0, 83);
            this.pnClient.Margin = new System.Windows.Forms.Padding(0);
            this.pnClient.Name = "pnClient";
            this.pnClient.Size = new System.Drawing.Size(248, 53);
            this.pnClient.TabIndex = 6;
            // 
            // btnStaff
            // 
            this.btnStaff.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(24)))), ((int)(((byte)(29)))));
            this.btnStaff.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnStaff.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(24)))), ((int)(((byte)(29)))));
            this.btnStaff.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnStaff.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnStaff.ForeColor = System.Drawing.Color.White;
            this.btnStaff.Image = ((System.Drawing.Image)(resources.GetObject("btnStaff.Image")));
            this.btnStaff.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnStaff.Location = new System.Drawing.Point(0, 0);
            this.btnStaff.Name = "btnStaff";
            this.btnStaff.Size = new System.Drawing.Size(248, 53);
            this.btnStaff.TabIndex = 4;
            this.btnStaff.Text = "             Nhân Viên";
            this.btnStaff.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnStaff.UseVisualStyleBackColor = false;
            this.btnStaff.Click += new System.EventHandler(this.btnStaff_Click);
            // 
            // pnProduct
            // 
            this.pnProduct.Controls.Add(this.button4);
            this.pnProduct.Location = new System.Drawing.Point(0, 136);
            this.pnProduct.Margin = new System.Windows.Forms.Padding(0);
            this.pnProduct.Name = "pnProduct";
            this.pnProduct.Size = new System.Drawing.Size(248, 53);
            this.pnProduct.TabIndex = 5;
            // 
            // button4
            // 
            this.button4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(24)))), ((int)(((byte)(29)))));
            this.button4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.button4.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(24)))), ((int)(((byte)(29)))));
            this.button4.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button4.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button4.ForeColor = System.Drawing.Color.White;
            this.button4.Image = ((System.Drawing.Image)(resources.GetObject("button4.Image")));
            this.button4.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.button4.Location = new System.Drawing.Point(0, 0);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(248, 53);
            this.button4.TabIndex = 4;
            this.button4.Text = "             Hàng hóa";
            this.button4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.button4.UseVisualStyleBackColor = false;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // pnSupplier
            // 
            this.pnSupplier.Controls.Add(this.btnSupplier);
            this.pnSupplier.Location = new System.Drawing.Point(0, 189);
            this.pnSupplier.Margin = new System.Windows.Forms.Padding(0);
            this.pnSupplier.Name = "pnSupplier";
            this.pnSupplier.Size = new System.Drawing.Size(248, 53);
            this.pnSupplier.TabIndex = 7;
            // 
            // btnSupplier
            // 
            this.btnSupplier.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(24)))), ((int)(((byte)(29)))));
            this.btnSupplier.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnSupplier.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(24)))), ((int)(((byte)(29)))));
            this.btnSupplier.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSupplier.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSupplier.ForeColor = System.Drawing.Color.White;
            this.btnSupplier.Image = ((System.Drawing.Image)(resources.GetObject("btnSupplier.Image")));
            this.btnSupplier.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnSupplier.Location = new System.Drawing.Point(0, 0);
            this.btnSupplier.Name = "btnSupplier";
            this.btnSupplier.Size = new System.Drawing.Size(248, 53);
            this.btnSupplier.TabIndex = 4;
            this.btnSupplier.Text = "             Nhà Cung Cấp";
            this.btnSupplier.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnSupplier.UseVisualStyleBackColor = false;
            this.btnSupplier.Click += new System.EventHandler(this.btnSupplier_Click);
            // 
            // menuContainer2
            // 
            this.menuContainer2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(33)))), ((int)(((byte)(36)))));
            this.menuContainer2.Controls.Add(this.panel2);
            this.menuContainer2.Controls.Add(this.panel3);
            this.menuContainer2.Controls.Add(this.panel4);
            this.menuContainer2.Controls.Add(this.panel5);
            this.menuContainer2.Location = new System.Drawing.Point(0, 242);
            this.menuContainer2.Margin = new System.Windows.Forms.Padding(0);
            this.menuContainer2.Name = "menuContainer2";
            this.menuContainer2.Size = new System.Drawing.Size(248, 55);
            this.menuContainer2.TabIndex = 9;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.menu2);
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Margin = new System.Windows.Forms.Padding(0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(330, 62);
            this.panel2.TabIndex = 5;
            // 
            // menu2
            // 
            this.menu2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(24)))), ((int)(((byte)(29)))));
            this.menu2.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(24)))), ((int)(((byte)(29)))));
            this.menu2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.menu2.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.menu2.ForeColor = System.Drawing.Color.White;
            this.menu2.Image = ((System.Drawing.Image)(resources.GetObject("menu2.Image")));
            this.menu2.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.menu2.Location = new System.Drawing.Point(0, -46);
            this.menu2.Name = "menu2";
            this.menu2.Size = new System.Drawing.Size(326, 151);
            this.menu2.TabIndex = 4;
            this.menu2.Text = "             Kho Hàng";
            this.menu2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.menu2.UseVisualStyleBackColor = false;
            this.menu2.Click += new System.EventHandler(this.menu2_Click_1);
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.ptnTonKho);
            this.panel3.Location = new System.Drawing.Point(0, 62);
            this.panel3.Margin = new System.Windows.Forms.Padding(0);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(248, 53);
            this.panel3.TabIndex = 7;
            // 
            // ptnTonKho
            // 
            this.ptnTonKho.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(33)))), ((int)(((byte)(36)))));
            this.ptnTonKho.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(24)))), ((int)(((byte)(29)))));
            this.ptnTonKho.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ptnTonKho.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ptnTonKho.ForeColor = System.Drawing.Color.White;
            this.ptnTonKho.Image = ((System.Drawing.Image)(resources.GetObject("ptnTonKho.Image")));
            this.ptnTonKho.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.ptnTonKho.Location = new System.Drawing.Point(0, -46);
            this.ptnTonKho.Name = "ptnTonKho";
            this.ptnTonKho.Size = new System.Drawing.Size(326, 120);
            this.ptnTonKho.TabIndex = 4;
            this.ptnTonKho.Text = "              Tồn Kho";
            this.ptnTonKho.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.ptnTonKho.UseVisualStyleBackColor = false;
            // 
            // panel4
            // 
            this.panel4.Controls.Add(this.btnPN);
            this.panel4.Location = new System.Drawing.Point(0, 115);
            this.panel4.Margin = new System.Windows.Forms.Padding(0);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(248, 53);
            this.panel4.TabIndex = 8;
            // 
            // btnPN
            // 
            this.btnPN.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(33)))), ((int)(((byte)(36)))));
            this.btnPN.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnPN.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(24)))), ((int)(((byte)(29)))));
            this.btnPN.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnPN.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnPN.ForeColor = System.Drawing.Color.White;
            this.btnPN.Image = ((System.Drawing.Image)(resources.GetObject("btnPN.Image")));
            this.btnPN.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnPN.Location = new System.Drawing.Point(0, 0);
            this.btnPN.Name = "btnPN";
            this.btnPN.Size = new System.Drawing.Size(248, 53);
            this.btnPN.TabIndex = 4;
            this.btnPN.Text = "              Phiếu Nhập";
            this.btnPN.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnPN.UseVisualStyleBackColor = false;
            // 
            // panel5
            // 
            this.panel5.Controls.Add(this.btnPX);
            this.panel5.Location = new System.Drawing.Point(0, 168);
            this.panel5.Margin = new System.Windows.Forms.Padding(0);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(248, 53);
            this.panel5.TabIndex = 9;
            // 
            // btnPX
            // 
            this.btnPX.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(33)))), ((int)(((byte)(36)))));
            this.btnPX.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnPX.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(24)))), ((int)(((byte)(29)))));
            this.btnPX.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnPX.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnPX.ForeColor = System.Drawing.Color.White;
            this.btnPX.Image = ((System.Drawing.Image)(resources.GetObject("btnPX.Image")));
            this.btnPX.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnPX.Location = new System.Drawing.Point(0, 0);
            this.btnPX.Name = "btnPX";
            this.btnPX.Size = new System.Drawing.Size(248, 53);
            this.btnPX.TabIndex = 4;
            this.btnPX.Text = "              Phiếu Xuất";
            this.btnPX.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnPX.UseVisualStyleBackColor = false;
            // 
            // pnPayMent
            // 
            this.pnPayMent.Controls.Add(this.btnPayMent);
            this.pnPayMent.Location = new System.Drawing.Point(0, 297);
            this.pnPayMent.Margin = new System.Windows.Forms.Padding(0);
            this.pnPayMent.Name = "pnPayMent";
            this.pnPayMent.Size = new System.Drawing.Size(248, 53);
            this.pnPayMent.TabIndex = 9;
            // 
            // btnPayMent
            // 
            this.btnPayMent.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(24)))), ((int)(((byte)(29)))));
            this.btnPayMent.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnPayMent.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(24)))), ((int)(((byte)(29)))));
            this.btnPayMent.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnPayMent.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnPayMent.ForeColor = System.Drawing.Color.White;
            this.btnPayMent.Image = ((System.Drawing.Image)(resources.GetObject("btnPayMent.Image")));
            this.btnPayMent.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnPayMent.Location = new System.Drawing.Point(0, 0);
            this.btnPayMent.Name = "btnPayMent";
            this.btnPayMent.Size = new System.Drawing.Size(248, 53);
            this.btnPayMent.TabIndex = 4;
            this.btnPayMent.Text = "             Thanh Toán";
            this.btnPayMent.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnPayMent.UseVisualStyleBackColor = false;
            this.btnPayMent.Click += new System.EventHandler(this.btnPayMent_Click);
            // 
            // menuContainer
            // 
            this.menuContainer.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(33)))), ((int)(((byte)(36)))));
            this.menuContainer.Controls.Add(this.panel1);
            this.menuContainer.Controls.Add(this.panel8);
            this.menuContainer.Controls.Add(this.panel6);
            this.menuContainer.Location = new System.Drawing.Point(0, 350);
            this.menuContainer.Margin = new System.Windows.Forms.Padding(0);
            this.menuContainer.Name = "menuContainer";
            this.menuContainer.Size = new System.Drawing.Size(248, 53);
            this.menuContainer.TabIndex = 6;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.menu);
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Margin = new System.Windows.Forms.Padding(0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(330, 62);
            this.panel1.TabIndex = 5;
            // 
            // menu
            // 
            this.menu.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(24)))), ((int)(((byte)(29)))));
            this.menu.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(24)))), ((int)(((byte)(29)))));
            this.menu.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.menu.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.menu.ForeColor = System.Drawing.Color.White;
            this.menu.Image = ((System.Drawing.Image)(resources.GetObject("menu.Image")));
            this.menu.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.menu.Location = new System.Drawing.Point(0, -46);
            this.menu.Name = "menu";
            this.menu.Size = new System.Drawing.Size(326, 151);
            this.menu.TabIndex = 4;
            this.menu.Text = "             Khách Hàng";
            this.menu.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.menu.UseVisualStyleBackColor = false;
            this.menu.Click += new System.EventHandler(this.menu_Click);
            // 
            // panel8
            // 
            this.panel8.Controls.Add(this.btnUpdate);
            this.panel8.Location = new System.Drawing.Point(0, 62);
            this.panel8.Margin = new System.Windows.Forms.Padding(0);
            this.panel8.Name = "panel8";
            this.panel8.Size = new System.Drawing.Size(248, 53);
            this.panel8.TabIndex = 7;
            // 
            // btnUpdate
            // 
            this.btnUpdate.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(33)))), ((int)(((byte)(36)))));
            this.btnUpdate.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(24)))), ((int)(((byte)(29)))));
            this.btnUpdate.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnUpdate.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnUpdate.ForeColor = System.Drawing.Color.White;
            this.btnUpdate.Image = ((System.Drawing.Image)(resources.GetObject("btnUpdate.Image")));
            this.btnUpdate.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnUpdate.Location = new System.Drawing.Point(0, -46);
            this.btnUpdate.Name = "btnUpdate";
            this.btnUpdate.Size = new System.Drawing.Size(326, 120);
            this.btnUpdate.TabIndex = 4;
            this.btnUpdate.Text = "              Cập Nhật Thông Tin";
            this.btnUpdate.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnUpdate.UseVisualStyleBackColor = false;
            this.btnUpdate.Click += new System.EventHandler(this.btnUpdateClient_Click);
            // 
            // panel6
            // 
            this.panel6.Controls.Add(this.btnPoint);
            this.panel6.Location = new System.Drawing.Point(0, 115);
            this.panel6.Margin = new System.Windows.Forms.Padding(0);
            this.panel6.Name = "panel6";
            this.panel6.Size = new System.Drawing.Size(248, 53);
            this.panel6.TabIndex = 8;
            // 
            // btnPoint
            // 
            this.btnPoint.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(33)))), ((int)(((byte)(36)))));
            this.btnPoint.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnPoint.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(24)))), ((int)(((byte)(29)))));
            this.btnPoint.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnPoint.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnPoint.ForeColor = System.Drawing.Color.White;
            this.btnPoint.Image = ((System.Drawing.Image)(resources.GetObject("btnPoint.Image")));
            this.btnPoint.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnPoint.Location = new System.Drawing.Point(0, 0);
            this.btnPoint.Name = "btnPoint";
            this.btnPoint.Size = new System.Drawing.Size(248, 53);
            this.btnPoint.TabIndex = 4;
            this.btnPoint.Text = "              Tích Điểm";
            this.btnPoint.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnPoint.UseVisualStyleBackColor = false;
            this.btnPoint.Click += new System.EventHandler(this.sbmenu2_Click);
            // 
            // pnEmployeeSalary
            // 
            this.pnEmployeeSalary.Controls.Add(this.btnEmployeeSalary);
            this.pnEmployeeSalary.Location = new System.Drawing.Point(0, 403);
            this.pnEmployeeSalary.Margin = new System.Windows.Forms.Padding(0);
            this.pnEmployeeSalary.Name = "pnEmployeeSalary";
            this.pnEmployeeSalary.Size = new System.Drawing.Size(248, 53);
            this.pnEmployeeSalary.TabIndex = 8;
            // 
            // btnEmployeeSalary
            // 
            this.btnEmployeeSalary.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(24)))), ((int)(((byte)(29)))));
            this.btnEmployeeSalary.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnEmployeeSalary.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(24)))), ((int)(((byte)(29)))));
            this.btnEmployeeSalary.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnEmployeeSalary.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnEmployeeSalary.ForeColor = System.Drawing.Color.White;
            this.btnEmployeeSalary.Image = ((System.Drawing.Image)(resources.GetObject("btnEmployeeSalary.Image")));
            this.btnEmployeeSalary.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnEmployeeSalary.Location = new System.Drawing.Point(0, 0);
            this.btnEmployeeSalary.Name = "btnEmployeeSalary";
            this.btnEmployeeSalary.Size = new System.Drawing.Size(248, 53);
            this.btnEmployeeSalary.TabIndex = 4;
            this.btnEmployeeSalary.Text = "             Tính Lương";
            this.btnEmployeeSalary.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnEmployeeSalary.UseVisualStyleBackColor = false;
            this.btnEmployeeSalary.Click += new System.EventHandler(this.btnEmployeeSalary_Click);
            // 
            // pnDashDoard
            // 
            this.pnDashDoard.Controls.Add(this.btnDashBoard);
            this.pnDashDoard.Location = new System.Drawing.Point(0, 456);
            this.pnDashDoard.Margin = new System.Windows.Forms.Padding(0);
            this.pnDashDoard.Name = "pnDashDoard";
            this.pnDashDoard.Size = new System.Drawing.Size(248, 53);
            this.pnDashDoard.TabIndex = 9;
            // 
            // btnDashBoard
            // 
            this.btnDashBoard.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(24)))), ((int)(((byte)(29)))));
            this.btnDashBoard.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnDashBoard.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(24)))), ((int)(((byte)(29)))));
            this.btnDashBoard.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDashBoard.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnDashBoard.ForeColor = System.Drawing.Color.White;
            this.btnDashBoard.Image = ((System.Drawing.Image)(resources.GetObject("btnDashBoard.Image")));
            this.btnDashBoard.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnDashBoard.Location = new System.Drawing.Point(0, 0);
            this.btnDashBoard.Name = "btnDashBoard";
            this.btnDashBoard.Size = new System.Drawing.Size(248, 53);
            this.btnDashBoard.TabIndex = 4;
            this.btnDashBoard.Text = "             Báo cáo, thống kê";
            this.btnDashBoard.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnDashBoard.UseVisualStyleBackColor = false;
            this.btnDashBoard.Click += new System.EventHandler(this.btnDashBoard_Click);
            // 
            // pnLogout
            // 
            this.pnLogout.Controls.Add(this.button5);
            this.pnLogout.Location = new System.Drawing.Point(0, 509);
            this.pnLogout.Margin = new System.Windows.Forms.Padding(0);
            this.pnLogout.Name = "pnLogout";
            this.pnLogout.Size = new System.Drawing.Size(248, 53);
            this.pnLogout.TabIndex = 6;
            // 
            // button5
            // 
            this.button5.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(24)))), ((int)(((byte)(29)))));
            this.button5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.button5.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(24)))), ((int)(((byte)(29)))));
            this.button5.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button5.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button5.ForeColor = System.Drawing.Color.White;
            this.button5.Image = ((System.Drawing.Image)(resources.GetObject("button5.Image")));
            this.button5.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.button5.Location = new System.Drawing.Point(0, 0);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(248, 53);
            this.button5.TabIndex = 4;
            this.button5.Text = "             Logout";
            this.button5.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.button5.UseVisualStyleBackColor = false;
            this.button5.Click += new System.EventHandler(this.button5_Click);
            // 
            // menuTransition
            // 
            this.menuTransition.Tick += new System.EventHandler(this.menuTransition_Tick);
            // 
            // sidebarTransition
            // 
            this.sidebarTransition.Interval = 10;
            this.sidebarTransition.Tick += new System.EventHandler(this.sidebarTransition_Tick);
            // 
            // menuTransition2
            // 
            this.menuTransition2.Tick += new System.EventHandler(this.menuTransition2_Tick);
            // 
            // Form1
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1189, 691);
            this.Controls.Add(this.flowLayoutPanel1);
            this.Controls.Add(this.sidebar);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.IsMdiContainer = true;
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.sidebar.ResumeLayout(false);
            this.sidebar.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.btnHam)).EndInit();
            this.flowLayoutPanel1.ResumeLayout(false);
            this.pUser.ResumeLayout(false);
            this.pnClient.ResumeLayout(false);
            this.pnProduct.ResumeLayout(false);
            this.pnSupplier.ResumeLayout(false);
            this.menuContainer2.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.panel4.ResumeLayout(false);
            this.panel5.ResumeLayout(false);
            this.pnPayMent.ResumeLayout(false);
            this.menuContainer.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel8.ResumeLayout(false);
            this.panel6.ResumeLayout(false);
            this.pnEmployeeSalary.ResumeLayout(false);
            this.pnDashDoard.ResumeLayout(false);
            this.pnLogout.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel sidebar;
        private System.Windows.Forms.Label label1;
        private ReaLTaiizor.Controls.NightControlBox nightControlBox1;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Timer menuTransition;
        private System.Windows.Forms.Timer sidebarTransition;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button menu;
        private System.Windows.Forms.Panel pUser;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Panel pnProduct;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.FlowLayoutPanel menuContainer;
        private System.Windows.Forms.Panel pnLogout;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.Panel panel8;
        private System.Windows.Forms.Button btnUpdate;
        private System.Windows.Forms.PictureBox btnHam;
        private System.Windows.Forms.Panel panel6;
        private System.Windows.Forms.Button btnPoint;
        private System.Windows.Forms.Panel pnClient;
        private System.Windows.Forms.Button btnStaff;
        private System.Windows.Forms.Panel pnSupplier;
        private System.Windows.Forms.Button btnSupplier;
        private System.Windows.Forms.Panel pnEmployeeSalary;
        private System.Windows.Forms.Button btnEmployeeSalary;
        private System.Windows.Forms.Panel pnDashDoard;
        private System.Windows.Forms.Button btnDashBoard;
        private System.Windows.Forms.Panel pnPayMent;
        private System.Windows.Forms.Button btnPayMent;
        private System.Windows.Forms.FlowLayoutPanel menuContainer2;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Button menu2;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Button ptnTonKho;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Button btnPN;
        private System.Windows.Forms.Timer menuTransition2;
        private System.Windows.Forms.Panel panel5;
        private System.Windows.Forms.Button btnPX;
    }
}

