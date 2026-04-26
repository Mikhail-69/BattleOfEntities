using System;
using System.Drawing;
using System.Windows.Forms;
using BattleOfEntities.Database;

namespace BattleOfEntities
{
    public partial class LoadSaveForm : Form
    {
        private DatabaseManager _db;
        private ListBox lstSaves;
        private Button btnLoad;
        private Button btnCancel;
        private Label lblTitle;

        public string SelectedPlayer { get; private set; }

        public LoadSaveForm(DatabaseManager db)
        {
            _db = db;
            InitializeComponent();
            LoadSaves();
        }

        private void InitializeComponent()
        {
            this.Text = "Загрузка игры";
            this.Size = new Size(1000, 550);
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = Color.FromArgb(20, 20, 30);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            lblTitle = new Label
            {
                Text = "Выберите сохранение:",
                Font = new Font("Arial", 12),
                ForeColor = Color.White,
                Location = new Point(20, 20),
                Size = new Size(350, 30)
            };

            lstSaves = new ListBox
            {
                Location = new Point(20, 60),
                Size = new Size(350, 250),
                Font = new Font("Arial", 11),
                BackColor = Color.FromArgb(40, 40, 50),
                ForeColor = Color.White
            };

            btnLoad = new Button
            {
                Text = "Загрузить",
                Location = new Point(80, 330),
                Size = new Size(100, 35),
                Font = new Font("Arial", 10),
                BackColor = Color.FromArgb(60, 120, 60),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnLoad.Click += BtnLoad_Click;

            btnCancel = new Button
            {
                Text = "Отмена",
                Location = new Point(220, 330),
                Size = new Size(100, 35),
                Font = new Font("Arial", 10),
                BackColor = Color.FromArgb(120, 60, 60),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnCancel.Click += (s, e) => this.Close();

            this.Controls.AddRange(new Control[] { lblTitle, lstSaves, btnLoad, btnCancel });
        }

        private void LoadSaves()
        {
            var players = _db.GetSavedPlayers();
            lstSaves.Items.Clear();
            foreach (var player in players)
            {
                lstSaves.Items.Add(player);
            }
        }

        private void BtnLoad_Click(object sender, EventArgs e)
        {
            if (lstSaves.SelectedItem != null)
            {
                SelectedPlayer = lstSaves.SelectedItem.ToString();
                DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                MessageBox.Show("Выберите сохранение!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}