using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Film_library.Models;
using Film_library.Services;

namespace Film_library
{
    public partial class Form1 : Form
    {
        private readonly DataService _dataService;
        private List<Movie> _movies;
        private List<Director> _directors;

        // Елементи інтерфейсу програми
        private DataGridView _dataGridView;
        private TextBox _txtSearchTitle;
        private ComboBox _cmbFilterGenre;
        private ComboBox _cmbFilterDirector;
        private Button _btnSearch;
        private Button _btnReset;
        private Button _btnAdd;
        private Button _btnEdit;
        private Button _btnDelete;

        public Form1()
        {
            InitializeComponent();

            _dataService = new DataService();

            // Базові налаштування вікна
            this.Text = "Фільмотека — Особиста колекція";
            this.Size = new System.Drawing.Size(900, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.KeyPreview = true; // Для відстеження гарячих клавіш

            InitializeCustomComponents();
            LoadData();
        }

        /// <summary>
        /// Програмне створення та розміщення елементів інтерфейсу на формі.
        /// </summary>
        /// <summary>
        /// Програмне створення та розміщення елементів інтерфейсу на формі.
        /// </summary>
        private void InitializeCustomComponents()
        {
            // Очищаємо абсолютно всі старі елементи з дизайнера, 
            // щоб вони не накладалися на наш новий код і не ховали таблицю з даними!
            this.Controls.Clear();

            // --- Панель фільтрації (Верхня частина) ---
            var pnlFilter = new Panel
            {
                Dock = DockStyle.Top,
                Height = 75,
                BorderStyle = BorderStyle.FixedSingle
            };

            // Вмикаємо AutoSize = true, щоб текст ніколи не обрізався через масштабування еккрана
            var lblTitle = new Label { Text = "Назва:", Left = 15, Top = 28, AutoSize = true };
            _txtSearchTitle = new TextBox { Left = 70, Top = 24, Width = 140, Height = 27 };

            var lblGenre = new Label { Text = "Жанр:", Left = 230, Top = 28, AutoSize = true };
            _cmbFilterGenre = new ComboBox { Left = 280, Top = 24, Width = 130, Height = 27, DropDownStyle = ComboBoxStyle.DropDownList };

            var lblDirector = new Label { Text = "Режисер:", Left = 430, Top = 28, AutoSize = true };
            _cmbFilterDirector = new ComboBox { Left = 505, Top = 24, Width = 150, Height = 27, DropDownStyle = ComboBoxStyle.DropDownList };

            // Задаємо чітку висоту (Height = 32) для кнопок, щоб текст не вилазив за межі
            _btnSearch = new Button { Text = "Пошук", Left = 675, Top = 20, Width = 95, Height = 32 };
            _btnReset = new Button { Text = "Скинути", Left = 780, Top = 20, Width = 95, Height = 32 };

            _btnSearch.Click += BtnSearch_Click;
            _btnReset.Click += BtnReset_Click;

            pnlFilter.Controls.AddRange(new Control[] { lblTitle, _txtSearchTitle, lblGenre, _cmbFilterGenre, lblDirector, _cmbFilterDirector, _btnSearch, _btnReset });

            // --- Панель дій (Нижня частина з кнопками) ---
            var pnlActions = new Panel { Dock = DockStyle.Bottom, Height = 65 };
            _btnAdd = new Button { Text = "Додати фільм", Left = 15, Top = 15, Width = 140, Height = 35 };
            _btnEdit = new Button { Text = "Редагувати", Left = 165, Top = 15, Width = 120, Height = 35 };
            _btnDelete = new Button { Text = "Видалити", Left = 295, Top = 15, Width = 120, Height = 35 };

            _btnAdd.Click += BtnAdd_Click;
            _btnEdit.Click += BtnEdit_Click;
            _btnDelete.Click += BtnDelete_Click;

            pnlActions.Controls.AddRange(new Control[] { _btnAdd, _btnEdit, _btnDelete });

            // --- Таблиця відображення даних (Центр) ---
            _dataGridView = new DataGridView
            {
                Dock = DockStyle.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };

            // Додаємо елементи на головне вікно в строгому порядку шарів
            this.Controls.Add(_dataGridView);
            this.Controls.Add(pnlFilter);
            this.Controls.Add(pnlActions);

            // Підключаємо гарячі клавіші
            this.KeyDown += Form1_KeyDown;
        }

        private void LoadData()
        {
            _directors = _dataService.LoadDirectors();
            _movies = _dataService.LoadMovies();

            _cmbFilterDirector.Items.Clear();
            _cmbFilterDirector.Items.Add("Всі режисери");
            foreach (var d in _directors) _cmbFilterDirector.Items.Add(d.ToString());
            _cmbFilterDirector.SelectedIndex = 0;

            _cmbFilterGenre.Items.Clear();
            _cmbFilterGenre.Items.Add("Всі жанри");
            var genres = _movies.Select(m => m.Genre).Distinct().ToArray();
            _cmbFilterGenre.Items.AddRange(genres);
            _cmbFilterGenre.SelectedIndex = 0;

            UpdateGrid(_movies);
        }

        private void UpdateGrid(List<Movie> displayList)
        {
            _dataGridView.DataSource = null;
            _dataGridView.DataSource = displayList.Select(m => new
            {
                Назва = m.Title,
                Жанр = m.Genre,
                Рік = m.Year,
                Режисер = m.MovieDirector?.ToString() ?? "Не вказано",
                Оцінка = $"{m.Rating}/10",
                Розмір = $"{m.FileSize} ГБ",
                Шлях = m.FilePath
            }).ToList();
        }

        private void BtnSearch_Click(object sender, EventArgs e)
        {
            var filtered = _movies.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(_txtSearchTitle.Text))
                filtered = filtered.Where(m => m.Title.IndexOf(_txtSearchTitle.Text, StringComparison.OrdinalIgnoreCase) >= 0);

            if (_cmbFilterGenre.SelectedIndex > 0)
                filtered = filtered.Where(m => m.Genre == _cmbFilterGenre.SelectedItem.ToString());

            if (_cmbFilterDirector.SelectedIndex > 0)
                filtered = filtered.Where(m => m.MovieDirector?.ToString() == _cmbFilterDirector.SelectedItem.ToString());

            UpdateGrid(filtered.ToList());
        }

