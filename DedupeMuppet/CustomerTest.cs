using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Should;

namespace DedupeMuppet
{
    [TestFixture]
    public class CustomerTest
    {
        private GroupedCustomer[] _deduped;

        [SetUp]
        public void SetUp()
        {
            //Match types in order or precedence 
            //--------------------------------------------------------
            //Match type A = telephone match
            //Match type B = Company name, address line 1 and postcode
            //Match type C = Company name and postcode
            //Match type D = Truncated company name (10 chars) and postcode

            var customers = new List<Customer>
                {
                    // Dupe 1 contains matches of type A and C
                    new Customer(1, "Company A", "123 London Street", "WN1 0PG", "00000000001"),
                    new Customer(2, "Company A", "123, London St", "WN1 0PG", "00000000005"),
                    // ------------------------------------------------------
                    new Customer(3, "Company A", "123, London St","Different Postcode", "00000000001"),
                    // ------------------------------------------------------

                    // Dupe 2 contains a D match
                    new Customer(4, "Ej Snell & Sons Ltd", "80 London Street", "WN1 0PG", "00000000002"),
                    new Customer(5, "E J Snell & Son Ltd", "88 London Street", "WN1 0PG", "00000000003"),
                    // ------------------------------------------------------
                    new Customer(6, "Company C", "123 London Street", "WN1 0PG", "00000000004"),
                    // ------------------------------------------------------

                    new Customer(7, "Stratfords Tools Ltd", "80 London Street", "WN1 0PG", "00000000005"),
                    new Customer(8, "Stratfords Tools Ltd", "88 London Street", "WN1 0PG", "00000000006"),
                    // ------------------------------------------------------

                    ////Dupe 4 contains a Name and Postcode match and also matches Dupe 3 as an Truncated Name and Postcode match, this should be ignored
                    //new Customer("Stratfords Ltd", "80 Hewit Street", "WN1 0PG", "00000000005"),
                    //new Customer("Stratfords Ltd", "88 Hewit Street", "WN1 0PG", "00000000006"),

                };

            var deduper = new Deduper(new NameAndPostcodeDedupeStrategy(),
                                      new PhoneNumberDedupeStrategy());

            _deduped = deduper.Dedupe(customers).ToArray();
        }

        [Test]
        public void Should_be_one_group_containing_1_and_2_using_name_and_postcode_match()
        {
            //var group = _deduped.GetGroupContaining(1, 2);

            _deduped.Any(d => d.Dupe.Select(dupe => dupe.Id).ShouldContainOnly(1, 2)
                            && d.StrategyType == typeof(PhoneNumberDedupeStrategy)).ShouldBeTrue();
        }

        [Test]
        public void Should_be_one_group_containing_1_and_3_using_phone_number ()
        {
            _deduped.Any(d => d.Dupe.Select(dupe => dupe.Id).ShouldContainOnly(1, 3)).ShouldBeTrue();
        }

        [Test]
        public void Should_not_be_a_group_containing_1_2_and_3()
        {
            _deduped.Any(d => d.Dupe.Select(dupe => dupe.Id).ShouldContainOnly(1, 2, 3)).ShouldBeFalse();
        }

        [Test]
        public void Should_be_company_A_and_postcode_wn1_0pg()
        {
            foreach (var customer in _deduped[0].Dupe)
            {
                customer.Name.ShouldEqual("Company A");
                customer.PostCode.ShouldEqual("WN1 0PG");
            }
        }

        [Test]
        public void Should_match_on_name_and_postcode()
        {
            _deduped.Any(d => d.Dupe.Select(dupe => dupe.Id).ShouldContainOnly(7,8)).ShouldBeTrue();
        }

        [Test]
        public void Should_Be_ejsnellson()
        {
            var customer = new Customer(0, "Ej Snell & Sons Ltd", "", "", "");
            customer.TruncatedName.ShouldEqual("ejsnellson");
        }

        [Test]
        public void Should_Be_companya()
        {
            var customer = new Customer(0, "Company A", "", "", "");
            customer.TruncatedName.ShouldEqual("companya");
        }
    }

    public static class ObjectExtensions
    {
        public static bool ShouldContainOnly<T>(this IEnumerable<T> actual, params T[] expected)
        {
            Console.WriteLine(string.Join(", ", actual));
            return actual.SequenceEqual(expected);
        }
        public static void GetGroupContaining(this GroupedCustomer[] groups, params int[] expectedIds)
        {
           // groups.First(g => g.Dupe.Select(dupe => dupe.Id).ShouldContainOnly(1, 2)
        }
    }
}