using NUnit.Framework;
using RoseByte.SqlAnalyser.SqlServer.Internal.Identifiers;

namespace SqlAnalyser.Tests.Identifiers
{
    [TestFixture]
    public class IdentifierInfoTests
    {
        [Test]
        public void ShouldCreateInstance()
        {
            var sut = new IdentifierInfo(IdentifierTypes.Function, "MyFunc", "lfo", "someDB", "someServer");
            
            Assert.That(sut.Type, Is.EqualTo(IdentifierTypes.Function));
            Assert.That(sut.ObjectName, Is.EqualTo("MyFunc"));
            Assert.That(sut.SchemaName, Is.EqualTo("lfo"));
            Assert.That(sut.DatabaseName, Is.EqualTo("someDB"));
            Assert.That(sut.ServerName, Is.EqualTo("someServer"));
        }

        [Test]
        public void ShouldReturnFullIdentifier()
        {
            var sut = new IdentifierInfo(IdentifierTypes.Function, "MyFunc", "lfo", "someDB", "someServer");
            Assert.That(sut.Name, Is.EqualTo("someServer.someDB.lfo.MyFunc"));
        }

	    [Test]
	    public void ShouldReturnFullIdentifierWithNulls()
	    {
		    var sut = new IdentifierInfo(IdentifierTypes.Function, "MyFunc", null, null, null);
		    Assert.That(sut.Name, Is.EqualTo("MyFunc"));
	    }

		[Test]
	    public void SameIdentifiersShouldEqual()
	    {
			var a = new IdentifierInfo(IdentifierTypes.Function, "MyFunc", "lfo", "someDB", "someServer");
			var b = new IdentifierInfo(IdentifierTypes.Function, "MyFunc", "lfo", "someDB", "someServer");

			Assert.That(a == b, Is.True);
		}

	    [Test]
	    public void SameIdentifiersShouldHaveSameHashCode()
	    {
		    var a = new IdentifierInfo(IdentifierTypes.Function, "MyFunc", "lfo", "someDB", "someServer");
		    var b = new IdentifierInfo(IdentifierTypes.Function, "MyFunc", "lfo", "someDB", "someServer");

		    Assert.That(a.GetHashCode(), Is.EqualTo(b.GetHashCode()));
	    }

	    [Test]
	    public void DifferentIdentifiersShouldNotEqual()
	    {
		    var original = new IdentifierInfo(IdentifierTypes.Function, "MyFunc", "lfo", "someDB", "someServer");
		    var differentType = new IdentifierInfo(IdentifierTypes.Procedure, "MyFunc", "lfo", "someDB", "someServer");
		    var differentObjectName = new IdentifierInfo(IdentifierTypes.Function, "otherFunc", "lfo", "someDB", "someServer");
		    var differentSchemaName = new IdentifierInfo(IdentifierTypes.Function, "MyFunc", "other", "someDB", "someServer");
		    var differentDatabasename = new IdentifierInfo(IdentifierTypes.Function, "MyFunc", "lfo", "otherDB", "someServer");
		    var differentServerName = new IdentifierInfo(IdentifierTypes.Function, "MyFunc", "lfo", "someDB", "otherServer");

		    Assert.That(original != differentType, Is.True);
		    Assert.That(original != differentObjectName, Is.True);
		    Assert.That(original != differentSchemaName, Is.True);
		    Assert.That(original != differentDatabasename, Is.True);
		    Assert.That(original != differentServerName, Is.True);
	    }

	    [Test]
	    public void DifferentIdentifiersShouldNotHaveSameHashCode()
	    {
		    var original = new IdentifierInfo(IdentifierTypes.Function, "MyFunc", "lfo", "someDB", "someServer");
		    var differentType = new IdentifierInfo(IdentifierTypes.Procedure, "MyFunc", "lfo", "someDB", "someServer");
		    var differentObjectName = new IdentifierInfo(IdentifierTypes.Function, "otherFunc", "lfo", "someDB", "someServer");
		    var differentSchemaName = new IdentifierInfo(IdentifierTypes.Function, "MyFunc", "other", "someDB", "someServer");
		    var differentDatabasename = new IdentifierInfo(IdentifierTypes.Function, "MyFunc", "lfo", "otherDB", "someServer");
		    var differentServerName = new IdentifierInfo(IdentifierTypes.Function, "MyFunc", "lfo", "someDB", "otherServer");

		    var originalHashCode = original.GetHashCode();


			Assert.That(originalHashCode, Is.Not.EqualTo(differentType.GetHashCode()));
		    Assert.That(originalHashCode, Is.Not.EqualTo(differentObjectName.GetHashCode()));
		    Assert.That(originalHashCode, Is.Not.EqualTo(differentSchemaName.GetHashCode()));
		    Assert.That(originalHashCode, Is.Not.EqualTo(differentDatabasename.GetHashCode()));
		    Assert.That(originalHashCode, Is.Not.EqualTo(differentServerName.GetHashCode()));
	    }
	}
}