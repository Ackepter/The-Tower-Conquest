using Godot;
using System;

public partial class Enemy : CharacterBody2D
{
	public int hp = 100;	
	
	public override void _Ready() {
		AddToGroup("enemy");
	}
	
	public void takeDamage(int value){
		hp -= value;
		if(hp <= 0) {
			hp = 0;
			Die();	
		}
	}
	public int GetHp() {
		return hp;
	}
	
	public void Die() {
		GD.Print("враг умер");
		QueueFree();
	}
	
}
