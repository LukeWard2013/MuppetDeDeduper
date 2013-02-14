using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Should;
using Should.Core.Exceptions;

namespace DedupeMuppet
{
    [TestFixture]
    public class CustomerTest
    {
        private IGrouping<StrategySignature, Company>[] _deduped;

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
                    new Company(1, "Company A", "123 London Street", "WN1 0PG", "phone1"),
                    new Company(2, "Company A", "123, London St", "WN1 0PG", "phone5"),
                    // ------------------------------------------------------
                    new Company(3, "Company A", "123, London St","Different Postcode", "phone1"),
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

                    ////Dupe 4 contains a Name and Postcode match and also matches Dupe 3 as an Truncated Name and Postcode match, this should be ignored
                    //new Company("Stratfords Ltd", "80 Hewit Street", "WN1 0PG", "00000000005"),
                    //new Company("Stratfords Ltd", "88 Hewit Street", "WN1 0PG", "00000000006"),

                };

            var deduper = new Deduper(new NameAddressAndPostcodeDedupeStrategy(),
                                      new NameAndPostcodeDedupeStrategy(),
                                      new PhoneNumberDedupeStrategy());

            _deduped = deduper.Dedupe(customers).ToArray();
        }

        [Test]
        public void Should_be_one_group_containing_1_and_2_using_name_and_postcode_match()
        {
            var group = _deduped.GetGroupContaining(1, 2);
            group.Key.Strategy.ShouldBeType<NameAndPostcodeDedupeStrategy>();
        }

        [Test]
        public void Should_be_one_group_containing_7_and_8_using_name_address_and_postcode_match()
        {
            var group = _deduped.GetGroupContaining(7, 8);
            group.Key.Strategy.ShouldBeType<NameAddressAndPostcodeDedupeStrategy>();
        }

        [Test]
        public void Should_be_one_group_containing_1_and_3_using_phone_number()
        {
            var group = _deduped.GetGroupContaining(1, 3);
            group.Key.Strategy.ShouldBeType<PhoneNumberDedupeStrategy>();
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

        [Test]
        public void Should_Be_ejsnellson()
        {
            var customer = new Company(0, "Ej Snell & Sons Ltd", "", "", "");
            customer.TruncatedName.ShouldEqual("ejsnellson");
        }

        [Test]
        public void Should_Be_companya()
        {
            var customer = new Company(0, "Company A", "", "", "");
            customer.TruncatedName.ShouldEqual("companya");
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
            var group = groups.FirstOrDefault(d => ArraysEqual(d.Select(g => g.Id).ToArray(), expectedIds));
            if (group != null)
                return group;

            throw new WrongIdsException("Could not find a group with company ids " + string.Join(", ", expectedIds));
        }

        static bool ArraysEqual<T>(T[] a1, T[] a2)
        {
            if (ReferenceEquals(a1, a2))
                return true;

            if (a1 == null || a2 == null)
                return false;

            if (a1.Length != a2.Length)
                return false;

            EqualityComparer<T> comparer = EqualityComparer<T>.Default;
            for (int i = 0; i < a1.Length; i++)
            {
                if (!comparer.Equals(a1[i], a2[i])) return false;
            }
            return true;
        }
    }
}