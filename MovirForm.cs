using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using Film_library.Models;

namespace Film_library
{
    /// <summary>
    /// Вікно для додавання або редагування фільму.
    /// </summary>
    public class MovieForm : Form
    {
        // Поля введення даних
        private TextBox _txtTitle;
        private TextBox _txtStudio;
        private ComboBox _cmbGenre;
        private NumericUpDown _numYear;
        private TextBox _txtActors;
        private TextBox _txtSummary;
        private NumericUpDown _numRating;
        private TextBox _txtFilePath;
        private Label _lblFileSize;

        // Поля для режисера
        private TextBox _txtDirFirstName;
        private TextBox _txtDirLastName;
        private TextBox _txtDirCountry;

        private Button _btnBrowse;
        private Button _btnSave;
        private Button _btnCancel;

        private double _fileSizeGb = 0;

        // Властивість, яка поверне створений або змінений фільм назад у головне вікно
        public Movie MovieResult { get; private set; }

        public MovieForm(Movie movieToEdit = null)
        {
            this.Text = movieToEdit == null ? "Додавання нового фільму" : "Редагування фільму";
            this.Size = new System.Drawing.Size(480, 580);
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
            _cmbGenre = new ComboBox { Left = inputLeft, Top = top - 3, Width = inputWidth, DropDownStyle = ComboBoxStyle.DropDown };
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

            // --- Група: Дані Режисера ---
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

            // Шлях до файлу (Валідація та Огляд)
            top += 35;
            var lblFile = new Label { Text = "Відеофайл *:", Left = labelLeft, Top = top, Width = 130 };
            _txtFilePath = new TextBox { Left = inputLeft, Top = top - 3, Width = 175, ReadOnly = true };
            _btnBrowse = new Button { Text = "Огляд...", Left = 345, Top = top - 5, Width = 75, Height = 25 };
            _btnBrowse.Click += BtnBrowse_Click;
            this.Controls.AddRange(new Control[] { lblFile, _txtFilePath, _btnBrowse });

            // Розмір файлу
            top += 30;
            _lblFileSize = new Label { Text = "Розмір файлу: 0 ГБ", Left = inputLeft, Top = top, Width = 260, Font = new System.Drawing.Font(this.Font, System.Drawing.FontStyle.Italic) };
            this.Controls.Add(_lblFileSize);

            // Кнопки знизу
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
            _txtFilePath.Text = movie.FilePath;
            _fileSizeGb = movie.FileSize;
            _lblFileSize.Text = $"Розмір файлу: {_fileSizeGb} ГБ";

            if (movie.MovieDirector != null)
            {
                _txtDirFirstName.Text = movie.MovieDirector.FirstName;
                _txtDirLastName.Text = movie.MovieDirector.LastName;
                _txtDirCountry.Text = movie.MovieDirector.Country;
            }
        }

        private void BtnBrowse_Click(object sender, EventArgs e)
        {
            using (var ofd = new OpenFileDialog())
            {
                ofd.Filter = "Відеофайли (*.mp4;*.mkv;*.avi)|*.mp4;*.mkv;*.avi|Усі файли (*.*)|*.*";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    _txtFilePath.Text = ofd.FileName;
                    var fileInfo = new FileInfo(ofd.FileName);
                    // Переводимо байти в гігабайти з округленням до 2 знаків
                    _fileSizeGb = Math.Round((double)fileInfo.Length / (1024 * 1024 * 1024), 2);
                    _lblFileSize.Text = $"Розмір файлу: {_fileSizeGb} ГБ";
                }
            }
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            // Валідація даних користувача (Захист від дурака)
            if (string.IsNullOrWhiteSpace(_txtTitle.Text) ||
                string.IsNullOrWhiteSpace(_cmbGenre.Text) ||
                string.IsNullOrWhiteSpace(_txtDirFirstName.Text) ||
                string.IsNullOrWhiteSpace(_txtDirLastName.Text) ||
                string.IsNullOrWhiteSpace(_txtFilePath.Text))
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
                _txtFilePath.Text,
                _fileSizeGb
            );

            this.DialogResult = DialogResult.OK;
        }

        private void MovieForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
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