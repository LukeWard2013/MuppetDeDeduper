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
        public void Should_be_one_group_containing_1_2_and_3_after_second_pass()
        {
            var groups = new SecondStageDeduper(_deduped).Combine();
            groups.ShouldContainGroup(1, 2, 3);
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

    public class SecondStageDeduper
    {
        private readonly IGrouping<StrategySignature, Company>[] _deduped;


        private List<HashSet<int>> groups = new List<HashSet<int>>(); 
        private Dictionary<int, int> groupIdContainingCompany = new Dictionary<int, int>();
 
        public SecondStageDeduper(IGrouping<StrategySignature, Company>[] deduped)
        {
            _deduped = deduped;
        }

        public CombineGroup Combine()
        {
            int groupId = 0;
            foreach (var group in _deduped)
            {
                int foundCompanyId = 0;
                foreach (var company in group)
                {
                    if (groupIdContainingCompany.ContainsKey(company.Id))
                    {
                        foundCompanyId = company.Id;
                        break;
                    }
                }
                if (foundCompanyId == 0)
                {
                    var newGroup = new HashSet<int>(group.Select(company => company.Id));
                    groups.Add(newGroup);
                    foreach (var company in group)
                    {
                        groupIdContainingCompany.Add(company.Id, groupId);
                    }
                    groupId++;
                }
                else
                {
                    var foundGroupId = groupIdContainingCompany[foundCompanyId];
                    HashSet<int> foundGroup = groups[foundGroupId];
                    foreach (var company in group)
                    {
                        if (!foundGroup.Contains(company.Id))
                        {
                            foundGroup.Add(company.Id);
                        }
                        groupIdContainingCompany.Add(company.Id, groupId);
                        
                    }
                }

            }
            // group 1 + 2
            // do we have a group containing 1 or 2
            // if yes, add these numbers to existing group and distinct
            // if no, create a new group with these ids
            // group 1 + 3
            // do we have a group containing 1 or 3
            // if yes, add these numbers to existing group and distinct
            // if no, create a new group with these ids
            // result = group 1 + 2 + 3
            return new CombineGroup(groups);
        }
    }

    public class CombineGroup
    {
        IEnumerable<IEnumerable<int>> _groups;

        public CombineGroup(IEnumerable<IEnumerable<int>> list)
        {
            _groups = list;
        }

        public void ShouldContainGroup(params int[] companyIds)
        {
            if (_groups.Any(list => list.ArraysEqual(companyIds)))
                return;
            
            throw new AssertException("fucked up");
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

        public static bool ArraysEqual<T>(this IEnumerable<T> arg1, IEnumerable<T> arg2)
        {
            if (arg1 == null || arg2 == null)
                return false;

            var a1 = arg1.ToArray();
            var a2 = arg2.ToArray();

            if (ReferenceEquals(a1, a2))
                return true;

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