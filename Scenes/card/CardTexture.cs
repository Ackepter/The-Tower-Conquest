using Godot;

public partial class CardTexture : TextureRect
{
	[Signal] public delegate void CardClickedEventHandler(string upgradeId);
	
	[Export] public string UpgradeId = "default_id";
	
	private Color _normalColor = new(1, 1, 1, 0.7f);
	private Color _hoverColor = new(1, 1, 1, 1.0f);
	
	public override void _Ready()
	{
		MouseFilter = Control.MouseFilterEnum.Stop;
		ProcessMode = ProcessModeEnum.Always;
		Modulate = _normalColor;
		
		MouseEntered += () => Modulate = _hoverColor;
		MouseExited += () => Modulate = _normalColor;
	}
	
	public override void _GuiInput(InputEvent inputEvent)
	{
		if (inputEvent is InputEventMouseButton mouseButton && 
			mouseButton.ButtonIndex == MouseButton.Left && 
			mouseButton.Pressed)
		{
			GD.Print($"[CardTexture] Клик: {UpgradeId}");
			EmitSignal(SignalName.CardClicked, UpgradeId);
		}
	}
}
