using System.Collections.Generic;

namespace Java2Dotnet.Spider.Extension.DbSupport.Dapper
{
	/// <summary>
	/// 
	/// </summary>
	public interface ISqlGenerator
	{
		#region Properties

		/// <summary>
		/// 
		/// </summary>
		bool IsIdentity { get; }

		/// <summary>
		/// 
		/// </summary>
		PropertyMetadata IdentityProperty { get; }

		/// <summary>
		/// 
		/// </summary>
		string TableName { get; }

		/// <summary>
		/// 
		/// </summary>
		string Scheme { get; }

		/// <summary>
		/// 
		/// </summary>
		IEnumerable<PropertyMetadata> KeyProperties { get; }

		/// <summary>
		/// 
		/// </summary>
		IEnumerable<PropertyMetadata> BaseProperties { get; }

		/// <summary>
		/// 
		/// </summary>
		PropertyMetadata StatusProperty { get; }

		/// <summary>
		/// 
		/// </summary>
		object LogicalDeleteValue { get; }

		/// <summary>
		/// 
		/// </summary>
		bool LogicalDelete { get; }

		#endregion

		#region Functions

		string GetCreateTable();

		string GetCreateSheme();

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		string GetSelectAll();

		/// <summary>
		/// 
		/// </summary>
		/// <param name="filters"></param>
		/// <returns></returns>
		string GetSelect(object filters);

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		string GetInsert(bool bulkInsert);

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		string GetUpdate();

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		string GetDelete();

		string GetDeleteWhere(object filters);
		
		#endregion
	}
}
