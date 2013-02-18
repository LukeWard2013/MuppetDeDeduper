using DedupeMuppet;
using NUnit.Framework;
using Should;

namespace DedupeMuppetTests
{
    public class AddressLine1Tests
    {
        [Test]
        public void Should_be_88hewit()
        {
            var company = new Company(0, "", "88 Hewit Street", "", "");
            company.TruncatedAddress.ShouldEqual("88hewit");
        }

        [Test]
        public void Should_be_JustThisTextLeft_for_truncated_address()
        {
            var company = new Company(0, "", "Just Street, This, Avenue,Text Left Drive.", "", "");
            company.TruncatedAddress.ShouldEqual("justthistextleft");
        }

        [Test]
        public void Should_be_oldpoliceation()
        {
            var company = new Company(0, "", "The Old Police Station", "", "");
            company.TruncatedAddress.ShouldEqual("oldpoliceation");
        }
    }
}
