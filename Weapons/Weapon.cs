using Godot;

namespace VampireSurvivorsLike.Weapons {

    //Level 0 weapon => not equipped yet.
    public abstract class Weapon : Node2D {
    
        protected int Counter { get; set; }
        protected int AttackSpeed { get; set; }
        public int Level { get; protected set; }
        protected float Damage {get; set; }
        public int MaxLevel {get; protected set; }

        public abstract void Upgrade();
        public abstract string UpgradeMessage();
    }

}