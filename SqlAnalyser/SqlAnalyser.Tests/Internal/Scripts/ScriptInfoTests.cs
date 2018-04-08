using Microsoft.SqlServer.TransactSql.ScriptDom;
using Moq;
using NUnit.Framework;
using SqlAnalyser.Internal;
using SqlAnalyser.Internal.Batches;
using SqlAnalyser.Internal.Identifiers;
using SqlAnalyser.Internal.Scripts;

namespace SqlAnalyser.Tests.Internal.Scripts
{
    [TestFixture]
    public class ScriptInfoTests
    {
        [Test]
        public void ShouldCreateInstance()
        {
            var sut = new ScriptInfo("SELECT 1", SqlVersion.Sql100, "A", "B", "C");
            
            Assert.That(sut.Sql, Is.EqualTo("SELECT 1"));
            Assert.That(sut.Version, Is.EqualTo(SqlVersion.Sql100));
            Assert.That(sut.DefaultSchema, Is.EqualTo("C"));
            Assert.That(sut.DefaultDatabase, Is.EqualTo("A"));
            Assert.That(sut.DefaultServer, Is.EqualTo("B"));
        }

        [Test]
        public void ShouldCollectDependencies()
        {
            var dependencyOne = new IdentifierInfo(BatchTypes.Procedure, "A");
            var dependencyTwo = new IdentifierInfo(BatchTypes.Procedure, "B");
            var dependencyThree = new IdentifierInfo(BatchTypes.Function, "C");
            
            var batchOne = new Mock<IBatchInfo>();
            batchOne.Setup(x => x.References).Returns(new[] {dependencyOne, dependencyTwo});
            var batchTwo = new Mock<IBatchInfo>();
            batchTwo.Setup(x => x.References).Returns(new[] {dependencyOne, dependencyThree});
            
            var sut = new ScriptInfo("SELECT 1", SqlVersion.Sql100, "A", "B", "C", batchOne.Object, batchTwo.Object);

            Assert.That(sut.References, Is.EquivalentTo(new []{dependencyOne, dependencyTwo, dependencyThree}));
        }
        
        [Test]
        public void ShouldCollectDoers()
        {
            var dependencyOne = new IdentifierInfo(BatchTypes.Procedure, "A");
            var dependencyTwo = new IdentifierInfo(BatchTypes.Procedure, "B");
            var dependencyThree = new IdentifierInfo(BatchTypes.Function, "C");
            
            var batchOne = new Mock<IBatchInfo>();
            batchOne.Setup(x => x.Doers).Returns(new[] {dependencyOne, dependencyTwo});
            var batchTwo = new Mock<IBatchInfo>();
            batchTwo.Setup(x => x.Doers).Returns(new[] {dependencyOne, dependencyThree});
            
            var sut = new ScriptInfo("SELECT 1", SqlVersion.Sql100, "A", "B", "C", batchOne.Object, batchTwo.Object);

            Assert.That(sut.Doers, Is.EquivalentTo(new []{dependencyOne, dependencyTwo, dependencyThree}));
        }
        
        [Test]
        public void ShouldReturnBatches()
        {
            var factory = new Mock<IBatchFactory>();
            var batchOne = new Mock<IBatchInfo>();
            var batchTwo = new Mock<IBatchInfo>();
            var errorOne = new ParseError(1, 1, 1, 1, "A");
            var errorTwo = new ParseError(1, 1, 1, 1, "B");

            factory.Setup(x => x.Generate("A", SqlVersion.Sql80, "B", "C", "D"))
                .Returns((new []{batchOne.Object, batchTwo.Object}, new []{errorOne, errorTwo}));

            var sut = new ScriptInfo("A", SqlVersion.Sql80, "C", "D", "B")
            {
                BatchFactory = factory.Object
            };
            
            var result = sut.Batches;

            Assert.That(result, Is.EquivalentTo(new []{batchOne.Object, batchTwo.Object}));
            Assert.That(sut.Errors, Is.EquivalentTo(new []{errorOne, errorTwo}));
            factory.Verify(x => x.Generate(It.IsAny<string>(), It.IsAny<SqlVersion>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }
        
        [Test]
        public void ShouldReturnErrors()
        {
            var factory = new Mock<IBatchFactory>();
            var batchOne = new Mock<IBatchInfo>();
            var batchTwo = new Mock<IBatchInfo>();
            var errorOne = new ParseError(1, 1, 1, 1, "A");
            var errorTwo = new ParseError(1, 1, 1, 1, "B");

            factory.Setup(x => x.Generate("A", SqlVersion.Sql80, "B", "C", "D"))
                .Returns((new []{batchOne.Object, batchTwo.Object}, new []{errorOne, errorTwo}));

            var sut = new ScriptInfo("A", SqlVersion.Sql80, "C", "D", "B")
            {
                BatchFactory = factory.Object
            };
            
            var result = sut.Errors;

            Assert.That(result, Is.EquivalentTo(new []{errorOne, errorTwo}));
            Assert.That(sut.Batches, Is.EquivalentTo(new []{batchOne.Object, batchTwo.Object}));
            factory.Verify(x => x.Generate(It.IsAny<string>(), It.IsAny<SqlVersion>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }
        
        [Test]
        public void ShouldBeInvalid()
        {
            var factory = new Mock<IBatchFactory>();
            var errorOne = new ParseError(1, 1, 1, 1, "A");

            factory.Setup(x => x.Generate("A", SqlVersion.Sql80, "B", "C", "D"))
                .Returns((new IBatchInfo[]{}, new []{errorOne}));

            var sut = new ScriptInfo("A", SqlVersion.Sql80, "C", "D", "B")
            {
                BatchFactory = factory.Object
            };
            
            Assert.That(sut.Valid, Is.False);
            Assert.That(sut.Errors, Is.EquivalentTo(new []{errorOne}));
            factory.Verify(x => x.Generate(It.IsAny<string>(), It.IsAny<SqlVersion>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }
        
        [Test]
        public void ShouldBeValid()
        {
            var factory = new Mock<IBatchFactory>();
            var batchOne = new Mock<IBatchInfo>();

            factory.Setup(x => x.Generate("A", SqlVersion.Sql80, "B", "C", "D"))
                .Returns((new []{batchOne.Object}, new ParseError[]{}));

            var sut = new ScriptInfo("A", SqlVersion.Sql80, "C", "D", "B")
            {
                BatchFactory = factory.Object
            };
            
            Assert.That(sut.Valid, Is.True);
            Assert.That(sut.Batches, Is.EquivalentTo(new []{batchOne.Object}));
            factory.Verify(x => x.Generate(It.IsAny<string>(), It.IsAny<SqlVersion>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }
    }
}