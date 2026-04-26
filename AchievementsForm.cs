using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using BattleOfEntities.Models;
using BattleOfEntities.Database;

namespace BattleOfEntities
{
    public partial class AchievementsForm : Form
    {
        private DatabaseManager _db;
        private FlowLayoutPanel flpAchievements;

        public AchievementsForm(DatabaseManager db)
        {
            _db = db;
            InitializeComponent();
            LoadAchievements();
        }

        private void InitializeComponent()
        {
            this.Text = "Глобальные достижения";
            this.Size = new Size(750, 800);  // Ещё увеличено: 750x800
            this.MinimumSize = new Size(700, 750);
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = Color.FromArgb(20, 20, 30);
            this.FormBorderStyle = FormBorderStyle.FixedSingle;

            Label lblTitle = new Label
            {
                Text = "🏆 ГЛОБАЛЬНЫЕ ДОСТИЖЕНИЯ 🏆",
                Font = new Font("Arial", 20, FontStyle.Bold),
                ForeColor = Color.Gold,
                TextAlign = ContentAlignment.MiddleCenter,
                Size = new Size(700, 60),
                Location = new Point(25, 20)
            };

            flpAchievements = new FlowLayoutPanel
            {
                Location = new Point(25, 100),
                Size = new Size(680, 580),  // Увеличена высота
                AutoScroll = true,
                BackColor = Color.FromArgb(30, 30, 40)
            };

            Button btnClose = new Button
            {
                Text = "✖ ЗАКРЫТЬ",
                Location = new Point(275, 700),
                Size = new Size(200, 50),
                Font = new Font("Arial", 14, FontStyle.Bold),
                BackColor = Color.FromArgb(80, 80, 100),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnClose.Click += (s, e) => this.Close();

            this.Controls.AddRange(new Control[] { lblTitle, flpAchievements, btnClose });
        }

        private void LoadAchievements()
        {
            var unlocked = _db.GetGlobalAchievements();
            var allAchievements = Achievement.GetAllAchievements();

            foreach (var achievement in allAchievements)
            {
                bool isUnlocked = unlocked.Contains(achievement.Name);

                Panel panel = new Panel
                {
                    Size = new Size(660, 70),
                    Margin = new Padding(5),
                    BackColor = isUnlocked ? Color.FromArgb(60, 80, 60) : Color.FromArgb(50, 50, 60),
                    BorderStyle = BorderStyle.FixedSingle
                };

                Label lblName = new Label
                {
                    Text = (isUnlocked ? "✅ " : "🔒 ") + achievement.Name,
                    Font = new Font("Arial", 13, FontStyle.Bold),
                    ForeColor = isUnlocked ? Color.Gold : Color.Gray,
                    Location = new Point(15, 10),
                    Size = new Size(300, 30)
                };

                Label lblDesc = new Label
                {
                    Text = achievement.Description,
                    Font = new Font("Arial", 11),
                    ForeColor = isUnlocked ? Color.White : Color.DarkGray,
                    Location = new Point(15, 42),
                    Size = new Size(630, 25)
                };

                panel.Controls.AddRange(new Control[] { lblName, lblDesc });
                flpAchievements.Controls.Add(panel);
            }
        }
    }
}