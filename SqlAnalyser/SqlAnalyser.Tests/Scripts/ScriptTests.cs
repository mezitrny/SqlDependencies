using System.Linq;
using Microsoft.SqlServer.Management.SqlParser.Parser;
using Microsoft.SqlServer.Management.SqlParser.SqlCodeDom;
using NUnit.Framework;
using RoseByte.SqlAnalyser.SqlServer.Internal.Batches;
using RoseByte.SqlAnalyser.SqlServer.Internal.Identifiers;

namespace SqlAnalyser.Tests.Scripts
{
    [TestFixture]
    public class ScriptTests
    {
	    private static SqlBatch GetBatch(string sql) => Parser.Parse(sql).Script.Batches.FirstOrDefault();
	    
	    [Test]
        public void ShouldConstructInstance()
	    {
		    var sql = "SELECT * FROM dbo.MyTable";
			var batch = GetBatch(sql);

            var sut = new Script(batch, 4);
            
            Assert.That(sut.Order, Is.EqualTo(4));
            Assert.That(sut.Value, Is.EqualTo(batch));
            Assert.That(sut.Sql, Is.EqualTo(sql));
        }

		[Test]
		public void ShouldFindProcedureNameInCreateStatement()
	    {
		    var reference = new IdentifierInfo(IdentifierTypes.Procedure, "ThisOne", null, null, null);
			const string sql = "CREATE PROCEDURE ThisOne AS BEGIN EXECUTE someProc @someParameter='A' END";

		    var sut = new Script(GetBatch(sql), 0);

		    Assert.That(sut.Definitions.SingleOrDefault(), Is.EqualTo(reference));
	    }

	    [Test]
	    public void ShouldFindFunctionNameInCreateStatement()
	    {
		    var reference = new IdentifierInfo(IdentifierTypes.Function, "ThisOne", null, null, null);
			const string sql = "CREATE FUNCTION ThisOne() RETURNS INT AS BEGIN RETURN SomeFunc() END";

		    var sut = new Script(GetBatch(sql), 0);

		    Assert.That(sut.Definitions.SingleOrDefault(), Is.EqualTo(reference));
	    }

	    [Test]
	    public void ShouldFindTableReferenceInCreateTable()
	    {
		    var reference = new IdentifierInfo(IdentifierTypes.Table, "someTable", null, null, null);
		    var reference2 = new IdentifierInfo(IdentifierTypes.Table, "someTable2", null, null, null);
			const string sql = "CREATE TABLE someTable (Id INT)\nCREATE TABLE someTable2 (Id INT)";

		    var sut = new Script(GetBatch(sql), 0);

		    Assert.That(sut.Definitions.Count, Is.EqualTo(2));
		    Assert.That(sut.Definitions.First(), Is.EqualTo(reference));
		    Assert.That(sut.Definitions.Last(), Is.EqualTo(reference2));
	    }

	    [Test]
	    public void ShouldNotFindSelectQuery()
	    {
		    var reference = new IdentifierInfo(IdentifierTypes.Table, "someTable", null, null, null);
			const string sql = "CREATE TABLE someTable (Id INT)\nSELECT 1 FROM prd.Something";

		    var sut = new Script(GetBatch(sql), 0);

		    Assert.That(sut.Definitions.SingleOrDefault(), Is.EqualTo(reference));
	    }

	    [Test]
	    public void ShouldPassEveryDefinitionOnlyOnce()
	    {
		    var reference = new IdentifierInfo(IdentifierTypes.Table, "someTable", null, null, null);
		    const string sql = "CREATE TABLE someTable (Id INT)\nCREATE TABLE someTable (Id INT)";

		    var sut = new Script(GetBatch(sql), 0);

		    Assert.That(sut.Definitions.SingleOrDefault(), Is.EqualTo(reference));
	    }

	    [Test]
	    public void ShouldPassEveryReferenceOnlyOnce()
	    {
		    var reference = new IdentifierInfo(IdentifierTypes.Table, "someTable", null, null, null);
		    const string sql = "SELECT * FROM someTable;SELECT * FROM someTable";

		    var sut = new Script(GetBatch(sql), 0);

		    Assert.That(sut.References.SingleOrDefault(), Is.EqualTo(reference));
	    }

		[Test]
		public void ShouldFindTableReferenceInSelect()
		{
			var reference = new IdentifierInfo(IdentifierTypes.Table, "myTable", null, null, null);
			const string sql = "SELECT * FROM myTable";

			var sut = new Script(GetBatch(sql), 0);

			Assert.That(sut.References.SingleOrDefault(), Is.EqualTo(reference));
		}

		[Test]
		public void ShouldFindTableReferenceInInsert()
		{
			const string sql = "INSERT INTO someTable VALUES(1)";
			var reference = new IdentifierInfo(IdentifierTypes.Table, "someTable", null, null, null);

			var sut = new Script(GetBatch(sql), 0);

			Assert.That(sut.References.SingleOrDefault(), Is.EqualTo(reference));
		}

