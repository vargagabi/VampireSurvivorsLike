using Godot;
using System;

namespace VampireSurvivorsLike {

    public class HUD : Control {

        private float elapsedTime;
        private HBoxContainer itemControl;
        private int nextEmptyItemFrame = 0;

        // Called when the node enters the scene tree for the first time.
        public override void _Ready() {
            this.elapsedTime = 0;
            this.itemControl = GetNode<HBoxContainer>("ItemControl");
            GetNode<Label>("GoldContainer/Label").Text = "0";
            this.CreateItemFrames();
        }

        // Called every frame. 'delta' is the elapsed time since the previous frame.
        public override void _Process(float delta) {
            this.elapsedTime += delta;
            GetNode<Label>("TimerControl/TimeLabel").Text =
                TimeSpan.FromSeconds(this.elapsedTime).ToString("hh':'mm':'ss");
        }

        private void CreateItemFrames() {
            for (int i = 1; i < ItemManager.MaximumItemCount; i++) {
                Control node = this.itemControl.GetChild<Control>(0).Duplicate() as Control;
                if (node == null) {
                    continue;
                }
                node.Name = $"Item{i}";
                node.RectPosition = new Vector2(i * 64, 0);
                this.itemControl.AddChild(node);
            }
        }

        public void SetItem(Texture icon, int level) {
            //Already have this item
            foreach (Control frame in this.itemControl.GetChildren()) {
                if (frame.GetChild<TextureRect>(1).Texture == null ||
                    !frame.GetChild<TextureRect>(1).Texture.Equals(icon)) {
                    continue;
                }
                frame.GetChild<Label>(2).Text = level.ToString();
                return;
            }

            //New item
            Control itemFrame = this.itemControl.GetChild<Control>(this.nextEmptyItemFrame++);
            itemFrame.GetChild<TextureRect>(1).Texture = icon;
            itemFrame.GetChild<Label>(2).Text = level.ToString();
        }

        public void SetHealthLabel(int currentHealth) {
            this.GetNode<Label>("HpControl/HpNumber").Text = currentHealth.ToString();
        }

        public void SetExpBar(int percent) {
            this.GetNode<TextureProgress>("ExpBar").Value = percent;
        }

        public void SetLevel(int value) {
            this.GetNode<Label>("LevelControl/LevelNumber").Text = value.ToString();
        }

        public void SetGold(int value) {
            this.GetNode<Label>("GoldContainer/Label").Text = value.ToString();
        }

    }

}