using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DedupeMuppet;
using DedupeMuppet.Strategies;
using NUnit.Framework;
using Should.Core.Exceptions;

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
            var groups = new SecondStageDeduper(_deduped).Combine();
            groups.ShouldContainGroup(1, 2, 3);
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
                var foundCompanyId = 0;

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
                        //Adding dictionary check here, as companies can be found multiple times
                        if (!groupIdContainingCompany.ContainsKey(company.Id))
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
            foreach (IEnumerable<int> list in _groups)
            {
                if (list.ArraysEqual(companyIds)) return;
            }

            throw new AssertException("fucked up");
        }
    }
}
