using System.Collections.Generic;

namespace BattleOfEntities.Models
{
    public class Achievement
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int RequiredValue { get; set; }
        public string Type { get; set; } // potion, artifact, kill, location, boss, class
        public bool IsUnlocked { get; set; }

        public Achievement(string id, string name, string description, int requiredValue, string type)
        {
            Id = id;
            Name = name;
            Description = description;
            RequiredValue = requiredValue;
            Type = type;
            IsUnlocked = false;
        }

        public static List<Achievement> GetAllAchievements()
        {
            return new List<Achievement>
            {
                new Achievement("first_kill", "Первый шаг", "Победить первого врага", 1, "kill"),
                new Achievement("potion_5", "Любитель зелий", "Выпить 5 зелий", 5, "potion"),
                new Achievement("potion_10", "Аптекарь", "Выпить 10 зелий", 10, "potion"),
                new Achievement("artifact_3", "Коллекционер", "Собрать 3 артефакта", 3, "artifact"),
                new Achievement("artifact_7", "Музей", "Собрать 7 артефактов", 7, "artifact"),
                new Achievement("location_1", "Мастер леса", "Пройти Тёмный лес", 1, "location"),
                new Achievement("location_2", "Покоритель гор", "Пройти Горящие горы", 2, "location"),
                new Achievement("location_3", "Мастер тьмы", "Пройти Цитадель тьмы", 3, "location"),
                new Achievement("complete_game", "Повелитель тьмы", "Пройти игру полностью", 1, "complete"),
                new Achievement("demon_player", "Демоническая сущность", "Играть за демона", 1, "class"),
                new Achievement("mage_player", "Магическая сила", "Играть за мага", 1, "class"),
                new Achievement("challenge", "Бессмертный", "Пройти игру без использования зелий", 1, "challenge"),
                new Achievement("boss_killer", "Убийца босса", "Победить Повелителя тьмы", 1, "boss"),
                new Achievement("kill_10", "Истребитель", "Убить 10 врагов", 10, "kill_total"),
                new Achievement("damage_1000", "Сокрушитель", "Нанести 1000 урона за игру", 1000, "damage")
            };
        }
    }
}