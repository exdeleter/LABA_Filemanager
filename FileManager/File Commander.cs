using System;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Ionic.Zip;

//Окно состоит из двух частей (аналог TotalComander), в каждой части отображается список файлов и каталогов выбранной директории. Навигация по каталогам в каждой половине происходить независимо от другой половины. 
//Для двух частей есть общая панель инструментов: создать файл, создать директорию, удалить, скопировать, переименовать. В каждой части наверху отображается адресная строка, содержащая полный путь к текущей директории.
//При двойном щелчке на имени каталога, происходит открытие содержимого этого каталога. При двойном щелчке на имени файла, происходит открытие этого файла (???).
//Предусмотреть контекстное меню: создать, удалить, переименовать, копировать, просмотреть свойства файла, сжать файл в архив (???).
//Предусмотреть возможность персональной настройки программы: цветовая схема, размер отображения шрифта, директория по умолчанию.
//Для разворачивания программы на компьютере использовать возможность ClickOnce.
//Особенности:
//Копирование.Разрешить копировать один файл, группу выделенных файлов. При копировании директории копировать ее вместе со всем содержимым. Копирование происходит в директорию, активную во второй половине окна.
//Удаление. При удалении директории удалять все ее содержимое. Перед удалением уточнить, действительно ли надо удалять.
//Копирование, удаление и сжатие в архив реализовать в отдельном потоке, чтобы пока идет процесс копирования/удаления/сжатия программа реагировала на действия пользователя.


