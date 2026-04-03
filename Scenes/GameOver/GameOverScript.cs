using Godot;
using System;

public partial class GameOverScript : Camera2D
{
    public override void _Process(double delta)
    {
        
        if (Input.IsKeyPressed(Key.Escape))
        {
            GetTree().Quit();
        }
    }
}
