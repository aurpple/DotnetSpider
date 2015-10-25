namespace Java2Dotnet.Spider.Core
{
	/// <summary>
	/// Interface for identifying different tasks.
	/// </summary>
	public interface ITask
	{
		/// <summary>
		/// Unique id for a task.
		/// </summary>
		string Identify { get; }

		/// <summary>
		/// Site of a task
		/// </summary>
		Site Site { get; }
	}
}
