using System.Data;

namespace Java2Dotnet.Spider.Extension.DbSupport.Dapper
{
	/// <summary>
	/// 
	/// </summary>
	public interface IDataRepository
	{
		IDbConnection CreateConnection();

		#region Sync

		void CreateSheme();

		void CreateTable();

		/// <summary>
		/// 
		/// </summary>
		/// <param name="instance"></param>
		/// <returns></returns>
		void Insert(object instance);

		/// <summary>
		/// 
		/// </summary>
		/// <param name="instance"></param>
		/// <returns></returns>
		void Update(object instance);

		int Execute(string sql);

		#endregion
	}
}
