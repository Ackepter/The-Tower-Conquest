using Godot;
using System;

public partial class MainCharacter : CharacterBody2D
{
	public float Speed = 100f; 
	public bool _isHurting = false;
	private float _cooldownHurting = 0f;
	private const float _cooldownHurtingTime = 0.4f;
<<<<<<< HEAD
	
	// 🔥 ИЗМЕНЕНО: было const, теперь обычная переменная
	private int _maxHp = 100;

	private int _currentHp = 100;
=======
	private const int _maxHp = 100;
	private const int _maxShield = 100;
		 
	private const float ShieldRegenDelay = 3.0f;      
	private const int ShieldRegenAmount = 10;      
	private const float ShieldRegenInterval = 5.0f;
	private Tween _shieldRegenTween;

	private int _currentHp = _maxHp;
	private int _currentShield = _maxShield;
	public int GetCurrentShield => _currentShield;
	public int GetMaxShield => _maxShield;
>>>>>>> 6cf3b0e42b158d40279ead1356d9f03cfa1ec528
	public int GetCurrentHp => _currentHp;
	public int GetMaxHp => _maxHp;

	
	private AnimationPlayer _attackPlayer;
	public AnimatedSprite2D _animatedSprite;
	private Area2D _attackHitbox;
	private bool _isAttacking = false;
	private const float _attackCooldownTime = 0.4f;
	
	private AnimatedSprite2D[] _arms = new AnimatedSprite2D[2];
	
	public Node2D CurrentEnemy { get; set; }

	private Vector2 _leftAttackHitboxPosition = new Vector2(-16, 0);
	private Vector2 _rightAttackHitboxPosition = new Vector2(16, 0);
	private Vector2 _upAttackHitboxPosition = new Vector2(0, -16);
	private Vector2 _downAttackHitboxPosition = new Vector2(0, 16);
	private float _directionMultiplier = 1f;

	private AudioStreamPlayer _audioPlayer;
	
<<<<<<< HEAD
	// 🔥 HpBar (ищем универсально)
	private ProgressBar _healthBar;

=======
>>>>>>> 6cf3b0e42b158d40279ead1356d9f03cfa1ec528
	public override void _Ready() 
	{
		AddToGroup("player1");
		 GD.Print("🎮 MainCharacter: добавлен в группу 'player'");
		
		_audioPlayer = GetNode<AudioStreamPlayer>("audioPlayer");
		_animatedSprite = GetNode<AnimatedSprite2D>("Player");	
		_attackPlayer = GetNode<AnimationPlayer>("AttackPlayer");	
		_arms[0] = GetNode<AnimatedSprite2D>("LeftArm");
		_arms[1] = GetNode<AnimatedSprite2D>("RightArm");
		_attackHitbox = GetNode<Area2D>("AttackHitbox");
		_attackHitbox.BodyEntered += OnBodyEntered;
		
		// 🔥 Ищем HpBar по всей сцене (универсальный поиск)
		_healthBar = GetTree().Root.FindChild("HpBar", true, false) as ProgressBar;
		
		if (_healthBar == null)
		{
			GD.PrintErr("[MainCharacter] ⚠️ HpBar не найден в сцене!");
		}
		
		LoadHealth();
	}
	
	private void LoadHealth()
	{
		if (GameManager.Instance != null)
		{
			_currentHp = GameManager.Instance.GetCurrentHealth();
			_maxHp = GameManager.Instance.GetMaxHealth();  // 🔥 Загружаем и максимум!
			GD.Print($"[MainCharacter] Здоровье загружено: {_currentHp}/{_maxHp}");
		}
		UpdateHealthUI();
	}
	
	private void SaveHealth()
	{
		if (GameManager.Instance != null)
		{
			GameManager.Instance.SaveHealth(_currentHp, _maxHp);
		}
	}
	
	private void UpdateHealthUI()
	{
		if (_healthBar != null)
		{
			_healthBar.MaxValue = _maxHp;
			_healthBar.Value = _currentHp;
			GD.Print($"[MainCharacter] ✅ HpBar обновлён: {_currentHp}/{_maxHp}");
		}
	}
	
	public void OnBodyEntered(Node2D body) 
	{
		if (body.IsInGroup("enemy") && _isAttacking) 
		{
			BaseEnemyScript enemy = body as BaseEnemyScript;
			if (enemy != null) 
			{
				enemy.TakeDamage(25);
				_isAttacking = false;
			}
		}
	}
	
