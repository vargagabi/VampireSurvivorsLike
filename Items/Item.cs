using Godot;

namespace VampireSurvivorsLike {

    public abstract class Item : Node2D {

        public int Id;
        public int Level = 0;
        public int MaxLevel;
        public Texture Icon;

        public abstract void Upgrade();

        public abstract override string ToString();

    }

}