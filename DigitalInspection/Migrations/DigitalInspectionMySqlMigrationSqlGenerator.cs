using MySql.Data.Entity;
using System.Data.Entity.Migrations.Model;
using System.Data.Entity.Migrations.Sql;


namespace DigitalInspection.Migrations
{
	//http://stackoverflow.com/a/23656678/2831961
	public class DigitalInspectionMySqlMigrationSqlGenerator: MySqlMigrationSqlGenerator
	{
		protected override MigrationStatement Generate(AddForeignKeyOperation addForeignKeyOperation)
		{
			addForeignKeyOperation.PrincipalTable = addForeignKeyOperation.PrincipalTable.Replace("dbo.", "");
			addForeignKeyOperation.DependentTable = addForeignKeyOperation.DependentTable.Replace("dbo.", "");
			MigrationStatement ms = base.Generate(addForeignKeyOperation);
			return ms;
		}
	}
}