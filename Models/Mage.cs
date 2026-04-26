using System;

namespace BattleOfEntities.Models
{
    public class Mage : Entity
    {
        private static Random _rnd = new Random();
        private int _mana;
        private int _maxMana;
        private int _spellCooldown;  // Оставшиеся ходы до следующего заклинания

        public int Mana
        {
            get { return _mana; }
            private set { _mana = Math.Max(0, Math.Min(value, _maxMana)); }
        }

        public int MaxMana => _maxMana;
        public int SpellCooldown => _spellCooldown;

        public Mage(string name) : base(name)
        {
            // Маг: средние характеристики + магия
            BasePower = _rnd.Next(30, 40);      // Сила: 20-40
            MaxHealth = _rnd.Next(350, 450);    // Здоровье: 250-450
            Health = MaxHealth;
            _maxMana = _rnd.Next(50, 100);      // Макс. мана: 50-100
            _mana = _maxMana;
            _spellCooldown = 0;
        }

        public override int Power
        {
            get
            {
                // Обычная атака
                return BasePower;
            }
        }
        public int CastSpell()
        {
            if (_spellCooldown > 0)
            {
                return -1; 
            }

            if (_mana < 20)
            {
                return -2;  
            }
            _mana -= 20;

            // Устанавливаем перезарядку (3 хода)
            _spellCooldown = 3;

            // Урон заклинания: сила × 2.5 + случайный бонус
            int spellDamage = (int)(BasePower * 2.5) + _rnd.Next(10, 30);
            return spellDamage;
        }

        // Вызывать в конце каждого хода (уменьшать перезарядку)
        public void ReduceCooldown()
        {
            if (_spellCooldown > 0)
                _spellCooldown--;
        }

        // Восстановление маны после победы
        public void RestoreMana(int amount)
        {
            Mana += amount;
        }

        public override string GetCharacterType() => "Маг";
        public override string GetSpecialInfo() => $"✨ Мана: {Mana}/{MaxMana}\n" +
                                                   $"⚡ Сила: {Power}\n" +
                                                   $"{(SpellCooldown > 0 ? $"⏳ Заклинание перезаряжается: {SpellCooldown} хода" : "🔮 ЗАКЛИНАНИЕ ГОТОВО!")}";
    }
}