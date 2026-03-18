using Godot;
using System;

public partial class MainCharacter : CharacterBody2D
{
	public const float Speed = 100f; 
	private AnimatedSprite2D _animatedSprite;
	private AnimationPlayer _animatedPlayer;
	private bool _isAttacking = false;
	private Area2D _attackHitbox;
	private float cooldownAttack = 0f;
	private const float attackCooldownTime = 0.5f;
	private CharacterBody2D enemy;

	public override void _Ready() 
	{
		_animatedSprite = GetNode<AnimatedSprite2D>("Sprite");	
		_attackHitbox = GetNode<Area2D>("AttackHitbox");
		_attackHitbox.BodyEntered += OnBodyEntered;
		_animatedPlayer = GetNode<AnimationPlayer>("Player");
	}
	
	public override void _PhysicsProcess(double delta)
	{
		if (cooldownAttack > 0)
			cooldownAttack -= (float)delta;
		
		// Движение (если не атакуем)
		if (!_isAttacking)
		{
			HandleMovement();
		}
		
		// Атака
		HandleAttack();
		
		MoveAndSlide();
	}
	
	public void HandleMovement() {
		float inputX = Input.GetAxis("Left", "Right");
		float inputY = Input.GetAxis("Up", "Down");
		
		Vector2 velocity = new Vector2(inputX, inputY);
		
		if (velocity.Length() > 0) 
		{
			velocity = velocity.Normalized() * Speed;	
		}
		
		if (velocity.Length() > 0) {
			if (Mathf.Abs(inputX) > Mathf.Abs(inputY)) {
				if (inputX > 0) {
					_animatedSprite.Play("character_right");
				} else {
					_animatedSprite.Play("character_left");
				}
			}
			else {
				if (inputY > 0) {
					_animatedSprite.Play("character_down");
				} else {
					_animatedSprite.Play("character_up");
				}
			}
		}
		
		else 
		{
			string currentAnimation = _animatedSprite.Animation;
			if (currentAnimation.Contains("up"))
				_animatedSprite.Play("idle_up");
			else if (currentAnimation.Contains("down"))
				_animatedSprite.Play("idle_down");
			else if (currentAnimation.Contains("left"))
				_animatedSprite.Play("idle_left");
			else if (currentAnimation.Contains("right"))
				_animatedSprite.Play("idle_right");
		}	
		
		Velocity = velocity;
	}
	
	public void HandleAttack() {
		if (Input.IsActionJustPressed("Attack") && cooldownAttack <= 0)
		{
			PerformAttack();

		}
	}
	
	public void PerformAttack() {
		_isAttacking = true;
		cooldownAttack = attackCooldownTime;
		var Timer = GetTree().CreateTimer(0.5f);
		
		string currentAnimation = _animatedSprite.Animation;
			if (currentAnimation.Contains("up"))
				_animatedPlayer.Play("attack_up");
			else if (currentAnimation.Contains("down"))
				_animatedPlayer.Play("attack_down");
			else if (currentAnimation.Contains("left"))
				_animatedPlayer.Play("attack_left");
			else if (currentAnimation.Contains("right"))
				_animatedPlayer.Play("attack_right");

		Timer.Timeout += () => {
			if (IsInstanceValid(this)) {
				_isAttacking = false;
			}
		};
	}
	
	public void OnBodyEntered(Node2D body) {		
		if (body.IsInGroup("enemy") && _isAttacking) {
			Enemy enemy = body as Enemy;
			
			if (enemy != null) {
				enemy.takeDamage(50);
				GD.Print(enemy.GetHp());
			}
		}
	}
	
}
