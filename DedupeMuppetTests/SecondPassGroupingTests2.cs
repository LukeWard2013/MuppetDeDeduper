using System.Collections.Generic;
using System.Linq;
using DedupeMuppet;
using DedupeMuppet.Strategies;
using NUnit.Framework;

namespace DedupeMuppetTests
{
    [TestFixture]
    public class SecondPassGroupingTests2
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
                    // Dupe 2 contains a D match
                    new Company(4, "Ej Snell & Sons Ltd", "80 London Street", "WN1 0PG", "phone2"),
                    new Company(5, "E J Snell & Son Ltd", "88 London Street", "WN1 0PG", "phone3"),
                    // ------------------------------------------------------
                    new Company(6, "Company C", "123 London Street", "WN1 0PG", "phone4"),
                    // ------------------------------------------------------

                    new Company(7, "Stratfords Tools Ltd", "80 London Street", "WN1 0PG", "phone6"),
                    new Company(8, "Stratfords Tools Ltd", "80 London Street", "WN1 0PG", "phone7"),
                    // ------------------------------------------------------

                    //Dupe 4 contains a Name and Postcode match and also matches Dupe 3 as an Truncated Name and Postcode match, this should be ignored
                    new Company(9,"Stratfords Ltd", "80 Hewit Street", "WN1 0PG", "00000000005"),
                    new Company(10,"Stratfords Ltd", "88 Hewit Street", "WN1 0PG", "00000000006"),                    
                };

            _deduped = _deduper.Dedupe(customers).ToArray();
        }

        [TestCase(1,2,3)]
        [TestCase(4,5)]
        [TestCase(7,8,9,10)]
        public void Should_be_one_group_containing_after_second_pass(params int[] companyIds)
        {
            var groups = new SecondStageSetDeduper(_deduped).Combine();
            groups.ShouldContainGroup(companyIds);
        }
    }
}