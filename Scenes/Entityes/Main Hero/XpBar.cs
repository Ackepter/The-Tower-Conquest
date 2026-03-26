using Godot;
using System;

public partial class XpBar: ProgressBar
{
	private MainCharacter _hero;
	private StyleBoxFlat sbFill;
	private StyleBoxFlat sbBackground; 
	
	public override void _Ready() {
		_hero = GetParent<Camera2D>().GetParent<MainCharacter>();
		XPBarTheme();
		
		// Узкая полоска: ширина 30, высота 1
		CustomMinimumSize = new Vector2(60, 3);
		Size = new Vector2(60, 3);
		
		SizeFlagsHorizontal = 0;
		SizeFlagsVertical = 0;
		
		ShowPercentage = false;
		
		// 🔧 Устанавливаем якоря в ЛЕВЫЙ ВЕРХНИЙ УГОЛ
		SetAnchor(Side.Left, 0.0f);
		SetAnchor(Side.Top, 0.0f);
		SetAnchor(Side.Right, 0.0f);
		SetAnchor(Side.Bottom, 0.0f);
		
		// 🔧 Позиция: под HP bar (HP bar обычно на Y=10, ставим на Y=40)
		Position = new Vector2(-128, -90);
		
		if (_hero != null) {
			MaxValue = _hero.MaxXP;
			Value = _hero.CurrentXP;
		}
	}
	
	public override void _Process(double delta) {
		if (_hero != null) {
			// 🔧 Убрали следование за персонажем
			// GlobalPosition больше не меняем
			
			MaxValue = _hero.MaxXP;
			Value = _hero.CurrentXP;
		}
	}
	
	private void XPBarTheme() {
		sbFill = new StyleBoxFlat();
		sbBackground = new StyleBoxFlat();
		
		// Синий цвет для XP
		sbFill.BgColor = new Color(0.0f, 0.5f, 1.0f, 1.0f);
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
