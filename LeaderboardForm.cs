using System;
using System.Drawing;
using System.Windows.Forms;
using BattleOfEntities.Database;

namespace BattleOfEntities
{
    public partial class LeaderboardForm : Form
    {
        private DatabaseManager _db;
        private DataGridView dgvLeaderboard;

        public LeaderboardForm(DatabaseManager db)
        {
            _db = db;
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "Таблица лидеров";
            this.Size = new Size(1000, 600);
            this.MinimumSize = new Size(900, 500);
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = Color.FromArgb(20, 20, 30);
            this.FormBorderStyle = FormBorderStyle.FixedSingle;

            // Заголовок
            Label lblTitle = new Label
            {
                Text = "🏆 ЛУЧШИЕ ИГРОКИ 🏆",
                Font = new Font("Arial", 20, FontStyle.Bold),
                ForeColor = Color.Gold,
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Top,
                Height = 60,
                BackColor = Color.Transparent
            };

            // Таблица
            dgvLeaderboard = new DataGridView
            {
                Dock = DockStyle.Fill,
                BackgroundColor = Color.FromArgb(40, 40, 50),
                ForeColor = Color.White,
                GridColor = Color.FromArgb(80, 80, 90),
                Font = new Font("Arial", 11),
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                RowHeadersVisible = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BorderStyle = BorderStyle.FixedSingle
            };

            // Панель для кнопки
            Panel bottomPanel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 70,
                BackColor = Color.FromArgb(30, 30, 40)
            };

            // Кнопка закрытия
            Button btnClose = new Button
            {
                Text = "✖ ЗАКРЫТЬ",
                Size = new Size(200, 45),
                Font = new Font("Arial", 12, FontStyle.Bold),
                BackColor = Color.FromArgb(80, 80, 100),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };

            btnClose.Location = new Point((bottomPanel.Width - btnClose.Width) / 2, 12);
            btnClose.Anchor = AnchorStyles.Top;
            btnClose.Click += (s, e) => this.Close();

            bottomPanel.Controls.Add(btnClose);

            // Добавляем всё на форму
            this.Controls.Add(dgvLeaderboard);
            this.Controls.Add(bottomPanel);
            this.Controls.Add(lblTitle);

            // Загружаем данные ПОСЛЕ того как форма создана
            this.Shown += LeaderboardForm_Shown;
        }

        private void LeaderboardForm_Shown(object sender, EventArgs e)
        {
            LoadLeaderboard();

            // Центрируем кнопку
            foreach (Control ctrl in this.Controls)
            {
                if (ctrl is Panel panel)
                {
                    foreach (Control btn in panel.Controls)
                    {
                        if (btn is Button button)
                        {
                            button.Location = new Point((panel.Width - button.Width) / 2, 12);
                        }
                    }
                }
            }
        }

        private void LoadLeaderboard()
        {
            try
            {
                var records = _db.GetLeaderboard(20);

                dgvLeaderboard.Columns.Clear();

                dgvLeaderboard.Columns.Add("Rank", "🏆 Место");
                dgvLeaderboard.Columns.Add("PlayerName", "👤 Игрок");
                dgvLeaderboard.Columns.Add("CharacterType", "⚔️ Тип");
                dgvLeaderboard.Columns.Add("EnemiesKilled", "💀 Убито");
                dgvLeaderboard.Columns.Add("TotalDamage", "⚡ Урон");
                dgvLeaderboard.Columns.Add("PlayTime", "⏱️ Время");
                dgvLeaderboard.Columns.Add("CompletionDate", "📅 Дата");

                // Настройка ширины колонок
                dgvLeaderboard.Columns["Rank"].FillWeight = 8;
                dgvLeaderboard.Columns["PlayerName"].FillWeight = 15;
                dgvLeaderboard.Columns["CharacterType"].FillWeight = 10;
                dgvLeaderboard.Columns["EnemiesKilled"].FillWeight = 10;
                dgvLeaderboard.Columns["TotalDamage"].FillWeight = 15;
                dgvLeaderboard.Columns["PlayTime"].FillWeight = 10;
                dgvLeaderboard.Columns["CompletionDate"].FillWeight = 20;

                // Стиль заголовков
                dgvLeaderboard.EnableHeadersVisualStyles = false;
                dgvLeaderboard.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(60, 60, 70);
                dgvLeaderboard.ColumnHeadersDefaultCellStyle.ForeColor = Color.Gold;
                dgvLeaderboard.ColumnHeadersDefaultCellStyle.Font = new Font("Arial", 10, FontStyle.Bold);
                dgvLeaderboard.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

                // Стиль ячеек
                dgvLeaderboard.RowsDefaultCellStyle.BackColor = Color.FromArgb(45, 45, 55);
                dgvLeaderboard.RowsDefaultCellStyle.ForeColor = Color.White;
                dgvLeaderboard.RowsDefaultCellStyle.SelectionBackColor = Color.FromArgb(80, 80, 120);
                dgvLeaderboard.RowsDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dgvLeaderboard.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(35, 35, 45);

                if (records.Count == 0)
                {
                    dgvLeaderboard.Rows.Add("", "Нет данных", "", "", "", "", "");
                    dgvLeaderboard.ClearSelection();
                }
                else
                {
                    int rank = 1;
                    foreach (var record in records)
                    {
                        string formattedDate = "";
                        if (DateTime.TryParse(record.CompletionDate, out DateTime date))
                        {
                            formattedDate = date.ToString("dd.MM.yyyy HH:mm");
                        }
                        else
                        {
                            formattedDate = record.CompletionDate;
                        }

                        dgvLeaderboard.Rows.Add(
                            rank++,
                            record.PlayerName,
                            record.CharacterType,
                            record.EnemiesKilled,
                            record.TotalDamageDealt,
                            record.PlayTime,
                            formattedDate
                        );
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки таблицы лидеров: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}