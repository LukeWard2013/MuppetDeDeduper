using System;
using System.Collections.Generic;
using System.Linq;

namespace DedupeMuppet
{
    public class Company
    {
        private static readonly string[] CompanyStopWords = new[]
            {"BROTHERS", "LIMITED", "COMPANY", "BROS.", "BROS", "PLC.", "CO.", "LTD.", "LTD", "PLC", "AND", "THE", "CO"}; 

        private static readonly string[] AddressStopWords = new []
            { " AVENUE", " STREET", " DRIVE", " DRV.", " PLC.", " LANE", " ROAD", " DRV", " Dr.", " PLC", " AND", "THE ", " AVE", " AV.", " STR", " RD.", " AV", " Dr", " ST", " LN.", " RD" };

        public Company(int id, string name, string address, string postcode, string telephone)
        {
            Id = id;
            Name = name;
            Address = address;
            TruncatedAddress = TruncateText(address, AddressStopWords);
            PostCode = postcode;
            Telephone = telephone;
            TruncatedName = TruncateText(name, CompanyStopWords, 10);
        }

        public int Id { get; set; }
        public string Name { get; set; }

        public string TruncatedName { get; set; }
        public string Address { get; set; }
        public string TruncatedAddress { get; set; }
        public string PostCode { get; set; }
        public string Telephone { get; set; }

        private string TruncateText(string text, IEnumerable<string> stopwords, int maxLength = int.MaxValue)
        {
            text=StripWords(text, stopwords);

            //remove any symbols
            var arr = text.ToCharArray();

            arr = Array.FindAll(arr, (c => (char.IsLetterOrDigit(c))));

            return new string(arr).Substring(0, arr.Length < maxLength ? arr.Length : maxLength).ToLower();    

        }

        private string StripWords(string textToStripFrom, IEnumerable<string> wordsToStrip)
        {
            textToStripFrom = textToStripFrom.ToUpper();
            return wordsToStrip.Aggregate(textToStripFrom, (currentWord, commonWord) => currentWord.Replace(commonWord.ToUpper(), ""));
        }
    }
}