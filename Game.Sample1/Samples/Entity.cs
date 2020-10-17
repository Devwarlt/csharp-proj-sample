using System;
using System.Numerics;

namespace Game.Sample1.Samples
{
    public abstract class Entity
    {
        protected static int LAST_ID = 0;

        public readonly int Id;

        protected readonly string _name;

        protected ushort _attackLevel;
        protected ushort _defenseLevel;
        protected uint _experience;
        protected uint _hp;
        protected uint _mp;
        protected Vector2 _position;

        public EventHandler<Entity> OnDeath;

        protected Entity(string name)
        {
            Id = GetNextId;

            _name = name;
        }

        private int GetNextId => ++LAST_ID;

        public void Behavior() => InternalBehavior();

        public void ShowCurrentPosition() => Console.WriteLine($"{GetType().Name} {Id} -> pos: {_position}");

        // optional
        protected virtual void InternalBehavior()
        { }
    }
}
