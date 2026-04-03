using Godot;
using System;

public partial class ShieldBar : TextureProgressBar
{
  private MainCharacter _hero;
  
  public override void _Ready() {
		_hero = GetParent<Camera2D>().GetParent<MainCharacter>();
		
		if (_hero != null) {
			Value = _hero.GetCurrentShield;
		}
	}
	
	public override void _Process(double delta) {
		if (_hero != null) {
			Value = _hero.GetCurrentShield;
		}
	}
}
