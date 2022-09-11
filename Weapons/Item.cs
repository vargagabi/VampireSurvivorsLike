using Godot;

namespace VampireSurvivorsLike.Weapons
{
    public abstract class Item : Node2D
    {
        private int _counter;
        private int _attackSpeed;
        private int _level;

        public abstract void Upgrade();
    }
}