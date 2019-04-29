using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace serializer
{
    public class RootDescriptor<T>
    {
        static RootDescriptor()
        {
            name = typeof(T).ToString().Split('.').Last();
        }
        static readonly string name;
        Dictionary<string,delegat> slovnik=new Dictionary<string, delegat>();
        public delegate string delegat(T instance);
        public void AddProperty(string name, delegat vypisovac)
        {
            slovnik.Add(name,vypisovac);
        }
        public void Serialize(TextWriter writer, T instance)
        {
            writer.WriteLine($"<{name}>");
            foreach(var x in slovnik)
            {
                writer.WriteLine($"<{x.Key}>{x.Value(instance)}</{x.Key}>");
            }
            writer.WriteLine($"</{name}>");
        }
        public string GetString(T instance)
        {
            StringBuilder s = new StringBuilder();
            foreach (var x in slovnik)
            {
                s.Append($"\n<{x.Key}>{x.Value(instance)}</{x.Key}>");
            }
            s.Append("\n");
            return s.ToString();
        }
    }
    class Address
    {
        public string Street { get; set; }
        public string City { get; set; }
    }

    class Country
    {
        public string Name { get; set; }
        public int AreaCode { get; set; }
    }

    class PhoneNumber
    {
        public Country Country { get; set; }
        public int Number { get; set; }
    }

    class Person
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public Address HomeAddress { get; set; }
        public Address WorkAddress { get; set; }
        public Country CitizenOf { get; set; }
        public PhoneNumber MobilePhone { get; set; }
    }

    class Program
    {
        static void Main(string[] args)
        {
            RootDescriptor<Person> rootDesc = GetPersonDescriptor();

            var czechRepublic = new Country { Name = "Czech Republic", AreaCode = 420 };
            var person = new Person
            {
                FirstName = "Pavel",
                LastName = "Jezek",
                HomeAddress = new Address { Street = "Patkova", City = "Prague" },
                WorkAddress = new Address { Street = "Malostranske namesti", City = "Prague" },
                CitizenOf = czechRepublic,
                MobilePhone = new PhoneNumber { Country = czechRepublic, Number = 123456789 }
            };

            rootDesc.Serialize(Console.Out, person);
        }

        static RootDescriptor<Person> GetPersonDescriptor()
        {
            var rootDesc = new RootDescriptor<Person>();
            rootDesc.AddProperty("FirstName",x => x.FirstName);
            rootDesc.AddProperty("LastName", x => x.LastName);
            rootDesc.AddProperty("HomeAddress", x => GetAddressDescriptor().GetString(x.HomeAddress));
            rootDesc.AddProperty("WorkAddress", x => GetAddressDescriptor().GetString(x.WorkAddress));
            rootDesc.AddProperty("CitizenOf", x => GetCountryDescriptor().GetString(x.CitizenOf));
            rootDesc.AddProperty("MobilePhone", x => GetPhoneNumberDescriptor().GetString(x.MobilePhone));
            return rootDesc;
        }
        static RootDescriptor<Address> GetAddressDescriptor()
        {
            var rootDesc = new RootDescriptor<Address>();
            rootDesc.AddProperty("Street", x => x.Street);
            rootDesc.AddProperty("City", x => x.City);
            return rootDesc;
        }
        static RootDescriptor<Country> GetCountryDescriptor()
        {
            var rootDesc = new RootDescriptor<Country>();
            rootDesc.AddProperty("Name", x => x.Name);
            rootDesc.AddProperty("AreaCode", x => x.AreaCode.ToString());
            return rootDesc;
        }
        static RootDescriptor<PhoneNumber> GetPhoneNumberDescriptor()
        {
            var rootDesc = new RootDescriptor<PhoneNumber>();
            rootDesc.AddProperty("Country", x => GetCountryDescriptor().GetString(x.Country));
            rootDesc.AddProperty("Number", x => x.Number.ToString());
            return rootDesc;
        }
    }

}
