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
        private string path;
        public PropertyOfFile(TreeNode tr)
        {
            InitializeComponent();
            path = tr.FullPath;
            this.Text = "Свойства: " + path;
            NameFIle.Text = path;
            
            DirectoryInfo directory = new DirectoryInfo(path);
            if(directory.Extension.Length == 0)
            {
                DriveInfo driveInfo = new DriveInfo(path);
                TypeOfFile.Text = "Тип файла: \t Папка ";
                double sizedisk = driveInfo.TotalSize;
                double availablesizedisk = driveInfo.TotalFreeSpace;
                double finishsize = sizedisk - availablesizedisk;
                SizeOfFile.Text = "Размер папки: " + Math.Round(finishsize/ 1000000000.00,3) + "ГБ ( " + finishsize + " ) байт";
                
            } else
            {
                TypeOfFile.Text = "Тип файла: " + "\t" + directory.Extension;
                FileInfo fileInfo = new FileInfo(path);
                SizeOfFile.Text = "Размер: " + ((double)(fileInfo.Length/1000000.00))+ "МБ (" + fileInfo.Length + ")байт";

            }
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
