using Godot;
 
public partial class PoorHuman : BaseEnemyScript
{
    public override float Speed => 30f;


    public override void _Ready()
    {
        GD.Print("ready child");
        base._Ready();
        
    }
    protected override void SetupEnemy()
    {
         
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
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
        UpdateAnimation();
    }

    protected override void Attack()
    {
        base.Attack();
        GD.Print("Attack");
    }
}