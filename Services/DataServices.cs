using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Film_library.Models;

namespace Film_library.Services
{
    /// <summary>
    /// Сервіс для роботи з файловою системою та збереження даних у форматі JSON.
    /// </summary>
    public class DataService
    {
        // Назви файлів, куди будуть зберігатися наші списки
        private readonly string _moviesFilePath = "movies.json";
        private readonly string _directorsFilePath = "directors.json";

        /// <summary>
        /// Зберігає списки фільмів та режисерів у текстові файли формату JSON.
        /// </summary>
        /// <param name="movies">Список об'єктів фільмів для збереження.</param>
        /// <param name="directors">Список об'єктів режисерів для збереження.</param>
        public void SaveData(List<Movie> movies, List<Director> directors)
        {
            try
            {
                // Налаштування для того, щоб JSON-файл був красивим і читабельним (з відступами)
                var options = new JsonSerializerOptions { WriteIndented = true };

                // Серіалізація та запис фільмів
                string moviesJson = JsonSerializer.Serialize(movies, options);
                File.WriteAllText(_moviesFilePath, moviesJson);

                // Серіалізація та запис режисерів
                string directorsJson = JsonSerializer.Serialize(directors, options);
                File.WriteAllText(_directorsFilePath, directorsJson);
            }
            catch (Exception)
            {
                // Захист програми від "падіння" у разі помилки доступу до файлу
            }
        }

        /// <summary>
        /// Завантажує список режисерів із файлу JSON.
        /// </summary>
        /// <returns>Список об'єктів режисерів або новий порожній список, якщо файл відсутній.</returns>
        public List<Director> LoadDirectors()
        {
            try
            {
                if (!File.Exists(_directorsFilePath))
                    return new List<Director>();

                string json = File.ReadAllText(_directorsFilePath);
                return JsonSerializer.Deserialize<List<Director>>(json) ?? new List<Director>();
            }
            catch (Exception)
            {
                return new List<Director>();
            }
        }

        /// <summary>
        /// Завантажує список фільмів із файлу JSON.
        /// </summary>
        /// <returns>Список об'єктів фільмів або новий порожній список, якщо файл відсутній.</returns>
        public List<Movie> LoadMovies()
        {
            try
            {
                if (!File.Exists(_moviesFilePath))
                    return new List<Movie>();

                string json = File.ReadAllText(_moviesFilePath);
                return JsonSerializer.Deserialize<List<Movie>>(json) ?? new List<Movie>();
            }
            catch (Exception)
            {
                return new List<Movie>();
            }
        }
    }
}