using Godot;

public partial class ArcherEnemy : CharacterBody2D
{
	private const float Speed = 20f;
	public const float AttackCooldown = 1.5f;
	public const int AttackDistance = 80;


	private AnimatedSprite2D Sprite;
	private Timer attackTimer;
	private RayCast2D visionRay;
	private NavigationAgent2D agent;
	private bool canAttack = true;
	private bool isAttacking = false;


	private CharacterBody2D hero;
	private NavigationAgent2D navAgent;
	public override void _Ready()
	{
		hero = GetNode<CharacterBody2D>("../FirstMap/MainCharacter");

		Sprite = GetNode<AnimatedSprite2D>("AnimatedSprite");

		visionRay = GetNode<RayCast2D>("VisionRay"); 

		agent = GetNode<NavigationAgent2D>("NavigationAgent2D");

		attackTimer = new Timer();
		attackTimer.OneShot = true;
		attackTimer.WaitTime = AttackCooldown;
		attackTimer.Timeout += OnAttackCooldownEnd;
		AddChild(attackTimer);
	}

	public override void _PhysicsProcess(double delta)
	{  
		float distance = Position.DistanceTo(hero.Position);

		agent.TargetPosition = hero.GlobalPosition;

		if (!agent.IsNavigationFinished())
		{
			Sprite.Play("stay");
		}

		if (visionRay != null)
		{
			Vector2 directionToHero = (hero.Position - Position);
			visionRay.TargetPosition = directionToHero.Normalized() * AttackDistance;
		}

		if(distance < 200 && !agent.IsNavigationFinished())
		{
			if(isAttacking) { return; }

			if(distance > AttackDistance - 10 && distance < AttackDistance + 10 && canAttack)
			{
				if(HasLineOfSight())
					Attack();
				else{Sprite.Play("stay");}
			}
			else if(distance > AttackDistance )
			{
				ChaseHero();
				MoveAndSlide();
			}
			else if(distance < AttackDistance)
			{
				EscapeHero();
				MoveAndSlide();
			}
		}
		else
		{
			Sprite.Play("stay");
		}
	}

	private bool HasLineOfSight()
	{
		if (visionRay == null) return true; // Если луча нет, считаем что видно

		// Если луч во что-то врезался (в стену), значит видимости нет
		// Важно: в настройках RayCast2D нужно убрать Hero и Enemy из Mask, 
		// чтобы он реагировал только на стены/препятствия
		return !visionRay.IsColliding();
	}

	private void Attack()
	{
		isAttacking = true;
		canAttack = false;
		
		Sprite.Play("attack");
		attackTimer.Start();
	}
	private void ChaseHero()
	{
		Vector2 nextPathPosition = agent.GetNextPathPosition();
		Vector2 direction = (nextPathPosition - GlobalPosition).Normalized();

		Sprite.Play("walk");
		flipSprite(Sprite);

		Velocity = direction * Speed;
	}
	private void EscapeHero()
	{
		Vector2 nextPathPosition = agent.GetNextPathPosition();
		Vector2 direction = -(nextPathPosition - GlobalPosition).Normalized();
		Sprite.Play("walk");
		flipSprite(Sprite);

		Velocity = direction * Speed;
	}
	private void OnAttackCooldownEnd()
	{
		canAttack = true;
		isAttacking = false;
	}
	  private void flipSprite(Sprite2D sprite)
	{
		if (hero.Position.X < Position.X) sprite.FlipH = true;
		else sprite.FlipH = false;
	}
	private void flipSprite(AnimatedSprite2D sprite)
	{
		if (hero.Position.X < Position.X) sprite.FlipH = true;
		else sprite.FlipH = false;
	}
}
