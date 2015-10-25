using Java2Dotnet.Spider.Core.Selector;

namespace Java2Dotnet.Spider.Extension.Configurable
{
	public class ExtractRule
	{
		public string FieldName { get; set; }
		public ExpressionType ExpressionType { get; set; }
		public string ExpressionValue { get; set; }
		public string[] ExpressionParams { get; set; }
		public bool IsMulti { get; set; }
		public bool IsNotNull { get; set; }
		private ISelector _selector;

		public ISelector Selector
		{
			get
			{
				if (_selector == null)
				{
					lock (this)
					{
						_selector = CompileSelector();
					}
				}
				return _selector;
			}
			set
			{
				_selector = value;
			}
		}

		private ISelector CompileSelector()
		{
			switch (ExpressionType)
			{
				case ExpressionType.Css:
					if (ExpressionParams.Length >= 1)
					{
						return Selectors.Css(ExpressionValue, ExpressionParams[0]);
					}
					return Selectors.Css(ExpressionValue);
				case ExpressionType.XPath:
					return Selectors.XPath(ExpressionValue);
				case ExpressionType.Regex:
					if (ExpressionParams.Length >= 1)
					{
						return Selectors.Regex(ExpressionValue, int.Parse(ExpressionParams[0]));
					}
					return Selectors.Regex(ExpressionValue);
				case ExpressionType.JsonPath:
					return new JsonPathSelector(ExpressionValue);
				default:
					return Selectors.XPath(ExpressionValue);
			}
		}
	}
}