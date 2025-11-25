using System;

using System.Data;
using System.Collections.Generic;

using System.Windows.Forms;
using System.Configuration;
using System.Data.SqlClient;
using System.Globalization;
using System.ComponentModel;
namespace LeLeStore
{
    public partial class formEmployeeSalary : Form
    {
        private readonly string connectionString;
        private readonly TextBox[] managedTextBoxes;
        private bool updatingWorkdayControls;
        private decimal previousWorkdayValue;
        private decimal previousHourValue;
        private readonly Dictionary<string, TextBox[]> roleTextBoxes;
        private readonly Dictionary<string, (decimal BaseSalary, decimal SalaryCoefficient, decimal HourlyRate)> roleSalaryDefaults;
        public formEmployeeSalary()
        {
            InitializeComponent();
            connectionString = ConfigurationManager
                  .ConnectionStrings["LeLeStore.Properties.Settings.GStoreConnectionString"]?.ConnectionString
                  ?? throw new InvalidOperationException("Không tìm thấy chuỗi kết nối GStore.");
            managedTextBoxes = new[]
           {
                
                txtHeSoLuong,
                txtLuongCoBan,
                txtLuongTheoGio,
                
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
            roleSalaryDefaults = new Dictionary<string, (decimal BaseSalary, decimal SalaryCoefficient, decimal HourlyRate)>(StringComparer.CurrentCultureIgnoreCase)
            {
                { "Nhân viên bán hàng", (3_000_000m, 1.1m, 15_000m) },
                { "Nhân viên kho", (3_200_000m, 1.05m, 16_000m) },
                { "Quản lý cửa hàng", (5_000_000m, 1.3m, 0m) }
            };
            Load += formEmployeeSalary_Load;
            cboNhanVien.SelectedIndexChanged += cboNhanVien_SelectedIndexChanged;
            btnTinhLuong.Click += btnTinhLuong_Click;
            btnLuu.Click += btnLuu_Click;
            btnThoat.Click += (sender, e) => Close();
            dtpKiLuong.ValueChanged += dtpKiLuong_ValueChanged;
            nudSoNgay.ValueChanged += nudSoNgay_ValueChanged;

            SetResultTextBoxesReadOnly();
            DisableAllManagedTextBoxes();
            ConfigurePayPeriodPicker();
            ApplyMonthDaysToControls();
            UpdateHoursFromDays();
            RegisterNonNegativeValidation();
            RegisterWorkScheduleValidation();
            CacheNonNegativeWorkValues();
        }

        private void RegisterNonNegativeValidation()
        {
            var textBoxesToValidate = new[]
            {
                txtHeSoLuong,
                txtLuongCoBan,
                txtLuongTheoGio,
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

            foreach (var textBox in textBoxesToValidate)
            {
                textBox.Validating += EnsureNonNegativeOnValidation;
            }
        }

        private void RegisterWorkScheduleValidation()
        {
            nudSoNgay.Enter += (_, __) => previousWorkdayValue = nudSoNgay.Value;
            nudSoNgay.Validating += nudSoNgay_Validating;

            nudSoGio.Enter += (_, __) => previousHourValue = nudSoGio.Value;
            nudSoGio.Validating += nudSoGio_Validating;
        }


        private void EnsureNonNegativeOnValidation(object sender, CancelEventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox == null)
            {
                return;
            }

            if (!textBox.Enabled || textBox.ReadOnly)
            {
                return;
            }

            var text = textBox.Text.Trim();
            if (string.IsNullOrEmpty(text))
            {
                return;
            }

            if (!decimal.TryParse(text, NumberStyles.Any, CultureInfo.CurrentCulture, out decimal value)
                && !decimal.TryParse(text, NumberStyles.Any, CultureInfo.GetCultureInfo("vi-VN"), out value)
                && !decimal.TryParse(text, NumberStyles.Any, CultureInfo.InvariantCulture, out value))
            {
                return;
            }

            if (value < 0m)
            {
                MessageBox.Show("Giá trị không được âm.", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                e.Cancel = true;
                textBox.SelectAll();
            }
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
                ApplyRoleSalaryDefaults(chucVu);
                ApplyCompensationRules();
                if (string.Equals(chucVu, "Quản lý cửa hàng", StringComparison.CurrentCultureIgnoreCase))
                {
                    try
                    {
                        var doanhThuKhuVuc = CalculateAreaRevenue(dtpKiLuong.Value.Year, dtpKiLuong.Value.Month);
                        txtDoanhThuKhuVuc.Text = FormatCurrency(doanhThuKhuVuc);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Không thể tính doanh thu khu vực: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        txtDoanhThuKhuVuc.Text = string.Empty;
                    }
                }
                else
                {
                    txtDoanhThuKhuVuc.Text = string.Empty;
                }
                if (string.Equals(chucVu, "Nhân viên bán hàng", StringComparison.CurrentCultureIgnoreCase)
                    && int.TryParse(rowView["MaNhanVien"].ToString(), out int maNhanVien))
                {
                    try
                    {
                        var doanhThuCa = CalculateSalesRevenue(maNhanVien, dtpKiLuong.Value.Year, dtpKiLuong.Value.Month);
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
                ApplyCompensationRules();
            }
            else
            {
                txtHoTen.Text = string.Empty;
                txtChucVu.Text = string.Empty;
                ClearManagedTextBoxes();
                EnableTextBoxesForRole(null);
                ApplyRoleSalaryDefaults(null);
                ApplyCompensationRules();
            }
        }

        private void dtpKiLuong_ValueChanged(object sender, EventArgs e)
        {
            ApplyMonthDaysToControls();
            UpdateHoursFromDays();
            ApplyCompensationRules();

            if (string.Equals(txtChucVu.Text, "Quản lý cửa hàng", StringComparison.CurrentCultureIgnoreCase))
            {
                try
                {
                    var doanhThuKhuVuc = CalculateAreaRevenue(dtpKiLuong.Value.Year, dtpKiLuong.Value.Month);
                    txtDoanhThuKhuVuc.Text = FormatCurrency(doanhThuKhuVuc);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Không thể tính doanh thu khu vực: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    txtDoanhThuKhuVuc.Text = string.Empty;
                }
            }
            if (string.Equals(txtChucVu.Text, "Nhân viên bán hàng", StringComparison.CurrentCultureIgnoreCase)
              && int.TryParse(cboNhanVien.SelectedValue?.ToString(), out int maNhanVien))
            {
                try
                {
                    var doanhThuCa = CalculateSalesRevenue(maNhanVien, dtpKiLuong.Value.Year, dtpKiLuong.Value.Month);
                    txtDoanhThuCa.Text = FormatCurrency(doanhThuCa);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Không thể tính doanh thu ca: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    txtDoanhThuCa.Text = string.Empty;
                }
            }
        }

        private void ApplyMonthDaysToControls()
        {
            updatingWorkdayControls = true;
            int daysInMonth = GetDaysInSelectedMonth();
            nudSoNgay.Value = Math.Min(nudSoNgay.Maximum, daysInMonth);
            updatingWorkdayControls = false;
            CacheNonNegativeWorkValues();
        }

        private int GetDaysInSelectedMonth()
        {
            return DateTime.DaysInMonth(dtpKiLuong.Value.Year, dtpKiLuong.Value.Month);
        }

        private void UpdateHoursFromDays()
        {
            if (updatingWorkdayControls)
            {
                return;
            }

            updatingWorkdayControls = true;
            decimal suggestedHours = nudSoNgay.Value * 8;
            if (suggestedHours > nudSoGio.Maximum)
            {
                suggestedHours = nudSoGio.Maximum;
            }

            nudSoGio.Value = suggestedHours;
            updatingWorkdayControls = false;
            CacheNonNegativeWorkValues();
        }

        private void ApplyCompensationRules()
        {
            int soNgayLam = (int)nudSoNgay.Value;
            int daysInMonth = GetDaysInSelectedMonth();
            var role = txtChucVu.Text?.Trim();

            decimal thuong = (soNgayLam >= daysInMonth && daysInMonth >= 29) ? 200_000m : 100_000m;
            txtThuong.Text = FormatCurrency(thuong);

            decimal khauTru = soNgayLam < 25 ? 50_000m : 0m;
            txtKhauTru.Text = FormatCurrency(khauTru);

            decimal phuCap = 0m;
            decimal phuCapCaDem = 0m;
            decimal phuCapQuanLy = 0m;
            decimal thuongQTHT = 0m;
            decimal tyLeDoanhThu = 0m;

            if (string.Equals(role, "Nhân viên bán hàng", StringComparison.CurrentCultureIgnoreCase))
            {
                phuCap = 200_000m;
                var doanhThuCa = GetDecimalValue(txtDoanhThuCa);
                var hoaHongBanHang = doanhThuCa < 100_000m
                    ? 0.01m
                    : doanhThuCa <= 500_000m
                        ? 0.02m
                        : 0.03m;
                txtHoaHongBanHang.Text = hoaHongBanHang.ToString("N2", CultureInfo.CurrentCulture);
            }
            else if (string.Equals(role, "Nhân viên kho", StringComparison.CurrentCultureIgnoreCase))
            {
                phuCap = 150_000m;
                phuCapCaDem = 200_000m;
                var doanhThuKhuVuc = GetDecimalValue(txtDoanhThuKhuVuc);
                tyLeDoanhThu = doanhThuKhuVuc < 500_000m
                    ? 0.01m
                    : doanhThuKhuVuc <= 900_000m
                        ? 0.02m
                        : 0.03m;
                
            }
            else if (string.Equals(role, "Quản lý cửa hàng", StringComparison.CurrentCultureIgnoreCase))
            {
                phuCapQuanLy = 500_000m;
                phuCap = 250_000m;
                thuongQTHT = 150_000m;
                var doanhThuKhuVuc = GetDecimalValue(txtDoanhThuKhuVuc);
                tyLeDoanhThu = doanhThuKhuVuc < 500_000m
                    ? 0.01m
                    : doanhThuKhuVuc <= 900_000m
                        ? 0.02m
                        : 0.03m;
            }

            txtPhuCap.Text = FormatCurrency(phuCap);
            txtPhuCapCaDem.Text = FormatCurrency(phuCapCaDem);
            txtPhuCapQuanLy.Text = FormatCurrency(phuCapQuanLy);
            txtThuongQTHT.Text = FormatCurrency(thuongQTHT);
            txtTyLeDoanhThu.Text = tyLeDoanhThu > 0m
             ? tyLeDoanhThu.ToString("N2", CultureInfo.CurrentCulture)
             : string.Empty;
        }

        private void ConfigurePayPeriodPicker()
        {
            dtpKiLuong.Format = DateTimePickerFormat.Custom;
            dtpKiLuong.CustomFormat = "yyyy-MM";
            dtpKiLuong.ShowUpDown = true;
        }
        private void CacheNonNegativeWorkValues()
        {
            if (nudSoNgay.Value >= 0)
            {
                previousWorkdayValue = nudSoNgay.Value;
            }

            if (nudSoGio.Value >= 0)
            {
                previousHourValue = nudSoGio.Value;
            }
        }

        private bool TryParseDecimalInput(string text, out decimal value)
        {
            if (decimal.TryParse(text, NumberStyles.Any, CultureInfo.CurrentCulture, out value))
            {
                return true;
            }

            if (decimal.TryParse(text, NumberStyles.Any, CultureInfo.GetCultureInfo("vi-VN"), out value))
            {
                return true;
            }

            if (decimal.TryParse(text, NumberStyles.Any, CultureInfo.InvariantCulture, out value))
            {
                return true;
            }

            value = 0m;
            return false;
        }

        private bool ValidateNonNegativeNumericUpDown(NumericUpDown control, ref decimal previousValue, CancelEventArgs e)
        {
            var text = control.Text?.Trim() ?? string.Empty;
            if (TryParseDecimalInput(text, out decimal typedValue) && typedValue < 0)
            {
                MessageBox.Show("Giá trị không được âm.", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                control.Value = previousValue;
                e.Cancel = true;
                control.Select(0, control.Text.Length);
                return false;
            }

            previousValue = control.Value;
            return true;
        }


        private void btnTinhLuong_Click(object sender, EventArgs e)
        {
            try
            {
                int soNgayLam = GetValidatedWorkDays();
                decimal soGioLam = nudSoGio.Value;
                decimal luongTheoGio = GetDecimalValue(txtLuongTheoGio);
                decimal luongCoBan = GetDecimalValue(txtLuongCoBan);
                decimal heSoLuong = GetDecimalValue(txtHeSoLuong);
                decimal doanhThuCa = GetNonNegativeDecimal(txtDoanhThuCa, "Doanh thu ca");
                decimal hoaHongBanHang = GetNonNegativeDecimal(txtHoaHongBanHang, "Hoa hồng bán hàng");
                decimal doanhThuKhuVuc = GetNonNegativeDecimal(txtDoanhThuKhuVuc, "Doanh thu khu vực");
                decimal tyLeDoanhThu = GetDecimalValue(txtTyLeDoanhThu);
                ApplyCompensationRules();
                decimal phuCap = GetNonNegativeDecimal(txtPhuCap, "Phụ cấp");
                decimal phuCapCaDem = GetNonNegativeDecimal(txtPhuCapCaDem, "Phụ cấp ca đêm");
                decimal phuCapQuanLy = GetDecimalValue(txtPhuCapQuanLy);
                decimal thuong = GetNonNegativeDecimal(txtThuong, "Thưởng");
                decimal thuongQTHT = GetNonNegativeDecimal(txtThuongQTHT, "Thưởng QTHT");
                decimal khauTru = GetNonNegativeDecimal(txtKhauTru, "Khấu trừ");

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
        private void ApplyRoleSalaryDefaults(string role)
        {
            if (string.IsNullOrWhiteSpace(role))
            {
                txtLuongCoBan.Text = string.Empty;
                txtHeSoLuong.Text = string.Empty;
                txtLuongTheoGio.Text = string.Empty;
                return;
            }

            var key = role.Trim();
            if (roleSalaryDefaults.TryGetValue(key, out var defaults))
            {
                txtLuongCoBan.Text = FormatCurrency(defaults.BaseSalary);
                txtHeSoLuong.Text = defaults.SalaryCoefficient.ToString("N2", CultureInfo.CurrentCulture);
                txtLuongTheoGio.Text = defaults.HourlyRate > 0m
                 ? FormatCurrency(defaults.HourlyRate)
                 : string.Empty;
            }
            else
            {
                txtLuongCoBan.Text = string.Empty;
                txtHeSoLuong.Text = string.Empty;
                txtLuongTheoGio.Text = string.Empty;
            }
        }

        private void btnLuu_Click(object sender, EventArgs e)
        {
            if (cboNhanVien.SelectedValue == null)
            {
                MessageBox.Show("Vui lòng chọn nhân viên.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            string ky = dtpKiLuong.Value.ToString("yyyy-MM");

            try
            {
                int maNv = Convert.ToInt32(cboNhanVien.SelectedValue);
                int soNgayLam = GetValidatedWorkDays();
                decimal soGioLam = nudSoGio.Value;
                decimal luongTheoGio = GetDecimalValue(txtLuongTheoGio);
                decimal doanhThuCa = GetNonNegativeDecimal(txtDoanhThuCa, "Doanh thu ca");
                decimal hoaHongBanHang = GetNonNegativeDecimal(txtHoaHongBanHang, "Hoa hồng bán hàng");
                decimal doanhThuKhuVuc = GetNonNegativeDecimal(txtDoanhThuKhuVuc, "Doanh thu khu vực");
                decimal tyLeDoanhThu = GetDecimalValue(txtTyLeDoanhThu);
                decimal luongCoBan = GetDecimalValue(txtLuongCoBan);
                decimal heSoLuong = GetDecimalValue(txtHeSoLuong);
                ApplyCompensationRules();
                decimal phuCap = GetNonNegativeDecimal(txtPhuCap, "Phụ cấp");
                decimal thuong = GetNonNegativeDecimal(txtThuong, "Thưởng");
                decimal khauTru = GetNonNegativeDecimal(txtKhauTru, "Khấu trừ");
                decimal phuCapCaDem = GetNonNegativeDecimal(txtPhuCapCaDem, "Phụ cấp ca đêm");
                decimal phuCapQuanLy = GetDecimalValue(txtPhuCapQuanLy);
                decimal thuongQTHT = GetNonNegativeDecimal(txtThuongQTHT, "Thưởng QTHT");

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

        private decimal GetNonNegativeDecimal(TextBox textBox, string fieldName)
        {
            var text = textBox.Text.Trim();
            if (text == "-1")
            {
                MessageBox.Show($"{fieldName} không được là -1. Vui lòng nhập giá trị không âm.", "Giá trị không hợp lệ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                throw new FormatException($"{fieldName} không được là -1.");
            }

            var value = GetDecimalValue(textBox);
            if (value < 0m)
            {
                throw new FormatException($"{fieldName} không được âm.");
            }

            return value;
        }

        private int GetValidatedWorkDays()
        {
            var text = nudSoNgay.Text.Trim();
            if (text == "-1")
            {
                MessageBox.Show("Số ngày làm không được là -1. Vui lòng nhập giá trị không âm.", "Giá trị không hợp lệ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                throw new FormatException("Số ngày làm không được là -1.");
            }

            int value = (int)nudSoNgay.Value;
            if (value < 0)
            {
                throw new FormatException("Số ngày làm không được âm.");
            }

            return value;
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
        private void ClearManagedTextBoxes()
        {
            foreach (var textBox in managedTextBoxes)
            {
                textBox.Text = string.Empty;
            }
        }
        private void EnableTextBoxesForRole(string role)
        {
            DisableAllManagedTextBoxes();
            nudSoNgay.Enabled = role != null;
            nudSoGio.Enabled = role != null;

            if (role == null)
            {
                return;
            }

           

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
        private decimal CalculateAreaRevenue(int year, int month)
        {
            using (var connection = new SqlConnection(connectionString))
            using (var command = new SqlCommand(
                       "SELECT SUM(ISNULL(TongTien, 0)) FROM HoaDon WHERE MONTH(NgayLap) = @Month AND YEAR(NgayLap) = @Year",
                       connection))
            {
                command.Parameters.Add("@Month", SqlDbType.Int).Value = month;
                command.Parameters.Add("@Year", SqlDbType.Int).Value = year;
                connection.Open();

                var result = command.ExecuteScalar();
                if (result == null || result == DBNull.Value)
                {
                    return 0m;
                }

                return Convert.ToDecimal(result, CultureInfo.InvariantCulture);
            }
        }

        private decimal CalculateSalesRevenue(int maNhanVien, int year, int month)
        {
            using (var connection = new SqlConnection(connectionString))
            using (var command = new SqlCommand(
                       "SELECT SUM(ISNULL(TongTien, 0)) FROM HoaDon WHERE MaNhanVien = @MaNhanVien AND MONTH(NgayLap) = @Month AND YEAR(NgayLap) = @Year",
                       connection))
            {
                command.Parameters.Add("@MaNhanVien", SqlDbType.Int).Value = maNhanVien;
                command.Parameters.Add("@Month", SqlDbType.Int).Value = month;
                command.Parameters.Add("@Year", SqlDbType.Int).Value = year;
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

        private void nudSoNgay_ValueChanged(object sender, EventArgs e)
        {
            UpdateHoursFromDays();
            ApplyCompensationRules();
        }

        private void nudSoNgay_Validating(object sender, CancelEventArgs e)
        {
            ValidateNonNegativeNumericUpDown(nudSoNgay, ref previousWorkdayValue, e);
        }

        private void nudSoGio_Validating(object sender, CancelEventArgs e)
        {
            ValidateNonNegativeNumericUpDown(nudSoGio, ref previousHourValue, e);
        }
    }
}
