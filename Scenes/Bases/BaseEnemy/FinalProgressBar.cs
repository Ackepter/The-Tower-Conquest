using Godot;

public partial class FinalProgressBar : BaseProgressBarScript
{
	private BaseEnemyScript _enemy;

	public override void _Ready() 
	{
		base._Ready();
		ShowPercentage = false;
		
		_enemy = GetNode<BaseEnemyScript>("../../");
		if (_enemy == null)
		{
			return;
		}

		if (_enemy != null)
		{
			MaxValue = _enemy.GetMaxHp();
			Value = _enemy.GetCurrentHp();
		}
	}
	
	public override void _Process(double delta)
	{
		if (_enemy != null)
		{
			Value = _enemy.GetCurrentHp();
		}
	}
}
