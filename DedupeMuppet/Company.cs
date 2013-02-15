using System;
using System.Collections.Generic;
using System.Linq;

namespace DedupeMuppet
{
    public class Company
    {
        public Company(int id, string name, string address, string postcode, string telephone)
        {
            Id = id;
            Name = name;
            Address = address;
            TruncatedAddress = TruncateAddress(address);
            PostCode = postcode;
            Telephone = telephone;
            TruncatedName = TruncateName(name);
        }

        public int Id { get; set; }
        public string Name { get; set; }

        public string TruncatedName { get; set; }
        public string Address { get; set; }
        public string TruncatedAddress { get; set; }
        public string PostCode { get; set; }
        public string Telephone { get; set; }


        private string TruncateName(string name)
        {
            //Strip common words from company name
            string[] commonWords = { "BROTHERS", "LIMITED", "COMPANY", "BROS.", "BROS", "PLC.", "CO.", "LTD.", "LTD", "PLC", "AND", "THE", "CO" };
            name = StripWords(name, commonWords);
            
            //remove and non alphanumeric characters
            char[] arr = name.ToCharArray();

            arr = Array.FindAll(arr, (c => (char.IsLetterOrDigit(c))));
            return new string(arr).Substring(0, arr.Length < 10 ? arr.Length : 10).ToLower();
        }

        private string TruncateAddress(string name)
        {
            string[] commonWords = { " AVENUE", " STREET", " DRIVE", " DRV.", " PLC.", " LANE" , " ROAD", " DRV", " Dr.", " PLC", " AND", "THE ", " AVE", " AV.", " STR", " RD.", " AV", " Dr", " ST", " LN.", " RD" };
            name = StripWords(name, commonWords);

            //remove any symbols
            char[] arr = name.ToCharArray();

            arr = Array.FindAll(arr, (c => (char.IsLetterOrDigit(c))));
            return new string(arr).ToLower();
        }

        private string StripWords(string textToStripFrom, IEnumerable<string> wordsToStrip)
        {
            textToStripFrom = textToStripFrom.ToUpper();
            return wordsToStrip.Aggregate(textToStripFrom, (currentWord, commonWord) => currentWord.Replace(commonWord.ToUpper(), ""));
        }
    }
}