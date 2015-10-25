namespace Java2Dotnet.Spider.Core
{
	/// <summary>
	/// Interface for identifying different tasks.
	/// </summary>
	public class DefaultTask : ITask
	{
		public DefaultTask(string uuid,Site site)
		{
			Identify = uuid;
			Site = site;
		}

		/// <summary>
		/// Unique id for a task.
		/// </summary>
		public string Identify { get; set; }


		/// <summary>
		/// Site of a task
		/// </summary>
		public Site Site { get; set; }
	}
}
