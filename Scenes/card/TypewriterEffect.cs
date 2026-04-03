using Godot;
using System.Threading.Tasks;

public partial class TypewriterEffect : Label
{
	[Export] public string FullText = "Выбери одно из улучшений";
	[Export] public float CharDelay = 0.05f;
	
	private int _currentChar = 0;
	private Timer _timer;

	public override void _Ready()
	{
		// 🔥 РАБОТАЕМ ДАЖЕ НА ПАУЗЕ!
		ProcessMode = ProcessModeEnum.Always;
		
		GD.Print("[Typewriter] _Ready вызван!");
		GD.Print($"[Typewriter] FullText: '{FullText}'");
		GD.Print($"[Typewriter] Text: '{Text}'");
		GD.Print($"[Typewriter] CharDelay: {CharDelay}");
		
		// Если текст не задан в FullText, берём из Text
		if (string.IsNullOrEmpty(FullText) || FullText == "Выбери одно из улучшений")
			FullText = Text;
		
		GD.Print($"[Typewriter] Используем FullText: '{FullText}'");
		
		// Создаём таймер
		_timer = new Timer();
		_timer.OneShot = false;
		_timer.WaitTime = CharDelay;
		_timer.Timeout += OnTimerTimeout;
		
		// 🔥 ТАЙМЕР ТОЖЕ РАБОТАЕТ НА ПАУЗЕ!
		_timer.ProcessMode = ProcessModeEnum.Always;
		
		AddChild(_timer);
		
		// Начинаем с пустого текста
		Text = "";
		_currentChar = 0;
		
		// Запускаем таймер
		_timer.Start();
		GD.Print("[Typewriter] Таймер запущен!");
	}

	private void OnTimerTimeout()
	{
		GD.Print($"[Typewriter] Timeout! _currentChar={_currentChar}, Length={FullText.Length}");
		
		if (_currentChar < FullText.Length)
		{
			// Добавляем по одному символу
			_currentChar++;
			Text = FullText.Substring(0, _currentChar);
			GD.Print($"[Typewriter] Text теперь: '{Text}'");
		}
		else
		{
			// Закончили - останавливаем таймер
			_timer.Stop();
			GD.Print("[Typewriter] Готово! Таймер остановлен.");
		}
	}
	
	public override void _ExitTree()
	{
		_timer?.QueueFree();
	}
}
