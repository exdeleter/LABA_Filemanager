using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
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
       
        
        public FileComm()
        {
            InitializeComponent();
            Init();
            
        }
        private void Init()
        {
            FillPic();
            //treeView1.BeforeSelect += treeView1_BeforeSelect;
            FirstTree.BeforeExpand += treeView1_BeforeExpand;
            // заполняем дерево дисками
            FillDriveNodes();
            FillComboBox();
        }
        private void FillPic()
        {
            
            ImageList imageList1 = new ImageList();
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
            FirstTree.ImageList = imageList1;
            ScreenFile.LargeImageList = imageList1;
        }

        void treeView1_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            e.Node.Nodes.Clear();
            string[] dirs;
            try
            {
                if (Directory.Exists(e.Node.FullPath))
                {
                    comboBox1.SelectedIndex = 0;
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
                    // перебор полученных файлов
                    foreach (string file in files)
                    {

                        ListViewItem lvi = new ListViewItem();
                        lvi.Text = file.Remove(0, file.LastIndexOf('\\') + 1);
                        // установка названия файла
                        string ext = Path.GetExtension(file);
                        if (ext ==".pdf")
                        {
                            lvi.ImageIndex = 1;
                        } else if (ext == ".png" || ext ==".JPG" || ext == ".mp4" || ext== ".jpg")
                        {
                            lvi.ImageIndex = 5;
                        } else if (ext == ".xls" || ext == ".csv" || ext == ".xlsx" )
                        {
                            lvi.ImageIndex = 3;
                        }

                        // установка картинки для файла
                        // добавляем элемент в ListView
                        ScreenFile.Items.Add(lvi);
                    }
                    //dirs = Directory.GetFiles(e.Node.FullPath);
                    //if (dirs.Length != 0)
                    //{
                    //    for (int i = 0; i < dirs.Length; i++)
                    //    {
                    //        TreeNode dirNode = new TreeNode(new DirectoryInfo(dirs[i]).Name);
                    //        dirNode.ImageIndex = 1;
                    //        FillTreeNode(dirNode, dirs[i]);
                    //        e.Node.Nodes.Add(dirNode);
                    //    }
                    //}
                    textBox1.Text = e.Node.FullPath;
                }
            }
            catch (Exception ex) { }
        }
        //событие перед выделением узла
        //void treeView1_BeforeSelect(object sender, TreeViewCancelEventArgs e)
        //{
        //    e.Node.Nodes.Clear();
        //    string[] dirs;
        //    try
        //    {
        //        if (Directory.Exists(e.Node.FullPath))
        //        {
        //            dirs = Directory.GetDirectories(e.Node.FullPath);
        //            if (dirs.Length != 0)
        //            {
        //                for (int i = 0; i < dirs.Length; i++)
        //                {

        //                    TreeNode dirNode = new TreeNode(new DirectoryInfo(dirs[i]).Name);
        //                    dirNode.ImageIndex = 2;
        //                    FillTreeNode(dirNode, dirs[i]);
        //                    e.Node.Nodes.Add(dirNode);
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex) { }
        //}
        private void FillDriveNodes()
        {
            try
            {
                
                foreach (DriveInfo drive in DriveInfo.GetDrives())
                {
                    TreeNode driveNode = new TreeNode { Text = drive.Name };
                    driveNode.ImageIndex = 2;
                    FillTreeNode(driveNode, drive.Name);
                    
                    FirstTree.Nodes.Add(driveNode);
                }
            }
            catch (Exception ex) { }
        }
        private void FillComboBox()
        {

            try
            {

                foreach (DriveInfo drive in DriveInfo.GetDrives())
                {
                    comboBox1.Items.Add(drive.Name);
                    
                }
            }
            catch (Exception ex) { }
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

        private void OpenSelectedFile(object sender, EventArgs e)
        {
            Process.Start(FirstTree.SelectedNode.FullPath);
        }

        private void CreateDirectory(object sender, EventArgs e)
        {
            string path = FirstTree.SelectedNode.FullPath + @"\Новая папка";
            if (Directory.Exists(path))
            {
                MessageBox.Show("Эта папка уже существует", "Создание файла", MessageBoxButtons.OK, MessageBoxIcon.Error);
            } else
            {
                Directory.CreateDirectory(path);
                
            }
           
        }

        private void OpenProperty(object sender, EventArgs e)
        {
            PropertyOfFile pr = new PropertyOfFile(FirstTree.SelectedNode);
            pr.Show();
        }

    }
}
