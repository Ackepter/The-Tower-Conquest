using Godot;
using System;

public partial class MannequinScript : BaseEnemyScript
{
    protected override int MaxHp => 50;

    private EnteredWall _wall;

    private bool isDied = false;
    public override void _Ready()
    {
        _wall = GetNode<EnteredWall>("../EnteredWall");
        base._Ready();
    }

    public override void Die()
    {
        if(!isDied) _wall.QueueFree();
        isDied = true;
    }

    public override void TakeDamage(int value)
    {
        _sprite.Play("hurt");
        base.TakeDamage(value);
        var timer = GetTree().CreateTimer(1f);
        timer.Timeout += () => {
            if (IsInstanceValid(this)) {
                _sprite.Play("stay");
            }
        };

    }

}
