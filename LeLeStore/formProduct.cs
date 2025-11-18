using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LeLeStore
{
    public partial class formProduct : Form
    {
        private enum ProductOperation
        {
            None,
            Add,
            Edit,
            Delete
        }

        private readonly Dictionary<string, Image> _imageCache = new Dictionary<string, Image>(StringComparer.OrdinalIgnoreCase);
        private ProductOperation _currentOperation = ProductOperation.None;

        public formProduct()
        {
            InitializeComponent();
        }

        private void formProduct_Load(object sender, EventArgs e)
        {
            // TODO: This line of code loads data into the 'gStoreDataSet.SanPham' table. You can move, or remove it, as needed.
            this.sanPhamTableAdapter.Fill(this.gStoreDataSet.SanPham);
            // số lượng
          

           

        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {

        }

        
        private void SetOperation(ProductOperation operation)
        {
            _currentOperation = operation;

            bool canEdit = operation == ProductOperation.Add || operation == ProductOperation.Edit;
            bool isDelete = operation == ProductOperation.Delete;

            txtMaSP.ReadOnly = true;
            txtTenSP.ReadOnly = !canEdit;
            txtDG.ReadOnly = !canEdit;
            numericUpDown1.Enabled = canEdit;
            txtHSD.ReadOnly = !canEdit;
            txtMLoai.ReadOnly = !canEdit;
            txtMDV.ReadOnly = !canEdit;
            txtNCC.ReadOnly = !canEdit;
            txtMaNV.ReadOnly = !canEdit;
            txtHinhAnh.ReadOnly = !canEdit;
            btnChonAnh.Enabled = canEdit;

            if (isDelete)
            {
                txtTenSP.ReadOnly = true;
                txtDG.ReadOnly = true;
                numericUpDown1.Enabled = false;
                txtHSD.ReadOnly = true;
                txtMLoai.ReadOnly = true;
                txtMDV.ReadOnly = true;
                txtNCC.ReadOnly = true;
                txtMaNV.ReadOnly = true;
                txtHinhAnh.ReadOnly = true;
                btnChonAnh.Enabled = false;
            }

            // Đổi text & khóa nút
            btnThem.Text = (operation == ProductOperation.Add) ? "Lưu" : "Thêm";
            btnSua.Text = (operation == ProductOperation.Edit) ? "Lưu" : "Sửa";

            btnThem.Enabled = (operation != ProductOperation.Edit && operation != ProductOperation.Delete);
            btnSua.Enabled = (operation != ProductOperation.Add && operation != ProductOperation.Delete);
            btnXoa.Enabled = (operation == ProductOperation.None);

            // Hủy chỉ bật khi đang thao tác
            btnHuy.Enabled = (operation != ProductOperation.None);

        }

        private void PopulateInputsFromSelection()
        {
            var row = GetCurrentProductRow();
            if (row != null)
            {
                PopulateInputs(row);
            }
            else
            {
                ClearInputFields();
            }
        }
        private GStoreDataSet.SanPhamRow GetCurrentProductRow()
        {
            if (dataGridView1.CurrentRow?.DataBoundItem is DataRowView rowView)
            {
                return rowView.Row as GStoreDataSet.SanPhamRow;
            }

            return null;
        }

        private void PopulateInputs(GStoreDataSet.SanPhamRow row)
        {
            txtMaSP.Text = row.MaSP.ToString();
            txtTenSP.Text = row.TenSP;
            txtDG.Text = row.DonGia.ToString();
            int minQuantity = (int)numericUpDown1.Minimum;
            int maxQuantity = (int)numericUpDown1.Maximum;
            int clampedQuantity = Math.Min(Math.Max(row.SoLuong, minQuantity), maxQuantity);
            numericUpDown1.Value = clampedQuantity;
            txtHSD.Text = row.IsHanSuDungNull() ? string.Empty : row.HanSuDung.ToString("yyyy-MM-dd");
            txtMLoai.Text = row.MaLoai.ToString();
            txtMDV.Text = row.MaDVT.ToString();
            txtNCC.Text = row.IsMaNCCNull() ? string.Empty : row.MaNCC.ToString();
            txtMaNV.Text = row.IsMaNhanVienNull() ? string.Empty : row.MaNhanVien.ToString();
            txtHinhAnh.Text = row.IsHinhAnhNull() ? string.Empty : row.HinhAnh;
            UpdateImagePreview();
        }

        private void ClearInputFields()
        {
            txtMaSP.Text = string.Empty;
            txtTenSP.Text = string.Empty;
            txtDG.Text = string.Empty;
            numericUpDown1.Value = numericUpDown1.Minimum;
            txtHSD.Text = string.Empty;
            txtMLoai.Text = string.Empty;
            txtMDV.Text = string.Empty;
            txtNCC.Text = string.Empty;
            txtMaNV.Text = string.Empty;
            txtHinhAnh.Text = string.Empty;
            UpdateImagePreview();
        }

        private bool TryValidateInputs(
            out string tenSp,
            out decimal donGia,
            out int soLuong,
            out DateTime? hanSuDung,
            out int maLoai,
            out int maDvt,
            out int? maNcc,
            out int? maNhanVien,
            out string hinhAnhPath)
        {
            tenSp = txtTenSP.Text.Trim();
            donGia = 0;
            soLuong = (int)numericUpDown1.Value;
            hanSuDung = null;
            maLoai = 0;
            maDvt = 0;
            maNcc = null;
            maNhanVien = null;
            hinhAnhPath = txtHinhAnh.Text.Trim();

            if (string.IsNullOrWhiteSpace(tenSp))
            {
                MessageBox.Show("Tên sản phẩm không được để trống.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtTenSP.Focus();
                return false;
            }

            if (!decimal.TryParse(txtDG.Text.Trim(), out donGia))
            {
                MessageBox.Show("Đơn giá không hợp lệ.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtDG.Focus();
                return false;
            }

            if (!string.IsNullOrWhiteSpace(txtHSD.Text))
            {
                if (DateTime.TryParse(txtHSD.Text.Trim(), out DateTime parsedDate))
                {
                    hanSuDung = parsedDate;
                }
                else
                {
                    MessageBox.Show("Hạn sử dụng không hợp lệ.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtHSD.Focus();
                    return false;
                }
            }

            if (!int.TryParse(txtMLoai.Text.Trim(), out maLoai))
            {
                MessageBox.Show("Mã loại không hợp lệ.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtMLoai.Focus();
                return false;
            }

            if (!int.TryParse(txtMDV.Text.Trim(), out maDvt))
            {
                MessageBox.Show("Mã đơn vị tính không hợp lệ.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtMDV.Focus();
                return false;
            }

            if (!string.IsNullOrWhiteSpace(txtNCC.Text))
            {
                if (int.TryParse(txtNCC.Text.Trim(), out int parsedNcc))
                {
                    maNcc = parsedNcc;
                }
                else
                {
                    MessageBox.Show("Mã nhà cung cấp không hợp lệ.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtNCC.Focus();
                    return false;
                }
            }

            if (!string.IsNullOrWhiteSpace(txtMaNV.Text))
            {
                if (int.TryParse(txtMaNV.Text.Trim(), out int parsedNhanVien))
                {
                    maNhanVien = parsedNhanVien;
                }
                else
                {
                    MessageBox.Show("Mã nhân viên không hợp lệ.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtMaNV.Focus();
                    return false;
                }
            }

            if (!string.IsNullOrWhiteSpace(hinhAnhPath))
            {
                string resolved = ResolveImagePath(hinhAnhPath);
                if (!File.Exists(resolved))
                {
                    MessageBox.Show("Đường dẫn hình ảnh không tồn tại.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtHinhAnh.Focus();
                    return false;
                }
            }

            return true;
        }
        private void SaveNewProduct()
        {
            if (!TryValidateInputs(out string tenSp, out decimal donGia, out int soLuong, out DateTime? hanSuDung, out int maLoai, out int maDvt, out int? maNcc, out int? maNhanVien, out string hinhAnhPath))
            {
                return;
            }

            try
            {
                sanPhamTableAdapter.Insert(
                    tenSp,
                    donGia,
                    soLuong,
                    hanSuDung,
                    maLoai,
                    maDvt,
                    maNcc,
                    maNhanVien,
                    string.IsNullOrWhiteSpace(hinhAnhPath) ? null : NormalizeImagePath(hinhAnhPath));

                RefreshData();
                MessageBox.Show("Thêm sản phẩm thành công.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Không thể thêm sản phẩm. Lỗi: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                SetOperation(ProductOperation.None);
            }
        }

        private void SaveEditedProduct()
        {
            if (!int.TryParse(txtMaSP.Text, out int maSp))
            {
                MessageBox.Show("Không xác định được sản phẩm cần sửa.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!TryValidateInputs(out string tenSp, out decimal donGia, out int soLuong, out DateTime? hanSuDung, out int maLoai, out int maDvt, out int? maNcc, out int? maNhanVien, out string hinhAnhPath))
            {
                return;
            }

            var row = gStoreDataSet.SanPham.FindByMaSP(maSp);
            if (row == null)
            {
                MessageBox.Show("Không tìm thấy sản phẩm cần sửa.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                row.TenSP = tenSp;
                row.DonGia = donGia;
                row.SoLuong = soLuong;

                if (hanSuDung.HasValue)
                {
                    row.HanSuDung = hanSuDung.Value;
                }
                else
                {
                    row.SetHanSuDungNull();
                }

                row.MaLoai = maLoai;
                row.MaDVT = maDvt;

                if (maNcc.HasValue)
                {
                    row.MaNCC = maNcc.Value;
                }
                else
                {
                    row.SetMaNCCNull();
                }

                if (maNhanVien.HasValue)
                {
                    row.MaNhanVien = maNhanVien.Value;
                }
                else
                {
                    row.SetMaNhanVienNull();
                }

                if (string.IsNullOrWhiteSpace(hinhAnhPath))
                {
                    row.SetHinhAnhNull();
                }
                else
                {
                    row.HinhAnh = NormalizeImagePath(hinhAnhPath);
                }

                sanPhamTableAdapter.Update(row);
                RefreshData();
                MessageBox.Show("Cập nhật sản phẩm thành công.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Không thể cập nhật sản phẩm. Lỗi: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                SetOperation(ProductOperation.None);
            }
        }
        private void DeleteProduct()
        {
            if (!int.TryParse(txtMaSP.Text, out int maSp))
            {
                MessageBox.Show("Không xác định được sản phẩm cần xóa.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var row = gStoreDataSet.SanPham.FindByMaSP(maSp);
            if (row == null)
            {
                MessageBox.Show("Không tìm thấy sản phẩm cần xóa.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var confirm = MessageBox.Show("Bạn có chắc chắn muốn xóa sản phẩm này?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirm != DialogResult.Yes)
            {
                return;
            }

            try
            {
                row.Delete();
                sanPhamTableAdapter.Update(gStoreDataSet.SanPham);
                RefreshData();
                MessageBox.Show("Xóa sản phẩm thành công.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Không thể xóa sản phẩm. Lỗi: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                SetOperation(ProductOperation.None);
            }
        }

        private void RefreshData()
        {
            gStoreDataSet.SanPham.Clear();
            sanPhamTableAdapter.Fill(gStoreDataSet.SanPham);
            PopulateInputsFromSelection();
            dataGridView1.Refresh();
        }
        private void CommitUI()
        {
            Validate();
            try { dataGridView1.EndEdit(); } catch { }
            sanPhamBindingSource.EndEdit();   // nếu tên BindingSource khác, đổi lại
        }

        private bool AskConfirm(string msg, MessageBoxIcon icon = MessageBoxIcon.Question)
            => MessageBox.Show(msg, "Xác nhận", MessageBoxButtons.YesNo, icon) == DialogResult.Yes;
        private void btnThem_Click(object sender, EventArgs e)
        {
            CommitUI();

            // PHA 1: chuyển sang chế độ thêm
            if (_currentOperation != ProductOperation.Add)
            {
                if (!AskConfirm("Bạn muốn thêm sản phẩm mới?")) return;

                SetOperation(ProductOperation.Add);
                ClearInputFields();
                txtTenSP.Focus();
                return;
            }

            // PHA 2: LƯU
            if (!TryValidateInputs(out string tenSp, out decimal donGia, out int soLuong,
                                   out DateTime? hanSuDung, out int maLoai, out int maDvt,
                                   out int? maNcc, out int? maNhanVien, out string hinhAnhPath))
                return;

            if (!AskConfirm("Xác nhận lưu sản phẩm mới?")) return;

            try
            {
                sanPhamTableAdapter.Insert(
                    tenSp, donGia, soLuong, hanSuDung, maLoai, maDvt,
                    maNcc, maNhanVien,
                    string.IsNullOrWhiteSpace(hinhAnhPath) ? null : NormalizeImagePath(hinhAnhPath)
                );

                // nạp lại & focus dòng cuối (thường là bản ghi mới)
                RefreshData();
                sanPhamBindingSource.Position = Math.Max(0, sanPhamBindingSource.Count - 1);

                MessageBox.Show("Thêm sản phẩm thành công.", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không thể thêm sản phẩm.\n" + ex.Message, "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                SetOperation(ProductOperation.None);
            }
        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            CommitUI();

            var row = GetCurrentProductRow();
            if (row == null || row.RowState == DataRowState.Deleted)
            {
                MessageBox.Show("Vui lòng chọn sản phẩm cần sửa.", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // PHA 1: vào chế độ sửa
            if (_currentOperation != ProductOperation.Edit)
            {
                if (!AskConfirm("Bạn muốn sửa sản phẩm này?")) return;

                SetOperation(ProductOperation.Edit);
                PopulateInputs(row);
                txtTenSP.Focus();
                return;
            }

            // PHA 2: LƯU
            if (!int.TryParse(txtMaSP.Text, out int maSp))
            {
                MessageBox.Show("Không xác định được sản phẩm cần sửa.", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!TryValidateInputs(out string tenSp, out decimal donGia, out int soLuong,
                                   out DateTime? hanSuDung, out int maLoai, out int maDvt,
                                   out int? maNcc, out int? maNhanVien, out string hinhAnhPath))
                return;

            if (!AskConfirm("Xác nhận lưu thay đổi sản phẩm?")) return;

            var editRow = gStoreDataSet.SanPham.FindByMaSP(maSp);
            if (editRow == null)
            {
                MessageBox.Show("Không tìm thấy sản phẩm.", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                editRow.TenSP = tenSp;
                editRow.DonGia = donGia;
                editRow.SoLuong = soLuong;

                if (hanSuDung.HasValue) editRow.HanSuDung = hanSuDung.Value; else editRow.SetHanSuDungNull();
                editRow.MaLoai = maLoai;
                editRow.MaDVT = maDvt;
                if (maNcc.HasValue) editRow.MaNCC = maNcc.Value; else editRow.SetMaNCCNull();
                if (maNhanVien.HasValue) editRow.MaNhanVien = maNhanVien.Value; else editRow.SetMaNhanVienNull();
                if (string.IsNullOrWhiteSpace(hinhAnhPath)) editRow.SetHinhAnhNull(); else editRow.HinhAnh = NormalizeImagePath(hinhAnhPath);

                sanPhamBindingSource.EndEdit();
                sanPhamTableAdapter.Update(editRow);     // không Fill lại để giữ vị trí
                sanPhamBindingSource.ResetCurrentItem(); // refresh UI hàng hiện tại

                MessageBox.Show("Cập nhật sản phẩm thành công.", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không thể cập nhật sản phẩm.\n" + ex.Message, "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                SetOperation(ProductOperation.None);
            }
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            CommitUI();

            var row = GetCurrentProductRow();
            if (row == null || row.RowState == DataRowState.Deleted)
            {
                MessageBox.Show("Vui lòng chọn sản phẩm cần xóa.", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (!AskConfirm("Bạn có chắc chắn muốn xóa sản phẩm này?", MessageBoxIcon.Warning)) return;

            int desiredPos = Math.Max(0, sanPhamBindingSource.Position - 1);

            try
            {
                row.Delete();
                sanPhamBindingSource.EndEdit();
                sanPhamTableAdapter.Update(gStoreDataSet.SanPham);

                RefreshData();
                if (sanPhamBindingSource.Count > 0)
                    sanPhamBindingSource.Position = Math.Min(desiredPos, sanPhamBindingSource.Count - 1);

                MessageBox.Show("Xóa sản phẩm thành công.", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không thể xóa sản phẩm.\n" + ex.Message, "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnLuu_Click(object sender, EventArgs e)
        {
            switch (_currentOperation)
            {
                case ProductOperation.Add:
                    SaveNewProduct();
                    break;
                case ProductOperation.Edit:
                    SaveEditedProduct();
                    break;
                case ProductOperation.Delete:
                    DeleteProduct();
                    break;
                default:
                    MessageBox.Show("Vui lòng chọn chức năng Thêm, Sửa hoặc Xóa trước khi lưu.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    break;
            }
        }

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            if (_currentOperation == ProductOperation.Add)
            {
                return;
            }

            PopulateInputsFromSelection();
        }

        private void btnChonAnh_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                txtHinhAnh.Text = NormalizeImagePath(openFileDialog1.FileName);
                UpdateImagePreview();
            }
        }

        private void txtHinhAnh_TextChanged(object sender, EventArgs e)
        {
            if (_currentOperation == ProductOperation.None)
            {
                return;
            }

            UpdateImagePreview();
        }
        private void UpdateImagePreview()
        {
            string path = txtHinhAnh.Text.Trim();
            string resolvedPath = ResolveImagePath(path);

            Image previewImage = null;
            if (File.Exists(resolvedPath))
            {
                var cached = GetImageFromCache(path);
                if (cached != null)
                {
                    previewImage = (Image)cached.Clone();
                }
            }

            if (pictureBox1.Image != null)
            {
                var old = pictureBox1.Image;
                pictureBox1.Image = null;
                old.Dispose();
            }

            pictureBox1.Image = previewImage;
        }

        private string ResolveImagePath(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                return string.Empty;
            }

            string trimmed = path.Trim();
            if (Path.IsPathRooted(trimmed))
            {
                return trimmed;
            }

            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory ?? string.Empty;
            string combined = Path.Combine(baseDirectory, trimmed);

            if (File.Exists(combined))
            {
                return combined;
            }

            string imageDirectory = Path.Combine(baseDirectory, "Image");
            string imageCombined = Path.Combine(imageDirectory, trimmed);
            if (File.Exists(imageCombined))
            {
                return imageCombined;
            }

            return combined;
        }

        private string NormalizeImagePath(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                return string.Empty;
            }

            string resolved = ResolveImagePath(path);
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory ?? string.Empty;

            if (!string.IsNullOrEmpty(resolved) && resolved.StartsWith(baseDirectory, StringComparison.OrdinalIgnoreCase))
            {
                string relative = resolved.Substring(baseDirectory.Length).TrimStart(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
                return relative;
            }

            return path.Trim();
        }

        private Image GetImageFromCache(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                return null;
            }

            if (_imageCache.TryGetValue(path, out Image cached) && cached != null)
            {
                return cached;
            }

            string resolved = ResolveImagePath(path);
            if (!File.Exists(resolved))
            {
                return null;
            }

            try
            {
                using (var fs = new FileStream(resolved, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    using (var ms = new MemoryStream())
                    {
                        fs.CopyTo(ms);
                        ms.Position = 0;
                        cached = Image.FromStream(ms);
                    }
                }

                _imageCache[path] = cached;
                return cached;
            }
            catch
            {
                return null;
            }
        }

        private void dataGridView1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (dataGridView1.Columns[e.ColumnIndex] == HinhAnh)
            {
                string path = null;
                if (dataGridView1.Rows[e.RowIndex].DataBoundItem is DataRowView rowView)
                {
                    path = rowView["HinhAnh"] as string;
                }

                e.Value = GetImageFromCache(path);
                e.FormattingApplied = true;
            }
        }

        private void dataGridView1_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            if (dataGridView1.Columns[e.ColumnIndex] == HinhAnh)
            {
                e.ThrowException = false;
                e.Cancel = true;
            }
        }
        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            base.OnFormClosed(e);
            foreach (var image in _imageCache.Values)
            {
                image?.Dispose();
            }
            _imageCache.Clear();
            if (pictureBox1.Image != null)
            {
                pictureBox1.Image.Dispose();
                pictureBox1.Image = null;
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
        private void CancelPendingUiEdits()
        {
            try { Validate(); } catch { }
            try { dataGridView1.CancelEdit(); } catch { }
            try { sanPhamBindingSource.CancelEdit(); } catch { }

            // Nếu dòng hiện tại đã bị sửa trong DataSet thì rollback tại chỗ
            var row = GetCurrentProductRow();
            if (row != null && (row.RowState == DataRowState.Modified || row.RowState == DataRowState.Added))
            {
                row.RejectChanges();
            }
        }

        private void btnHuy_Click(object sender, EventArgs e)
        {
            // Nếu không ở chế độ thao tác thì thôi
            if (_currentOperation == ProductOperation.None)
            {
                PopulateInputsFromSelection();
                return;
            }

            // Xác nhận
            var confirm = MessageBox.Show(
                "Hủy các thay đổi đang thực hiện?",
                "Xác nhận",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (confirm != DialogResult.Yes) return;

            // Hủy mọi chỉnh sửa treo ở UI/BindingSource
            CancelPendingUiEdits();

            // Trả UI về trạng thái bình thường + nạp lại dữ liệu của dòng đang chọn
            SetOperation(ProductOperation.None);
            PopulateInputsFromSelection();
            UpdateImagePreview(); // đảm bảo ảnh hiển thị đúng
        }
    }
}
