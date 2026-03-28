using Godot;
using System;

public partial class MainCharacter : CharacterBody2D
{
	public const float Speed = 100f; 
    public bool _isHurting  = false;
    private float _cooldownHurting = 0f;
    private const float _cooldownHurtingTime = 0.4f;
    private const int _maxHp = 100;

    private int _currentHp = _maxHp;
    public int GetCurrentHp => _currentHp;
    public int GetMaxHp => _maxHp;
    
    private AnimationPlayer _attackPlayer;
    public AnimatedSprite2D _animatedSprite;
    private Area2D _attackHitbox;
    private bool _isAttacking = false;
    private const float _attackCooldownTime = 0.4f;
    
    private AnimatedSprite2D[] _arms = new AnimatedSprite2D[2];
    
    public Node2D CurrentEnemy { get; set; }

    private Vector2 _leftAttackHitboxPosition  = new Vector2(-16,0);
    private Vector2 _rightAttackHitboxPosition = new Vector2(16,0);
    private Vector2 _upAttackHitboxPosition = new Vector2(0,-16);
    private Vector2 _downAttackHitboxPosition = new Vector2(0,16);
    private float _directionMultiplier = 1f;


    private AudioStreamPlayer _audioPlayer;
	public override void _Ready() 
	{
        _audioPlayer = GetNode<AudioStreamPlayer>("audioPlayer");

		_animatedSprite = GetNode<AnimatedSprite2D>("Player");	
        _attackPlayer = GetNode< AnimationPlayer>("AttackPlayer");	

		_arms[0]= GetNode<AnimatedSprite2D>("LeftArm");
		_arms[1]= GetNode<AnimatedSprite2D>("RightArm");

        _attackHitbox = GetNode<Area2D>("AttackHitbox");
        _attackHitbox.BodyEntered += OnBodyEntered;
        
	}
	public void OnBodyEntered(Node2D body) {
		if (body.IsInGroup("enemy") && _isAttacking) {
			BaseEnemyScript enemy = body as BaseEnemyScript;
			if (enemy != null) {
				enemy.TakeDamage(25);
                _isAttacking = false;
			}
		}
	}
	public override void _PhysicsProcess(double delta)
	{

        if (_cooldownHurting > 0)
			_cooldownHurting -= (float)delta;
        else if(_cooldownHurting <= 0) _isHurting = false;

		if (!_isAttacking && !_isHurting)
		{
			
			HandleMovement();

            HandleAttack();

			MoveAndSlide();
		}
		
	}
	public void HandleMovement() {
		float inputX = Input.GetAxis("Left", "Right");
		float inputY = Input.GetAxis("Up", "Down");
		
		Vector2 velocity = new Vector2(inputX, inputY);
		if (velocity.Length() > 0) {
            velocity = velocity.Normalized() * Speed;
            float x = velocity.X;
            float y = velocity.Y;

			if(Math.Abs(x) > Math.Abs(y))
            {   
                if(x > 0)
                {
                    _arms[1].Play("right");
                    _arms[1].Position = new Vector2(6,1);
                    _arms[1].ZIndex = 5;

                    _arms[0].Play("right");
                    _arms[0].Position = new Vector2(-6,1);
                    _arms[0].ZIndex = 1;

                    _animatedSprite.Play("walkToRight");
                    _attackHitbox.Position = _rightAttackHitboxPosition;
                }
                else
                {
                    _arms[1].Play("left");
                    _arms[1].Position = new Vector2(6,1);
                    _arms[1].ZIndex = 1;

                    _arms[0].Play("left");
                    _arms[0].Position = new Vector2(-6,1);
                    _arms[0].ZIndex = 5;

                    _animatedSprite.Play("walkToLeft");
                    _attackHitbox.Position = _leftAttackHitboxPosition;
                }
            }
            else
            {
                if(y < 0)
                {
                    _arms[0].Play("upRight");
                    _arms[0].Position = new Vector2(6,0);
                    _arms[0].ZIndex = 1;

                    _arms[1].Play("upLeft");
                    _arms[1].Position = new Vector2(-6,0);
                    _arms[1].ZIndex = 1;

                    _animatedSprite.Play("walkFromMe");
                    _attackHitbox.Position = _upAttackHitboxPosition;
                }
                else
                {

                    _arms[0].Play("downLeft");
                    _arms[0].Position = new Vector2(8,0);
                    _arms[0].ZIndex = 1;

                    _arms[1].Play("downRight");
                    _arms[1].Position = new Vector2(-8,0);
                    _arms[1].ZIndex = 1;

                    _animatedSprite.Play("walkToMe");
                    _attackHitbox.Position = _downAttackHitboxPosition;
                }
            }
		}
		else 
		{
			_animatedSprite.Play("idle");
		}	
		

		Velocity = velocity;
	}
	
	public void HandleAttack() {
		if (Input.IsActionJustPressed("Attack") && !_isAttacking) 
		{
			PerformAttack();
		}
	}
	
	public async void PerformAttack() {

        if (!_isAttacking)
        {
            _isAttacking = true;
        
        
            if(_attackHitbox.Position == _rightAttackHitboxPosition) _attackPlayer.Play("attackRight");
            else if(_attackHitbox.Position == _leftAttackHitboxPosition) _attackPlayer.Play("attackLeft");
            else if(_attackHitbox.Position == _downAttackHitboxPosition) _attackPlayer.Play("attackDown");
            else if(_attackHitbox.Position == _upAttackHitboxPosition) _attackPlayer.Play("attackUp");

            _audioPlayer.Stream = GD.Load<AudioStream>("res://Scenes/Entityes/Main Hero/sounds/punch.mp3");
            _audioPlayer.Play();

            var Timer = GetTree().CreateTimer(_attackCooldownTime);

            Timer.Timeout += () => {
                if (IsInstanceValid(this)) {
                    _isAttacking = false;
                }
            };
        }
        
    }
	
	public void GetDamage(int value)
	{
        _isHurting = true;
        _cooldownHurting = _cooldownHurtingTime;
		if(value < 0) value *= -1;
		
		if(_currentHp - value > 0)
		{
			_currentHp -= value;
		}
        else
        {
            _currentHp = 0;
            GetTree().ChangeSceneToFile("res://Scenes/GameOver/game_over.tscn");
        }
	}
	public void GetHeal(int value)
	{
		if(value < 0) value *= -1;

        _audioPlayer.Stream = GD.Load<AudioStream>("res://Scenes/Entityes/Main Hero/sounds/heal.mp3");
        _audioPlayer.VolumeDb += 30;
		_audioPlayer.Play();
        _audioPlayer.VolumeDb -= 30;

		if(_currentHp + value <= 100)
		{
			_currentHp += value;				
		}
		else _currentHp = 100;
	}

}
