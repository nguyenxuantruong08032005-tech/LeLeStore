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
    public partial class formTichDIem : Form
    {
        private readonly GStoreDataSet _dataSet = new GStoreDataSet();
        private readonly GStoreDataSetTableAdapters.KhachHangTableAdapter _khachHangTableAdapter = new GStoreDataSetTableAdapters.KhachHangTableAdapter();
        private readonly BindingSource _bindingSource = new BindingSource();

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
                dataGridView1.ClearSelection();
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
            }
            else
            {
                numericUpDown1.Enabled = false;
                MessageBox.Show("Không tìm thấy khách hàng với số điện thoại đã nhập.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }
}
