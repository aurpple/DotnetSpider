using System;
using System.Collections.Generic;

namespace Java2Dotnet.Spider.Core.Selector
{
	public class EnviromentSelector : ISelector
	{
		private readonly string _field;

		static EnviromentSelector()
		{
		}

		public EnviromentSelector(string field)
		{
			_field = field;
		}

		public object GetValue(Page page)
		{
			if (_field.ToLower() == "url")
			{
				return page.GetUrl().ToString();
			}

			if (_field.ToLower() == "targeturl")
			{
				return page.GetTargetUrl().Value;
			}

			return page.GetRequest().GetExtra(_field);
		}

		public IList<string> GetValueList(Page page)
		{
			throw new NotImplementedException();
		}

		public IList<string> SelectList(string text)
		{
			throw new NotImplementedException();
		}

		public string Select(string text)
		{
			throw new NotImplementedException();
		}
	}
}
