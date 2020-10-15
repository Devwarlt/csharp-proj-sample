using System;
using System.Numerics;

namespace Game.Sample.Samples
{
    public sealed class Enemy : Entity
    {
        private static readonly Random RNG = new Random();

        public string Name => _name;

        public uint Attack() => (uint)(_attackLevel * 10 + 5) / 3;

        public uint Defense() => (uint)Math.Abs(_hp - _defenseLevel) / 2;

        public uint Experience { get => _experience; set => _experience = value; }

        public uint MaxHP { get; private set; } = 0;

        public uint HP
        {
            get => _hp;
            set
            {
                if (MaxHP == 0)
                    MaxHP = _hp;

                _hp = value;
            }
        }

        public uint MP { get => _mp; set => _mp = value; }
        public Vector2 Position { get => _position; set => _position = value; }

        public bool IsDead { get; private set; } = false;

        private bool _isLootDropped = false;

        public int PlayersKilledCount = 0;

        public Enemy(string name, ushort attackLevel, ushort defenseLevel) : base(name)
        {
            _attackLevel = attackLevel;
            _defenseLevel = defenseLevel;
            _experience = 0;
            _hp = 100;
            _mp = 0;
            _position = Vector2.Zero;

            OnDeath += (s, e) =>
            {
                if (_isLootDropped || !(e is Player))
                    return;

                _isLootDropped = true;

                var player = e as Player;
                player.Experience += Experience;
                player.EnemiesKilledCount++;

                Console.WriteLine($"[Loot] Enemy {Id} dropped a loot and Player {e.Id} gained {Experience} EXP!");

                IsDead = true;
                OnDeath = null;
            };
        }

        protected override void InternalBehavior()
        {
            if (IsDead)
                return;

            _position.X = RNG.Next(-1, 1);
            _position.Y = RNG.Next(-1, 1);
        }
    }
}
