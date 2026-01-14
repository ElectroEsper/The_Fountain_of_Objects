

public static class Program
{
	public static void Main()
	{
		Dungeon.Init(5,3);
		Player player =  new Player();
		bool isPlaying = true;
		Intro();
		// Game Loop for now, may refractor later...
		while (isPlaying)
		{
			// 1. Player is told what they sense...
			AtFountainCheck();
			player.Sense();
			// 2. Player input their action
			GetPlayerInput();
			player.Run();
			// 3. Action is resolved, gameState updates
			GameOverCheck();
			TextEngine.Display(Dialogs.HorizontalLine,MessageType.Neutral);
		}
		void GetPlayerInput()
		{
			string? input = TextEngine.Input();
			player.Command = input switch
			{
				"move north" => new MoveNorth(),
				"move east" => new MoveEast(),
				"move south" => new MoveSouth(),
				"move west" => new MoveWest(),
				"turn on" => new TurnOn(),
				"turn off" => new TurnOff(),
				_ => null
			};
			
		}

		void AtFountainCheck()
		{
			if (Dungeon.IsInFountainRoom(player))
			{
				// Assume player is at fountain's room...
				Dungeon.GetFountainRoom().Visited = true;
			}
		}
		void Intro()
		{
			TextEngine.Display(Dialogs.Intro,MessageType.Narrative);
			TextEngine.Display(Dialogs.HorizontalLine,MessageType.Neutral);
		}
		void GameOverCheck()
		{
			if (Dungeon.FountainIsActive && (player.Coordinate.X == 0 && player.Coordinate.Y == 0))
			{
				isPlaying = false;
				TextEngine.Display("YOU WIN!", MessageType.Positive);
			}
		}
	}
}
//#################################################
// STATIC CLASSES
public static class Dungeon
{
	// Made it a Static class, figured it should be available from anywhere???
	public static IRoom[,] Rooms { get; private set; }
	public static int Width { get; private set; }
	public static int Height { get; private set; }
	public static bool FountainIsActive { get; private set; }
	public static Coordinate FountainCoordinate { get; private set; }
	public static void Init(int x, int y)
	{
		// Initialize fields and array...
		Rooms = new IRoom[x, y];
		Width = x;
		Height = y;
		
		// Generate basic rooms then set entry and fountain
		Generate(x, y);
		
		SetEntry(0,0);
		SetFountain(0,2);
	}

	public static IRoom[] GetAdjacentRooms(int x, int y)
	{
		// Only 5 entries; 4 directions to move to and where the player stands... 
		IRoom[] output = new IRoom[5];
		
		// If room is out of bounds, make a variant with a null type.
		output[0] = Rooms[x, y];
		output[1] = IsInBounds(x, y + 1) ? Rooms[x, y + 1] : new Room(0,0,RoomType.Null);
		output[2] = IsInBounds(x + 1, y) ? Rooms[x + 1, y] : new Room(0,0,RoomType.Null);
		output[3] = IsInBounds(x , y - 1) ? Rooms[x , y - 1] : new Room(0,0,RoomType.Null);
		output[4] = IsInBounds(x - 1, y) ? Rooms[x - 1, y] : new Room(0,0,RoomType.Null);
		
		return output;
	}

	public static void TurnOnFountain()
	{
		FountainIsActive = true;
	}

	public static void TurnOffFountain()
	{
		FountainIsActive = false;
	}
	public static bool IsInFountainRoom(Player player)
	{
		return player.Coordinate.X == FountainCoordinate.X && player.Coordinate.Y == FountainCoordinate.Y;
	}

	public static FountainRoom GetFountainRoom()
	{
		return (FountainRoom)Rooms[FountainCoordinate.X, FountainCoordinate.Y];
	}
	private static bool IsInBounds(int x, int y)
	{
		return x >= 0 && x < Width && y >= 0 && y < Height;
	}
	
