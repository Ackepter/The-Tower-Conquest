using Godot;

public partial class GlobalData : Node
{
	public static GlobalData Instance { get; private set; }
	
	public int PlayerScore { get; set; } = 0;
	public int PlayerHealth { get; set; } = 100;
	public Vector2 SpawnPosition { get; set; } = Vector2.Zero;

	public override void _Ready()
	{
		Instance = this;
	}
}
