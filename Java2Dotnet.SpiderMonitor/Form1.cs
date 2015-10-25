using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Java2Dotnet.Spider.Extension.Scheduler;

namespace Java2Dotnet.Spider.Monitor
{
	public partial class Form1 : Form
	{
		private RedisSchedulerManager manager;
		private string _selectedIdentify = string.Empty;

		public Form1()
		{
			InitializeComponent();
		}

		private void Form1_Load(object sender, EventArgs e)
		{
			string host = ConfigurationManager.AppSettings["redishost"];
			string pass = ConfigurationManager.AppSettings["redishostpass"];

            if (!string.IsNullOrEmpty(host))
			{
				manager = new RedisSchedulerManager(host, pass);
			}

			Task.Factory.StartNew(() =>
			{
				while (true)
				{
					if ((!IsDisposed || IsHandleCreated || this.components != null))
					{
						Invoke(new Action(RefreshTask));
					}
					Thread.Sleep(60000);
				}
			});

			Task.Factory.StartNew(() =>
			{
				while (true)
				{
					if ((!IsDisposed || IsHandleCreated || this.components != null))
					{
						Invoke(new Action(() =>
						{
							if (!string.IsNullOrEmpty(_selectedIdentify))
							{
								SpiderStatus spiderStatus = manager.GetTaskStatus(_selectedIdentify);

								this.tbErrorPageCount.Text = spiderStatus.ErrorPageCount.ToString();
								this.tbLeftRequestCount.Text = spiderStatus.LeftPageCount.ToString();
								this.tbTotalRequestCount.Text = spiderStatus.TotalPageCount.ToString();
								this.tbPagePerSecond.Text = spiderStatus.PagePerSecond.ToString(CultureInfo.InvariantCulture);
								this.tbRunningProcessCount.Text = spiderStatus.AliveThreadCount.ToString();
								this.tbProcessCount.Text = spiderStatus.ThreadCount.ToString();
								this.tbStartTime.Text = spiderStatus.StartTime.ToString(CultureInfo.InvariantCulture);
								this.tbEndTime.Text = spiderStatus.EndTime.ToString(CultureInfo.InvariantCulture);
								this.tbTaskStatus.Text = spiderStatus.Status;
							}
							else
							{
								SentEmptyInfo();
							}
						}));
					}
					Thread.Sleep(1000);
				}
			});
		}

		[MethodImpl(MethodImplOptions.Synchronized)]
		private void RefreshTask()
		{
			this.listBox1.Items.Clear();
			IDictionary<string, double> taskList = manager.GetTaskList(0, 10);
			foreach (var task in taskList)
			{
				this.listBox1.Items.Add(task.Key);
			}

			if (taskList.Keys.Contains(_selectedIdentify))
			{
				this.listBox1.SelectedItem = _selectedIdentify;
			}
			else
			{
				_selectedIdentify = string.Empty;
			}
		}

		private void SentEmptyInfo()
		{
			this.tbErrorPageCount.Text = "";
			this.tbLeftRequestCount.Text = "";
			this.tbTotalRequestCount.Text = "";
			this.tbPagePerSecond.Text = "";
			this.tbRunningProcessCount.Text = "";
			this.tbProcessCount.Text = "";
			this.tbStartTime.Text = "";
			this.tbEndTime.Text = "";
			this.tbTaskStatus.Text = "";
		}

		private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (this.listBox1.SelectedItem != null)
			{
				_selectedIdentify = this.listBox1.SelectedItem.ToString();
			}
		}


		private void 刷新ToolStripMenuItem_Click(object sender, EventArgs e)
		{
			RefreshTask();
		}

		private void 删除ToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (MessageBox.Show(this, @"您确定要删除此任务的记录吗?", @"警告", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) ==
				DialogResult.Yes)
			{
				//SpiderStatus spiderStatus = manager.GetTaskStatus(_selectedIdentify);
				//if (spiderStatus.Status != "Running" && spiderStatus.Status != "Init")
				//{
					manager.RemoveTask(_selectedIdentify);
					RefreshTask();
				//}
				//else
				//{
				//	MessageBox.Show(this, @"只能删除已完成或停止的任务", @"错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
				//}
			}
		}

		private void btnClearDb_Click(object sender, EventArgs e)
		{
			manager.ClearDb();
			RefreshTask();
		}
	}
}
