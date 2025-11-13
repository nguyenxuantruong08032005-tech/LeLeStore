using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Excel = Microsoft.Office.Interop.Excel;

namespace LeLeStore
{
    public partial class formDanhSachTinhLuong : Form
    {
        private readonly string connectionString;
        public formDanhSachTinhLuong()
        {
            InitializeComponent();
            connectionString = ConfigurationManager
                 .ConnectionStrings["LeLeStore.Properties.Settings.GStoreConnectionString"]?.ConnectionString
                 ?? throw new InvalidOperationException("Không tìm thấy chuỗi kết nối GStore.");
        }

        private void formDanhSachTinhLuong_Load(object sender, EventArgs e)
        {
            LoadBangLuong(null);
        }

        private void btnXem_Click(object sender, EventArgs e)
        {
            string ky = txtKyFilter.Text.Trim();
            if (string.IsNullOrEmpty(ky))
            {
                MessageBox.Show("Vui lòng nhập kỳ lương (ví dụ: 2025-10) hoặc dùng nút 'Xem tất cả'.");
                return;
            }

            LoadBangLuong(ky);
        }

        private void btnXemTatCa_Click(object sender, EventArgs e)
        {
            txtKyFilter.Clear();
            LoadBangLuong(null);
        }
        private void LoadBangLuong(string ky)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string sql = @"
SELECT
    c.MaNV,
    nv.HoTen,
    nv.ChucVu,
    c.Ky,
    c.SoNgayLam,
    c.SoGioLam,
    c.LuongTheoGio,
    c.LuongCoBan,
    c.HeSoLuong,
    c.DoanhThuCa,
    c.HoaHongBanHang,
    c.DoanhThuKhuVuc,
    c.TyLeDoanhThu,
    c.PhuCap,
    c.PhuCapCaDem,
    c.PhuCapQuanLy,
    c.Thuong,
    c.ThuongQTHT,
    c.KhauTru,

    -- Các cột tính toán giống form Tính lương
    ISNULL(c.SoGioLam,0) * ISNULL(c.LuongTheoGio,0)                                  AS LuongGio,
    ISNULL(c.LuongCoBan,0) * ISNULL(c.HeSoLuong,0)                                    AS LuongCoBanTinh,
    ISNULL(c.DoanhThuCa,0) * ISNULL(c.HoaHongBanHang,0)                               AS TienHoaHong,
    ISNULL(c.DoanhThuKhuVuc,0) * ISNULL(c.TyLeDoanhThu,0)                             AS LuongKPI,
    (ISNULL(c.PhuCap,0) + ISNULL(c.PhuCapCaDem,0) + ISNULL(c.PhuCapQuanLy,0))         AS TongPhuCap,
    (
        ISNULL(c.SoGioLam,0)    * ISNULL(c.LuongTheoGio,0) +
        ISNULL(c.LuongCoBan,0)  * ISNULL(c.HeSoLuong,0) +
        ISNULL(c.DoanhThuCa,0)  * ISNULL(c.HoaHongBanHang,0) +
        ISNULL(c.DoanhThuKhuVuc,0) * ISNULL(c.TyLeDoanhThu,0) +
        (ISNULL(c.PhuCap,0) + ISNULL(c.PhuCapCaDem,0) + ISNULL(c.PhuCapQuanLy,0)) +
        ISNULL(c.Thuong,0) +
        ISNULL(c.ThuongQTHT,0)
    ) AS TongThuNhap,
    (
        (
            ISNULL(c.SoGioLam,0)    * ISNULL(c.LuongTheoGio,0) +
            ISNULL(c.LuongCoBan,0)  * ISNULL(c.HeSoLuong,0) +
            ISNULL(c.DoanhThuCa,0)  * ISNULL(c.HoaHongBanHang,0) +
            ISNULL(c.DoanhThuKhuVuc,0) * ISNULL(c.TyLeDoanhThu,0) +
            (ISNULL(c.PhuCap,0) + ISNULL(c.PhuCapCaDem,0) + ISNULL(c.PhuCapQuanLy,0)) +
            ISNULL(c.Thuong,0) +
            ISNULL(c.ThuongQTHT,0)
        ) - ISNULL(c.KhauTru,0)
    ) AS LuongThucNhan
FROM dbo.CongNV c
JOIN dbo.NhanVien nv ON c.MaNV = nv.MaNhanVien
WHERE (@Ky IS NULL OR c.Ky = @Ky)
ORDER BY c.Ky, nv.HoTen;";

                SqlCommand cmd = new SqlCommand(sql, conn);
                if (string.IsNullOrEmpty(ky))
                    cmd.Parameters.AddWithValue("@Ky", DBNull.Value);
                else
                    cmd.Parameters.AddWithValue("@Ky", ky);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                dgvBangLuong.DataSource = dt;

