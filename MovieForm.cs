using System;
using System.Windows.Forms;
using Film_library.Models;

namespace Film_library
{
    public class MovieForm : Form
    {
        private TextBox _txtTitle;
        private TextBox _txtStudio;
        private ComboBox _cmbGenre;
        private NumericUpDown _numYear;
        private TextBox _txtActors;
        private TextBox _txtSummary;
        private NumericUpDown _numRating;
        private TextBox _txtTrailerUrl;

        // ЗМІНА ТУТ: Елемент для введення тривалості
        private NumericUpDown _numDuration;

        private TextBox _txtDirFirstName;
        private TextBox _txtDirLastName;
        private TextBox _txtDirCountry;

        private Button _btnSave;
        private Button _btnCancel;

        public Movie MovieResult { get; private set; }

        public MovieForm(Movie movieToEdit = null)
        {
            this.Text = movieToEdit == null ? "Додавання нового фільму" : "Редагування фільму";
            this.Size = new System.Drawing.Size(480, 560); // Трохи підправили висоту під нове поле
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.StartPosition = FormStartPosition.CenterParent;
            this.KeyPreview = true;

            InitializeFormComponents();

            if (movieToEdit != null)
                FillFormWithData(movieToEdit);

            this.KeyDown += MovieForm_KeyDown;
        }

        private void InitializeFormComponents()
        {
            int top = 15;
            int labelLeft = 20;
            int inputLeft = 160;
            int inputWidth = 260;

            // Назва
            var lblTitle = new Label { Text = "Назва фільму *:", Left = labelLeft, Top = top, Width = 130 };
            _txtTitle = new TextBox { Left = inputLeft, Top = top - 3, Width = inputWidth };
            this.Controls.AddRange(new Control[] { lblTitle, _txtTitle });

            // Студія
            top += 35;
            var lblStudio = new Label { Text = "Кіностудія:", Left = labelLeft, Top = top, Width = 130 };
            _txtStudio = new TextBox { Left = inputLeft, Top = top - 3, Width = inputWidth };
            this.Controls.AddRange(new Control[] { lblStudio, _txtStudio });

            // Жанр
            top += 35;
            var lblGenre = new Label { Text = "Жанр *:", Left = labelLeft, Top = top, Width = 130 };
            _cmbGenre = new ComboBox { Left = inputLeft, Top = top - 3, Width = inputWidth, DropDownStyle = ComboBoxStyle.DropDownList };
            _cmbGenre.Items.AddRange(new string[] { "Бойовик", "Комедія", "Драма", "Триллер", "Фантастика", "Жахи" });
            if (_cmbGenre.Items.Count > 0) _cmbGenre.SelectedIndex = 0;
            this.Controls.AddRange(new Control[] { lblGenre, _cmbGenre });

            // Рік
            top += 35;
            var lblYear = new Label { Text = "Рік випуску:", Left = labelLeft, Top = top, Width = 130 };
            _numYear = new NumericUpDown { Left = inputLeft, Top = top - 3, Width = 80, Minimum = 1895, Maximum = DateTime.Now.Year, Value = DateTime.Now.Year };
            this.Controls.AddRange(new Control[] { lblYear, _numYear });

            // Оцінка
            var lblRating = new Label { Text = "Оцінка (1-10):", Left = 260, Top = top, Width = 90 };
            _numRating = new NumericUpDown { Left = 350, Top = top - 3, Width = 70, Minimum = 1, Maximum = 10, Value = 8 };
            this.Controls.AddRange(new Control[] { lblRating, _numRating });

            // Група Режисера
            top += 40;
            var grpDirector = new GroupBox { Text = " Інформація про режисера ", Left = labelLeft, Top = top, Width = 410, Height = 120 };
            grpDirector.Controls.Add(new Label { Text = "Ім'я *:", Left = 15, Top = 25, Width = 100 });
            _txtDirFirstName = new TextBox { Left = 120, Top = 22, Width = 260 };
            grpDirector.Controls.Add(new Label { Text = "Прізвище *:", Left = 15, Top = 55, Width = 100 });
            _txtDirLastName = new TextBox { Left = 120, Top = 52, Width = 260 };
            grpDirector.Controls.Add(new Label { Text = "Країна:", Left = 15, Top = 85, Width = 100 });
            _txtDirCountry = new TextBox { Left = 120, Top = 82, Width = 260 };
            grpDirector.Controls.AddRange(new Control[] { _txtDirFirstName, _txtDirLastName, _txtDirCountry });
            this.Controls.Add(grpDirector);

            top += 125;
            // Актори
            var lblActors = new Label { Text = "Головні актори:", Left = labelLeft, Top = top, Width = 130 };
            _txtActors = new TextBox { Left = inputLeft, Top = top - 3, Width = inputWidth };
            this.Controls.AddRange(new Control[] { lblActors, _txtActors });

            // Опис
            top += 35;
            var lblSummary = new Label { Text = "Короткий зміст:", Left = labelLeft, Top = top, Width = 130 };
            _txtSummary = new TextBox { Left = inputLeft, Top = top - 3, Width = inputWidth };
            this.Controls.AddRange(new Control[] { lblSummary, _txtSummary });

            // ЗМІНА ТУТ: Додаємо поле введення тривалості фільму
            top += 35;
            var lblDuration = new Label { Text = "Тривалість (хв) *:", Left = labelLeft, Top = top, Width = 130 };
            _numDuration = new NumericUpDown { Left = inputLeft, Top = top - 3, Width = 80, Minimum = 1, Maximum = 1000, Value = 120 };
            this.Controls.AddRange(new Control[] { lblDuration, _numDuration });

            // Посилання на трейлер
            top += 35;
            var lblUrl = new Label { Text = "Посилання на трейлер *:", Left = labelLeft, Top = top, Width = 140 };
            _txtTrailerUrl = new TextBox { Left = inputLeft, Top = top - 3, Width = inputWidth };
            this.Controls.AddRange(new Control[] { lblUrl, _txtTrailerUrl });

            // Кнопки
            top += 45;
            _btnSave = new Button { Text = "Зберегти (Enter)", Left = 160, Top = top, Width = 120, Height = 30 };
            _btnCancel = new Button { Text = "Скасувати (Esc)", Left = 295, Top = top, Width = 125, Height = 30 };

            _btnSave.Click += BtnSave_Click;
            _btnCancel.Click += (s, e) => this.DialogResult = DialogResult.Cancel;

            this.Controls.AddRange(new Control[] { _btnSave, _btnCancel });
        }

