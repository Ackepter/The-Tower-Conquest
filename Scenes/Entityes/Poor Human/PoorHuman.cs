using System;
using Godot;
 
public partial class PoorHuman : BaseEnemyScript
{
    public override float Speed => 30f;


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


    private Area2D _attackHitbox;
    private CollisionShape2D _attackHitboxCollison;

    public override void _Ready()
    {
        _attackHitbox = GetNode<Area2D>("AttackHitbox");
		_attackHitboxCollison = GetNode<CollisionShape2D>("AttackHitbox/CollisionShape2D");
        _swords[0]= GetNode<Sprite2D>("SwordLeft");
		_swords[1]= GetNode<Sprite2D>("SwordRight");
		_currentSword = _swords[1];
		_currentSword.Show();
		_swordBasePosition = _currentSword.Position;
		_swordBaseRotation = _currentSword.Rotation;


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
        	UpdateAnimation();
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

		Vector2 directionToHero = (_hero.GlobalPosition - GlobalPosition);
		if(Math.Abs(directionToHero.X) < Math.Abs(directionToHero.Y))
		{
			if(directionToHero.Y > 0)
			{
				_attackHitbox.Position = new Vector2(0,16);
			}
			else if(directionToHero.Y < 0){
				_attackHitbox.Position = new Vector2(0,-16);
			}
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

     protected override void UpdateAnimation()
    {
        if (_sprite == null) return;

        if (Velocity.Length() > 10)
            _sprite.Play("walk");
        else if (!_isAttacking)
            _sprite.Play("idle");
            
        FlipSprite(Velocity);
    }

	protected override void FlipSprite(Vector2 velocity)
	{
		bool shouldFaceLeft = velocity.X < 0;
		
		if (shouldFaceLeft)
		{
			_currentSword.Hide();
			_currentSword = _swords[0];
			_currentSword.Show();
			_sprite.FlipH = true;
			_directionMultiplier = -1f;
		}
		else
		{
			_currentSword.Hide();
			_currentSword = _swords[1];
			_currentSword.Show();
			_sprite.FlipH = false;
			_directionMultiplier = 1f;
		}
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

    protected override void Attack()
    {
        base.Attack();

        PerformAttack();
    }
    
}