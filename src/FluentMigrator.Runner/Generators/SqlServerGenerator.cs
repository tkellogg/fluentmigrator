﻿using System;
using System.Data;
using System.Text;
using FluentMigrator.Expressions;
using FluentMigrator.Model;

namespace FluentMigrator.Runner.Generators
{
	public class SqlServerGenerator : GeneratorBase
	{
		public const int AnsiStringCapacity = 8000;
		public const int AnsiTextCapacity = 2147483647;
		public const int UnicodeStringCapacity = 4000;
		public const int UnicodeTextCapacity = 1073741823;
		public const int ImageCapacity = 2147483647;
		public const int DecimalCapacity = 19;
		public const int XmlCapacity = 1073741823;

		protected override void SetupTypeMaps()
		{
			SetTypeMap(DbType.AnsiStringFixedLength, "CHAR(255)");
			SetTypeMap(DbType.AnsiStringFixedLength, "CHAR($size)", AnsiStringCapacity);
			SetTypeMap(DbType.AnsiString, "VARCHAR(255)");
			SetTypeMap(DbType.AnsiString, "VARCHAR($size)", AnsiStringCapacity);
			SetTypeMap(DbType.AnsiString, "TEXT", AnsiTextCapacity);
			SetTypeMap(DbType.Binary, "VARBINARY(8000)");
			SetTypeMap(DbType.Binary, "VARBINARY($size)", AnsiStringCapacity);
			SetTypeMap(DbType.Binary, "IMAGE", ImageCapacity);
			SetTypeMap(DbType.Boolean, "BIT");
			SetTypeMap(DbType.Byte, "TINYINT");
			SetTypeMap(DbType.Currency, "MONEY");
			SetTypeMap(DbType.Date, "DATETIME");
			SetTypeMap(DbType.DateTime, "DATETIME");
			SetTypeMap(DbType.Decimal, "DECIMAL(19,5)");
			SetTypeMap(DbType.Decimal, "DECIMAL(19,$size)", DecimalCapacity);
			SetTypeMap(DbType.Double, "DOUBLE PRECISION");
			SetTypeMap(DbType.Guid, "UNIQUEIDENTIFIER");
			SetTypeMap(DbType.Int16, "SMALLINT");
			SetTypeMap(DbType.Int32, "INT");
			SetTypeMap(DbType.Int64, "BIGINT");
			SetTypeMap(DbType.Single, "REAL");
			SetTypeMap(DbType.StringFixedLength, "NCHAR(255)");
			SetTypeMap(DbType.StringFixedLength, "NCHAR($size)", UnicodeStringCapacity);
			SetTypeMap(DbType.String, "NVARCHAR(255)");
			SetTypeMap(DbType.String, "NVARCHAR($size)", UnicodeStringCapacity);
			SetTypeMap(DbType.String, "NTEXT", UnicodeTextCapacity);
			SetTypeMap(DbType.Time, "DATETIME");
			SetTypeMap(DbType.Xml, "XML", XmlCapacity);
		}

		public override string Generate(CreateTableExpression expression)
		{
			return FormatExpression("CREATE TABLE [{0}] ({1})", expression.TableName, GetColumnDDL(expression.Columns));
		}

		public override string Generate(CreateColumnExpression expression)
		{
		    return FormatExpression("ALTER TABLE [{0}] ADD {1}", expression.TableName, GenerateDDLForColumn(expression.Column));
		}

		public override string Generate(DeleteTableExpression expression)
		{
			return FormatExpression("DROP TABLE [{0}]", expression.TableName);
		}

		public override string Generate(DeleteColumnExpression expression)
		{

		    return FormatExpression("ALTER TABLE [{0}] DROP COLUMN {1}", expression.TableName, expression.ColumnName);            
		}

		public override string Generate(CreateForeignKeyExpression expression)
		{
			/*ALTER TABLE <table_name>
				ADD <constraint_name> FOREIGN KEY
					(<column_name1> ,
					<column_name2> )
				REFERENCES <table_name>
					(<column_name1> ,
					<column_name2>)*/
 
			throw new NotImplementedException();
		}

		public override string Generate(DeleteForeignKeyExpression expression)
		{
			/*ALTER TABLE <table_name>
				DROP FOREIGN KEY <foreignkey_name>*/

			throw new NotImplementedException();
		}

		public override string Generate(CreateIndexExpression expression)
		{
		    var result = new StringBuilder("CREATE");
            if (expression.Index.IsUnique)            
                result.Append(" UNIQUE");

            if (expression.Index.IsClustered)            
                result.Append(" CLUSTERED");            
            else            
                result.Append(" NONCLUSTERED");            

		    result.Append(" INDEX {0} ON {1} (");

			bool first = true;
            foreach (IndexColumnDefinition column in expression.Index.Columns)
            {
				if (first)	
					first = false;				
				else			
					result.Append(",");

                result.Append(column.Name);
                if (column.Direction == Direction.Ascending)
                {
                    result.Append(" ASC");
                }
                else
                {
                    result.Append(" DESC");
                }
            }
			result.Append(")");

		    return FormatExpression(result.ToString(), expression.Index.Name, expression.Index.TableName);
		}

		public override string Generate(DeleteIndexExpression expression)
		{
			throw new NotImplementedException();
		}

		public override string Generate(RenameTableExpression expression)
		{
		    return FormatExpression("sp_rename [{0}], [{1}]", expression.OldName, expression.NewName);
		}

		public override string Generate(RenameColumnExpression expression)
		{
		    return FormatExpression("sp_rename '[{0}].[{1}]', [{2}]", expression.TableName, expression.OldName, expression.NewName);
		}

		public string FormatExpression(string template, params object[] args)
		{
			return String.Format(template, args);
		}
	}
}