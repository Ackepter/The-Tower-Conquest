using Godot;

public partial class BaseProgressBarScript : Godot.ProgressBar
{ 
	//enemy короче в дочернем классе будет ок
	protected StyleBoxFlat sbFill{get;set;}
	protected StyleBoxFlat sbBackground{get;set;}
 
	public override void _Ready()
	{ 
		HealthBarTheme();
		ShowPercentage = false;
		Size = new Vector2(20, 2);
		
		SizeFlagsHorizontal = 0;
		SizeFlagsVertical = 0;
	}

	protected void HealthBarTheme() {
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
