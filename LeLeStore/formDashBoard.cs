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

namespace LeLeStore
{
    public partial class formDashBoard : Form
    {
        private readonly string _connectionString = Properties.Settings.Default.GStoreConnectionString;
        public formDashBoard()
        {
            InitializeComponent();
        }

        private void formDashBoard_Load(object sender, EventArgs e)
        {
            
            InitializeBanHangFilters();
        }
        private void InitializeBanHangFilters()
        {
            var today = DateTime.Today;
            dtpFromBH.Value = today.AddDays(-5);
            dtpToBH.Value = today;

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
                SELECT CONVERT(date, NgayLap) AS Ngay,
                       SUM(TongTien) AS TongDoanhThu,
                       COUNT(*) AS TongHoaDon
                FROM HoaDon
                WHERE NgayLap >= @FromDate AND NgayLap < @ToDateExclusive
                GROUP BY CONVERT(date, NgayLap)
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

                var totalRevenue = dataTable.AsEnumerable()
                    .Sum(row => row.Field<decimal?>("TongDoanhThu") ?? 0m);
                var totalInvoices = dataTable.AsEnumerable()
                    .Sum(row => row.Field<int?>("TongHoaDon") ?? 0);

                txtTongDoanhThu.Text = totalRevenue.ToString("N2");
                txtTongSoHoaDon.Text = totalInvoices.ToString();
                txtTongSoSPbanra.Text = string.Empty;
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
    }
}
