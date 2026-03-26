using Godot;
using System;

public partial class TransitionManager : Node
{
	// Поля для сохранения прогресса
	private Vector2 _nextSpawnPosition = Vector2.Zero;
	private bool _hasNextSpawn = false;
	private int _savedHp = 100;
	private float _savedXp = 0f;
	private int _savedLevel = 1;
	private float _savedMaxXp = 100f;
	
	// 🔧 ИСПРАВЛЕНО для Godot 4.x
	public static TransitionManager Instance
	{
		get
		{
			var tree = Engine.GetMainLoop() as SceneTree;
			return tree.Root.GetNode<TransitionManager>("TransitionManager");
		}
	}
	
	public static void SetNextSpawn(Vector2 position)
	{
		Instance._nextSpawnPosition = position;
		Instance._hasNextSpawn = true;
	}
	
	public static Vector2 GetNextSpawn()
	{
		if (Instance._hasNextSpawn)
		{
			Vector2 pos = Instance._nextSpawnPosition;
			Instance._hasNextSpawn = false;
			return pos;
		}
		return Vector2.Zero;
	}
	
	public static void SavePlayerProgress(int hp, float xp, int level, float maxXp)
	{
		Instance._savedHp = hp;
		Instance._savedXp = xp;
		Instance._savedLevel = level;
		Instance._savedMaxXp = maxXp;
		GD.Print($"💾 Прогресс сохранен: HP={hp}, XP={xp}, Lvl={level}");
	}

	public static (int, float, int, float) GetSavedProgress()
	{
		return (Instance._savedHp, Instance._savedXp, Instance._savedLevel, Instance._savedMaxXp);
	}
}
