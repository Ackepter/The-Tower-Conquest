using Godot;
using System;

public partial class ShieldBar : TextureProgressBar
{
	private MainCharacter _hero;
	
	public override async void _Ready() 
	{
		await ToSignal(GetTree(), "process_frame");
		
		_hero = GetTree().GetFirstNodeInGroup("player") as MainCharacter;
		
		MinValue = 0;
		MaxValue = 100;
		
		if (_hero != null) {
			Value = _hero.GetCurrentShield;
		} else {
			Value = MaxValue;
		}
	}
	
	public override void _Process(double delta) 
	{
		if (_hero != null) {
			Value = _hero.GetCurrentShield;
		}
	}
}
