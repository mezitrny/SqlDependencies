using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using NUnit.Framework;

namespace SqlAnalyser.Tests
{
	[TestFixture()]
	public class SqlParserTests
	{
		public class ReferenceScanner : TSqlFragmentVisitor
		{
			private readonly List<string> _references = new List<string>();
			
			public IEnumerable<string> GetReferences(TSqlBatch batch)
			{
				Visit(batch);

				return _references;
			}
			
			public override void Visit(ProcedureReferenceName node)
			{
				_references.Add(string.Join(".", node.ScriptTokenStream.Select(x => x.Text)));
				
				base.ExplicitVisit(node);
			}
			
			public override void ExplicitVisit(ColumnReferenceExpression node)
			{
				_references.Add(string.Join(".", node.ScriptTokenStream.Select(x => x.Text)));
				
				base.ExplicitVisit(node);
			}
			
			public override void ExplicitVisit(XmlDataTypeReference node)
			{
				_references.Add(string.Join(".", node.ScriptTokenStream.Select(x => x.Text)));
				
				base.ExplicitVisit(node);
			}
			
			public override void ExplicitVisit(VariableTableReference node)
			{
				_references.Add(string.Join(".", node.ScriptTokenStream.Select(x => x.Text)));
				
				base.ExplicitVisit(node);
			}
			
			public override void ExplicitVisit(VariableReference node)
			{
				_references.Add(string.Join(".", node.ScriptTokenStream.Select(x => x.Text)));
				
				base.ExplicitVisit(node);
			}
			
			public override void ExplicitVisit(VariableMethodCallTableReference node)
			{
				_references.Add(string.Join(".", node.ScriptTokenStream.Select(x => x.Text)));
				
				base.ExplicitVisit(node);
			}
			
			public override void ExplicitVisit(UserDataTypeReference node)
			{
				_references.Add(string.Join(".", node.ScriptTokenStream.Select(x => x.Text)));
				
				base.ExplicitVisit(node);
			}
			
			public override void ExplicitVisit(UnpivotedTableReference node)
			{
				_references.Add(string.Join(".", node.ScriptTokenStream.Select(x => x.Text)));
				
				base.ExplicitVisit(node);
			}
			
			public override void ExplicitVisit(SqlDataTypeReference node)
			{
				_references.Add(string.Join(".", node.ScriptTokenStream.Select(x => x.Text)));
				
				base.ExplicitVisit(node);
			}
			
			public override void ExplicitVisit(SemanticTableReference node)
			{
				_references.Add(string.Join(".", node.ScriptTokenStream.Select(x => x.Text)));
				
				base.ExplicitVisit(node);
			}
			
			public override void ExplicitVisit(SchemaObjectFunctionTableReference node)
			{
				_references.Add(string.Join(".", node.ScriptTokenStream.Select(x => x.Text)));
				
				base.ExplicitVisit(node);
			}
			
			public override void ExplicitVisit(ProcedureReference node)
			{
				_references.Add(string.Join(".", node.ScriptTokenStream.Select(x => x.Text)));
				
				base.ExplicitVisit(node);
			}
			
			public override void ExplicitVisit(ParameterizedDataTypeReference node)
			{
				_references.Add(string.Join(".", node.ScriptTokenStream.Select(x => x.Text)));
				
				base.ExplicitVisit(node);
			}
			
			public override void ExplicitVisit(OpenXmlTableReference node)
			{
				_references.Add(string.Join(".", node.ScriptTokenStream.Select(x => x.Text)));
				
				base.ExplicitVisit(node);
			}
			
			public override void ExplicitVisit(OpenRowsetTableReference node)
			{
				_references.Add(string.Join(".", node.ScriptTokenStream.Select(x => x.Text)));
				
				base.ExplicitVisit(node);
			}
			
			public override void ExplicitVisit(OpenQueryTableReference node)
			{
				_references.Add(string.Join(".", node.ScriptTokenStream.Select(x => x.Text)));
				
				base.ExplicitVisit(node);
			}
			
			public override void ExplicitVisit(OpenJsonTableReference node)
			{
				_references.Add(string.Join(".", node.ScriptTokenStream.Select(x => x.Text)));
				
				base.ExplicitVisit(node);
			}
			
			public override void ExplicitVisit(OdbcQualifiedJoinTableReference node)
			{
				_references.Add(string.Join(".", node.ScriptTokenStream.Select(x => x.Text)));
				
				base.ExplicitVisit(node);
			}
			
