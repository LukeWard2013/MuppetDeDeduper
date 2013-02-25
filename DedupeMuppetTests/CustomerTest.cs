using System.Collections.Generic;
using System.Linq;
using DedupeMuppet;
using DedupeMuppet.Strategies;
using NUnit.Framework;
using Should;
using Should.Core.Exceptions;

namespace DedupeMuppetTests
{
    [TestFixture]
    public class CustomerTest
    {
        private IGrouping<StrategySignature, Company>[] _deduped;

        private readonly Deduper _deduper = new Deduper(new NameAddressAndPostcodeDedupeStrategy(),
                                                        new NameAndPostcodeDedupeStrategy(),
                                                        new PhoneNumberDedupeStrategy(),
                                                        new TruncatedNameAndPostcodeStrategy());

        [SetUp]
        public void SetUp()
        {
            //Match types in order or precedence 
            //--------------------------------------------------------
            //Match type A = telephone match
            //Match type B = Company name, address line 1 and postcode
            //Match type C = Company name and postcode
            //Match type D = Truncated company name (10 chars) and postcode

            var customers = new List<Company>
                {
                    // Dupe 1 contains matches of type A and C
                    new Company(1, "Company A", "123 London Street", "WN1 0PG", "phone1"), // 
                    new Company(2, "Company A", "123, London St", "WN1 0PG", "phone5"),
                    // ------------------------------------------------------
                    new Company(3, "Company A", "123, London St", "Different Postcode", "phone1"),
                    // ------------------------------------------------------

                    // Dupe 2 contains a D match
                    new Company(4, "Ej Snell & Sons Ltd", "80 London Street", "WN1 0PG", "phone2"),
                    new Company(5, "E J Snell & Son Ltd", "88 London Street", "WN1 0PG", "phone3"),
                    // ------------------------------------------------------
                    new Company(6, "Company C", "123 London Street", "WN1 0PG", "phone4"),
                    // ------------------------------------------------------

                    new Company(7, "Stratfords Tools Ltd", "80 London Street", "WN1 0PG", "phone5"),
                    new Company(8, "Stratfords Tools Ltd", "80 London Street", "WN1 0PG", "phone6"),
                    // ------------------------------------------------------

                    //Dupe 4 contains a Name and Postcode match and also matches Dupe 3 as an Truncated Name and Postcode match, this should be ignored
                    new Company(9,"Stratfords Ltd", "80 Hewit Street", "WN1 0PG", "00000000005"),
                    new Company(10,"Stratfords Ltd", "88 Hewit Street", "WN1 0PG", "00000000006"),

                };

            _deduped = _deduper.Dedupe(customers).ToArray();

        }

        [Test]
        public void Should_be_one_group_containing_1_and_2_using_name_and_postcode_match()
        {
            var group = _deduped.GetGroupContaining(1, 2);
            group.Key.Strategy.ShouldBeType<NameAndPostcodeDedupeStrategy>();
        }

        [Test]
        public void Should_be_one_group_containing_1_and_3_using_phone_number()
        {
            var group = _deduped.GetGroupContaining(1, 3);
            group.Key.Strategy.ShouldBeType<PhoneNumberDedupeStrategy>();
        }

        [Test]
        public void Should_be_one_group_containing_9_and_10_using_name_and_postcode_match()
        {
            var group = _deduped.GetGroupContaining(9, 10);
            group.Key.Strategy.ShouldBeType<NameAndPostcodeDedupeStrategy>();
        }

        [Test]
        public void Should_be_one_group_containing_4_and_5_using_name_and_postcode_match()
        {
            var group = _deduped.GetGroupContaining(4, 5);
            group.Key.Strategy.ShouldBeType<TruncatedNameAndPostcodeStrategy>();
        }

        [Test]
        public void Should_be_one_group_containing_7_and_8_using_name_address_and_postcode_match()
        {
            var group = _deduped.GetGroupContaining(7, 8);
            group.Key.Strategy.ShouldBeType<NameAddressAndPostcodeDedupeStrategy>();
        }

        [Test]
        public void Should_not_be_a_group_containing_1_2_and_3()
        {
            Assert.Throws<WrongIdsException>(() => _deduped.GetGroupContaining(1, 2, 3));
        }

        [Test]
        public void Should_be_company_A_and_postcode_wn1_0pg()
        {
            foreach (var customer in _deduped[0])
            {
                customer.Name.ShouldEqual("Company A");
                customer.PostCode.ShouldEqual("WN1 0PG");
            }
        }

        [Test]
        public void Should_match_on_name_and_postcode()
        {
            var group = _deduped.GetGroupContaining(1, 2);
            group.Key.Strategy.ShouldBeType<NameAndPostcodeDedupeStrategy>();
        }
    }



    public class WrongIdsException : AssertException
    {
        public WrongIdsException(string message)
            : base(message)
        {

        }
    }

    public static class ObjectExtensions
    {
        public static IGrouping<StrategySignature, Company> GetGroupContaining(this IGrouping<StrategySignature, Company>[] groups, params int[] expectedIds)
        {
            var group = groups.FirstOrDefault(d => d.Select(g => g.Id).ToArray().ArraysEqual(expectedIds));
            if (group != null)
                return group;

            throw new WrongIdsException("Could not find a group with company ids " + string.Join(", ", expectedIds));
        }

    }
}