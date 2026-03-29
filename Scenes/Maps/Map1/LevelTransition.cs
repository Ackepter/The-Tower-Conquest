using Godot;

public partial class LevelTransition : Area2D
{
	[Export] public string NextLevelPath { get; set; } = "res://node_2d.tscn";
	[Export] public Vector2 SpawnPosition { get; set; } = new Vector2(510, 588); // Координаты появления
	
	public override void _Ready()
	{
		BodyEntered += OnBodyEntered;
	}
	
	private void OnBodyEntered(Node2D body)
	{
		if (body.IsInGroup("player") || body.Name == "MainCharacter")
		{
			// Сохраняем позицию появления в GlobalData
			GlobalData.Instance.SpawnPosition = SpawnPosition;
			GetTree().ChangeSceneToFile(NextLevelPath);
		}
	}
}
