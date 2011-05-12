namespace Plugin.ArchiveTools
{
    partial class PAKViewer
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
            this.listView1 = new System.Windows.Forms.ListView();
            this.chFileName = new System.Windows.Forms.ColumnHeader();
            this.chFileSize = new System.Windows.Forms.ColumnHeader();
            this.chOffset = new System.Windows.Forms.ColumnHeader();
            this.chFlag = new System.Windows.Forms.ColumnHeader();
            this.SuspendLayout();
            // 
            // listView1
            // 
            this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.chFileName,
            this.chFileSize,
            this.chOffset,
            this.chFlag});
            this.listView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listView1.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.listView1.Location = new System.Drawing.Point(0, 0);
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(615, 402);
            this.listView1.TabIndex = 1;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.View = System.Windows.Forms.View.Details;
            // 
            // chFileName
            // 
            this.chFileName.Text = "NAME:FileName";
            this.chFileName.Width = 109;
            // 
            // chFileSize
            // 
            this.chFileSize.Text = "NAME:FileSize";
            this.chFileSize.Width = 115;
            // 
            // chOffset
            // 
            this.chOffset.Text = "Name:Offset";
            this.chOffset.Width = 110;
            // 
            // chFlag
            // 
            this.chFlag.Text = "NAME:ARCFlag";
            this.chFlag.Width = 92;
            // 
            // PAKViewer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(615, 402);
            this.Controls.Add(this.listView1);
            this.Name = "PAKViewer";
            this.TabText = "GUI:PAKViewer";
            this.Text = "GUI:PAKViewer";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.ColumnHeader chFileName;
        private System.Windows.Forms.ColumnHeader chFileSize;
        private System.Windows.Forms.ColumnHeader chOffset;
        private System.Windows.Forms.ColumnHeader chFlag;
    }
}