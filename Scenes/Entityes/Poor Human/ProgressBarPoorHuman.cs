using Godot;
 
public partial class ProgressBarPoorHuman : ProgressBarScript
{ 
    private BaseEnemyScript _enemy;

    public override void _Ready() 
    {
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
}