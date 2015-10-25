namespace Java2Dotnet.Spider.Extension.Monitor
{
	partial class Form1
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
			this.label1 = new System.Windows.Forms.Label();
			this.tbLeftRequestCount = new System.Windows.Forms.TextBox();
			this.tbTotalRequestCount = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.tbErrorPageCount = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.tbPagePerSecond = new System.Windows.Forms.TextBox();
			this.label4 = new System.Windows.Forms.Label();
			this.tbProcessCount = new System.Windows.Forms.TextBox();
			this.label5 = new System.Windows.Forms.Label();
			this.btnStop = new System.Windows.Forms.Button();
			this.btnStart = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(24, 28);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(53, 12);
			this.label1.TabIndex = 1;
			this.label1.Text = "剩余请求";
			// 
			// tbLeftRequestCount
			// 
			this.tbLeftRequestCount.Location = new System.Drawing.Point(83, 25);
			this.tbLeftRequestCount.Name = "tbLeftRequestCount";
			this.tbLeftRequestCount.Size = new System.Drawing.Size(100, 21);
			this.tbLeftRequestCount.TabIndex = 2;
			// 
			// tbTotalRequestCount
			// 
			this.tbTotalRequestCount.Location = new System.Drawing.Point(303, 25);
			this.tbTotalRequestCount.Name = "tbTotalRequestCount";
			this.tbTotalRequestCount.Size = new System.Drawing.Size(100, 21);
			this.tbTotalRequestCount.TabIndex = 4;
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(233, 28);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(53, 12);
			this.label2.TabIndex = 3;
			this.label2.Text = "请求总数";
			// 
			// tbErrorPageCount
			// 
			this.tbErrorPageCount.Location = new System.Drawing.Point(83, 66);
			this.tbErrorPageCount.Name = "tbErrorPageCount";
			this.tbErrorPageCount.Size = new System.Drawing.Size(100, 21);
			this.tbErrorPageCount.TabIndex = 6;
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(13, 69);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(65, 12);
			this.label3.TabIndex = 5;
			this.label3.Text = "错误页面数";
			// 
			// tbPagePerSecond
			// 
			this.tbPagePerSecond.Location = new System.Drawing.Point(303, 66);
			this.tbPagePerSecond.Name = "tbPagePerSecond";
			this.tbPagePerSecond.Size = new System.Drawing.Size(100, 21);
			this.tbPagePerSecond.TabIndex = 8;
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(233, 69);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(65, 12);
			this.label4.TabIndex = 7;
			this.label4.Text = "每秒页面数";
			// 
			// tbProcessCount
			// 
			this.tbProcessCount.Location = new System.Drawing.Point(83, 105);
			this.tbProcessCount.Name = "tbProcessCount";
			this.tbProcessCount.Size = new System.Drawing.Size(100, 21);
			this.tbProcessCount.TabIndex = 10;
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(13, 108);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(41, 12);
			this.label5.TabIndex = 9;
			this.label5.Text = "线程数";
			// 
			// btnStop
			// 
			this.btnStop.Location = new System.Drawing.Point(26, 174);
			this.btnStop.Name = "btnStop";
			this.btnStop.Size = new System.Drawing.Size(75, 23);
			this.btnStop.TabIndex = 11;
			this.btnStop.Text = "停止任务";
			this.btnStop.UseVisualStyleBackColor = true;
			this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
			// 
			// btnStart
			// 
			this.btnStart.Location = new System.Drawing.Point(118, 174);
			this.btnStart.Name = "btnStart";
			this.btnStart.Size = new System.Drawing.Size(75, 23);
			this.btnStart.TabIndex = 12;
			this.btnStart.Text = "开始";
			this.btnStart.UseVisualStyleBackColor = true;
			this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
			// 
			// Form1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(478, 427);
			this.Controls.Add(this.btnStart);
			this.Controls.Add(this.btnStop);
			this.Controls.Add(this.tbProcessCount);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.tbPagePerSecond);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.tbErrorPageCount);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.tbTotalRequestCount);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.tbLeftRequestCount);
			this.Controls.Add(this.label1);
			this.Name = "Form1";
			this.Text = "Form1";
			this.Load += new System.EventHandler(this.Form1_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox tbLeftRequestCount;
		private System.Windows.Forms.TextBox tbTotalRequestCount;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox tbErrorPageCount;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox tbPagePerSecond;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.TextBox tbProcessCount;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Button btnStop;
		private System.Windows.Forms.Button btnStart;
	}
}