			public override void Visit(NamedTableReference node)
			{
				_references.Add(string.Join(".", node.ScriptTokenStream.Select(x => x.Text)));
				
				base.Visit(node);
			}
			
			public override void ExplicitVisit(JoinTableReference node)
			{
				_references.Add(string.Join(".", node.ScriptTokenStream.Select(x => x.Text)));
				
				base.ExplicitVisit(node);
			}
			
			public override void ExplicitVisit(JoinParenthesisTableReference node)
			{
				_references.Add(string.Join(".", node.ScriptTokenStream.Select(x => x.Text)));
				
				base.ExplicitVisit(node);
			}
			
			public override void ExplicitVisit(GlobalFunctionTableReference node)
			{
				_references.Add(string.Join(".", node.ScriptTokenStream.Select(x => x.Text)));
				
				base.ExplicitVisit(node);
			}
			
			public override void ExplicitVisit(FullTextTableReference node)
			{
				_references.Add(string.Join(".", node.ScriptTokenStream.Select(x => x.Text)));
				
				base.ExplicitVisit(node);
			}
			
			public override void ExplicitVisit(DataTypeReference node)
			{
				_references.Add(string.Join(".", node.ScriptTokenStream.Select(x => x.Text)));
				
				base.ExplicitVisit(node);
			}
			
			public override void ExplicitVisit(DataModificationTableReference node)
			{
				_references.Add(string.Join(".", node.ScriptTokenStream.Select(x => x.Text)));
				
				base.ExplicitVisit(node);
			}
			
			public override void ExplicitVisit(ChangeTableVersionTableReference node)
			{
				_references.Add(string.Join(".", node.ScriptTokenStream.Select(x => x.Text)));
				
				base.ExplicitVisit(node);
			}
			
			public override void ExplicitVisit(ChangeTableChangesTableReference node)
			{
				_references.Add(string.Join(".", node.ScriptTokenStream.Select(x => x.Text)));
				
				base.ExplicitVisit(node);
			}
			
			public override void ExplicitVisit(BuiltInFunctionTableReference node)
			{
				_references.Add(string.Join(".", node.ScriptTokenStream.Select(x => x.Text)));
				
				base.ExplicitVisit(node);
			}
			
			public override void ExplicitVisit(AuditActionGroupReference node)
			{
				_references.Add(string.Join(".", node.ScriptTokenStream.Select(x => x.Text)));
				
				base.ExplicitVisit(node);
			}
			
			public override void ExplicitVisit(ExecutableProcedureReference node)
			{
				_references.Add(string.Join(".", node.ScriptTokenStream.Select(x => x.Text)));
				
				base.ExplicitVisit(node);
			}
			
			public override void ExplicitVisit(AdHocTableReference node)
			{
				_references.Add(string.Join(".", node.ScriptTokenStream.Select(x => x.Text)));
				
				base.ExplicitVisit(node);
			}
			
			public override void ExplicitVisit(TableReferenceWithAliasAndColumns node)
			{
				_references.Add(string.Join(".", node.ScriptTokenStream.Select(x => x.Text)));
				
				base.ExplicitVisit(node);
			}
			
			public override void ExplicitVisit(TableReferenceWithAlias node)
			{
				_references.Add(string.Join(".", node.ScriptTokenStream.Select(x => x.Text)));
				
				base.ExplicitVisit(node);
			}
			
			public override void Visit(TableReference node)
			{
				_references.Add(string.Join(".", node.ScriptTokenStream.Select(x => x.Text)));
				
				base.ExplicitVisit(node);
			}
		}
		
		[Test]
		public void ShouldFindBracketedQualifiedTableReference()
		{
			var sql = "SELECT tbl.Something AS First --this is first\n, 'ab''d''c' AS 'text' /* this is text */ FROM [dbo].[table] AS tbl\n" +
			          "SELECT tbl.Something AS First --this is first\n, 'ab''d''c' AS 'text' /* this is text */ FROM [dbo].[table] AS tbl";
			
			var scriptFragment = Parse(sql, SqlVersion.Sql100, true, out var errors);

			if (!(scriptFragment is TSqlScript tsqlScriptFragment))
				return;
			
			var references = new ReferenceScanner().GetReferences(tsqlScriptFragment.Batches.First());

			var options = new SqlScriptGeneratorOptions { SqlVersion = SqlVersion.Sql100, KeywordCasing = KeywordCasing.PascalCase };

			foreach (var batch in tsqlScriptFragment.Batches)
			{
				Console.WriteLine("---------------------");
				var batchText = ToScript(batch, options);
				Console.WriteLine(batchText);
			}
		}
		
