using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Java2Dotnet.Spider.Core;

namespace Java2Dotnet.Spider.Extension.Pipeline
{
	public class CollectorPageModelPipeline : IPageModelPipeline
	{
		private readonly List<dynamic> _list = new List<dynamic>();

		[MethodImpl(MethodImplOptions.Synchronized)]
		public void Process(dynamic obj, ITask task)
		{
			_list.Add(obj);
		}

		public ICollection GetCollected()
		{
			return _list;
		}
	}
}
