using Godot;

namespace VampireSurvivorsLike.Weapons {

    //Level 0 weapon => not equipped yet.
    public abstract class Weapon : Node2D {
    
        protected int Counter { get; set; }
        protected int AttackSpeed { get; set; }
        protected int Level { get; set; }
        protected float Damage {get; set; }
        protected int MaxLevel {get; set; }

        public abstract void Upgrade();
        public abstract string UpgradeMessage();
    }

}