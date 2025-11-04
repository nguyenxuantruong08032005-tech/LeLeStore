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
    public partial class formDashBoard : Form
    {
        public formDashBoard()
        {
            InitializeComponent();
        }

        private void formDashBoard_Load(object sender, EventArgs e)
        {
            this.ControlBox = false;
        }
    }
}
