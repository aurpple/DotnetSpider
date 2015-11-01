using System.Text.RegularExpressions;

namespace Java2Dotnet.Spider.Core
{
	/// <summary>
	/// Interface for identifying different tasks.
	/// </summary>
	public class DefaultTask : ITask
	{
		private static  readonly Regex IdentifyRegex=new Regex(@"[0-9A-Za-z_-\s]+");
		public DefaultTask(string uuid, Site site)
		{
			if (!IdentifyRegex.IsMatch(uuid))
			{
				throw new SpiderExceptoin("Task Identify only can contains A-Z a-z 0-9 _ - [SPACE]");
			}
			Identify = uuid;
			Site = site;
		}

		/// <summary>
		/// Unique id for a task.
		/// </summary>
		public string Identify { get; }


		/// <summary>
		/// Site of a task
		/// </summary>
		public Site Site { get; }
	}
}
