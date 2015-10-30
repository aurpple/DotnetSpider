using System;
using System.Collections.Generic;
using Java2Dotnet.Spider.Core;

namespace Java2Dotnet.Spider.Extension.Pipeline
{
	/// <summary>
	/// Print page model in con
	/// Usually used in test.
	/// </summary>
	public class ConsolePageModelPipeline : IPageModelPipeline
	{
		public void Process(Dictionary<Type, List<dynamic>> data, ITask task)
		{
			throw new NotImplementedException();
		}
	}
}