		[Test]
		public void Test()
		{
			Main(new string[]{});
		}

		static void Main(string[] args)
		{
			string sql = @"
/*
GO
*/ 
SELECT * FROM [table]

SELECT 'GO' AS [GO] FROM [table]

--GO

SELECT * FROM [table]

/*GO*/

SELECT * FROM [table]
SELECT * FROM [table]

GO

SELECT * FROM [table]";

			string[] errors;
			var scriptFragment = Parse(sql, SqlVersion.Sql100, true, out errors);
			if (errors != null)
			{
				foreach (string error in errors)
				{
					Console.WriteLine(error);
					return;
				}
			}

			TSqlScript tsqlScriptFragment = scriptFragment as TSqlScript;
			if (tsqlScriptFragment == null)
				return;

			var options = new SqlScriptGeneratorOptions { SqlVersion = SqlVersion.Sql100, KeywordCasing = KeywordCasing.PascalCase };

			foreach (TSqlBatch batch in tsqlScriptFragment.Batches)
			{
				Console.WriteLine("---------------------");
				string batchText = ToScript(batch, options);
				Console.WriteLine(batchText);
			}
		}

		public static TSqlParser GetParser(SqlVersion level, bool quotedIdentifiers)
		{
			switch (level)
			{
				case SqlVersion.Sql80:
					return new TSql80Parser(quotedIdentifiers);
				case SqlVersion.Sql90:
					return new TSql90Parser(quotedIdentifiers);
				case SqlVersion.Sql100:
					return new TSql100Parser(quotedIdentifiers);
				case SqlVersion.Sql110:
					return new TSql110Parser(quotedIdentifiers);
				case SqlVersion.Sql120:
					return new TSql120Parser(quotedIdentifiers);
				case SqlVersion.Sql130:
					return new TSql130Parser(quotedIdentifiers);
				case SqlVersion.Sql140:
					return new TSql140Parser(quotedIdentifiers);
				default:
					throw new ArgumentOutOfRangeException("level");
			}
		}

		public static TSqlFragment Parse(string sql, SqlVersion level, bool quotedIndentifiers, out string[] errors)
		{
			errors = null;
			if (string.IsNullOrWhiteSpace(sql))
				return null;
			sql = sql.Trim();
			TSqlFragment scriptFragment;
			IList<ParseError> errorlist;
			using (var sr = new StringReader(sql))
			{
				scriptFragment = GetParser(level, quotedIndentifiers).Parse(sr, out errorlist);
			}
			if (errorlist != null && errorlist.Count > 0)
			{
				errors = errorlist.Select(e => string.Format("Column {0}, Message {1}, Line {2}, Offset {3}",
					                               e.Column, e.Message, e.Line, e.Offset) +
				                               Environment.NewLine + e.Message).ToArray();
				return null;
			}
			return scriptFragment;
		}

		public static SqlScriptGenerator GetScripter(SqlScriptGeneratorOptions options)
		{
			if (options == null)
				return null;
			SqlScriptGenerator generator;
			switch (options.SqlVersion)
			{
				case SqlVersion.Sql80:
					generator = new Sql80ScriptGenerator(options);
					break;
				case SqlVersion.Sql90:
					generator = new Sql90ScriptGenerator(options);
					break;
				case SqlVersion.Sql100:
					generator = new Sql100ScriptGenerator(options);
					break;
				case SqlVersion.Sql110:
					generator = new Sql100ScriptGenerator(options);
					break;
				case SqlVersion.Sql120:
					generator = new Sql100ScriptGenerator(options);
					break;
				case SqlVersion.Sql130:
					generator = new Sql100ScriptGenerator(options);
					break;
				case SqlVersion.Sql140:
					generator = new Sql100ScriptGenerator(options);
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
			return generator;
		}

		public static string ToScript(TSqlFragment scriptFragment, SqlScriptGeneratorOptions options)
		{
			var scripter = GetScripter(options);
			if (scripter == null)
				return string.Empty;
			string script;
			scripter.GenerateScript(scriptFragment, out script);
			return script;
		}
	}
}