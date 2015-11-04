using System.Reflection;
using Java2Dotnet.Spider.Core.Selector;
using Java2Dotnet.Spider.Extension.Model.Attribute;
using Java2Dotnet.Spider.Extension.Model.Formatter;

namespace Java2Dotnet.Spider.Extension.Model
{
	/// <summary>
	/// Wrapper of field and extractor.
	/// </summary>
	public class FieldExtractor : Extractor
	{
		public FieldExtractor(PropertyInfo field, ISelector selector, ExtractSource source, bool notNull, bool multi, Stoper stoper = null)
			: base(selector, source, notNull, multi)
		{
			Field = field;
			Stoper = stoper;
		}

		public PropertyInfo Field { get; private set; }

		public IObjectFormatter ObjectFormatter { get; set; }

		public Stoper Stoper { get; set; }

		public bool Download { get; set; }
	}
}