using System.Collections;
using Godot;

public partial class HealPoisonBase : Sprite2D
{
    private Area2D _hitBox;
    private bool isHealing;

    protected virtual int HealAmount{get;} = 100;

    public override void _Ready()
    {
        _hitBox = GetNode<Area2D>("Area2D");

        _hitBox.BodyEntered += OnBodyEntered;
        _hitBox.Monitoring = true;
    }

    public virtual void OnBodyEntered(Node2D body)
    {
        if (body is MainCharacter) {
			MainCharacter hero = body as MainCharacter;
			
			if (hero != null) {
                if(hero.GetCurrentHp != hero.GetMaxHp && !isHealing)
                {
                    isHealing = true;
                    hero.GetHeal(HealAmount);
                    QueueFree();
                }
			}
		}
    }
}
