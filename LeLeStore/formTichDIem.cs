using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LeLeStore
{
    public partial class formTichDIem : Form
    {
        private readonly GStoreDataSet _dataSet = new GStoreDataSet();
        private readonly GStoreDataSetTableAdapters.KhachHangTableAdapter _khachHangTableAdapter = new GStoreDataSetTableAdapters.KhachHangTableAdapter();
        private readonly BindingSource _bindingSource = new BindingSource();
        private readonly CultureInfo _currencyCulture = CultureInfo.GetCultureInfo("vi-VN");
        private decimal _currentDiscountAmount;
        public formTichDIem()
        {
            InitializeComponent();
            InitializeDataBinding();
        }
        private void InitializeDataBinding()
        {
            _bindingSource.DataSource = _dataSet;
            _bindingSource.DataMember = _dataSet.KhachHang.TableName;

            dataGridView1.AutoGenerateColumns = true;
            dataGridView1.DataSource = _bindingSource;

            numericUpDown1.Enabled = false;
            numericUpDown1.Minimum = 0;
            numericUpDown1.Maximum = int.MaxValue;
            btnUpdate.Enabled = false;

            txtChietKhau.ReadOnly = true;
            ResetDiscountDisplay();
        }

        private void formTichDIem_Load(object sender, EventArgs e)
        {
            try
            {
                _khachHangTableAdapter.Fill(_dataSet.KhachHang);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Không thể tải dữ liệu khách hàng. Lỗi: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            string phoneNumber = txtSearch.Text.Trim();

            if (string.IsNullOrWhiteSpace(phoneNumber))
            {
                MessageBox.Show("Vui lòng nhập số điện thoại để tìm kiếm.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                _bindingSource.RemoveFilter();
                numericUpDown1.Enabled = false;
                btnUpdate.Enabled = false;
                numericUpDown1.Value = numericUpDown1.Minimum;
                dataGridView1.ClearSelection();
                ResetDiscountDisplay();
                return;
            }

            if (_dataSet.KhachHang.Count == 0)
            {
                try
                {
                    _khachHangTableAdapter.Fill(_dataSet.KhachHang);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Không thể tải dữ liệu khách hàng. Lỗi: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            string escapedPhone = phoneNumber.Replace("'", "''");

            try
            {
                _bindingSource.Filter = $"SoDienThoai = '{escapedPhone}'";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Không thể tìm kiếm khách hàng. Lỗi: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (_bindingSource.Count > 0)
            {
                numericUpDown1.Enabled = true;
                btnUpdate.Enabled = true;
                _bindingSource.Position = 0;
                if (dataGridView1.Rows.Count > 0)
                {
                    dataGridView1.ClearSelection();
                    foreach (DataGridViewRow row in dataGridView1.Rows)
                    {
                        if (row.Visible)
                        {
                            row.Selected = true;
                            dataGridView1.CurrentCell = row.Cells[0];
                            break;
                        }
                    }
                }
                UpdateNumericUpDownWithCurrentCustomer();
            }
            else
            {
                numericUpDown1.Enabled = false;
                btnUpdate.Enabled = false;
                numericUpDown1.Value = numericUpDown1.Minimum;
                ResetDiscountDisplay();
                MessageBox.Show("Không tìm thấy khách hàng với số điện thoại đã nhập.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        private void UpdateNumericUpDownWithCurrentCustomer()
        {
            if (!numericUpDown1.Enabled)
            {
                return;
            }

            if (_bindingSource.Current is DataRowView currentView && currentView.Row is GStoreDataSet.KhachHangRow row)
            {
                var points = row.IsNull(row.Table.DiemTichLuyColumn) ? 0 : row.DiemTichLuy;
                var value = Math.Max(numericUpDown1.Minimum, Math.Min(numericUpDown1.Maximum, points));
                numericUpDown1.Value = value;
                UpdateDiscountDisplay(points);
            }
        }

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            if (!numericUpDown1.Enabled)
            {
                return;
            }

            UpdateNumericUpDownWithCurrentCustomer();
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (!numericUpDown1.Enabled || _bindingSource.Count == 0)
            {
                MessageBox.Show("Vui lòng tìm kiếm và chọn khách hàng trước khi cập nhật.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (!(_bindingSource.Current is DataRowView currentView) || !(currentView.Row is GStoreDataSet.KhachHangRow row))
            {
                MessageBox.Show("Không thể xác định khách hàng được chọn.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var newPoints = (int)numericUpDown1.Value;

            try
            {
                row.DiemTichLuy = newPoints;
                _bindingSource.EndEdit();

                var rowsAffected = _khachHangTableAdapter.Update(row);
                if (rowsAffected > 0)
                {
                    MessageBox.Show("Cập nhật điểm tích lũy thành công.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    _bindingSource.ResetCurrentItem();
                    UpdateNumericUpDownWithCurrentCustomer();
                }
                else
                {
                    MessageBox.Show("Không có thay đổi nào được lưu.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                row.RejectChanges();
                MessageBox.Show($"Không thể cập nhật điểm tích lũy. Lỗi: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void UpdateDiscountDisplay(int points)
        {
            _currentDiscountAmount = CalculateDiscountAmount(points);
            txtChietKhau.Text = FormatCurrency(_currentDiscountAmount);
            btnChietKhau.Enabled = _bindingSource.Count > 0 && _currentDiscountAmount > 0;
        }

        private void ResetDiscountDisplay()
        {
            _currentDiscountAmount = 0m;
            txtChietKhau.Text = FormatCurrency(_currentDiscountAmount);
            btnChietKhau.Enabled = false;
        }

        private decimal CalculateDiscountAmount(int points)
        {
            if (points >= 20)
            {
                return 11000m;
            }

            if (points >= 15)
            {
                return 9000m;
            }

            if (points >= 10)
            {
                return 7000m;
            }

            if (points >= 5)
            {
                return 5000m;
            }

            return 0m;
        }

        private string FormatCurrency(decimal amount)
        {
            return amount.ToString("N0", _currencyCulture) + " ₫";
        }

        private void btnChietKhau_Click(object sender, EventArgs e)
        {
            if (_bindingSource.Count == 0 || !(_bindingSource.Current is DataRowView currentView) || !(currentView.Row is GStoreDataSet.KhachHangRow))
            {
                MessageBox.Show("Vui lòng chọn khách hàng trước khi áp dụng chiết khấu.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (_currentDiscountAmount <= 0)
            {
                MessageBox.Show("Khách hàng chưa đủ điểm tích lũy để nhận khuyến mãi.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (Owner is formPayMent paymentForm)
            {
                paymentForm.ApplyLoyaltyDiscount(_currentDiscountAmount);
                MessageBox.Show($"Đã áp dụng chiết khấu {FormatCurrency(_currentDiscountAmount)} vào hóa đơn.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                DialogResult = DialogResult.OK;
                Close();
            }
            else
            {
                MessageBox.Show("Không tìm thấy hóa đơn để áp dụng chiết khấu.", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}
