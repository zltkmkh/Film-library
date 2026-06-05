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

            this.Text = "Фільмотека — Особиста колекція";
            this.Size = new System.Drawing.Size(950, 600); // Трохи розширили вікно для лінків
            this.StartPosition = FormStartPosition.CenterScreen;
            this.KeyPreview = true;

            InitializeCustomComponents();
            LoadData();
        }

        private void InitializeCustomComponents()
        {
            this.Controls.Clear();

            // Верхня панель
            var pnlFilter = new Panel { Dock = DockStyle.Top, Height = 75, BorderStyle = BorderStyle.FixedSingle };
            var lblTitle = new Label { Text = "Назва:", Left = 15, Top = 28, AutoSize = true };
            _txtSearchTitle = new TextBox { Left = 70, Top = 24, Width = 140, Height = 27 };
            var lblGenre = new Label { Text = "Жанр:", Left = 230, Top = 28, AutoSize = true };
            _cmbFilterGenre = new ComboBox { Left = 280, Top = 24, Width = 130, Height = 27, DropDownStyle = ComboBoxStyle.DropDownList };
            var lblDirector = new Label { Text = "Режисер:", Left = 430, Top = 28, AutoSize = true };
            _cmbFilterDirector = new ComboBox { Left = 505, Top = 24, Width = 150, Height = 27, DropDownStyle = ComboBoxStyle.DropDownList };
            _btnSearch = new Button { Text = "Пошук", Left = 675, Top = 20, Width = 95, Height = 32 };
            _btnReset = new Button { Text = "Скинути", Left = 780, Top = 20, Width = 95, Height = 32 };

            _btnSearch.Click += BtnSearch_Click;
            _btnReset.Click += BtnReset_Click;
            pnlFilter.Controls.AddRange(new Control[] { lblTitle, _txtSearchTitle, lblGenre, _cmbFilterGenre, lblDirector, _cmbFilterDirector, _btnSearch, _btnReset });

            // Нижня панель
            var pnlActions = new Panel { Dock = DockStyle.Bottom, Height = 65 };
            _btnAdd = new Button { Text = "Додати фільм", Left = 15, Top = 15, Width = 140, Height = 35 };
            _btnEdit = new Button { Text = "Редагувати", Left = 165, Top = 15, Width = 120, Height = 35 };
            _btnDelete = new Button { Text = "Видалити", Left = 295, Top = 15, Width = 120, Height = 35 };

            _btnAdd.Click += BtnAdd_Click;
            _btnEdit.Click += BtnEdit_Click;
            _btnDelete.Click += BtnDelete_Click;
            pnlActions.Controls.AddRange(new Control[] { _btnAdd, _btnEdit, _btnDelete });

            // Таблиця
            _dataGridView = new DataGridView
            {
                Dock = DockStyle.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };

            // ЗМІНА ТУТ: Підключаємо клік на клітинку таблиці для відкриття посилань!
            _dataGridView.CellClick += DataGridView_CellClick;

            this.Controls.Add(_dataGridView);
            this.Controls.Add(pnlFilter);
            this.Controls.Add(pnlActions);

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

                // ЗМІНА ТУТ: Відображаємо тривалість замість розміру файлу
                Тривалість = $"{m.Duration} хв",

                Трейлер = m.FilePath
            }).ToList();
        }

        // ДОДАЄМО НОВИЙ МЕТОД: Відкриття трейлера за кліком!
        private void DataGridView_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            // Перевіряємо, чи клікнули по реальному рядку і саме по колонці "Трейлер"
            if (e.RowIndex >= 0 && _dataGridView.Columns[e.ColumnIndex].Name == "Трейлер")
            {
                string target = _dataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value?.ToString();

                if (!string.IsNullOrWhiteSpace(target))
                {
                    try
                    {
                        // Магічний код, який запускає браузер або плеєр в залежності від того, що всередині (URL чи файл)
                        System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                        {
                            FileName = target,
                            UseShellExecute = true
                        });
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Не вдалося відкрити посилання або файл: {ex.Message}", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
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
            using (var addForm = new MovieForm())
            {
                if (addForm.ShowDialog() == DialogResult.OK)
                {
                    var newMovie = addForm.MovieResult;
                    _movies.Add(newMovie);
                    bool directorExists = _directors.Any(d => d.FirstName.Equals(newMovie.MovieDirector.FirstName, StringComparison.OrdinalIgnoreCase) && d.LastName.Equals(newMovie.MovieDirector.LastName, StringComparison.OrdinalIgnoreCase));
                    if (!directorExists) _directors.Add(newMovie.MovieDirector);
                    _dataService.SaveData(_movies, _directors);
                    LoadData();
                }
            }
        }

        private void BtnEdit_Click(object sender, EventArgs e)
        {
            if (_dataGridView.CurrentRow == null)
            {
                MessageBox.Show("Будь ласка, виберіть фільм для редагування.", "Увага", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            int selectedIndex = _dataGridView.CurrentRow.Index;
            var movieToEdit = _movies[selectedIndex];
            using (var editForm = new MovieForm(movieToEdit))
            {
                if (editForm.ShowDialog() == DialogResult.OK)
                {
                    _movies[selectedIndex] = editForm.MovieResult;
                    var updatedDirector = editForm.MovieResult.MovieDirector;
                    bool directorExists = _directors.Any(d => d.FirstName.Equals(updatedDirector.FirstName, StringComparison.OrdinalIgnoreCase) && d.LastName.Equals(updatedDirector.LastName, StringComparison.OrdinalIgnoreCase));
                    if (!directorExists) _directors.Add(updatedDirector);
                    _dataService.SaveData(_movies, _directors);
                    LoadData();
                }
            }
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (_dataGridView.CurrentRow == null) return;
            var result = MessageBox.Show("Ви впевнені, що хочете видалити цей фільм із колекції?", "Підтвердження", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
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
            if (e.KeyCode == Keys.Enter) { BtnSearch_Click(this, EventArgs.Empty); e.Handled = true; }
            else if (e.KeyCode == Keys.Escape) { BtnReset_Click(this, EventArgs.Empty); e.Handled = true; }
        }
    }
}