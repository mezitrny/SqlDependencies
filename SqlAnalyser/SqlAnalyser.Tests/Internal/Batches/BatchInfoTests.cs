using System.Linq;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using Moq;
using NUnit.Framework;
using SqlAnalyser.Internal;
using SqlAnalyser.Internal.Batches;
using SqlAnalyser.Internal.Identifiers;
using SqlAnalyser.Internal.Visitors;

namespace SqlAnalyser.Tests.Internal.Batches
{
    [TestFixture]
    public class BatchInfoTests
    {
        [Test]
        public void ShouldConstructInstance()
        {
            var batch = new TSqlBatch();

            var sut = new BatchInfo(batch, 4, "A", "B", "C");
            
            Assert.That(sut.Order, Is.EqualTo(4));
            Assert.That(sut.Value, Is.EqualTo(batch));
            Assert.That(sut.DefaultSchema, Is.EqualTo("A"));
            Assert.That(sut.DefaultDatabase, Is.EqualTo("B"));
            Assert.That(sut.DefaultServer, Is.EqualTo("C"));
        }
        
        [Test]
        public void ShouldReturnBatchSqlScript()
        {
            const string sql = "SELECT 1 FROM tbl1\nGO\nSELECT 2 FROM tbl2\nGO\nSELECT 3 FROM tbl3";

            var batches = SqlParser.Parse(sql, SqlVersion.Sql100, out var errors);
            
            var sut = new BatchInfo(batches.Skip(1).First(), 4, "A", "B", "C");
            
            Assert.That(sut.Sql, Is.EqualTo("SELECT 2 FROM tbl2"));
        }
        
        [Test]
        public void ShouldReturnOtherType()
        {
            var batch = new TSqlBatch();
            var doerOne = new IdentifierInfo(BatchTypes.Procedure, "A1");
            var doerTwo = new IdentifierInfo(BatchTypes.Function, "A2");
            
            var sut = new BatchInfo(batch, 2, new []{doerOne, doerTwo}, new IdentifierInfo[]{});

            Assert.That(sut.BatchType, Is.EqualTo(BatchTypes.Other));
        }
        
        [Test]
        public void ShouldReturnProcedureType()
        {
            var batch = new TSqlBatch();
            var doerOne = new IdentifierInfo(BatchTypes.Procedure, "A1");
            var doerTwo = new IdentifierInfo(BatchTypes.Procedure, "A2");
            
            var sut = new BatchInfo(batch, 2, new []{doerOne, doerTwo}, new IdentifierInfo[]{});

            Assert.That(sut.BatchType, Is.EqualTo(BatchTypes.Other));
        }
        
        [Test]
        public void ShouldReturnEmptyType()
        {
            var batch = new TSqlBatch();
            
            var sut = new BatchInfo(batch, 2, new IdentifierInfo[]{}, new IdentifierInfo[]{});

            Assert.That(sut.BatchType, Is.EqualTo(BatchTypes.Other));
        }
        
        [Test]
        public void ShouldReturnDoers()
        {
            var batch = new TSqlBatch();
            var factory = new Mock<IDoerVisitor>();
            var doerOne = new IdentifierInfo(BatchTypes.Procedure, "A1");
            var doerTwo = new IdentifierInfo(BatchTypes.Procedure, "A2");
            factory.Setup(x => x.GetReferences(batch, "A", "B", "C")).Returns(new[] {doerOne, doerTwo});
            
            var sut = new BatchInfo(batch, 2, "A", "B", "C");
            sut.DoerVisitor = factory.Object;

            var result = sut.Doers.ToList();

            Assert.That(result, Is.EquivalentTo(new[]{doerOne, doerTwo}));
        }
        
        [Test]
        public void ShouldReturnReferences()
        {
            var batch = new TSqlBatch();
            var factory = new Mock<IReferenceVisitor>();
            var factory2 = new Mock<IDoerVisitor>();
            
            var refOne = new IdentifierInfo(BatchTypes.Procedure, "A1");
            var refTwo = new IdentifierInfo(BatchTypes.Procedure, "A2");
            var doerOne = new IdentifierInfo(BatchTypes.Procedure, "B1");
            var doerTwo = new IdentifierInfo(BatchTypes.Procedure, "B2");
            var doers = new[] { doerOne, doerTwo};
            
            factory2.Setup(x => x.GetReferences(batch, "A", "B", "C")).Returns(doers);
            factory.Setup(x => x.GetReferences(batch, doers, "A", "B", "C"))
                .Returns((new[] {refOne, refTwo}, doers));

            var sut = new BatchInfo(batch, 2, "A", "B", "C")
            {
                DoerVisitor = factory2.Object,
                ReferenceVisitor = factory.Object
            };

            var result = sut.References.ToList();

            Assert.That(result, Is.EquivalentTo(new[]{refOne, refTwo}));
        }
    }
}