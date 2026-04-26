using System.Collections.Generic;

namespace BattleOfEntities.Models
{
    public class Enemy : Entity
    {
        public int Level { get; set; }

        public Enemy(string name, int level, int basePower, int health) : base(name)
        {
            Level = level;
            BasePower = basePower;
            MaxHealth = health;
            Health = health;
        }

        public static List<Enemy> GetEnemiesForLocation(int locationIndex)
        {
            var enemies = new List<Enemy>();
            var rnd = new System.Random();

            switch (locationIndex)
            {
                case 1: // 2 противника
                    enemies.Add(new Enemy("Гоблин", 1, rnd.Next(15, 25), rnd.Next(80, 120)));
                    enemies.Add(new Enemy("Лесной волк", 1, rnd.Next(20, 30), rnd.Next(100, 150)));
                    break;

                case 2: // 4 противника
                    enemies.Add(new Enemy("Огненный элементаль", 2, rnd.Next(30, 45), rnd.Next(150, 200)));
                    enemies.Add(new Enemy("Магмодемон", 2, rnd.Next(35, 50), rnd.Next(180, 250)));
                    enemies.Add(new Enemy("Лавовый голем", 2, rnd.Next(40, 55), rnd.Next(200, 280)));
                    enemies.Add(new Enemy("Дракончик", 2, rnd.Next(45, 60), rnd.Next(220, 300)));
                    break;

                case 3: // 6 противников
                    enemies.Add(new Enemy("Рыцарь смерти", 3, rnd.Next(50, 70), rnd.Next(250, 350)));
                    enemies.Add(new Enemy("Некромант", 3, rnd.Next(55, 75), rnd.Next(280, 380)));
                    enemies.Add(new Enemy("Тёмный маг", 3, rnd.Next(60, 80), rnd.Next(300, 400)));
                    enemies.Add(new Enemy("Древний демон", 3, rnd.Next(70, 90), rnd.Next(350, 450)));
                    enemies.Add(new Enemy("Ледяной дракон", 3, rnd.Next(80, 100), rnd.Next(400, 500)));
                    enemies.Add(new Enemy("Повелитель тьмы", 3, rnd.Next(100, 150), rnd.Next(500, 700)));
                    break;
            }

            return enemies;
        }

        public override string GetCharacterType() => "Враг";
    }
}