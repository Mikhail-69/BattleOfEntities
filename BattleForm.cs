using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using BattleOfEntities.Models;

namespace BattleOfEntities
{
    public partial class BattleForm : Form
    {
        private Entity _player;
        private Enemy _enemy;
        private Label lblPlayerName;
        private Label lblEnemyName;
        private ProgressBar pbPlayerHealth;
        private ProgressBar pbEnemyHealth;
        private Label lblPlayerHealth;
        private Label lblEnemyHealth;
        private RichTextBox txtBattleLog;
        private Button btnAttack;
        private Action<bool> _onBattleEnd;

        public BattleForm(Entity player, Enemy enemy, Action<bool> onBattleEnd)
        {
            _player = player;
            _enemy = enemy;
            _onBattleEnd = onBattleEnd;
            InitializeComponent();
            UpdateUI();
        }

        private void InitializeComponent()
        {
            this.Text = "Битва!";
            this.Size = new Size(800, 600);
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = Color.FromArgb(20, 20, 30);

            // Игрок
            lblPlayerName = new Label
            {
                Location = new Point(50, 50),
                Size = new Size(300, 30),
                Font = new Font("Arial", 14, FontStyle.Bold),
                ForeColor = Color.LightGreen,
                Text = $"{_player.Name} ({_player.GetCharacterType()})"
            };

            pbPlayerHealth = new ProgressBar
            {
                Location = new Point(50, 90),
                Size = new Size(300, 20),
                Minimum = 0,
                Maximum = 100
            };

            lblPlayerHealth = new Label
            {
                Location = new Point(50, 115),
                Size = new Size(300, 20),
                ForeColor = Color.White
            };

            // Враг
            lblEnemyName = new Label
            {
                Location = new Point(450, 50),
                Size = new Size(300, 30),
                Font = new Font("Arial", 14, FontStyle.Bold),
                ForeColor = Color.LightCoral,
                Text = _enemy.Name
            };

            pbEnemyHealth = new ProgressBar
            {
                Location = new Point(450, 90),
                Size = new Size(300, 20),
                Minimum = 0,
                Maximum = 100
            };

            lblEnemyHealth = new Label
            {
                Location = new Point(450, 115),
                Size = new Size(300, 20),
                ForeColor = Color.White
            };

            // Лог боя
            txtBattleLog = new RichTextBox
            {
                Location = new Point(50, 200),
                Size = new Size(700, 300),
                Font = new Font("Consolas", 10),
                BackColor = Color.FromArgb(30, 30, 40),
                ForeColor = Color.LightGreen,
                ReadOnly = true
            };

            // Кнопка атаки
            btnAttack = new Button
            {
                Text = "АТАКОВАТЬ!",
                Location = new Point(300, 520),
                Size = new Size(200, 40),
                Font = new Font("Arial", 14, FontStyle.Bold),
                BackColor = Color.FromArgb(180, 60, 60),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnAttack.Click += BtnAttack_Click;

            this.Controls.AddRange(new Control[] { lblPlayerName, pbPlayerHealth, lblPlayerHealth,
                                                   lblEnemyName, pbEnemyHealth, lblEnemyHealth,
                                                   txtBattleLog, btnAttack });
        }

        private void UpdateUI()
        {
            int playerPercent = (int)((double)_player.Health / _player.MaxHealth * 100);
            pbPlayerHealth.Value = Math.Max(0, Math.Min(100, playerPercent));
            lblPlayerHealth.Text = $"❤️ {_player.Health}/{_player.MaxHealth}";

            int enemyPercent = (int)((double)_enemy.Health / _enemy.MaxHealth * 100);
            pbEnemyHealth.Value = Math.Max(0, Math.Min(100, enemyPercent));
            lblEnemyHealth.Text = $"❤️ {_enemy.Health}/{_enemy.MaxHealth}";
        }

        private async void BtnAttack_Click(object sender, EventArgs e)
        {
            btnAttack.Enabled = false;
            Random rnd = new Random();

            int playerDamage = (int)(_player.Power * (0.8 + rnd.NextDouble() * 0.4));
            _enemy.TakeDamage(playerDamage);
            _player.TotalDamageDealt += playerDamage;
            AddBattleLog($"{_player.Name} наносит {playerDamage} урона {_enemy.Name}!", Color.YellowGreen);
            UpdateUI();

            await Task.Delay(800);

            if (_enemy.IsDead)
            {
                AddBattleLog($"ПОБЕДА! {_enemy.Name} повержен!", Color.Gold);
                _onBattleEnd?.Invoke(true);
                this.Close();
                return;
            }

            int enemyDamage = (int)(_enemy.Power * (0.8 + rnd.NextDouble() * 0.4));
            _player.TakeDamage(enemyDamage);
            AddBattleLog($"{_enemy.Name} наносит {enemyDamage} урона {_player.Name}!", Color.OrangeRed);
            UpdateUI();

            await Task.Delay(800);

            if (_player.IsDead)
            {
                AddBattleLog($"ПОРАЖЕНИЕ! {_player.Name} погиб...", Color.Red);
                _onBattleEnd?.Invoke(false);
                this.Close();
                return;
            }

            btnAttack.Enabled = true;
        }

        private void AddBattleLog(string message, Color color)
        {
            txtBattleLog.SelectionColor = color;
            txtBattleLog.AppendText($"{DateTime.Now:HH:mm:ss} - {message}\n");
            txtBattleLog.ScrollToCaret();
        }
    }
}