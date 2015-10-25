using System;

namespace Java2Dotnet.Spider.Extension.Model.Attribute
{
	[AttributeUsage(AttributeTargets.Property)]
	public class ComboExtract : System.Attribute
	{
		public enum ExtractOp
		{
			/// <summary>
			/// All extractors will be arranged as a pipeline. 
			/// The next extractor uses the result of the previous as source.
			/// </summary>
			And,

			/// <summary>
			/// All extractors will do extracting separately,  
			/// and the results of extractors will combined as the final result.
			/// </summary>
			Or
		}

		/// <summary>
		/// Types of source for extracting.
		/// </summary>
		/// <param name="value"></param>
		/// <param name="op"></param>
		/// <param name="notNull"></param>
		/// <param name="source"></param>
		/// <param name="multi"></param>
		public ComboExtract(ExtractBy[] value, ExtractOp op, bool notNull, ExtractSource source, bool multi)
		{
			Value = value;
			Op = op;
			NotNull = notNull;
			Source = source;
			Multi = multi;
		}

		/// <summary>
		/// The extractors to be combined.
		/// </summary>
		public ExtractBy[] Value { get; set; }

		/// <summary>
		/// Combining operation of extractors.
		/// </summary>
		public ExtractOp Op { get; set; }

		/// <summary>
		/// Define whether the field can be null. 
		/// If set to 'true' and the extractor get no result, the entire class will be discarded.
		/// </summary>
		public bool NotNull { get; set; }

		/// <summary>
		/// The source for extracting.
		/// It works only if you already added 'ExtractBy' to Class.
		/// </summary>
		public ExtractSource Source { get; set; }

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
