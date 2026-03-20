using Godot;
using System;

public partial class MainCharacter : CharacterBody2D
{
	public const float Speed = 100f; 
	private const int _maxHp = 100;

	private int _currentHp = _maxHp;
	public int GetCurrentHp => _currentHp;
	private AnimatedSprite2D _animatedSprite;
	private AnimationPlayer _animatedPlayer;
	private bool _isAttacking = false;
	private Area2D _attackHitbox;
	private CollisionShape2D _attackHitboxCollison;
	private float cooldownAttack = 0f;
	private const float attackCooldownTime = 0.5f;
	private CharacterBody2D enemy;
	private Sprite2D[] _swords = new Sprite2D[2];
	private Sprite2D _currentSword;
	private Tween _swordTween;

	// Параметры анимации меча (в радианах и пикселях)
	private const float WindUpAngle = -0.5f;      // Занесение назад (против часовой)
	private const float SlashAngle = 1.2f;        // Удар вперёд
	private const float WindUpOffset = -15f;      // Смещение назад при занесении
	private const float SlashOffset = 20f;        // Выдвижение вперёд при ударе

	private float _cooldownAttack = 0f;
	private const float _attackCooldownTime = 0.5f;

	// Тайминги анимации (в секундах)
	private const float WindUpTime = 0.15f;
	private const float SlashTime = 0.1f;
	private const float ReturnTime = 0.15f;

	private Vector2 _swordBasePosition = Vector2.Zero;
	private float _swordBaseRotation = 0f;

	private float _directionMultiplier = 1f;
	public override void _Ready() 
	{
		_animatedSprite = GetNode<AnimatedSprite2D>("Player");	
		_attackHitbox = GetNode<Area2D>("AttackHitbox");
		_attackHitboxCollison = GetNode<CollisionShape2D>("AttackHitbox/CollisionShape2D");
		_swords[0]= GetNode<Sprite2D>("SwordLeft");
		_swords[1]= GetNode<Sprite2D>("SwordRight");
		_currentSword = _swords[1];
		_currentSword.Show();
		_swordBasePosition = _currentSword.Position;
		_swordBaseRotation = _currentSword.Rotation;

		_attackHitbox.BodyEntered += OnBodyEntered;
	}
	
