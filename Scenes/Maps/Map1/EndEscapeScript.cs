using Godot;
using System;

public partial class EndEscapeScript : Area2D
{
	private string startEscapeMessage = "Ты прошёл!!!";

	private RichTextLabel text;
	private Panel panel;
	private Control control;
	
	private bool isEnded = true;

	public void OnBodyEntered(Node2D body) {
		MainCharacter hero = body as MainCharacter;
		if(hero is null) return;

		control = hero.GetNode<Camera2D>("Camera2D").GetNode<Control>("Control");
		panel = control.GetNode<Panel>("Panel");
		text = panel.GetNode<RichTextLabel>("RichTextLabel");

		control.Show();

		if(isEnded){
			isEnded = false;
			panel.Position = new Vector2(-panel.Size.X - 8, panel.Position.Y);
		
			var tween = CreateTween();
			tween.SetTrans(Tween.TransitionType.Cubic);
			tween.SetEase(Tween.EaseType.Out);
			tween.TweenProperty(panel, "position", new Vector2(8, panel.Position.Y), 0.5f);
			
			tween.TweenCallback(Callable.From(() => printIt(0)));


			var timer = GetTree().CreateTimer(startEscapeMessage.Length * 0.1 + 2);
			timer.Timeout += () => {
				if (IsInstanceValid(this)) {
					var tween2 = CreateTween();
					tween2.TweenProperty(panel, "position", new Vector2(-panel.Size.X - 20, panel.Position.Y), 0.3f);
					tween2.TweenCallback(Callable.From(() => control.Hide()));
					text.Text = "";
					isEnded = true;
				}
			};
		}
	}
	private void printIt(int i)
	{
		if (i >= startEscapeMessage.Length) return;

		text.Text += startEscapeMessage[i++];
		var timer = GetTree().CreateTimer(0.1f);
		timer.Timeout += () => {
			if (IsInstanceValid(this)) {
				printIt(i);
			}
		};
	}
}
