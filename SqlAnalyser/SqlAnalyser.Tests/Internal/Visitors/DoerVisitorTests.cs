using System.Collections.Generic;
using System.Linq;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using NUnit.Framework;
using SqlAnalyser.Internal;

namespace SqlAnalyser.Tests
{
    [TestFixture]
    public class NameVisitorTests
    {
	    private static List<IdentifierInfo> GetReferences(string sql, string schema = null, string database = null, 
		    string server = null)
	    {
		    var batches = SqlParser.Parse(sql, SqlVersion.Sql100, out var errors);
			
		    var sut = new DoerVisitor(schema, database, server);

		    return batches.SelectMany(x => sut.GetReferences(x)).ToList();
	    }
	    
	    [Test]
        public void ShouldFindProcedureName()
        {
            const string sql = "CREATE PROCEDURE ThisOne AS BEGIN EXECUTE someProc @someParameter='A' END";            			
            			
			var reference = new IdentifierInfo(BatchTypes.Procedure, "someProc");

			var result = GetReferences(sql);
			
			Assert.That(result.SingleOrDefault(), Is.EqualTo(reference));
        }
	    
	    [Test]
	    public void ShouldNotFindTableReferenceInCreate()
	    {
		    const string sql = "CREATE TABLE someTable (Id INT)";

		    var result = GetReferences(sql);
		    var reference = new IdentifierInfo(BatchTypes.Procedure, "someProc");
		    
		    Assert.That(result.SingleOrDefault(), Is.EqualTo(reference));
	    }
    }
}