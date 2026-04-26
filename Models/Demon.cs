namespace BattleOfEntities.Models
{
    public class Demon : Entity
    {
        private static Random _rnd = new Random();
        public int Brain { get; private set; }

        public Demon(string name) : base(name)
        {
            // Демон: базовые характеристики ниже, но ум даёт преимущество
            BasePower = _rnd.Next(20, 35);
            MaxHealth = _rnd.Next(150, 350);
            Health = MaxHealth;
            Brain = _rnd.Next(1, 4);
        }

        public override int Power => BasePower * Brain;

        public void IncreaseBrain()
        {
            Brain++;
            AddBattleLog($"🧠 Ум демона увеличен! Теперь ум: {Brain}", Color.Cyan);
        }

        public override string GetCharacterType() => "Демон";
        public override string GetSpecialInfo() => $"🧠 Ум: {Brain}";

        private void AddBattleLog(string message, Color color) { }
    }
}