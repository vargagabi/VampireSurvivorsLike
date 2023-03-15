using Godot;
using System;

namespace VampireSurvivorsLike {

    public class FloatingValue : Position2D {

        private Tween tween { get; set; }
        private Label DamageValue { get; set; }
        private int amount { get; set; }
        private Color color { get; set; }

        // Called when the node enters the scene tree for the first time.
        public override void _Ready() {
            this.tween = GetNode<Tween>("Tween");
            this.DamageValue = GetNode<Label>("Label");

            this.DamageValue.Text = amount.ToString();
            this.DamageValue.Modulate = this.color;
            Vector2 newPos = this.GlobalPosition;
            newPos.y -= 30;
            this.tween.InterpolateProperty(this, "position", this.GlobalPosition, newPos, 0.5f);
            this.tween.InterpolateProperty(this.DamageValue, "modulate:a", 1, 0, 1);
            this.tween.Start();
        }

        public static void CreateFloatingValue(Vector2 position, Color color, int amount, Node parent) {
            FloatingValue floatingValue = ResourceLoader.Load<PackedScene>("res://GUI/GUI/FloatingValue.tscn")
                .Instance<FloatingValue>();
            floatingValue.color = color;
            floatingValue.amount = amount;
            position.y -= 20;
            floatingValue.GlobalPosition = position;
            parent.CallDeferred("add_child", floatingValue);
        }


        public void TweenCompleted() {
            CallDeferred("queue_free");
        }

    }

}