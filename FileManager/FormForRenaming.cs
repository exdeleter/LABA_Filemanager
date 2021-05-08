using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FileManager
{
    public partial class FormForRenaming : Form
    {
        public FormForRenaming()
        {
            InitializeComponent();
        }

        private void FinallyRename(object sender, EventArgs e)
        {
            FileComm fr = new FileComm();
            fr.TakeString(textBox1.Text);
            this.Close();
        }
    }
}
