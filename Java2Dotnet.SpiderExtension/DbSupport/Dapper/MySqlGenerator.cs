using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Java2Dotnet.Spider.Extension.DbSupport.Dapper.Attributes;

namespace Java2Dotnet.Spider.Extension.DbSupport.Dapper
{
	/// <summary>
	/// 
	/// </summary>
	public sealed class MySqlGenerator : ISqlGenerator
	{
		#region Constructors

		/// <summary>
		/// 
		/// </summary>
		public MySqlGenerator(Type type)
		{
			LoadEntityMetadata(type);
		}

		private void LoadEntityMetadata(Type type)
		{
			var entityType = type;

			var aliasAttribute = entityType.GetCustomAttribute<StoredAs>();
			var schemeAttribute = entityType.GetCustomAttribute<Scheme>();
			TableName = aliasAttribute != null ? aliasAttribute.Value : entityType.Name;
			Scheme = schemeAttribute != null ? schemeAttribute.Value : "";

			//Load all the "primitive" entity properties
			IEnumerable<PropertyInfo> props = entityType.GetProperties().Where(p => p.PropertyType.IsValueType ||
																					p.PropertyType.Name.Equals("String", StringComparison.InvariantCultureIgnoreCase) ||
																					p.PropertyType.Name.Equals("Byte[]", StringComparison.InvariantCultureIgnoreCase));

			//Filter the non stored properties
			var propertyInfos = props as IList<PropertyInfo> ?? props.ToList();
			BaseProperties = propertyInfos.Where(p => !p.GetCustomAttributes<NonStored>().Any()).Select(p => new PropertyMetadata(p));

			//Filter key properties
			KeyProperties = propertyInfos.Where(p => p.GetCustomAttributes<KeyProperty>().Any()).Select(p => new PropertyMetadata(p));

			//Use identity as key pattern
			var identityProperty = propertyInfos.SingleOrDefault(p => p.GetCustomAttributes<KeyProperty>().Any(k => k.Identity));
			IdentityProperty = identityProperty != null ? new PropertyMetadata(identityProperty) : null;

			//Status property (if exists, and if it does, it must be an enumeration)
			var statusProperty = propertyInfos.FirstOrDefault(p => p.PropertyType.IsEnum && p.GetCustomAttributes<StatusProperty>().Any());

			if (statusProperty != null)
			{
				StatusProperty = new PropertyMetadata(statusProperty);
				var statusPropertyType = statusProperty.PropertyType;
				var deleteOption = statusPropertyType.GetFields().FirstOrDefault(f => f.GetCustomAttribute<Deleted>() != null);

				if (deleteOption != null)
				{
					var enumValue = Enum.Parse(statusPropertyType, deleteOption.Name);

					if (enumValue != null)
						LogicalDeleteValue = Convert.ChangeType(enumValue, Enum.GetUnderlyingType(statusPropertyType));
				}
			}
		}

		#endregion

		#region Properties

		/// <summary>
		/// 
		/// </summary>
		public bool IsIdentity => IdentityProperty != null;

		/// <summary>
		/// 
		/// </summary>
		public bool LogicalDelete => StatusProperty != null;

		/// <summary>
		/// 
		/// </summary>
		public string TableName { get; private set; }

		/// <summary>
		/// 
		/// </summary>
		public string Scheme { get; private set; }

		/// <summary>
		/// 
		/// </summary>
		public PropertyMetadata IdentityProperty { get; private set; }

		/// <summary>
		/// 
		/// </summary>
		public IEnumerable<PropertyMetadata> KeyProperties { get; private set; }

		/// <summary>
		/// 
		/// </summary>
		public IEnumerable<PropertyMetadata> BaseProperties { get; private set; }

		/// <summary>
		/// 
		/// </summary>
		public PropertyMetadata StatusProperty { get; private set; }

		/// <summary>
		/// 
		/// </summary>
		public object LogicalDeleteValue { get; private set; }

		#endregion