	private static void Generate(int x, int y)
	{
		for (int _x = 0; _x < x; _x++)
		{
			for (int _y = 0; _y < y; _y++)
			{
				Rooms[_x, _y] = new Room(_x, _y, RoomType.None);
			}
		}
	}
	private static void SetEntry(int x, int y)
	{
		Rooms[x, y] = new Room(x, y , RoomType.Entry);
	}
	private static void SetFountain(int x, int y)
	{
		Rooms[x, y] = new FountainRoom(x, y);
		FountainCoordinate = Rooms[x, y].Coordinate;
	}
}
public static class TextEngine
{
	// Your job is to take in events and display text relevant to said events
	
	public static string Input()
	{
		DisplayInline(Dialogs.InputPrompt, MessageType.Question);
		SetColor(MessageType.Player);
		return new string(Console.ReadLine());
	}
	
	public static void SetColor(ConsoleColor color)
	{
		Console.ForegroundColor = color;
	}
	public static void Display(string message, ConsoleColor color)
	{
		SetColor(color);
		Console.WriteLine("\n"+message);
	}

	public static void Display(string[] message, ConsoleColor color)
	{
		SetColor(color);
		string text = "\n";
		foreach (string s in message)
		{
			text += s;
		}
		Console.WriteLine(text);
	}

	public static void DisplayInline(string message, ConsoleColor color)
	{
		SetColor(color);
		Console.Write(message);
	}
}

public static class MessageType
{
	public static ConsoleColor Narrative = ConsoleColor.Magenta;
	public static ConsoleColor Sense =  ConsoleColor.Yellow;
	public static ConsoleColor Player = ConsoleColor.Cyan;
	public static ConsoleColor Question = ConsoleColor.White;
	public static ConsoleColor Positive = ConsoleColor.Green;
	public static ConsoleColor Negative = ConsoleColor.Red;
	public static ConsoleColor Neutral = ConsoleColor.Gray;
	//public static ConsoleColor Important = ConsoleColor.
}

public static class Dialogs
{
	public static string Intro = "You have made your way to the Cavern of Objects, high atop jagged mountains. Within these caverns" +
	                             "\nlies the Fountain of Objects, the one-time source of the River of Objects that gave life to this entire" +
	                             "\nisland. By returning the Heart of Object-Oriented Programming —the gem you received from Simula" +
	                             "\nafter arriving on this island— to the Fountain of Objects, you can repair and restore the fountain to its" +
	                             "\nformer glory." +
	                             "\n" +
	                             "\nThe cavern is a grid of rooms, and no natural or human-made light works within due to unnatural" +
	                             "\ndarkness. You can see nothing, but you can hear and smell your way through the caverns to find the" +
	                             "\nFountain of Objects, restore it, and escape to the exit." +
	                             "\n";

	public static string InputPrompt = "What do you do?: ";
	public static string PrefixSameRoom = "In this room,";
	public static string PrefixAdjacentRooms = "of you,";
	public static string FountainNotVisited = " You suspect it could be the Fountain of ";
	public static string FountainVisited = " You know its source is the Fountain of Objects.";
	public static string FountainOff = " you hear water dripping.";
	public static string FountainOn = " you hear flowing water.";
	public static string InvalidMove = "A heavy *Thud* echoes. You've hit a wall.";

	public static string HorizontalLine =
		"-----------------------------------------------------------------------------";

	public static string FountainOffActivate =
		"As you feel blindly in front of you, you touch something wet. You smile," +
		"\nknowing well this is it, the Fountain of Objects." +
		"\n" +
		"\nYou, clumsily, manage to place the Heart of Objects in the right spot." +
		"\nAs if to reward you, the slow drip of water awakens into a strong, continuous" +
		"\nflow." +
		"\n" +
		"\nThe Fountain is now active.";