        private void FillFormWithData(Movie movie)
        {
            _txtTitle.Text = movie.Title;
            _txtStudio.Text = movie.Studio;
            _cmbGenre.Text = movie.Genre;
            _numYear.Value = movie.Year;
            _numRating.Value = movie.Rating;
            _txtActors.Text = movie.Actors;
            _txtSummary.Text = movie.Summary;
            _numDuration.Value = movie.Duration; // Завантажуємо тривалість
            _txtTrailerUrl.Text = movie.FilePath;
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(_txtTitle.Text) ||
                string.IsNullOrWhiteSpace(_cmbGenre.Text) ||
                string.IsNullOrWhiteSpace(_txtDirFirstName.Text) ||
                string.IsNullOrWhiteSpace(_txtDirLastName.Text) ||
                string.IsNullOrWhiteSpace(_txtTrailerUrl.Text))
            {
                MessageBox.Show("Будь ласка, заповніть усі поля, позначені зірочкою (*).",
                    "Помилка валідації", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var director = new Director(
                _txtDirFirstName.Text.Trim(),
                _txtDirLastName.Text.Trim(),
                string.IsNullOrWhiteSpace(_txtDirCountry.Text) ? "Невідомо" : _txtDirCountry.Text.Trim()
            );

            MovieResult = new Movie(
                _txtTitle.Text.Trim(),
                string.IsNullOrWhiteSpace(_txtStudio.Text) ? "Невідомо" : _txtStudio.Text.Trim(),
                _cmbGenre.Text.Trim(),
                (int)_numYear.Value,
                director,
                string.IsNullOrWhiteSpace(_txtActors.Text) ? "Не вказано" : _txtActors.Text.Trim(),
                string.IsNullOrWhiteSpace(_txtSummary.Text) ? "Немає опису" : _txtSummary.Text.Trim(),
                (int)_numRating.Value,
                _txtTrailerUrl.Text.Trim(),
                (int)_numDuration.Value // Передаємо хвилини
            );

            this.DialogResult = DialogResult.OK;
        }

        private void MovieForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) { BtnSave_Click(this, EventArgs.Empty); e.Handled = true; }
            else if (e.KeyCode == Keys.Escape) { this.DialogResult = DialogResult.Cancel; e.Handled = true; }
        }
    }
}