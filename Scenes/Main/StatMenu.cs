using Godot;
using System;

public partial class StatMenu : CanvasLayer
{
	[Export] public MainCharacter Player;
	
	private Label _hpLabel;
	private Label _powerLabel;
	private Label _attackSpeedLabel;
	private Label _agilityLabel;
	private FontFile _pixelFont;
	
	public override void _Ready()
	{
		_hpLabel = GetNode<Label>("Panel/VBoxContainer/health");
		_powerLabel = GetNode<Label>("Panel/VBoxContainer/power");
		_attackSpeedLabel = GetNode<Label>("Panel/VBoxContainer/attackSpeed");
		_agilityLabel = GetNode<Label>("Panel/VBoxContainer/agility");
		
		_pixelFont = GD.Load<FontFile>("res://fonts/vcrosdmonorusbyd.ttf");
		
		Visible = false;
		
		SetupAnchors();
		SetupStyle();
	}
	
	public override void _Process(double delta)
	{
		if (Visible && Player != null)
		{
			UpdateStats();
		}
	}
	
	private void UpdateStats()
	{
		_hpLabel.Text = $"stamina: 1";
		_powerLabel.Text = $"power: 1";
		_attackSpeedLabel.Text = $"attack speed: 1";
		_agilityLabel.Text = $"agility: 1";
	}
	
	public void Toggle()
	{
		Visible = !Visible;
		
		if (Visible)
		{
			UpdateStats();
		}
	}
	
	public override void _Input(InputEvent @event)
	{
		if (@event.IsActionPressed("ui_focus_next") ||
			(@event is InputEventKey key && 
		 	key.Pressed && 
		 	key.Keycode == Key.Tab))
		{
			Toggle();
			GetViewport().SetInputAsHandled();
		}
	}
	
	private void SetupStyle()
	{
		var panel = GetNode<Panel>("Panel");
		
		var texture = GD.Load<Texture2D>("res://Textures/UI/panel_background.png");;
		
		StyleBoxTexture sb = new StyleBoxTexture();
		sb.Texture = texture;
		
		sb.ContentMarginLeft = 10;
		sb.ContentMarginTop = 10;
		sb.ContentMarginRight = 10;
		sb.ContentMarginBottom = 10;
		
		panel.AddThemeStyleboxOverride("panel", sb);
		
		if (_pixelFont != null)
		{
			_hpLabel.AddThemeFontOverride("font", _pixelFont);
			_hpLabel.AddThemeFontSizeOverride("font_size", 8);
			_hpLabel.AddThemeColorOverride("font_color", Colors.White);
			
			_powerLabel.AddThemeFontOverride("font", _pixelFont);
			_powerLabel.AddThemeFontSizeOverride("font_size", 8);
			_powerLabel.AddThemeColorOverride("font_color", Colors.White);
			
			_attackSpeedLabel.AddThemeFontOverride("font", _pixelFont);
			_attackSpeedLabel.AddThemeFontSizeOverride("font_size", 8);
			_attackSpeedLabel.AddThemeColorOverride("font_color", Colors.White);
			
			_agilityLabel.AddThemeFontOverride("font", _pixelFont);
			_agilityLabel.AddThemeFontSizeOverride("font_size", 8);
			_agilityLabel.AddThemeColorOverride("font_color", Colors.White);
		}
		
		var vbox = GetNodeOrNull<VBoxContainer>("Panel/VBoxContainer");
	
		if (vbox != null)
		{
			vbox.Position = new Vector2(23, 20);
			
			vbox.AddThemeConstantOverride("separation", 10);
		}
	}
	
	private void SetupAnchors()
	{
		var panel = GetNode<Panel>("Panel");
		
		panel.SetAnchorsPreset(Control.LayoutPreset.Center);
		
		panel.Size = new Vector2(50, 100);
		
		panel.OffsetLeft = -70;   // -половина ширины
		panel.OffsetTop = -70;    // -половина высоты
		panel.OffsetRight = 70;   // +половина ширины
		panel.OffsetBottom = 70;
	}
}
