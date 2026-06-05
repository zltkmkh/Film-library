using System;

namespace Film_library.Models
{
    /// <summary>
    /// Клас, що описує сутність Фільму у відеотеці.
    /// </summary>
    public class Movie
    {
        // Властивості сутності відповідно до вимог проєкту
        public string Title { get; set; }
        public string Studio { get; set; }
        public string Genre { get; set; }
        public int Year { get; set; }
        public Director MovieDirector { get; set; } // Зв'язок із сутністю Режисера
        public string Actors { get; set; }
        public string Summary { get; set; }
        public int Rating { get; set; }
        public string FilePath { get; set; }
        public double FileSize { get; set; }

        /// <summary>
        /// Конструктор для створення об'єкта фільму з усіма характеристиками.
        /// </summary>
        public Movie(string title, string studio, string genre, int year,
                     Director director, string actors, string summary,
                     int rating, string filePath, double fileSize)
        {
            Title = title;
            Studio = studio;
            Genre = genre;
            Year = year;
            MovieDirector = director;
            Actors = actors;
            Summary = summary;
            Rating = rating;
            FilePath = filePath;
            FileSize = fileSize;
        }
    }
}