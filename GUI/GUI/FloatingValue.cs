using Godot;

namespace VampireSurvivorsLike {

    public class FloatingValue : Position2D {

        private int value;
        private Color color;

        // Called when the node enters the scene tree for the first time.
        public override void _Ready() {
            Tween tween = GetNode<Tween>("Tween");
            Label valueLabel = GetNode<Label>("Label");

            valueLabel.Text = this.value.ToString();
            valueLabel.Modulate = this.color;
            Vector2 newPos = this.GlobalPosition;
            newPos.y -= 40;
            tween.InterpolateProperty(this, "position", this.GlobalPosition, newPos, 0.4f);
            tween.InterpolateProperty(valueLabel, "modulate:a", 1, 0, 1);
            tween.Start();
        }

        public static void CreateFloatingValue(Vector2 position, Color color, int value, Node parent) {
            FloatingValue floatingValue = ResourceLoader.Load<PackedScene>("res://GUI/GUI/FloatingValue.tscn")
                .Instance<FloatingValue>();
            floatingValue.color = color;
            floatingValue.value = value;
            position.y -= 20;
            floatingValue.GlobalPosition = position;
            parent.CallDeferred("add_child", floatingValue);
        }


        public void TweenCompleted() {
            this.CallDeferred("queue_free");
        }

    }

}