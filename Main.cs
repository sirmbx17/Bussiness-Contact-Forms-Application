using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Forms
{
    public partial class Main : Form
    {
        public Main()
        {
            InitializeComponent();
        }

        private void businessToolStripMenuItem_Click(object sender, EventArgs e)
        {
            BizContacts frm = new BizContacts();// create neww form
            frm.MdiParent = this;// set form as parent of all other forms.
            frm.Show(); //display form
        }

        private void cascadeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.Cascade); // cascade child form inside main form
        }

        private void tileVerticalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.TileVertical); // TileVertical child form inside main form
        }

        private void tileHotizontalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.TileHorizontal); // TileHorizontal child form inside main form
        }
    }
}
