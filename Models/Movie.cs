using System;

namespace Film_library.Models
{
    /// <summary>
    /// Клас, що описує сутність Фільму у відеотеці.
    /// </summary>
    public class Movie
    {
        public string Title { get; set; }
        public string Studio { get; set; }
        public string Genre { get; set; }
        public int Year { get; set; }
        public Director MovieDirector { get; set; }
        public string Actors { get; set; }
        public string Summary { get; set; }
        public int Rating { get; set; }
        public string FilePath { get; set; }

        // ЗМІНА ТУТ: Замість FileSize додаємо тривалість у хвилинах
        public int Duration { get; set; }

        // Порожній конструктор для JSON
        public Movie() { }

        // Оновлений конструктор
        public Movie(string title, string studio, string genre, int year,
                     Director director, string actors, string summary,
                     int rating, string filePath, int duration)
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
            Duration = duration;
        }
    }
}