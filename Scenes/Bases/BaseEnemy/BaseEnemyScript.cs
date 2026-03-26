using Godot;
using System;

//ВАЖНЫЙ НЮАНС КОГДА БУДЕТЕ СОЗДАВАТЬ ОТ ЭТОГО КЛАССА НАСЛЕДНИКОВ ДЛЯ БУДУЮЩИХ ВРАГОВ, ТО ИХ ПРИКРЕПЛЯЙТЕ КАК СКРИПТ В УЗЛЕ, А НЕ ЭТОТ СКРИПТ
public partial class BaseEnemyScript : CharacterBody2D
{ 
    
    public virtual float Speed => 7f;

    protected virtual float AttackCooldown => 1.5f;

    protected virtual int RecognizeDistance => 200;
    protected virtual int MaxHp => 100;

    // Состояние
    protected int _currentHp;
    protected bool _canAttack = true;
    protected bool _isAttacking = false;

    // Узлы
    protected CharacterBody2D _hero;
    protected AnimatedSprite2D _sprite;
    protected RayCast2D _visionRay;
    protected NavigationAgent2D _agent;

    public override void _Ready()
    {
        AddToGroup("enemy");
        
        _sprite = GetNode<AnimatedSprite2D>("AnimatedSprite");
        _visionRay = GetNode<RayCast2D>("VisionRay");
        _agent = GetNode<NavigationAgent2D>("NavigationAgent2D");
        _hero = GetNode<CharacterBody2D>("../FirstMap/MainCharacter");

        _currentHp = MaxHp;
        
        SetupEnemy();
    }

    protected virtual void SetupEnemy() { }

    protected float distance;
    public override void _PhysicsProcess(double delta)
    {
        if (_hero == null) return;

        distance = GlobalPosition.DistanceTo(_hero.GlobalPosition);

        if (_agent != null)
            _agent.TargetPosition = _hero.GlobalPosition;

        if (_visionRay != null && _hero != null)
        {
            Vector2 directionToHero = (_hero.GlobalPosition - GlobalPosition);
            _visionRay.TargetPosition = directionToHero;
        }

        
    }

    protected bool HasLineOfSight()
    {
        if (_visionRay == null) return true;
        return !_visionRay.IsColliding();
    }

    protected virtual void ChaseHero(){}

    protected virtual void Attack(){}

    public virtual void TakeDamage(int value)
    {
        _currentHp -= value;
        if (_currentHp <= 0)
        {
            _currentHp = 0;
            Die();
        }
    }

    public int GetCurrentHp() => _currentHp;
    public int GetMaxHp() => MaxHp;

    public virtual void Die()
    {
        QueueFree();
    }
}