using System;
using System.IO;
using System.Windows.Forms;

namespace FileManager
{
    public partial class DefaultDirectories : Form
    {
        public DefaultDirectories()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if(Directory.Exists(textBox1.Text))
            {
                using (StreamWriter sw = File.CreateText("defaultdirectories.txt"))
                {
                    sw.WriteLine(textBox1.Text);
                    this.Close();
                }
            }
        }
    }
}
