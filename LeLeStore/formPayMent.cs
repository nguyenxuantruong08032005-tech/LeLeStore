using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
namespace LeLeStore
{
    public partial class formPayMent : Form
    {
        private readonly GStoreDataSet _dataSet = new GStoreDataSet();
        private readonly GStoreDataSetTableAdapters.SanPhamTableAdapter _sanPhamTableAdapter = new GStoreDataSetTableAdapters.SanPhamTableAdapter();
        private readonly GStoreDataSetTableAdapters.LoaiSPTableAdapter _loaiSpTableAdapter = new GStoreDataSetTableAdapters.LoaiSPTableAdapter();
        private readonly GStoreDataSetTableAdapters.NhanVienTableAdapter _nhanVienTableAdapter = new GStoreDataSetTableAdapters.NhanVienTableAdapter();
        private readonly GStoreDataSetTableAdapters.NguoiDungTableAdapter _nguoiDungTableAdapter = new GStoreDataSetTableAdapters.NguoiDungTableAdapter();
        private readonly Dictionary<string, Image> _imageCache = new Dictionary<string, Image>(StringComparer.OrdinalIgnoreCase);
        private readonly List<Image> _productImageClones = new List<Image>();
        private readonly CultureInfo _currencyCulture = CultureInfo.GetCultureInfo("vi-VN");
        private readonly string _username;
        private int? _currentEmployeeId;
        private DataTable _invoiceTable;
        private decimal _loyaltyDiscountAmount;
        private static DataTable _savedInvoiceSnapshot;
        private static decimal _savedLoyaltyDiscount;

        private sealed class ProductDisplayInfo
        {
            public int Id { get; set; }

            public string Name { get; set; } = string.Empty;

            public decimal Price { get; set; }

            public string ImagePath { get; set; } = string.Empty;
            public int AvailableQuantity { get; set; }
        }
        public formPayMent() : this(string.Empty)
        {
        }