		[Test]
		public void ShouldFindTableReferenceInUpdate()
		{
			const string sql = "UPDATE someTable SET Id = 0";
			var reference = new IdentifierInfo(IdentifierTypes.Table, "someTable", null, null, null);

			var sut = new Script(GetBatch(sql), 0);

			Assert.That(sut.References.SingleOrDefault(), Is.EqualTo(reference));
		}

		[Test]
		public void ShouldFindTableReferenceInDelete()
		{
			const string sql = "DELETE FROM someTable WHERE Id = 0";
			var reference = new IdentifierInfo(IdentifierTypes.Table, "someTable", null, null, null);

			var sut = new Script(GetBatch(sql), 0);

			Assert.That(sut.References.SingleOrDefault(), Is.EqualTo(reference));
		}

		[Test]
		public void ShouldNotFindTableReferenceInCreate()
		{
			const string sql = "CREATE TABLE someTable (Id INT)";

			var sut = new Script(GetBatch(sql), 0);

			Assert.That(sut.References.Any(), Is.False);
		}

		[Test]
		public void ShouldFindTableReferenceInSubSelect()
		{
			const string sql = "SELECT (SELECT TOP 1 Id FROM subTable) AS Id FROM myTable";
			var reference = new IdentifierInfo(IdentifierTypes.Table, "subTable", null, null, null);

			var sut = new Script(GetBatch(sql), 0);

			Assert.That(sut.References.Count, Is.EqualTo(2));

			Assert.That(sut.References.ToList()[0], Is.EqualTo(reference));
		}

		[Test]
		public void ShouldFindTableReferenceInWhere()
		{
			const string sql = "SELECT 1 AS Id FROM myTable WHERE 1 = (SELECT TOP 1 Id FROM subTable)";
			var reference = new IdentifierInfo(IdentifierTypes.Table, "subTable", null, null, null);

			var sut = new Script(GetBatch(sql), 0);

			Assert.That(sut.References.Count, Is.EqualTo(2));

			Assert.That(sut.References.ToList()[1], Is.EqualTo(reference));
		}

		[Test]
		public void ShouldFindTableReferenceInJoin()
		{
			const string sql = "SELECT 1 AS Id FROM myTable AS mT JOIN subTable AS sT On mT.Id = sT.Id";
			var reference = new IdentifierInfo(IdentifierTypes.Table, "subTable", null, null, null);

			var sut = new Script(GetBatch(sql), 0);

			Assert.That(sut.References.Count, Is.EqualTo(2));

			Assert.That(sut.References.ToList()[1], Is.EqualTo(reference));
		}

		[Test]
		public void ShouldFillDefaultIdentifiers()
		{
			const string sql = "SELECT * FROM someServer.someDb.someSchema.myTable";
			var reference = new IdentifierInfo(IdentifierTypes.Table, "myTable", "someSchema", "someDb", "someServer");

			var sut = new Script(GetBatch(sql), 0);

			Assert.That(sut.References.SingleOrDefault(), Is.EqualTo(reference));
		}

		[Test]
		public void ShouldFindNamedTableReferenceInSelect()
		{
			const string sql = "SELECT * FROM dbo.myTable";
			var reference = new IdentifierInfo(IdentifierTypes.Table, "myTable", "dbo", null, null);

			var sut = new Script(GetBatch(sql), 0);

			Assert.That(sut.References.SingleOrDefault(), Is.EqualTo(reference));
		}

		[Test]
		public void ShouldFindNamedTableReferenceWithSchemaDatabase()
		{
			const string sql = "SELECT * FROM db1.dbo.myTable";
			var reference = new IdentifierInfo(IdentifierTypes.Table, "myTable", "dbo", "db1", null);

			var sut = new Script(GetBatch(sql), 0);

			Assert.That(sut.References.SingleOrDefault(), Is.EqualTo(reference));
		}

		[Test]
		public void ShouldFindNamedTableReferenceWithSchemaDatabaseServer()
		{
			const string sql = "SELECT * FROM server1.db1.dbo.myTable";
			var reference = new IdentifierInfo(IdentifierTypes.Table, "myTable", "dbo", "db1", "server1");

			var sut = new Script(GetBatch(sql), 0);

			Assert.That(sut.References.SingleOrDefault(), Is.EqualTo(reference));
		}

		[Test]
		public void ShouldFindNamedTableReferenceInSelectWithBrackets()
		{
			const string sql = "SELECT * FROM [server1].[db1].[dbo].[myTable]";

			var reference = new IdentifierInfo(IdentifierTypes.Table, "myTable", "dbo", "db1", "server1");

			var sut = new Script(GetBatch(sql), 0);

			Assert.That(sut.References.SingleOrDefault(), Is.EqualTo(reference));
		}

