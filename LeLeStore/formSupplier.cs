using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LeLeStore
{
    public partial class formSupplier : Form
    {
        private enum SupplierOperation
        {
            None,
            Add,
            Edit,
            Delete
        }

        private SupplierOperation _currentOperation = SupplierOperation.None;
        public formSupplier()
        {
            InitializeComponent();
            SetOperation(SupplierOperation.None);
            PopulateInputsFromSelection();
        }

        private void formSupplier_Load(object sender, EventArgs e)
        {
            // TODO: This line of code loads data into the 'gStoreDataSet.NhaCungCap' table. You can move, or remove it, as needed.
            this.nhaCungCapTableAdapter.Fill(this.gStoreDataSet.NhaCungCap);

        }
        private void SetOperation(SupplierOperation operation)
        {
            _currentOperation = operation;

            bool canEditFields = operation == SupplierOperation.Add || operation == SupplierOperation.Edit;
            bool isDelete = operation == SupplierOperation.Delete;

            txtMaNCC.ReadOnly = true;
            txtTenNCC.ReadOnly = !canEditFields;
            txtSdtNCC.ReadOnly = !canEditFields;
            txtDiaChiNCC.ReadOnly = !canEditFields;
            txtManv.ReadOnly = !canEditFields;

            if (isDelete)
            {
                txtTenNCC.ReadOnly = true;
                txtSdtNCC.ReadOnly = true;
                txtDiaChiNCC.ReadOnly = true;
                txtManv.ReadOnly = true;
            }
        }

        private void PopulateInputsFromSelection()
        {
            var row = GetCurrentSupplierRow();
            if (row != null)
            {
                PopulateInputs(row);
            }
            else
            {
                ClearInputFields();
            }
        }

        private void PopulateInputs(GStoreDataSet.NhaCungCapRow row)
        {
            txtMaNCC.Text = row.MaNCC.ToString();
            txtTenNCC.Text = row.IsNull("TenNCC") ? string.Empty : row.TenNCC;

            txtSdtNCC.Text = row.IsNull("SoDienThoai") ? string.Empty : row.SoDienThoai;
            txtDiaChiNCC.Text = row.IsNull("DiaChi") ? string.Empty : row.DiaChi;
            txtManv.Text = row.MaNhanVien.ToString();
        }

        private void ClearInputFields()
        {
            txtMaNCC.Text = string.Empty;
            txtTenNCC.Text = string.Empty;
            txtSdtNCC.Text = string.Empty;
            txtDiaChiNCC.Text = string.Empty;
            txtManv.Text = string.Empty;
        }

        private GStoreDataSet.NhaCungCapRow GetCurrentSupplierRow()
        {
            if (dataGridView1.CurrentRow?.DataBoundItem is DataRowView rowView)
            {
                return rowView.Row as GStoreDataSet.NhaCungCapRow;
            }

            return null;
        }

        private bool TryValidateInputs(out string tenNcc, out string soDienThoai, out string diaChi, out int maNhanVien)
        {
            tenNcc = txtTenNCC.Text.Trim();
            soDienThoai = txtSdtNCC.Text.Trim();
            diaChi = txtDiaChiNCC.Text.Trim();
            maNhanVien = 0;

            if (string.IsNullOrWhiteSpace(tenNcc))
            {
                MessageBox.Show("Tên nhà cung cấp không được để trống.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtTenNCC.Focus();
                return false;
            }

            if (!int.TryParse(txtManv.Text.Trim(), out maNhanVien))
            {
                MessageBox.Show("Mã nhân viên phải là số.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtManv.Focus();
                return false;
            }

            return true;
        }

        private void SaveNewSupplier()
        {
            if (!TryValidateInputs(out string tenNcc, out string soDienThoai, out string diaChi, out int maNhanVien))
            {
                return;
            }

            try
            {
                nhaCungCapTableAdapter.Insert(tenNcc, string.IsNullOrWhiteSpace(soDienThoai) ? null : soDienThoai, string.IsNullOrWhiteSpace(diaChi) ? null : diaChi, maNhanVien);
                RefreshData();
                MessageBox.Show("Thêm nhà cung cấp thành công.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Không thể thêm nhà cung cấp. Lỗi: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                SetOperation(SupplierOperation.None);
            }
        }

        private void SaveEditedSupplier()
        {
            if (!int.TryParse(txtMaNCC.Text, out int maNcc))
            {
                MessageBox.Show("Không xác định được nhà cung cấp cần sửa.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!TryValidateInputs(out string tenNcc, out string soDienThoai, out string diaChi, out int maNhanVien))
            {
                return;
            }

            var row = gStoreDataSet.NhaCungCap.FindByMaNCC(maNcc);
            if (row == null)
            {
                MessageBox.Show("Không tìm thấy nhà cung cấp cần sửa.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                row.TenNCC = tenNcc;
                if (string.IsNullOrWhiteSpace(soDienThoai))
                {
                    row.SetSoDienThoaiNull();
                }
                else
                {
                    row.SoDienThoai = soDienThoai;
                }

                if (string.IsNullOrWhiteSpace(diaChi))
                {
                    row.SetDiaChiNull();
                }
                else
                {
                    row.DiaChi = diaChi;
                }

                row.MaNhanVien = maNhanVien;

                nhaCungCapTableAdapter.Update(row);
                RefreshData();
                MessageBox.Show("Cập nhật nhà cung cấp thành công.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Không thể cập nhật nhà cung cấp. Lỗi: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                SetOperation(SupplierOperation.None);
            }
        }

        private void DeleteSupplier()
        {
            if (!int.TryParse(txtMaNCC.Text, out int maNcc))
            {
                MessageBox.Show("Không xác định được nhà cung cấp cần xóa.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var row = gStoreDataSet.NhaCungCap.FindByMaNCC(maNcc);
            if (row == null)
            {
                MessageBox.Show("Không tìm thấy nhà cung cấp cần xóa.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var confirm = MessageBox.Show("Bạn có chắc chắn muốn xóa nhà cung cấp này?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirm != DialogResult.Yes)
            {
                return;
            }

            try
            {
                row.Delete();
                nhaCungCapTableAdapter.Update(gStoreDataSet.NhaCungCap);
                RefreshData();
                MessageBox.Show("Xóa nhà cung cấp thành công.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Không thể xóa nhà cung cấp. Lỗi: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                SetOperation(SupplierOperation.None);
            }
        }

        private void RefreshData()
        {
            gStoreDataSet.NhaCungCap.Clear();
            nhaCungCapTableAdapter.Fill(gStoreDataSet.NhaCungCap);
            PopulateInputsFromSelection();
        }

        private void btnThem_Click(object sender, EventArgs e)
        {
            SetOperation(SupplierOperation.Add);
            ClearInputFields();
            txtMaNCC.Text = string.Empty;
            txtTenNCC.Focus();
        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            var row = GetCurrentSupplierRow();
            if (row == null)
            {
                MessageBox.Show("Vui lòng chọn nhà cung cấp cần sửa.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            SetOperation(SupplierOperation.Edit);
            PopulateInputs(row);
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            var row = GetCurrentSupplierRow();
            if (row == null)
            {
                MessageBox.Show("Vui lòng chọn nhà cung cấp cần xóa.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            SetOperation(SupplierOperation.Delete);
            PopulateInputs(row);
        }

        private void btnLuu_Click(object sender, EventArgs e)
        {
            switch (_currentOperation)
            {
                case SupplierOperation.Add:
                    SaveNewSupplier();
                    break;
                case SupplierOperation.Edit:
                    SaveEditedSupplier();
                    break;
                case SupplierOperation.Delete:
                    DeleteSupplier();
                    break;
                default:
                    MessageBox.Show("Vui lòng chọn chức năng Thêm, Sửa hoặc Xóa trước khi lưu.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    break;
            }
        }

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            if (_currentOperation == SupplierOperation.Add)
            {
                return;
            }

            PopulateInputsFromSelection();
        }
    }
}
