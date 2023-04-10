using Godot;

namespace VampireSurvivorsLike {

    public class RewardItemContainer : Control {

        private int index;

        [Signal] public delegate void ButtonPressed(int index);

        public void SetItemContainer(int index, Texture icon, string text, LevelUpScreen receiver) {
            this.index = index;
            this.GetChild<Button>(0).Icon = icon;
            this.GetChild<Label>(1).Text = text;
            this.Connect(nameof(ButtonPressed), receiver, nameof(receiver.OnRewardSelected));
        }

        public void OnButtonPressed() {
            this.EmitSignal(nameof(ButtonPressed), this.index);
        }

    }

}