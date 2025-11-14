using System;

using System.Data;
using System.Collections.Generic;

using System.Windows.Forms;
using System.Configuration;
using System.Data.SqlClient;
using System.Globalization;
namespace LeLeStore
{
    public partial class formEmployeeSalary : Form
    {
        private readonly string connectionString;
        private readonly TextBox[] managedTextBoxes;
        private readonly Dictionary<string, TextBox[]> roleTextBoxes;
        public formEmployeeSalary()
        {
            InitializeComponent();
            connectionString = ConfigurationManager
                  .ConnectionStrings["LeLeStore.Properties.Settings.GStoreConnectionString"]?.ConnectionString
                  ?? throw new InvalidOperationException("Không tìm thấy chuỗi kết nối GStore.");
            managedTextBoxes = new[]
           {
                txtKy,
                txtHeSoLuong,
                txtLuongCoBan,
                txtLuongTheoGio,
                txtSoGioLam,
                txtSoNgayLam,
                txtTyLeDoanhThu,
                txtDoanhThuKhuVuc,
                txtHoaHongBanHang,
                txtDoanhThuCa,
                txtKhauTru,
                txtThuongQTHT,
                txtThuong,
                txtPhuCapQuanLy,
                txtPhuCapCaDem,
                txtPhuCap,
                txtLuongThucNhan,
                txtTongThuNhap,
                txtTongPhuCap,
                txtTienHoaHong,
                txtLuongKPI,
                txtLuongCoBanTinh,
                txtLuongGio
            };

            roleTextBoxes = new Dictionary<string, TextBox[]>(StringComparer.CurrentCultureIgnoreCase)
            {
                {
                    "Nhân viên bán hàng",
                    new[]
                    {
                        txtSoNgayLam,
                        txtSoGioLam,
                        txtLuongTheoGio,
                        txtLuongCoBan,
                        txtHeSoLuong,
                        txtDoanhThuCa,
                        txtHoaHongBanHang,
                        txtPhuCap,
                        txtThuong,
                        txtKhauTru
                    }
                },
                {
                    "Nhân viên kho",
                    new[]
                    {
                        txtSoNgayLam,
                        txtSoGioLam,
                        txtLuongTheoGio,
                        txtLuongCoBan,
                        txtHeSoLuong,
                        txtPhuCap,
                        txtThuong,
                        txtKhauTru,
                        txtPhuCapCaDem
                    }
                },
                {
                    "Quản lý cửa hàng",
                    new[]
                    {
                        txtSoNgayLam,
                        txtSoGioLam,
                        txtLuongCoBan,
                        txtHeSoLuong,
                        txtPhuCap,
                        txtThuong,
                        txtKhauTru,
                        txtDoanhThuKhuVuc,
                        txtThuongQTHT
                    }
                }
            };
            Load += formEmployeeSalary_Load;
            cboNhanVien.SelectedIndexChanged += cboNhanVien_SelectedIndexChanged;
            btnTinhLuong.Click += btnTinhLuong_Click;
            btnLuu.Click += btnLuu_Click;
            btnThoat.Click += (sender, e) => Close();

            SetResultTextBoxesReadOnly();
            DisableAllManagedTextBoxes();
        }

        private void groupBox5_Enter(object sender, EventArgs e)
        {

        }

