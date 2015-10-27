using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using Java2Dotnet.Spider.Core;
using Java2Dotnet.Spider.Core.Pipeline;
using Java2Dotnet.Spider.Extension.Model.Attribute;

namespace Java2Dotnet.Spider.Extension.Pipeline
{
	/// <summary>
	/// The extension to Pipeline for page model extractor.
	/// </summary>
	public class ModelPipeline : IPipeline
	{
		private readonly ConcurrentDictionary<Type, IPageModelPipeline> _pageModelPipelines = new ConcurrentDictionary<Type, IPageModelPipeline>();

		public ModelPipeline Put(Type type, IPageModelPipeline pageModelPipeline)
		{
			_pageModelPipelines.TryAdd(type, pageModelPipeline);
			return this;
		}

		public void Process(ResultItems resultItems, ITask task)
		{
			foreach (var classPageModelPipelineEntry in _pageModelPipelines)
			{
				object data = resultItems.Get(classPageModelPipelineEntry.Key.FullName);
				classPageModelPipelineEntry.Value.Process(data, task);

				//if (data != null)
				//{
				//	Attribute annotation = classPageModelPipelineEntry.Key.GetCustomAttribute(typeof(ExtractBy), false);

				//	if (annotation == null || !((ExtractBy)annotation).Multi)
				//	{
				//		classPageModelPipelineEntry.Value.Process(data, task);
				//	}
				//	else
				//	{
				//		IList<object> list = (List<object>)data;
				//		foreach (object o1 in list)
				//		{
				//			classPageModelPipelineEntry.Value.Process(o1, task);
				//		}
				//	}
				//}
			}
		}
	}
}