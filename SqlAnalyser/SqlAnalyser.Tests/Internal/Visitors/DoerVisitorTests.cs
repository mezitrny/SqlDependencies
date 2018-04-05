using System.Collections.Generic;
using System.Linq;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using NUnit.Framework;
using SqlAnalyser.Internal;
using SqlAnalyser.Internal.Identifiers;

namespace SqlAnalyser.Tests.Internal.Visitors
{
    [TestFixture]
    public class NameVisitorTests
    {
	    private static List<IdentifierInfo> GetReferences(string sql, string schema = null, string database = null, 
		    string server = null)
	    {
		    var batches = SqlParser.Parse(sql, SqlVersion.Sql100, out var errors);
			
		    var sut = new DoerVisitor();

		    return batches.SelectMany(x => sut.GetReferences(x, schema, database, server)).ToList();
	    }
	    
	    [Test]
        public void ShouldFindProcedureNameInCreateStatement()
        {
            const string sql = "CREATE PROCEDURE ThisOne AS BEGIN EXECUTE someProc @someParameter='A' END";            			
            			
			var reference = new IdentifierInfo(BatchTypes.Procedure, "ThisOne");

			var result = GetReferences(sql);
			
			Assert.That(result.SingleOrDefault(), Is.EqualTo(reference));
        }
	    
	    [Test]
	    public void ShouldFindProcedureNameInAlterStatement()
	    {
		    const string sql = "ALTER PROCEDURE ThisOne AS BEGIN EXECUTE someProc @someParameter='A' END";            			
            			
		    var reference = new IdentifierInfo(BatchTypes.Procedure, "ThisOne");

		    var result = GetReferences(sql);
			
		    Assert.That(result.SingleOrDefault(), Is.EqualTo(reference));
	    }
	    
	    [Test]
	    public void ShouldFindFunctionNameInAlterStatement()
	    {
		    const string sql = "ALTER FUNCTION ThisOne() RETURNS INT AS BEGIN RETURN SomeFunc() END";            			
            			
		    var reference = new IdentifierInfo(BatchTypes.Procedure, "ThisOne");

		    var result = GetReferences(sql);
			
		    Assert.That(result.SingleOrDefault(), Is.EqualTo(reference));
	    }
	    
	    [Test]
	    public void ShouldFindFunctionNameInCreateStatement()
	    {
		    const string sql = "CREATE FUNCTION ThisOne() RETURNS INT AS BEGIN RETURN SomeFunc() END";            			
            			
		    var reference = new IdentifierInfo(BatchTypes.Procedure, "ThisOne");

		    var result = GetReferences(sql);
			
		    Assert.That(result.SingleOrDefault(), Is.EqualTo(reference));
	    }
	    
	    [Test]
	    public void ShouldFindTableReferenceInCreateTable()
	    {
		    const string sql = "CREATE TABLE someTable (Id INT)\nCREATE TABLE someTable2 (Id INT)";

		    var result = GetReferences(sql);
		    var reference = new IdentifierInfo(BatchTypes.Table, "someTable");
		    var reference2 = new IdentifierInfo(BatchTypes.Table, "someTable2");
		    
		    Assert.That(result.Count, Is.EqualTo(2));
		    Assert.That(result.First(), Is.EqualTo(reference));
		    Assert.That(result.Last(), Is.EqualTo(reference2));
	    }
	    
	    [Test]
	    public void ShouldFindTableReferenceInAlterTable()
	    {
		    const string sql = "ALTER TABLE someTable ALTER COLUMN One BIT";

		    var result = GetReferences(sql);
		    var reference = new IdentifierInfo(BatchTypes.Table, "someTable");
		    
		    Assert.That(result.SingleOrDefault(), Is.EqualTo(reference));
	    }
	    
	    [Test]
	    public void ShouldNotFindSelectQuery()
	    {
		    const string sql = "CREATE TABLE someTable (Id INT)\nSELECT 1 FROM prd.Something";

		    var result = GetReferences(sql);
		    var reference = new IdentifierInfo(BatchTypes.Table, "someTable");
		    
		    Assert.That(result.SingleOrDefault(), Is.EqualTo(reference));
	    }
    }
}