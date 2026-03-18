using Godot;
using System;

public partial class MainHeroScript : CharacterBody2D
{
    public const float Speed = 1f;

    public override void _PhysicsProcess(double delta)
    {
        var velocity = new Vector2();

        if (Input.IsKeyPressed(Key.W)){
            //вверх
            velocity.Y += -Speed;
        }
        if (Input.IsKeyPressed(Key.S)){
            //вниз
            velocity.Y += Speed;
        }

        if (Input.IsKeyPressed(Key.A)){
            //влево
            velocity.X += -Speed;
        }
        if (Input.IsKeyPressed(Key.D)){
            //вправо
            velocity.X += Speed;
        }

        MoveAndCollide(velocity);
    }
}
