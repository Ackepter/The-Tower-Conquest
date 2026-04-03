using System;
using System.Collections.Generic;
using Godot;
 
public partial class PoorHuman : BaseEnemyScript
{
	public bool IsDied {get; set;} = false;
    public override float Speed => 30f;

	public bool _isHurting  = false;
    private float _cooldownHurting = 0f;
    private const float _cooldownHurtingTime = 0.3f;

	private CollisionShape2D _collisionShape;
	private AnimationPlayer _attackPlayer;
    private Dictionary<String, Sprite2D>_swords = new Dictionary<String, Sprite2D>();
	private String _currentSword;
	private Tween _swordTween;

	private const float _attackCooldownTime = 1f;

	private Vector2 _swordBasePosition = Vector2.Zero;
	private float _swordBaseRotation = 0f;

	private float _directionMultiplier = 1f;


    private Area2D _attackHitbox;

	private Vector2 _leftAttackHitboxPosition  = new Vector2(-16,0);
    private Vector2 _rightAttackHitboxPosition = new Vector2(16,0);
    private Vector2 _upAttackHitboxPosition = new Vector2(0,-16);
    private Vector2 _downAttackHitboxPosition = new Vector2(0,16);

	private AudioStreamPlayer _audioPlayer;

    public override void _Ready()
    {
		base._Ready();
		
		_audioPlayer = GetNode<AudioStreamPlayer>("audioPlayer");

		_collisionShape = GetNode<CollisionShape2D>("CollisionShape2D");

        _attackHitbox = GetNode<Area2D>("AttackHitbox");
		_attackPlayer = GetNode<AnimationPlayer>("AttackPlayer");

        _swords["left"]= GetNode<Sprite2D>("SwordLeft");
		_swords["right"]= GetNode<Sprite2D>("SwordRight");
		_swords["up"]= GetNode<Sprite2D>("SwordUp");
		_swords["down"]= GetNode<Sprite2D>("SwordDown");

		_attackHitbox.BodyEntered += OnBodyEntered;
    }
    protected override void SetupEnemy()
    {
         
    }

	private bool _isChasing = false;
    public override void _PhysicsProcess(double delta)
    {
		if (!IsDied)
		{
			if (_cooldownHurting > 0)
			_cooldownHurting -= (float)delta;
        else if(_cooldownHurting <= 0) _isHurting = false;

        base._PhysicsProcess(delta);

		if (!_isHurting)
		{
			if (HasLineOfSight())
			{
				if (!_isAttacking)
				{
					if (distance < RecognizeDistance)
					{
						if (distance > 22)
						{
							if(!_audioPlayer.Playing && !_isChasing) 
							{
							_audioPlayer.Stream = GD.Load<AudioStream>("res://Scenes/Bases/BaseEnemy/sounds/takeSword.wav");
							_audioPlayer.Play();
							}
							_isChasing = true;
							ChaseHero();
						}
						else if (distance <= 22 && _canAttack && !_isAttacking)
						{
							Attack();
						}
					}
					else
					{
						Velocity = Velocity.MoveToward(Vector2.Zero, Speed * (float)delta);
						MoveAndSlide();
					}
				}
			}
			else
			{
				_sprite.Play("idle");
				_isChasing = false;
				foreach(Sprite2D i in _swords.Values)
				{
					i.Hide();
				}
			}
		}
		else
		{
			_sprite.Play("hurt");
		}
		}

    }

    protected override void ChaseHero()
    {
        if (_agent == null) return;
        Vector2 nextPathPosition = _agent.GetNextPathPosition();
        Vector2 relativeDirection = nextPathPosition - GlobalPosition;

		if (relativeDirection.Length() > 0) {
            relativeDirection = relativeDirection.Normalized() * Speed;
            float x = relativeDirection.X;
            float y = relativeDirection.Y;

			if(Math.Abs(x) > Math.Abs(y))
            {   
                if(x > 0)
                {
					foreach(var i in _swords.Keys)
					{
						if(i.CompareTo("right") == 0) _swords[i].Show();
						else _swords[i].Hide();
					}

					_currentSword = "right";
                    _sprite.Play("walkToRight");
                    _attackHitbox.Position = _rightAttackHitboxPosition;
                }
                else
                {
					foreach(var i in _swords.Keys)
					{
						if(i.CompareTo("left") == 0) _swords[i].Show();
						else _swords[i].Hide();
					}

					_currentSword = "left";
                    _sprite.Play("walkToLeft");
                    _attackHitbox.Position = _leftAttackHitboxPosition;
                }
            }
            else
            {
                if(y < 0)
                {
					foreach(var i in _swords.Keys)
					{
						if(i.CompareTo("up") == 0) _swords[i].Show();
						else _swords[i].Hide();
					}
					
					_currentSword = "up";
                    _sprite.Play("walkFromMe");
                    _attackHitbox.Position = _upAttackHitboxPosition;
                }
                else
                {
					foreach(var i in _swords.Keys)
					{
						if(i.CompareTo("down") == 0) _swords[i].Show();
						else _swords[i].Hide();
					}

					_currentSword = "down";
                    _sprite.Play("walkToMe");
                    _attackHitbox.Position = _downAttackHitboxPosition;
                }
            }
		}

		Velocity = relativeDirection.Normalized() * Speed;
        MoveAndSlide();

    }

    public async void PerformAttack() 
	{
		_isAttacking = true;

		_sprite.Play("idle");
		_attackPlayer.Play(_currentSword); 

		_audioPlayer.Stream = GD.Load<AudioStream>("res://Scenes/Bases/BaseEnemy/sounds/sword.mp3");
		_audioPlayer.Play();

		var Timer = GetTree().CreateTimer(_attackCooldownTime + 0.2);
		Timer.Timeout += () => {
			if (IsInstanceValid(this)) {
				_isAttacking = false;
			}
		};
	}
	
	public void OnBodyEntered(Node2D body) {		
		if (body is MainCharacter && _isAttacking) {
			MainCharacter hero = body as MainCharacter;
			
			if (hero != null) {
                hero.GetDamage(5);
				hero._animatedSprite.Play("hurt");
			}
			else
			{
				GD.Print("Hero is not finded");
			}
		}
	}

    protected override void Attack()
    {
		if (!_isAttacking) 
		{
			PerformAttack();
		}
    }

    public override void TakeDamage(int value)
    {
		_isHurting = true;
        _cooldownHurting = _cooldownHurtingTime;
        base.TakeDamage(value);
    }

	public override void Die()
    {
        IsDied = true;
		_sprite.Play("die");
		foreach(Sprite2D i in _swords.Values)
		{
			i.Hide();
		}
		var Timer = GetTree().CreateTimer(2f);
		Timer.Timeout += () => {
			if (IsInstanceValid(this)) {
				_sprite.Play("corp");
				
				_collisionShape.Disabled = true;
			}
		};
    }

}