	public override void _PhysicsProcess(double delta)
	{
		if (_cooldownHurting > 0)
			_cooldownHurting -= (float)delta;
		else if (_cooldownHurting <= 0) 
			_isHurting = false;

		if (!_isAttacking && !_isHurting)
		{
			HandleMovement();
			HandleAttack();
			MoveAndSlide();
		}
	}
	
	public void HandleMovement() 
	{
		float inputX = Input.GetAxis("Left", "Right");
		float inputY = Input.GetAxis("Up", "Down");
		
		Vector2 velocity = new Vector2(inputX, inputY);
		if (velocity.Length() > 0) 
		{
			velocity = velocity.Normalized() * Speed * _directionMultiplier;
			float x = velocity.X;
			float y = velocity.Y;

			if (Math.Abs(x) > Math.Abs(y))
			{   
				if (x > 0)
				{
					_arms[1].Play("right");
					_arms[1].Position = new Vector2(6, 1);
					_arms[1].ZIndex = 5;
					_arms[0].Play("right");
					_arms[0].Position = new Vector2(-6, 1);
					_arms[0].ZIndex = 1;
					_animatedSprite.Play("walkToRight");
					_attackHitbox.Position = _rightAttackHitboxPosition;
				}
				else
				{
					_arms[1].Play("left");
					_arms[1].Position = new Vector2(6, 1);
					_arms[1].ZIndex = 1;
					_arms[0].Play("left");
					_arms[0].Position = new Vector2(-6, 1);
					_arms[0].ZIndex = 5;
					_animatedSprite.Play("walkToLeft");
					_attackHitbox.Position = _leftAttackHitboxPosition;
				}
			}
			else
			{
				if (y < 0)
				{
					_arms[0].Play("upRight");
					_arms[0].Position = new Vector2(6, 0);
					_arms[0].ZIndex = 1;
					_arms[1].Play("upLeft");
					_arms[1].Position = new Vector2(-6, 0);
					_arms[1].ZIndex = 1;
					_animatedSprite.Play("walkFromMe");
					_attackHitbox.Position = _upAttackHitboxPosition;
				}
				else
				{
					_arms[0].Play("downLeft");
					_arms[0].Position = new Vector2(8, 0);
					_arms[0].ZIndex = 1;
					_arms[1].Play("downRight");
					_arms[1].Position = new Vector2(-8, 0);
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
	
	public void HandleAttack() 
	{
		if (Input.IsActionJustPressed("Attack") && !_isAttacking) 
		{
			PerformAttack();
		}
	}
	
	public async void PerformAttack() 
	{
		if (!_isAttacking)
		{
			_isAttacking = true;
		
			if (_attackHitbox.Position == _rightAttackHitboxPosition) 
				_attackPlayer.Play("attackRight");
			else if (_attackHitbox.Position == _leftAttackHitboxPosition) 
				_attackPlayer.Play("attackLeft");
			else if (_attackHitbox.Position == _downAttackHitboxPosition) 
				_attackPlayer.Play("attackDown");
			else if (_attackHitbox.Position == _upAttackHitboxPosition) 
				_attackPlayer.Play("attackUp");

			_audioPlayer.Stream = GD.Load<AudioStream>("res://Scenes/Entityes/Main Hero/sounds/punch.mp3");
			_audioPlayer.Play();

			var timer = GetTree().CreateTimer(_attackCooldownTime);
			timer.Timeout += () => {
				if (IsInstanceValid(this)) 
					_isAttacking = false;
			};
		}
	}
	
	private void StartShieldRegenSystem()
	  {
		if (_shieldRegenTween != null && _shieldRegenTween.IsRunning())
		  _shieldRegenTween.Kill();
		
		_shieldRegenTween = CreateTween();
		
		_shieldRegenTween.TweenInterval(ShieldRegenDelay);
		_shieldRegenTween.TweenCallback(Callable.From(StartRegenLoop));
	  }
  
 	 private void RegenShieldTick()
	 {
		if (_currentShield < _maxShield)
		{
		  _currentShield += ShieldRegenAmount;
		  if (_currentShield > _maxShield)
			_currentShield = _maxShield;
		}
	  }
	
	public void GetDamage(int value)
	{	
		_isHurting = true;
		_cooldownHurting = _cooldownHurtingTime;
<<<<<<< HEAD
		if (value < 0) value *= -1;
		
		if (_currentHp - value > 0)
		{
			_currentHp -= value;
=======
				
		if(value < 0) value *= -1;
		RestartShieldRegenTimer();
		int remainingDamage = value;
		
		if (_currentShield > 0) {
			if (_currentShield >= remainingDamage) {
				_currentShield -= remainingDamage;
				remainingDamage = 0;
			}
			else {
				remainingDamage -= _currentShield;
				_currentShield = 0;
			}
>>>>>>> 6cf3b0e42b158d40279ead1356d9f03cfa1ec528
		}
		
		if (remainingDamage > 0 && _currentHp > 0) {
			_currentHp -= remainingDamage;
			
			if (_currentHp <= 0) {
				_currentHp = 0;
				GD.Print($"💀 GAME OVER!");
				GetTree().ChangeSceneToFile("res://Scenes/GameOver/game_over.tscn");
			}
		}
		
		SaveHealth();
		UpdateHealthUI();
	}
	
<<<<<<< HEAD
=======
	private void RestartShieldRegenTimer()
	  {
		if (_shieldRegenTween != null && _shieldRegenTween.IsRunning())
		  _shieldRegenTween.Kill();
		
		_shieldRegenTween = CreateTween();
		_shieldRegenTween.TweenInterval(ShieldRegenDelay);
		_shieldRegenTween.TweenCallback(Callable.From(StartRegenLoop));
	  }

  private void StartRegenLoop()
	  {
		RegenShieldTick();
		
		var regenTween = CreateTween();
		regenTween.TweenInterval(ShieldRegenInterval);
		regenTween.TweenCallback(Callable.From(StartRegenLoop));
	  }
  
  public override void _ExitTree()
	  {
		if (_shieldRegenTween != null && _shieldRegenTween.IsRunning())
		  _shieldRegenTween.Kill();
	  }

	
>>>>>>> 6cf3b0e42b158d40279ead1356d9f03cfa1ec528
	public void GetHeal(int value)
	{
		if (value < 0) value *= -1;

		_audioPlayer.Stream = GD.Load<AudioStream>("res://Scenes/Entityes/Main Hero/sounds/heal.mp3");
		_audioPlayer.VolumeDb += 30;
		_audioPlayer.Play();
		_audioPlayer.VolumeDb -= 30;

		// 🔥 Лечим, но не больше ТЕКУЩЕГО максимума (_maxHp)
		if (_currentHp + value <= _maxHp)
		{
			_currentHp += value;                
		}
		else 
			_currentHp = _maxHp;
		
		SaveHealth();
		UpdateHealthUI();
	}
	
	private void ShowUpgradeFeedback(string upgradeId)
	{
		var tween = GetTree().CreateTween();
		tween.TweenProperty(this, "modulate", new Color(1, 1, 0.5f, 1), 0.1f);
		tween.TweenInterval(0.1f);
		tween.TweenProperty(this, "modulate", new Color(1, 1, 1, 1), 0.1f);
		
		var feedback = new Label();
		feedback.Text = upgradeId switch {
			"damage_up" => "🔥 +Урон!",
			"speed_up" => "⚡ +Скорость!",
			"health_up" => "❤️ +Здоровье!",
			_ => "+Улучшение!"
		};
		feedback.Position = new Vector2(0, -50);
		feedback.AddThemeFontSizeOverride("font_size", 24);
		feedback.AddThemeColorOverride("font_color", Colors.White);
		feedback.ZIndex = 100;
		AddChild(feedback);
		
		var textTween = GetTree().CreateTween();
		textTween.TweenProperty(feedback, "position:y", -80, 0.5f);
		textTween.Parallel().TweenProperty(feedback, "modulate:a", 0, 0.5f);
		textTween.TweenCallback(Callable.From(() => feedback.QueueFree()));
		
		UpdateHealthUI();
	}
	
	public void ApplyUpgrade(string upgradeId)
	{
		GD.Print($"[MainCharacter] Применяю улучшение: {upgradeId}");
		
		ShowUpgradeFeedback(upgradeId);
		
		switch (upgradeId)
		{
			case "damage_up":
				GD.Print("[MainCharacter] 🔥 Урон увеличен!");
				break;
				
			case "speed_up":
				_directionMultiplier += 0.5f;
				GD.Print($"[MainCharacter] ⚡ Скорость: {_directionMultiplier}x");
				break;
				
			case "health_up":
				// 🔥 ТОЛЬКО увеличиваем максимум, НЕ лечим!
				GD.Print("[MainCharacter] ❤️ Макс. здоровье увеличено (текущее не изменено)!");
				break;
		}
	}
}
