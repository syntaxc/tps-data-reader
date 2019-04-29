/*
 * Created by SharpDevelop.
 * User: Cameron
 * Date: 20/03/2014
 * Time: 3:09 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
namespace TPSReaderGUI
{
	partial class MainForm
	{
		/// <summary>
		/// Designer variable used to keep track of non-visual components.
		/// </summary>
		private System.ComponentModel.IContainer components = null;
		
		/// <summary>
		/// Disposes resources used by the form.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing) {
				if (components != null) {
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}
		
		/// <summary>
		/// This method is required for Windows Forms designer support.
		/// Do not change the method contents inside the source code editor. The Forms designer might
		/// not be able to load this method if it was changed manually.
		/// </summary>
		private void InitializeComponent()
		{
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openTPSFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripLoadedFile = new System.Windows.Forms.ToolStripStatusLabel();
            this.fileInfoBW = new System.ComponentModel.BackgroundWorker();
            this.tableDataBW = new System.ComponentModel.BackgroundWorker();
            this.tableList = new System.Windows.Forms.ListBox();
            this.tableFieldList = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader5 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.viewDataTb = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.exportToCSVBtn = new System.Windows.Forms.Button();
            this.exportBW = new System.ComponentModel.BackgroundWorker();
            this.exportFolder = new System.Windows.Forms.FolderBrowserDialog();
            this.menuStrip1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.Filter = "TPS Files|*.TPS";
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(814, 24);
            this.menuStrip1.TabIndex = 5;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openTPSFileToolStripMenuItem,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // openTPSFileToolStripMenuItem
            // 
            this.openTPSFileToolStripMenuItem.Name = "openTPSFileToolStripMenuItem";
            this.openTPSFileToolStripMenuItem.Size = new System.Drawing.Size(147, 22);
            this.openTPSFileToolStripMenuItem.Text = "Open TPS File";
            this.openTPSFileToolStripMenuItem.Click += new System.EventHandler(this.OpenTPSFileToolStripMenuItemClick);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(147, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.ExitToolStripMenuItemClick);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripLoadedFile});
            this.statusStrip1.Location = new System.Drawing.Point(0, 598);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(814, 22);
            this.statusStrip1.TabIndex = 6;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripLoadedFile
            // 
            this.toolStripLoadedFile.Name = "toolStripLoadedFile";
            this.toolStripLoadedFile.Size = new System.Drawing.Size(0, 17);
            // 
            // fileInfoBW
            // 
            this.fileInfoBW.WorkerReportsProgress = true;
            this.fileInfoBW.DoWork += new System.ComponentModel.DoWorkEventHandler(this.FileInfoBWDoWork);
            this.fileInfoBW.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.FileInfoBWRunWorkerCompleted);
            // 
            // tableDataBW
            // 
            this.tableDataBW.WorkerReportsProgress = true;
            this.tableDataBW.DoWork += new System.ComponentModel.DoWorkEventHandler(this.TableDataBWDoWork);
            this.tableDataBW.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.TableDataBWRunWorkerCompleted);
            // 
            // tableList
            // 
            this.tableList.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.tableList.FormattingEnabled = true;
            this.tableList.Location = new System.Drawing.Point(2, 55);
            this.tableList.Name = "tableList";
            this.tableList.Size = new System.Drawing.Size(207, 511);
            this.tableList.TabIndex = 7;
            this.tableList.SelectedIndexChanged += new System.EventHandler(this.TableListSelectedIndexChanged);
            // 
            // tableFieldList
            // 
            this.tableFieldList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableFieldList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3,
            this.columnHeader4,
            this.columnHeader5});
            this.tableFieldList.Location = new System.Drawing.Point(215, 55);
            this.tableFieldList.Name = "tableFieldList";
            this.tableFieldList.Size = new System.Drawing.Size(587, 511);
            this.tableFieldList.TabIndex = 8;
            this.tableFieldList.UseCompatibleStateImageBehavior = false;
            this.tableFieldList.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Field No";
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Field Name";
            this.columnHeader2.Width = 181;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Data Type";
            this.columnHeader3.Width = 151;
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "Data Length";
            this.columnHeader4.Width = 84;
            // 
            // columnHeader5
            // 
            this.columnHeader5.Text = "Data Offset";
            this.columnHeader5.Width = 88;
            // 
            // viewDataTb
            // 
            this.viewDataTb.Enabled = false;
            this.viewDataTb.Location = new System.Drawing.Point(396, 570);
            this.viewDataTb.Name = "viewDataTb";
            this.viewDataTb.Size = new System.Drawing.Size(75, 23);
            this.viewDataTb.TabIndex = 9;
            this.viewDataTb.Text = "View Data";
            this.viewDataTb.UseVisualStyleBackColor = true;
            this.viewDataTb.Click += new System.EventHandler(this.ViewDataTbClick);
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(9, 31);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(189, 23);
            this.label1.TabIndex = 11;
            this.label1.Text = "TABLES";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(447, 29);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(100, 23);
            this.label2.TabIndex = 12;
            this.label2.Text = "FIELDS";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // exportToCSVBtn
            // 
            this.exportToCSVBtn.Enabled = false;
            this.exportToCSVBtn.Location = new System.Drawing.Point(477, 570);
            this.exportToCSVBtn.Name = "exportToCSVBtn";
            this.exportToCSVBtn.Size = new System.Drawing.Size(94, 23);
            this.exportToCSVBtn.TabIndex = 13;
            this.exportToCSVBtn.Text = "Export To CSV";
            this.exportToCSVBtn.UseVisualStyleBackColor = true;
            this.exportToCSVBtn.Click += new System.EventHandler(this.ExportToCSVBtnClick);
            // 
            // exportBW
            // 
            this.exportBW.DoWork += new System.ComponentModel.DoWorkEventHandler(this.ExportBWDoWork);
            this.exportBW.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.ExportBWRunWorkerCompleted);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(814, 620);
            this.Controls.Add(this.exportToCSVBtn);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.viewDataTb);
            this.Controls.Add(this.tableFieldList);
            this.Controls.Add(this.tableList);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "MainForm";
            this.Text = "TPSReaderGUI";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		private System.Windows.Forms.FolderBrowserDialog exportFolder;
		private System.ComponentModel.BackgroundWorker exportBW;
		private System.Windows.Forms.Button exportToCSVBtn;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button viewDataTb;
		private System.Windows.Forms.ColumnHeader columnHeader5;
		private System.Windows.Forms.ColumnHeader columnHeader4;
		private System.Windows.Forms.ColumnHeader columnHeader3;
		private System.Windows.Forms.ColumnHeader columnHeader2;
		private System.Windows.Forms.ColumnHeader columnHeader1;
		private System.Windows.Forms.ListView tableFieldList;
		private System.Windows.Forms.ListBox tableList;
		private System.ComponentModel.BackgroundWorker tableDataBW;
		private System.ComponentModel.BackgroundWorker fileInfoBW;
		private System.Windows.Forms.ToolStripStatusLabel toolStripLoadedFile;
		private System.Windows.Forms.StatusStrip statusStrip1;
		private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem openTPSFileToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
		private System.Windows.Forms.MenuStrip menuStrip1;
		private System.Windows.Forms.OpenFileDialog openFileDialog1;
	}
}
