using Godot;
using System;

public partial class HpBarScripts : ProgressBar
{ 
    private MainCharacter _hero;
	private StyleBoxFlat sbFill;
	private StyleBoxFlat sbBackground; 
	
	public override void _Ready() {
		_hero = GetParent<Camera2D>().GetParent<MainCharacter>();
		HealthBarTheme();
		Size = new Vector2(60, 2);
		
		SizeFlagsHorizontal = 0;
		SizeFlagsVertical = 0;
		
		if (_hero != null) {
			Value = _hero.GetCurrentHp;
		}
	}
	
	public override void _Process(double delta) {
		if (_hero != null) {
			Value = _hero.GetCurrentHp;
		}
	}
	
	private void HealthBarTheme() {
		sbFill = new StyleBoxFlat();
		sbBackground = new StyleBoxFlat();
		
		sbFill.BgColor = new Color(1.0f, 0.0f, 0.0f, 1.0f);
		sbBackground.BgColor = new Color(0.2f, 0.2f, 0.2f, 1f);
		
		sbBackground.CornerRadiusTopLeft = 0;
		sbBackground.CornerRadiusTopRight = 0;
		sbBackground.CornerRadiusBottomLeft = 0;
		sbBackground.CornerRadiusBottomRight = 0;
		
		sbFill.CornerRadiusTopLeft = 0;
		sbFill.CornerRadiusTopRight = 0;
		sbFill.CornerRadiusBottomLeft = 0;
		sbFill.CornerRadiusBottomRight = 0;
		
		sbFill.SetContentMarginAll(0);
		sbBackground.SetContentMarginAll(0);
		
		AddThemeStyleboxOverride("fill", sbFill);
		AddThemeStyleboxOverride("background", sbBackground);
	}
}
