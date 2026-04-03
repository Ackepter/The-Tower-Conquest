using Godot;

public partial class LevelTransition : Area2D
{
	[Export] public string NextLevelPath { get; set; } = "res://node_2d.tscn";
	[Export] public Vector2 SpawnPosition { get; set; } = new Vector2(510, 588);
	[Export] public string CardSelectionScenePath { get; set; } = "res://Scenes/card/card.tscn";
	
	[Export] public float DelayAfterSelection { get; set; } = 1.0f;
	
	private bool _hasTriggered = false;
	private bool _canSelectCard = false;
	
	public override void _Ready()
	{
		BodyEntered += OnBodyEntered;
	}
	
	private void OnBodyEntered(Node2D body)
	{
		if (_hasTriggered) return;
		
		if (body.IsInGroup("player") || body.Name == "MainCharacter" || body.Name == "Player")
		{
			_hasTriggered = true;
			GD.Print("[LevelTransition] Игрок вошел!");
			
			if (GlobalData.Instance != null)
				GlobalData.Instance.SpawnPosition = SpawnPosition;
			
			LoadCardSelection();
		}
	}
	
	private async void LoadCardSelection()
	{
		GD.Print("[LevelTransition] Загружаю карты...");
		
		if (!ResourceLoader.Exists(CardSelectionScenePath))
		{
			GD.PrintErr($"[LevelTransition] Файл не найден: {CardSelectionScenePath}");
			GetTree().ChangeSceneToFile(NextLevelPath);
			return;
		}
		
		PackedScene cardScene = GD.Load<PackedScene>(CardSelectionScenePath);
		Node cardInstance = cardScene.Instantiate();
		cardInstance.Name = "CardSelectionOverlay";
		
		GetTree().Root.AddChild(cardInstance);
		
		if (cardInstance is Control control)
		{
			control.SetProcessInput(true);
			control.FocusMode = Control.FocusModeEnum.All;
		}
		
		await GetTree().ToSignal(GetTree(), "process_frame");
		
		ConnectToCardTextures(cardInstance);
		
		 // 🔥 СТАВИМ ПАУЗУ — здоровье не меняется!
		GetTree().Paused = true;
		GD.Print("[LevelTransition] ⏸️ Игра на паузе");
		
		GD.Print("[LevelTransition] Ждём 2с...");
		await GetTree().ToSignal(GetTree().CreateTimer(2.0f), "timeout");
		
		_canSelectCard = true;
		GD.Print("[LevelTransition] Карты активны!");
	}
	
	private void ConnectToCardTextures(Node cardInstance)
	{
		GD.Print("[LevelTransition] 🔍 Ищу карты...");
		
		// 🔥 Способ 1: ищем по имени узла (card1, card2, card3)
		var cardsByName = new Node[] {
			cardInstance.GetNodeOrNull("Panel/card1"),
			cardInstance.GetNodeOrNull("Panel2/card2"), 
			cardInstance.GetNodeOrNull("Panel3/card3"),
			// Альтернативные пути:
			cardInstance.GetNodeOrNull("CanvasLayer/Panel/card1"),
			cardInstance.GetNodeOrNull("CanvasLayer/Panel2/card2"),
			cardInstance.GetNodeOrNull("CanvasLayer/Panel3/card3"),
		};
		
		int connected = 0;
		
		foreach (var card in cardsByName)
		{
			if (card != null && card is CardTexture texture)
			{
				texture.Connect("CardClicked", Callable.From((string id) => OnCardSelected(id)));
				GD.Print($"[LevelTransition] ✅ Подключился к {card.Name}: {texture.UpgradeId}");
				connected++;
			}
		}
		
		// 🔥 Способ 2: если не нашли по имени — ищем все TextureRect с нужным скриптом
		if (connected == 0)
		{
			GD.Print("[LevelTransition] Пробую поиск по всем узлам...");
			var allNodes = cardInstance.FindChildren("*", "TextureRect", true, false);
			
			foreach (var node in allNodes)
			{
				if (node is CardTexture texture)
				{
					texture.Connect("CardClicked", Callable.From((string id) => OnCardSelected(id)));
					GD.Print($"[LevelTransition] ✅ Подключился к {node.Name}: {texture.UpgradeId}");
					connected++;
				}
			}
		}
		
		if (connected == 0)
		{
			GD.PrintErr("[LevelTransition] ❌ Не удалось подключиться ни к одной карте!");
			GD.PrintErr("[LevelTransition] Проверь:");
			GD.PrintErr("  1. Имена узлов: card1, card2, card3");
			GD.PrintErr("  2. Скрипт CardTexture.cs прикреплён к каждому TextureRect");
			GD.PrintErr("  3. Структура: Panel/card1, Panel2/card2, Panel3/card3");
		}
		else
		{
			GD.Print($"[LevelTransition] ✅ Подключено карт: {connected}");
		}
	}
	
	private async void OnCardSelected(string upgradeId)
	{
		if (!_canSelectCard) return;
		_canSelectCard = false;
		
		GD.Print($"[LevelTransition] 🎯 Выбрано: {upgradeId}");
		
		// 🔥 1. Применяем через GameManager (для статистики)
		if (GameManager.Instance != null)
			GameManager.Instance.ApplyUpgrade(upgradeId);
		
		// 🔥 2. 🔥 НОВОЕ: Применяем напрямую к игроку!
		var player = GetTree().GetFirstNodeInGroup("player");
		if (player is MainCharacter character)
		{
			character.ApplyUpgrade(upgradeId);
			GD.Print("[LevelTransition] ✅ Улучшение применено к игроку");
		}
		// Альтернатива: если игрок не в группе "player", ищем по имени
		else
		{
			var playerByName = GetTree().Root.GetNodeOrNull("MainCharacter");
			if (playerByName is MainCharacter character2)
			{
				character2.ApplyUpgrade(upgradeId);
				GD.Print("[LevelTransition] ✅ Улучшение применено к игроку (по имени)");
			}
		}
		
		GetTree().Paused = false;
		
		var cardOverlay = GetTree().Root.GetNodeOrNull("CardSelectionOverlay");
		cardOverlay?.QueueFree();
		
		// 🔥 ПРОВЕРКА пути
		GD.Print($"[LevelTransition] Путь к уровню: {NextLevelPath}");
		if (!ResourceLoader.Exists(NextLevelPath))
		{
			GD.PrintErr($"[LevelTransition] ❌ Файл не найден: {NextLevelPath}");
			return;
		}
		
		await GetTree().ToSignal(GetTree().CreateTimer(DelayAfterSelection), "timeout");
		
		GD.Print($"[LevelTransition] 🚀 Загружаю: {NextLevelPath}");
		GetTree().ChangeSceneToFile(NextLevelPath);
	}
}