		#region Query generators

		public string GetCreateSheme()
		{
			return $"CREATE SCHEMA IF NOT EXISTS `{Scheme}` DEFAULT CHARACTER SET utf8mb4 ;";
		}

		public string GetCreateTable()
		{
			IEnumerable<PropertyMetadata> properties = BaseProperties.ToList();

			//			CREATE TABLE `video` (
			//  `id` int(11) NOT NULL AUTO_INCREMENT,
			//  `name` varchar(200) DEFAULT NULL,
			//  `url` text,
			//  `count` varchar(45) DEFAULT NULL,
			//  `cdate` timestamp NULL DEFAULT CURRENT_TIMESTAMP,
			//  PRIMARY KEY (`id`)
			//) ENGINE=InnoDB AUTO_INCREMENT=127 DEFAULT CHARSET=utf8;

			StringBuilder builder = new StringBuilder($"CREATE TABLE IF NOT EXISTS `{Scheme}`.`{TableName}` (");

			string columNames = string.Join(", ", properties.Select(p =>
				$"`{p.ColumnName}` {p.ValueType} {(p.PrimaryKey ? "AUTO_INCREMENT" : "")}"));
			builder.Append(columNames.Substring(0, columNames.Length));
			if (KeyProperties != null && KeyProperties.Any())
			{
				builder.Append(", PRIMARY KEY(");
				foreach (var p in KeyProperties)
				{
					builder.Append("`").Append(p.ColumnName).Append("`");
				}
				builder.Append(")");
			}
			builder.Append(") ENGINE=InnoDB  DEFAULT CHARSET=utf8");
			return builder.ToString();
		}

