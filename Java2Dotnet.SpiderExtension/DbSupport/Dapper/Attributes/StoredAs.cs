using System;

namespace Java2Dotnet.Spider.Extension.DbSupport.Dapper.Attributes
{
	/// <summary>
	/// 
	/// </summary>
	public class StoredAs : Attribute
	{
		public enum ValueType { Text, Varchar, Char, Time, Float, Bool, Int, Date, Long }

		/// <summary>
		/// 
		/// </summary>
		public string Value { get; set; }

		public ValueType Type { get; set; }

		public bool NotNull { get; set; }

		public int Lenth { get; set; }

		public bool PrimaryKey { get; set; }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="name"></param>
		/// <param name="type"></param>
		/// <param name="primaryKey"></param>
		/// <param name="length"></param>
		/// <param name="notNull"></param>
		public StoredAs(string name, ValueType type = ValueType.Varchar, bool primaryKey = false, int length = 0, bool notNull = false)
		{
			Value = name;
			Type = type;
			Lenth = length;
			NotNull = notNull;
			PrimaryKey = primaryKey;
		}
	}
}
