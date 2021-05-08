using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FileManager
{
    public partial class PropertyOfFile : Form
    {
        public PropertyOfFile(TreeNode tr)
        {
            InitializeComponent();
            DirectoryInfo directory = new DirectoryInfo(tr.FullPath);
            this.Text = "Свойства: " + directory.Name;
            NameFIle.Text = directory.Name;
            if (tr.FullPath == "C:\\")
            {
                DriveInfo driveInfo = new DriveInfo(tr.FullPath);
                TypeOfFile.Text = "Тип файла: \t Диск ";
                double sizedisk = driveInfo.TotalSize;
                double availablesizedisk = driveInfo.TotalFreeSpace;
                double finishsize = sizedisk - availablesizedisk;
                SizeOfFile.Text = "Размер папки: " + Math.Round(finishsize / 1000000000.00, 3) + "ГБ ( " + finishsize + " ) байт";
            }else if (directory.Extension.Length == 0)
            {
                TypeOfFile.Text = "Тип папки: \t Папка";
                SizeOfFile.Text = "Размер: " + Math.Round(((double)(DirSize(directory)/ 1000000.00)),2) + "МБ (" + DirSize(directory) + ") байт";
            } 
        }

        public static long DirSize(DirectoryInfo d)
        {
            long Size = 0;
            FileInfo[] fis = d.GetFiles();
            foreach (FileInfo fi in fis)
            {
                Size += fi.Length;
            }
            DirectoryInfo[] dis = d.GetDirectories();
            foreach (DirectoryInfo di in dis)
            {
                Size += DirSize(di);
            }
            return (Size);
        }
        private void PaintLines(object sender, PaintEventArgs e)
        {
            using (Graphics gr = e.Graphics)
            {
                for(int i=1; i<5;i++)
                {
                    Pen p = new Pen(Color.Black, 1);// цвет линии и ширина
                    Point p1 = new Point(10, i*75);// первая точка
                    Point p2 = new Point(this.Size.Width - 25, i*75);// вторая точка
                    gr.DrawLine(p, p1, p2);// рисуем линию
                }
                
            }
        }
    }
}
