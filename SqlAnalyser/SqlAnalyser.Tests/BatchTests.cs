using System.Linq;
using Moq;
using NUnit.Framework;
using RoseByte.SqlAnalyser.SqlServer;
using RoseByte.SqlAnalyser.SqlServer.Internal.Batches;
using RoseByte.SqlAnalyser.SqlServer.Internal.Identifiers;

namespace SqlAnalyser.Tests
{
    [TestFixture]
    public class BatchTests
    {
        [Test]
        public void ShouldCreateInstance()
        {
            var sut = new Batch("SELECT 1");
            Assert.That(sut.Sql, Is.EqualTo("SELECT 1"));
        }

        [Test]
        public void ShouldCollectReferencies()
        {
            var dependencyOne = new IdentifierInfo(IdentifierTypes.Procedure, "A", null, null, null);
            var dependencyTwo = new IdentifierInfo(IdentifierTypes.Procedure, "B", null, null, null);
            var dependencyThree = new IdentifierInfo(IdentifierTypes.Function, "C", null, null, null);
            
            var batchOne = new Mock<IScript>();
            batchOne.Setup(x => x.References).Returns(new[] {dependencyOne, dependencyTwo});
            var batchTwo = new Mock<IScript>();
            batchTwo.Setup(x => x.References).Returns(new[] {dependencyOne, dependencyThree});
            
            var sut = new Batch("SELECT 1", batchOne.Object, batchTwo.Object);

            Assert.That(sut.References, Is.EquivalentTo(new []{dependencyOne, dependencyTwo, dependencyThree}));
        }
        
        [Test]
        public void ShouldCollectDefinitions()
        {
            var dependencyOne = new IdentifierInfo(IdentifierTypes.Procedure, "A", null, null, null);
            var dependencyTwo = new IdentifierInfo(IdentifierTypes.Procedure, "B", null, null, null);
            var dependencyThree = new IdentifierInfo(IdentifierTypes.Function, "C", null, null, null);
            
            var batchOne = new Mock<IScript>();
            batchOne.Setup(x => x.Definitions).Returns(new[] {dependencyOne, dependencyTwo});
            var batchTwo = new Mock<IScript>();
            batchTwo.Setup(x => x.Definitions).Returns(new[] {dependencyOne, dependencyThree});
            
            var sut = new Batch("SELECT 1", batchOne.Object, batchTwo.Object);

            Assert.That(sut.Definitions, Is.EquivalentTo(new []{dependencyOne, dependencyTwo, dependencyThree}));
        }
        
        [Test]
        public void ShouldReturnBatches()
        {
	        var sut = new Batch("SELECT 1 GO SELECT 2");

	        Assert.That(sut.Batches.Count(), Is.EqualTo(2));
	        Assert.That(sut.Batches.First().Sql, Is.EqualTo("SELECT 1 "));
	        Assert.That(sut.Batches.Last().Sql, Is.EqualTo(" SELECT 2"));
		}

	    [Test]
	    public void ShouldReturnErrors()
	    {
		    var sut = new Batch("SELECT * FROM");

		    Assert.That(sut.Errors.FirstOrDefault()?.Message, Is.EqualTo("Incorrect syntax near 'End Of File'."));
	    }
	}
}