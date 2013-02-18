using System;
using System.Collections.Generic;
using System.Linq;
using DedupeMuppet.Strategies;

namespace DedupeMuppet
{
    public class Company
    {
        public Company(int id, string name, string address, string postcode, string telephone)
        {
            Id = id;
            Name = name;
            Address = address;
            TruncatedAddress = TruncateText(address, new TruncateAdressLine1Strategy());
            PostCode = postcode;
            Telephone = telephone;
            TruncatedName = TruncateText(name, new TruncateCompanyNameStrategy());
        }

        public int Id { get; set; }
        public string Name { get; set; }

        public string TruncatedName { get; set; }
        public string Address { get; set; }
        public string TruncatedAddress { get; set; }
        public string PostCode { get; set; }
        public string Telephone { get; set; }

        private string TruncateText(string text, ITruncateStrategy truncateStrategy)
        {
            text = text.ToUpper();
            text = truncateStrategy.CommonWords().Aggregate(text, (currentWord, commonWord) => currentWord.Replace(commonWord.ToUpper(), ""));

            var arr = text.ToCharArray();

            arr = Array.FindAll(arr, (c => (char.IsLetterOrDigit(c))));

            if (truncateStrategy.GetType()==typeof(TruncateCompanyNameStrategy))
            {
                return new string(arr).Substring(0, arr.Length < 10 ? arr.Length : 10).ToLower();    
            }
            return new string(arr).ToLower();
        }
    }
}