	public static string FountainOnActivate = "You search you pockets, looking for the Heart of Objects. Why you're doing" +
	                                          "\nthis is unclear, as you know well that you have placed the heart into " +
	                                          "\nits socket yourself.";
	public static string FountainOffDeactivate = "You're searching for something, the Heart of Object. A lapse in" +
	                                             "\nmental clarity perhaps?" +
	                                             "\n" +
	                                             "\nWhat is certain, is that your hand returns empty, if a bit more wet" +
	                                             "\nthan it was.";
	public static string FountainOnDeactivate = "For reasons that are yours only, you reach back for the fountain, and" +
	                                            "\nreach for the Heart of Objects. You contemplate for a moment the weight" +
	                                            "\nof your wish, before removing the artifact from its receptacle." +
	                                            "\n" +
	                                            "\nThe Fountain of Objects, deprived of its core, returns to dormancy.";
	public static string FountainNotPresent = "You search for the fountain, yet find nothing resembling it.";
	
	public static string EmptyRoom = " nothing catches your attention.";
	public static string EntryRoom = " you feel the warmth of the sun. You assume it is this place's entry.";
	public static string NullRoom = " a wall.";
}

//#################################################
// PUBLIC CLASSES
public class Player
{
	// Personnal reminder, this is a "Character" not the controller...
	public Coordinate Coordinate { get; set; }
	public ICommand? Command { get; set; }
	public Player()
	{
		Coordinate =  new Coordinate(0,0);
	}

	public void Run()
	{
		Command?.Run(this);
	}

	public void Sense()
	{
		IRoom[] adjacentRooms = Dungeon.GetAdjacentRooms(Coordinate.X, Coordinate.Y);
		SenseInfo[] senseData = new SenseInfo[adjacentRooms.Length];
		senseData[0] = new SenseInfo(Direction.Current, adjacentRooms[0]);
		senseData[1] = new SenseInfo(Direction.North, adjacentRooms[1]);
		senseData[2] = new SenseInfo(Direction.East, adjacentRooms[2]);
		senseData[3] = new SenseInfo(Direction.South, adjacentRooms[3]);
		senseData[4] = new SenseInfo(Direction.West, adjacentRooms[4]);
		
		Sense(senseData);
	}
	
	public void Sense(SenseInfo[]?  senseData)
	{
		string[] input = new string[0];
		foreach (SenseInfo info in senseData)
		{
			// Set the prefix - is it in this room, or adjacent?
			Array.Resize(ref input, input.Length + 1);
			input[input.Length - 1] = info.Direction == Direction.Current ? Dialogs.PrefixSameRoom : $" {info.Direction} {Dialogs.PrefixAdjacentRooms}";
			
			
			// Then add based on room type...
			Array.Resize(ref input, input.Length + 1);
			if (info.Room.Type is RoomType.Null)
			{
				input[input.Length - 1] = Dialogs.NullRoom;
			}

			if (info.Room.Type is RoomType.None)
			{
				input[input.Length - 1] = Dialogs.EmptyRoom;
			}

			if (info.Room.Type is RoomType.Entry)
			{
				input[input.Length - 1] = Dialogs.EntryRoom;
			}

			if (info.Room.Type is RoomType.Fountain)
			{
				FountainRoom room = (FountainRoom)info.Room;
				if (room.IsActive)
				{
					input[input.Length - 1] = Dialogs.FountainOn;
				}
				else
				{
					input[input.Length - 1] = Dialogs.FountainOff;
				}

				Array.Resize(ref input, input.Length + 1);
				if (room.Visited)
				{
					input[input.Length - 1] = Dialogs.FountainVisited;
				}
				else
				{
					input[input.Length - 1] = Dialogs.FountainNotVisited;
				}
			}
			
		}
		TextEngine.Display(input, MessageType.Sense);
		
		
	}
}

