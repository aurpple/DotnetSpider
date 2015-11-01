using System;
using System.Collections.Generic;
using System.Data;
using Dapper;

namespace Java2Dotnet.Spider.Extension.DbSupport.Dapper
{
	/// <summary>
	/// 
	/// </summary>
	public sealed class DataRepository : IDataRepository
	{
		public IDbConnection CreateConnection()
		{
			return GetConnection();
		}

		/// <summary>
		/// 
		/// </summary>
		public DataRepository(Type type)
		{
			SqlGenerator = new MySqlGenerator(type);
			GetConnection = DbProviderUtil.GetProvider;
		}

		private ISqlGenerator SqlGenerator { get; }

		private Func<IDbConnection> GetConnection { get; }

		#region Repository sync base actions

		/// <summary>
		/// 
		/// </summary>
		/// <param name="instance"></param>
		/// <returns></returns>
		public void Insert(object instance)
		{
			var sql = SqlGenerator.GetInsert(false);
			using (IDbConnection conn = GetConnection())
			{
				conn.Execute(sql, instance, null, 99999, CommandType.Text);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="filters"></param>
		/// <returns></returns>
		public IEnumerable<dynamic> GetWhere(object filters)
		{
			var sql = SqlGenerator.GetSelect(filters);
			using (IDbConnection conn = GetConnection())
			{
				return conn.Query(sql, filters, null, false, 99999, CommandType.Text);
			}
		}

		//public dynamic GetWhere

		/// <summary>
		/// 
		/// </summary>
		/// <param name="instance"></param>
		/// <returns></returns>
		public void Update(object instance)
		{
			var sql = SqlGenerator.GetUpdate();
			using (IDbConnection conn = GetConnection())
			{
				conn.Execute(sql, instance, null, 99999, CommandType.Text);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sql"></param>
		/// <returns></returns>
		public int Execute(string sql)
		{
			using (IDbConnection conn = GetConnection())
			{
				return conn.Execute(sql, null, null, 99999, CommandType.Text);
			}
		}

		#endregion


		public void CreateTable()
		{
			using (IDbConnection conn = GetConnection())
			{
				conn.Execute(SqlGenerator.GetCreateTable(), null, null, 99999, CommandType.Text);
			}
		}

		public void CreateSheme()
		{
			using (IDbConnection conn = GetConnection())
			{
				conn.Execute(SqlGenerator.GetCreateSheme(), null, null, 99999, CommandType.Text);
			}
		}
	}
}
