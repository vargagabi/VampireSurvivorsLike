using Godot;
using System.Collections.Generic;

namespace VampireSurvivorsLike {

    public class Aura : Weapon {

        private float RadiusIncreaseAmount { get; set; }
        public float BonusExperience { get; set; }
        private CircleShape2D CollisionShape { get; set; }
        private Sprite AuraTexture { get; set; }
        private List<Node2D> overlappingBodies = new List<Node2D>();

        // Called when the node enters the scene tree for the first time.
        public override void _Ready() {
            this.SetIcon();
            this.Damage = 1f;
            this.Level = 0;
            this.MaxLevel = 7;
            this.Counter = 0;
            this.AttackSpeed = 50;
            this.BonusExperience = 0f;
            this.CollisionShape = this.GetNode<Area2D>("Area2D").GetChild<CollisionShape2D>(0).Shape as CircleShape2D;
            this.AuraTexture = this.GetNode<Sprite>("AuraTexture");
            if (this.CollisionShape != null) {
                this.RadiusIncreaseAmount = this.CollisionShape.Radius * 0.1f;
            }
        }

        public override void Upgrade() {
            this.Level++;
            switch (this.Level) {
                case 2:
                    this.IncreaseRadius();
                    break;
                case 3:
                    this.Damage += 0.1f;
                    break;
                case 4:
                    this.AttackSpeed -= 1;
                    break;
                case 5:
                    this.IncreaseRadius();
                    break;
                case 6:
                    this.BonusExperience = 0.1f;
                    break;
                case 7:
                    this.IncreaseRadius();
                    break;
            }
        }


        public override string UpgradeMessage() {
            switch (this.Level) {
                case 0: return "Aura: an ability to passively damage enemies around you.";
                case 1: return "Aura: Increase Aura radius by 10%.";
                case 2: return "Aura: Increase damage by 0.1.";
                case 3: return "Aura: Increase attack speed.";
                case 4: return "Aura: Increase Aura radius by 10%.";
                case 5: return "Aura: Increase experience dropped by enemies killed by Aura.";
                case 6: return "Aura: Increase Aura radius by 10%";
            }
            return "No more upgrades for Aura";
        }

        public override void SetIcon() {
            this.Icon = ResourceLoader.Load("res://MyPixelArts/images/AuraIcon.png") as Texture;
        }


        private void IncreaseRadius() {
            this.CollisionShape.Radius += this.RadiusIncreaseAmount;
            this.AuraTexture.Scale *= 1.1f;
        }

        public void OnBodyEntered(Node2D body) {
            if (body.HasMethod("OnHit")) {
                this.overlappingBodies.Add(body);
            }
        }

        public void OnBodyExited(Node2D body) {
            if (body.HasMethod("OnHit")) {
                this.overlappingBodies.Remove(body);
            }
        }

        // Called every frame. 'delta' is the elapsed time since the previous frame.
        public override void _Process(float delta) {
            if (++this.Counter % this.AttackSpeed == 0) {
                this.Counter = 0;
                foreach (Node2D body in this.overlappingBodies) {
                    Enemy enemy = (Enemy)body;
                    enemy.OnHit(this.Damage, this);
                }
            }
        }

        public override string ToString() {
            switch (this.Level) {
                case 0: return "Aura: an ability to damage enemies around you.";
                case 1: return "Aura: Increase Aura radius by 10%.";
                case 2: return "Aura: Increase damage by 0.1.";
                case 3: return "Aura: Increase attack speed.";
                case 4: return "Aura: Increase Aura radius by 10%.";
                case 5: return "Aura: Increase experience dropped by enemies killed by Aura.";
                case 6: return "Aura: Increase Aura radius by 10%";
            }
            return "No more upgrades for Aura";
        }

    }

}