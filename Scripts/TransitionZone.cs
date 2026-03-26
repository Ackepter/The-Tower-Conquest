using Godot;
using System;

public partial class TransitionZone : Area2D
{
	[Export] public string TargetScene { get; set; } = "";
	[Export] public Vector2 SpawnPosition { get; set; } = new Vector2(100, 100);
	
	private bool _canTransition = true;
	
	public override void _Ready()
	{
		BodyEntered += OnBodyEntered;
	}
	
	private void OnBodyEntered(Node2D body)
	{
		if (!_canTransition) return;
		
		if (body.IsInGroup("Player") || body.Name == "MainCharacter")
		{
			GD.Print($"🚪 Переход на: {TargetScene}");
			GD.Print($"📍 Спавн: {SpawnPosition}");
			TransitionToNextLevel();
		}
	}
	
	private async void TransitionToNextLevel()
	{
		_canTransition = false;
		
		// 🔧 1. СОХРАНЯЕМ ПРОГРЕСС (ПЕРЕД сменой сцены!)
		var player = GetTree().GetFirstNodeInGroup("Player") as MainCharacter;
		if (player != null)
		{
			GD.Print($"💾 Сохраняем: HP={player.GetCurrentHp}, XP={player.CurrentXP}, Lvl={player.Level}");
			
			TransitionManager.SavePlayerProgress(
				player.GetCurrentHp,
				player.CurrentXP,
				player.Level,
				player.MaxXP
			);
		}
		
		// 🔧 2. Сохраняем позицию спавна
		TransitionManager.SetNextSpawn(SpawnPosition);
		
		// 🔧 3. Меняем сцену (ПОСЛЕ сохранения!)
		GetTree().ChangeSceneToFile(TargetScene);
		
		// Кулдаун (опционально)
		await ToSignal(GetTree().CreateTimer(1.0f), "timeout");
		_canTransition = true;
	}
}
