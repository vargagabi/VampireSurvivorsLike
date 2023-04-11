using Godot;
using System.Collections.Generic;

namespace VampireSurvivorsLike {

    public class Aura : Weapon {

        public float BonusExperience;
        private readonly List<Enemy> overlappingBodies = new List<Enemy>();

        // Called when the node enters the scene tree for the first time.
        public override void _Ready() {
            this.MaxLevel = 7;
            this.Icon = ResourceLoader.Load("res://MyPixelArts/images/AuraIcon.png") as Texture;
            this.AttackSpeed = 50;
            this.Damage = 1;
            this.BonusExperience = 0.1f;
        }

        // Called every frame. 'delta' is the elapsed time since the previous frame.
        public override void _Process(float delta) {
            if (GameStateManagerSingleton.Instance.IsMultiplayer && !this.IsNetworkMaster()) {
                return;
            }
            if (++this.Counter > this.AttackSpeed) {
                this.Counter = 0;
                foreach (Enemy enemy in this.overlappingBodies) {
                    enemy.OnHit(this.Damage, this);
                }
            }
        }

        private void IncreaseRadius() {
            ((CircleShape2D)this.GetNode<Area2D>("Area2D").GetChild<CollisionShape2D>(0).Shape).Radius *= 1.1f;
            this.GetNode<Sprite>("AuraTexture").Scale *= 1.1f;
        }

        public override void Upgrade() {
            this.Level++;
            switch (this.Level) {
                case 2:
                    this.IncreaseRadius();
                    break;
                case 3:
                    this.Damage += 1;
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

        public override string ToString() {
            switch (this.Level) {
                case 0: return "Aura: an ability to damage enemies around you.";
                case 1: return "Aura: Increase Aura radius by 10%.";
                case 2: return "Aura: Increase damage by 1.";
                case 3: return "Aura: Increase attack speed.";
                case 4: return "Aura: Increase Aura radius by 10%.";
                case 5: return "Aura: Increase experience dropped by enemies killed by Aura.";
                case 6: return "Aura: Increase Aura radius by 10%";
            }
            return "No more upgrades for Aura";
        }

        public void OnBodyEntered(Node2D body) {
            if (GameStateManagerSingleton.Instance.IsMultiplayer && !this.IsNetworkMaster()) {
                return;
            }
            if (body is Enemy enemy) {
                this.overlappingBodies.Add(enemy);
            }
        }

        public void OnBodyExited(Node2D body) {
            if (GameStateManagerSingleton.Instance.IsMultiplayer && !this.IsNetworkMaster()) {
                return;
            }
            if (body is Enemy enemy) {
                this.overlappingBodies.Remove(enemy);
            }
        }

    }

}