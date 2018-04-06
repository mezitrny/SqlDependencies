using NUnit.Framework;
using SqlAnalyser.Internal.Identifiers;

namespace SqlAnalyser.Tests.Internal.Identifiers
{
    [TestFixture]
    public class QualifierTests
    {
        [Test]
        public void ShouldCreateInstance()
        {
            var sut = new Qualifier("A", QualifierTypes.Database);
            
            Assert.That(sut.Name, Is.EqualTo("A"));
            Assert.That(sut.Type, Is.EqualTo(QualifierTypes.Database));
        }

        [Test]
        public void ShouldSetDefaultName()
        {
            var sut = new Qualifier("A", QualifierTypes.Database);

            sut.DefaultName = "B";
            
            Assert.That(sut.DefaultName, Is.EqualTo("B"));
        }

        [Test]
        public void ShouldReturnFullName()
        {
            var sut = new Qualifier("myDb", QualifierTypes.Database);

            sut.DefaultName = "B";
            
            Assert.That(sut.FullName, Is.EqualTo("myDb."));
        }
        
        [Test]
        public void ShouldReturnShortName()
        {
            var sut = new Qualifier("myDb", QualifierTypes.Database);

            sut.DefaultName = "B";
            
            Assert.That(sut.ShortName, Is.EqualTo("myDb."));
        }
        
        [Test]
        public void ShouldReturnFullDefaultNameWhenNameNull()
        {
            var sut = new Qualifier(null, QualifierTypes.Database);

            sut.DefaultName = "B";
            
            Assert.That(sut.ShortName, Is.EqualTo("B."));
        }
        
        [Test]
        public void ShouldReturnFullDefaultNameWhenNameEmpty()
        {
            var sut = new Qualifier(string.Empty, QualifierTypes.Database);

            sut.DefaultName = "B";
            
            Assert.That(sut.ShortName, Is.EqualTo("B."));
        }
    }
}