using Godot;
using VampireSurvivorsLike.Weapons;

public class Gun : Item {
    private KinematicBody2D player;
    private int NumberOfBullets { get; set; }
    private PackedScene bullet;
    private Node2D bulletNode;
    private int piercing = 1;
    private float bulletSpeed = 200;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready() {
        this.level = 0;
        this.counter = 0;
        this.attackSpeed = 100;
        this.player = GetNode<KinematicBody2D>("../../Player");
        this.NumberOfBullets = 1;
        this.bullet = (PackedScene)ResourceLoader.Load("res://Weapons/Gun/Bullet.tscn");
        this.bulletNode = this.bullet.Instance<Bullet>();
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta) {
        this.counter++;
        if (this.counter % this.attackSpeed == 0) {
            Shoot();
        }
    }

    private void Shoot() {
        for (int i = 0; i < this.NumberOfBullets; i++) {
            Bullet bulletInst = (Bullet)this.bullet.Instance();
            bulletInst.Set("piercing", this.piercing);
            bulletInst.Set("Direction", ((Vector2)this.player.Get("Direction"))
                .Rotated((i * Mathf.Pi / 12) - (Mathf.Pi / 12) * (this.NumberOfBullets - 1) / 2.0f).Normalized());
            bulletInst.GlobalPosition =
                this.player.GlobalPosition + ((Vector2)this.player.Get("Direction")).Normalized() * 10;
            bulletInst.Visible = true;
            AddChild(bulletInst);
            bulletInst.SetAsToplevel(true);
        }
    }

    public override void Upgrade() {
        this.level++;
        switch (this.level) {
            case 1:
                this.attackSpeed -= 20;
                break;
            case 2:
                this.NumberOfBullets++;
                break;
            case 3:
                this.piercing++;
                break;
            case 4:
                this.bulletSpeed += 50;
                break;
            case 5:
                this.NumberOfBullets++;
                break;
            case 6:
                this.piercing++;
                break;
            case 7:
                this.attackSpeed -= 10;
                break;
            default:
                this.NumberOfBullets++;
                break;
        }
    }

    public override string ToString() {
        switch (this.level) {
            case 0:
                return "Increase Attack Speed Of Gun";
            case 1:
                return "Increase Number Of Bullets Of Gun By 1";
            case 2:
                return "Increase Piercing Of Bullets By 1";
            case 3:
                return "Increase Bullet Speed";
            case 4:
                return "Increase Number Of Bullets By 1";
            case 5:
                return "Increase Piercing Of Bullets By 1";
            case 6:
                return "Increase Attack Speed And Number Of Bullets";
            default:
                return "Upgrade A Random Attribute Of The Gun";
        }
    }
}