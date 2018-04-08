using Microsoft.SqlServer.TransactSql.ScriptDom;
using NUnit.Framework;

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
    }
}