using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.IO;

namespace serializer
{
    public class RootDescriptor<T>
    {
        delegate string vypisovac(T instance);
        public void Serialize(TextWriter writer, T instance)
        {
            foreach (PropertyInfo property in instance.GetType().GetProperties())
            {
                if (property.PropertyType == typeof(string) || property.PropertyType == typeof(int))
                {
                    writer.WriteLine(property.Name.ToString() + ": " + typeof(T).GetProperty(property.Name).GetValue(instance).ToString());
                }
                else
                {
                    //delegate void vypisovac(PropertyAttributes. instance);
                    //Serialize(writer, property);
                    writer.WriteLine("zavolej se na " + property.PropertyType.ToString());
                }
            }
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

            return rootDesc;
        }
    }

}
