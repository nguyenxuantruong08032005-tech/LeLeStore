using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using LeLeStore.GStoreDataSetTableAdapters;
namespace LeLeStore
{
    public partial class formPhieuNhap : Form
    {
        private readonly string _username;
        private readonly GiaoDichKhoTableAdapter giaoDichKhoTableAdapter = new GiaoDichKhoTableAdapter();
        private readonly NhaCungCapTableAdapter nhaCungCapTableAdapter = new NhaCungCapTableAdapter();
        private readonly SanPhamTableAdapter sanPhamTableAdapter = new SanPhamTableAdapter();
        private readonly NhanVienTableAdapter nhanVienTableAdapter = new NhanVienTableAdapter();
        private readonly NguoiDungTableAdapter nguoiDungTableAdapter = new NguoiDungTableAdapter();
        private readonly BindingSource giaoDichBindingSource = new BindingSource();

        private class SupplierComboItem
        {
            public int MaNCC { get; set; }
            public string Display { get; set; }
            public override string ToString()
            {
                return string.IsNullOrWhiteSpace(Display) ? base.ToString() : Display;
            }
        }
        private class ProductComboItem
        {
            public int MaSP { get; set; }
            public string Display { get; set; }
            public override string ToString()
            {
                return string.IsNullOrWhiteSpace(Display) ? base.ToString() : Display;
            }
        }
        private bool AskConfirm(string msg, MessageBoxIcon icon = MessageBoxIcon.Question)
    => MessageBox.Show(msg, "Xác nhận", MessageBoxButtons.YesNo, icon) == DialogResult.Yes;

        private void CommitUI()
        {
            Validate();
            try { dataGridView1.EndEdit(); } catch { }
            try { dataGridView2.EndEdit(); } catch { }
            giaoDichBindingSource.EndEdit();
            chiTietGiaoDichKhoBindingSource.EndEdit();
        }

        // Điều khiển 2-pha cho header
        private void SetTransactionMode(EditMode m)
        {
            transactionMode = m;

            bool canEdit = (m == EditMode.Add || m == EditMode.Edit);
            txtMaGD.ReadOnly = true;
            cbLoaiGD.Enabled = canEdit;
            dateTimePicker1.Enabled = canEdit;
            cboMaNCC.Enabled = canEdit;
            cboMaNV.Enabled = canEdit;

            btnThem.Enabled = (m != EditMode.Edit);
            btnSua.Enabled = (m != EditMode.Add);
            btnXoa.Enabled = (m == EditMode.None);
            btnThem.Text = (m == EditMode.Add) ? "Lưu" : "Thêm";
            btnSua.Text = (m == EditMode.Edit) ? "Lưu" : "Sửa";

            // Khi đang Add/Edit header, chặn sửa/xóa chi tiết
            btnThemCT.Enabled = btnSuaCT.Enabled = btnXoaCT.Enabled = (m == EditMode.None);
        }

        // Điều khiển 2-pha cho detail
        private void SetDetailMode(EditMode m)
        {
            detailMode = m;

            bool canEdit = (m == EditMode.Add || m == EditMode.Edit);
            cboMaGD.Enabled = canEdit;
            cbMaSP.Enabled = canEdit;
            numericUpDown1.Enabled = canEdit;

            btnThemCT.Text = (m == EditMode.Add) ? "Lưu CT" : "Thêm";
            btnSuaCT.Text = (m == EditMode.Edit) ? "Lưu CT" : "Sửa";

            // Khi đang Add/Edit detail, khóa header
            bool lockHeader = (m != EditMode.None);
            btnThem.Enabled = btnSua.Enabled = btnXoa.Enabled = !lockHeader;
        }
        private enum EditMode
        {
            None,
            Add,
            Edit
        }

        private EditMode transactionMode = EditMode.None;
        private EditMode detailMode = EditMode.None;
        private bool transactionPendingChanges;
        private bool detailPendingChanges;
        private int? transactionEditingId;
        private (int MaGD, int MaSP)? detailEditingKey;
        private bool suppressProductComboUpdate;
        private bool suppressTransactionComboUpdate;
        public formPhieuNhap(string username = "")
        {
            _username = username ?? string.Empty;
            InitializeComponent();

            cbLoaiGD.SelectedIndexChanged += cboMaNCC_SelectedIndexChanged_1;

            giaoDichBindingSource.DataSource = gStoreDataSet;
            giaoDichBindingSource.DataMember = gStoreDataSet.GiaoDichKho.TableName;
            dataGridView1.AutoGenerateColumns = true;
            dataGridView1.DataSource = giaoDichBindingSource;

            if (dataGridView2.DataSource == null)
            {
                dataGridView2.AutoGenerateColumns = true;
                dataGridView2.DataSource = chiTietGiaoDichKhoBindingSource;
            }

            cbLoaiGD.DropDownStyle = ComboBoxStyle.DropDownList;
            if (cbLoaiGD.Items.Count == 0)
            {
                cbLoaiGD.Items.AddRange(new object[] { "Nhap", "Xuat" });
            }

            numericUpDown1.Minimum = 1;
            numericUpDown1.Maximum = 1000000;

            cbMaSP.DropDownStyle = ComboBoxStyle.DropDownList;
            cbMaSP.DisplayMember = nameof(ProductComboItem.Display);
            cbMaSP.ValueMember = nameof(ProductComboItem.MaSP);
            cboMaNCC.DropDownStyle = ComboBoxStyle.DropDownList;
            cboMaNCC.DisplayMember = nameof(SupplierComboItem.Display);
            cboMaNCC.ValueMember = nameof(SupplierComboItem.MaNCC);
            cboMaNV.DropDownStyle = ComboBoxStyle.DropDownList;


            cboMaGD.DropDownStyle = ComboBoxStyle.DropDownList;
            cboMaGD.SelectedIndexChanged += cboMaGD_SelectedIndexChanged;
            cboMaNCC.SelectedIndexChanged += cboMaNCC_SelectedIndexChanged;


        }

