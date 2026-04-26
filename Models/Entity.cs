using System.Collections.Generic;

namespace BattleOfEntities.Models
{
    public abstract class Entity
    {
        protected static Random _rnd = new Random();

        public string Name { get; set; }
        public int Health { get; set; }
        public int MaxHealth { get; set; }
        public int BasePower { get; set; }
        public List<string> Artifacts { get; set; }
        public int PotionsUsed { get; set; }
        public int TotalDamageDealt { get; set; }
        public int EnemiesKilled { get; set; }

        public Entity(string name)
        {
            Name = name;
            BasePower = _rnd.Next(10, 40);
            MaxHealth = _rnd.Next(100, 300);
            Health = MaxHealth;
            Artifacts = new List<string>();
            PotionsUsed = 0;
            TotalDamageDealt = 0;
            EnemiesKilled = 0;
        }

        public virtual int Power => BasePower;

        public virtual void TakeDamage(int damage)
        {
            Health -= damage;
            if (Health < 0) Health = 0;
        }

        public virtual void Heal(int amount)
        {
            Health += amount;
            if (Health > MaxHealth) Health = MaxHealth;
        }

        public bool IsDead => Health <= 0;

        public void AddArtifact(string artifact)
        {
            Artifacts.Add(artifact);
        }

        public void IncreasePower(int amount)
        {
            BasePower += amount;
        }

        public virtual void ApplyArtifactBonus()
        {
            foreach (var artifact in Artifacts)
            {
                if (artifact.Contains("силы"))
                    BasePower += 5;
                else if (artifact.Contains("здоровья"))
                    Heal(20);
            }
        }

        public virtual string GetCharacterType() => "Сущность";
        public virtual string GetSpecialInfo() => "";
    }
}