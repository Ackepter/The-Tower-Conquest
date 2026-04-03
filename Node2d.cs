using Godot;
using System;

public partial class Node2d : Node2D
{
	private AudioStreamPlayer _audioPlayer;

    public override void _Ready()
    {
        _audioPlayer = GetNode<AudioStreamPlayer>("audioPlayer");
		_audioPlayer.Play();
    }

	public override void _Process(double delta)
	{
		if (Input.IsKeyPressed(Key.Escape))
		{
			GetTree().Quit();
		}
	}

}