	public override void _PhysicsProcess(double delta)
	{
		if (cooldownAttack > 0)
			cooldownAttack -= (float)delta;
		
		HandleMovement();

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
		
		if(velocity.X != 0)
		{
			FlipSprite(velocity);
		}

		if (velocity.Length() > 0) {
			_animatedSprite.Play("walk");
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
	
	public async void PerformAttack() 
	{
		_isAttacking = true;
		_cooldownAttack = _attackCooldownTime + WindUpTime + SlashTime + ReturnTime;

		if (_currentSword == _swords[0])
		{
			_attackHitbox.Position = new Vector2(-16,0);
		}
		else if(_currentSword == _swords[1])
		{
			_attackHitbox.Position = new Vector2(16,0);
		}

		// Останавливаем предыдущую анимацию
		if (_swordTween != null && _swordTween.IsRunning())
			_swordTween.Kill();
	
		//Берём текущие базовые значения ПЕРЕД анимацией
		_swordBasePosition = _currentSword.Position;
		_swordBaseRotation = _currentSword.Rotation;

		//Рассчитываем смещения с учётом направления
		float currentWindUpOffset = WindUpOffset * _directionMultiplier;
		float currentSlashOffset = SlashOffset * _directionMultiplier;
		
		//Инвертируем угол замаха для левой стороны, чтобы замах был назад
		float currentWindUpAngle = WindUpAngle * _directionMultiplier;
		float currentSlashAngle = SlashAngle * _directionMultiplier;

		//ЗАНЕСЕНИЕ НАЗАД
	
		_swordTween = CreateTween();
		_swordTween.SetParallel();
	
		// Поворот: от базового к занесению
		_swordTween.TweenProperty(_currentSword, "rotation", 
			_swordBaseRotation + currentWindUpAngle, WindUpTime)
			.SetTrans(Tween.TransitionType.Quad)
			.SetEase(Tween.EaseType.Out);
	
		// Смещение: от базового к занесению
		_swordTween.TweenProperty(_currentSword, "position:x", 
			_swordBasePosition.X + currentWindUpOffset, WindUpTime)
			.SetTrans(Tween.TransitionType.Quad)
			.SetEase(Tween.EaseType.Out);
	
		await ToSignal(_swordTween, "finished");
	
		//УДАР

		//активирую атаку
		_attackHitboxCollison.Disabled = false;

		_attackHitbox.Monitoring = true;
	
		_swordTween = CreateTween();
		_swordTween.SetParallel();
	
			// Удар: от занесения к максимальной точке
		_swordTween.TweenProperty(_currentSword, "rotation", 
			_swordBaseRotation + currentSlashAngle, SlashTime)
			.SetTrans(Tween.TransitionType.Cubic)
			.SetEase(Tween.EaseType.In);
	
		_swordTween.TweenProperty(_currentSword, "position:x", 
			_swordBasePosition.X + currentSlashOffset, SlashTime)
			.SetTrans(Tween.TransitionType.Cubic)
			.SetEase(Tween.EaseType.In);
	
		await ToSignal(_swordTween, "finished");
	
		_attackHitbox.Monitoring = false;
	
		//ВОЗВРАТ
	
		_swordTween = CreateTween();
		_swordTween.SetParallel();
	
		// Возврат к БАЗОВЫМ значениям (не к нулю!)
		_swordTween.TweenProperty(_currentSword, "rotation", 
			_swordBaseRotation, ReturnTime)
			.SetTrans(Tween.TransitionType.Quad)
			.SetEase(Tween.EaseType.Out);
	
		_swordTween.TweenProperty(_currentSword, "position:x", 
			_swordBasePosition.X, ReturnTime)
			.SetTrans(Tween.TransitionType.Quad)
			.SetEase(Tween.EaseType.Out);
	
		await ToSignal(_swordTween, "finished");

		_attackHitboxCollison.Disabled = true;
		_isAttacking = false;
}
	
	public void OnBodyEntered(Node2D body) {		
		if (body.IsInGroup("enemy") && _isAttacking) {
			BaseEnemyScript enemy = body as BaseEnemyScript;
			
			if (enemy != null) {
				enemy.TakeDamage(50);
				GD.Print(enemy.GetCurrentHp());
			}
			else
			{
				GD.Print("Enemy is not finded");
			}
		}
	}
	private void FlipSprite(Vector2 velocity)
	{
		bool shouldFaceLeft = velocity.X < 0;
	
		// Если хотим влево И уже смотрим влево (FlipH = true) -> выходим
		if (shouldFaceLeft && _animatedSprite.FlipH) return;
		
		// Если хотим вправо И уже смотрим вправо (FlipH = false) -> выходим
		if (!shouldFaceLeft && !_animatedSprite.FlipH) return;
		
		// Сбрасываем анимацию меча перед поворотом
		ResetSwordTransform();
		
		if (shouldFaceLeft)
		{
			_currentSword.Hide();
			_currentSword = _swords[0];
			_currentSword.Show();
			_animatedSprite.FlipH = true;
			_directionMultiplier = -1f;
		}
		else
		{
			_currentSword.Hide();
			_currentSword = _swords[1];
			_currentSword.Show();
			_animatedSprite.FlipH = false;
			_directionMultiplier = 1f;
		}
		
		// Обновляем базу для нового меча
		_swordBasePosition = _currentSword.Position;
		_swordBaseRotation = _currentSword.Rotation;
	}

	private void ResetSwordTransform()
	{
		_isAttacking = false;
		// Если меч анимируется — останавливаем
		if (_swordTween != null && _swordTween.IsRunning())
		{
			_swordTween.Kill();
		}
	
		// Мгновенно возвращаем меч в базовое состояние
		_currentSword.Rotation = _swordBaseRotation;
		_currentSword.Position = _swordBasePosition;

		_attackHitboxCollison.Disabled = true;
	}

	public void GetDamage(int value)
	{
		if(_currentHp - value >= 0)
		{
			_currentHp -= value;
		}
		else _currentHp = 0;
	}
}