		[Test]
		public void ShouldFindProcReferenceInExec()
		{
			const string sql = "EXECUTE someProc @someParameter='A'";

			var reference = new IdentifierInfo(IdentifierTypes.Procedure, "someProc", null, null, null);

			var sut = new Script(GetBatch(sql), 0);

			Assert.That(sut.References.SingleOrDefault(), Is.EqualTo(reference));
		}

		[Test]
		public void ShouldFindProcReferenceInCreatedProc()
		{
			const string sql = "CREATE PROCEDURE ThisOne AS BEGIN EXECUTE someProc @someParameter='A' END";

			var reference = new IdentifierInfo(IdentifierTypes.Procedure, "someProc", null, null, null);

			var sut = new Script(GetBatch(sql), 0);

			Assert.That(sut.References.SingleOrDefault(), Is.EqualTo(reference));
		}

		[Test]
		public void ShouldFindProcReferenceInAlterProc()
		{
			const string sql = "ALTER PROCEDURE ThisOne AS BEGIN EXECUTE someProc @someParameter='A' END";

			var reference1 = new IdentifierInfo(IdentifierTypes.Procedure, "ThisOne", null, null, null);
			var reference2 = new IdentifierInfo(IdentifierTypes.Procedure, "someProc", null, null, null);

			var sut = new Script(GetBatch(sql), 0);

			Assert.That(sut.References.Count, Is.EqualTo(2));
			Assert.That(sut.References.ToList()[0], Is.EqualTo(reference1));
			Assert.That(sut.References.ToList()[1], Is.EqualTo(reference2));
		}

		[Test]
		public void ShouldNotFindProcReferenceInDeleteProc()
		{
			const string sql = "DROP PROCEDURE ThisOne";

			var sut = new Script(GetBatch(sql), 0);

			Assert.That(sut.References.Any(), Is.False);
		}

		[Test]
		public void ShouldFindFuncReferenceInCreatedProc()
		{
			const string sql = "CREATE FUNCTION ThisOne() RETURNS INT AS BEGIN RETURN SomeFunc() END";

			var reference = new IdentifierInfo(IdentifierTypes.Function, "SomeFunc", null, null, null);

			var sut = new Script(GetBatch(sql), 0);

			Assert.That(sut.References.SingleOrDefault(), Is.EqualTo(reference));
		}

		[Test]
		public void ShouldFindFuncReferenceInAlterProc()
		{
			const string sql = "ALTER FUNCTION ThisOne() RETURNS INT AS BEGIN RETURN SomeFunc() END";

			var reference1 = new IdentifierInfo(IdentifierTypes.Function, "ThisOne", null, null, null);
			var reference2 = new IdentifierInfo(IdentifierTypes.Function, "SomeFunc", null, null, null);

			var sut = new Script(GetBatch(sql), 0);

			Assert.That(sut.References.Count, Is.EqualTo(2));
			Assert.That(sut.References.ToList()[0], Is.EqualTo(reference1));
			Assert.That(sut.References.ToList()[1], Is.EqualTo(reference2));
		}

		[Test]
		public void ShouldFindFuncReferenceTableLikeCall()
		{
			const string sql = "ALTER FUNCTION ThisOne() RETURNS INT AS BEGIN RETURN SELECT Id FROM SomeFunc(4) END";

			var reference1 = new IdentifierInfo(IdentifierTypes.Function, "ThisOne", null, null, null);
			var reference2 = new IdentifierInfo(IdentifierTypes.Function, "SomeFunc", null, null, null);

			var sut = new Script(GetBatch(sql), 0);

			Assert.That(sut.References.Count, Is.EqualTo(2));
			Assert.That(sut.References.ToList()[0], Is.EqualTo(reference1));
			Assert.That(sut.References.ToList()[1], Is.EqualTo(reference2));
		}

		[Test]
		public void ShouldFindFuncReferenceCrossApply()
		{
			const string sql = "SELECT * FROM dbo.SomeTable CROSS APPLY dbo.SomeFunc(4)";

			var reference = new IdentifierInfo(IdentifierTypes.Function, "SomeFunc", "dbo", null, null);

			var sut = new Script(GetBatch(sql), 0);

			Assert.That(sut.References.Last(), Is.EqualTo(reference));
		}

		[Test]
		public void ShouldNotFindFuncReferenceInDeleteFunc()
		{
			const string sql = "DROP FUNCTION ThisOne";

			var sut = new Script(GetBatch(sql), 0);

			Assert.That(sut.References.Any(), Is.False);
		}

		[Test]
		public void ShouldNotFindCteTable()
		{
			const string sql = "WITH cte (Id) AS (SELECT 1 AS Id) " +
							   "SELECT * FROM cte _a LEFT JOIN cte _b ON _a.Id = _b.Id";

			var sut = new Script(GetBatch(sql), 0);

			Assert.That(sut.References.Any(), Is.False);
		}
	}
}