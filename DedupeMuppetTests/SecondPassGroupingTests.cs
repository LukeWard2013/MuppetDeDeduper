using System.Collections.Generic;
using System.Linq;
using DedupeMuppet;
using DedupeMuppet.Strategies;
using NUnit.Framework;

namespace DedupeMuppetTests
{
    [TestFixture]
    public class SecondPassGroupingTests
    {
        private IGrouping<StrategySignature, Company>[] _deduped;

        private readonly Deduper _deduper = new Deduper(new NameAddressAndPostcodeDedupeStrategy(),
                                                        new NameAndPostcodeDedupeStrategy(),
                                                        new PhoneNumberDedupeStrategy(),
                                                        new TruncatedNameAndPostcodeStrategy());

        [SetUp]
        public void TestData()
        {
            var customers = new List<Company>
                {
                    // Dupe 1 contains matches of type A and C
                    new Company(1, "Company A", "123 London Street", "WN1 0PG", "phone1"), // 
                    new Company(2, "Company A", "123, London St", "WN1 0PG", "phone5"),
                    // ------------------------------------------------------
                    new Company(3, "Company A", "123, London St", "Different Postcode", "phone1"),
                };

            _deduped = _deduper.Dedupe(customers).ToArray();
        }

        [Test]
        public void Should_be_one_group_containing_1_2_and_3_after_second_pass()
        {
            var groups = new SecondStageSetDeduper(_deduped).Combine();
            groups.ShouldContainGroup(1, 2, 3);
        }
    }
}
