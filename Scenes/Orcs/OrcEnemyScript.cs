using Godot;

public partial class OrcEnemyScript : CharacterBody2D
{
    private const float Speed = 15f;
    public const float AttackCooldown = 1f;


    private CharacterBody2D hero;
    private AnimatedSprite2D Sprite;
    private Timer attackTimer;
    private bool canAttack = true;
    private bool isAttacking = false;
    public override void _Ready()
    {
        hero = GetNode<CharacterBody2D>("../MainHero");

        Sprite = GetNode<AnimatedSprite2D>("AnimatedSprite");

        attackTimer = new Timer();
        attackTimer.OneShot = true;
        attackTimer.WaitTime = AttackCooldown;
        attackTimer.Timeout += OnAttackCooldownEnd;
        AddChild(attackTimer);
    }

    public override void _PhysicsProcess(double delta)
    {  
        float distance = Position.DistanceTo(hero.Position);

        if(distance < 150)
        {
            if(isAttacking) { return; }

            if(distance < 20 && canAttack)
            {
                Attack();
            }
            else
            {
                ChaseHero();
                MoveAndSlide();
            }
        }
        else
        {
            Sprite.Play("stay");
        }
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
        Vector2 direction = (hero.Position - Position).Normalized();
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
