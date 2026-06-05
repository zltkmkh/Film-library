using System;

namespace Film_library.Models
{
    /// <summary>
    /// Клас, що описує сутність Режисера.
    /// </summary>
    public class Director
    {
        // Властивості сутності
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Country { get; set; }

        /// <summary>
        /// Конструктор для створення об'єкта режисера.
        /// </summary>
        public Director(string firstName, string lastName, string country)
        {
            FirstName = firstName;
            LastName = lastName;
            Country = country;
        }

        /// <summary>
        /// Перевизначення методу для зручного відображення імені режисера в інтерфейсі.
        /// </summary>
        public override string ToString()
        {
            return $"{FirstName} {LastName} ({Country})";
        }
    }
}