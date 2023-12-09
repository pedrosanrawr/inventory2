using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace inventorymanagement3
{
    public partial class items : Form
    {
        private User currentUser;

        public items(User user)
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            inventory inventory = new inventory(currentUser);
            inventory.Show();
            this.Hide();
        }
    }
}
