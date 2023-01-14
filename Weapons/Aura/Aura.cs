using Godot;
using System;
using System.Collections.Generic;
using VampireSurvivorsLike.Enemies;
using VampireSurvivorsLike.Weapons;

public class Aura : Weapon {

    private float RadiusIncreaseAmount { get; set; }
    private CircleShape2D CollisionShape { get; set; }
    private Sprite AuraTexture { get; set; }
    private List<Node2D> overlappingBodies = new List<Node2D>();

    // Called when the node enters the scene tree for the first time.
    public override void _Ready() {
        this.Damage = 0.1f;
        this.Level = 0;
        this.Counter = 0;
        this.AttackSpeed = 5;
        this.CollisionShape = this.GetNode<Area2D>("Area2D").GetChild<CollisionShape2D>(0).Shape as CircleShape2D;
        this.AuraTexture = this.GetNode<Sprite>("AuraTexture");
        if (this.CollisionShape != null) {
            this.RadiusIncreaseAmount = this.CollisionShape.Radius * 0.1f;
        }
    }

    public override void Upgrade() {
        this.Level++;
        switch (this.Level) {
            case 2: this.IncreaseRadius(); break;
            case 3: break;
            case 4: break;
            case 5: break;
            case 6: break;
            case 7: break;
            case 8: break;
            case 9: break;
            case 10: break;
            case 11: break;
        }
    }

    public override string UpgradeMessage() {
        switch (this.Level) {
            case 0: return "Aura: an ability to passively damage enemies around you.";
            case 1: return "Increase Aura radius by 10%";
            case 2: break;
            case 3: break;
            case 4: break;
            case 5: break;
            case 6: break;
            case 7: break;
            case 8: break;
            case 9: break;
        }
        return "No more upgrades for Aura";
    }


    private void IncreaseRadius() {
        this.CollisionShape.Radius += this.RadiusIncreaseAmount;
        this.AuraTexture.Scale *= 1.1f;
    }

    public void OnBodyEntered(Node2D body) {
        GD.Print("Body entered: ");
        GD.Print(body);
        if (body.HasMethod("OnHit")) {
            this.overlappingBodies.Add(body);
            GD.Print(this.overlappingBodies.Count);
        }
    }

    public void OnBodyExited(Node2D body) {
        if (body.HasMethod("OnHit")) {
            this.overlappingBodies.Remove(body);
            GD.Print(this.overlappingBodies.Count);
        }
    }

    //  // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta) {
        if (++this.Counter % this.AttackSpeed == 0) {
            this.Counter = 0;
            foreach (Node2D body in this.overlappingBodies) {
                ((Enemy)body).OnHit(this.Damage);
            }
        }
    }

}