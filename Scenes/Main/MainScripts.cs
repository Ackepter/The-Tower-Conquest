using Godot;
using System;

public partial class MainScripts : Node2D
{
	public override void _Process(double delta)
	{
		
		if (Input.IsKeyPressed(Key.Escape))
		{
			GetTree().Quit();
		}
	}

}
