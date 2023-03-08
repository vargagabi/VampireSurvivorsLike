using Godot;

namespace VampireSurvivorsLike {

    public abstract class Item : Node2D {

        public int Id { get; protected set; }
        public int Level { get; protected set; }
        public int MaxLevel { get; protected set; }
        public Texture Icon { get; protected set; }
        protected Player player;

        public abstract void Upgrade();
        public abstract string UpgradeMessage();

        public abstract void SetIcon();

        public abstract override string ToString();

    }

}