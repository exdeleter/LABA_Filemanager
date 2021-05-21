using System;
using System.Windows.Forms;
using Ionic.Zip;

namespace FileManager
{
    public partial class FormForRenaming : Form
    {
        private ZipFile zip;
        private string path;
        public FormForRenaming()
        {
            InitializeComponent();
        }
        public FormForRenaming(ZipFile zip, string path)
        {

            InitializeComponent();
            this.label1.Text = "Введите название архива";
            button1.Visible = false;
            button2.Visible = true;
            this.zip = zip;
            path = path.Substring(0, path.LastIndexOf("\\")+2);
            this.path = path;

        }
        private void FinallyRename(object sender, EventArgs e)
        {
            FileComm fr = new FileComm();
            fr.TakeNewName(textBox1.Text);
            this.Close();
        }

        private void FinallZip(object sender, EventArgs e)
        {
            try
            {
                this.zip.Save(path+textBox1.Text + ".zip");
            }
            catch
            {
                MessageBox.Show("Что - то пошло не так");
                return;
            }
            this.Close();
        }
    }
}
