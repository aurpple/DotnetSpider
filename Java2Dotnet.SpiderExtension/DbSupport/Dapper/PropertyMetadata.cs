using System.Reflection;
using Java2Dotnet.Spider.Extension.DbSupport.Dapper.Attributes;

namespace Java2Dotnet.Spider.Extension.DbSupport.Dapper
{
	/// <summary>
	/// 
	/// </summary>
	public class PropertyMetadata
	{
		private PropertyInfo PropertyInfo { get; }

		public string ValueType { get; }

		/// <summary>
		/// 
		/// </summary>
		public string Alias { get; }

		public bool PrimaryKey { get; }

		/// <summary>
		/// 
		/// </summary>
		public string ColumnName => string.IsNullOrEmpty(Alias) ? PropertyInfo.Name : Alias;

		/// <summary>
		/// 
		/// </summary>
		public string Name => PropertyInfo.Name;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="propertyInfo"></param>
		public PropertyMetadata(PropertyInfo propertyInfo)
		{
			PropertyInfo = propertyInfo;

			var alias = PropertyInfo.GetCustomAttribute<StoredAs>();
			if (alias != null)
			{
				Alias = alias.Value;
				StoredAs.ValueType vt = alias.Type;
				int length = alias.Lenth;
				bool notNull = alias.NotNull;
				PrimaryKey = alias.PrimaryKey;
				ValueType = Convert(vt, length, notNull);
			}
		}

		private string Convert(StoredAs.ValueType vt, int length, bool notNull)
		{
			string result = "";
			switch (vt)
			{
				case StoredAs.ValueType.Bool:
					{
						result = "TINYINT(1) " + (notNull ? "NOT NULL" : "NULL");
						break;
					}
				case StoredAs.ValueType.Char:
					{
						result = $"CHAR ({(length <= 1 ? 1 : length)}) " + (notNull ? "NOT NULL" : "NULL");
						break;
					}
				case StoredAs.ValueType.Float:
					{
						result = $"FLOAT {(length > 0 ? $"({length})" : "")} " + (notNull ? "NOT NULL" : "NULL");
						break;
					}
				case StoredAs.ValueType.Int:
					{
						result = $"INT {(length > 0 ? $"({length})" : "(11)")} " + (PrimaryKey ? "NOT NULL" : (notNull ? "NOT NULL" : "NULL"));
						break;
					}
				case StoredAs.ValueType.Text:
					{
						result = "TEXT " + (notNull ? "NOT NULL" : "NULL");
						break;
					}
				case StoredAs.ValueType.Time:
					{
						result = $"TIMESTAMP {(length > 0 ? $"({length})" : "")} " + (notNull ? "NOT NULL" : "NULL") + " DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP";
						break;
					}
				case StoredAs.ValueType.Varchar:
					{
						result = $"VARCHAR {(length > 0 ? $"({length})" : "(50)")} " + (notNull ? "NOT NULL" : "NULL");
						break;
					}
				case StoredAs.ValueType.Date:
					{
						result = "DATE " + (notNull ? "NOT NULL" : "NULL");
						break;
					}
				case StoredAs.ValueType.Long:
					{
						result = $"BIGINT {(length > 0 ? $"({length})" : "(20)")} " + (PrimaryKey ? "NOT NULL" : (notNull ? "NOT NULL" : "NULL"));
						break;
					}
			}
			return result;
		}
	}
}
