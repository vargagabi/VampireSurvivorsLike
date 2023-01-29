using Godot;
using System;

namespace VampireSurvivorsLike {

    public class RewardItemContainer : Control {

        private int index;
        private Button button;
        private Label label;

        [Signal] public delegate void ButtonPressed(int index);

        public override void _Ready() {
            this.index = -1;
            this.button = GetChild<Button>(0);
            this.label = GetChild<Label>(1);
        }

        public void SetItemContainer(int index, Texture icon, string text, LevelUpScreen receiver) {
            this.index = index;
            this.button.Icon = icon;
            this.label.Text = text;
            Connect(nameof(ButtonPressed), receiver,nameof(receiver.OnRewardSelected));
            if (this.index == 0) {
                this.button.GrabFocus();
            }
        }

        public void OnButtonPressed() {
            GD.Print($"Signal sent about chosen reward with index {this.index}");
            EmitSignal(nameof(ButtonPressed), this.index);
        }

    }

}