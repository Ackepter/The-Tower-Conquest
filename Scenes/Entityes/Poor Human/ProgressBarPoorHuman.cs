using Godot;
 
public partial class ProgressBarPoorHuman : ProgressBarScript
{
    private BaseEnemyScript _enemy;

    public override void _Ready() 
    {
        GD.Print("ProgressBar: Ready child");
		base._Ready();
        ShowPercentage = false;
        

        _enemy = GetNode<BaseEnemyScript>("../../");
        if (_enemy == null)
        {
            GD.PrintErr("ProgressBar: Could not find Enemy parent!");
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
            // Плавно обновляем значение (опционально)
            Value = _enemy.GetCurrentHp();
            
            if (_enemy.GetCurrentHp() <= 0)
            {
                // Опционально: скрыть бар когда враг умер
                // Visible = false; 
            }
        }
    }
}