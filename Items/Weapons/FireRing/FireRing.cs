using Godot;
using System;
using VampireSurvivorsLike;

public class FireRing : Weapon {

    private float counter = 0;
    private float speed = 1f;

    // private float distanceBetweenBullets;
    private readonly float distanceFromCenter = 100f;
    private readonly float distanceBetweenRings = 50f;

    private Player player;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready() {
        GD.Print("FIRERING!");
    }

    // |electrons| = 2 * n^2
    // n^2 = e/2
    // n = sqrt (e/2)

    public override void _Input(InputEvent @event) {
        if (@event.IsActionPressed("ui_space")) {
            this.AddChild(this.GetChild(0).Duplicate());
            GD.Print("FIRERING!");
        }
    }

    public override void _Process(float delta) {
        this.counter += delta;

        // this.distanceBetweenBullets = Mathf.Tau / this.GetChildCount();
        // this.c += 1;
        // if (this.c % 500 == 0) {
        // GD.Print("---------------------");
        // }
        for (int i = 0; i < GetChildCount(); i++) {
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
            float value = (this.counter * this.speed + distanceBetweenBullets * (i + 1));
            if (currentShell % 2 == 0) {
                value *= -1;
            }
            float distance = this.distanceFromCenter + currentShell * this.distanceBetweenRings;
            this.GetChild<Node2D>(i).GlobalPosition = this.GetParent<Node2D>().GlobalPosition + new Vector2(Mathf.Sin(value) * distance, Mathf.Cos(value) * distance);

            // if (this.c % 500 == 0) {
            // this.c = 0;
            // GD.Print("Atom: " + (i + 1) + " Shell: " + currentShell + " Lower Shells: " + electronsOnLowerShells + " till next: " + electronTilNextShell);
            // }
        }
        if (this.counter >= 2 * Math.PI) {
            this.counter = 0;
        }
    }

    public override void Upgrade() {
        // throw new NotImplementedException();
    }

    public override string UpgradeMessage() {
        // throw new NotImplementedException();
        return "hello";
    }

    public override void SetIcon() {
        // throw new NotImplementedException();
    }

    public override string ToString() {
        // throw new NotImplementedException();
        return "Hello";
    }

}