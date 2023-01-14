using Godot;

namespace VampireSurvivorsLike.Enemies {

    public class Enemy : KinematicBody2D {
        protected float Health { get; set; }
        protected float Strength { get; set; }
        protected int ExpValue { get; set; }
        protected float Speed { get; set; }
        protected KinematicBody2D Player { get; set; }
        private PackedScene ExpOrb { get; set; }

        protected Enemy() {
            this.ExpOrb = ResourceLoader.Load<PackedScene>("res://ExpOrbs/ExpOrb.tscn");
        }

        /*
         * When receiving damage reduce the Health by damage amount.
         * If the Health is less than or equals 0 remove the enemy and add an
         * instance of an ExpOrb at its place.
         */
        public void OnHit(float damage) {
            this.Health -= damage;
            if (this.Health <= 0) {
                this.QueueFree();
                Node2D expOrb = this.ExpOrb.Instance<Node2D>();
                expOrb.GlobalPosition = this.GlobalPosition;
                expOrb.Set("Experience", this.ExpValue);
                Node viewport = this.GetTree().Root.GetChild(0);
                viewport.CallDeferred("add_child", expOrb);
            }
        }
    }

}