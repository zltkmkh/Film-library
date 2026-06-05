using System;

namespace Film_library.Models
{
    public class Director
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Country { get; set; }

        // ДОДАЄМО ЦЕЙ РЯДОК: Порожній конструктор для JSON
        public Director() { }

        public Director(string firstName, string lastName, string country)
        {
            FirstName = firstName;
            LastName = lastName;
            Country = country;
        }

        public override string ToString()
        {
            return $"{FirstName} {LastName} ({Country})";
        }
    }
}