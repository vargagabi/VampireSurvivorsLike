using Godot;

namespace VampireSurvivorsLike {

    //Level 0 weapon => not equipped yet.
    public abstract class Weapon : Item {

        protected int Counter { get; set; }
        protected int AttackSpeed { get; set; }
        protected int Damage { get; set; }

    }

}