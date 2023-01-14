using Godot;

namespace VampireSurvivorsLike.Weapons
{
    public abstract class Item : Node2D
    {
        protected int counter;
        protected int attackSpeed;
        protected int level;

        public abstract void Upgrade();
    }
}