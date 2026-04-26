namespace BattleOfEntities.Models
{
    public class Monster : Entity
    {
        public Monster(string name) : base(name)
        {
            // Монстр: высокое здоровье и сила
            BasePower = _rnd.Next(30, 40);
            MaxHealth = _rnd.Next(600, 700);
            Health = MaxHealth;
        }

        public override string GetCharacterType() => "Монстр";
        public override string GetSpecialInfo() => "💪 Сильный и выносливый боец";

        public override int Power => BasePower;
        public void IncreaseMaxHealth(int amount)
        {
            MaxHealth += amount;
            Health += amount;
        }
    }
}