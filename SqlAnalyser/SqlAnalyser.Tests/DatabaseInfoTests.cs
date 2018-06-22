using System.Linq;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using NUnit.Framework;
using RoseByte.SqlAnalyser.SqlServer;

namespace SqlAnalyser.Tests
{
    [TestFixture]
    public class DatabaseInfoTests
    {
        [Test]
        public void ShouldCreateInstance()
        {
            var sut = new DatabaseInfo(
                "Data Source=ServerName;Initial Catalog=DataBaseName;Integrated Security=SSPI;",
                SqlVersion.Sql140, "dbo");
            
            Assert.That(sut.DatabaseName, Is.EqualTo("DataBaseName"));
            Assert.That(sut.ServerName, Is.EqualTo("ServerName"));
            Assert.That(sut.DefaultSchema, Is.EqualTo("dbo"));
            Assert.That(sut.Version, Is.EqualTo(SqlVersion.Sql140));
        }
        
        [Test, Ignore("Wait for working SQL Server DB")]
        public void ShouldReadVersionAndSchemaFromDatabse()
        {
            var sut = new DatabaseInfo(
                "Data Source=ServerName;Initial Catalog=DataBaseName;Integrated Security=SSPI;");
            
            Assert.That(sut.DatabaseName, Is.EqualTo("DataBaseName"));
            Assert.That(sut.ServerName, Is.EqualTo("ServerName"));
            Assert.That(sut.DefaultSchema, Is.EqualTo("dbo"));
            Assert.That(sut.Version, Is.EqualTo(SqlVersion.Sql140));
        }
        
        [Test, Ignore("Wait for working SQL Server DB")]
        public void ShouldFetchProcedure()
        {
            var sut = new DatabaseInfo(
                "Data Source=ServerName;Initial Catalog=DataBaseName;Integrated Security=SSPI;");
            
            Assert.That(sut.DatabaseName, Is.EqualTo("DataBaseName"));
            Assert.That(sut.ServerName, Is.EqualTo("ServerName"));
            Assert.That(sut.DefaultSchema, Is.EqualTo("dbo"));
            Assert.That(sut.Version, Is.EqualTo(SqlVersion.Sql140));
        }
        
        [Test, Ignore("Wait for working SQL Server DB")]
        public void ShouldFetchFunction()
        {
            var sut = new DatabaseInfo(
                "Data Source=ServerName;Initial Catalog=DataBaseName;Integrated Security=SSPI;");
            
            Assert.That(sut.DatabaseName, Is.EqualTo("DataBaseName"));
            Assert.That(sut.ServerName, Is.EqualTo("ServerName"));
            Assert.That(sut.DefaultSchema, Is.EqualTo("dbo"));
            Assert.That(sut.Version, Is.EqualTo(SqlVersion.Sql140));
        }
        
        [Test, Ignore("Wait for working SQL Server DB")]
        public void ShouldFetchView()
        {
            var sut = new DatabaseInfo(
                "Data Source=ServerName;Initial Catalog=DataBaseName;Integrated Security=SSPI;");
            
            Assert.That(sut.DatabaseName, Is.EqualTo("DataBaseName"));
            Assert.That(sut.ServerName, Is.EqualTo("ServerName"));
            Assert.That(sut.DefaultSchema, Is.EqualTo("dbo"));
            Assert.That(sut.Version, Is.EqualTo(SqlVersion.Sql140));
        }
        
        [Test, Ignore("Wait for table scripting implementation")]
        public void ShouldFetchTable()
        {
            var sut = new DatabaseInfo(
                "Data Source=ServerName;Initial Catalog=DataBaseName;Integrated Security=SSPI;");
            
            Assert.That(sut.DatabaseName, Is.EqualTo("DataBaseName"));
            Assert.That(sut.ServerName, Is.EqualTo("ServerName"));
            Assert.That(sut.DefaultSchema, Is.EqualTo("dbo"));
            Assert.That(sut.Version, Is.EqualTo(SqlVersion.Sql140));
        }
        
        [Test]
        public void ShouldAnalyzeScript()
        {
            var sql = "SELECT 1";
            
            var sut = new DatabaseInfo(
                "Data Source=ServerName;Initial Catalog=DataBaseName;Integrated Security=SSPI;",
                SqlVersion.Sql140, "dbo");

            var result = sut.AnalyseScript(sql);
            
            Assert.That(result.Sql, Is.EqualTo(sql));
            Assert.That(result.DefaultDatabase, Is.EqualTo(sut.DatabaseName));
            Assert.That(result.DefaultServer, Is.EqualTo(sut.ServerName));
            Assert.That(result.DefaultSchema, Is.EqualTo(sut.DefaultSchema));
            Assert.That(result.Version, Is.EqualTo(sut.Version));
        }
        
