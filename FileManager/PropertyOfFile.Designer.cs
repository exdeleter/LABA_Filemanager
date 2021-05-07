
namespace FileManager
{
    partial class PropertyOfFile
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PropertyOfFile));
            this.NameFIle = new System.Windows.Forms.TextBox();
            this.TypeOfFile = new System.Windows.Forms.Label();
            this.SizeOfFile = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // NameFIle
            // 
            this.NameFIle.Location = new System.Drawing.Point(104, 28);
            this.NameFIle.Name = "NameFIle";
            this.NameFIle.Size = new System.Drawing.Size(268, 22);
            this.NameFIle.TabIndex = 0;
            // 
            // TypeOfFile
            // 
            this.TypeOfFile.AutoSize = true;
            this.TypeOfFile.Location = new System.Drawing.Point(23, 108);
            this.TypeOfFile.Name = "TypeOfFile";
            this.TypeOfFile.Size = new System.Drawing.Size(46, 17);
            this.TypeOfFile.TabIndex = 1;
            this.TypeOfFile.Text = "label1";
            // 
            // SizeOfFile
            // 
            this.SizeOfFile.AutoSize = true;
            this.SizeOfFile.Location = new System.Drawing.Point(23, 136);
            this.SizeOfFile.Name = "SizeOfFile";
            this.SizeOfFile.Size = new System.Drawing.Size(46, 17);
            this.SizeOfFile.TabIndex = 2;
            this.SizeOfFile.Text = "label1";
            // 
            // PropertyOfFile
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(384, 450);
            this.Controls.Add(this.SizeOfFile);
            this.Controls.Add(this.TypeOfFile);
            this.Controls.Add(this.NameFIle);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "PropertyOfFile";
            this.Text = "PropertyOfFile";
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.PaintLines);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox NameFIle;
        private System.Windows.Forms.Label TypeOfFile;
        private System.Windows.Forms.Label SizeOfFile;
    }
}