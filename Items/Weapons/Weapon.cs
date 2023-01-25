using Godot;
using VampireSurvivorsLike.Items;

namespace VampireSurvivorsLike.Weapons {

    //Level 0 weapon => not equipped yet.
    public abstract class Weapon : Item {

        protected int Counter { get; set; }
        protected int AttackSpeed { get; set; }
        protected float Damage { get; set; }

    }

}