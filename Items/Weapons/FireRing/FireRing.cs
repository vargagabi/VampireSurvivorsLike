using Godot;
using System;

namespace VampireSurvivorsLike {

    public class FireRing : Weapon {

        private float counter = 0f;
        private float Speed = 2f;
        private const float DistanceFromCenter = 100f;
        private const float DistanceBetweenRings = 50f;

        public FireRing() {
            this.MaxLevel = 50;
            this.Icon = ResourceLoader.Load("res://MyPixelArts/images/FireRingIcon.png") as Texture;
            this.AttackSpeed = 50;
            this.Damage = 1;
        }

        // |electrons| = 2 * n^2
        // n^2 = e/2
        // n = sqrt (e/2)

        public override void _Process(float delta) {
            this.counter += delta;
            for (int i = 0; i < this.GetChildCount(); i++) {
                int currentShell = 1;
                int electronTilNextShell = 2;
                int electronsOnLowerShells = 0;
                while ((i + 1) > electronTilNextShell) {
                    electronsOnLowerShells += 2 * currentShell * currentShell;
                    currentShell++;
                    electronTilNextShell += 2 * currentShell * currentShell;
                }
                float distanceBetweenBullets = Mathf.Tau / (2 * currentShell * currentShell);
                if (electronTilNextShell > this.GetChildCount()) {
                    distanceBetweenBullets = Mathf.Tau / (this.GetChildCount() - electronsOnLowerShells);
                }
                float value = (this.counter * Speed + distanceBetweenBullets * (i + 1));
                if (currentShell % 2 == 0) {
                    value *= -1;
                }
                float distance = DistanceFromCenter + currentShell * DistanceBetweenRings;
                this.GetChild<Node2D>(i).GlobalPosition = this.GetParent<Node2D>().GlobalPosition +
                                                          new Vector2(Mathf.Sin(value) * distance,
                                                              Mathf.Cos(value) * distance);
            }
            if (this.counter >= 2 * Math.PI) {
                this.counter = 0;
            }
        }

        public override void Upgrade() {
            switch (++this.Level) {
                case 1: break;
                case 2:
                    this.Damage += 1;
                    break;
                default:
                    this.AddChild(this.GetChild(0).Duplicate());
                    break;
            }
        }

        public override string ToString() {
            switch (this.Level) {
                case 0: return "Spawns fireballs around you.";
                case 1: return "Increase damage by 1";
                default: return "Add 1 fireball";
            }
        }

        public void OnBodyEntered(Node2D body) {
            if (GameStateManagerSingleton.Instance.IsMultiplayer && !this.IsNetworkMaster()) {
                return;
            }
            if (body is Enemy enemy) {
                enemy.OnHit(this.Damage, this);
            }
        }

    }

}