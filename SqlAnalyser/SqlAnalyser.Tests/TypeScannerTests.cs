﻿using System.Linq;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using NUnit.Framework;
using SqlAnalyser.Internal;

namespace SqlAnalyser.Tests
{
    [TestFixture]
    public class TypeScannerTests
    {
        [Test]
        public void ShouldIdentifyTable()
        {
            var sql = "CREATE TABLE someTable (Id INT)";

            var batch = SqlParser.Parse(sql, SqlVersion.Sql100, out var errors);

            var result = new TypeScanner().ScanScriptType(batch.SingleOrDefault());
            
            Assert.That(result, Is.EqualTo(BatchTypes.Table));
        }
        
        [Test]
        public void ShouldIdentifyTableAmongstOtherStatements()
        {
            var sql = "CREATE TABLE someTable (Id INT)\r\nSELECT 1 FROM someTable";

            var batch = SqlParser.Parse(sql, SqlVersion.Sql100, out var errors);

            var result = new TypeScanner().ScanScriptType(batch.SingleOrDefault());
            
            Assert.That(result, Is.EqualTo(BatchTypes.Table));
        }
        
        [Test]
        public void ShouldIdentifyProcedure()
        {
            var sql = "CREATE PROCEDURE ThisOne AS BEGIN EXECUTE someProc @someParameter='A' END";

            var batch = SqlParser.Parse(sql, SqlVersion.Sql100, out var errors);

            var result = new TypeScanner().ScanScriptType(batch.SingleOrDefault());
            
            Assert.That(result, Is.EqualTo(BatchTypes.Procedure));
        }
        
        [Test]
        public void ShouldIdentifyFunction()
        {
            var sql = "CREATE FUNCTION ThisOne() RETURNS INT AS BEGIN RETURN SomeFunc() END";

            var batch = SqlParser.Parse(sql, SqlVersion.Sql100, out var errors);

            var result = new TypeScanner().ScanScriptType(batch.SingleOrDefault());
            
            Assert.That(result, Is.EqualTo(BatchTypes.Function));
        }
        
        [Test]
        public void ShouldIdentifyOther()
        {
            var sql = "SELECT 1 FROM someTable";

            var batch = SqlParser.Parse(sql, SqlVersion.Sql100, out var errors);

            var result = new TypeScanner().ScanScriptType(batch.SingleOrDefault());
            
            Assert.That(result, Is.EqualTo(BatchTypes.Other));
        }
    }
}