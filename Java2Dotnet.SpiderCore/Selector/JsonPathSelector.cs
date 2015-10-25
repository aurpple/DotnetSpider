using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Java2Dotnet.Spider.Core.Selector
{
	/// <summary>
	/// JsonPath selector. 
	/// Used to extract content from JSON. 
	/// </summary>
	public class JsonPathSelector : ISelector
	{
		private readonly string _jsonPathStr;

		public JsonPathSelector(string jsonPathStr)
		{
			_jsonPathStr = jsonPathStr;
		}

		public string Select(string text)
		{
			IList<string> result = SelectList(text);
			if (result.Count > 0)
			{
				return result[0];
			}
			return null;
		}

		public IList<string> SelectList(string text)
		{
			List<string> list = new List<string>();
			JObject o = (JObject)JsonConvert.DeserializeObject(text);
			var items = o.SelectTokens(_jsonPathStr);

			if (items == null)
			{
				return list;
			}

			list.AddRange(items.Select(item => item.ToString()));

			return list;
		}
	}
}