using Godot;

namespace VampireSurvivorsLike {

    //Level 0 weapon => not equipped yet.
    public abstract class Weapon : Item {

        protected int Counter = 0;
        protected int AttackSpeed;
        protected int Damage;

    }

}