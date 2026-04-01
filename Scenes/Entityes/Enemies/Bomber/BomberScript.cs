using Godot;
using System;

public partial class BomberScript : BaseEnemyScript
{
    public override float Speed => 20f;


    private Sprite2D[] _bombs = new Sprite2D[2];
	private Sprite2D _bomb;
	private Tween _bombTween;

	// Параметры анимации меча (в радианах и пикселях)
	private const float WindUpAngle = -0.5f;      // Занесение назад (против часовой)
	private const float SlashAngle = 1.2f;        // Удар вперёд
	private const float WindUpOffset = -15f;      // Смещение назад при занесении
	private const float SlashOffset = 20f;        // Выдвижение вперёд при ударе

	private float _cooldownAttack = 0f;
	private const float _attackCooldownTime = 0.5f;
    private float _directionMultiplier = 1f;

    private Area2D _attackHitbox;
    private CollisionShape2D _attackHitboxCollison;

    public override void _Ready()
    {
        _attackHitbox = GetNode<Area2D>("AttackHitbox");
		_attackHitboxCollison = GetNode<CollisionShape2D>("AttackHitbox/CollisionShape2D");
        _bombs[0]= GetNode<Sprite2D>("SwordLeft");
		_bombs[1]= GetNode<Sprite2D>("SwordRight");
		_bomb = _bombs[1];
		_bomb.Show();


		_attackHitbox.BodyEntered += OnBodyEntered;
        base._Ready();
        
    }
    protected override void SetupEnemy()
    {
         
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);

        if (_cooldownAttack > 0)
			_cooldownAttack -= (float)delta;

		if (!_isAttacking)
		{
			if (distance < RecognizeDistance && HasLineOfSight())
        {
            if (distance > 20)
            {
                ChaseHero();
            }
            else if (distance <= 20 && _canAttack && !_isAttacking)
            {
                Attack();
            }
        }
        else
        {
            Velocity = Velocity.MoveToward(Vector2.Zero, Speed * (float)delta);
            MoveAndSlide();
        }
        	//UpdateAnimation();
		}
    }

    public async void PerformAttack() 
	{
		_isAttacking = true;
		
		_attackHitboxCollison.Disabled = true;
		_isAttacking = false;
}
	
	public void OnBodyEntered(Node2D body) {		
		if (body is MainCharacter && _isAttacking) {
			MainCharacter hero = body as MainCharacter;
			
			if (hero != null) {
                hero.GetDamage(10);
			}
			else
			{
				GD.Print("Hero is not finded");
			}
		}
	}

    //  protected override void UpdateAnimation()
    // {
    //     if (_sprite == null) return;

    //     if (Velocity.Length() > 10)
    //         _sprite.Play("walk");
    //     else if (!_isAttacking)
    //         _sprite.Play("idle");
            
    //     FlipSprite(Velocity);
    // }

	// protected override void FlipSprite(Vector2 velocity)
	// {
	// 	bool shouldFaceLeft = velocity.X < 0;
		
	// 	if (shouldFaceLeft)
	// 	{
	// 		_bomb.Hide();
	// 		_bomb = _bombs[0];
	// 		_bomb.Show();
	// 		_sprite.FlipH = true;
	// 		_directionMultiplier = -1f;
	// 	}
	// 	else
	// 	{
	// 		_bomb.Hide();
	// 		_bomb = _bombs[1];
	// 		_bomb.Show();
	// 		_sprite.FlipH = false;
	// 		_directionMultiplier = 1f;
	// 	}
	// }

	private void ResetSwordTransform()
	{
		_isAttacking = false;
		if (_bombTween != null && _bombTween.IsRunning())
		{
			_bombTween.Kill();
		}

		_attackHitboxCollison.Disabled = true;
	}

    protected override void Attack()
    {
        base.Attack();

        PerformAttack();
    }
}