namespace FileManager
{
    public partial class FileComm : Form
    {
        static ImageList imageList1 = new ImageList();
        static string newname;
        private int sizeIcons = 32;
        public int SizeIcons
        {
            get
            {
                return sizeIcons;
            }
            set
            {
                sizeIcons = value;
            }

        }
        public FileComm()
        {
            InitializeComponent();
            Initialization();

        }
        private void Initialization()
        {
            FillPic();
            //treeView1.BeforeSelect += treeView1_BeforeSelect;
            TreeDirectories.BeforeExpand += treeView1_BeforeExpand;
            // заполняем дерево дисками
            FillDriveNodes();
            FillComboBox();
        }
        private void FillPic()
        {
            Bitmap bmp = new Bitmap("Resources\\folder.ico");
            imageList1.Images.Add(bmp);
            Bitmap bmp1 = new Bitmap("Resources\\pdf.ico");
            imageList1.Images.Add(bmp1);
            Bitmap bmp2 = new Bitmap("Resources\\disk.ico");
            imageList1.Images.Add(bmp2);
            Bitmap bmp3 = new Bitmap("Resources\\excel.ico");
            imageList1.Images.Add(bmp3);
            Bitmap bmp4 = new Bitmap("Resources\\word.ico");
            imageList1.Images.Add(bmp4);
            Bitmap bmp5 = new Bitmap("Resources\\image.ico");
            imageList1.Images.Add(bmp5);
            Bitmap bmp6 = new Bitmap("Resources\\json.ico");
            imageList1.Images.Add(bmp6);
            Bitmap bmp7 = new Bitmap("Resources\\question.ico");
            imageList1.Images.Add(bmp7);
            imageList1.ImageSize = new Size(this.SizeIcons, this.SizeIcons);
            TreeDirectories.ImageList = imageList1;
            ScreenFile.LargeImageList = imageList1;
        }
        private void FillDriveNodes()
        {
            try
            {

                foreach (DriveInfo drive in DriveInfo.GetDrives())
                {
                    TreeNode driveNode = new TreeNode { Text = drive.Name };
                    driveNode.ImageIndex = 2;
                    FillTreeNode(driveNode, drive.Name);
                    TreeDirectories.Nodes.Add(driveNode);
                }
            }
            catch (Exception ex) { }
        }
        private void FillComboBox()
        {
            foreach (DriveInfo drive in DriveInfo.GetDrives())
            {
                Disks.Items.Add(drive.Name);

            }
        }
        private void FillTreeNode(TreeNode driveNode, string path)
        {
            try
            {

                string[] dirs = Directory.GetDirectories(path);
                foreach (string dir in dirs)
                {
                    TreeNode dirNode = new TreeNode();
                    dirNode.Text = dir.Remove(0, dir.LastIndexOf("\\") + 1);
                    dirNode.ImageIndex = 0;
                    driveNode.Nodes.Add(dirNode);
                }
                string[] files = Directory.GetFiles(path);
                foreach (string fl in files)
                {
                    TreeNode dirNode = new TreeNode();
                    dirNode.Text = fl.Remove(0, fl.LastIndexOf("\\") + 1);
                    dirNode.ImageIndex = 1;
                    driveNode.Nodes.Add(dirNode);
                }


            }
            catch (Exception ex) { }
        }
        private int ExtensionsFile(string file)
        {
            string ext = Path.GetExtension(file);
            if (ext == ".pdf")
            {
                return 1;
            }
            else if (ext == ".png" || ext == ".JPG" || ext == ".mp4" || ext == ".jpg" || ext == ".MOV")
            {
                return 5;
            }
            else if (ext == ".xls" || ext == ".csv" || ext == ".xlsx")
            {
                return 3;
            }
            else if (ext == ".doc" || ext == ".docx" || ext == ".txt" || ext == ".rtf")
            {
                return 4;
            }
            else if (ext == ".json")
            {
                return 6;
            }
            else
            {
                return 7;
            }
        }
        private void treeView1_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            e.Node.Nodes.Clear();
            string[] dirs;
            try
            {
                if (Directory.Exists(e.Node.FullPath))
                {
                    Disks.SelectedIndex = 0;
                    dirs = Directory.GetDirectories(e.Node.FullPath);
                    if (dirs.Length != 0)
                    {
                        for (int i = 0; i < dirs.Length; i++)
                        {
                            TreeNode dirNode = new TreeNode(new DirectoryInfo(dirs[i]).Name);
                            FillTreeNode(dirNode, dirs[i]);
                            e.Node.Nodes.Add(dirNode);
                        }
                    }
                    string[] files = Directory.GetFiles(e.Node.FullPath);
                    ScreenFile.Items.Clear();
                    foreach (string file in files)
                    {

                        ListViewItem lvi = new ListViewItem();
                        lvi.Text = file.Remove(0, file.LastIndexOf('\\') + 1);
                        lvi.ImageIndex = ExtensionsFile(file);
                        ScreenFile.Items.Add(lvi);
                    }
                    PathTree.Text = e.Node.FullPath;
                    AddresLine.Text = e.Node.FullPath;
                }
            }
            catch (Exception ex) { }
        }
        private void OpenSelectedFiles(object sender, EventArgs e)
        {
            try
            {
                Process.Start(AddresLine.Text);

            }
            catch (Exception ex)
            {
                MessageBox.Show("Выберите папку, из которой вы хотите открыть файл", "Открытие файла", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        private void CreateDirectory(object sender, EventArgs e)
        {

            string path = TreeDirectories.SelectedNode.FullPath + @"\Новая папка";
            if (Directory.Exists(path))
            {
                MessageBox.Show("Эта папка уже существует", "Создание папки", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                Directory.CreateDirectory(path);
                TreeDirectories.SelectedNode.Nodes.Add("Новая папка");

            }
        }

        private void OpenProperty(object sender, EventArgs e)
        {
            PropertyOfFile pr = new PropertyOfFile(AddresLine.Text);
            pr.Show();
        }

        private void RenameFile(object sender, EventArgs e)
        {
            string path = TreeDirectories.SelectedNode.FullPath;
            DirectoryInfo directory = new DirectoryInfo(path);
            FormForRenaming fr = new FormForRenaming();
            fr.ShowDialog();
            string newpath = TreeDirectories.SelectedNode.FullPath.Substring(0, path.Length - directory.Name.Length) + newname;
            Directory.Move(TreeDirectories.SelectedNode.FullPath, newpath);
            newname = String.Empty;
        }

        public void TakeNewName(string a)
        {
            newname = a;
        }

        private void Navigate(object sender, EventArgs e)
        {
            try
            {
                if (Directory.Exists(PathTree.Text))
                {
                    ScreenFile.Items.Clear();
                    string[] files = Directory.GetFiles(PathTree.Text);
                    // перебор полученных файлов
                    foreach (string file in files)
                    {

                        ListViewItem lvi = new ListViewItem();
                        lvi.Text = file.Remove(0, file.LastIndexOf('\\') + 1);
                        // установка названия файла
                        lvi.ImageIndex = ExtensionsFile(file);
                        ScreenFile.Items.Add(lvi);
                    }
                }
            }
            catch (Exception ex) { }
        }
        private void DeleteDirectory(object sender, EventArgs e)
        {
            try
            {
                DialogResult result = MessageBox.Show("Вы уверены, что хотите удалить папку?", "Удаление папки", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    string path = TreeDirectories.SelectedNode.FullPath;
                    Directory.Delete(path, true);
                    TreeDirectories.SelectedNode.Remove();

                }
                else
                {
                    return;
                }


            }
            catch (DirectoryNotFoundException ex)
            {
                MessageBox.Show("Директория не найдена. Ошибка: " + ex.Message, "Ошибка при удалении", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
            catch (UnauthorizedAccessException ex)
            {
                MessageBox.Show("Отсутствует доступ. Ошибка: " + ex.Message, "Ошибка при удалении", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
            catch (Exception ex)
            {
                MessageBox.Show("Произошла ошибка: " + ex.Message, "Ошибка при удалении", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void FocusFile(object sender, EventArgs e)
        {
            AddresLine.Text = TreeDirectories.SelectedNode.FullPath + "\\" + ScreenFile.FocusedItem.Text;
        }

        private void SizeChange(object sender, EventArgs e)
        {
            TreeDirectories.Font = new Font(TreeDirectories.Font.FontFamily, int.Parse(toolStripComboBox1.SelectedItem.ToString()));
            ScreenFile.Font = new Font(ScreenFile.Font.FontFamily, int.Parse(toolStripComboBox1.SelectedItem.ToString()));

            //Bitmap bmp = new Bitmap("Resources\\folder.ico");
            //imageList1.Images.Add(bmp);
            //Bitmap bmp1 = new Bitmap("Resources\\pdf.ico");
            //imageList1.Images.Add(bmp1);
            //Bitmap bmp2 = new Bitmap("Resources\\disk.ico");
            //imageList1.Images.Add(bmp2);
            //Bitmap bmp3 = new Bitmap("Resources\\excel.ico");
            //imageList1.Images.Add(bmp3);
            //Bitmap bmp4 = new Bitmap("Resources\\word.ico");
            //imageList1.Images.Add(bmp4);
            //Bitmap bmp5 = new Bitmap("Resources\\image.ico");
            //imageList1.Images.Add(bmp5);
            //Bitmap bmp6 = new Bitmap("Resources\\json.ico");
            //imageList1.Images.Add(bmp6);
            //Bitmap bmp7 = new Bitmap("Resources\\question.ico");
            //imageList1.Images.Add(bmp7);
            sizeIcons = int.Parse(toolStripComboBox1.SelectedItem.ToString());
            imageList1.ImageSize = new Size(sizeIcons, sizeIcons);

            TreeDirectories.ImageList = imageList1;
            ScreenFile.LargeImageList = imageList1;

        }

        private void черныйToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TreeDirectories.BackColor = Color.Black;
            TreeDirectories.ForeColor = Color.White;
            ScreenFile.BackColor = Color.Black;
            ScreenFile.ForeColor = Color.White;
            this.ForeColor = Color.White;
            this.BackColor = Color.Black;
            menuStrip1.BackColor = Color.Black;
            menuStrip1.ForeColor = Color.White;
        }

        private void белыйToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TreeDirectories.BackColor = Color.White;
            TreeDirectories.ForeColor = Color.Black;
            ScreenFile.BackColor = Color.White;
            ScreenFile.ForeColor = Color.Black;
            this.ForeColor = Color.Black;
            this.BackColor = Color.White;
            menuStrip1.BackColor = Color.White;
            menuStrip1.ForeColor = Color.Black;
        }

        private void синийToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TreeDirectories.BackColor = Color.Blue;
            ScreenFile.BackColor = Color.Blue;
            this.BackColor = Color.Blue;
            menuStrip1.BackColor = Color.Blue;
        }

        private void красныйToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TreeDirectories.BackColor = Color.Red;
            ScreenFile.BackColor = Color.Red;
            this.BackColor = Color.Red;
            menuStrip1.BackColor = Color.Red;
        }

        private void NavigateEnter(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                try
                {
                    if (Directory.Exists(AddresLine.Text))
                    {
                        ScreenFile.Items.Clear();
                        string[] files = Directory.GetFiles(AddresLine.Text);
                        // перебор полученных файлов
                        foreach (string file in files)
                        {

                            ListViewItem lvi = new ListViewItem();
                            lvi.Text = file.Remove(0, file.LastIndexOf('\\') + 1);
                            // установка названия файла
                            lvi.ImageIndex = ExtensionsFile(file);
                            ScreenFile.Items.Add(lvi);
                        }
                    }
                    else
                    {
                        MessageBox.Show("Данной папки нет", "Навигация", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex) { }
            }
        }

        private async void CompressAsync(object sender, EventArgs e)
        {
            try
            {
                using (ZipFile zip = new ZipFile())
                {
                    zip.AlternateEncodingUsage = ZipOption.Always;
                    zip.AlternateEncoding = Encoding.GetEncoding(866);
                    string path = TreeDirectories.SelectedNode.FullPath;
                    string[] files = Directory.GetFiles(path);
                    string[] dirs = Directory.GetDirectories(path);
                    foreach (string dir in dirs)
                    {
                        if (Directory.Exists(path))
                        {
                            await Task.Run(() => zip.AddDirectory(dir, ""));
                        }
                    }
                    //// перебор полученных файлов
                    foreach (string file in files)
                    {
                        if (File.Exists(file))
                        {
                            await Task.Run(() => zip.AddFile(file, ""));
                        }
                    }
                    FormForRenaming fr = new FormForRenaming(zip, path);
                    fr.Show();
                }
            }
            catch (Exception ex)
            { }
        }

        private void DeleteFIle(object sender, EventArgs e)
        {
            try
            {
                DialogResult result = MessageBox.Show("Вы уверены, что хотите удалить файл?", "Удаление файла", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    if (ScreenFile.SelectedItems.Count != 0)
                    {
                        for (int i = 0; i < ScreenFile.SelectedItems.Count-1; i++)
                        {
                            File.Delete(PathTree.Text + "\\" + ScreenFile.SelectedItems[i].Text);
                            ScreenFile.SelectedItems[i].Remove();

                        }
                    }
                    else
                    {
                        File.Delete(AddresLine.Text);
                        ScreenFile.FocusedItem.Remove();
                    }

                }
                else
                {
                    return;
                }


            }
            catch (DirectoryNotFoundException ex)
            {
                MessageBox.Show("Директория не найдена. Ошибка: " + ex.Message, "Ошибка при удалении", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
            catch (UnauthorizedAccessException ex)
            {
                MessageBox.Show("Отсутствует доступ. Ошибка: " + ex.Message, "Ошибка при удалении", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
            catch (Exception ex)
            {
                MessageBox.Show("Произошла ошибка: " + ex.Message, "Ошибка при удалении", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void CopyDirectories(object sender, EventArgs e)
        {
            
            StringCollection paths = new StringCollection();
            string path = TreeDirectories.SelectedNode.FullPath;
            string[] files = Directory.GetFiles(path);
            string[] dirs = Directory.GetDirectories(path);
            foreach (string dir in dirs)
            {
                if (Directory.Exists(path))
                {
                    paths.Add(dir);
                }
            }
            foreach (string file in files)
            {
                if (File.Exists(file))
                {
                    paths.Add(file);
                }
            }
            Clipboard.SetFileDropList(paths);
        }
        private void CopyFiles(object sender, EventArgs e)
        {
            if(ScreenFile.SelectedItems.Count ==0)
            {
                return;
            } else
            {

            StringCollection paths = new StringCollection();
            if (ScreenFile.SelectedItems.Count!=1)
            {
                for (int i = 0; i < ScreenFile.SelectedItems.Count; i++)
                {
                    paths.Add(PathTree.Text+"\\"+ScreenFile.SelectedItems[i].Text);
                }
            }else
            {
                paths.Add(AddresLine.Text);
            }
            Clipboard.SetFileDropList(paths);
            }
        }
        private async void PasteFiles(object sender, EventArgs e)
        {
            string dest = PathTree.Text;
            StringCollection paths = new StringCollection();
            paths = Clipboard.GetFileDropList();
            foreach (var p in paths)
            {
                int r = p.LastIndexOf("\\");
                await Task.Run(() => File.Copy(p, dest + "\\" + p.Substring(r)));
            }
        }
        private async  void PasteDirectories(object sender, EventArgs e)
        {
            string dest = TreeDirectories.SelectedNode.FullPath;
            StringCollection paths = new StringCollection();
            paths = Clipboard.GetFileDropList();
            foreach (var p in paths)
            {
                
                string name = p.Substring(p.LastIndexOf("\\"));
                if (File.Exists(p))
                {
                    await Task.Run(() => File.Copy(p, dest+name));
                } 
                else if (Directory.Exists(p))
                {
                    await DirectoryCopy(p, dest + name);
                    //await Task.Run(() => Directory.C(p, dest + name));
                }
            }
        }
        private async Task DirectoryCopy(string src, string dstn)
        {
            DirectoryInfo srcDir = new DirectoryInfo(src);
            DirectoryInfo dstnDir = new DirectoryInfo(dstn);
            Directory.CreateDirectory(dstn);
            foreach (DirectoryInfo dir in srcDir.GetDirectories())
            {
                await DirectoryCopy(dir.FullName, Path.Combine(dstn, dir.Name));
            }
            foreach (FileInfo file in srcDir.GetFiles())
            {
                await Task.Run(() => file.CopyTo(Path.Combine(dstnDir.FullName, file.Name), false));
            }
        }
    }
}

        /*
private void OpenDiskFromCB(object sender, KeyEventArgs e)
{
//try
//{
//    if (e.KeyCode == Keys.Enter)
//    {
//        TreeDirectories.Nodes.Clear();
//        string[] dirs = Directory.GetDirectories(Disks.SelectedItem.ToString());
//        foreach (string dir in dirs)
//        {
//            TreeNode dirNode = new TreeNode();
//            dirNode.Text = dir.Remove(0, dir.LastIndexOf("\\") + 1);
//            dirNode.ImageIndex = 0;
//            TreeDirectories.Nodes.Add(dirNode);
//            //driveNode.Nodes.Add(dirNode);
//        }
//        string[] files = Directory.GetFiles(Disks.SelectedItem.ToString());
//        foreach (string fl in files)
//        {
//            TreeNode dirNode = new TreeNode();
//            dirNode.Text = fl.Remove(0, fl.LastIndexOf("\\") + 1);
//            dirNode.ImageIndex = 1;
//            TreeDirectories.Nodes.Add(dirNode);
//            //driveNode.Nodes.Add(dirNode);
//        }
//    }

//}
//catch (Exception ex) { }
//TreeDirectories.BeforeExpand += treeView1_BeforeExpand;
// заполняем дерево дисками
//FillDriveNodes();
//FillComboBox();
TreeDirectories.Nodes.Clear();
string[] dirs;
try
{
if (Directory.Exists(Disks.SelectedItem.ToString()))
{
Disks.SelectedIndex = 0;
dirs = Directory.GetDirectories(Disks.SelectedItem.ToString());
if (dirs.Length != 0)
{
for (int i = 0; i < dirs.Length; i++)
{
 TreeNode dirNode = new TreeNode(new DirectoryInfo(dirs[i]).Name);
 FillTreeNode(dirNode, dirs[i]);
 TreeDirectories.Nodes.Add(dirNode);
 //e.Node.Nodes.Add(dirNode);
}
}
string[] files = Directory.GetFiles(Disks.SelectedItem.ToString());
ScreenFile.Items.Clear();
// перебор полученных файлов
foreach (string file in files)
{

ListViewItem lvi = new ListViewItem();
lvi.Text = file.Remove(0, file.LastIndexOf('\\') + 1);
// установка названия файла
string ext = Path.GetExtension(file);
if (ext == ".pdf")
{
 lvi.ImageIndex = 1;
}
else if (ext == ".png" || ext == ".JPG" || ext == ".mp4" || ext == ".jpg" || ext == ".MOV")
{
 lvi.ImageIndex = 5;
}
else if (ext == ".xls" || ext == ".csv" || ext == ".xlsx")
{
 lvi.ImageIndex = 3;
}
else if (ext == ".doc" || ext == ".docx" || ext == ".txt" || ext == ".rtf")
{
 lvi.ImageIndex = 4;
}
else if (ext == ".json")
{
 lvi.ImageIndex = 6;
}
else
{
 lvi.ImageIndex = 7;
}
ScreenFile.Items.Add(lvi);
}
//textBox1.Text = e.Node.FullPath;
}
}
catch (Exception ex) { }
}
*/
