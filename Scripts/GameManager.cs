using Godot;

public partial class GameManager : Node
{
	public static GameManager Instance { get; private set; }
	
	// Статы игрока
	public int PlayerDamage { get; set; } = 10;
	public float PlayerSpeed { get; set; } = 200f;
	public int PlayerMaxHealth { get; set; } = 100;
	public int PlayerCurrentHealth { get; set; } = 100;
	
	public override void _Ready()
	{
		Instance = this;
		GD.Print("[GameManager] Инициализирован");
	}
	
	public void ApplyUpgrade(string upgradeId)
	{
		GD.Print($"[GameManager] Применяю улучшение: {upgradeId}");
		
		switch (upgradeId)
		{
			case "damage_up":
				PlayerDamage += 5;
				break;
			case "speed_up":
				PlayerSpeed += 50f;
				break;
			case "health_up":
				PlayerMaxHealth += 20;
				PlayerCurrentHealth += 20;
				break;
		}
	}
	
	public void SaveHealth(int currentHealth, int maxHealth)
	{
		PlayerCurrentHealth = currentHealth;
		PlayerMaxHealth = maxHealth;
		GD.Print($"[GameManager] Здоровье сохранено: {currentHealth}/{maxHealth}");
	}
	
	public int GetCurrentHealth() => PlayerCurrentHealth;
	public int GetMaxHealth() => PlayerMaxHealth;
	
	public void ResetStats()
	{
		PlayerDamage = 10;
		PlayerSpeed = 200f;
		PlayerMaxHealth = 100;
		PlayerCurrentHealth = 100;
		GD.Print("[GameManager] Статы сброшены");
	}
}
