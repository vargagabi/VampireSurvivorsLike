using Godot;

public class Bullet : Node2D
{
    private Vector2 Direction { set; get; }
    private float _speed = 200;
    private float _damage = 5;
    private int _counter = 0;
    private int _lifeSpan = 500;
    private int _piercing = 1;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
    }

    //  // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        _counter++;
        if (_counter % _lifeSpan == 0)
        {
            QueueFree();
            return;
        }

        GlobalPosition += Direction * _speed * delta;
    }

    public void OnBodyEntered(Node body)
    {
        if (body.HasMethod("OnHit") && body.GetClass() == "KinematicBody2D")
        {
            ((Enemy1)body).OnHit(_damage);
            _piercing--;
        }

        if (_piercing <= 0)
        {
            QueueFree();
        }
    }
}