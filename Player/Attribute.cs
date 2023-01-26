using Godot;

namespace VampireSurvivorsLike {

    public class Attribute {

        private string Message { get; set;}
        private readonly float initialValue;
        private CircleShape2D shape;
        private readonly float bonusModifier;
        private int level;

        public Attribute(string message, float initialValue, float bonusInPercent) {
            this.Message = message;
            this.initialValue = initialValue;
            this.bonusModifier = bonusInPercent / 100f;
        }

        public Attribute(string message, CircleShape2D shape, float bonusInPercent) {
            this.Message = message;
            this.shape = shape;
            this.initialValue = shape.Radius;
            this.bonusModifier = bonusInPercent / 100f;
        }

        public float GetCurrentValue() {
            return this.initialValue + (this.initialValue * this.bonusModifier) * this.level;
        }

        public void Increase() {
            this.level++;
            if (this.shape != null) {
                this.shape.Radius = this.initialValue + this.initialValue * this.bonusModifier * this.level;
            }
        }

        public override string ToString() {
            return this.Message + " " + this.bonusModifier*100 + "%";
        }

    }

}