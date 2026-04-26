using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using BattleOfEntities.Models;
using Newtonsoft.Json;

namespace BattleOfEntities.Database
{
    public class DatabaseManager
    {
        private string _connectionString;
        private string _dbPath;

        public DatabaseManager()
        {
            _dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "BattleOfEntities.db");
            _connectionString = $"Data Source={_dbPath};Version=3;";
            InitializeDatabase();
        }

        public string GetConnectionString()
        {
            return _connectionString;
        }

        private void InitializeDatabase()
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();

                string createRecordsTable = @"
                    CREATE TABLE IF NOT EXISTS Records (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        PlayerName TEXT NOT NULL,
                        CharacterType TEXT NOT NULL,
                        FinalHealth INTEGER,
                        FinalPower INTEGER,
                        TotalDamageDealt INTEGER,
                        EnemiesKilled INTEGER,
                        PotionsUsed INTEGER,
                        PlayTime TEXT,
                        CompletionDate TEXT,
                        IsVictory INTEGER
                    )";

                string createSavesTable = @"
                    CREATE TABLE IF NOT EXISTS Saves (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        PlayerName TEXT NOT NULL,
                        SaveData TEXT NOT NULL,
                        SaveDate TEXT,
                        IsActive INTEGER DEFAULT 1
                    )";

                string createGlobalAchievementsTable = @"
                    CREATE TABLE IF NOT EXISTS GlobalAchievements (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        AchievementName TEXT NOT NULL UNIQUE,
                        UnlockedDate TEXT
                    )";

                using (var cmd = new SQLiteCommand(createRecordsTable, connection))
                    cmd.ExecuteNonQuery();

                using (var cmd = new SQLiteCommand(createSavesTable, connection))
                    cmd.ExecuteNonQuery();

                using (var cmd = new SQLiteCommand(createGlobalAchievementsTable, connection))
                    cmd.ExecuteNonQuery();
            }
        }

        public void SaveRecord(GameRecord record)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();
                string query = @"
                    INSERT INTO Records (PlayerName, CharacterType, FinalHealth, FinalPower, 
                                        TotalDamageDealt, EnemiesKilled, PotionsUsed, PlayTime, CompletionDate, IsVictory)
                    VALUES (@name, @type, @health, @power, @damage, @killed, @potions, @time, @date, @victory)";

                using (var cmd = new SQLiteCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@name", record.PlayerName);
                    cmd.Parameters.AddWithValue("@type", record.CharacterType);
                    cmd.Parameters.AddWithValue("@health", record.FinalHealth);
                    cmd.Parameters.AddWithValue("@power", record.FinalPower);
                    cmd.Parameters.AddWithValue("@damage", record.TotalDamageDealt);
                    cmd.Parameters.AddWithValue("@killed", record.EnemiesKilled);
                    cmd.Parameters.AddWithValue("@potions", record.PotionsUsed);
                    cmd.Parameters.AddWithValue("@time", record.PlayTime);
                    cmd.Parameters.AddWithValue("@date", record.CompletionDate);
                    cmd.Parameters.AddWithValue("@victory", record.IsVictory ? 1 : 0);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public List<GameRecord> GetLeaderboard(int limit = 10)
        {
            var records = new List<GameRecord>();

            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();
                string query = @"
                    SELECT * FROM Records 
                    WHERE IsVictory = 1 
                    ORDER BY EnemiesKilled DESC, TotalDamageDealt DESC 
                    LIMIT @limit";

                using (var cmd = new SQLiteCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@limit", limit);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            records.Add(new GameRecord
                            {
                                Id = Convert.ToInt32(reader["Id"]),
                                PlayerName = reader["PlayerName"].ToString(),
                                CharacterType = reader["CharacterType"].ToString(),
                                FinalHealth = Convert.ToInt32(reader["FinalHealth"]),
                                FinalPower = Convert.ToInt32(reader["FinalPower"]),
                                TotalDamageDealt = Convert.ToInt32(reader["TotalDamageDealt"]),
                                EnemiesKilled = Convert.ToInt32(reader["EnemiesKilled"]),
                                PotionsUsed = Convert.ToInt32(reader["PotionsUsed"]),
                                PlayTime = reader["PlayTime"].ToString(),
                                CompletionDate = reader["CompletionDate"].ToString(),
                                IsVictory = Convert.ToInt32(reader["IsVictory"]) == 1
                            });
                        }
                    }
                }
            }

            return records;
        }

        // ГЛОБАЛЬНЫЕ ДОСТИЖЕНИЯ (для всех игроков)
        public void SaveGlobalAchievement(string achievementName)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();

                string checkQuery = "SELECT COUNT(*) FROM GlobalAchievements WHERE AchievementName = @name";
                using (var checkCmd = new SQLiteCommand(checkQuery, connection))
                {
                    checkCmd.Parameters.AddWithValue("@name", achievementName);
                    int count = Convert.ToInt32(checkCmd.ExecuteScalar());
                    if (count > 0) return;
                }

                string query = "INSERT INTO GlobalAchievements (AchievementName, UnlockedDate) VALUES (@name, @date)";
                using (var cmd = new SQLiteCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@name", achievementName);
                    cmd.Parameters.AddWithValue("@date", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public List<string> GetGlobalAchievements()
        {
            var achievements = new List<string>();

            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();
                string query = "SELECT AchievementName FROM GlobalAchievements";

                using (var cmd = new SQLiteCommand(query, connection))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        achievements.Add(reader["AchievementName"].ToString());
                    }
                }
            }

            return achievements;
        }

        // СОХРАНЕНИЯ
        public void SaveGame(SaveData save)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();

                string deleteQuery = "DELETE FROM Saves WHERE PlayerName = @name AND IsActive = 1";
                using (var deleteCmd = new SQLiteCommand(deleteQuery, connection))
                {
                    deleteCmd.Parameters.AddWithValue("@name", save.PlayerName);
                    deleteCmd.ExecuteNonQuery();
                }

                if (save.IsActive)
                {
                    string query = "INSERT INTO Saves (PlayerName, SaveData, SaveDate, IsActive) VALUES (@name, @data, @date, @active)";
                    using (var cmd = new SQLiteCommand(query, connection))
                    {
                        string json = JsonConvert.SerializeObject(save);
                        cmd.Parameters.AddWithValue("@name", save.PlayerName);
                        cmd.Parameters.AddWithValue("@data", json);
                        cmd.Parameters.AddWithValue("@date", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                        cmd.Parameters.AddWithValue("@active", save.IsActive ? 1 : 0);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }

        public SaveData LoadGame(string playerName)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();
                string query = "SELECT SaveData FROM Saves WHERE PlayerName = @name AND IsActive = 1 ORDER BY Id DESC LIMIT 1";

                using (var cmd = new SQLiteCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@name", playerName);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            string json = reader["SaveData"].ToString();
                            return JsonConvert.DeserializeObject<SaveData>(json);
                        }
                    }
                }
            }

            return null;
        }

        public List<string> GetSavedPlayers()
        {
            var players = new List<string>();

            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();
                string query = "SELECT DISTINCT PlayerName FROM Saves WHERE IsActive = 1";

                using (var cmd = new SQLiteCommand(query, connection))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        players.Add(reader["PlayerName"].ToString());
                    }
                }
            }

            return players;
        }
    }
}