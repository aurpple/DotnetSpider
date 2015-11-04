using System;
using System.Collections.Generic;
using System.Linq;
using Java2Dotnet.Spider.Core;
using Java2Dotnet.Spider.Core.Pipeline;
using Java2Dotnet.Spider.Extension.Pipeline;
using Java2Dotnet.Spider.Extension.Processor;

namespace Java2Dotnet.Spider.Extension.Model
{
	/// <summary>
	/// The spider for page model extractor.
	/// In webmagic, we call a POJO containing extract result as "page model".
	/// You can customize a crawler by write a page model with annotations.
	/// Such as:
	/// <pre>
	/// {@literal @}TargetUrl("http://my.oschina.net/flashsword/blog/\\d+")
	///  public class OschinaBlog{
	/// 
	///      {@literal @}ExtractBy("//title")
	///      private String title;
	/// 
	///      {@literal @}ExtractBy(value = "div.BlogContent",type = ExtractBy.Type.Css)
	///      private String content;
	/// 
	///      {@literal @}ExtractBy(value = "//div[@class='BlogTags']/a/text()", multi = true)
	/// }
	/// </pre>
	/// And start the spider by:
	/// <pre>
	///   OOSpider.create(Site.me().addStartUrl("http://my.oschina.net/flashsword/blog")
	///        ,new JsonFilePageModelPipeline(), OschinaBlog.class).run();
	/// }
	/// </pre>
	/// </summary>
	public class OoSpider : Core.Spider
	{
		private readonly ModelPageProcessor _modelPageProcessor;
		private readonly ModelPipeline _modelPipeline;
		//private readonly IList<Type> _pageModelTypes = new List<Type>();

		private OoSpider(string identify, ModelPageProcessor modelPageProcessor)
			: base(identify, modelPageProcessor)
		{
			_modelPageProcessor = modelPageProcessor;
		}

		/// <summary>
		/// Create a spider
		/// </summary>
		/// <param name="identify"></param>
		/// <param name="site"></param>
		/// <param name="pageModelPipeline"></param>
		/// <param name="modelTypes"></param>
		private OoSpider(string identify, Site site, IPageModelPipeline[] pageModelPipeline, params Type[] modelTypes)
			: this(identify, ModelPageProcessor.Create(site, modelTypes))
		{
			_modelPipeline = new ModelPipeline();

			AddPipeline(_modelPipeline);

			foreach (Type modelType in modelTypes)
			{
				if (pageModelPipeline != null)
				{
					foreach (var modelPipeline in pageModelPipeline)
					{
						_modelPipeline.Put(modelType, modelPipeline);
					}
				}
				//_pageModelTypes.Add(modelType);
			}
		}

		public ModelPipeline ModelPipeline => _modelPipeline;

		protected override List<ICollectorPipeline> GetCollectorPipeline(params Type[] types)
		{
			return types.Select(type => new PageModelCollectorPipeline(type)).Cast<ICollectorPipeline>().ToList();
		}

		public static OoSpider Create(Site site, params Type[] pageModels)
		{
			return new OoSpider(null, site, null, pageModels);
		}

		public static OoSpider Create(Site site, IPageModelPipeline pageModelPipeline, params Type[] pageModels)
		{
			return new OoSpider(null, site, new[] { pageModelPipeline }, pageModels);
		}

		public static OoSpider Create(string identify, Site site, params Type[] pageModels)
		{
			return new OoSpider(identify, site, null, pageModels);
		}

		public static OoSpider Create(string identify, Site site, IPageModelPipeline pageModelPipeline, params Type[] pageModels)
		{
			return new OoSpider(identify, site, new[] { pageModelPipeline }, pageModels);
		}

		public static OoSpider Create(string identify, Site site, IPageModelPipeline[] pageModelPipeline, params Type[] pageModels)
		{
			return new OoSpider(identify, site, pageModelPipeline, pageModels);
		}

		public OoSpider AddPageModel(IPageModelPipeline pageModelPipeline, params Type[] pageModels)
		{
			foreach (Type pageModel in pageModels)
			{
				_modelPageProcessor.AddPageModel(pageModel);
				_modelPipeline.Put(pageModel, pageModelPipeline);
			}
			return this;
		}
	}
}