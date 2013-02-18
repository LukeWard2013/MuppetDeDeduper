using DedupeMuppet;
using NUnit.Framework;
using Should;

namespace DedupeMuppetTests
{
    [TestFixture]
    public class CompanyNameTest
    {
        [Test]
        public void Should_Be_ejsnellson()
        {
            var company = new Company(0, "Ej Snell & Sons Ltd", "", "", "");
            company.TruncatedName.ShouldEqual("ejsnellson");
        }

        [Test]
        public void Should_Be_a()
        {
            var customer = new Company(0, "Company A", "", "", "");
            customer.TruncatedName.ShouldEqual("a");
        }

        [Test]
        public void Should_be_myname()
        {
            var company = new Company(0, "My Name Ltd.", "", "", "");
            company.TruncatedName.ShouldEqual("myname");
        }
    }
}
