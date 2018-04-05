using System.Linq;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using NUnit.Framework;
using SqlAnalyser.Internal.Batches;

namespace SqlAnalyser.Tests.Batches
{
    [TestFixture]
    public class BatchFactoryTests
    {
        [Test]
        public void ShouldGenerateBatches()
        {
            const string sql = "SELECT 1";

            var sut = new BatchFactory();

            (var batches, var errors) = sut.Generate(sql, SqlVersion.Sql100, "A", "B", "C");
            var batchList = batches.ToList();
            
            Assert.That(batchList.Count, Is.EqualTo(1));
            Assert.That(errors, Is.Empty);
            Assert.That(batchList.First().Sql, Is.EqualTo("SELECT 1"));
        }
        
        [Test]
        public void ShouldGenerateError()
        {
            const string sql = "SELECT FROM tbl";

            var sut = new BatchFactory();

            (var batches, var errors) = sut.Generate(sql, SqlVersion.Sql100, "A", "B", "C");
            var errorList = errors.ToList();

            Assert.That(batches, Is.Empty);
            Assert.That(errorList.Count, Is.EqualTo(1));
            Assert.That(errorList.First().Message, Is.EqualTo("Incorrect syntax near FROM."));
        }
    }
}