using System.Collections.Generic;

namespace BattleOfEntities.Models
{
    public class SaveData
    {
        public string PlayerName { get; set; }
        public string CharacterType { get; set; }
        public int CurrentLocation { get; set; }
        public int CurrentEnemyIndex { get; set; }
        public int Health { get; set; }
        public int MaxHealth { get; set; }
        public int BasePower { get; set; }
        public List<string> Artifacts { get; set; }
        public int PotionsUsed { get; set; }
        public int TotalDamageDealt { get; set; }
        public int EnemiesKilled { get; set; }
        public List<string> UnlockedAchievements { get; set; }

        public bool IsActive { get; set; } = true;

        public int? Brain { get; set; }
        public int? Mana { get; set; }
        public int? MaxMana { get; set; }
    }
}