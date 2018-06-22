using NUnit.Framework;
using RoseByte.SqlAnalyser.SqlServer.Internal.Batches;
using RoseByte.SqlAnalyser.SqlServer.Internal.Identifiers;

namespace SqlAnalyser.Tests.Internal.Identifiers
{
    [TestFixture]
    public class IdentifierInfoTests
    {
        [Test]
        public void ShouldCreateInstance()
        {
            var sut = new IdentifierInfo(BatchTypes.Function, "MyFunc", "lfo", "someDB", "someServer");
            
            Assert.That(sut.BatchTypes, Is.EqualTo(BatchTypes.Function));
            Assert.That(sut.Name, Is.EqualTo("MyFunc"));
            Assert.That(sut.Schema.Name, Is.EqualTo("lfo"));
            Assert.That(sut.Database.Name, Is.EqualTo("someDB"));
            Assert.That(sut.Server.Name, Is.EqualTo("someServer"));
        }

        [Test]
        public void ShouldReturnShortIdentifier()
        {
            var sut = new IdentifierInfo(BatchTypes.Function, "MyFunc", "lfo", "someDB", "someServer");
            sut.Schema.DefaultName = "lfo";
            sut.Database.DefaultName = "someDB";
            sut.Server.DefaultName = "someServer";
            
            Assert.That(sut.ShortIdentifier, Is.EqualTo("MyFunc"));
        }
        
        [Test]
        public void ShouldReturnFullIdentifier()
        {
            var sut = new IdentifierInfo(BatchTypes.Function, "MyFunc", "lfo", "someDB", "someServer");
            sut.Schema.DefaultName = "lfo";
            sut.Database.DefaultName = "someDB";
            sut.Server.DefaultName = "someServer";
            
            Assert.That(sut.FullIdentifier, Is.EqualTo("someServer.someDB.lfo.MyFunc"));
        }
        
        [Test]
        public void ShouldReturnIdentifier()
        {
            var sut = new IdentifierInfo(BatchTypes.Function, "MyFunc", "lfo");
            sut.Schema.DefaultName = "lfo2";
            sut.Database.DefaultName = "someDB";
            sut.Server.DefaultName = "someServer";
            
            Assert.That(sut.Identifier, Is.EqualTo("lfo.MyFunc"));
        }
    }
}