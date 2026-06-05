using System;
using System.Windows.Forms;
using Film_library.Models;

namespace Film_library
{
    /// <summary>
    /// Вікно для додавання або редагування фільму (максимально простора та зручна версія).
    /// </summary>
    public class MovieForm : Form
    {
        private TextBox _txtTitle;
        private TextBox _txtStudio;
        private ComboBox _cmbGenre;
        private NumericUpDown _numYear;
        private TextBox _txtActors; // Збільшене поле
        private TextBox _txtSummary; // Значно збільшене поле
        private NumericUpDown _numRating;
        private TextBox _txtTrailerUrl;
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

            // ЗБІЛЬШЕННЯ ВІКНА: Зробили форму набагато ширшою та вищою для твого комфорту
            this.Size = new System.Drawing.Size(555, 720);
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
            int top = 20;
            int labelLeft = 20;
            int inputLeft = 180;
            int inputWidth = 320; // РОЗШИРЕННЯ ПОЛІВ: Тепер довгі посилання та назви вміщаються легко

            // Назва
            var lblTitle = new Label { Text = "Назва фільму *:", Left = labelLeft, Top = top, AutoSize = true };
            _txtTitle = new TextBox { Left = inputLeft, Top = top - 3, Width = inputWidth };
            this.Controls.AddRange(new Control[] { lblTitle, _txtTitle });

            // Студія
            top += 35;
            var lblStudio = new Label { Text = "Кіностудія:", Left = labelLeft, Top = top, AutoSize = true };
            _txtStudio = new TextBox { Left = inputLeft, Top = top - 3, Width = inputWidth };
            this.Controls.AddRange(new Control[] { lblStudio, _txtStudio });

            // Жанр
            top += 35;
            var lblGenre = new Label { Text = "Жанр *:", Left = labelLeft, Top = top, AutoSize = true };
            _cmbGenre = new ComboBox { Left = inputLeft, Top = top - 3, Width = inputWidth, DropDownStyle = ComboBoxStyle.DropDownList };
            _cmbGenre.Items.AddRange(new string[] { "Бойовик", "Комедія", "Драма", "Триллер", "Фантастика", "Жахи" });
            if (_cmbGenre.Items.Count > 0) _cmbGenre.SelectedIndex = 0;
            this.Controls.AddRange(new Control[] { lblGenre, _cmbGenre });

            // Рік випуску
            top += 35;
            var lblYear = new Label { Text = "Рік випуску:", Left = labelLeft, Top = top, AutoSize = true };
            _numYear = new NumericUpDown { Left = inputLeft, Top = top - 3, Width = 90, Minimum = 1895, Maximum = DateTime.Now.Year, Value = DateTime.Now.Year };
            this.Controls.AddRange(new Control[] { lblYear, _numYear });

            // Оцінка
            top += 35;
            var lblRating = new Label { Text = "Оцінка (1-10):", Left = labelLeft, Top = top, AutoSize = true };
            _numRating = new NumericUpDown { Left = inputLeft, Top = top - 3, Width = 90, Minimum = 1, Maximum = 10, Value = 8 };
            this.Controls.AddRange(new Control[] { lblRating, _numRating });

            // --- Група: Дані Режисера ---
            top += 40;
            var grpDirector = new GroupBox { Text = " Інформація про режисера ", Left = labelLeft, Top = top, Width = 495, Height = 130 };

            grpDirector.Controls.Add(new Label { Text = "Ім'я *:", Left = 15, Top = 25, AutoSize = true });
            _txtDirFirstName = new TextBox { Left = 140, Top = 22, Width = 330 };

            grpDirector.Controls.Add(new Label { Text = "Прізвище *:", Left = 15, Top = 55, AutoSize = true });
            _txtDirLastName = new TextBox { Left = 140, Top = 52, Width = 330 };

            grpDirector.Controls.Add(new Label { Text = "Країна:", Left = 15, Top = 85, AutoSize = true });
            _txtDirCountry = new TextBox { Left = 140, Top = 82, Width = 330 };

            grpDirector.Controls.AddRange(new Control[] { _txtDirFirstName, _txtDirLastName, _txtDirCountry });
            this.Controls.Add(grpDirector);

            top += 145;
            // --- ПОКРАЩЕННЯ №1: Велике багаторядкове поле для акторів ---
            var lblActors = new Label { Text = "Головні актори:", Left = labelLeft, Top = top, AutoSize = true };
            _txtActors = new TextBox
            {
                Left = inputLeft,
                Top = top - 3,
                Width = inputWidth,
                Multiline = true,
                Height = 45, // Дозволяє вмістити 2-3 рядки акторів без проблем
                ScrollBars = ScrollBars.Vertical
            };
            this.Controls.AddRange(new Control[] { lblActors, _txtActors });

            // --- ПОКРАЩЕННЯ №2: Величезне глибоке поле для сюжету фільму ---
            top += 55;
            var lblSummary = new Label { Text = "Короткий зміст:", Left = labelLeft, Top = top, AutoSize = true };
            _txtSummary = new TextBox
            {
                Left = inputLeft,
                Top = top - 3,
                Width = inputWidth,
                Multiline = true,
                Height = 110, // Суттєво збільшили висоту під гарний опис
                ScrollBars = ScrollBars.Vertical
            };
            this.Controls.AddRange(new Control[] { lblSummary, _txtSummary });

            // Тривалість
            top += 120;
            var lblDuration = new Label { Text = "Тривалість (хв) *:", Left = labelLeft, Top = top, AutoSize = true };
            _numDuration = new NumericUpDown { Left = inputLeft, Top = top - 3, Width = 90, Minimum = 1, Maximum = 1000, Value = 120 };
            this.Controls.AddRange(new Control[] { lblDuration, _numDuration });

            // Посилання на трейлер
            top += 35;
            var lblUrl = new Label { Text = "Посилання на трейлер *:", Left = labelLeft, Top = top, AutoSize = true };
            _txtTrailerUrl = new TextBox { Left = inputLeft, Top = top - 3, Width = inputWidth };
            this.Controls.AddRange(new Control[] { lblUrl, _txtTrailerUrl });

            // Кнопки дій (масштабовані та вирівняні по правому краю)
            top += 50;
            _btnSave = new Button { Text = "Зберегти (Enter)", Left = 210, Top = top, Width = 140, Height = 35 };
            _btnCancel = new Button { Text = "Скасувати (Esc)", Left = 360, Top = top, Width = 140, Height = 35 };

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
            _numDuration.Value = movie.Duration;
            _txtTrailerUrl.Text = movie.FilePath;

            if (movie.MovieDirector != null)
            {
                _txtDirFirstName.Text = movie.MovieDirector.FirstName;
                _txtDirLastName.Text = movie.MovieDirector.LastName;
                _txtDirCountry.Text = movie.MovieDirector.Country;
            }
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
                (int)_numDuration.Value
            );

            this.DialogResult = DialogResult.OK;
        }

        private void MovieForm_KeyDown(object sender, KeyEventArgs e)
        {
            // РОЗУМНИЙ ЗАХИСТ: Тепер натискання клавіші Enter не буде випадково закривати форму, 
            // якщо ти в цей момент набираєш текст всередині багаторядкових полів Акторів або Опису!
            if (e.KeyCode == Keys.Enter && !_txtSummary.Focused && !_txtActors.Focused)
            {
                BtnSave_Click(this, EventArgs.Empty);
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.Escape)
            {
                this.DialogResult = DialogResult.Cancel;
                e.Handled = true;
            }
        }
    }
}