using System.Data;

namespace Java2Dotnet.Spider.Extension.DbSupport
{
	/// <summary>
	/// Data provider interface
	/// </summary>
	public interface IDataProvider
	{
		/// <summary>
		/// Create a connection
		/// </summary>
		/// <returns></returns>
		IDbConnection CreateConnection();

		/// <summary>
		/// Create a connection
		/// </summary>
		/// <returns></returns>
		IDbConnection CreateConnection(string connStr);

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		string GetHost();
	}
}