                // tuỳ chọn: đặt caption cột đẹp hơn
                if (dgvBangLuong.Columns.Count > 0)
                {
                    dgvBangLuong.Columns["MaNV"].HeaderText = "Mã NV";
                    dgvBangLuong.Columns["HoTen"].HeaderText = "Họ tên";
                    dgvBangLuong.Columns["ChucVu"].HeaderText = "Chức vụ";
                    dgvBangLuong.Columns["Ky"].HeaderText = "Kỳ";

                    dgvBangLuong.Columns["LuongGio"].HeaderText = "Lương giờ";
                    dgvBangLuong.Columns["LuongCoBanTinh"].HeaderText = "Lương cơ bản";
                    dgvBangLuong.Columns["TienHoaHong"].HeaderText = "Hoa hồng";
                    dgvBangLuong.Columns["LuongKPI"].HeaderText = "Lương KPI";
                    dgvBangLuong.Columns["TongPhuCap"].HeaderText = "Tổng phụ cấp";
                    dgvBangLuong.Columns["TongThuNhap"].HeaderText = "Tổng thu nhập";
                    dgvBangLuong.Columns["LuongThucNhan"].HeaderText = "Thực nhận";
                }
            }
        }

        private void btnExportExcel_Click(object sender, EventArgs e)
        {
            if (dgvBangLuong.Rows.Count == 0)
            {
                MessageBox.Show("Không có dữ liệu để xuất.", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.Filter = "Excel Workbook|*.xlsx";
                sfd.Title = "Chọn nơi lưu file Excel";
                sfd.FileName = "BangLuong.xlsx";

                if (sfd.ShowDialog() != DialogResult.OK)
                    return;

                Excel.Application app = null;
                Excel.Workbook workbook = null;
                Excel._Worksheet ws = null;

                try
                {
                    app = new Excel.Application();
                    workbook = app.Workbooks.Add(Type.Missing);
                    ws = workbook.ActiveSheet;

                    ws.Name = "BangLuong";

                    int colCount = dgvBangLuong.Columns.Count;
                    int rowCount = dgvBangLuong.Rows.Count;

                    // ===========================
                    // 1) TIÊU ĐỀ LỚN
                    // ===========================
                    ws.Range["A1", GetExcelColumnName(colCount) + "1"].Merge();
                    ws.Range["A1"].Value = "BẢNG LƯƠNG NHÂN VIÊN";
                    ws.Range["A1"].Font.Size = 18;
                    ws.Range["A1"].Font.Bold = true;
                    ws.Range["A1"].HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;

                    // ===========================
                    // 2) GHI HEADER DỮ LIỆU
                    // ===========================
                    for (int col = 0; col < colCount; col++)
                    {
                        ws.Cells[3, col + 1] = dgvBangLuong.Columns[col].HeaderText;
                    }

                    // Style cho header
                    var headerRange = ws.Range["A3", GetExcelColumnName(colCount) + "3"];
                    headerRange.Font.Bold = true;
                    headerRange.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.LightGray);
                    headerRange.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                    headerRange.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;

                    // ===========================
                    // 3) GHI DỮ LIỆU
                    // ===========================
                    for (int row = 0; row < rowCount; row++)
                    {
                        for (int col = 0; col < colCount; col++)
                        {
                            object value = dgvBangLuong.Rows[row].Cells[col].Value;
                            ws.Cells[row + 4, col + 1] = value;
                        }
                    }

                    // ===========================
                    // 4) TẠO BORDER CHO TOÀN BẢNG
                    // ===========================
                    var fullTable = ws.Range[
                        "A3",
                        GetExcelColumnName(colCount) + (rowCount + 3).ToString()
                    ];
                    fullTable.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;

                    // ===========================
                    // 5) FORMAT TIỀN TỆ
                    // Chỉ format các cột có liên quan đến tiền
                    // ===========================
                    string[] moneyColumns =
                    {
                "LuongGio", "LuongCoBanTinh", "TienHoaHong",
                "LuongKPI", "TongPhuCap", "TongThuNhap", "LuongThucNhan"
            };

                    for (int col = 0; col < colCount; col++)
                    {
                        string colName = dgvBangLuong.Columns[col].Name;
                        if (moneyColumns.Contains(colName))
                        {
                            string excelCol = GetExcelColumnName(col + 1);
                            var moneyRange = ws.Range[
                                excelCol + "4",
                                excelCol + (rowCount + 3)
                            ];
                            moneyRange.NumberFormat = "#,##0";
                        }
                    }

                    // ===========================
                    // 6) AUTO FIT CỘT
                    // ===========================
                    ws.Columns.AutoFit();

                    // ===========================
                    // 7) LƯU FILE
                    // ===========================
                    workbook.SaveAs(sfd.FileName);
                    workbook.Close();
                    app.Quit();

                    MessageBox.Show("Xuất Excel thành công!", "Thành công",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi xuất Excel: " + ex.Message, "Lỗi",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    if (ws != null) System.Runtime.InteropServices.Marshal.ReleaseComObject(ws);
                    if (workbook != null) System.Runtime.InteropServices.Marshal.ReleaseComObject(workbook);
                    if (app != null) System.Runtime.InteropServices.Marshal.ReleaseComObject(app);
                    ws = null;
                    workbook = null;
                    app = null;
                    GC.Collect();
                }
            }
        }
        private string GetExcelColumnName(int columnNumber)
        {
            string columnName = "";
            while (columnNumber > 0)
            {
                int modulo = (columnNumber - 1) % 26;
                columnName = Convert.ToChar('A' + modulo) + columnName;
                columnNumber = (columnNumber - modulo) / 26;
            }
            return columnName;
        }
    }
}
