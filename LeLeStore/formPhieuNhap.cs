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
        private readonly GiaoDichKhoTableAdapter giaoDichKhoTableAdapter = new GiaoDichKhoTableAdapter();
        private readonly NhaCungCapTableAdapter nhaCungCapTableAdapter = new NhaCungCapTableAdapter();
        private readonly SanPhamTableAdapter sanPhamTableAdapter = new SanPhamTableAdapter();
        private readonly BindingSource giaoDichBindingSource = new BindingSource();
        private class ProductComboItem
        {
            public int MaSP { get; set; }
            public string Display { get; set; }
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
        public formPhieuNhap()
        {
            InitializeComponent();

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

            txtMaGD1.TextChanged += txtMaGD1_TextChanged;

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

        private int? GetCurrentDetailTransactionId()
        {
            if (int.TryParse(txtMaGD1.Text.Trim(), out int maGd))
            {
                return maGd;
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

        private void txtMaGD1_TextChanged(object sender, EventArgs e)
        {
            if (suppressProductComboUpdate)
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
                txtMaNCC.Text = supplier.MaNCC.ToString();
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
            transactionMode = EditMode.Add;
            transactionEditingId = null;
            transactionPendingChanges = false;
            ClearTransactionInputs();
            cbLoaiGD.Focus();
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            if (giaoDichBindingSource.Current is DataRowView view && view.Row is GStoreDataSet.GiaoDichKhoRow row && row.RowState != DataRowState.Deleted)
            {
                view.Delete();
                transactionPendingChanges = true;
                transactionMode = EditMode.None;
                transactionEditingId = null;
                giaoDichBindingSource.ResetBindings(false);
            }
            else
            {
                MessageBox.Show("Vui lòng chọn giao dịch cần xóa.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            if (giaoDichBindingSource.Current is DataRowView view && view.Row is GStoreDataSet.GiaoDichKhoRow row && row.RowState != DataRowState.Deleted)
            {
                transactionMode = EditMode.Edit;
                transactionEditingId = row.MaGD;
                PopulateTransactionInputs(row);
            }
            else
            {
                MessageBox.Show("Vui lòng chọn giao dịch cần sửa.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btnLuu_Click(object sender, EventArgs e)
        {
            GStoreDataSet.GiaoDichKhoRow newRow = null;

            if (transactionMode == EditMode.Add)
            {
                if (!TryGetTransactionValues(out string loaiGD, out DateTime ngayGD, out int? maNcc, out int maNhanVien))
                {
                    return;
                }

                newRow = gStoreDataSet.GiaoDichKho.NewGiaoDichKhoRow();
                newRow.LoaiGD = loaiGD;
                newRow.NgayGD = ngayGD;
                if (maNcc.HasValue)
                {
                    newRow.MaNCC = maNcc.Value;
                }
                else
                {
                    newRow.SetMaNCCNull();
                }

                newRow.MaNhanVien = maNhanVien;
                gStoreDataSet.GiaoDichKho.AddGiaoDichKhoRow(newRow);
                transactionPendingChanges = true;
            }
            else if (transactionMode == EditMode.Edit)
            {
                if (!transactionEditingId.HasValue)
                {
                    MessageBox.Show("Không xác định được giao dịch cần sửa.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (!TryGetTransactionValues(out string loaiGD, out DateTime ngayGD, out int? maNcc, out int maNhanVien))
                {
                    return;
                }

                var existingRow = gStoreDataSet.GiaoDichKho.FindByMaGD(transactionEditingId.Value);
                if (existingRow == null)
                {
                    MessageBox.Show("Không tìm thấy giao dịch để cập nhật.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                existingRow.LoaiGD = loaiGD;
                existingRow.NgayGD = ngayGD;
                if (maNcc.HasValue)
                {
                    existingRow.MaNCC = maNcc.Value;
                }
                else
                {
                    existingRow.SetMaNCCNull();
                }

                existingRow.MaNhanVien = maNhanVien;
                transactionPendingChanges = true;
            }

            if (!transactionPendingChanges)
            {
                MessageBox.Show("Không có thay đổi để lưu.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                giaoDichBindingSource.EndEdit();
                int affected = giaoDichKhoTableAdapter.Update(gStoreDataSet.GiaoDichKho);
                transactionPendingChanges = false;
                transactionMode = EditMode.None;
                transactionEditingId = null;

                if (newRow != null)
                {
                    txtMaGD.Text = newRow.MaGD.ToString();
                    suppressProductComboUpdate = true;
                    txtMaGD1.Text = newRow.MaGD.ToString();
                    suppressProductComboUpdate = false;
                    UpdateProductComboBoxForTransaction(newRow.MaGD);
                }

                RefreshTransactions(false);
                MessageBox.Show($"Đã lưu {affected} thay đổi giao dịch.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lưu giao dịch thất bại.\nChi tiết: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnThemCT_Click(object sender, EventArgs e)
        {
            detailMode = EditMode.Add;
            detailEditingKey = null;
            detailPendingChanges = false;
            ClearDetailInputs();

            if (int.TryParse(txtMaGD.Text, out int maGd))
            {
                suppressProductComboUpdate = true;
                txtMaGD1.Text = maGd.ToString();
                txtMaGD1.Text = maGd.ToString();
                suppressProductComboUpdate = false;
                UpdateProductComboBoxForTransaction(maGd);
            }
            else
            {
                UpdateProductComboBoxForTransaction(null);
            }

            txtMaGD1.Focus();
        }

        private void btnSuaCT_Click(object sender, EventArgs e)
        {
            if (chiTietGiaoDichKhoBindingSource.Current is DataRowView view && view.Row is GStoreDataSet.ChiTietGiaoDichKhoRow row && row.RowState != DataRowState.Deleted)
            {
                detailMode = EditMode.Edit;
                detailEditingKey = (row.MaGD, row.MaSP);
                PopulateDetailInputs(row);
            }
            else
            {
                MessageBox.Show("Vui lòng chọn chi tiết giao dịch cần sửa.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btnXoaCT_Click(object sender, EventArgs e)
        {
            if (chiTietGiaoDichKhoBindingSource.Current is DataRowView view && view.Row is GStoreDataSet.ChiTietGiaoDichKhoRow row && row.RowState != DataRowState.Deleted)
            {
                view.Delete();
                detailPendingChanges = true;
                detailMode = EditMode.None;
                detailEditingKey = null;
                chiTietGiaoDichKhoBindingSource.ResetBindings(false);
            }
            else
            {
                MessageBox.Show("Vui lòng chọn chi tiết giao dịch cần xóa.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btnLuuCT_Click(object sender, EventArgs e)
        {
          
            if (detailMode != EditMode.None)
            {
                if (!TryGetDetailValues(out int maGd, out int maSp, out int soLuong))
                {
                    return;
                }

                if (detailMode == EditMode.Add)
                {
                    var existing = gStoreDataSet.ChiTietGiaoDichKho.FindByMaGDMaSP(maGd, maSp);
                    if (existing != null && existing.RowState != DataRowState.Deleted)
                    {
                        MessageBox.Show("Chi tiết giao dịch đã tồn tại.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    var newRow = gStoreDataSet.ChiTietGiaoDichKho.NewChiTietGiaoDichKhoRow();
                    newRow.MaGD = maGd;
                    newRow.MaSP = maSp;
                    newRow.SoLuong = soLuong;
                    gStoreDataSet.ChiTietGiaoDichKho.AddChiTietGiaoDichKhoRow(newRow);
                    detailPendingChanges = true;
                   
                }
                else if (detailMode == EditMode.Edit)
                {
                    if (!detailEditingKey.HasValue)
                    {
                        MessageBox.Show("Không xác định được chi tiết cần sửa.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    var row = gStoreDataSet.ChiTietGiaoDichKho.FindByMaGDMaSP(detailEditingKey.Value.MaGD, detailEditingKey.Value.MaSP);
                    if (row == null)
                    {
                        MessageBox.Show("Không tìm thấy chi tiết giao dịch để cập nhật.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    var duplicate = gStoreDataSet.ChiTietGiaoDichKho.FindByMaGDMaSP(maGd, maSp);
                    if (duplicate != null && duplicate != row && duplicate.RowState != DataRowState.Deleted)
                    {
                        MessageBox.Show("Chi tiết giao dịch với mã này đã tồn tại.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    int originalMaSp = row.MaSP;
                    int originalQuantity = row.SoLuong;
                    row.MaGD = maGd;
                    row.MaSP = maSp;
                    row.SoLuong = soLuong;
                    detailPendingChanges = true;
                   
                }
            }

            if (!detailPendingChanges)
            {
                MessageBox.Show("Không có thay đổi để lưu.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                chiTietGiaoDichKhoBindingSource.EndEdit();
                var productAdjustments = CalculateProductAdjustmentsFromPendingChanges();
                int affected = chiTietGiaoDichKhoTableAdapter.Update(gStoreDataSet.ChiTietGiaoDichKho);
                ApplyProductAdjustments(productAdjustments);
                detailPendingChanges = false;
                detailMode = EditMode.None;
                detailEditingKey = null;
                RefreshDetails(false);
                MessageBox.Show($"Đã lưu {affected} thay đổi chi tiết giao dịch.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lưu chi tiết giao dịch thất bại.\nChi tiết: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void ClearTransactionInputs()
        {
            txtMaGD.Clear();
            cbLoaiGD.SelectedIndex = -1;
            dateTimePicker1.Value = DateTime.Now;
            txtMaNCC.Clear();
            txtMaNV.Clear();
            txtMaGD.Text = GenerateNextTransactionId().ToString();
            if (detailMode != EditMode.Add)
            {
                suppressProductComboUpdate = true;
                txtMaGD1.Text = txtMaGD.Text;
                suppressProductComboUpdate = false;

                if (int.TryParse(txtMaGD.Text, out int newTransactionId))
                {
                    UpdateProductComboBoxForTransaction(newTransactionId);
                }
                else
                {
                    UpdateProductComboBoxForTransaction(null);
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
            txtMaNCC.Text = row.IsMaNCCNull() ? string.Empty : row.MaNCC.ToString();
            txtMaNV.Text = row.MaNhanVien.ToString();
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

            maNcc = null;
            if (!string.IsNullOrWhiteSpace(txtMaNCC.Text))
            {
                if (int.TryParse(txtMaNCC.Text.Trim(), out int parsedMaNcc))
                {
                    maNcc = parsedMaNcc;
                }
                else
                {
                    MessageBox.Show("Mã nhà cung cấp không hợp lệ.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    maNhanVien = 0;
                    return false;
                }
            }

            if (!int.TryParse(txtMaNV.Text.Trim(), out maNhanVien))
            {
                MessageBox.Show("Mã nhân viên không hợp lệ.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }

        private void ClearDetailInputs()
        {
            if (detailMode != EditMode.Add)
            {
                suppressProductComboUpdate = true;
                txtMaGD1.Clear();
                suppressProductComboUpdate = false;
            }

            UpdateProductComboBoxForTransaction(GetCurrentDetailTransactionId());
            cbMaSP.SelectedIndex = -1;
            numericUpDown1.Value = Math.Max(numericUpDown1.Minimum, 1);
        }
        private void PopulateDetailInputs(GStoreDataSet.ChiTietGiaoDichKhoRow row)
        {
            suppressProductComboUpdate = true;
            txtMaGD1.Text = row.MaGD.ToString();
            suppressProductComboUpdate = false;
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
            if (!int.TryParse(txtMaGD1.Text.Trim(), out maGd))
            {
                MessageBox.Show("Mã giao dịch không hợp lệ.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                maSp = 0;
                soLuong = 0;
                return false;
            }

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
            try
            {
                giaoDichKhoTableAdapter.Fill(gStoreDataSet.GiaoDichKho);
                giaoDichBindingSource.ResetBindings(false);
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
            if (giaoDichBindingSource.Current is DataRowView view && view.Row is GStoreDataSet.GiaoDichKhoRow row && row.RowState != DataRowState.Deleted)
            {
                PopulateTransactionInputs(row);
                int? selectedProductId = GetSelectedProductId();
                int? targetTransactionId = detailMode != EditMode.Add ? row.MaGD : GetCurrentDetailTransactionId();


                if (detailMode != EditMode.Add)
                {
                    suppressProductComboUpdate = true;
                    txtMaGD1.Text = row.MaGD.ToString();
                    suppressProductComboUpdate = false;
                    targetTransactionId = row.MaGD;
                }

                if (!targetTransactionId.HasValue)
                {
                    targetTransactionId = row.MaGD;
                }
                UpdateProductComboBoxForTransaction(targetTransactionId, selectedProductId);
            }
        }

        private void dataGridView2_SelectionChanged(object sender, EventArgs e)
        {
            if (chiTietGiaoDichKhoBindingSource.Current is DataRowView view && view.Row is GStoreDataSet.ChiTietGiaoDichKhoRow row && row.RowState != DataRowState.Deleted)
            {
                PopulateDetailInputs(row);
            }
        }
    }
}
