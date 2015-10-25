using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Java2Dotnet.Spider.Extension.Monitor
{
	public partial class Form1 : Form
	{
		private readonly ISpiderStatus _spiderStatus;

		public Form1(ISpiderStatus spiderStatus)
		{
			InitializeComponent();
			this._spiderStatus = spiderStatus;
		}

		private void Form1_Load(object sender, EventArgs e)
		{
			Text = _spiderStatus.Name;

			Task.Factory.StartNew(() =>
			{
				while (true)
				{
					if (!IsDisposed || IsHandleCreated || this.components != null)
					{
						Invoke(new Action(() =>
						{
							this.tbErrorPageCount.Text = _spiderStatus.ErrorPageCount.ToString();
							this.tbLeftRequestCount.Text = _spiderStatus.LeftPageCount.ToString();
							this.tbTotalRequestCount.Text = _spiderStatus.TotalPageCount.ToString();
							this.tbPagePerSecond.Text = _spiderStatus.PagePerSecond.ToString(CultureInfo.InvariantCulture);
							this.tbProcessCount.Text = _spiderStatus.ThreadCount.ToString();
						}));
					}
					Thread.Sleep(1000);
				}
			});
		}

		private void btnStop_Click(object sender, EventArgs e)
		{
			_spiderStatus.Stop();
		}

		private void btnStart_Click(object sender, EventArgs e)
		{
			_spiderStatus.Start();
		}
	}
}
