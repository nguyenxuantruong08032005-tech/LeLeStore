using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LeLeStore
{
    public partial class formInHoaDon : Form
    {
        private readonly InvoiceSnapshot _invoiceSnapshot;
        private readonly BindingList<InvoiceLine> _invoiceLines;
        private readonly decimal _discountAmount;
        private readonly CultureInfo _currencyCulture = CultureInfo.GetCultureInfo("vi-VN");
        private readonly GStoreDataSet _dataSet = new GStoreDataSet();
        private readonly GStoreDataSetTableAdapters.KhachHangTableAdapter _khachHangTableAdapter = new GStoreDataSetTableAdapters.KhachHangTableAdapter();
        private int? _persistedInvoiceId;
        private bool _isSaved;
        private int? _suggestedInvoiceNumber;
        private GStoreDataSet.KhachHangRow _currentCustomerRow;
        private decimal _currentDiscountAmount;
        private decimal _appliedLoyaltyDiscount;
        private int? _selectedCustomerId;
        public bool IsInvoiceSaved => _isSaved;
        public formInHoaDon() : this(new InvoiceSnapshot(Array.Empty<InvoiceLine>(), DateTime.Now, null, string.Empty))
        {
        }

        public formInHoaDon(InvoiceSnapshot invoiceSnapshot)
        {
            _invoiceSnapshot = invoiceSnapshot ?? throw new ArgumentNullException(nameof(invoiceSnapshot));
            InitializeComponent();
            dataGridView1.AutoGenerateColumns = false;
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.MultiSelect = false;
            dataGridView1.ReadOnly = true;
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            ConfigureGridColumns();

            _invoiceLines = new BindingList<InvoiceLine>(_invoiceSnapshot.Lines.Select(line => line.Clone()).ToList());
            var subtotal = _invoiceLines.Sum(line => line.Total);
            _discountAmount = Math.Min(subtotal, Math.Max(0m, _invoiceSnapshot.DiscountAmount));
            dataGridView1.DataSource = _invoiceLines;

            textBox1.ReadOnly = true;
            txtMaNv.ReadOnly = false;
            dateTimePicker1.Enabled = false;

            btnInHoaDon.Click += btnInHoaDon_Click;
            btnLuu.Click += btnLuu_Click;
            Load += formInHoaDon_Load;
            InitializeCustomerFeatures();
        }

        private void panel1_Click(object sender, EventArgs e)
        {

        }

        private void formInHoaDon_Load(object sender, EventArgs e)
        {
            PopulateInvoiceMetadata();
            UpdateTotalsDisplay();
            TryReloadCustomerData();
        }
        private void ConfigureGridColumns()
        {
            dataGridView1.Columns.Clear();
            var customerColumn = new DataGridViewTextBoxColumn
            {
                DataPropertyName = nameof(InvoiceLine.CustomerId),
                HeaderText = "Mã KH",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells,
                ReadOnly = true,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    NullValue = string.Empty
                }
            };


            var orderColumn = new DataGridViewTextBoxColumn
            {
                DataPropertyName = nameof(InvoiceLine.Sequence),
                HeaderText = "STT",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells,
                ReadOnly = true
            };

            var nameColumn = new DataGridViewTextBoxColumn
            {
                DataPropertyName = nameof(InvoiceLine.ProductName),
                HeaderText = "Tên SP",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                ReadOnly = true
            };

            var quantityColumn = new DataGridViewTextBoxColumn
            {
                DataPropertyName = nameof(InvoiceLine.Quantity),
                HeaderText = "SL",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells,
                ReadOnly = true
            };

            var priceColumn = new DataGridViewTextBoxColumn
            {
                DataPropertyName = nameof(InvoiceLine.UnitPrice),
                HeaderText = "Đơn giá",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells,
                ReadOnly = true,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Alignment = DataGridViewContentAlignment.MiddleRight,
                    Format = "N0"
                }
            };

            var totalColumn = new DataGridViewTextBoxColumn
            {
                DataPropertyName = nameof(InvoiceLine.Total),
                HeaderText = "Thành tiền",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells,
                ReadOnly = true,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Alignment = DataGridViewContentAlignment.MiddleRight,
                    Format = "N0"
                }
            };

            dataGridView1.Columns.AddRange(customerColumn, orderColumn, nameColumn, quantityColumn, priceColumn, totalColumn);
        }
        private void PopulateInvoiceMetadata()
        {
            var invoiceDate = _invoiceSnapshot.CreatedAt;

            if (invoiceDate < dateTimePicker1.MinDate)
            {
                invoiceDate = dateTimePicker1.MinDate;
            }
            else if (invoiceDate > dateTimePicker1.MaxDate)
            {
                invoiceDate = dateTimePicker1.MaxDate;
            }

            dateTimePicker1.Value = invoiceDate;

            if (_invoiceSnapshot.EmployeeId.HasValue)
            {
                txtMaNv.Text = _invoiceSnapshot.EmployeeId.Value.ToString();
            }

            _suggestedInvoiceNumber = GetNextInvoiceNumber();
            if (_suggestedInvoiceNumber.HasValue)
            {
                textBox1.Text = _suggestedInvoiceNumber.Value.ToString();
            }
        }

        private int? GetNextInvoiceNumber()
        {
            try
            {
                using (var connection = new SqlConnection(Properties.Settings.Default.GStoreConnectionString))
                using (var command = new SqlCommand("SELECT ISNULL(MAX(MaHD), 0) + 1 FROM HoaDon", connection))
                {
                    connection.Open();
                    var result = command.ExecuteScalar();

                    if (result != null && result != DBNull.Value && int.TryParse(Convert.ToString(result, CultureInfo.InvariantCulture), out var value))
                    {
                        return value;
                    }
                }
            }
            catch
            {
                // Bỏ qua lỗi, người dùng vẫn có thể lưu hóa đơn
            }

            return null;
        }

        private void btnInHoaDon_Click(object sender, EventArgs e)
        {
            if (_invoiceLines.Count == 0)
            {
                MessageBox.Show(
                    "Không có dữ liệu để in.",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return;
            }

            using (var dialog = new SaveFileDialog
            {
                Filter = "PDF Files (*.pdf)|*.pdf",
                FileName = $"HoaDon_{(string.IsNullOrWhiteSpace(textBox1.Text) ? DateTime.Now.ToString("yyyyMMddHHmmss") : textBox1.Text)}.pdf"
            })
            {
                if (dialog.ShowDialog(this) == DialogResult.OK)
                {
                    try
                    {
                        ExportInvoiceToPdf(dialog.FileName);
                        MessageBox.Show(
                            "In hóa đơn thành công.",
                            "Thành công",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(
                            "Không thể tạo file PDF.\n" + ex.Message,
                            "Lỗi",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void btnLuu_Click(object sender, EventArgs e)
        {
            if (_isSaved)
            {
                MessageBox.Show(
                    "Hóa đơn này đã được lưu.",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return;
            }

            if (_invoiceLines.Count == 0)
            {
                MessageBox.Show(
                    "Không có dữ liệu để lưu.",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return;
            }

            if (!int.TryParse(txtMaNv.Text.Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out var employeeId) || employeeId <= 0)
            {
                MessageBox.Show(
                    "Mã nhân viên không hợp lệ.",
                    "Cảnh báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                txtMaNv.Focus();
                txtMaNv.SelectAll();
                return;
            }

            try
            {
                var invoiceId = SaveInvoiceToDatabase(employeeId);
                _persistedInvoiceId = invoiceId;
                _isSaved = true;
                textBox1.Text = invoiceId.ToString(CultureInfo.InvariantCulture);

                MessageBox.Show(
                    "Lưu hóa đơn thành công.",
                    "Thành công",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Không thể lưu hóa đơn.\n" + ex.Message,
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }
        private int SaveInvoiceToDatabase(int employeeId)
        {
            using (var connection = new SqlConnection(Properties.Settings.Default.GStoreConnectionString))
            {
                connection.Open();

                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        var invoiceId = InsertInvoiceHeader(connection, transaction, employeeId);

                        foreach (var line in _invoiceLines)
                        {
                            ReduceProductStock(connection, transaction, line);
                            InsertInvoiceDetail(connection, transaction, invoiceId, line);
                        }

                        transaction.Commit();
                        return invoiceId;
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }
        private void ReduceProductStock(SqlConnection connection, SqlTransaction transaction, InvoiceLine line)
        {
            using (var command = new SqlCommand(
                "UPDATE SanPham SET SoLuong = SoLuong - @SoLuong WHERE MaSP = @MaSP AND SoLuong >= @SoLuong;",
                connection,
                transaction))
            {
                command.Parameters.Add("@MaSP", SqlDbType.Int).Value = line.ProductId;
                command.Parameters.Add("@SoLuong", SqlDbType.Int).Value = line.Quantity;

                var affectedRows = command.ExecuteNonQuery();
                if (affectedRows == 0)
                {
                    throw new InvalidOperationException(
                        $"Không đủ tồn kho cho sản phẩm \"{line.ProductName}\".");
                }
            }
        }

        private int InsertInvoiceHeader(SqlConnection connection, SqlTransaction transaction, int employeeId)
        {
            var totalAmount = CalculateTotalAmount();

            using (var command = new SqlCommand(
"INSERT INTO HoaDon (NgayLap, TongTien, MaKhachHang, MaNhanVien) OUTPUT INSERTED.MaHD VALUES (@NgayLap, @TongTien, @MaKhachHang, @MaNhanVien);", connection, transaction))
            {
                command.Parameters.Add("@NgayLap", SqlDbType.DateTime2).Value = dateTimePicker1.Value;

                var totalParameter = command.Parameters.Add("@TongTien", SqlDbType.Decimal);
                totalParameter.Precision = 18;
                totalParameter.Scale = 2;
                totalParameter.Value = totalAmount;
                var customerParameter = command.Parameters.Add("@MaKhachHang", SqlDbType.Int);
                if (_selectedCustomerId.HasValue)
                {
                    customerParameter.Value = _selectedCustomerId.Value;
                }
                else
                {
                    customerParameter.Value = DBNull.Value;
                }
                command.Parameters.Add("@MaNhanVien", SqlDbType.Int).Value = employeeId;

                var result = command.ExecuteScalar();

                if (result == null || result == DBNull.Value)
                {
                    throw new InvalidOperationException("Không nhận được mã hóa đơn mới từ cơ sở dữ liệu.");
                }

                return Convert.ToInt32(result, CultureInfo.InvariantCulture);
            }
        }

        private void InsertInvoiceDetail(SqlConnection connection, SqlTransaction transaction, int invoiceId, InvoiceLine line)
        {
            using (var command = new SqlCommand(
                "INSERT INTO ChiTietHoaDon (MaHD, MaSP, SoLuong, DonGia) VALUES (@MaHD, @MaSP, @SoLuong, @DonGia);",
                connection, transaction))
            {
                command.Parameters.Add("@MaHD", SqlDbType.Int).Value = invoiceId;
                command.Parameters.Add("@MaSP", SqlDbType.Int).Value = line.ProductId;
                command.Parameters.Add("@SoLuong", SqlDbType.Int).Value = line.Quantity;

                var priceParameter = command.Parameters.Add("@DonGia", SqlDbType.Decimal);
                priceParameter.Precision = 18;
                priceParameter.Scale = 2;
                priceParameter.Value = line.UnitPrice;

                command.ExecuteNonQuery();
            }
        }
        private (decimal Subtotal, decimal Discount, decimal Total) CalculateFinancialSummary()
        {
            var subtotal = _invoiceLines.Sum(line => line.Total);
            if (subtotal <= 0m)
            {
                return (0m, 0m, 0m);
            }

            var baseDiscount = Math.Max(0m, Math.Min(subtotal, _discountAmount));
            var remainingAfterBase = subtotal - baseDiscount;
            var loyaltyDiscount = Math.Max(0m, Math.Min(remainingAfterBase, _appliedLoyaltyDiscount));
            var discount = baseDiscount + loyaltyDiscount;
            var total = subtotal - discount;

            return (subtotal, discount, total);
        }
        private decimal CalculateTotalAmount()
        {
            return CalculateFinancialSummary().Total;
        }
        private void UpdateTotalsDisplay()
        {
            var summary = CalculateFinancialSummary();

            lblSubtotal.Text = "Tổng trước giảm: " + summary.Subtotal.ToString("N0", _currencyCulture) + " ₫";
            lblDiscount.Text = "Chiết khấu: " + summary.Discount.ToString("N0", _currencyCulture) + " ₫";
            lblTotalPayable.Text = "Tổng thanh toán: " + summary.Total.ToString("N0", _currencyCulture) + " ₫";
        }

        private void ExportInvoiceToPdf(string filePath)
        {
            var lines = _invoiceLines.ToList();
            var summary = CalculateFinancialSummary();
            var subtotalAmount = summary.Subtotal;
            var discountAmount = summary.Discount;
            var totalAmount = summary.Total;

            var contentBuilder = new StringBuilder();

            contentBuilder.AppendLine("BT");
            contentBuilder.AppendLine("/F1 18 Tf");
            contentBuilder.AppendLine($"50 800 Td ({EscapePdfText("HOA DON BAN HANG")}) Tj");
            contentBuilder.AppendLine("/F1 12 Tf");
            contentBuilder.AppendLine($"0 -24 Td ({EscapePdfText($"Ma hoa don: {GetInvoiceIdentifier()}")}) Tj");
            contentBuilder.AppendLine($"0 -16 Td ({EscapePdfText($"Ngay lap: {dateTimePicker1.Value:dd/MM/yyyy HH:mm}")}) Tj");
            contentBuilder.AppendLine($"0 -16 Td ({EscapePdfText($"Ma nhan vien: {txtMaNv.Text.Trim()}")}) Tj");
            contentBuilder.AppendLine("0 -24 Td (" + EscapePdfText("Danh sach san pham:") + ") Tj");
            contentBuilder.AppendLine("0 -18 Td (" + EscapePdfText("STT   Ten SP                     SL   Don gia        Thanh tien") + ") Tj");

            foreach (var line in lines)
            {
                var unitPrice = line.UnitPrice.ToString("N0", _currencyCulture);
                var total = line.Total.ToString("N0", _currencyCulture);
                var rowText = string.Format(CultureInfo.InvariantCulture,
                    "{0,2}   {1,-25}   {2,3}   {3,12}   {4,12}",
                    line.Sequence,
                    Truncate(line.ProductName, 25),
                    line.Quantity,
                    unitPrice,
                    total);

                contentBuilder.AppendLine("0 -16 Td (" + EscapePdfText(rowText) + ") Tj");
            }

            contentBuilder.AppendLine("0 -24 Td (" + EscapePdfText($"Tong truoc giam: {subtotalAmount.ToString("N0", _currencyCulture)} VND") + ") Tj");
            if (discountAmount > 0)
            {
                contentBuilder.AppendLine("0 -16 Td (" + EscapePdfText($"Chiet khau: {discountAmount.ToString("N0", _currencyCulture)} VND") + ") Tj");
            }

            contentBuilder.AppendLine("0 -16 Td (" + EscapePdfText($"Tong cong: {totalAmount.ToString("N0", _currencyCulture)} VND") + ") Tj");
            contentBuilder.AppendLine("ET");

            var contentString = contentBuilder.ToString();
            var contentBytes = Encoding.ASCII.GetBytes(contentString);

            var objects = new List<string>
            {
                "1 0 obj << /Type /Catalog /Pages 2 0 R >> endobj",
                "2 0 obj << /Type /Pages /Count 1 /Kids [3 0 R] >> endobj",
                "3 0 obj << /Type /Page /Parent 2 0 R /MediaBox [0 0 595 842] /Contents 4 0 R /Resources << /Font << /F1 5 0 R >> >> >> endobj",
                $"4 0 obj << /Length {contentBytes.Length} >>\nstream\n{contentString}endstream\nendobj",
                "5 0 obj << /Type /Font /Subtype /Type1 /BaseFont /Helvetica >> endobj"
            };

            using (var stream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
            using (var writer = new BinaryWriter(stream, Encoding.ASCII))
            {
                var offsets = new List<long> { 0 };

                WriteLine(writer, "%PDF-1.4");

                foreach (var obj in objects)
                {
                    offsets.Add(stream.Position);
                    WriteLine(writer, obj);
                }

                var xrefPosition = stream.Position;
                WriteLine(writer, $"xref\n0 {objects.Count + 1}");
                WriteLine(writer, "0000000000 65535 f ");

                for (int i = 1; i < offsets.Count; i++)
                {
                    WriteLine(writer, string.Format(CultureInfo.InvariantCulture, "{0:0000000000} 00000 n ", offsets[i]));
                }

                WriteLine(writer, "trailer");
                WriteLine(writer, $"<< /Size {objects.Count + 1} /Root 1 0 R >>");
                WriteLine(writer, "startxref");
                WriteLine(writer, xrefPosition.ToString(CultureInfo.InvariantCulture));
                WriteLine(writer, "%%EOF", false);
            }
        }

        private static void WriteLine(BinaryWriter writer, string value, bool appendNewLine = true)
        {
            if (appendNewLine)
            {
                value += "\n";
            }

            var bytes = Encoding.ASCII.GetBytes(value);
            writer.Write(bytes);
        }

        private string GetInvoiceIdentifier()
        {
            if (_persistedInvoiceId.HasValue)
            {
                return _persistedInvoiceId.Value.ToString(CultureInfo.InvariantCulture);
            }

            if (!string.IsNullOrWhiteSpace(textBox1.Text))
            {
                return textBox1.Text.Trim();
            }

            return DateTime.Now.ToString("yyyyMMddHHmmss", CultureInfo.InvariantCulture);
        }

        private static string Truncate(string value, int length)
        {
            if (string.IsNullOrEmpty(value) || value.Length <= length)
            {
                return value ?? string.Empty;
            }

            if (length <= 3)
            {
                return value.Substring(0, length);
            }

            return value.Substring(0, length - 3) + "...";
        }

        private static string EscapePdfText(string input)
        {
            if (input == null)
            {
                return string.Empty;
            }

            return input
                .Replace("\\", "\\\\")
                .Replace("(", "\\(")
                .Replace(")", "\\)");
        }
        private void InitializeCustomerFeatures()
        {
            numericUpDown1.Minimum = 0;
            numericUpDown1.Maximum = int.MaxValue;
            numericUpDown1.Enabled = false;

            txtChietKhau.ReadOnly = true;

            btnUpdate.Enabled = false;
            btnChietKhau.Enabled = false;

            ResetCustomerState();
        }

        private bool TryReloadCustomerData()
        {
            try
            {
                _dataSet.KhachHang.Clear();
                _khachHangTableAdapter.Fill(_dataSet.KhachHang);
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Không thể tải dữ liệu khách hàng. Lỗi: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }
        private void ResetCustomerState()
        {
            _currentCustomerRow = null;
            _selectedCustomerId = null;
            numericUpDown1.Enabled = false;
            btnUpdate.Enabled = false;
            numericUpDown1.Value = numericUpDown1.Minimum;
            ResetDiscountDisplay();
            UpdateCustomerIdOnInvoiceLines(null);
        }

        private void UpdateCustomerIdOnInvoiceLines(int? customerId)
        {
            foreach (var line in _invoiceLines)
            {
                line.CustomerId = customerId;
            }

            dataGridView1.Refresh();
        }
        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            var phoneNumber = txtSearch.Text.Trim();

            if (string.IsNullOrWhiteSpace(phoneNumber))
            {
                MessageBox.Show("Vui lòng nhập số điện thoại để tìm kiếm.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ResetCustomerState();
                txtSearch.Focus();
                return;
            }

            if (!TryReloadCustomerData())
            {
                return;
            }

            var customerRow = _dataSet.KhachHang.FirstOrDefault(row => string.Equals(row.SoDienThoai?.Trim(), phoneNumber, StringComparison.OrdinalIgnoreCase));

            if (customerRow == null)
            {
                MessageBox.Show("Không tìm thấy khách hàng với số điện thoại đã nhập.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ResetCustomerState();
                return;
            }

            _currentCustomerRow = customerRow;
            _selectedCustomerId = customerRow.MaKhachHang;
            _appliedLoyaltyDiscount = 0m;

            numericUpDown1.Enabled = true;
            btnUpdate.Enabled = true;

            UpdateCustomerIdOnInvoiceLines(_selectedCustomerId);
            UpdateNumericUpDownWithCurrentCustomer();
            UpdateTotalsDisplay();
        }
        private void UpdateNumericUpDownWithCurrentCustomer()
        {
            if (_currentCustomerRow == null)
            {
                return;
            }

            var points = _currentCustomerRow.IsDiemTichLuyNull() ? 0 : _currentCustomerRow.DiemTichLuy;
            var value = Math.Max(numericUpDown1.Minimum, Math.Min(numericUpDown1.Maximum, points));
            numericUpDown1.Value = value;
            UpdateDiscountDisplay(points);
            _appliedLoyaltyDiscount = Math.Min(_appliedLoyaltyDiscount, _currentDiscountAmount);
            UpdateTotalsDisplay();
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (_currentCustomerRow == null)
            {
                MessageBox.Show("Vui lòng tìm kiếm và chọn khách hàng trước khi cập nhật.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var newPoints = (int)numericUpDown1.Value;

            try
            {
                _currentCustomerRow.DiemTichLuy = newPoints;

                var rowsAffected = _khachHangTableAdapter.Update(_currentCustomerRow);

                if (rowsAffected > 0)
                {
                    MessageBox.Show("Cập nhật điểm tích lũy thành công.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    UpdateNumericUpDownWithCurrentCustomer();
                }
                else
                {
                    MessageBox.Show("Không có thay đổi nào được lưu.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                _currentCustomerRow.RejectChanges();
                MessageBox.Show($"Không thể cập nhật điểm tích lũy. Lỗi: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void UpdateDiscountDisplay(int points)
        {
            _currentDiscountAmount = CalculateDiscountAmount(points);
            txtChietKhau.Text = FormatCurrency(_currentDiscountAmount);
            btnChietKhau.Enabled = _currentDiscountAmount > 0;
        }

        private void ResetDiscountDisplay()
        {
            _currentDiscountAmount = 0m;
            _appliedLoyaltyDiscount = 0m;
            txtChietKhau.Text = FormatCurrency(_currentDiscountAmount);
            btnChietKhau.Enabled = false;
            UpdateTotalsDisplay();
        }

        private decimal CalculateDiscountAmount(int points)
        {
            if (points >= 20)
            {
                return 11000m;
            }

            if (points >= 15)
            {
                return 9000m;
            }

            if (points >= 10)
            {
                return 7000m;
            }

            if (points >= 5)
            {
                return 5000m;
            }

            return 0m;
        }

        private string FormatCurrency(decimal amount)
        {
            return amount.ToString("N0", _currencyCulture) + " ₫";
        }

        private void btnChietKhau_Click(object sender, EventArgs e)
        {
            if (_currentCustomerRow == null)
            {
                MessageBox.Show("Vui lòng chọn khách hàng trước khi áp dụng chiết khấu.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (_currentDiscountAmount <= 0)
            {
                MessageBox.Show("Khách hàng chưa đủ điểm tích lũy để nhận khuyến mãi.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            _appliedLoyaltyDiscount = _currentDiscountAmount;
            UpdateTotalsDisplay();

            MessageBox.Show($"Đã áp dụng chiết khấu {FormatCurrency(_currentDiscountAmount)} vào hóa đơn.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
