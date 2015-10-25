using System.Collections.Generic;
using Java2Dotnet.Spider.Core.Utils;
using Newtonsoft.Json;

namespace Java2Dotnet.Spider.Core.Selector
{
	/// <summary>
	/// Parse json
	/// </summary>
	public class Json : PlainText
	{
		public Json(IList<string> strings)
			: base(strings)
		{
		}

		public Json(string text)
			: base(text)
		{
		}

		/// <summary>
		/// Remove padding for JSONP
		/// </summary>
		/// <param name="padding"></param>
		/// <returns></returns>
		public Json RemovePadding(string padding)
		{
			string text = GetFirstSourceText();
			XTokenQueue tokenQueue = new XTokenQueue(text);
			tokenQueue.ConsumeWhitespace();
			tokenQueue.Consume(padding);
			tokenQueue.ConsumeWhitespace();
			string chompBalanced = tokenQueue.ChompBalancedNotInQuotes('(', ')');

			return new Json(chompBalanced);
		}

		public T ToObject<T>()
		{
			if (GetFirstSourceText() == null)
			{
				return default(T);
			}
			return JsonConvert.DeserializeObject<T>(GetFirstSourceText());
		}

		public List<T> ToList<T>()
		{
			if (GetFirstSourceText() == null)
			{
				return null;
			}
			return JsonConvert.DeserializeObject<List<T>>(GetFirstSourceText());
		}

		public override ISelectable JsonPath(string jsonPath)
		{
			JsonPathSelector jsonPathSelector = new JsonPathSelector(jsonPath);
			return SelectList(jsonPathSelector, GetSourceTexts());
		}
	}
}