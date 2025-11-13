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
                txtMaSP.Text = product.MaSP.ToString();
                hasResult = true;
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
                    txtMaGD1.Text = newRow.MaGD.ToString();
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
                txtMaGD1.Text = maGd.ToString();
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
                int affected = chiTietGiaoDichKhoTableAdapter.Update(gStoreDataSet.ChiTietGiaoDichKho);
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
                txtMaGD1.Clear();
            }

            txtMaSP.Clear();
            numericUpDown1.Value = Math.Max(numericUpDown1.Minimum, 1);
        }
        private void PopulateDetailInputs(GStoreDataSet.ChiTietGiaoDichKhoRow row)
        {
            txtMaGD1.Text = row.MaGD.ToString();
            txtMaSP.Text = row.MaSP.ToString();
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

            if (!int.TryParse(txtMaSP.Text.Trim(), out maSp))
            {
                MessageBox.Show("Mã sản phẩm không hợp lệ.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                soLuong = 0;
                return false;
            }

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

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            if (giaoDichBindingSource.Current is DataRowView view && view.Row is GStoreDataSet.GiaoDichKhoRow row && row.RowState != DataRowState.Deleted)
            {
                PopulateTransactionInputs(row);

                if (detailMode != EditMode.Add)
                {
                    txtMaGD1.Text = row.MaGD.ToString();
                }
            }
        }

        private void dataGridView2_SelectionChanged(object sender, EventArgs e)
        {
            if (giaoDichBindingSource.Current is DataRowView view && view.Row is GStoreDataSet.GiaoDichKhoRow row && row.RowState != DataRowState.Deleted)
            {
                PopulateTransactionInputs(row);

                if (detailMode != EditMode.Add)
                {
                    txtMaGD1.Text = row.MaGD.ToString();
                }
            }
        }
    }
}
