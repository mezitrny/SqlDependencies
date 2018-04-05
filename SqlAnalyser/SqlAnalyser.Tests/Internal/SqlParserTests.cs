using System.Linq;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using NUnit.Framework;
using SqlAnalyser.Internal;

namespace SqlAnalyser.Tests.Internal
{
	[TestFixture]
	public class SqlParserTests
	{
		[Test]
		public void ShouldParseWithoutErrors()
		{
			const string sql = "SELECT 1\nGO\nSelect 2";

			var batches = SqlParser.Parse(sql, SqlVersion.Sql100, out var errors).ToList();
			
			Assert.That(errors, Is.Empty);
			Assert.That(batches.Count, Is.EqualTo(2));

			var firstBatch = batches.First();
			var first = string.Join("", firstBatch.ScriptTokenStream
				.Skip(firstBatch.FirstTokenIndex)
				.Take(firstBatch.LastTokenIndex - firstBatch.FirstTokenIndex + 1)
				.Select(x => x.Text));
			
			var lastBatch = batches.Last();
			var second = string.Join("", lastBatch.ScriptTokenStream
				.Skip(lastBatch.FirstTokenIndex)
				.Take(lastBatch.LastTokenIndex - lastBatch.FirstTokenIndex + 1)
				.Select(x => x.Text));
			
			Assert.That(first, Is.EqualTo("SELECT 1"));
			Assert.That(second, Is.EqualTo("Select 2"));
		}
		
		[Test]
		public void ShouldParseWithErrors()
		{
			const string sql = "SELECT 1\nGO\nSelect FROM tbl\nSelect 3";

			var batches = SqlParser.Parse(sql, SqlVersion.Sql100, out var errors).ToList();
			
			Assert.That(batches, Is.Empty);
			Assert.That(errors.Count, Is.EqualTo(1));
			Assert.That(errors.First().Message, Is.EqualTo("Incorrect syntax near FROM."));
		}
	}
}