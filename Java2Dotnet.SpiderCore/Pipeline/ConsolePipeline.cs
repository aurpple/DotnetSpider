using System.Collections;

namespace Java2Dotnet.Spider.Core.Pipeline
{
	/// <summary>
	/// Write results in console.
	/// Usually used in test.
	/// </summary>
	public class ConsolePipeline : IPipeline
	{
		public void Process(ResultItems resultItems, ITask task)
		{
			foreach (DictionaryEntry entry in resultItems.GetAll())
			{
				System.Console.WriteLine(entry.Key + ":\t" + entry.Value);
			}
		}
	}
}