        public formPayMent(string username)
        {
            _username = username ?? string.Empty;
            InitializeComponent();
            dgvInvoice.AutoGenerateColumns = false;
            dgvInvoice.AllowUserToAddRows = false;
            dgvInvoice.MultiSelect = false;
            dgvInvoice.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvInvoice.CellValidating += dgvInvoice_CellValidating;
            dgvInvoice.DataError += dgvInvoice_DataError;

            btnSearch.Click += btnSearch_Click;
           
            LoadCurrentEmployeeInformation();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void btnThanhToan_Click(object sender, EventArgs e)
        {
            if (_invoiceTable == null || _invoiceTable.Rows.Count == 0)
            {
                MessageBox.Show(
                    "Không có sản phẩm trong hóa đơn.",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return;
            }
            if (!TryReloadProductData())
            {
                return;
            }

            if (!EnsureInvoiceQuantitiesWithinStock())
            {
                return;
            }
            var lines = new List<InvoiceLine>();
            var sequence = 1;
            foreach (DataRow row in _invoiceTable.Rows)
            {
                if (row.RowState == DataRowState.Deleted)
                {
                    continue;
                }

                try
                {
                    var line = new InvoiceLine
                    {
                        Sequence = sequence++,
                        ProductId = row.Field<int>("MaSP"),
                        ProductName = row.Field<string>("TenSP"),
                        Quantity = row.Field<int>("SoLuong"),
                        UnitPrice = row.Field<decimal>("DonGia")
                    };

                    lines.Add(line);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        "Không thể tạo dòng sản phẩm cho hóa đơn.\n" + ex.Message,
                        "Lỗi",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    return;
                }
            }

            if (lines.Count == 0)
            {
                MessageBox.Show(
                    "Không có sản phẩm hợp lệ trong hóa đơn.",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return;
            }
            var subtotal = lines.Sum(line => line.Total);
            var discountToApply = Math.Min(subtotal, Math.Max(0m, _loyaltyDiscountAmount));
            var snapshot = new InvoiceSnapshot(
                lines,
                DateTime.Now,
                _currentEmployeeId,
                  _username,
                discountToApply);
            var invoiceSaved = false;
            using (var invoiceForm = new formInHoaDon(snapshot, _username))
            {
                invoiceForm.ShowDialog(this);
                invoiceSaved = invoiceForm.IsInvoiceSaved;
                if (invoiceSaved)
                {
                    LoadProductsFromDatabase();
                }
            }
            if (invoiceSaved && _invoiceTable != null)
            {
                _invoiceTable.Clear();
            }

            UpdateTotalLabel();
        }

        private void splitContainer1_Panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void formPayMent_Load(object sender, EventArgs e)
        {
            InitializeInvoiceTable();
            ConfigureInvoiceGrid();
            dgvInvoice.DataSource = _invoiceTable;
            UpdateTotalLabel();
            LoadProductCategories();
            LoadProductsFromDatabase();
        }
        private void LoadProductCategories()
        {
            try
            {
                _loaiSpTableAdapter.ClearBeforeFill = true;
                _loaiSpTableAdapter.Fill(_dataSet.LoaiSP);

                var categories = new List<KeyValuePair<int?, string>>
                {
                    new KeyValuePair<int?, string>(null, "Tất cả sản phẩm")
                };

                foreach (var category in _dataSet.LoaiSP)
                {
                    categories.Add(new KeyValuePair<int?, string>(category.MaLoai, category.TenLoai));
                }

                cbSearch.DisplayMember = "Value";
                cbSearch.ValueMember = "Key";
                cbSearch.DataSource = categories;
                cbSearch.SelectedIndex = categories.Count > 0 ? 0 : -1;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Không thể tải danh sách loại sản phẩm.\n" + ex.Message,
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }
        private void InitializeInvoiceTable()
        {
            if (_invoiceTable != null)
            {
                return;
            }

            _invoiceTable = new DataTable("Invoice");

            var idColumn = _invoiceTable.Columns.Add("MaSP", typeof(int));
            idColumn.AllowDBNull = false;
            idColumn.Unique = true;

            _invoiceTable.Columns.Add("TenSP", typeof(string));
            _invoiceTable.Columns.Add("DonGia", typeof(decimal));

            var quantityColumn = _invoiceTable.Columns.Add("SoLuong", typeof(int));
            quantityColumn.DefaultValue = 1;

            var totalColumn = _invoiceTable.Columns.Add("ThanhTien", typeof(decimal));
            totalColumn.Expression = "[DonGia] * [SoLuong]";

            _invoiceTable.PrimaryKey = new[] { idColumn };

            _invoiceTable.RowChanged += InvoiceTable_RowChanged;
            _invoiceTable.RowDeleted += InvoiceTable_RowDeleted;
            _invoiceTable.ColumnChanged += InvoiceTable_ColumnChanged;
            RestoreInvoiceState();
        }

        private void ConfigureInvoiceGrid()
        {
            dgvInvoice.Columns.Clear();

            var nameColumn = new DataGridViewTextBoxColumn
            {
                DataPropertyName = "TenSP",
                HeaderText = "Tên sản phẩm",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                MinimumWidth = 150,
                ReadOnly = true
            };

            var priceColumn = new DataGridViewTextBoxColumn
            {
                DataPropertyName = "DonGia",
                HeaderText = "Đơn giá",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells,
                ReadOnly = true,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Alignment = DataGridViewContentAlignment.MiddleRight,
                    Format = "N0"
                }
            };

            var quantityColumn = new DataGridViewTextBoxColumn
            {
                DataPropertyName = "SoLuong",
                HeaderText = "Số lượng",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells,
                ReadOnly = false
            };

            var totalColumn = new DataGridViewTextBoxColumn
            {
                DataPropertyName = "ThanhTien",
                HeaderText = "Thành tiền",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells,
                ReadOnly = true,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Alignment = DataGridViewContentAlignment.MiddleRight,
                    Format = "N0"
                }
            };

            dgvInvoice.Columns.AddRange(nameColumn, priceColumn, quantityColumn, totalColumn);
        }
        private void LoadProductsFromDatabase()
        {
            try
            {
                _sanPhamTableAdapter.ClearBeforeFill = true;
                _sanPhamTableAdapter.Fill(_dataSet.SanPham);
                PopulateProductCards(_dataSet.SanPham);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Không thể tải danh sách sản phẩm từ cơ sở dữ liệu.\n" + ex.Message,
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void PopulateProductCards(IEnumerable<GStoreDataSet.SanPhamRow> products)
        {
            if (products == null)
            {
                return;
            }

            DisposeProductImages();

            flpProducts.SuspendLayout();
            flpProducts.Controls.Clear();

            foreach (var product in products)
            {
                if (product.RowState == DataRowState.Deleted)
                {
                    continue;
                }

                var info = new ProductDisplayInfo
                {
                    Id = product.MaSP,
                    Name = product.TenSP,
                    Price = product.DonGia,
                    ImagePath = product.IsHinhAnhNull() ? string.Empty : product.HinhAnh,
                    AvailableQuantity = product.SoLuong
                };

                var card = CreateProductCard(info);
                flpProducts.Controls.Add(card);
            }

            flpProducts.ResumeLayout();
        }

        private Control CreateProductCard(ProductDisplayInfo info)
        {
            var container = new Panel
            {
                Width = 200,
                Height = 260,
                Margin = new Padding(10),
                BackColor = Color.WhiteSmoke,
                BorderStyle = BorderStyle.FixedSingle
            };

            var picture = new PictureBox
            {
                Dock = DockStyle.Top,
                Height = 150,
                Margin = new Padding(10),
                SizeMode = PictureBoxSizeMode.Zoom,
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.White
            };

            var image = GetImageFromCache(info.ImagePath);
            if (image != null)
            {
                var clone = (Image)image.Clone();
                picture.Image = clone;
                _productImageClones.Add(clone);
            }

            var nameLabel = new Label
            {
                Dock = DockStyle.Top,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font(Font.FontFamily, 10F, FontStyle.Bold),
                Height = 40,
                Text = info.Name,
                Padding = new Padding(5)
            };

            var priceLabel = new Label
            {
                Dock = DockStyle.Top,
                TextAlign = ContentAlignment.MiddleCenter,
                Height = 30,
                ForeColor = Color.DarkGreen,
                Text = info.Price.ToString("N0", _currencyCulture) + " ₫"
            };

            var addButton = new Button
            {
                Dock = DockStyle.Bottom,
                Height = 35,
                Text = "Thêm",
                BackColor = Color.FromArgb(65, 140, 240),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Tag = info,
                Enabled = info.AvailableQuantity > 0
            };
            if (!addButton.Enabled)
            {
                addButton.Text = "Hết hàng";
                addButton.BackColor = Color.Gray;
            }
            addButton.FlatAppearance.BorderSize = 0;
            addButton.Click += OnAddProductButtonClick;

            container.Controls.Add(addButton);
            container.Controls.Add(priceLabel);
            container.Controls.Add(nameLabel);
            container.Controls.Add(picture);

            return container;
        }
        private void OnAddProductButtonClick(object sender, EventArgs e)
        {
            if (sender is Button button && button.Tag is ProductDisplayInfo info)
            {
                AddProductToInvoice(info);
            }
        }

        private void AddProductToInvoice(ProductDisplayInfo info)
        {
            if (info == null || _invoiceTable == null)
            {
                return;
            }
            if (!TryGetProductStock(info.Id, out var availableQuantity))
            {
                MessageBox.Show(
                    "Không thể xác định tồn kho cho sản phẩm đã chọn.",
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }

            if (availableQuantity <= 0)
            {
                MessageBox.Show(
                    "Sản phẩm đã hết hàng.",
                    "Cảnh báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }
            var existingRow = _invoiceTable.Rows.Find(info.Id);
            if (existingRow != null)
            {
                var currentQuantity = existingRow.Field<int>("SoLuong");
                if (currentQuantity >= availableQuantity)
                {
                    MessageBox.Show(
                        $"Số lượng bán không được vượt quá tồn kho ({availableQuantity}).",
                        "Cảnh báo",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return;
                }
                existingRow["SoLuong"] = currentQuantity + 1;
            }
            else
            {
                var newRow = _invoiceTable.NewRow();
                newRow["MaSP"] = info.Id;
                newRow["TenSP"] = info.Name;
                newRow["DonGia"] = info.Price;
                newRow["SoLuong"] = 1;
                _invoiceTable.Rows.Add(newRow);
            }

            UpdateTotalLabel();
        }
        private void InvoiceTable_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            if (e.Action == DataRowAction.Add || e.Action == DataRowAction.Change)
            {
                UpdateTotalLabel();
            }
        }

        private void InvoiceTable_RowDeleted(object sender, DataRowChangeEventArgs e)
        {
            UpdateTotalLabel();
        }

        private void InvoiceTable_ColumnChanged(object sender, DataColumnChangeEventArgs e)
        {
            if (string.Equals(e.Column.ColumnName, "SoLuong", StringComparison.OrdinalIgnoreCase))
            {
                UpdateTotalLabel();
            }
        }

        private void dgvInvoice_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            if (dgvInvoice.Columns[e.ColumnIndex].DataPropertyName == "SoLuong")
            {
                if (!int.TryParse(Convert.ToString(e.FormattedValue, CultureInfo.InvariantCulture), out var quantity) || quantity <= 0)
                {
                    e.Cancel = true;
                    MessageBox.Show(
                        "Số lượng phải là số nguyên dương.",
                        "Cảnh báo",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                }
                else if (dgvInvoice.Rows[e.RowIndex].DataBoundItem is DataRowView rowView)
                {
                    var productId = rowView.Row.Field<int>("MaSP");
                    if (!TryGetProductStock(productId, out var availableQuantity))
                    {
                        e.Cancel = true;
                        MessageBox.Show(
                            "Không thể xác định tồn kho cho sản phẩm.",
                            "Lỗi",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                    }
                    else if (quantity > availableQuantity)
                    {
                        e.Cancel = true;
                        MessageBox.Show(
                            $"Số lượng bán không được vượt quá tồn kho ({availableQuantity}).",
                            "Cảnh báo",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning);
                    }
                }
            }
        }

        private void dgvInvoice_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            e.Cancel = true;
            e.ThrowException = false;
            MessageBox.Show(
                "Giá trị vừa nhập không hợp lệ.",
                "Lỗi",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);
        }

        private void UpdateTotalLabel()
        {
            if (_invoiceTable == null || _invoiceTable.Rows.Count == 0)
            {
                lblTotalText.Text = "Tổng Tiền: 0 ₫";
                _loyaltyDiscountAmount = 0m;
                PersistInvoiceState();
                return;
            }

            decimal total = 0m;
            foreach (DataRow row in _invoiceTable.Rows)
            {
                if (row.RowState != DataRowState.Deleted)
                {
                    total += row.Field<decimal>("ThanhTien");
                }
            }

            var discountToApply = Math.Min(total, Math.Max(0m, _loyaltyDiscountAmount));
            var payableTotal = total - discountToApply;

            var totalText = "Tổng Tiền: " + payableTotal.ToString("N0", _currencyCulture) + " ₫";
            if (discountToApply > 0)
            {
                totalText += " (Đã giảm " + discountToApply.ToString("N0", _currencyCulture) + " ₫)";
            }

            lblTotalText.Text = totalText;
            PersistInvoiceState();
        }
        private bool TryGetProductStock(int productId, out int availableQuantity)
        {
            availableQuantity = 0;

            try
            {
                var productRow = _dataSet.SanPham.FindByMaSP(productId);
                if (productRow == null || productRow.RowState == DataRowState.Deleted)
                {
                    return false;
                }

                availableQuantity = productRow.SoLuong;
                return true;
            }
            catch
            {
                return false;
            }
        }

        private bool TryReloadProductData()
        {
            try
            {
                _sanPhamTableAdapter.ClearBeforeFill = true;
                _sanPhamTableAdapter.Fill(_dataSet.SanPham);
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Không thể tải lại dữ liệu sản phẩm.\n" + ex.Message,
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return false;
            }
        }

        private bool EnsureInvoiceQuantitiesWithinStock()
        {
            if (_invoiceTable == null)
            {
                return false;
            }

            foreach (DataRow row in _invoiceTable.Rows)
            {
                if (row.RowState == DataRowState.Deleted)
                {
                    continue;
                }

                var productId = row.Field<int>("MaSP");
                var requestedQuantity = row.Field<int>("SoLuong");

                if (!TryGetProductStock(productId, out var availableQuantity))
                {
                    MessageBox.Show(
                        "Không thể xác định tồn kho cho sản phẩm \"" + row.Field<string>("TenSP") + "\".",
                        "Lỗi",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    return false;
                }

                if (requestedQuantity > availableQuantity)
                {
                    MessageBox.Show(
                        $"Số lượng bán của \"{row.Field<string>("TenSP")}\" vượt quá tồn kho hiện có ({availableQuantity}).",
                        "Cảnh báo",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return false;
                }
            }

            return true;
        }
        private Image GetImageFromCache(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                return null;
            }

            if (_imageCache.TryGetValue(path, out var cached) && cached != null)
            {
                return cached;
            }

            var resolved = ResolveImagePath(path);
            if (!File.Exists(resolved))
            {
                return null;
            }

            try
            {
                using (var fs = new FileStream(resolved, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                using (var ms = new MemoryStream())
                {
                    fs.CopyTo(ms);
                    ms.Position = 0;
                    cached = Image.FromStream(ms);
                }

                _imageCache[path] = cached;
                return cached;
            }
            catch
            {
                return null;
            }
        }

        private string ResolveImagePath(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                return string.Empty;
            }

            var trimmed = path.Trim();
            if (Path.IsPathRooted(trimmed))
            {
                return trimmed;
            }

            var baseDirectory = AppDomain.CurrentDomain.BaseDirectory ?? string.Empty;
            var combined = Path.Combine(baseDirectory, trimmed);

            if (File.Exists(combined))
            {
                return combined;
            }

            var imageDirectory = Path.Combine(baseDirectory, "Image");
            var imageCombined = Path.Combine(imageDirectory, trimmed);
            if (File.Exists(imageCombined))
            {
                return imageCombined;
            }

            return combined;
        }

        private void DisposeProductImages()
        {
            if (_productImageClones.Count == 0)
            {
                return;
            }

            foreach (var image in _productImageClones)
            {
                image?.Dispose();
            }

            _productImageClones.Clear();
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            base.OnFormClosed(e);
            PersistInvoiceState();

            if (_invoiceTable != null)
            {
                _invoiceTable.RowChanged -= InvoiceTable_RowChanged;
                _invoiceTable.RowDeleted -= InvoiceTable_RowDeleted;
                _invoiceTable.ColumnChanged -= InvoiceTable_ColumnChanged;
            }

            DisposeProductImages();

            foreach (var image in _imageCache.Values)
            {
                image?.Dispose();
            }

            _imageCache.Clear();
            _sanPhamTableAdapter?.Dispose();
            _loaiSpTableAdapter?.Dispose();
            _nhanVienTableAdapter?.Dispose();
            _nguoiDungTableAdapter?.Dispose();
            _dataSet?.Dispose();
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            int? selectedCategoryId = null;
            var selectedValue = cbSearch.SelectedValue;

            if (selectedValue is int id)
            {
                selectedCategoryId = id;
            }
            // nếu null thì giữ selectedCategoryId = null

            ApplyProductFilter(selectedCategoryId);
        }

        private void ApplyProductFilter(int? categoryId)
        {
            if (_dataSet?.SanPham == null)
            {
                return;
            }

            IEnumerable<GStoreDataSet.SanPhamRow> products = _dataSet.SanPham;

            if (categoryId.HasValue)
            {
                products = products.Where(row => row.RowState != DataRowState.Deleted && row.MaLoai == categoryId.Value);
            }

            PopulateProductCards(products);
        }
        private void LoadCurrentEmployeeInformation()
        {
            if (string.IsNullOrWhiteSpace(_username))
            {
                return;
            }

            try
            {
                _nguoiDungTableAdapter.ClearBeforeFill = true;
                _nguoiDungTableAdapter.Fill(_dataSet.NguoiDung);

                var userRow = _dataSet.NguoiDung
                     .FirstOrDefault(row => !row.IsNull(_dataSet.NguoiDung.TenDangNhapColumn) &&
                        string.Equals(row.TenDangNhap, _username, StringComparison.OrdinalIgnoreCase));

                if (userRow == null)
                {
                    return;
                }

                _nhanVienTableAdapter.ClearBeforeFill = true;
                _nhanVienTableAdapter.Fill(_dataSet.NhanVien);

                var employeeRow = _dataSet.NhanVien
                    .FirstOrDefault(row => !row.IsMaNguoiDungNull() && row.MaNguoiDung == userRow.MaNguoiDung);

                if (employeeRow != null)
                {
                    _currentEmployeeId = employeeRow.MaNhanVien;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Không thể xác định mã nhân viên hiện tại.\n" + ex.Message,
                    "Cảnh báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }
        }

        private void btnTichDiem_Click(object sender, EventArgs e)
        {
            using (var loyaltyForm = new formTichDIem())
            {
                loyaltyForm.ShowDialog(this);
            }
        }

        private void PersistInvoiceState()
        {
            if (_invoiceTable == null || _invoiceTable.Rows.Count == 0)
            {
                _savedInvoiceSnapshot = null;
                _savedLoyaltyDiscount = 0m;
                return;
            }

            _savedInvoiceSnapshot = _invoiceTable.Copy();
            _savedLoyaltyDiscount = _loyaltyDiscountAmount;
        }

        private void RestoreInvoiceState()
        {
            if (_savedInvoiceSnapshot == null)
            {
                return;
            }

            foreach (DataRow row in _savedInvoiceSnapshot.Rows)
            {
                if (row.RowState == DataRowState.Deleted)
                {
                    continue;
                }

                var newRow = _invoiceTable.NewRow();
                newRow["MaSP"] = row["MaSP"];
                newRow["TenSP"] = row["TenSP"];
                newRow["DonGia"] = row["DonGia"];
                newRow["SoLuong"] = row["SoLuong"];
                _invoiceTable.Rows.Add(newRow);
            }

            _loyaltyDiscountAmount = _savedLoyaltyDiscount;
        }

        public void ApplyLoyaltyDiscount(decimal discountAmount)
        {
            _loyaltyDiscountAmount = discountAmount < 0m ? 0m : discountAmount;
            UpdateTotalLabel();
        }

        private void btnHuy_Click(object sender, EventArgs e)
        {
            if (_invoiceTable == null || _invoiceTable.Rows.Count == 0)
            {
                _loyaltyDiscountAmount = 0m;
                UpdateTotalLabel();
                return;
            }

            var confirm = MessageBox.Show(
                "Bạn có chắc chắn muốn hủy hóa đơn hiện tại?",
                "Xác nhận",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (confirm == DialogResult.Yes)
            {
                _invoiceTable.Clear();
                _loyaltyDiscountAmount = 0m;
                UpdateTotalLabel();
            }
        }
    }
}
