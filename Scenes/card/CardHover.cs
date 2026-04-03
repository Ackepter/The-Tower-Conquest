using Godot;
using System.Collections.Generic;

public partial class CardHover : Control
{
	// 🔥 Сигнал, который летит в GameManagerв
	[Signal] public delegate void CardSelectedEventHandler(string upgradeId);

	[Export] public string UpgradeId = "Card"; // Задай в редакторе: "Ярость", "Блиц" или "Кровь"
	
	[Export] public float HoverLift = 25f;
	[Export] public float HoverSpeed = 0.2f;
	[Export] public float ClickSpeed = 1.2f;
	[Export] public Vector2 ManualCenterPosition = new Vector2(0, 0); 

	private Vector2 _basePosition;
	private float _baseRotation;
	private Tween _activeTween;
	private List<CardHover> _siblings = new List<CardHover>();
	private bool _isClicked = false;

	public override void _Ready()
	{
		MouseFilter = MouseFilterEnum.Stop;
		MouseEntered += OnMouseEntered;
		MouseExited += OnMouseExited;
		GuiInput += OnGuiInput;

		_basePosition = Position;
		_baseRotation = Rotation;

		var parent = GetParent();
		foreach (var child in parent.GetChildren())
		{
			if (child is CardHover card && child != this)
				_siblings.Add(card);
		}
	}

	private void OnGuiInput(InputEvent @event)
	{
		if (@event is InputEventMouseButton mouseEvent && 
			mouseEvent.Pressed && mouseEvent.ButtonIndex == MouseButton.Left)
		{
			OnCardClicked();
			AcceptEvent();
		}
	}

	private void OnCardClicked()
	{
		if (_isClicked) return;
		_isClicked = true;
		MouseFilter = MouseFilterEnum.Ignore;

		GD.Print($"[Card] Выбрана карта: {UpgradeId}");
		
		// 🔥 Отправляем сигнал в GameManager
		EmitSignal(SignalName.CardSelected, UpgradeId);

		PlaySelectedAnimation(ManualCenterPosition);
		foreach (var card in _siblings) card.PlayDiscardAnimation();
	}

	private void PlaySelectedAnimation(Vector2 targetPos)
	{
		if (_activeTween != null) { _activeTween.Kill(); _activeTween = null; }
		_activeTween = GetTree().CreateTween();

		_activeTween.TweenProperty(this, "position", targetPos, ClickSpeed)
					.SetEase(Tween.EaseType.Out).SetTrans(Tween.TransitionType.Cubic);
		_activeTween.Parallel().TweenProperty(this, "scale", Vector2.One * 1.2f, ClickSpeed)
					.SetEase(Tween.EaseType.Out).SetTrans(Tween.TransitionType.Cubic);
		_activeTween.Parallel().TweenProperty(this, "rotation", 0f, ClickSpeed)
					.SetEase(Tween.EaseType.Out).SetTrans(Tween.TransitionType.Cubic);
		
		_activeTween.TweenCallback(Callable.From(() => Rotation = 0f));
	}

	private void PlayDiscardAnimation()
	{
		if (_activeTween != null) { _activeTween.Kill(); _activeTween = null; }
		_activeTween = GetTree().CreateTween();

		_activeTween.TweenProperty(this, "position", new Vector2(1000, Position.Y), ClickSpeed)
					.SetEase(Tween.EaseType.In).SetTrans(Tween.TransitionType.Cubic);
		_activeTween.Parallel().TweenProperty(this, "rotation", Mathf.DegToRad(45), ClickSpeed);
		_activeTween.Parallel().TweenProperty(this, "modulate:a", 0.0f, ClickSpeed * 0.8f);
	}

	private void OnMouseEntered()
	{
		if (_isClicked) return;
		if (_activeTween != null) { _activeTween.Kill(); _activeTween = null; }
		MoveToFront();
		_activeTween = GetTree().CreateTween();
		_activeTween.TweenProperty(this, "position", _basePosition + new Vector2(0, -HoverLift), HoverSpeed)
					.SetEase(Tween.EaseType.Out);
	}

	private void OnMouseExited()
	{
		if (_isClicked) return;
		if (_activeTween != null) { _activeTween.Kill(); _activeTween = null; }
		_activeTween = GetTree().CreateTween();
		_activeTween.TweenProperty(this, "position", _basePosition, HoverSpeed)
					.SetEase(Tween.EaseType.Out);
		_activeTween.Parallel().TweenProperty(this, "rotation", _baseRotation, HoverSpeed)
					.SetEase(Tween.EaseType.Out);
	}
}