        private void EnsureEmployeeDataLoaded()
        {
            if (gStoreDataSet.NguoiDung.Count == 0)
            {
                ExecuteSafely(() => nguoiDungTableAdapter.Fill(gStoreDataSet.NguoiDung), "Không thể tải dữ liệu người dùng");
            }

            if (gStoreDataSet.NhanVien.Count == 0)
            {
                ExecuteSafely(() => nhanVienTableAdapter.Fill(gStoreDataSet.NhanVien), "Không thể tải dữ liệu nhân viên");
            }
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void label8_Click(object sender, EventArgs e)
        {

        }

        private void formPhieuNhap_Load(object sender, EventArgs e)
        {
            LoadInitialData();
            dataGridView1_SelectionChanged(null, EventArgs.Empty);
            dataGridView2_SelectionChanged(null, EventArgs.Empty);

        }
        private void LoadInitialData()
        {
            ExecuteSafely(() => giaoDichKhoTableAdapter.Fill(gStoreDataSet.GiaoDichKho), "Không thể tải dữ liệu giao dịch kho");
            ExecuteSafely(() => chiTietGiaoDichKhoTableAdapter.Fill(gStoreDataSet.ChiTietGiaoDichKho), "Không thể tải dữ liệu chi tiết giao dịch");
            EnsureLookupDataLoaded();
            EnsureEmployeeDataLoaded();
            PopulateTransactionComboBox();
            PopulateSupplierComboBox();
            PopulateEmployeeComboBox();
        }
        private void PopulateEmployeeComboBox(int? selectedEmployeeId = null)
        {
            EnsureEmployeeDataLoaded();

            var employees = new List<KeyValuePair<int, string>>();

            if (!string.IsNullOrWhiteSpace(_username))
            {
                var userRow = gStoreDataSet.NguoiDung
                    .FirstOrDefault(row => !row.IsNull(gStoreDataSet.NguoiDung.TenDangNhapColumn)
                                          && string.Equals(row.TenDangNhap, _username, StringComparison.OrdinalIgnoreCase));

                if (userRow != null)
                {
                    employees = gStoreDataSet.NhanVien
                        .Where(row => row.RowState != DataRowState.Deleted && !row.IsMaNguoiDungNull() && row.MaNguoiDung == userRow.MaNguoiDung)
                        .Select(row => new KeyValuePair<int, string>(row.MaNhanVien, $"{row.MaNhanVien} - {row.HoTen}"))
                        .OrderBy(item => item.Key)
                        .ToList();
                }
            }

            if (employees.Count == 0)
            {
                employees = gStoreDataSet.NhanVien
                    .Where(row => row.RowState != DataRowState.Deleted)
                    .Select(row => new KeyValuePair<int, string>(row.MaNhanVien, $"{row.MaNhanVien} - {row.HoTen}"))
                    .OrderBy(item => item.Key)
                    .ToList();
            }

            cboMaNV.DisplayMember = "Value";
            cboMaNV.ValueMember = "Key";
            cboMaNV.DropDownStyle = ComboBoxStyle.DropDownList;
            cboMaNV.DataSource = employees;

            if (selectedEmployeeId.HasValue && employees.Any(emp => emp.Key == selectedEmployeeId.Value))
            {
                cboMaNV.SelectedValue = selectedEmployeeId.Value;
            }
            else
            {
                cboMaNV.SelectedIndex = employees.Count > 0 ? 0 : -1;
            }
        }

        private void PopulateTransactionComboBox(int? selectedTransactionId = null)
        {
            var transactionIds = gStoreDataSet.GiaoDichKho
                .Where(row => row.RowState != DataRowState.Deleted)
                .Select(row => row.MaGD)
                .OrderBy(id => id)
                .ToList();

            suppressTransactionComboUpdate = true;
            cboMaGD.DataSource = null;
            cboMaGD.DataSource = transactionIds;

            if (selectedTransactionId.HasValue && transactionIds.Contains(selectedTransactionId.Value))
            {
                cboMaGD.SelectedItem = selectedTransactionId.Value;
            }
            else
            {
                cboMaGD.SelectedIndex = transactionIds.Count > 0 ? 0 : -1;
            }

            suppressTransactionComboUpdate = false;
        }

        private void EnsureLookupDataLoaded()
        {
            if (gStoreDataSet.NhaCungCap.Count == 0)
            {
                ExecuteSafely(() => nhaCungCapTableAdapter.Fill(gStoreDataSet.NhaCungCap), "Không thể tải dữ liệu nhà cung cấp");
            }

            if (gStoreDataSet.SanPham.Count == 0)
            {
                ExecuteSafely(() => sanPhamTableAdapter.Fill(gStoreDataSet.SanPham), "Không thể tải dữ liệu sản phẩm");
            }
        }
        private void UpdateProductComboBoxForTransaction(int? maGd, int? selectedProductId = null)
        {
            EnsureLookupDataLoaded();

            int? supplierId = null;
            if (maGd.HasValue)
            {
                var transactionRow = gStoreDataSet.GiaoDichKho.FindByMaGD(maGd.Value);
                if (transactionRow != null && transactionRow.RowState != DataRowState.Deleted && !transactionRow.IsMaNCCNull())
                {
                    supplierId = transactionRow.MaNCC;
                }
            }
            if (!supplierId.HasValue)
            {
                supplierId = GetSelectedSupplierId();
            }
            var items = gStoreDataSet.SanPham
                .Where(row => row.RowState != DataRowState.Deleted)
                .Where(row => !supplierId.HasValue || (!row.IsMaNCCNull() && row.MaNCC == supplierId.Value))
                .Select(row => new ProductComboItem
                {
                    MaSP = row.MaSP,
                    Display = $"{row.MaSP} - {row.TenSP}"
                })
                .OrderBy(item => item.MaSP)
                .ToList();

            cbMaSP.DataSource = null;
            cbMaSP.DataSource = items;
            cbMaSP.SelectedIndex = -1;

            if (selectedProductId.HasValue && items.Any(item => item.MaSP == selectedProductId.Value))
            {
                cbMaSP.SelectedValue = selectedProductId.Value;
            }
            else if (items.Count == 1)
            {
                cbMaSP.SelectedIndex = 0;
            }
        }
        private void PopulateSupplierComboBox(int? selectedSupplierId = null)
        {
            EnsureLookupDataLoaded();

            var items = gStoreDataSet.NhaCungCap
                .Where(row => row.RowState != DataRowState.Deleted)
                .Select(row => new SupplierComboItem
                {
                    MaNCC = row.MaNCC,
                    Display = $"{row.MaNCC} - {row.TenNCC}"
                })
                .OrderBy(item => item.MaNCC)
                .ToList();

            cboMaNCC.DataSource = null;
            cboMaNCC.DataSource = items;
            cboMaNCC.SelectedIndex = -1;

            if (selectedSupplierId.HasValue && items.Any(item => item.MaNCC == selectedSupplierId.Value))
            {
                cboMaNCC.SelectedValue = selectedSupplierId.Value;
            }
            else if (items.Count == 1)
            {
                cboMaNCC.SelectedIndex = 0;
            }
        }

        private int? GetSelectedSupplierId()
        {
            if (cboMaNCC.SelectedValue is int selectedValue)
            {
                return selectedValue;
            }

            if (cboMaNCC.SelectedItem is SupplierComboItem item)
            {
                return item.MaNCC;
            }

            if (!string.IsNullOrWhiteSpace(cboMaNCC.Text) && int.TryParse(cboMaNCC.Text.Trim(), out int parsedValue))
            {
                return parsedValue;
            }

            return null;
        }
        private int? GetSelectedTransactionId()
        {
            if (cboMaGD.SelectedValue is int selectedValue)
            {
                return selectedValue;
            }

            if (int.TryParse(cboMaGD.Text.Trim(), out int parsedValue))
            {
                return parsedValue;
            }

            return null;
        }
        private int? GetCurrentDetailTransactionId()
        {
            return GetSelectedTransactionId();
        }
        private int? GetSelectedEmployeeId()
        {
            if (cboMaNV.SelectedValue is int selectedValue)
            {
                return selectedValue;
            }

            if (cboMaNV.SelectedItem is KeyValuePair<int, string> item)
            {
                return item.Key;
            }

            if (!string.IsNullOrWhiteSpace(cboMaNV.Text) && int.TryParse(cboMaNV.Text.Trim(), out int parsedValue))
            {
                return parsedValue;
            }

            return null;
        }


        private int? GetSelectedProductId()
        {
            if (cbMaSP.SelectedValue is int selectedValue)
            {
                return selectedValue;
            }

            if (cbMaSP.SelectedItem is ProductComboItem item)
            {
                return item.MaSP;
            }

            if (!string.IsNullOrWhiteSpace(cbMaSP.Text) && int.TryParse(cbMaSP.Text.Trim(), out int parsedValue))
            {
                return parsedValue;
            }

            return null;
        }

        private bool TrySelectProductForCurrentTransaction(int maSp)
        {
            int? currentTransactionId = GetCurrentDetailTransactionId();
            UpdateProductComboBoxForTransaction(currentTransactionId, maSp);

            return cbMaSP.SelectedValue is int selected && selected == maSp;
        }

        private void cboMaGD_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (suppressTransactionComboUpdate)
            {
                return;
            }

            UpdateProductComboBoxForTransaction(GetCurrentDetailTransactionId(), GetSelectedProductId());
        }
        private void ExecuteSafely(Action action, string errorMessage)
        {
            try
            {
                action();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{errorMessage}.\nChi tiết: {ex.Message}", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            string keyword = txtSearch.Text.Trim();
            if (string.IsNullOrEmpty(keyword))
            {
                MessageBox.Show("Vui lòng nhập tên cần tìm kiếm.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            EnsureLookupDataLoaded();

            bool hasResult = false;

            var supplier = gStoreDataSet.NhaCungCap.FirstOrDefault(row => row.TenNCC.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0);
            if (supplier != null)
            {
                PopulateSupplierComboBox(supplier.MaNCC);
                hasResult = true;
            }

            var product = gStoreDataSet.SanPham.FirstOrDefault(row => row.TenSP.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0);
            if (product != null)
            {
               
                hasResult = true;
                if (!TrySelectProductForCurrentTransaction(product.MaSP))
                {
                    MessageBox.Show("Sản phẩm không thuộc nhà cung cấp của giao dịch hiện tại.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }

            if (!hasResult)
            {
                MessageBox.Show("Không tìm thấy nhà cung cấp hoặc sản phẩm phù hợp.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btnThem_Click(object sender, EventArgs e)
        {
            CommitUI();

            if (transactionMode != EditMode.Add)
            {
                if (!AskConfirm("Bạn muốn tạo giao dịch kho mới?")) return;

                SetTransactionMode(EditMode.Add);
                ClearTransactionInputs();    // đã auto sinh MaGD mới
                cbLoaiGD.Focus();
                return;
            }

            // Pha LƯU
            if (!TryGetTransactionValues(out string loaiGD, out DateTime ngayGD, out int? maNcc, out int maNV))
                return;

            if (!AskConfirm("Xác nhận lưu giao dịch mới?")) return;

            try
            {
                // Tạo row mới
                var newRow = gStoreDataSet.GiaoDichKho.NewGiaoDichKhoRow();
                newRow.MaGD = int.Parse(txtMaGD.Text);
                newRow.LoaiGD = loaiGD;
                newRow.NgayGD = ngayGD;
                if (maNcc.HasValue) newRow.MaNCC = maNcc.Value; else newRow.SetMaNCCNull();
                newRow.MaNhanVien = maNV;

                gStoreDataSet.GiaoDichKho.AddGiaoDichKhoRow(newRow);
                giaoDichBindingSource.EndEdit();
                giaoDichKhoTableAdapter.Update(gStoreDataSet.GiaoDichKho);

                // Đồng bộ & focus dòng vừa thêm (theo MaGD)
                int id = newRow.MaGD;
                RefreshTransactions(false);
                var rows = gStoreDataSet.GiaoDichKho.Select($"MaGD = {id}");
                if (rows.Length > 0)
                {
                    int pos = gStoreDataSet.GiaoDichKho.Rows.IndexOf(rows[0]);
                    if (pos >= 0) giaoDichBindingSource.Position = pos;
                }

                MessageBox.Show("Đã thêm giao dịch kho.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không thể thêm giao dịch.\n" + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                SetTransactionMode(EditMode.None);
            }
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            CommitUI();

            var view = giaoDichBindingSource.Current as DataRowView;
            if (view == null)
            {
                MessageBox.Show("Vui lòng chọn giao dịch cần xóa.", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var row = view.Row as GStoreDataSet.GiaoDichKhoRow;
            if (row == null || row.RowState == DataRowState.Deleted)
            {
                MessageBox.Show("Vui lòng chọn giao dịch cần xóa.", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (!AskConfirm("Bạn có chắc chắn muốn xóa giao dịch này?", MessageBoxIcon.Warning)) return;

            int desiredPos = Math.Max(0, giaoDichBindingSource.Position - 1);

            try
            {
                view.Delete();
                giaoDichBindingSource.EndEdit();
                giaoDichKhoTableAdapter.Update(gStoreDataSet.GiaoDichKho);

                RefreshTransactions(false);
                if (giaoDichBindingSource.Count > 0)
                    giaoDichBindingSource.Position = Math.Min(desiredPos, giaoDichBindingSource.Count - 1);

                MessageBox.Show("Đã xóa giao dịch.", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không thể xóa giao dịch.\n" + ex.Message, "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            CommitUI();

            var view = giaoDichBindingSource.Current as DataRowView;
            if (view == null)
            {
                MessageBox.Show("Vui lòng chọn giao dịch cần sửa.", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var row = view.Row as GStoreDataSet.GiaoDichKhoRow;
            if (row == null || row.RowState == DataRowState.Deleted)
            {
                MessageBox.Show("Vui lòng chọn giao dịch cần sửa.", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (transactionMode != EditMode.Edit)
            {
                if (!AskConfirm("Bạn muốn sửa giao dịch này?")) return;

                transactionEditingId = row.MaGD;
                SetTransactionMode(EditMode.Edit);
                PopulateTransactionInputs(row);
                return;
            }

            if (!TryGetTransactionValues(out string loaiGD, out DateTime ngayGD, out int? maNcc, out int maNV))
                return;

            if (!AskConfirm("Xác nhận lưu thay đổi giao dịch?")) return;

            try
            {
                row.LoaiGD = loaiGD;
                row.NgayGD = ngayGD;
                if (maNcc.HasValue) row.MaNCC = maNcc.Value; else row.SetMaNCCNull();
                row.MaNhanVien = maNV;

                giaoDichBindingSource.EndEdit();
                giaoDichKhoTableAdapter.Update(row); // không Fill
                giaoDichBindingSource.ResetCurrentItem();

                MessageBox.Show("Đã cập nhật giao dịch.", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                UpdateProductComboBoxForTransaction(row.MaGD, GetSelectedProductId());
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không thể cập nhật giao dịch.\n" + ex.Message, "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                transactionEditingId = null;
                SetTransactionMode(EditMode.None);
            }
        }

     

        private void btnThemCT_Click(object sender, EventArgs e)
        {
            CommitUI();

            if (detailMode != EditMode.Add)
            {
                if (!AskConfirm("Bạn muốn thêm chi tiết cho giao dịch?")) return;

                SetDetailMode(EditMode.Add);
                ClearDetailInputs();

                // mặc định gán MaGD chi tiết theo MaGD header
                if (int.TryParse(txtMaGD.Text, out int maGd))
                {
                    suppressTransactionComboUpdate = true;
                    cboMaGD.SelectedItem = maGd;
                    suppressTransactionComboUpdate = false;
                    UpdateProductComboBoxForTransaction(maGd);
                }
                else
                {
                    UpdateProductComboBoxForTransaction(null);
                }
                cboMaGD.Focus();
                return;
            }

            // Pha LƯU
            if (!TryGetDetailValues(out int maGD, out int maSP, out int soLuong)) return;

            if (DetailExists(maGD, maSP))  // đã có khóa (MaGD, MaSP)
            {
                // Hỏi cộng dồn
                if (AskConfirm("Chi tiết (MaGD, MaSP) đã tồn tại. Bạn có muốn cộng dồn số lượng?", MessageBoxIcon.Question))
                {
                    // Lấy row hiện có và cộng dồn
                    var row = gStoreDataSet.ChiTietGiaoDichKho.FindByMaGDMaSP(maGD, maSP);
                    if (row == null || row.RowState == DataRowState.Deleted)
                    {
                        MessageBox.Show("Không tìm thấy chi tiết hiện có để cập nhật.", "Thông báo",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    // Điều chỉnh tồn kho theo loại giao dịch hiện chọn (NHAP/XUAT)
                    int sign = InterpretTransactionSign(cbLoaiGD.Text); // +1 nhập, -1 xuất
                    ApplyProductAdjustments(new List<(int MaSP, int Delta)> { (maSP, soLuong * sign) });

                    // Cộng dồn số lượng và lưu
                    row.SoLuong = row.SoLuong + soLuong;
                    chiTietGiaoDichKhoBindingSource.EndEdit();
                    chiTietGiaoDichKhoTableAdapter.Update(row);   // update 1 row
                    chiTietGiaoDichKhoBindingSource.ResetCurrentItem();

                    RefreshDetails(false);
                    MessageBox.Show("Đã cộng dồn số lượng cho chi tiết giao dịch.", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    SetDetailMode(EditMode.None);
                    return;
                }
                else
                {
                    // Không cộng dồn thì thôi
                    return;
                }
            }

            // Chưa tồn tại -> thêm mới
            if (!AskConfirm("Xác nhận lưu chi tiết giao dịch?")) return;

            try
            {
                var newRow = gStoreDataSet.ChiTietGiaoDichKho.NewChiTietGiaoDichKhoRow();
                newRow.MaGD = maGD;
                newRow.MaSP = maSP;
                newRow.SoLuong = soLuong;

                gStoreDataSet.ChiTietGiaoDichKho.AddChiTietGiaoDichKhoRow(newRow);
                chiTietGiaoDichKhoBindingSource.EndEdit();
                chiTietGiaoDichKhoTableAdapter.Update(gStoreDataSet.ChiTietGiaoDichKho);

                // Điều chỉnh tồn kho theo loại GD
                ApplyProductAdjustments(new List<(int MaSP, int Delta)> { (maSP, soLuong * InterpretTransactionSign(cbLoaiGD.Text)) });

                RefreshDetails(false);
                MessageBox.Show("Đã thêm chi tiết giao dịch.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không thể thêm chi tiết.\n" + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                SetDetailMode(EditMode.None);
            }

        }

        private void btnSuaCT_Click(object sender, EventArgs e)
        {
            CommitUI();

            var view = chiTietGiaoDichKhoBindingSource.Current as DataRowView;
            if (view == null)
            {
                MessageBox.Show("Vui lòng chọn chi tiết giao dịch cần sửa.", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var row = view.Row as GStoreDataSet.ChiTietGiaoDichKhoRow;
            if (row == null || row.RowState == DataRowState.Deleted)
            {
                MessageBox.Show("Vui lòng chọn chi tiết giao dịch cần sửa.", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (row.SoLuong <= 0)
            {
                MessageBox.Show("Số lượng chi tiết phải lớn hơn 0 trước khi chỉnh sửa.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Pha 1: vào chế độ sửa
            if (detailMode != EditMode.Edit)
            {
                if (!AskConfirm("Bạn muốn sửa chi tiết giao dịch này?")) return;

                detailEditingKey = (row.MaGD, row.MaSP);
                SetDetailMode(EditMode.Edit);
                PopulateDetailInputs(row);
                return;
            }

            // Pha 2: LƯU
            if (!TryGetDetailValues(out int newMaGD, out int newMaSP, out int newSoLuong)) return;
            if (!AskConfirm("Xác nhận lưu thay đổi chi tiết giao dịch?")) return;

            try
            {
                // Giá trị cũ (để tính chênh lệch và thay thế nếu đổi khóa)
                int oldMaGD = (int)row["MaGD", DataRowVersion.Original];
                int oldMaSP = (int)row["MaSP", DataRowVersion.Original];
                int oldSL = (int)row["SoLuong", DataRowVersion.Original];

                bool keyChanged = (oldMaGD != newMaGD) || (oldMaSP != newMaSP);

                // Tính sign cũ/mới theo loại GD
                int oldSign = GetTransactionQuantitySign(oldMaGD, DataRowVersion.Original);
                if (oldSign == 0) oldSign = InterpretTransactionSign(cbLoaiGD.Text);

                int newSign = GetTransactionQuantitySign(newMaGD, DataRowVersion.Current);
                if (newSign == 0) newSign = InterpretTransactionSign(cbLoaiGD.Text);

                // Nếu có đổi khóa, cần thay thế hàng
                if (keyChanged)
                {
                    // Tránh trùng khóa (MaGD, MaSP) mới
                    if (DetailExists(newMaGD, newMaSP))
                    {
                        MessageBox.Show("Chi tiết với cặp (MaGD, MaSP) mới đã tồn tại. Vui lòng chọn khóa khác.",
                            "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    // Điều chỉnh tồn kho: hoàn tác số cũ, áp số mới
                    var deltas = new List<(int MaSP, int Delta)>
            {
                (oldMaSP, -oldSL * oldSign),
                (newMaSP,  newSoLuong * newSign)
            };
                    ApplyProductAdjustments(deltas);

                    // Thay thế hàng trong DataSet
                    // 1) Xóa hàng cũ
                    var oldView = chiTietGiaoDichKhoBindingSource.Current as DataRowView; // vẫn là view hiện tại
                    oldView.Delete();

                    // 2) Thêm hàng mới
                    var newRow = gStoreDataSet.ChiTietGiaoDichKho.NewChiTietGiaoDichKhoRow();
                    newRow.MaGD = newMaGD;
                    newRow.MaSP = newMaSP;
                    newRow.SoLuong = newSoLuong;
                    gStoreDataSet.ChiTietGiaoDichKho.AddChiTietGiaoDichKhoRow(newRow);

                    // Cập nhật DB
                    chiTietGiaoDichKhoBindingSource.EndEdit();
                    // (tuỳ cấu hình, có thể hữu ích)
                    try { chiTietGiaoDichKhoTableAdapter.Adapter.AcceptChangesDuringUpdate = true; } catch { }
                    chiTietGiaoDichKhoTableAdapter.Update(gStoreDataSet.ChiTietGiaoDichKho);

                    // Làm mới lưới & giữ vị trí gần kề
                    RefreshDetails(false);
                }
                else
                {
                    // Không đổi khóa -> cập nhật trực tiếp
                    // Điều chỉnh tồn kho theo chênh lệch số lượng
                    int deltaSL = newSoLuong - oldSL;
                    if (deltaSL != 0)
                    {
                        ApplyProductAdjustments(new List<(int MaSP, int Delta)> { (newMaSP, deltaSL * newSign) });
                    }

                    // Cập nhật row
                    row.SoLuong = newSoLuong;
                    // (nếu muốn cho phép sửa MaGD/MaSP mà không đổi, gán lại để chắc chắn)
                    row.MaGD = newMaGD;
                    row.MaSP = newMaSP;

                    chiTietGiaoDichKhoBindingSource.EndEdit();
                    chiTietGiaoDichKhoTableAdapter.Update(row);   // không Fill để giữ vị trí
                    chiTietGiaoDichKhoBindingSource.ResetCurrentItem();
                }

                MessageBox.Show("Đã cập nhật chi tiết giao dịch.", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không thể cập nhật chi tiết.\n" + ex.Message, "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                detailEditingKey = null;
                SetDetailMode(EditMode.None);
            }
        }

        private bool DetailExists(int maGD, int maSP)
        {
            try
            {
                // Nếu DataTable có composite PK được cấu hình trong Typed DataSet:
                var found = gStoreDataSet.ChiTietGiaoDichKho.FindByMaGDMaSP(maGD, maSP);
                if (found != null && found.RowState != DataRowState.Deleted) return true;
                return false;
            }
            catch
            {
                // Fallback LINQ nếu không có FindBy...
                return gStoreDataSet.ChiTietGiaoDichKho.Any(r =>
                    r.RowState != DataRowState.Deleted &&
                    r.MaGD == maGD && r.MaSP == maSP);
            }
        }


        private void btnXoaCT_Click(object sender, EventArgs e)
        {
            CommitUI();

            var view = chiTietGiaoDichKhoBindingSource.Current as DataRowView;
            if (view == null)
            {
                MessageBox.Show("Vui lòng chọn chi tiết giao dịch cần xóa.", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var row = view.Row as GStoreDataSet.ChiTietGiaoDichKhoRow;
            if (row == null || row.RowState == DataRowState.Deleted)
            {
                MessageBox.Show("Vui lòng chọn chi tiết giao dịch cần xóa.", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (row.SoLuong <= 0)
            {
                MessageBox.Show("Số lượng chi tiết phải lớn hơn 0 trước khi xóa.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }


            if (!AskConfirm("Bạn có chắc chắn muốn xóa chi tiết này?", MessageBoxIcon.Warning)) return;

            // Lưu vị trí mong muốn sau khi xóa (dịch về trước 1 dòng)

            int desiredPos = Math.Max(0, chiTietGiaoDichKhoBindingSource.Position - 1);

            try
            {
                // Lấy dữ liệu trước khi xóa để điều chỉnh tồn kho
                int maGD = row.MaGD;
                int maSP = row.MaSP;
                int sl = row.SoLuong;
                int sign = GetTransactionQuantitySign(maGD, DataRowVersion.Current);
                if (sign == 0) sign = InterpretTransactionSign(cbLoaiGD.Text);

                // Điều chỉnh tồn kho ngược lại lượng đã nhập/xuất
                ApplyProductAdjustments(new List<(int MaSP, int Delta)> { (maSP, -sl * sign) });

                // Xóa & cập nhật DB
                view.Delete();
                chiTietGiaoDichKhoBindingSource.EndEdit();
                chiTietGiaoDichKhoTableAdapter.Update(gStoreDataSet.ChiTietGiaoDichKho);

                // Refresh + khôi phục vị trí “gần kề”
                RefreshDetails(false);
                if (chiTietGiaoDichKhoBindingSource.Count > 0)
                    chiTietGiaoDichKhoBindingSource.Position =
                        Math.Min(desiredPos, chiTietGiaoDichKhoBindingSource.Count - 1);

                MessageBox.Show("Đã xóa chi tiết giao dịch.", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không thể xóa chi tiết.\n" + ex.Message, "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

      
        private void ClearTransactionInputs()
        {
            txtMaGD.Clear();
            cbLoaiGD.SelectedIndex = -1;
            dateTimePicker1.Value = DateTime.Now;
            cboMaNCC.SelectedIndex = -1;
            cboMaNV.SelectedIndex = cboMaNV.Items.Count > 0 ? 0 : -1;
            txtMaGD.Text = GenerateNextTransactionId().ToString();
            if (detailMode != EditMode.Add)
            {
                if (int.TryParse(txtMaGD.Text, out int newTransactionId) &&
     cboMaGD.Items.Cast<object>().Any(item => Convert.ToInt32(item) == newTransactionId))
                {
                    suppressTransactionComboUpdate = true;
                    cboMaGD.SelectedItem = newTransactionId;
                    suppressTransactionComboUpdate = false;
                    UpdateProductComboBoxForTransaction(newTransactionId);
                }
                else
                {
                    UpdateProductComboBoxForTransaction(GetCurrentDetailTransactionId());
                }
            }
            else
            {
                UpdateProductComboBoxForTransaction(GetCurrentDetailTransactionId());
            }
        }

        private void PopulateTransactionInputs(GStoreDataSet.GiaoDichKhoRow row)
        {
            txtMaGD.Text = row.MaGD.ToString();
            EnsureLoaiGiaoDichItem(row.LoaiGD);
            cbLoaiGD.SelectedItem = row.LoaiGD;
            dateTimePicker1.Value = row.NgayGD;
            PopulateSupplierComboBox(row.IsMaNCCNull() ? null : (int?)row.MaNCC);
            PopulateEmployeeComboBox(row.MaNhanVien);
        }
        private void EnsureLoaiGiaoDichItem(string loaiGiaoDich)
        {
            if (string.IsNullOrWhiteSpace(loaiGiaoDich))
            {
                return;
            }

            if (!cbLoaiGD.Items.Contains(loaiGiaoDich))
            {
                cbLoaiGD.Items.Add(loaiGiaoDich);
            }
        }

        private bool TryGetTransactionValues(out string loaiGD, out DateTime ngayGD, out int? maNcc, out int maNhanVien)
        {
            loaiGD = cbLoaiGD.SelectedItem as string ?? cbLoaiGD.Text.Trim();
            if (string.IsNullOrWhiteSpace(loaiGD))
            {
                MessageBox.Show("Vui lòng chọn loại giao dịch.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                ngayGD = DateTime.Now;
                maNcc = null;
                maNhanVien = 0;
                return false;
            }

            ngayGD = dateTimePicker1.Value;

            maNcc = GetSelectedSupplierId();

            var selectedEmployeeId = GetSelectedEmployeeId();
            if (!selectedEmployeeId.HasValue)
            {
                MessageBox.Show("Vui lòng chọn mã nhân viên hợp lệ.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                maNhanVien = 0;
                return false;
            }
            // 🔒 NHẬP bắt buộc có NCC
            var norm = loaiGD.Trim().ToUpperInvariant();
            if (norm == "NHAP" && !maNcc.HasValue)
            {
                MessageBox.Show("Giao dịch NHẬP bắt buộc phải chọn Mã nhà cung cấp.",
                                "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                maNhanVien = 0;
                return false;
            }

            if (norm == "XUAT" && !maNcc.HasValue)
            {
                MessageBox.Show("Giao dịch XUẤT bắt buộc phải chọn Mã nhà cung cấp.",
                                "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                maNhanVien = 0;
                return false;
            }
            if (!selectedEmployeeId.HasValue)
            {
                MessageBox.Show("Vui lòng chọn mã nhân viên hợp lệ.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                maNhanVien = 0;
                return false;
            }
            maNhanVien = selectedEmployeeId.Value;
            return true;
        }

        private void ClearDetailInputs()
        {
            if (detailMode != EditMode.Add)
            {
                suppressTransactionComboUpdate = true;
                cboMaGD.SelectedIndex = -1;
                suppressTransactionComboUpdate = false;
            }

            UpdateProductComboBoxForTransaction(GetCurrentDetailTransactionId());
            cbMaSP.SelectedIndex = -1;
            numericUpDown1.Value = Math.Max(numericUpDown1.Minimum, 1);
        }
        private void PopulateDetailInputs(GStoreDataSet.ChiTietGiaoDichKhoRow row)
        {
            suppressTransactionComboUpdate = true;
            cboMaGD.SelectedItem = row.MaGD;
            suppressTransactionComboUpdate = false;
            UpdateProductComboBoxForTransaction(row.MaGD, row.MaSP);
            numericUpDown1.Value = ClampQuantity(row.SoLuong);
        }

        private decimal ClampQuantity(int value)
        {
            if (value < numericUpDown1.Minimum)
            {
                return numericUpDown1.Minimum;
            }

            if (value > numericUpDown1.Maximum)
            {
                return numericUpDown1.Maximum;
            }

            return value;
        }

        private bool TryGetDetailValues(out int maGd, out int maSp, out int soLuong)
        {
            var selectedTransactionId = GetSelectedTransactionId();
            if (!selectedTransactionId.HasValue)
            {
                MessageBox.Show("Mã giao dịch không hợp lệ.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                maSp = 0;
                soLuong = 0;
                maGd = 0;
                return false;
            }
            maGd = selectedTransactionId.Value;


            int? selectedProductId = GetSelectedProductId();
            if (!selectedProductId.HasValue)
            {
                MessageBox.Show("Vui lòng chọn mã sản phẩm.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                maSp = 0;
                soLuong = 0;
                return false;
            }
            maSp = selectedProductId.Value;
            soLuong = (int)numericUpDown1.Value;
            if (soLuong <= 0)
            {
                MessageBox.Show("Số lượng phải lớn hơn 0.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }

        private void RefreshTransactions(bool showError)
        {
            int? currentSelectedTransaction = GetSelectedTransactionId();
            try
            {
                giaoDichKhoTableAdapter.Fill(gStoreDataSet.GiaoDichKho);
                giaoDichBindingSource.ResetBindings(false);
                PopulateTransactionComboBox(currentSelectedTransaction);
            }
            catch (Exception ex)
            {
                if (showError)
                {
                    MessageBox.Show($"Không thể tải lại giao dịch kho.\nChi tiết: {ex.Message}", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }
        private void RefreshDetails(bool showError)
        {
            try
            {
                chiTietGiaoDichKhoTableAdapter.Fill(gStoreDataSet.ChiTietGiaoDichKho);
                chiTietGiaoDichKhoBindingSource.ResetBindings(false);
            }
            catch (Exception ex)
            {
                if (showError)
                {
                    MessageBox.Show($"Không thể tải lại chi tiết giao dịch.\nChi tiết: {ex.Message}", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }
        private int GenerateNextTransactionId()
        {
            int maxId = 0;
            foreach (GStoreDataSet.GiaoDichKhoRow row in gStoreDataSet.GiaoDichKho.Rows)
            {
                if (row.RowState == DataRowState.Deleted)
                {
                    continue;
                }

                int currentId = row.MaGD;
                if (currentId > maxId)
                {
                    maxId = currentId;
                }
            }

            return maxId + 1;
        }

        private void ApplyProductAdjustments(List<(int MaSP, int Delta)> adjustments)
        {
            if (adjustments == null || adjustments.Count == 0)
            {
                return;
            }

            EnsureLookupDataLoaded();

            bool hasChanges = false;

            foreach (var adjustment in adjustments)
            {
                if (adjustment.Delta == 0)
                {
                    continue;
                }

                var productRow = gStoreDataSet.SanPham.FindByMaSP(adjustment.MaSP);
                if (productRow == null)
                {
                    MessageBox.Show($"Không tìm thấy sản phẩm với mã {adjustment.MaSP} để cập nhật số lượng.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    continue;
                }

                int newQuantity = productRow.SoLuong + adjustment.Delta;
                if (newQuantity < 0)
                {
                    newQuantity = 0;
                }

                productRow.SoLuong = newQuantity;
                hasChanges = true;
            }

            if (!hasChanges)
            {
                return;
            }

            try
            {
                sanPhamTableAdapter.Update(gStoreDataSet.SanPham);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Cập nhật số lượng sản phẩm thất bại.\nChi tiết: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                RefreshProducts();
            }
        }
        private List<(int MaSP, int Delta)> CalculateProductAdjustmentsFromPendingChanges()
        {
            var aggregatedAdjustments = new Dictionary<int, int>();

            foreach (GStoreDataSet.ChiTietGiaoDichKhoRow row in gStoreDataSet.ChiTietGiaoDichKho.Rows)
            {
                switch (row.RowState)
                {
                    case DataRowState.Added:
                        AddProductAdjustment(aggregatedAdjustments, row.MaSP, row.SoLuong * GetTransactionQuantitySign(row.MaGD, DataRowVersion.Current));
                        break;
                    case DataRowState.Modified:
                        int originalMaGd = (int)row["MaGD", DataRowVersion.Original];
                        int originalMaSp = (int)row["MaSP", DataRowVersion.Original];
                        int originalQuantity = (int)row["SoLuong", DataRowVersion.Original];
                        AddProductAdjustment(aggregatedAdjustments, originalMaSp, -originalQuantity * GetTransactionQuantitySign(originalMaGd, DataRowVersion.Original));

                        AddProductAdjustment(aggregatedAdjustments, row.MaSP, row.SoLuong * GetTransactionQuantitySign(row.MaGD, DataRowVersion.Current));
                        break;
                    case DataRowState.Deleted:
                        int deletedMaGd = (int)row["MaGD", DataRowVersion.Original];
                        int deletedMaSp = (int)row["MaSP", DataRowVersion.Original];
                        int deletedQuantity = (int)row["SoLuong", DataRowVersion.Original];
                        AddProductAdjustment(aggregatedAdjustments, deletedMaSp, -deletedQuantity * GetTransactionQuantitySign(deletedMaGd, DataRowVersion.Original));
                        break;
                }
            }

            return aggregatedAdjustments
                .Where(kvp => kvp.Value != 0)
                .Select(kvp => (kvp.Key, kvp.Value))
                .ToList();
        }
        private void AddProductAdjustment(Dictionary<int, int> adjustments, int maSp, int delta)
        {
            if (delta == 0)
            {
                return;
            }

            if (adjustments.TryGetValue(maSp, out int existing))
            {
                adjustments[maSp] = existing + delta;
            }
            else
            {
                adjustments[maSp] = delta;
            }
        }

        private int GetTransactionQuantitySign(int maGd, DataRowVersion version)
        {
            var transactionRow = gStoreDataSet.GiaoDichKho.FindByMaGD(maGd);
            if (transactionRow != null && transactionRow.RowState != DataRowState.Deleted)
            {
                if (version == DataRowVersion.Original && transactionRow.HasVersion(DataRowVersion.Original))
                {
                    return InterpretTransactionSign(transactionRow["LoaiGD", DataRowVersion.Original]?.ToString());
                }

                return InterpretTransactionSign(transactionRow.LoaiGD);
            }

            foreach (GStoreDataSet.GiaoDichKhoRow row in gStoreDataSet.GiaoDichKho.Rows)
            {
                if (row.RowState == DataRowState.Deleted)
                {
                    int originalId = (int)row["MaGD", DataRowVersion.Original];
                    if (originalId == maGd)
                    {
                        return InterpretTransactionSign(row["LoaiGD", DataRowVersion.Original]?.ToString());
                    }
                }
            }

            return 0;
        }

        private int InterpretTransactionSign(string loaiGiaoDich)
        {
            if (string.IsNullOrWhiteSpace(loaiGiaoDich))
            {
                return 0;
            }

            string normalized = loaiGiaoDich.Trim().ToUpperInvariant();
            if (normalized == "NHAP")
            {
                return 1;
            }

            if (normalized == "XUAT")
            {
                return -1;
            }

            return 0;
        }
        private void RefreshProducts()
        {
            try
            {
                sanPhamTableAdapter.Fill(gStoreDataSet.SanPham);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Không thể tải lại dữ liệu sản phẩm.\nChi tiết: {ex.Message}", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            if (transactionMode == EditMode.Add || transactionMode == EditMode.Edit) return;

            if (giaoDichBindingSource.Current is DataRowView view &&
                view.Row is GStoreDataSet.GiaoDichKhoRow row &&
                row.RowState != DataRowState.Deleted)
            {
                PopulateTransactionInputs(row);
                int? selectedProductId = GetSelectedProductId();
                int? targetTransactionId = (detailMode != EditMode.Add) ? row.MaGD : GetCurrentDetailTransactionId();

                if (detailMode != EditMode.Add)
                {
                    suppressTransactionComboUpdate = true;
                    cboMaGD.SelectedItem = row.MaGD;
                    suppressTransactionComboUpdate = false;
                    targetTransactionId = row.MaGD;
                }

                if (!targetTransactionId.HasValue) targetTransactionId = row.MaGD;
                UpdateProductComboBoxForTransaction(targetTransactionId, selectedProductId);
            }
        }

        private void dataGridView2_SelectionChanged(object sender, EventArgs e)
        {
            if (detailMode == EditMode.Add || detailMode == EditMode.Edit) return;

            if (chiTietGiaoDichKhoBindingSource.Current is DataRowView view &&
                view.Row is GStoreDataSet.ChiTietGiaoDichKhoRow row &&
                row.RowState != DataRowState.Deleted)
            {
                PopulateDetailInputs(row);
            }
        }

        private void cboMaNCC_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (suppressProductComboUpdate)
            {
                return;
            }

            UpdateProductComboBoxForTransaction(GetCurrentDetailTransactionId(), GetSelectedProductId());
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
        // ======= CANCEL / RESTORE HELPERS =======
        private void CancelTransactionPendingEdits()
        {
            
            try { dataGridView1.CancelEdit(); } catch { }
            try { giaoDichBindingSource.CancelEdit(); } catch { }
            try { gStoreDataSet.GiaoDichKho.RejectChanges(); } catch { }
        }

        private void CancelDetailPendingEdits()
        {
            
            try { dataGridView2.CancelEdit(); } catch { }
            try { chiTietGiaoDichKhoBindingSource.CancelEdit(); } catch { }
            try { gStoreDataSet.ChiTietGiaoDichKho.RejectChanges(); } catch { }
        }

        private void RestoreTransactionInputs(int? focusMaGD = null)
        {
            // Nếu có MaGD cần focus -> di chuyển con trỏ tới đúng dòng
            if (focusMaGD.HasValue)
            {
                var rows = gStoreDataSet.GiaoDichKho.Select($"MaGD = {focusMaGD.Value}");
                if (rows.Length > 0)
                {
                    int pos = gStoreDataSet.GiaoDichKho.Rows.IndexOf(rows[0]);
                    if (pos >= 0) giaoDichBindingSource.Position = pos;
                }
            }

            // Cập nhật khu nhập theo selection hiện tại
            if (giaoDichBindingSource.Current is DataRowView v &&
                v.Row is GStoreDataSet.GiaoDichKhoRow row &&
                row.RowState != DataRowState.Deleted)
            {
                PopulateTransactionInputs(row);

                // Giữ lựa chọn sản phẩm nếu có
                int? selectedProductId = GetSelectedProductId();
                int? targetTransactionId = (detailMode != EditMode.Add) ? row.MaGD : GetCurrentDetailTransactionId();
                if (!targetTransactionId.HasValue) targetTransactionId = row.MaGD;
                UpdateProductComboBoxForTransaction(targetTransactionId, selectedProductId);
            }
            else
            {
                ClearTransactionInputs();
            }

            try { giaoDichBindingSource.ResetCurrentItem(); } catch { }
        }

        private void RestoreDetailInputs((int MaGD, int MaSP)? focusKey = null)
        {
            if (focusKey.HasValue)
            {
                // Dò lại dòng detail theo khóa ghép
                foreach (DataRowView rv in chiTietGiaoDichKhoBindingSource)
                {
                    var r = rv.Row as GStoreDataSet.ChiTietGiaoDichKhoRow;
                    if (r != null && r.RowState != DataRowState.Deleted &&
                        r.MaGD == focusKey.Value.MaGD && r.MaSP == focusKey.Value.MaSP)
                    {
                        chiTietGiaoDichKhoBindingSource.Position = chiTietGiaoDichKhoBindingSource.IndexOf(rv);
                        break;
                    }
                }
            }

            if (chiTietGiaoDichKhoBindingSource.Current is DataRowView v &&
                v.Row is GStoreDataSet.ChiTietGiaoDichKhoRow row &&
                row.RowState != DataRowState.Deleted)
            {
                PopulateDetailInputs(row);
            }
            else
            {
                ClearDetailInputs();
            }

            try { chiTietGiaoDichKhoBindingSource.ResetCurrentItem(); } catch { }
        }

        private void btnHuy_Click(object sender, EventArgs e)
        {
            // Nếu đang không ở chế độ Add/Edit thì chỉ làm tươi lại inputs
            if (transactionMode == EditMode.None)
            {
                RestoreTransactionInputs(transactionEditingId);
                return;
            }

            if (!AskConfirm("Hủy các thay đổi giao dịch đang thực hiện?")) return;

            // Rollback mọi chỉnh sửa chưa lưu của header
            CancelTransactionPendingEdits();

            // Trả UI về trạng thái bình thường
            SetTransactionMode(EditMode.None);

            // Khôi phục nhập liệu theo selection (ưu tiên quay về đúng giao dịch đang sửa dở)
            RestoreTransactionInputs(transactionEditingId);

            // Xóa dấu vết phiên sửa
            transactionEditingId = null;
        }

        private void btnHuyCT_Click(object sender, EventArgs e)
        {
            // Nếu không ở chế độ Add/Edit detail thì chỉ làm tươi lại inputs
            if (detailMode == EditMode.None)
            {
                RestoreDetailInputs(detailEditingKey);
                return;
            }

            if (!AskConfirm("Hủy các thay đổi chi tiết giao dịch đang thực hiện?")) return;

            // Rollback mọi chỉnh sửa chưa lưu của detail
            CancelDetailPendingEdits();

            // Trả UI về trạng thái bình thường
            SetDetailMode(EditMode.None);

            // Khôi phục nhập liệu (ưu tiên khóa đang sửa dở nếu còn tồn tại)
            RestoreDetailInputs(detailEditingKey);

            // Clear key đang sửa
            detailEditingKey = null;

            // Đồng bộ lại combobox sản phẩm theo MaGD hiện tại ở khu detail
            UpdateProductComboBoxForTransaction(GetCurrentDetailTransactionId(), GetSelectedProductId());
        }

        private void cboMaNCC_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            var norm = (cbLoaiGD.SelectedItem as string ?? cbLoaiGD.Text ?? "").Trim().ToUpperInvariant();
            bool requireNcc = norm == "NHAP" || norm == "XUAT";
            cboMaNCC.Enabled = true;
           

            // Cập nhật lại danh sách sản phẩm theo NCC hiện tại (nếu có)
            UpdateProductComboBoxForTransaction(GetCurrentDetailTransactionId(), GetSelectedProductId());
        }
    }
}
