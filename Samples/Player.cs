using System;
using System.Numerics;

namespace Game.Sample.Samples
{
    public sealed class Player : Entity
    {
        private static readonly Random RNG = new Random();

        public string Name => _name;

        public uint Attack() => (uint)(_attackLevel * 10 + 5) / 3;

        public uint Defense() => (uint)Math.Abs(_hp - _defenseLevel) / 2;

        public ushort AttackLevel { get => _attackLevel; set => _attackLevel = value; }
        public ushort DefenseLevel { get => _defenseLevel; set => _defenseLevel = value; }
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

        public int EnemiesKilledCount = 0;

        public Player(string name) : base(name)
        {
            _attackLevel = 1;
            _defenseLevel = 1;
            _experience = 0;
            _hp = 100;
            _mp = 0;
            _position = Vector2.Zero;

            OnDeath += (s, e) =>
            {
                if (e is Enemy)
                {
                    var enemy = e as Enemy;
                    enemy.PlayersKilledCount++;

                    Console.WriteLine($"[MvP] Enemy {e.Id} killed the Player {Id}!");
                }
                else
                    Console.WriteLine($"[PvP] Player {e.Id} killed the Player {Id}!");

                IsDead = true;
                OnDeath = null;
            };
        }

        protected override void InternalBehavior()
        {
            if (IsDead)
                return;

            _position.X = RNG.Next(-2, 2);
            _position.Y = RNG.Next(-2, 2);
        }
    }
}
