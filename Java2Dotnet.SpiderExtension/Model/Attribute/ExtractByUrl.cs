using System;

namespace Java2Dotnet.Spider.Extension.Model.Attribute
{
	/// <summary>
	/// Define a extractor to extract data in url of current page. Only regex can be used. 
	/// </summary>
	[AttributeUsage(AttributeTargets.Property)]
	public class ExtractByUrl : System.Attribute
	{
		public ExtractByUrl(string value = "", bool notNull = false, bool multi = false)
		{
			Value = value;
			NotNull = notNull;
			Multi = multi;
		}

		/// <summary>
		/// Extractor expression, only regex can be used
		/// </summary>
		public string Value { get; set; }

		/// <summary>
		/// Define whether the field can be null.
		/// If set to 'true' and the extractor get no result, the entire class will be discarded.
		/// </summary>
		public bool NotNull { get; set; }

		/// <summary>
		/// Define whether the extractor return more than one result.
		/// When set to 'true', the extractor return a list of string (so you should define the field as List).
		/// 
		/// Deprecated since 0.4.2. This option is determined automatically by the class of field.
		/// @deprecated since 0.4.2
		/// </summary>
		public bool Multi { get; set; }
	}
}