//#################################################
// COMMANDS
public interface ICommand
{
	void Run(Player player);
}
public abstract class Command : ICommand
{
	public abstract void Run(Player player);
	public bool IsValidMove(int x, int y )
	{
		if (x < 0 || x >= Dungeon.Width) {TextEngine.Display("\nYou hit a wall, making an unsurprising *THUD* doing so." +
		                                                     "\nNothing feels different, apart from a bruised ego.", MessageType.Narrative);return false;}
		if (y < 0 || y >= Dungeon.Height ) {TextEngine.Display("\nYou hit a wall, making an unsurprising *THUD* doing so." +
		                                                                  "\nYou feel fine, bar a scratch on your pride.", MessageType.Narrative); return false;}
		
		return true;
	}
}
public class MoveNorth : Command
{
	public override void Run(Player player)
	{
		if (!IsValidMove(player.Coordinate.X,player.Coordinate.Y + 1)) return;
		player.Coordinate = new Coordinate(player.Coordinate.X, player.Coordinate.Y + 1);
	}
}
public class MoveSouth : Command
{
	public override void Run(Player player)
	{
		if (!IsValidMove(player.Coordinate.X, player.Coordinate.Y - 1)) return;
		player.Coordinate = new Coordinate(player.Coordinate.X, player.Coordinate.Y- 1);
	}
}
public class MoveEast : Command
{
	public override void Run(Player player)
	{
		if (!IsValidMove(player.Coordinate.X + 1, player.Coordinate.Y)) return;
		player.Coordinate = new Coordinate(player.Coordinate.X + 1, player.Coordinate.Y);
	}
}
public class MoveWest : Command
{
	public override void Run(Player player)
	{
		if (!IsValidMove(player.Coordinate.X - 1, player.Coordinate.Y)) return;
		player.Coordinate = new Coordinate(player.Coordinate.X - 1, player.Coordinate.Y);
	}
}

public class TurnOn : Command
{
	public override void Run(Player player)
	{
		if (Dungeon.IsInFountainRoom(player))
		{
			if (Dungeon.GetFountainRoom().IsActive)
			{
				TextEngine.Display(Dialogs.FountainOnActivate, MessageType.Narrative);
			}
			else
			{
				Dungeon.GetFountainRoom().IsActive = true;
				Dungeon.TurnOnFountain();
				TextEngine.Display(Dialogs.FountainOffActivate, MessageType.Positive);
			}
		}
		else
		{
			TextEngine.Display(Dialogs.FountainNotPresent, MessageType.Narrative);
		}
	}
}

public class TurnOff : Command
{
	public override void Run(Player player)
	{
		if (Dungeon.IsInFountainRoom(player))
		{
			if (Dungeon.GetFountainRoom().IsActive)
			{
				Dungeon.GetFountainRoom().IsActive = false;
				Dungeon.TurnOffFountain();
				TextEngine.Display(Dialogs.FountainOnDeactivate, MessageType.Negative);
			}
			else
			{
				TextEngine.Display(Dialogs.FountainOffDeactivate, MessageType.Narrative);
			}
		}
		else
		{
			TextEngine.Display(Dialogs.FountainNotPresent, MessageType.Narrative);
		}
	}
}

//#################################################
// STRUCTS
public interface IRoom
{
	public Coordinate Coordinate { get; init; }
	public RoomType Type { get; init; }
}
public class Room : IRoom
{
	public Coordinate Coordinate { get; init; }
	public RoomType Type { get; init; }

	public Room(int x, int y, RoomType type)
	{
		Coordinate  = new Coordinate(x, y);
		Type = type;
	}
}
public class FountainRoom : IRoom
{
	public Coordinate Coordinate { get; init; }
	public RoomType Type { get; init; } = RoomType.Fountain;
	public bool IsActive { get; set; } = false;
	public bool Visited { get; set; } = false;

	public FountainRoom(int x, int y)
	{
		Coordinate = new Coordinate(x, y);
	}
}
public record struct SenseInfo(Direction Direction, IRoom Room);
public struct Coordinate
{
	public int X;
	public int Y;

	public Coordinate(int x, int y)
	{
		X = x;
		Y = y;
	}
}
// ENUMS
public enum RoomType { None, Entry, Fountain, Null}
public enum Direction { Current, North, East, South, West}
