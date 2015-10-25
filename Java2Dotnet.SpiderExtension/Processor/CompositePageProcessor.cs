using System.Collections.Generic;
using System.Linq;
using Java2Dotnet.Spider.Core;
using Java2Dotnet.Spider.Core.Processor;
using Java2Dotnet.Spider.Extension.Handler;

namespace Java2Dotnet.Spider.Extension.Processor
{
	public class CompositePageProcessor : IPageProcessor
	{
		private Site _site;
		private IList<ISubPageProcessor> _subPageProcessors = new List<ISubPageProcessor>();

		public CompositePageProcessor(Site site)
		{
			_site = site;
		}

		public void Process(Page page)
		{
			if ((from subPageProcessor in _subPageProcessors where subPageProcessor.Match(page.GetRequest()) select subPageProcessor.ProcessPage(page)).Any(matchOtherProcessorProcessor => matchOtherProcessorProcessor != MatchOther.Yes))
			{
			}
		}

		public CompositePageProcessor SetSite(Site site)
		{
			_site = site;
			return this;
		}

		public CompositePageProcessor AddSubPageProcessor(ISubPageProcessor subPageProcessor)
		{
			_subPageProcessors.Add(subPageProcessor);
			return this;
		}

		public CompositePageProcessor SetSubPageProcessors(params ISubPageProcessor[] subPageProcessors)
		{
			_subPageProcessors = new List<ISubPageProcessor>();
			foreach (ISubPageProcessor subPageProcessor in subPageProcessors)
			{
				_subPageProcessors.Add(subPageProcessor);
			}
			return this;
		}

		public Site Site => _site;
	}
}