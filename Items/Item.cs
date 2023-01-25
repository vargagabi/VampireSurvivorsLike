using Godot;

namespace VampireSurvivorsLike.Items {

    public abstract class Item : Node2D {

        public int Level { get; protected set; }
        public int MaxLevel { get; protected set; }
        public Texture Icon { get; protected set; }

        public abstract void Upgrade();
        public abstract string UpgradeMessage();

        public abstract void SetIcon();
    }

}