        [Test]
        public void ShouldAnalyzeScriptReferences()
        {
            var sql = "CREATE FUNCTION dbo.TestFunction2() RETURNS INT AS BEGIN RETURN (SELECT dbo.TestFunction1()) END";
            
            var sut = new DatabaseInfo(
                "Data Source=ServerName;Initial Catalog=DataBaseName;Integrated Security=SSPI;",
                SqlVersion.Sql140, "dbo");

            var result = sut.AnalyseScript(sql);

            var firstBatch = result.Batches.First();
            
            Assert.That(firstBatch.References.Count(), Is.EqualTo(1));
            Assert.That(firstBatch.References.First().Schema.DefaultName, Is.EqualTo("dbo"));
            Assert.That(firstBatch.References.First().Schema.Name, Is.EqualTo("dbo"));
        }
        
        [Test]
        public void ShouldAnalyzeScriptReferencesWithDatabase()
        {
            var sql = "CREATE FUNCTION dbo.TestFunction2() RETURNS INT AS BEGIN RETURN (SELECT myBase.dbo.TestFunction1()) END";
            
            var sut = new DatabaseInfo(
                "Data Source=ServerName;Initial Catalog=DataBaseName;Integrated Security=SSPI;",
                SqlVersion.Sql140, "dbo");

            var result = sut.AnalyseScript(sql);

            var firstBatch = result.Batches.First();
            
            Assert.That(firstBatch.References.Count(), Is.EqualTo(1));
            Assert.That(firstBatch.References.First().Schema.DefaultName, Is.EqualTo("dbo"));
            Assert.That(firstBatch.References.First().Schema.Name, Is.EqualTo("dbo"));
            Assert.That(firstBatch.References.First().Database.DefaultName, Is.EqualTo("DataBaseName"));
            Assert.That(firstBatch.References.First().Database.Name, Is.EqualTo("myBase"));
        }
        
        [Test]
        public void ShouldAnalyzeScriptReferencesWithServer()
        {
            var sql = "CREATE FUNCTION dbo.TestFunction2() RETURNS INT AS BEGIN RETURN (SELECT myserver.myBase.dbo.TestFunction1()) END";
            
            var sut = new DatabaseInfo(
                "Data Source=ServerName;Initial Catalog=DataBaseName;Integrated Security=SSPI;",
                SqlVersion.Sql140, "dbo");

            var result = sut.AnalyseScript(sql);

            var firstBatch = result.Batches.First();
            
            Assert.That(firstBatch.References.Count(), Is.EqualTo(1));
            Assert.That(firstBatch.References.First().Schema.DefaultName, Is.EqualTo("dbo"));
            Assert.That(firstBatch.References.First().Schema.Name, Is.EqualTo("dbo"));
            Assert.That(firstBatch.References.First().Database.DefaultName, Is.EqualTo("DataBaseName"));
            Assert.That(firstBatch.References.First().Database.Name, Is.EqualTo("myBase"));
            Assert.That(firstBatch.References.First().Server.DefaultName, Is.EqualTo("ServerName"));
            Assert.That(firstBatch.References.First().Server.Name, Is.EqualTo("myserver"));
        }
        
        [Test]
        public void ShouldAnalyzeTableWithIfClause()
        {
            var sql = "IF NOT EXISTS(SELECT * FROM sys.objects WHERE object_id=OBJECT_ID(N'[dbo].[Tests]') AND type IN(N'U')) CREATE TABLE dbo.Tests(Id INT PRIMARY KEY IDENTITY(1, 1), Message VARCHAR(MAX))";
            
            var sut = new DatabaseInfo(
                "Data Source=ServerName;Initial Catalog=DataBaseName;Integrated Security=SSPI;",
                SqlVersion.Sql140, "dbo");

            var result = sut.AnalyseScript(sql);

            var firstBatch = result.Batches.First();
            var references = firstBatch.References.ToList();
            
            Assert.That(references.Count, Is.EqualTo(2));
            Assert.That(references[0].Schema.Name, Is.EqualTo("sys"));
            Assert.That(references[0].Name, Is.EqualTo("objects"));
            Assert.That(references[1].Schema.FullName, Is.EqualTo("dbo."));
            Assert.That(references[1].Name, Is.EqualTo("OBJECT_ID"));
        }
    }
}