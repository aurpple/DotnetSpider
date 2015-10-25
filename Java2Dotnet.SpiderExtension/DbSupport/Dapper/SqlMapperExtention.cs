using System;
using System.Data;
using System.Linq;
using Dapper;

namespace Java2Dotnet.Spider.Extension.DbSupport.Dapper
{
	public static class SqlMapperExtention
	{
		#region util

		public static long GetIdentity(this IDbConnection conn, string primarykey, string tableName)
		{
			if (string.IsNullOrEmpty(primarykey)) primarykey = "id";
			if (string.IsNullOrEmpty(tableName))
			{
				throw new ArgumentException("tableName参数不能为空，为查询的表名");
			}
			string query = $"SELECT max({primarykey}) as Id FROM {tableName}";
			NewId identity = conn.Query<NewId>(query).Single();
			return identity.Id;
		}

		public static long GetIdentity(this IDbConnection conn, string primarykey, string tableName, IDbTransaction transaction)
		{
			if (string.IsNullOrEmpty(primarykey)) primarykey = "id";
			if (string.IsNullOrEmpty(tableName))
			{
				throw new ArgumentException("tableName参数不能为空，为查询的表名");
			}
			string query = $"SELECT max({primarykey}) as Id FROM {tableName}";
			NewId identity = conn.Query<NewId>(query, null, transaction).Single();
			return identity.Id;
		}

		private class NewId
		{
			// ReSharper disable once UnusedAutoPropertyAccessor.Local
			public long Id { get; set; }
		}

		#endregion
	}
}