        private void formEmployeeSalary_Load(object sender, EventArgs e)
        {
            try
            {
                LoadNhanVien();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Không thể tải danh sách nhân viên. Chi tiết: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void LoadNhanVien()
        {
            using (var connection = new SqlConnection(connectionString))
            using (var command = new SqlCommand("SELECT MaNhanVien, HoTen, ChucVu FROM NhanVien ORDER BY HoTen", connection))
            using (var adapter = new SqlDataAdapter(command))
            {
                var table = new DataTable();
                adapter.Fill(table);

                cboNhanVien.DisplayMember = "HoTen";
                cboNhanVien.ValueMember = "MaNhanVien";
                cboNhanVien.DataSource = table;
            }
            cboNhanVien.SelectedIndex = -1;
            UpdateNhanVienInfo();
        }

        private void cboNhanVien_SelectionChangeCommitted(object sender, EventArgs e)
        {

        }

        private void cboNhanVien_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateNhanVienInfo();
        }
        private void UpdateNhanVienInfo()
        {
            if (cboNhanVien.SelectedItem is DataRowView rowView)
            {
                txtHoTen.Text = rowView["HoTen"].ToString();
                var chucVu = rowView["ChucVu"] == DBNull.Value ? string.Empty : rowView["ChucVu"].ToString();
                txtChucVu.Text = chucVu;
                EnableTextBoxesForRole(chucVu);
                if (string.Equals(chucVu, "Nhân viên bán hàng", StringComparison.CurrentCultureIgnoreCase)
                    && int.TryParse(rowView["MaNhanVien"].ToString(), out int maNhanVien))
                {
                    try
                    {
                        var doanhThuCa = CalculateSalesRevenue(maNhanVien);
                        txtDoanhThuCa.Text = FormatCurrency(doanhThuCa);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Không thể tính doanh thu ca: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        txtDoanhThuCa.Text = string.Empty;
                    }
                }
                else
                {
                    txtDoanhThuCa.Text = string.Empty;
                }
            }
            else
            {
                txtHoTen.Text = string.Empty;
                txtChucVu.Text = string.Empty;
            }
        }

        private void btnTinhLuong_Click(object sender, EventArgs e)
        {
            try
            {
                int soNgayLam = GetIntValue(txtSoNgayLam);
                decimal soGioLam = GetDecimalValue(txtSoGioLam);
                decimal luongTheoGio = GetDecimalValue(txtLuongTheoGio);
                decimal luongCoBan = GetDecimalValue(txtLuongCoBan);
                decimal heSoLuong = GetDecimalValue(txtHeSoLuong);
                decimal doanhThuCa = GetDecimalValue(txtDoanhThuCa);
                decimal hoaHongBanHang = GetDecimalValue(txtHoaHongBanHang);
                decimal doanhThuKhuVuc = GetDecimalValue(txtDoanhThuKhuVuc);
                decimal tyLeDoanhThu = GetDecimalValue(txtTyLeDoanhThu);
                decimal phuCap = GetDecimalValue(txtPhuCap);
                decimal phuCapCaDem = GetDecimalValue(txtPhuCapCaDem);
                decimal phuCapQuanLy = GetDecimalValue(txtPhuCapQuanLy);
                decimal thuong = GetDecimalValue(txtThuong);
                decimal thuongQTHT = GetDecimalValue(txtThuongQTHT);
                decimal khauTru = GetDecimalValue(txtKhauTru);

                decimal luongGio = soGioLam * luongTheoGio;
                decimal luongCoBanTinh = luongCoBan * heSoLuong;
                decimal tienHoaHong = doanhThuCa * hoaHongBanHang;
                decimal luongKPI = doanhThuKhuVuc * tyLeDoanhThu;
                decimal tongPhuCap = phuCap + phuCapCaDem + phuCapQuanLy;
                decimal tongThuNhap = luongGio + luongCoBanTinh + tienHoaHong + luongKPI + tongPhuCap + thuong + thuongQTHT;
                decimal luongThucNhan = tongThuNhap - khauTru;

                txtLuongGio.Text = FormatCurrency(luongGio);
                txtLuongCoBanTinh.Text = FormatCurrency(luongCoBanTinh);
                txtTienHoaHong.Text = FormatCurrency(tienHoaHong);
                txtLuongKPI.Text = FormatCurrency(luongKPI);
                txtTongPhuCap.Text = FormatCurrency(tongPhuCap);
                txtTongThuNhap.Text = FormatCurrency(tongThuNhap);
                txtLuongThucNhan.Text = FormatCurrency(luongThucNhan);
            }
            catch (FormatException ex)
            {
                MessageBox.Show(ex.Message, "Giá trị không hợp lệ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Có lỗi xảy ra khi tính lương: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnLuu_Click(object sender, EventArgs e)
        {
            if (cboNhanVien.SelectedValue == null)
            {
                MessageBox.Show("Vui lòng chọn nhân viên.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            string ky = txtKy.Text.Trim();
            if (string.IsNullOrEmpty(ky))
            {
                MessageBox.Show("Vui lòng nhập kỳ lương.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                int maNv = Convert.ToInt32(cboNhanVien.SelectedValue);
                int soNgayLam = GetIntValue(txtSoNgayLam);
                decimal soGioLam = GetDecimalValue(txtSoGioLam);
                decimal luongTheoGio = GetDecimalValue(txtLuongTheoGio);
                decimal doanhThuCa = GetDecimalValue(txtDoanhThuCa);
                decimal hoaHongBanHang = GetDecimalValue(txtHoaHongBanHang);
                decimal doanhThuKhuVuc = GetDecimalValue(txtDoanhThuKhuVuc);
                decimal tyLeDoanhThu = GetDecimalValue(txtTyLeDoanhThu);
                decimal luongCoBan = GetDecimalValue(txtLuongCoBan);
                decimal heSoLuong = GetDecimalValue(txtHeSoLuong);
                decimal phuCap = GetDecimalValue(txtPhuCap);
                decimal thuong = GetDecimalValue(txtThuong);
                decimal khauTru = GetDecimalValue(txtKhauTru);
                decimal phuCapCaDem = GetDecimalValue(txtPhuCapCaDem);
                decimal phuCapQuanLy = GetDecimalValue(txtPhuCapQuanLy);
                decimal thuongQTHT = GetDecimalValue(txtThuongQTHT);

                UpsertCongNv(maNv, ky, soNgayLam, soGioLam, luongTheoGio, doanhThuCa, hoaHongBanHang, doanhThuKhuVuc,
                    tyLeDoanhThu, luongCoBan, heSoLuong, phuCap, thuong, khauTru, phuCapCaDem, phuCapQuanLy, thuongQTHT);

                MessageBox.Show("Lưu dữ liệu công nhân viên thành công.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (FormatException ex)
            {
                MessageBox.Show(ex.Message, "Giá trị không hợp lệ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Không thể lưu dữ liệu: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void UpsertCongNv(int maNv, string ky, int soNgayLam, decimal soGioLam, decimal luongTheoGio,
          decimal doanhThuCa, decimal hoaHongBanHang, decimal doanhThuKhuVuc, decimal tyLeDoanhThu, decimal luongCoBan,
          decimal heSoLuong, decimal phuCap, decimal thuong, decimal khauTru, decimal phuCapCaDem, decimal phuCapQuanLy,
          decimal thuongQTHT)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();

                bool exists;
                using (var checkCmd = new SqlCommand("SELECT COUNT(1) FROM CongNV WHERE MaNV = @MaNV AND Ky = @Ky", connection))
                {
                    checkCmd.Parameters.AddWithValue("@MaNV", maNv);
                    checkCmd.Parameters.AddWithValue("@Ky", ky);
                    exists = Convert.ToInt32(checkCmd.ExecuteScalar()) > 0;
                }

                string sql;
                if (exists)
                {
                    sql = @"UPDATE CongNV
SET SoNgayLam = @SoNgayLam,
    SoGioLam = @SoGioLam,
    LuongTheoGio = @LuongTheoGio,
    DoanhThuCa = @DoanhThuCa,
    HoaHongBanHang = @HoaHongBanHang,
    DoanhThuKhuVuc = @DoanhThuKhuVuc,
    TyLeDoanhThu = @TyLeDoanhThu,
    LuongCoBan = @LuongCoBan,
    HeSoLuong = @HeSoLuong,
    PhuCap = @PhuCap,
    Thuong = @Thuong,
    KhauTru = @KhauTru,
    PhuCapCaDem = @PhuCapCaDem,
    PhuCapQuanLy = @PhuCapQuanLy,
    ThuongQTHT = @ThuongQTHT
WHERE MaNV = @MaNV AND Ky = @Ky";
                }
                else
                {
                    sql = @"INSERT INTO CongNV
    (MaNV, Ky, SoNgayLam, SoGioLam, LuongTheoGio, DoanhThuCa, HoaHongBanHang, DoanhThuKhuVuc, TyLeDoanhThu, LuongCoBan,
     HeSoLuong, PhuCap, Thuong, KhauTru, PhuCapCaDem, PhuCapQuanLy, ThuongQTHT)
VALUES
    (@MaNV, @Ky, @SoNgayLam, @SoGioLam, @LuongTheoGio, @DoanhThuCa, @HoaHongBanHang, @DoanhThuKhuVuc, @TyLeDoanhThu, @LuongCoBan,
     @HeSoLuong, @PhuCap, @Thuong, @KhauTru, @PhuCapCaDem, @PhuCapQuanLy, @ThuongQTHT)";
                }

                using (var cmd = new SqlCommand(sql, connection))
                {
                    cmd.Parameters.AddWithValue("@MaNV", maNv);
                    cmd.Parameters.AddWithValue("@Ky", ky);
                    cmd.Parameters.AddWithValue("@SoNgayLam", soNgayLam);
                    cmd.Parameters.AddWithValue("@SoGioLam", soGioLam);
                    cmd.Parameters.AddWithValue("@LuongTheoGio", luongTheoGio);
                    cmd.Parameters.AddWithValue("@DoanhThuCa", doanhThuCa);
                    cmd.Parameters.AddWithValue("@HoaHongBanHang", hoaHongBanHang);
                    cmd.Parameters.AddWithValue("@DoanhThuKhuVuc", doanhThuKhuVuc);
                    cmd.Parameters.AddWithValue("@TyLeDoanhThu", tyLeDoanhThu);
                    cmd.Parameters.AddWithValue("@LuongCoBan", luongCoBan);
                    cmd.Parameters.AddWithValue("@HeSoLuong", heSoLuong);
                    cmd.Parameters.AddWithValue("@PhuCap", phuCap);
                    cmd.Parameters.AddWithValue("@Thuong", thuong);
                    cmd.Parameters.AddWithValue("@KhauTru", khauTru);
                    cmd.Parameters.AddWithValue("@PhuCapCaDem", phuCapCaDem);
                    cmd.Parameters.AddWithValue("@PhuCapQuanLy", phuCapQuanLy);
                    cmd.Parameters.AddWithValue("@ThuongQTHT", thuongQTHT);

                    cmd.ExecuteNonQuery();
                }
            }
        }
        private int GetIntValue(TextBox textBox)
        {
            var text = textBox.Text.Trim();
            if (string.IsNullOrEmpty(text))
            {
                return 0;
            }

            if (int.TryParse(text, NumberStyles.Integer, CultureInfo.CurrentCulture, out int value))
            {
                return value;
            }

            if (int.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture, out value))
            {
                return value;
            }

            throw new FormatException($"Giá trị '{text}' không hợp lệ.");
        }
        private decimal GetDecimalValue(TextBox textBox)
        {
            var text = textBox.Text.Trim();
            if (string.IsNullOrEmpty(text))
            {
                return 0m;
            }

            if (decimal.TryParse(text, NumberStyles.Any, CultureInfo.CurrentCulture, out decimal value))
            {
                return value;
            }

            if (decimal.TryParse(text, NumberStyles.Any, CultureInfo.GetCultureInfo("vi-VN"), out value))
            {
                return value;
            }

            if (decimal.TryParse(text, NumberStyles.Any, CultureInfo.InvariantCulture, out value))
            {
                return value;
            }

            throw new FormatException($"Giá trị '{text}' không hợp lệ.");
        }
        private void SetResultTextBoxesReadOnly()
        {
            txtLuongGio.ReadOnly = true;
            txtLuongCoBanTinh.ReadOnly = true;
            txtTienHoaHong.ReadOnly = true;
            txtLuongKPI.ReadOnly = true;
            txtTongPhuCap.ReadOnly = true;
            txtTongThuNhap.ReadOnly = true;
            txtLuongThucNhan.ReadOnly = true;
        }
        private void DisableAllManagedTextBoxes()
        {
            foreach (var textBox in managedTextBoxes)
            {
                textBox.Enabled = false;
            }
        }

        private void EnableTextBoxesForRole(string role)
        {
            DisableAllManagedTextBoxes();

            if (role == null)
            {
                return;
            }

            txtKy.Enabled = true;

            var key = role.Trim();
            if (key.Length == 0)
            {
                return;
            }

            if (roleTextBoxes.TryGetValue(key, out var textBoxes))
            {
                foreach (var textBox in textBoxes)
                {
                    textBox.Enabled = true;
                }
            }
        }
        private decimal CalculateSalesRevenue(int maNhanVien)
        {
            using (var connection = new SqlConnection(connectionString))
            using (var command = new SqlCommand(
                       "SELECT SUM(ISNULL(TongTien, 0)) FROM HoaDon WHERE MaNhanVien = @MaNhanVien",
                       connection))
            {
                command.Parameters.Add("@MaNhanVien", SqlDbType.Int).Value = maNhanVien;
                connection.Open();

                var result = command.ExecuteScalar();
                if (result == null || result == DBNull.Value)
                {
                    return 0m;
                }

                return Convert.ToDecimal(result, CultureInfo.InvariantCulture);
            }
        }
        private string FormatCurrency(decimal value)
        {
            return value.ToString("N2", CultureInfo.CurrentCulture);
        }

        private void btnBangLuong_Click(object sender, EventArgs e)
        {
            using (var f = new formDanhSachTinhLuong())
            {
                f.StartPosition = FormStartPosition.CenterScreen;
                f.ShowDialog();
            }
        }
    }
}
