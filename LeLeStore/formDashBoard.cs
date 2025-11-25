using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Windows.Forms.DataVisualization.Charting;
namespace LeLeStore
{
    public partial class formDashBoard : Form
    {
        private readonly string _connectionString = Properties.Settings.Default.GStoreConnectionString;
        private Chart chartBanHang;
        private Chart chartKho;
        private SplitContainer splitBanHang;
        private SplitContainer splitKho;

        public formDashBoard()
        {
            InitializeComponent();
        }

        private void formDashBoard_Load(object sender, EventArgs e)
        {
            
            InitializeBanHangFilters();
            InitializeKhoFilters();
            CreateBanHangChart();
            CreateKhoChart();          // tạo chart kho
           

        }
        private void InitializeKhoFilters()
        {
            var today = DateTime.Today;
            dtpFromKho.Value = today.AddDays(-5);
            dtpToKho.Value = today;

            cboLoaiThongKeKho.Items.Clear();
            cboLoaiThongKeKho.Items.Add("Tồn kho hiện tại");
            cboLoaiThongKeKho.Items.Add("Nhập - Xuất theo ngày");
            if (cboLoaiThongKeKho.Items.Count > 0)
            {
                cboLoaiThongKeKho.SelectedIndex = 0;
            }

            ClearKhoSummaryFields();
            dgvKho.DataSource = null;
            if (dgvKho.Rows.Count > 0)
            {
                dgvKho.Rows.Clear();
            }
            dgvKho.Columns.Clear();
        }

       

        private void InitializeBanHangFilters()
        {
            var today = DateTime.Today;
            dtpFromBH.Value = today.AddDays(-5);
            dtpToBH.Value = today;
            dtpToBH.MaxDate = today;
            cboLoaiThongKeBH.Items.Clear();
            cboLoaiThongKeBH.Items.Add("Doanh thu theo ngày");
            cboLoaiThongKeBH.Items.Add("Top sản phẩm bán chạy");
            if (cboLoaiThongKeBH.Items.Count > 0)
            {
                cboLoaiThongKeBH.SelectedIndex = 0;
            }

            ClearSummaryFields();
        }

