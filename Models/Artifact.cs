using System.Collections.Generic;

namespace BattleOfEntities.Models
{
    public class Artifact
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int PowerBonus { get; set; }
        public int HealthBonus { get; set; }
        public int ManaBonus { get; set; }
        public int CritChance { get; set; }  // Шанс критического удара (%)
        public int LifeSteal { get; set; }    // Вампиризм (%)

        public Artifact(string name, string desc, int powerBonus = 0, int healthBonus = 0,
                        int manaBonus = 0, int critChance = 0, int lifeSteal = 0)
        {
            Name = name;
            Description = desc;
            PowerBonus = powerBonus;
            HealthBonus = healthBonus;
            ManaBonus = manaBonus;
            CritChance = critChance;
            LifeSteal = lifeSteal;
        }

        public static List<Artifact> GetAllArtifacts()
        {
            return new List<Artifact>
            {
                new Artifact("Меч ярости", "Увеличивает силу на 15", 15, 0, 0, 0, 0),
                new Artifact("Щит дракона", "Увеличивает здоровье на 50", 0, 50, 0, 0, 0),
                new Artifact("Амулет вампира", "10% вампиризма", 0, 0, 0, 0, 10),
                new Artifact("Кольцо критической", "15% крит. урон x2", 0, 0, 0, 15, 0),
                new Artifact("Плащ теней", "Сила +5, Здоровье +20", 5, 20, 0, 0, 0),
                new Artifact("Кристалл маны", "Восстанавливает 30 маны", 0, 0, 30, 0, 0),
                new Artifact("Корона мудреца", "Сила +10, Мана +20", 10, 0, 20, 0, 0),
                new Artifact("Наручи силы", "Сила +8", 8, 0, 0, 0, 0),
                new Artifact("Сапоги скорости", "5% крит. урона", 0, 0, 0, 5, 0),
                new Artifact("Кольцо жизни", "Здоровье +30, 5% вампиризм", 0, 30, 0, 0, 5),
                new Artifact("Демонический глаз", "Сила +12, 10% крит", 12, 0, 0, 10, 0),
                new Artifact("Сердце леса", "Здоровье +40, каждое зелье лечит +20", 0, 40, 0, 0, 0)
            };
        }
    }
}