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
    public partial class formUpdateClient : Form
    {
        private enum ClientOperation
        {
            None,
            Add,
            Edit,
            Delete
        }

        private ClientOperation _currentOperation = ClientOperation.None;
        public formUpdateClient()
        {
            InitializeComponent();
            SetOperation(ClientOperation.None);
            PopulateInputsFromSelection();
        }

        private void formUpdateClient_Load(object sender, EventArgs e)
        {
            // TODO: This line of code loads data into the 'gStoreDataSet.KhachHang' table. You can move, or remove it, as needed.
            this.khachHangTableAdapter.Fill(this.gStoreDataSet.KhachHang);
            PopulateInputsFromSelection();

        }
        private void SetOperation(ClientOperation operation)
        {
            _currentOperation = operation;

            bool canEditFields = operation == ClientOperation.Add || operation == ClientOperation.Edit;
            bool isDelete = operation == ClientOperation.Delete;

            txtMaKH.ReadOnly = true;
            txtTenKH.ReadOnly = !canEditFields;
            txtSDTKH.ReadOnly = !canEditFields;
            txtDiaChiKH.ReadOnly = !canEditFields;
            txtMaNVKH.ReadOnly = !canEditFields;

            // Điểm tích lũy chỉ cho phép nhập khi thêm mới
            txtDiem.ReadOnly = operation != ClientOperation.Add;

            if (isDelete)
            {
                txtTenKH.ReadOnly = true;
                txtSDTKH.ReadOnly = true;
                txtDiaChiKH.ReadOnly = true;
                txtMaNVKH.ReadOnly = true;
                txtDiem.ReadOnly = true;
            }
        }

        private void PopulateInputsFromSelection()
        {
            var row = GetCurrentClientRow();
            if (row != null)
            {
                PopulateInputs(row);
            }
            else
            {
                ClearInputFields();
            }
        }

        private void PopulateInputs(GStoreDataSet.KhachHangRow row)
        {
            txtMaKH.Text = row.MaKhachHang.ToString();
            txtTenKH.Text = row.IsNull("HoTen") ? string.Empty : row.HoTen;
            txtSDTKH.Text = row.IsNull("SoDienThoai") ? string.Empty : row.SoDienThoai;
            txtDiaChiKH.Text = row.IsNull("DiaChi") ? string.Empty : row.DiaChi;
            txtDiem.Text = row.DiemTichLuy.ToString();
            txtMaNVKH.Text = row.IsNull("MaNhanVien") ? string.Empty : row.MaNhanVien.ToString();
        }

        private void ClearInputFields()
        {
            txtMaKH.Text = string.Empty;
            txtTenKH.Text = string.Empty;
            txtSDTKH.Text = string.Empty;
            txtDiaChiKH.Text = string.Empty;
            txtDiem.Text = string.Empty;
            txtMaNVKH.Text = string.Empty;
        }

        private GStoreDataSet.KhachHangRow GetCurrentClientRow()
        {
            if (dataGridView1.CurrentRow?.DataBoundItem is DataRowView rowView)
            {
                return rowView.Row as GStoreDataSet.KhachHangRow;
            }

            return null;
        }

        private bool TryValidateInputsForAdd(out string hoTen, out string soDienThoai, out string diaChi, out int diemTichLuy, out int maNhanVien)
        {
            hoTen = txtTenKH.Text.Trim();
            soDienThoai = txtSDTKH.Text.Trim();
            diaChi = txtDiaChiKH.Text.Trim();
            diemTichLuy = 0;
            maNhanVien = 0;

            if (string.IsNullOrWhiteSpace(hoTen))
            {
                MessageBox.Show("Tên khách hàng không được để trống.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtTenKH.Focus();
                return false;
            }

            if (!int.TryParse(txtDiem.Text.Trim(), out diemTichLuy))
            {
                MessageBox.Show("Điểm tích lũy phải là số nguyên.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtDiem.Focus();
                return false;
            }

            if (diemTichLuy < 0)
            {
                MessageBox.Show("Điểm tích lũy không được âm.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtDiem.Focus();
                return false;
            }

            if (!int.TryParse(txtMaNVKH.Text.Trim(), out maNhanVien))
            {
                MessageBox.Show("Mã nhân viên phải là số.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtMaNVKH.Focus();
                return false;
            }

            return true;
        }

        private bool TryValidateInputsForEdit(out string hoTen, out string soDienThoai, out string diaChi, out int maNhanVien)
        {
            hoTen = txtTenKH.Text.Trim();
            soDienThoai = txtSDTKH.Text.Trim();
            diaChi = txtDiaChiKH.Text.Trim();
            maNhanVien = 0;

            if (string.IsNullOrWhiteSpace(hoTen))
            {
                MessageBox.Show("Tên khách hàng không được để trống.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtTenKH.Focus();
                return false;
            }

            if (!int.TryParse(txtMaNVKH.Text.Trim(), out maNhanVien))
            {
                MessageBox.Show("Mã nhân viên phải là số.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtMaNVKH.Focus();
                return false;
            }

            return true;
        }

        private void SaveNewClient()
        {
            if (!TryValidateInputsForAdd(out string hoTen, out string soDienThoai, out string diaChi, out int diemTichLuy, out int maNhanVien))
            {
                return;
            }

            try
            {
                khachHangTableAdapter.Insert(hoTen,
                    string.IsNullOrWhiteSpace(soDienThoai) ? null : soDienThoai,
                    string.IsNullOrWhiteSpace(diaChi) ? null : diaChi,
                    diemTichLuy,
                    maNhanVien);
                RefreshData();
                MessageBox.Show("Thêm khách hàng thành công.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Không thể thêm khách hàng. Lỗi: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                SetOperation(ClientOperation.None);
            }
        }

        private void SaveEditedClient()
        {
            if (!int.TryParse(txtMaKH.Text, out int maKhachHang))
            {
                MessageBox.Show("Không xác định được khách hàng cần sửa.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!TryValidateInputsForEdit(out string hoTen, out string soDienThoai, out string diaChi, out int maNhanVien))
            {
                return;
            }

            var row = gStoreDataSet.KhachHang.FindByMaKhachHang(maKhachHang);
            if (row == null)
            {
                MessageBox.Show("Không tìm thấy khách hàng cần sửa.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                row.HoTen = hoTen;

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

                khachHangTableAdapter.Update(row);
                RefreshData();
                MessageBox.Show("Cập nhật khách hàng thành công.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Không thể cập nhật khách hàng. Lỗi: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                SetOperation(ClientOperation.None);
            }
        }

        private void DeleteClient()
        {
            if (!int.TryParse(txtMaKH.Text, out int maKhachHang))
            {
                MessageBox.Show("Không xác định được khách hàng cần xóa.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var row = gStoreDataSet.KhachHang.FindByMaKhachHang(maKhachHang);
            if (row == null)
            {
                MessageBox.Show("Không tìm thấy khách hàng cần xóa.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var confirm = MessageBox.Show("Bạn có chắc chắn muốn xóa khách hàng này?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirm != DialogResult.Yes)
            {
                return;
            }

            try
            {
                row.Delete();
                khachHangTableAdapter.Update(gStoreDataSet.KhachHang);
                RefreshData();
                MessageBox.Show("Xóa khách hàng thành công.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Không thể xóa khách hàng. Lỗi: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                SetOperation(ClientOperation.None);
            }
        }

        private void RefreshData()
        {
            gStoreDataSet.KhachHang.Clear();
            khachHangTableAdapter.Fill(gStoreDataSet.KhachHang);
            PopulateInputsFromSelection();
        }

        private void btnThem_Click(object sender, EventArgs e)
        {
            SetOperation(ClientOperation.Add);
            ClearInputFields();
            txtTenKH.Focus();
        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            var row = GetCurrentClientRow();
            if (row == null)
            {
                MessageBox.Show("Vui lòng chọn khách hàng cần sửa.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            SetOperation(ClientOperation.Edit);
            PopulateInputs(row);
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            var row = GetCurrentClientRow();
            if (row == null)
            {
                MessageBox.Show("Vui lòng chọn khách hàng cần xóa.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            SetOperation(ClientOperation.Delete);
            PopulateInputs(row);
        }

        private void btnLuu_Click(object sender, EventArgs e)
        {
            switch (_currentOperation)
            {
                case ClientOperation.Add:
                    SaveNewClient();
                    break;
                case ClientOperation.Edit:
                    SaveEditedClient();
                    break;
                case ClientOperation.Delete:
                    DeleteClient();
                    break;
                default:
                    MessageBox.Show("Vui lòng chọn chức năng Thêm, Sửa hoặc Xóa trước khi lưu.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    break;
            }
        }

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            if (_currentOperation == ClientOperation.Add)
            {
                return;
            }

            PopulateInputsFromSelection();
        }
    }
}
