using Java2Dotnet.Spider.Core;

namespace Java2Dotnet.Spider.Extension.Pipeline
{
	/// <summary>
	/// Print page model in con
	/// Usually used in test.
	/// </summary>
	public class ConsolePageModelPipeline : IPageModelPipeline
	{
		public void Process(dynamic o, ITask task)
		{
			//System.Console.WriteLine(ToStringBuilder.reflectionToString(o));
		}
	}
}
