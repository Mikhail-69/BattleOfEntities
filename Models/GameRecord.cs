namespace BattleOfEntities.Models
{
    public class GameRecord
    {
        public int Id { get; set; }
        public string PlayerName { get; set; }
        public string CharacterType { get; set; }
        public int FinalHealth { get; set; }
        public int FinalPower { get; set; }
        public int TotalDamageDealt { get; set; }
        public int EnemiesKilled { get; set; }
        public int PotionsUsed { get; set; }
        public string PlayTime { get; set; }
        public string CompletionDate { get; set; }
        public bool IsVictory { get; set; }
    }
}