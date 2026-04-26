using BattleOfEntities.Database;
using BattleOfEntities.Models;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace BattleOfEntities
{
    public partial class GameForm : Form
    {
        private DatabaseManager _db;
        private Entity _player;
        private List<Enemy> _currentEnemies;
        private int _currentLocation;
        private int _currentEnemyIndex;
        private DateTime _gameStartTime;
        private List<string> _unlockedAchievements;
        private List<Achievement> _allAchievements;
        private string _currentLocationName;
        private int _potionsUsedTotal;
        private int _artifactsCollected;
        private Label lblLocation;
        private Label lblPlayerInfo;
        private Label lblEnemyInfo;
        private ListBox lstEnemies;
        private Button btnFight;
        private Button btnUsePotion;
        private Button btnUseArtifacts;
        private Button btnSaveAndExit;
        private Button btnAchievements;
        private RichTextBox txtBattleLog;
        private Label lblEnemyList;
        private Label lblCurrentEnemy;
        private Label lblBattleLog;
        private ProgressBar pbPlayerHealth;
        private ProgressBar pbEnemyHealth;

        public GameForm(Entity player, DatabaseManager db, bool isNewGame, SaveData saveData = null)
        {
            _db = db;
            _unlockedAchievements = new List<string>();
            _allAchievements = Achievement.GetAllAchievements();

            if (isNewGame)
            {
                _player = player;
                _currentLocation = 1;
                _currentEnemyIndex = 0;
                _gameStartTime = DateTime.Now;
                _potionsUsedTotal = 0;
                _artifactsCollected = 0;
                LoadLocation();
            }
            else if (saveData != null)
            {
                LoadFromSave(saveData);
                _gameStartTime = DateTime.Now;
            }
            InitializeComponent();
            UpdateUI();
        }

        private void LoadFromSave(SaveData saveData)
        {
            if (saveData.Health <= 0)
            {
                MessageBox.Show("Это сохранение повреждено или игрок мёртв. Начните новую игру.",
                              "Ошибка загрузки", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
                return;
            }
            switch (saveData.CharacterType)
            {
                case "Монстр":
                    _player = new Monster(saveData.PlayerName);
                    break;
                case "Демон":
                    var demon = new Demon(saveData.PlayerName);
                    if (saveData.Brain.HasValue)
                    {
                        for (int i = 1; i < saveData.Brain.Value; i++)
                            demon.IncreaseBrain();
                    }
                    _player = demon;
                    break;
                case "Маг":
                    var mage = new Mage(saveData.PlayerName);
                    if (saveData.Mana.HasValue)
                    {
                        for (int i = 0; i < saveData.Mana.Value; i++)
                            mage.RestoreMana(1);
                    }
                    _player = mage;
                    break;
            }

            _player.Health = saveData.Health;
            _player.MaxHealth = saveData.MaxHealth;
            _player.BasePower = saveData.BasePower;
            _player.Artifacts = saveData.Artifacts ?? new List<string>();
            _player.PotionsUsed = saveData.PotionsUsed;
            _player.TotalDamageDealt = saveData.TotalDamageDealt;
            _player.EnemiesKilled = saveData.EnemiesKilled;
            _unlockedAchievements = saveData.UnlockedAchievements ?? new List<string>();
            _potionsUsedTotal = saveData.PotionsUsed;
            _artifactsCollected = saveData.Artifacts?.Count ?? 0;
            _currentLocation = saveData.CurrentLocation;
            _currentEnemyIndex = saveData.CurrentEnemyIndex;

            LoadLocation();

            for (int i = 0; i < _currentEnemyIndex && i < _currentEnemies.Count; i++)
            {
                _currentEnemies[i] = null;
            }
        }

        private void LoadLocation()
        {
            _currentEnemies = Enemy.GetEnemiesForLocation(_currentLocation);
            string[] locationNames = { "", "Тёмный лес", "Горящие горы", "Цитадель тьмы" };
            _currentLocationName = locationNames[_currentLocation];
        }

        private void InitializeComponent()
        {
            this.Text = "Battle of Entities - Игра";
            this.Size = new Size(1000, 800);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(20, 20, 30);
            this.FormClosing += GameForm_FormClosing;

            lblLocation = new Label
            {
                Location = new Point(20, 20),
                Size = new Size(400, 30),
                Font = new Font("Arial", 16, FontStyle.Bold),
                ForeColor = Color.Gold,
                BackColor = Color.Transparent
            };

            lblPlayerInfo = new Label
            {
                Location = new Point(20, 70),
                Size = new Size(400, 160),
                Font = new Font("Arial", 10),
                ForeColor = Color.White,
                BackColor = Color.Transparent
            };

            pbPlayerHealth = new ProgressBar
            {
                Location = new Point(20, 230),
                Size = new Size(400, 25),
                Minimum = 0,
                Maximum = 100,
                Style = ProgressBarStyle.Continuous
            };

            lblEnemyList = new Label
            {
                Text = "Противники в локации:",
                Location = new Point(20, 270),
                Size = new Size(400, 25),
                Font = new Font("Arial", 10, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.Transparent
            };

            lstEnemies = new ListBox
            {
                Location = new Point(20, 300),
                Size = new Size(400, 200),
                Font = new Font("Arial", 10),
                BackColor = Color.FromArgb(40, 40, 50),
                ForeColor = Color.White
            };

            lblCurrentEnemy = new Label
            {
                Text = "Текущий противник:",
                Location = new Point(460,90),
                Size = new Size(400, 25),
                Font = new Font("Arial", 10, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.Transparent
            };

            lblEnemyInfo = new Label
            {
                Location = new Point(460, 120),
                Size = new Size(400, 100),
                Font = new Font("Arial", 10),
                ForeColor = Color.White,
                BackColor = Color.Transparent
            };

            pbEnemyHealth = new ProgressBar
            {
                Location = new Point(460, 230),
                Size = new Size(400, 25),
                Minimum = 0,
                Maximum = 100,
                Style = ProgressBarStyle.Continuous,
                BackColor = Color.DarkRed,
                ForeColor = Color.Lime
            };

            lblBattleLog = new Label
            {
                Text = "Журнал боя:",
                Location = new Point(460, 270),
                Size = new Size(400, 25),
                Font = new Font("Arial", 10, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.Transparent
            };

            txtBattleLog = new RichTextBox
            {
                Location = new Point(460, 300),
                Size = new Size(450, 300),
                Font = new Font("Consolas", 9),
                BackColor = Color.FromArgb(30, 30, 40),
                ForeColor = Color.LightGreen,
                ReadOnly = true
            };

            btnFight = new Button
            {
                Text = "⚔️ СРАЖАТЬСЯ ⚔️",
                Location = new Point(20, 500),
                Size = new Size(180, 50),
                Font = new Font("Arial", 12, FontStyle.Bold),
                BackColor = Color.FromArgb(180, 60, 60),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnFight.Click += BtnFight_Click;

            btnUsePotion = new Button
            {
                Text = "💊 Зелье здоровья",
                Location = new Point(210, 500),
                Size = new Size(100, 50),
                Font = new Font("Arial", 10),
                BackColor = Color.FromArgb(60, 100, 60),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnUsePotion.Click += BtnUsePotion_Click;

            btnUseArtifacts = new Button
            {
                Text = "✨ Артефакты",
                Location = new Point(320, 500),
                Size = new Size(100, 50),
                Font = new Font("Arial", 10),
                BackColor = Color.FromArgb(100, 60, 120),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnUseArtifacts.Click += BtnUseArtifacts_Click;

            btnAchievements = new Button
            {
                Text = "🏆 Достижения",
                Location = new Point(20, 560),
                Size = new Size(180, 40),
                Font = new Font("Arial", 10),
                BackColor = Color.FromArgb(80, 80, 120),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnAchievements.Click += BtnAchievements_Click;

            btnSaveAndExit = new Button
            {
                Text = "💾 Сохранить и выйти",
                Location = new Point(210, 560),
                Size = new Size(210, 40),
                Font = new Font("Arial", 10),
                BackColor = Color.FromArgb(70, 70, 100),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnSaveAndExit.Click += BtnSaveAndExit_Click;

            this.Controls.AddRange(new Control[] {
                lblLocation, lblPlayerInfo, pbPlayerHealth,
                lblEnemyList, lstEnemies, lblCurrentEnemy,
                lblEnemyInfo, pbEnemyHealth, lblBattleLog,
                txtBattleLog, btnFight, btnUsePotion,
                btnUseArtifacts, btnAchievements, btnSaveAndExit
            });

            pbEnemyHealth.BringToFront();
        }

        private void UpdateUI()
        {
            UpdateLocationColors();

            lblLocation.Text = $"📍 {_currentLocationName} (Уровень {_currentLocation}/3)";

            string specialInfo = _player.GetSpecialInfo();
            lblPlayerInfo.Text = $"👤 {_player.Name} ({_player.GetCharacterType()})\n" +
                                $"❤️ Здоровье: {_player.Health}/{_player.MaxHealth}\n" +
                                $"⚔️ Сила: {_player.Power}\n" +
                                $"📦 Артефактов: {_player.Artifacts.Count}\n" +
                                $"💊 Зелий использовано: {_player.PotionsUsed}\n" +
                                $"🗡️ Убито врагов: {_player.EnemiesKilled}\n" +
                                $"{specialInfo}";

            int playerHealthPercent = (int)((double)_player.Health / _player.MaxHealth * 100);
            pbPlayerHealth.Value = Math.Max(0, Math.Min(100, playerHealthPercent));

            lstEnemies.Items.Clear();
            for (int i = 0; i < _currentEnemies.Count; i++)
            {
                if (_currentEnemies[i] != null && !_currentEnemies[i].IsDead)
                {
                    lstEnemies.Items.Add($"⚔️ {_currentEnemies[i].Name} (❤️ {_currentEnemies[i].Health})");
                }
            }

            Enemy currentEnemy = null;
            for (int i = 0; i < _currentEnemies.Count; i++)
            {
                if (_currentEnemies[i] != null && !_currentEnemies[i].IsDead)
                {
                    currentEnemy = _currentEnemies[i];
                    break;
                }
            }

            if (currentEnemy != null)
            {
                lblEnemyInfo.Text = $"👾 {currentEnemy.Name} (Уровень {currentEnemy.Level})\n" +
                                   $"❤️ Здоровье: {currentEnemy.Health}/{currentEnemy.MaxHealth}\n" +
                                   $"⚔️ Сила: {currentEnemy.Power}";

                int enemyHealthPercent = (int)((double)currentEnemy.Health / currentEnemy.MaxHealth * 100);
                pbEnemyHealth.Value = Math.Max(0, Math.Min(100, enemyHealthPercent));
            }
        }

        private void NextLocation()
        {
            bool allEnemiesDefeated = true;
            for (int i = 0; i < _currentEnemies.Count; i++)
            {
                if (_currentEnemies[i] != null && !_currentEnemies[i].IsDead)
                {
                    allEnemiesDefeated = false;
                    break;
                }
            }

            if (allEnemiesDefeated)
            {
                string locationName = GetLocationName(_currentLocation);
                if (!string.IsNullOrEmpty(locationName))
                {
                    if (locationName == "леса")
                        CheckAchievement("location_1");
                    else if (locationName == "гор")
                        CheckAchievement("location_2");
                    else if (locationName == "тьмы")
                        CheckAchievement("location_3");
                }

                if (_currentLocation < 3)
                {
                    _currentLocation++;
                    LoadLocation();
                    _currentEnemyIndex = 0;
                    AddBattleLog($"🏆 Поздравляем! Вы переходите в локацию: {_currentLocationName}!", Color.Gold);
                    UpdateUI();
                }
                else
                {
                    EndGame(true);
                }
            }
            else
            {
                for (int i = 0; i < _currentEnemies.Count; i++)
                {
                    if (_currentEnemies[i] != null && !_currentEnemies[i].IsDead)
                    {
                        _currentEnemyIndex = i;
                        AddBattleLog($"⚔️ Следующий противник: {_currentEnemies[i].Name}! Осталось врагов: {GetAliveEnemiesCount()}", Color.Yellow);
                        break;
                    }
                }
                UpdateUI();
            }
        }

        private int GetAliveEnemiesCount()
        {
            int count = 0;
            for (int i = 0; i < _currentEnemies.Count; i++)
            {
                if (_currentEnemies[i] != null && !_currentEnemies[i].IsDead)
                    count++;
            }
            return count;
        }

        private string GetLocationName(int location)
        {
            string[] names = { "", "леса", "гор", "тьмы" };
            return names[location];
        }

        private async void BtnFight_Click(object sender, EventArgs e)
        {
            Enemy currentEnemy = null;
            int aliveIndex = -1;
            for (int i = 0; i < _currentEnemies.Count; i++)
            {
                if (_currentEnemies[i] != null && !_currentEnemies[i].IsDead)
                {
                    currentEnemy = _currentEnemies[i];
                    aliveIndex = i;
                    break;
                }
            }

            if (currentEnemy == null)
            {
                AddBattleLog("Нет доступных противников для боя!", Color.Yellow);
                NextLocation();
                return;
            }

            btnFight.Enabled = false;
            btnUsePotion.Enabled = false;
            btnUseArtifacts.Enabled = false;

            AddBattleLog($"⚔️ НАЧАЛО БОЯ: {_player.Name} VS {currentEnemy.Name} ⚔️", Color.Cyan);

            _player.ApplyArtifactBonus();

            bool playerWon = await AutoBattle(currentEnemy);

            if (playerWon)
            {
                string defeatedEnemyName = currentEnemy.Name;
                AddBattleLog($"🎉 ПОБЕДА! {defeatedEnemyName} повержен!", Color.Green);
                _player.EnemiesKilled++;

                Random rnd = new Random();
                if (rnd.Next(0, 3) == 0)
                {
                    string[] artifacts = Artifact.GetAllArtifacts().Select(a => a.Name).ToArray();
                    string artifactName = artifacts[rnd.Next(artifacts.Length)];
                    _player.AddArtifact(artifactName);
                    _artifactsCollected++;
                    AddBattleLog($"✨ Вы получили артефакт: {artifactName}!", Color.Magenta);
                    CheckAchievement("artifact_3");
                    CheckAchievement("artifact_7");
                }

                if (_player is Demon demon)
                    demon.IncreaseBrain();
                else if (_player is Mage mage)
                    mage.RestoreMana(30);
                else if (_player is Monster monster)
                    monster.IncreaseMaxHealth(100);

                _currentEnemies[aliveIndex] = null;

                if (_player.EnemiesKilled == 1)
                    CheckAchievement("first_kill");
                if (defeatedEnemyName == "Повелитель тьмы")
                    CheckAchievement("boss_killer");

                UpdateUI();

                bool hasLivingEnemies = false;
                for (int i = 0; i < _currentEnemies.Count; i++)
                {
                    if (_currentEnemies[i] != null && !_currentEnemies[i].IsDead)
                    {
                        hasLivingEnemies = true;
                        break;
                    }
                }

                if (hasLivingEnemies)
                {
                    int aliveCount = GetAliveEnemiesCount();
                    AddBattleLog($"⚔️ Осталось врагов в этой локации: {aliveCount}", Color.Yellow);
                }
                else
                {
                    AddBattleLog($"🏆 Все враги в локации побеждены!", Color.Gold);
                    NextLocation();
                }
            }
            else
            {
                AddBattleLog($"💀 ПОРАЖЕНИЕ! {_player.Name} погиб в бою...", Color.Red);
                EndGame(false);
                return;
            }

            btnFight.Enabled = true;
            btnUsePotion.Enabled = true;
            btnUseArtifacts.Enabled = true;
        }

        private async System.Threading.Tasks.Task<bool> AutoBattle(Enemy enemy)
        {
            Random rnd = new Random();

            while (!_player.IsDead && !enemy.IsDead)
            {
                int playerDamage;
                bool usedSpell = false;

                // Логика для мага
                if (_player is Mage mage)
                {
                    // Пробуем использовать заклинание
                    int spellDamage = mage.CastSpell();

                    if (spellDamage > 0)
                    {
                        // Успешное заклинание
                        playerDamage = spellDamage;
                        usedSpell = true;
                        AddBattleLog($"✨ {_player.Name} использует МОЩНОЕ ЗАКЛИНАНИЕ и наносит {playerDamage} урона! (Мана: {mage.Mana})", Color.Magenta);
                    }
                    else if (spellDamage == -1)
                    {
                        // Заклинание на перезарядке
                        playerDamage = (int)(_player.Power * (0.8 + rnd.NextDouble() * 0.4));
                        AddBattleLog($"👉 {_player.Name} наносит {playerDamage} урона! (Заклинание перезаряжается: {mage.SpellCooldown} хода)", Color.YellowGreen);
                    }
                    else
                    {
                        // Недостаточно маны
                        playerDamage = (int)(_player.Power * (0.8 + rnd.NextDouble() * 0.4));
                        AddBattleLog($"👉 {_player.Name} наносит {playerDamage} урона! (Недостаточно маны для заклинания)", Color.YellowGreen);
                    }

                    // Уменьшаем перезарядку
                    mage.ReduceCooldown();
                }
                else
                {
                    // Обычный урон для других классов
                    playerDamage = (int)(_player.Power * (0.8 + rnd.NextDouble() * 0.4));
                    AddBattleLog($"👉 {_player.Name} наносит {playerDamage} урона!", Color.YellowGreen);
                }

                enemy.TakeDamage(playerDamage);
                _player.TotalDamageDealt += playerDamage;
                UpdateUI();

                await System.Threading.Tasks.Task.Delay(800);

                if (enemy.IsDead) break;

                int enemyDamage = (int)(enemy.Power * (0.8 + rnd.NextDouble() * 0.4));
                _player.TakeDamage(enemyDamage);
                AddBattleLog($"👈 {enemy.Name} наносит {enemyDamage} урона!", Color.OrangeRed);
                UpdateUI();

                await System.Threading.Tasks.Task.Delay(800);
            }

            return !_player.IsDead;
        }

        private void BtnUsePotion_Click(object sender, EventArgs e)
        {
            if (_player.PotionsUsed >= 10)
            {
                AddBattleLog("Вы использовали максимум зелий за игру (10)!", Color.Yellow);
                return;
            }

            Random rnd = new Random();
            int healAmount = rnd.Next(30, 101);
            _player.Heal(healAmount);
            _player.PotionsUsed++;
            _potionsUsedTotal++;
            AddBattleLog($"💊 Вы выпили зелье и восстановили {healAmount} здоровья! (Всего выпито: {_potionsUsedTotal})", Color.LightGreen);

            CheckAchievement("potion_5");
            CheckAchievement("potion_10");
            UpdateUI();
        }

        private void BtnUseArtifacts_Click(object sender, EventArgs e)
        {
            if (_player.Artifacts.Count == 0)
            {
                AddBattleLog("У вас нет артефактов!", Color.Yellow);
                return;
            }

            var artifactForm = new Form
            {
                Text = "Выберите артефакт",
                Size = new Size(400, 300),
                StartPosition = FormStartPosition.CenterParent,
                BackColor = Color.FromArgb(20, 20, 30)
            };

            var listBox = new ListBox
            {
                Location = new Point(20, 20),
                Size = new Size(340, 180),
                Font = new Font("Arial", 10),
                BackColor = Color.FromArgb(40, 40, 50),
                ForeColor = Color.White
            };

            foreach (var artifactName in _player.Artifacts)
            {
                var artifact = Artifact.GetAllArtifacts().Find(a => a.Name == artifactName);
                if (artifact != null)
                {
                    listBox.Items.Add($"{artifact.Name} - {artifact.Description}");
                }
                else
                {
                    listBox.Items.Add(artifactName);
                }
            }

            var btnUse = new Button
            {
                Text = "Использовать",
                Location = new Point(80, 220),
                Size = new Size(100, 30),
                BackColor = Color.FromArgb(60, 120, 60),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };

            var btnCancel = new Button
            {
                Text = "Отмена",
                Location = new Point(200, 220),
                Size = new Size(100, 30),
                BackColor = Color.FromArgb(120, 60, 60),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };

            btnUse.Click += (s, args) =>
            {
                if (listBox.SelectedIndex >= 0)
                {
                    string selected = listBox.SelectedItem.ToString();
                    string artifactName = selected.Split('-')[0].Trim();
                    ApplyArtifact(artifactName);
                    _player.Artifacts.Remove(artifactName);
                    artifactForm.Close();
                    UpdateUI();
                }
            };

            btnCancel.Click += (s, args) => artifactForm.Close();

            artifactForm.Controls.AddRange(new Control[] { listBox, btnUse, btnCancel });
            artifactForm.ShowDialog();
        }

        private void ApplyArtifact(string artifactName)
        {
            var artifact = Artifact.GetAllArtifacts().Find(a => a.Name == artifactName);
            if (artifact != null)
            {
                _player.BasePower += artifact.PowerBonus;
                _player.Heal(artifact.HealthBonus);

                if (_player is Mage mage && artifact.ManaBonus > 0)
                {
                    mage.RestoreMana(artifact.ManaBonus);
                }

                if (artifact.CritChance > 0 || artifact.LifeSteal > 0)
                {
                    AddBattleLog($"✨ Бонусы активированы: +{artifact.CritChance}% к криту, {artifact.LifeSteal}% вампиризм!", Color.Magenta);
                }

                AddBattleLog($"✨ Использован артефакт {artifact.Name}: {artifact.Description}!", Color.Magenta);
            }
        }

        private void BtnAchievements_Click(object sender, EventArgs e)
        {
            var achievementsForm = new AchievementsForm(_db);
            achievementsForm.ShowDialog();
        }

        private void BtnSaveAndExit_Click(object sender, EventArgs e)
        {
            SaveGame();
            MessageBox.Show("Игра сохранена!", "Сохранение", MessageBoxButtons.OK, MessageBoxIcon.Information);
            this.Close();
        }

        private void SaveGame()
        {
            var saveData = new SaveData
            {
                PlayerName = _player.Name,
                CharacterType = _player.GetCharacterType(),
                CurrentLocation = _currentLocation,
                CurrentEnemyIndex = _currentEnemyIndex,
                Health = _player.Health,
                MaxHealth = _player.MaxHealth,
                BasePower = _player.BasePower,
                Artifacts = _player.Artifacts,
                PotionsUsed = _player.PotionsUsed,
                TotalDamageDealt = _player.TotalDamageDealt,
                EnemiesKilled = _player.EnemiesKilled,
                UnlockedAchievements = _unlockedAchievements,
                IsActive = true
            };

            if (_player is Demon demon)
                saveData.Brain = demon.Brain;
            else if (_player is Mage mage)
            {
                saveData.Mana = mage.Mana;
                saveData.MaxMana = mage.MaxMana;
            }

            _db.SaveGame(saveData);
        }

        private void EndGame(bool isVictory)
        {
            TimeSpan playTime = DateTime.Now - _gameStartTime;
            string timeStr = $"{playTime.Minutes:D2}:{playTime.Seconds:D2}";

            SoundManager.StopBackgroundMusic();
            DeleteSave();

            if (isVictory)
            {
                SoundManager.PlayOneShot("victory.wav");

                // Проверяем достижения (они сами проверят, получены ли уже)
                CheckAchievement("complete_game");
                CheckAchievement("kill_10");
                CheckAchievement("damage_1000");

                if (_player.PotionsUsed == 0)
                    CheckAchievement("challenge");

                // Достижения за класс
                if (_player is Demon)
                    CheckAchievement("demon_player");
                else if (_player is Mage)
                    CheckAchievement("mage_player");

                var record = new GameRecord
                {
                    PlayerName = _player.Name,
                    CharacterType = _player.GetCharacterType(),
                    FinalHealth = _player.Health,
                    FinalPower = _player.Power,
                    TotalDamageDealt = _player.TotalDamageDealt,
                    EnemiesKilled = _player.EnemiesKilled,
                    PotionsUsed = _player.PotionsUsed,
                    PlayTime = timeStr,
                    CompletionDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    IsVictory = true
                };

                _db.SaveRecord(record);
                MessageBox.Show($"ПОБЕДА! Вы прошли игру за {timeStr}\n" +
                              $"Убито врагов: {_player.EnemiesKilled}\n" +
                              $"Нанесено урона: {_player.TotalDamageDealt}\n" +
                              $"Использовано зелий: {_player.PotionsUsed}",
                              "Поздравляем!", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                SoundManager.PlayOneShot("defeat.wav");
                MessageBox.Show($"Поражение... Вы погибли в бою.\n" +
                              $"Убито врагов: {_player.EnemiesKilled}\n" +
                              $"Нанесено урона: {_player.TotalDamageDealt}",
                              "Игра окончена", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            this.Close();
        }

        private void DeleteSave()
        {
            try
            {
                using (var connection = new SQLiteConnection(_db.GetConnectionString()))
                {
                    connection.Open();
                    string query = "DELETE FROM Saves WHERE PlayerName = @name";
                    using (var cmd = new SQLiteCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@name", _player.Name);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка удаления сохранения: {ex.Message}");
            }
        }

        private void CheckAchievement(string achievementId)
        {
            var achievement = _allAchievements.FirstOrDefault(a => a.Id == achievementId);
            if (achievement == null) return;

            // Проверяем глобальные достижения
            var globalAchievements = _db.GetGlobalAchievements();
            if (globalAchievements.Contains(achievement.Name)) return;

            bool isUnlocked = false;

            switch (achievement.Type)
            {
                case "potion":
                    if (_potionsUsedTotal >= achievement.RequiredValue)
                        isUnlocked = true;
                    break;
                case "artifact":
                    if (_artifactsCollected >= achievement.RequiredValue)
                        isUnlocked = true;
                    break;
                case "kill":
                case "kill_total":
                    if (_player.EnemiesKilled >= achievement.RequiredValue)
                        isUnlocked = true;
                    break;
                case "damage":
                    if (_player.TotalDamageDealt >= achievement.RequiredValue)
                        isUnlocked = true;
                    break;
                case "location":
                    if (_currentLocation >= achievement.RequiredValue)
                        isUnlocked = true;
                    break;
                case "complete":
                    isUnlocked = true;
                    break;
                case "class":
                    string classType = _player.GetCharacterType();
                    if ((achievementId == "demon_player" && classType == "Демон") ||
                        (achievementId == "mage_player" && classType == "Маг"))
                        isUnlocked = true;
                    break;
                case "challenge":
                    if (_player.PotionsUsed == 0)
                        isUnlocked = true;
                    break;
                case "boss":
                    isUnlocked = true;
                    break;
            }

            if (isUnlocked && !globalAchievements.Contains(achievement.Name))
            {
                _db.SaveGlobalAchievement(achievement.Name);
                ShowAchievementNotification(achievement.Name, achievement.Description);
            }
        }

        private void ShowAchievementNotification(string achievementName, string description)
        {
            AddBattleLog($"🏆 ДОСТИЖЕНИЕ ПОЛУЧЕНО: {achievementName} - {description} 🏆", Color.Gold);
        }

        private void AddBattleLog(string message, Color? color = null)
        {
            if (txtBattleLog.InvokeRequired)
            {
                txtBattleLog.Invoke(new Action(() => AddBattleLog(message, color)));
                return;
            }

            if (color.HasValue)
                txtBattleLog.SelectionColor = color.Value;
            else
                txtBattleLog.SelectionColor = Color.LightGreen;

            txtBattleLog.AppendText($"{DateTime.Now:HH:mm:ss} - {message}\n");
            txtBattleLog.ScrollToCaret();
        }

        private void GameForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_player != null && !_player.IsDead && _player.Health > 0)
            {
                bool hasLivingEnemies = false;
                for (int i = 0; i < _currentEnemies.Count; i++)
                {
                    if (_currentEnemies[i] != null && !_currentEnemies[i].IsDead)
                    {
                        hasLivingEnemies = true;
                        break;
                    }
                }

                if (hasLivingEnemies)
                {
                    var result = MessageBox.Show("Сохранить прогресс перед выходом?", "Сохранение",
                                                MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result == DialogResult.Yes)
                    {
                        SaveGame();
                        MessageBox.Show("Игра сохранена! В следующий раз вы сможете продолжить.",
                                      "Сохранение", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
        }

        private void UpdateLocationColors()
        {
            switch (_currentLocation)
            {
                case 1:
                    this.BackColor = Color.FromArgb(40, 20, 20);
                    lblLocation.ForeColor = Color.OrangeRed;
                    break;
                case 2:
                    this.BackColor = Color.FromArgb(20, 30, 50);
                    lblLocation.ForeColor = Color.LightBlue;
                    break;
                case 3:
                    this.BackColor = Color.FromArgb(20, 30, 20);
                    lblLocation.ForeColor = Color.LightGreen;
                    break;
            }
        }
    }
}