        private void btnThongKeBH_Click(object sender, EventArgs e)
        {
            if (cboLoaiThongKeBH.SelectedItem == null)
            {
                MessageBox.Show("Vui lòng chọn loại thống kê.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var fromDate = dtpFromBH.Value.Date;
            var toDate = dtpToBH.Value.Date;

            if (fromDate > toDate)
            {
                MessageBox.Show("Ngày bắt đầu phải nhỏ hơn hoặc bằng ngày kết thúc.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var selectedReport = cboLoaiThongKeBH.SelectedItem.ToString();

            try
            {
                if (selectedReport == "Doanh thu theo ngày")
                {
                    LoadRevenueByDate(fromDate, toDate);
                }
                else if (selectedReport == "Top sản phẩm bán chạy")
                {
                    LoadTopSellingProducts(fromDate, toDate);
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show($"Đã xảy ra lỗi khi truy vấn dữ liệu: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void LoadRevenueByDate(DateTime fromDate, DateTime toDate)
        {
            const string query = @"
        SELECT CONVERT(date, hd.NgayLap) AS Ngay,
               SUM(hd.TongTien)            AS TongDoanhThu,
               COUNT(*)                    AS TongHoaDon,
               SUM(ISNULL(ct.SoLuong,0))   AS TongSPBan
        FROM HoaDon hd
        LEFT JOIN ChiTietHoaDon ct ON ct.MaHD = hd.MaHD
        WHERE hd.NgayLap >= @FromDate AND hd.NgayLap < @ToDateExclusive
        GROUP BY CONVERT(date, hd.NgayLap)
        ORDER BY Ngay";

            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand(query, connection))
            {
                command.Parameters.Add("@FromDate", SqlDbType.Date).Value = fromDate;
                command.Parameters.Add("@ToDateExclusive", SqlDbType.Date).Value = toDate.AddDays(1);

                var adapter = new SqlDataAdapter(command);
                var dataTable = new DataTable();
                adapter.Fill(dataTable);

                dgvBanHang.DataSource = dataTable;
                UpdateBanHangChartForRevenue(dataTable);

                var totalRevenue = dataTable.AsEnumerable()
                    .Sum(row => row.Field<decimal?>("TongDoanhThu") ?? 0m);
                var totalInvoices = dataTable.AsEnumerable()
                    .Sum(row => row.Field<int?>("TongHoaDon") ?? 0);
                var totalQty = dataTable.AsEnumerable()
                    .Sum(row => row.Field<int?>("TongSPBan") ?? 0);

                txtTongDoanhThu.Text = totalRevenue.ToString("N2");
                txtTongSoHoaDon.Text = totalInvoices.ToString();
                txtTongSoSPbanra.Text = totalQty.ToString();
            }
        }


        private void LoadTopSellingProducts(DateTime fromDate, DateTime toDate)
        {
            const string query = @"
                SELECT TOP 10 sp.MaSP,
                               sp.TenSP,
                               SUM(ct.SoLuong) AS TongSoLuong,
                               SUM(ct.SoLuong * ct.DonGia) AS TongDoanhThu
                FROM ChiTietHoaDon ct
                INNER JOIN HoaDon hd ON ct.MaHD = hd.MaHD
                INNER JOIN SanPham sp ON ct.MaSP = sp.MaSP
                WHERE hd.NgayLap >= @FromDate AND hd.NgayLap < @ToDateExclusive
                GROUP BY sp.MaSP, sp.TenSP
                ORDER BY TongSoLuong DESC, sp.TenSP";

            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand(query, connection))
            {
                command.Parameters.Add("@FromDate", SqlDbType.Date).Value = fromDate;
                command.Parameters.Add("@ToDateExclusive", SqlDbType.Date).Value = toDate.AddDays(1);

                var adapter = new SqlDataAdapter(command);
                var dataTable = new DataTable();
                adapter.Fill(dataTable);

                dgvBanHang.DataSource = dataTable;

                // Cập nhật biểu đồ cột
                // Đúng
                UpdateBanHangChartForTopProducts(dataTable);

                var totalQuantity = dataTable.AsEnumerable()
                    .Sum(row => row.Field<int?>("TongSoLuong") ?? 0);
                var totalRevenue = dataTable.AsEnumerable()
                    .Sum(row => row.Field<decimal?>("TongDoanhThu") ?? 0m);
                var totalInvoices = GetInvoiceCount(fromDate, toDate);

                txtTongSoSPbanra.Text = totalQuantity.ToString();
                txtTongDoanhThu.Text = totalRevenue.ToString("N2");
                txtTongSoHoaDon.Text = totalInvoices.ToString();
            }
        }
        private int GetInvoiceCount(DateTime fromDate, DateTime toDate)
        {
            const string query = @"
                SELECT COUNT(*)
                FROM HoaDon
                WHERE NgayLap >= @FromDate AND NgayLap < @ToDateExclusive";

            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand(query, connection))
            {
                command.Parameters.Add("@FromDate", SqlDbType.Date).Value = fromDate;
                command.Parameters.Add("@ToDateExclusive", SqlDbType.Date).Value = toDate.AddDays(1);

                connection.Open();
                var result = command.ExecuteScalar();
                return result != null ? Convert.ToInt32(result) : 0;
            }
        }

        private void ClearSummaryFields()
        {
            txtTongDoanhThu.Text = string.Empty;
            txtTongSoHoaDon.Text = string.Empty;
            txtTongSoSPbanra.Text = string.Empty;
        }

        private void btnThongKeKho_Click(object sender, EventArgs e)
        {
            if (cboLoaiThongKeKho.SelectedItem == null)
            {
                MessageBox.Show("Vui lòng chọn loại thống kê.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var fromDate = dtpFromKho.Value.Date;
            var toDate = dtpToKho.Value.Date;

            if (fromDate > toDate)
            {
                MessageBox.Show("Ngày bắt đầu phải nhỏ hơn hoặc bằng ngày kết thúc.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            ClearKhoSummaryFields();

            var selectedReport = cboLoaiThongKeKho.SelectedItem.ToString();

            try
            {
                if (selectedReport == "Tồn kho hiện tại")
                {
                    LoadTonKhoHienTai();
                }
                else if (selectedReport == "Nhập - Xuất theo ngày")
                {
                    LoadNhapXuatTheoNgay(fromDate, toDate);
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show($"Đã xảy ra lỗi khi truy vấn dữ liệu: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void ClearKhoSummaryFields()
        {
            txtTongSoSP.Text = string.Empty;
            txtTongSLTon.Text = string.Empty;
            txtSLNhap.Text = string.Empty;
            txtSLXuat.Text = string.Empty;
        }
        private void CreateKhoChart()
        {
            if (chartKho != null) return;

            chartKho = new Chart();
            var area = new ChartArea("KhoArea");
            area.AxisX.MajorGrid.Enabled = false;
            area.AxisY.MajorGrid.LineDashStyle = ChartDashStyle.Dot;
            area.AxisX.Interval = 1;
            chartKho.ChartAreas.Add(area);

            var legend = new Legend("LegendKho");
            chartKho.Legends.Add(legend);

            chartKho.Dock = DockStyle.Bottom;
            chartKho.Height = 260;
            this.tabPageKho.Controls.Add(chartKho);

        }
        private void UpdateKhoChartForTonKho(DataTable data)
        {
            CreateKhoChart();
            chartKho.Series.Clear();
            chartKho.Titles.Clear();

            // Lấy Top 10 theo SoLuong (giảm dần)
            var top = data.AsEnumerable()
                          .OrderByDescending(r => r.Field<int?>("SoLuong") ?? 0)
                          .Take(10)
                          .Select(r => new
                          {
                              TenSP = Convert.ToString(r["TenSP"]) ?? "?",
                              SoLuong = Convert.ToDouble(r["SoLuong"] ?? 0)
                          })
                          .ToList();

            var series = new Series("Tồn kho")
            {
                ChartType = SeriesChartType.Column,
                XValueType = ChartValueType.String,
                YValueType = ChartValueType.Double,
                IsValueShownAsLabel = true,
                IsXValueIndexed = true
            };

            foreach (var x in top)
                series.Points.AddXY(x.TenSP, x.SoLuong);

            chartKho.Series.Add(series);
            chartKho.Titles.Add("Top 10 tồn kho hiện tại");

            var area = chartKho.ChartAreas["KhoArea"];
            area.AxisY.Title = "Số lượng tồn";
        }
        private void UpdateKhoChartForNhapXuat(DataTable data)
        {
            CreateKhoChart();
            chartKho.Series.Clear();
            chartKho.Titles.Clear();

            var sNhap = new Series("Nhập")
            {
                ChartType = SeriesChartType.Column,
                XValueType = ChartValueType.String,
                YValueType = ChartValueType.Double,
                IsValueShownAsLabel = true,
                IsXValueIndexed = true
            };

            var sXuat = new Series("Xuất")
            {
                ChartType = SeriesChartType.Column,
                XValueType = ChartValueType.String,
                YValueType = ChartValueType.Double,
                IsValueShownAsLabel = true,
                IsXValueIndexed = true
            };

            foreach (DataRow row in data.Rows)
            {
                var ngay = (DateTime)row["Ngay"];
                var tongNhap = Convert.ToDouble(row["TongNhap"] ?? 0);
                var tongXuat = Convert.ToDouble(row["TongXuat"] ?? 0);
                var x = ngay.ToString("dd/MM");

                sNhap.Points.AddXY(x, tongNhap);
                sXuat.Points.AddXY(x, tongXuat);
            }

            chartKho.Series.Add(sNhap);
            chartKho.Series.Add(sXuat);
            chartKho.Titles.Add("Nhập - Xuất theo ngày");

            var area = chartKho.ChartAreas["KhoArea"];
            area.AxisY.Title = "Số lượng";
        }

        private void LoadTonKhoHienTai()
        {
            const string query = @"
                SELECT MaSP,
                       TenSP,
                       SoLuong,
                       DonGia,
                       SoLuong * DonGia AS GiaTriTon,
                       CASE
                           WHEN SoLuong <= 0 THEN N'Hết hàng'
                           WHEN SoLuong < 10 THEN N'Sắp hết'
                           ELSE N'Còn hàng'
                       END AS TrangThai
                FROM SanPham";

            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand(query, connection))
            {
                var adapter = new SqlDataAdapter(command);
                var dataTable = new DataTable();
                adapter.Fill(dataTable);

                dgvKho.DataSource = dataTable;
                UpdateKhoChartForTonKho(dataTable);


                var totalProducts = dataTable.Rows.Count;
                var totalQuantity = dataTable.AsEnumerable()
                    .Sum(row => row.Field<int?>("SoLuong") ?? 0);

                txtTongSoSP.Text = totalProducts.ToString();
                txtTongSLTon.Text = totalQuantity.ToString();
            }
        }

        private void LoadNhapXuatTheoNgay(DateTime fromDate, DateTime toDate)
        {
            const string query = @"
                SELECT CONVERT(date, gd.NgayGD) AS Ngay,
                       SUM(CASE WHEN gd.LoaiGD = 'NHAP' THEN ctk.SoLuong ELSE 0 END) AS TongNhap,
                       SUM(CASE WHEN gd.LoaiGD = 'XUAT' THEN ctk.SoLuong ELSE 0 END) AS TongXuat
                FROM GiaoDichKho gd
                INNER JOIN ChiTietGiaoDichKho ctk ON gd.MaGD = ctk.MaGD
                WHERE gd.NgayGD >= @FromDate AND gd.NgayGD < DATEADD(day, 1, @ToDate)
                GROUP BY CONVERT(date, gd.NgayGD)
                ORDER BY Ngay";

            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand(query, connection))
            {
                command.Parameters.Add("@FromDate", SqlDbType.DateTime).Value = fromDate;
                command.Parameters.Add("@ToDate", SqlDbType.DateTime).Value = toDate;

                var adapter = new SqlDataAdapter(command);
                var dataTable = new DataTable();
                adapter.Fill(dataTable);

                dgvKho.DataSource = dataTable;
                UpdateKhoChartForNhapXuat(dataTable);


                var totalImport = dataTable.AsEnumerable()
                    .Sum(row => row.Field<int?>("TongNhap") ?? 0);
                var totalExport = dataTable.AsEnumerable()
                    .Sum(row => row.Field<int?>("TongXuat") ?? 0);

                txtSLNhap.Text = totalImport.ToString();
                txtSLXuat.Text = totalExport.ToString();
            }
        }
        private void CreateBanHangChart()
        {
            // Create chart if not exists
            if (chartBanHang != null) return;

            chartBanHang = new Chart();
            var area = new ChartArea("MainArea");
            area.AxisX.MajorGrid.Enabled = false;
            area.AxisY.MajorGrid.LineDashStyle = ChartDashStyle.Dot;
            area.AxisX.Interval = 1;
            chartBanHang.ChartAreas.Add(area);

            var legend = new Legend("Legend1");
            chartBanHang.Legends.Add(legend);

            chartBanHang.Dock = DockStyle.Bottom;   // ⟵ đổi từ Bottom → Fill
            chartBanHang.Height = 260;
            this.tabPageBanHang.Controls.Add(chartBanHang);
        }


        private void UpdateBanHangChartForRevenue(DataTable data)
        {
            CreateBanHangChart();
            chartBanHang.Series.Clear();
            chartBanHang.Titles.Clear();

            var sRevenue = new Series("Doanh thu")
            {
                ChartType = SeriesChartType.Column,
                XValueType = ChartValueType.String,
                YValueType = ChartValueType.Double,
                IsValueShownAsLabel = true
            };

            var sQty = new Series("Số lượng bán")
            {
                ChartType = SeriesChartType.Line,    // hoặc Column nếu bạn muốn
                XValueType = ChartValueType.String,
                YValueType = ChartValueType.Double,
                IsValueShownAsLabel = true,
                YAxisType = AxisType.Secondary       // dùng trục Y phụ cho số lượng
            };

            foreach (DataRow row in data.Rows)
            {
                var ngay = (DateTime)row["Ngay"];
                var doanhThu = Convert.ToDouble(row["TongDoanhThu"] ?? 0);
                var soLuong = Convert.ToDouble(row["TongSPBan"] ?? 0);

                var x = ngay.ToString("dd/MM");
                sRevenue.Points.AddXY(x, doanhThu);
                sQty.Points.AddXY(x, soLuong);
            }

            chartBanHang.Series.Add(sRevenue);
            chartBanHang.Series.Add(sQty);

            // Tiêu đề & nhãn trục
            chartBanHang.Titles.Add("Doanh thu theo ngày");
            var area = chartBanHang.ChartAreas["MainArea"];
            area.AxisY.Title = "Doanh thu";
            area.AxisY2.Enabled = AxisEnabled.True;
            area.AxisY2.Title = "Số lượng";
        }


        private void UpdateBanHangChartForTopProducts(DataTable data)
        {
            CreateBanHangChart();
            chartBanHang.Series.Clear();

            var series = new Series("Số lượng bán")
            {
                ChartType = SeriesChartType.Column,
                XValueType = ChartValueType.String,
                YValueType = ChartValueType.Double,
                IsValueShownAsLabel = true
            };

            foreach (DataRow row in data.Rows)
            {
                var ten = Convert.ToString(row["TenSP"]) ?? "?";
                var sl = Convert.ToDouble(row["TongSoLuong"] ?? 0);
                series.Points.AddXY(ten, sl);
            }

            chartBanHang.Series.Add(series);
            chartBanHang.Titles.Clear();
            chartBanHang.Titles.Add("Top sản phẩm bán chạy");
        }

        private void dtpToBH_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter)
            {
                return;
            }

            var today = DateTime.Today;
            if (dtpToBH.Value.Date > today)
            {
                MessageBox.Show("Ngày kết thúc không được lớn hơn ngày hiện tại.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                dtpToBH.Value = today;
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
        }
    }
}
