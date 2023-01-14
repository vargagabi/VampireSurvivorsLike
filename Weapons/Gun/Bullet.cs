using Godot;

public class Bullet : Node2D {
    private Vector2 Direction { set; get; }
    private float speed = 200;
    private float damage = 5;
    private int counter = 0;
    private int lifeSpan = 500;
    private int piercing = 1;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready() {
    }

    //  // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta) {
        this.counter++;
        if (this.counter % this.lifeSpan == 0) {
            QueueFree();
            return;
        }

        GlobalPosition += Direction * this.speed * delta;
    }

    public void OnBodyEntered(Node body) {
        if (body.HasMethod("OnHit") && body.GetClass() == "KinematicBody2D") {
            ((Enemy1)body).OnHit(this.damage);
            this.piercing--;
        }
        if (this.piercing <= 0) {
            QueueFree();
        }
    }
}