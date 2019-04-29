/*
 * Created by SharpDevelop.
 * User: Cameron
 * Date: 31/03/2014
 * Time: 9:16 AM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
namespace TPSReaderGUI
{
	partial class PleaseWaitForm
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
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label1.Location = new System.Drawing.Point(31, 41);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(366, 23);
			this.label1.TabIndex = 0;
			this.label1.Text = "Please Wait...";
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(31, 64);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(233, 23);
			this.label2.TabIndex = 1;
			this.label2.Text = "Some operations may take up to 10 minutes.";
			// 
			// PleaseWaitForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(456, 143);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			this.Name = "PleaseWaitForm";
			this.ShowInTaskbar = false;
			this.Text = "Please Wait";
			this.ResumeLayout(false);
		}
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label1;
	}
}
