using System;

namespace DedupeMuppet
{
    public class Company
    {
        public Company(int id, string name, string address, string postcode, string telephone)
        {
            Id = id;
            Name = name;
            Address = address;
            PostCode = postcode;
            Telephone = telephone;
            TruncatedName = TruncateName(name);
        }

        public int Id { get; set; }
        public string Name { get; set; }

        public string TruncatedName { get; set; }
        public string Address { get; set; }
        public string PostCode { get; set; }
        public string Telephone { get; set; }


        private string TruncateName(string name)
        {
            string[] commonWords = { "Dr.", "Dr", "DRIVE", "DRV", "DRV.", "PLC", "PLC.", "AND", "THE", "AVE", "AV", "AV.", "STR", "ST", "STREET", "LANE", "LN.", "RD", "RD.", "ROAD" };
            foreach (var commonWord in commonWords)
            {
                name.Replace(commonWord, "");
            }

            char[] arr = name.ToCharArray();

            arr = Array.FindAll(arr, (c => (char.IsLetterOrDigit(c))));
            return new string(arr).Substring(0, arr.Length < 10 ? arr.Length : 10).ToLower();

        }
    }
}