		/// <summary>
		///  
		/// </summary>
		/// <returns></returns>
		public string GetInsert(bool bulkInsert)
		{
			//Enumerate the entity properties
			//Identity property (if exists) has to be ignored
			IEnumerable<PropertyMetadata> properties = (IsIdentity ?
														BaseProperties.Where(p => !p.Name.Equals(IdentityProperty.Name, StringComparison.InvariantCultureIgnoreCase)) :
														BaseProperties).ToList();

			string columNames = string.Join(", ", properties.Select(p => $"`{p.ColumnName}`"));
			string values = string.Join(", ", properties.Select(p => $"@{p.Name}"));

			var sqlBuilder = new StringBuilder();
			sqlBuilder.AppendFormat("INSERT IGNORE INTO `{0}`.`{1}` {2} {3};",
									Scheme,
									TableName,
									string.IsNullOrEmpty(columNames) ? string.Empty : $"({columNames})",
									string.IsNullOrEmpty(values) ? string.Empty : $" VALUES ({values})");

			if (IsIdentity && !bulkInsert)
			{
				string query = $"SELECT max(`{IdentityProperty.ColumnName}`) as Id FROM `{Scheme}`.`{TableName}`;";
				sqlBuilder.Append(query);
			}

			return sqlBuilder.ToString();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public string GetUpdate()
		{
			var properties = BaseProperties.Where(p => !KeyProperties.Any(k => k.Name.Equals(p.Name, StringComparison.InvariantCultureIgnoreCase)));

			var sqlBuilder = new StringBuilder();
			sqlBuilder.AppendFormat("UPDATE `{0}`.`{1}` SET {2} WHERE {3}",
									Scheme,
									TableName,
									string.Join(", ", properties.Select(p => $"`{p.ColumnName}` = @{p.Name}")),
									string.Join(" AND ", KeyProperties.Select(p => $"`{p.ColumnName}` = @{p.Name}")));

			return sqlBuilder.ToString();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public string GetSelectAll()
		{
			return GetSelect(new { });
		}

		public string GetSelect(object filters)
		{
			//Projection function
			Func<PropertyMetadata, string> projectionFunction = p =>
			{
				if (!string.IsNullOrEmpty(p.Alias))
					return $"`{p.ColumnName}` AS `{p.Name}`";

				return $" `{p.ColumnName}`";
			};

			var sqlBuilder = new StringBuilder();
			sqlBuilder.AppendFormat("SELECT {0} FROM `{1}`.`{2}` ",
									string.Join(", ", BaseProperties.Select(projectionFunction)),
									Scheme,
									TableName);
			bool containsFilter = false;
			var s = filters as string;
			if (s != null)
			{
				string realFilters = s;
				if (!string.IsNullOrEmpty(realFilters))
				{
					sqlBuilder.AppendFormat(" WHERE {0} ", realFilters);
					containsFilter = true;
				}
			}
			else
			{
				//Properties of the dynamic filters object
				var filterProperties = filters.GetType().GetProperties().Select(p => p.Name);
				var properties = filterProperties as IList<string> ?? filterProperties.ToList();
				containsFilter = (properties.Any());

				if (containsFilter)
					sqlBuilder.AppendFormat(" WHERE {0} ", ToWhere(properties));
			}

			//Evaluates if this repository implements logical delete
			if (LogicalDelete)
			{
				sqlBuilder.AppendFormat(containsFilter ? " AND `{0}` != {1}" : " WHERE `{0}` != {1}",StatusProperty.Name,LogicalDeleteValue);
			}

			return sqlBuilder.ToString();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public string GetDelete()
		{
			var sqlBuilder = new StringBuilder();

			if (!LogicalDelete)
			{
				sqlBuilder.AppendFormat("DELETE FROM `{0}`.`{1}` WHERE {2}",
										Scheme,
										TableName,
										string.Join(" AND ", KeyProperties.Select(p => $"`{p.ColumnName}` = @{p.Name}")));
			}
			else
				sqlBuilder.AppendFormat("UPDATE `{0}`.`{1}` SET {2} WHERE {3}",
									Scheme,
									TableName,
					$"`{TableName}`.`{StatusProperty.ColumnName}` = {LogicalDeleteValue}",
									string.Join(" AND ", KeyProperties.Select(p => $"`{p.ColumnName}` = @{p.Name}")));

			return sqlBuilder.ToString();
		}

		#endregion

		#region Private utility

		/// <summary>
		/// 
		/// </summary>
		/// <param name="properties"></param>
		/// <returns></returns>
		private string ToWhere(IEnumerable<string> properties)
		{
			return string.Join(" AND ", properties.Select(p =>
			{
				var propertyMetadata = BaseProperties.FirstOrDefault(pm => pm.Name.Equals(p, StringComparison.InvariantCultureIgnoreCase));

				if (propertyMetadata != null)
					return $"`{propertyMetadata.ColumnName}` = @{propertyMetadata.Name}";

				return $"`{p}` = @{p}";
			}));
		}

		#endregion


		public string GetDeleteWhere(object filters)
		{
			var sqlBuilder = new StringBuilder();
			sqlBuilder.AppendFormat("DELETE FROM `{0}`.`{1}` ",
									Scheme,
									TableName);
			bool containsFilter = false;
			var s = filters as string;
			if (s != null)
			{
				string realFilters = s;
				if (!string.IsNullOrEmpty(realFilters))
				{
					sqlBuilder.AppendFormat(" WHERE {0} ", realFilters);
					containsFilter = true;
				}
			}
			else
			{
				//Properties of the dynamic filters object
				var filterProperties = filters.GetType().GetProperties().Select(p => p.Name);
				var properties = filterProperties as IList<string> ?? filterProperties.ToList();
				containsFilter = (properties.Any());

				if (containsFilter)
					sqlBuilder.AppendFormat(" WHERE {0} ", ToWhere(properties));
			}

			//Evaluates if this repository implements logical delete
			if (LogicalDelete)
			{
				sqlBuilder.AppendFormat(containsFilter ? " AND `{0}` != {1}" : " WHERE `{0}` != {1}",
					StatusProperty.Name,
					LogicalDeleteValue);
			}

			return sqlBuilder.ToString();
		}
	}
}
