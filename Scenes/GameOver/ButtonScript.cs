using Godot;
using System;

public partial class ButtonScript : Button
{
    public void OnButtonPresed()
    {
        GetTree().ChangeSceneToFile("res://Scenes/Main/main.tscn");
    }
}
