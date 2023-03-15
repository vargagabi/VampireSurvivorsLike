using Godot;
using System;

namespace VampireSurvivorsLike {

    public class RewardItemContainer : Control {

        public int Index {get; private set;}
        private Button button;
        private Label label;

        [Signal] public delegate void ButtonPressed(int index);

        public override void _Ready() {
            this.Index = -1;
            this.button = GetChild<Button>(0);
            this.label = GetChild<Label>(1);
        }

        public void SetItemContainer(int index, Texture icon, string text, LevelUpScreen receiver) {
            this.Index = index;
            this.button.Icon = icon;
            this.label.Text = text;
            Connect(nameof(ButtonPressed), receiver, nameof(receiver.OnRewardSelected));
        }


        public void OnButtonPressed() {
            EmitSignal(nameof(ButtonPressed), this.Index);
        }

    }

}