        private void BtnReset_Click(object sender, EventArgs e)
        {
            _txtSearchTitle.Clear();
            _cmbFilterGenre.SelectedIndex = 0;
            _cmbFilterDirector.SelectedIndex = 0;
            UpdateGrid(_movies);
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            // Відкриваємо форму додавання як модальне вікно
            using (var addForm = new MovieForm())
            {
                if (addForm.ShowDialog() == DialogResult.OK)
                {
                    var newMovie = addForm.MovieResult;

                    // Додаємо новий фільм у загальний список
                    _movies.Add(newMovie);

                    // Перевіряємо, чи є такий режисер у довіднику, якщо немає — додаємо
                    bool directorExists = _directors.Any(d =>
                        d.FirstName.Equals(newMovie.MovieDirector.FirstName, StringComparison.OrdinalIgnoreCase) &&
                        d.LastName.Equals(newMovie.MovieDirector.LastName, StringComparison.OrdinalIgnoreCase));

                    if (!directorExists)
                    {
                        _directors.Add(newMovie.MovieDirector);
                    }

                    // Зберігаємо оновлені списки у файли JSON
                    _dataService.SaveData(_movies, _directors);

                    // Оновлюємо таблицю та випадаючі списки фільтрів
                    LoadData();
                }
            }
        }

        private void BtnEdit_Click(object sender, EventArgs e)
        {
            // Перевіряємо, чи виділено якийсь рядок у таблиці
            if (_dataGridView.CurrentRow == null)
            {
                MessageBox.Show("Будь ласка, виберіть фільм для редагування.", "Увага", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // Отримуємо індекс виділеного фільму
            int selectedIndex = _dataGridView.CurrentRow.Index;
            var movieToEdit = _movies[selectedIndex];

            // Відкриваємо форму, передаючи туди дані обраного фільму для редагування
            using (var editForm = new MovieForm(movieToEdit))
            {
                if (editForm.ShowDialog() == DialogResult.OK)
                {
                    // Замінюємо старий об'єкт фільму на оновлений
                    _movies[selectedIndex] = editForm.MovieResult;

                    // Перевіряємо та оновлюємо довідник режисерів
                    var updatedDirector = editForm.MovieResult.MovieDirector;
                    bool directorExists = _directors.Any(d =>
                        d.FirstName.Equals(updatedDirector.FirstName, StringComparison.OrdinalIgnoreCase) &&
                        d.LastName.Equals(updatedDirector.LastName, StringComparison.OrdinalIgnoreCase));

                    if (!directorExists)
                    {
                        _directors.Add(updatedDirector);
                    }

                    // Перезаписуємо дані у JSON
                    _dataService.SaveData(_movies, _directors);

                    // Перезавантажуємо інтерфейс
                    LoadData();
                }
            }
        }
        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (_dataGridView.CurrentRow == null) return;

            var result = MessageBox.Show("Ви впевнені, що хочете видалити цей фільм із колекції?",
                "Підтвердження", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                int index = _dataGridView.CurrentRow.Index;
                _movies.RemoveAt(index);
                _dataService.SaveData(_movies, _directors);
                UpdateGrid(_movies);
            }
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                BtnSearch_Click(this, EventArgs.Empty);
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.Escape)
            {
                BtnReset_Click(this, EventArgs.Empty);
                e.Handled = true;
            }
        }
    }
}