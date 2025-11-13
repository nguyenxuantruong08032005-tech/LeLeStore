using PdfSharp;
using PdfSharp.Drawing;
using PdfSharp.Drawing.Layout;
using PdfSharp.Fonts;
using PdfSharp.Pdf;
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
        private static readonly object PdfFontInitializationLock = new object();
        private static bool _pdfFontResolverInitialized;
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
            EnsurePdfFontResolver();

            var lines = _invoiceLines.ToList();
            var summary = CalculateFinancialSummary();
            var subtotalAmount = summary.Subtotal;
            var discountAmount = summary.Discount;
            var totalAmount = summary.Total;

            using (var document = new PdfDocument())
            {
                var page = document.AddPage();
                page.Size = PageSize.A4;

                using (var graphics = XGraphics.FromPdfPage(page))
                {
                    var fontOptions = new XPdfFontOptions(PdfFontEncoding.Unicode, PdfFontEmbedding.Always);
                    var titleFont = new XFont("DejaVu Sans", 18, XFontStyle.Bold, fontOptions);
                    var labelFont = new XFont("DejaVu Sans", 12, XFontStyle.Regular, fontOptions);
                    var labelBoldFont = new XFont("DejaVu Sans", 12, XFontStyle.Bold, fontOptions);
                    var tableHeaderFont = new XFont("DejaVu Sans", 11, XFontStyle.Bold, fontOptions);
                    var tableCellFont = new XFont("DejaVu Sans", 11, XFontStyle.Regular, fontOptions);

                    const double margin = 50;
                    double availableWidth = page.Width - (2 * margin);
                    double left = margin;
                    double cursorY = margin;

                    double titleHeight = GetLineHeight(graphics, titleFont);
                    graphics.DrawString("HOÁ ĐƠN BÁN HÀNG", titleFont, XBrushes.Black, new XRect(left, cursorY, availableWidth, titleHeight), XStringFormats.TopCenter);
                    cursorY += titleHeight + 12;

                    double infoLineHeight = GetLineHeight(graphics, labelFont);
                    graphics.DrawString($"Mã hoá đơn: {GetInvoiceIdentifier()}", labelFont, XBrushes.Black, new XRect(left, cursorY, availableWidth, infoLineHeight), XStringFormats.TopLeft);
                    cursorY += infoLineHeight;

                    graphics.DrawString($"Ngày lập: {dateTimePicker1.Value:dd/MM/yyyy HH:mm}", labelFont, XBrushes.Black, new XRect(left, cursorY, availableWidth, infoLineHeight), XStringFormats.TopLeft);
                    cursorY += infoLineHeight;

                    graphics.DrawString($"Mã nhân viên: {txtMaNv.Text.Trim()}", labelFont, XBrushes.Black, new XRect(left, cursorY, availableWidth, infoLineHeight), XStringFormats.TopLeft);
                    cursorY += infoLineHeight * 1.5;

                    graphics.DrawString("Danh sách sản phẩm", labelBoldFont, XBrushes.Black, new XRect(left, cursorY, availableWidth, infoLineHeight), XStringFormats.TopLeft);
                    cursorY += infoLineHeight * 1.2;

                    double[] columnWidths = { 50, 220, 70, 110, 110 };
                    double headerHeight = GetLineHeight(graphics, tableHeaderFont) + 8;
                    double rowHeight = GetLineHeight(graphics, tableCellFont) + 8;

                    var borderPen = new XPen(XColors.Black, 0.75);
                    var headerBrush = new XSolidBrush(XColor.FromArgb(235, 235, 235));
                    var headerTexts = new[] { "STT", "Tên sản phẩm", "SL", "Đơn giá", "Thành tiền" };
                    var headerFormat = new XStringFormat
                    {
                        Alignment = XStringAlignment.Center,
                        LineAlignment = XLineAlignment.Center
                    };


                    double cursorX = left;
                    for (int i = 0; i < headerTexts.Length; i++)
                    {
                        var cellRect = new XRect(cursorX, cursorY, columnWidths[i], headerHeight);
                        graphics.DrawRectangle(borderPen, headerBrush, cellRect);
                        graphics.DrawString(headerTexts[i], tableHeaderFont, XBrushes.Black, cellRect, headerFormat);
                        cursorX += columnWidths[i];
                    }

                    cursorY += headerHeight;

                    var cellFormats = new[]
                  {
                        new XStringFormat { Alignment = XStringAlignment.Center, LineAlignment = XLineAlignment.Center },
                        new XStringFormat { Alignment = XStringAlignment.Near, LineAlignment = XLineAlignment.Center },
                        new XStringFormat { Alignment = XStringAlignment.Center, LineAlignment = XLineAlignment.Center },
                        new XStringFormat { Alignment = XStringAlignment.Far, LineAlignment = XLineAlignment.Center },
                        new XStringFormat { Alignment = XStringAlignment.Far, LineAlignment = XLineAlignment.Center }
                    };

                    var textFormatter = new XTextFormatter(graphics)
                    {
                        Alignment = XParagraphAlignment.Left
                    };

                    foreach (var line in lines)
                    {
                        var rowValues = new[]
                        {
                            line.Sequence.ToString(CultureInfo.InvariantCulture),
                            line.ProductName,
                            line.Quantity.ToString(CultureInfo.InvariantCulture),
                            line.UnitPrice.ToString("N0", _currencyCulture),
                            line.Total.ToString("N0", _currencyCulture)
                        };

                        cursorX = left;
                        for (int i = 0; i < rowValues.Length; i++)
                        {
                            var cellRect = new XRect(cursorX, cursorY, columnWidths[i], rowHeight);
                            graphics.DrawRectangle(borderPen, cellRect);

                            if (i == 1)
                            {
                                var textRect = new XRect(cellRect.X + 4, cellRect.Y + 2, cellRect.Width - 8, cellRect.Height - 4);
                                textFormatter.DrawString(rowValues[i], tableCellFont, XBrushes.Black, textRect, XStringFormats.TopLeft);
                            }
                            else
                            {
                                graphics.DrawString(rowValues[i], tableCellFont, XBrushes.Black, cellRect, cellFormats[i]);
                            }

                            cursorX += columnWidths[i];
                        }

                        cursorY += rowHeight;
                    }

                    cursorY += infoLineHeight;

                    graphics.DrawString($"Tổng trước giảm: {subtotalAmount.ToString("N0", _currencyCulture)} VND", labelFont, XBrushes.Black, new XRect(left, cursorY, availableWidth, infoLineHeight), XStringFormats.TopLeft);
                    cursorY += infoLineHeight;

                    if (discountAmount > 0)
                    {
                        graphics.DrawString($"Chiết khấu: {discountAmount.ToString("N0", _currencyCulture)} VND", labelFont, XBrushes.Black, new XRect(left, cursorY, availableWidth, infoLineHeight), XStringFormats.TopLeft);
                        cursorY += infoLineHeight;
                    }
                    graphics.DrawString($"Tổng cộng: {totalAmount.ToString("N0", _currencyCulture)} VND", labelBoldFont, XBrushes.Black, new XRect(left, cursorY, availableWidth, infoLineHeight), XStringFormats.TopLeft);
                }
                document.Save(filePath);
            }
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

        private static void EnsurePdfFontResolver()
        {
            if (_pdfFontResolverInitialized)
            {
                return ;
            }

            lock (PdfFontInitializationLock)
            {
                if (_pdfFontResolverInitialized)
                {
                    return;
                }

                GlobalFontSettings.FontResolver = new PdfEmbeddedFontResolver();
                _pdfFontResolverInitialized = true;
            }
        }

        private static double GetLineHeight(XGraphics graphics, XFont font)
        {
            return graphics.MeasureString("Ag", font).Height;
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

            var points = _currentCustomerRow.IsNull(_currentCustomerRow.Table.Columns["DiemTichLuy"]) ? 0 : _currentCustomerRow.DiemTichLuy;
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

        private void btnChietKhau_Click_1(object sender, EventArgs e)
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
