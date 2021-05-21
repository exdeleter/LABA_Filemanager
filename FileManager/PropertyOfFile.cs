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
        public PropertyOfFile(string pathe)
        {
            InitializeComponent();
            DirectoryInfo directory = new DirectoryInfo(pathe);
            this.Text = "Свойства: " + directory.Name;
            NameFIle.Text = directory.Name;
            if (pathe == "C:\\")
            {
                DriveInfo driveInfo = new DriveInfo(pathe);
                TypeOfFile.Text = "Тип файла: \t Диск ";
                double sizedisk = driveInfo.TotalSize;
                double availablesizedisk = driveInfo.TotalFreeSpace;
                double finishsize = sizedisk - availablesizedisk;
                SizeOfFile.Text = "Размер папки: " + Math.Round(finishsize / 1000000000.00, 3) + "ГБ ( " + finishsize + " ) байт";
            }else if (directory.Extension.Length == 0)
            {
                TypeOfFile.Text = "Тип папки: \t Папка";
                SizeOfFile.Text = "Размер: " + Math.Round(((double)(DirSize(directory)/ 1000000.00)),2) + "МБ (" + DirSize(directory) + ") байт";

            } else
            {
                FileInfo fileInfo = new FileInfo(pathe);
                TypeOfFile.Text = "Тип файла: \t" + $"{Path.GetExtension(pathe)}";
                SizeOfFile.Text = "Размер: " + Math.Round(((double)(fileInfo.Length) / 1000000.00), 2) + "МБ (" + fileInfo.Length + ") байт";
                label1.Text = "Дата создания: " + fileInfo.LastWriteTime;
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
