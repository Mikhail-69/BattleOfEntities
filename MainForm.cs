using System;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Reflection;
using BattleOfEntities.Models;
using BattleOfEntities.Database;

namespace BattleOfEntities
{
    public partial class MainForm : Form
    {
        private DatabaseManager _db;
        private TextBox txtPlayerName;
        private ComboBox cmbCharacterType;
        private Button btnNewGame;
        private Button btnLoadGame;
        private Button btnLeaderboard;
        private Button btnExit;
        private Label lblTitle;

        public MainForm()
        {

            InitializeComponent();
            _db = new DatabaseManager();

            // Запуск фоновой музыки
            SoundManager.PlayBackgroundMusic("battle_theme.wav");
        }

        private void InitializeComponent()
        {
            this.Text = "Battle of Entities - Главное меню";
            this.Size = new Size(550, 550);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(20, 20, 30);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            // Заголовок
            lblTitle = new Label
            {
                Text = "BATTLE OF ENTITIES",
                Font = new Font("Arial", 24, FontStyle.Bold),
                ForeColor = Color.Gold,
                BackColor = Color.Transparent,
                TextAlign = ContentAlignment.MiddleCenter,
                Size = new Size(450, 60),
                Location = new Point(25, 30)
            };

            // Поле ввода имени
            Label lblName = new Label
            {
                Text = "Имя игрока:",
                ForeColor = Color.White,
                Font = new Font("Arial", 12),
                Location = new Point(50, 120),
                Size = new Size(150, 30)
            };

            txtPlayerName = new TextBox
            {
                Location = new Point(200, 120),
                Size = new Size(200, 30),
                Font = new Font("Arial", 12),
                BackColor = Color.FromArgb(40, 40, 50),
                ForeColor = Color.White
            };

            // Выбор типа персонажа
            Label lblType = new Label
            {
                Text = "Тип персонажа:",
                ForeColor = Color.White,
                Font = new Font("Arial", 12),
                Location = new Point(50, 170),
                Size = new Size(150, 30)
            };

            cmbCharacterType = new ComboBox
            {
                Location = new Point(200, 170),
                Size = new Size(200, 30),
                Font = new Font("Arial", 12),
                DropDownStyle = ComboBoxStyle.DropDownList,
                BackColor = Color.FromArgb(40, 40, 50),
                ForeColor = Color.White
            };
            cmbCharacterType.Items.AddRange(new[] { "Монстр", "Демон", "Маг" });
            cmbCharacterType.SelectedIndex = 0;

            // Кнопка Новая игра
            btnNewGame = new Button
            {
                Text = "Новая игра",
                Location = new Point(150, 230),
                Size = new Size(200, 50),
                Font = new Font("Arial", 14, FontStyle.Bold),
                BackColor = Color.FromArgb(60, 120, 60),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnNewGame.Click += BtnNewGame_Click;

            // Кнопка Загрузить игру
            btnLoadGame = new Button
            {
                Text = "Загрузить игру",
                Location = new Point(150, 290),  // сдвинуто
                Size = new Size(200, 50),
                Font = new Font("Arial", 14, FontStyle.Bold),
                BackColor = Color.FromArgb(70, 70, 150),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnLoadGame.Click += BtnLoadGame_Click;

            // Кнопка Достижения (НОВАЯ)
            Button btnAchievements = new Button
            {
                Text = "🏆 Достижения",
                Location = new Point(150, 350),
                Size = new Size(200, 40),
                Font = new Font("Arial", 12),
                BackColor = Color.FromArgb(100, 80, 40),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnAchievements.Click += BtnAchievements_Click;

            // Кнопка Таблица лидеров
            btnLeaderboard = new Button
            {
                Text = "Таблица лидеров",
                Location = new Point(150, 400),
                Size = new Size(200, 40),
                Font = new Font("Arial", 12),
                BackColor = Color.FromArgb(80, 80, 80),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnLeaderboard.Click += BtnLeaderboard_Click;

            // Кнопка Выход
            btnExit = new Button
            {
                Text = "Выход",
                Location = new Point(150, 460),
                Size = new Size(200, 30),
                Font = new Font("Arial", 10),
                BackColor = Color.FromArgb(120, 40, 40),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnExit.Click += (s, e) => Application.Exit();

            this.Controls.AddRange(new Control[] {
        lblTitle, lblName, txtPlayerName, lblType,
        cmbCharacterType, btnNewGame, btnLoadGame,
        btnAchievements, btnLeaderboard, btnExit
    });
        }

        private void BtnNewGame_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtPlayerName.Text))
            {
                MessageBox.Show("Введите имя игрока!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            Entity player = null;
            string name = txtPlayerName.Text.Trim();
            string type = cmbCharacterType.SelectedItem.ToString();

            switch (type)
            {
                case "Монстр":
                    player = new Monster(name);
                    break;
                case "Демон":
                    player = new Demon(name);
                    break;
                case "Маг":
                    player = new Mage(name);
                    break;
            }

            var gameForm = new GameForm(player, _db, true);
            gameForm.ShowDialog();
        }

        private void BtnLoadGame_Click(object sender, EventArgs e)
        {
            var savedPlayers = _db.GetSavedPlayers();
            if (savedPlayers.Count == 0)
            {
                MessageBox.Show("Нет сохранённых игр!", "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var loadForm = new LoadSaveForm(_db);
            if (loadForm.ShowDialog() == DialogResult.OK && loadForm.SelectedPlayer != null)
            {
                var saveData = _db.LoadGame(loadForm.SelectedPlayer);
                if (saveData != null)
                {
                    var gameForm = new GameForm(null, _db, false, saveData);
                    gameForm.ShowDialog();
                }
            }
        }

        private void BtnLeaderboard_Click(object sender, EventArgs e)
        {
            var leaderboardForm = new LeaderboardForm(_db);
            leaderboardForm.ShowDialog();
        }
        private void BtnAchievements_Click(object sender, EventArgs e)
        {
            var achievementsForm = new AchievementsForm(_db);
            achievementsForm.ShowDialog();
        }
    }
}