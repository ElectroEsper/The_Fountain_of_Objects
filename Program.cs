using System;
using System.Collections;

namespace CustomExtensions
{
    public static class Extensions
    {

        public static T[] AddTo<T>(this T[] array, T item)
        {
            T[] newArray = new T[array.Length + 1];
            Array.Copy(array, newArray, array.Length);
            newArray[array.Length] = item;
            return newArray;
        }

        public static void RemoveAt<T>(this T[] array, int index)
        {
            T[] newArray = new T[array.Length - 1];
            if (index > 0)
                Array.Copy(array, 0, newArray, 0, index);
            if (index < array.Length - 1)
                Array.Copy(array, index + 1, newArray, index, array.Length - 1);
        }
    }
}


namespace The_Fountain_of_Objects
{
    using CustomExtensions;
    using The_Fountain_of_Objects;

    public static partial class Program
    {
        public static void Main()
        {
            Game game = new Game();
            game.Run();
        }
    }

    // GAME CLASS
    public class Game
    {

        public Game()
        {
            // Do things...


        }

        public void Run()
        {
            // Start game...
            MainMenu();
        }

        public void MainMenu()
        {
            TextEngine.Display(Dialogs.HorizontalLine, MessageType.Neutral);
            TextEngine.Display("Welcome to The Fountain of Objects!", MessageType.Neutral);
            TextEngine.Display(Dialogs.HorizontalLine, MessageType.Neutral);
            TextEngine.Display("", MessageType.Neutral);
            string[] menus = new string[]
            {
                "New Game",
                "Options (Does Nothing)",
                "Exit"
            };
            TextEngine.DisplayList(menus, MessageType.Question);
            TextEngine.DisplayInline("\nEnter your action:", MessageType.Neutral);

            string? input = TextEngine.Input();
            switch (input)
            {
                case "1":
                    NewGame();
                    break;
                case "2":
                    //Options();
                    break;
                case "3":
                    return;
                default:
                    break;
            };
        }

        public void NewGame()
        {
            Console.Clear();
            TextEngine.Display(Dialogs.HorizontalLine, MessageType.Neutral);
            TextEngine.Display("Choose the cavern's size:", MessageType.Narrative);
            TextEngine.Display("", MessageType.Narrative);
            TextEngine.DisplayList(new string[] { "small", "medium", "large" }, MessageType.Question);
            TextEngine.DisplayInline("\nEnter your choice:", MessageType.Neutral);
            string? input = TextEngine.Input();
            switch (input)
            {
                case "1":
                    Dungeon.Init(4, 4);
                    new Trap(0, 1);
                    break;
                case "2":
                    Dungeon.Init(6, 6);
                    break;
                case "3":
                    Dungeon.Init(8, 8);
                    break;
                default:
                    break;
            };
            //Console.Clear();


            GameLoop();
        }

        public void GameLoop()
        {
            Player player = new Player();
            bool isPlaying = true;
            Intro();

            while (player.Alive && !GameState.GameOver)
            {
                // Player senses environment
                TextEngine.Display(Dialogs.HorizontalLine, MessageType.Neutral);
                player.Sense();

                // Wait for player's input...
                player.Input();

                // Apply player's command
                player.Run();

                // Handle NPCs
                ProcessNPCs();
                CheckWinConditions(player);
            }
        }

        void CheckWinConditions(Player player)
        {
            GameState.GameOver = GameState.IsFountainActive && (player.Pos.X == 0 && player.Pos.Y == 0);
        }

        void ProcessNPCs()
        {
            if (GameState.Enemies.Length == 0) return;
			ICanAttack[] EntitiesThatAttack = new ICanAttack[0];
            foreach (GameObject gameObject in GameState.Enemies)
            {
                if (gameObject as ICanAttack != null) { EntitiesThatAttack.AddTo((ICanAttack)gameObject); }

            }

            foreach (ICanAttack attacker in EntitiesThatAttack)
            {
                attacker.Attack(attacker.SearchEnemy());
            }
        }

        void Intro()
        {
            TextEngine.Display(Dialogs.Intro, MessageType.Narrative);
            TextEngine.Display(Dialogs.HorizontalLine, MessageType.Neutral);
            TextEngine.Display("", MessageType.Neutral);
        }
    }

    public static class GameState
    {
        public static GameObject[] Enemies { get; private set; }
        public static Player[] Players { get; private set; }
        public static bool IsFountainActive { get; private set; }

        public static void AddEnemy(GameObject gameObject) { Enemies = Enemies.AddTo(gameObject); }
        public static void RemoveEnemy(GameObject gameObject) { }

        public static void AddPlayer(Player player) { Players = Players.AddTo(player); }

        public static void ActivateFountain() { IsFountainActive = true; }
        public static void DeactivateFountain() { IsFountainActive = false; }

        public static string DeathMessage { get; set; }
        public static bool GameOver { get; set; } = false;

    }

    //#################################################
    // STATIC CLASSES
    public static class Settings
    {
        public static int MaxCharPerLine = 100;
    }
    public static class Dungeon
    {
        // Made it a Static class, figured it should be available from anywhere???
        public static Room[,] Rooms { get; private set; }
        public static int Width { get; private set; }
        public static int Height { get; private set; }
        public static bool FountainIsActive { get; private set; }

        public static void Init(int x, int y)
        {
            // Initialize fields and array...
            Rooms = new Room[x, y];
            Width = x;
            Height = y;

            // Generate basic rooms then set entry and fountain
            Generate(x, y);

            SetEntry(0, 0);
            SetFountain(0, 2);
        }

        public static Room[] GetAdjacentRooms(int x, int y)
        {
            // Only 5 entries; 4 directions to move to and where the player stands... 
            Room[] output = [];

            // If room is out of bounds, make a variant with a null type.
            output = output.AddTo(Rooms[x, y]);
            if (IsInBounds(x, y + 1)) output = output.AddTo(Rooms[x, y + 1]);
            if (IsInBounds(x + 1, y)) output = output.AddTo(Rooms[x + 1, y]);
            if (IsInBounds(x, y - 1)) output = output.AddTo(Rooms[x, y - 1]);
            if (IsInBounds(x - 1, y)) output = output.AddTo(Rooms[x - 1, y]);

            return output;
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
                    Rooms[_x, _y] = new Room(_x, _y);
                }
            }
        }
        private static void SetEntry(int x, int y)
        {
            //Rooms[x, y] = new Room(x, y, new GameObject[] { new Entry(x, y) });
            new Entry(x, y);
        }
        private static void SetFountain(int x, int y)
        {
            Rooms[x, y] = new Room(x, y, new GameObject[] { new Fountain(x, y) });
        }
    }
    public static class TextEngine
    {
        // Your job is to take in events and display text relevant to said events

        public static string Input()
        {
            //DisplayInline(Dialogs.InputPrompt, MessageType.Question);
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
            Console.WriteLine($"{Truncate(message, Settings.MaxCharPerLine)}");
        }

        public static void Display(string[] message, ConsoleColor color) // TODO
        {
            SetColor(color);
            string text = "";
            foreach (string s in message)
            {
                text += s;
            }
            Console.WriteLine($"{Truncate(text, Settings.MaxCharPerLine)}");
        }

        public static void DisplayInline(string message, ConsoleColor color)
        {
            SetColor(color);
            Console.Write(message);
        }

        public static void DisplayList(string[] message, ConsoleColor color)
        {
            SetColor(color);
            for (int i = 1; i <= message.Length; i++)
            {
                Console.WriteLine($"{i}- {message[i - 1]}");
            }
        }

        private static string Truncate(string text, int lineLenght)
        {
            string output = "";
            string line = "";
            if (text.Length <= lineLenght) return text;

            for (int c = 1; c <= text.Length; c++)
            {
                // If c is same value as truncated reference...
                if (c % lineLenght == 0)
                {
                    // Add current line to output, then reset line...
                    //
                    output += $"{line}\n";
                    line = "";
                }

                line += text[c - 1];
            }

            return output;
        }
    }

    public static class MessageType
    {
        public static ConsoleColor Narrative = ConsoleColor.Magenta;
        public static ConsoleColor Sense = ConsoleColor.Yellow;
        public static ConsoleColor Player = ConsoleColor.Cyan;
        public static ConsoleColor Question = ConsoleColor.White;
        public static ConsoleColor Positive = ConsoleColor.Green;
        public static ConsoleColor Negative = ConsoleColor.Red;
        public static ConsoleColor Neutral = ConsoleColor.Gray;
        //public static ConsoleColor Important = ConsoleColor.
    }

    public static class Teams
    {
        public static int WorldObject = 0;
        public static int Player = 1;
        public static int Trap = 2;
        public static int Monster = 3;
    }
    public static class Dialogs
    {
        public static string Intro = "You have made your way to the Cavern of Objects, high atop jagged mountains. Within these caverns" +
                                     "lies the Fountain of Objects, the one-time source of the River of Objects that gave life to this entire" +
                                     "island. By returning the Heart of Object-Oriented Programming —the gem you received from Simula" +
                                     "after arriving on this island— to the Fountain of Objects, you can repair and restore the fountain to its" +
                                     "former glory." +
                                     "\n" +
                                     "The cavern is a grid of rooms, and no natural or human-made light works within due to unnatural" +
                                     "darkness. You can see nothing, but you can hear and smell your way through the caverns to find the" +
                                     "Fountain of Objects, restore it, and escape to the exit." +
                                     "\n";

        public static string InputPrompt = "What do you do?: ";
        public static string PrefixSameRoom = "In this room,";
        public static string PrefixAdjacentRooms = "of you,";
        public static string FountainOff = "water dripping. The Fountain of Objects.";
        public static string FountainOn = "flowing water. The Fountain of Objects.";
        public static string InvalidMove = "A heavy *Thud* echoes. You've hit a wall.";
        public static string AmarokAdjacent = "something putrid, an Amarok is Nearby";
        public static string AmarokDead = "something heavy, the putrid corpse of an Amarok.";
        public static string AmarokAttack = "Before you have time to react, you are struck down by an Amarok.";
        public static string HorizontalLine =
            "-----------------------------------------------------------------------------";

        public static string FountainOffActivate =
            "As you feel blindly in front of you, you touch something wet. You smile," +
            "knowing well this is it, the Fountain of Objects." +
            "\n" +
            "You, clumsily, manage to place the Heart of Objects in the right spot." +
            "As if to reward you, the slow drip of water awakens into a strong, continuous" +
            "flow." +
            "\n" +
            "The Fountain is now active.";

        public static string FountainOnActivate = "You search you pockets, looking for the Heart of Objects. Why you're doing" +
                                                  "this is unclear, as you know well that you have placed the heart into " +
                                                  "its socket yourself.";
        public static string FountainOffDeactivate = "You're searching for something, the Heart of Object. A lapse in" +
                                                     "mental clarity perhaps?" +
                                                     "\n" +
                                                     "What is certain, is that your hand returns empty, if a bit more wet" +
                                                     "than it was.";
        public static string FountainOnDeactivate = "For reasons that are yours only, you reach back for the fountain, and" +
                                                    "reach for the Heart of Objects. You contemplate for a moment the weight" +
                                                    "of your wish, before removing the artifact from its receptacle." +
                                                    "\n" +
                                                    "The Fountain of Objects, deprived of its core, returns to dormancy.";
        public static string FountainNotPresent = "You search for the fountain, yet find nothing resembling it.";

        public static string EmptyRoom = " nothing catches your attention.";
        public static string EntryRoom = "light coming from outside, through the cavern's entrance.";
        public static string Trap = "a draft of air, a pitfall is nearby.";
        public static string NullRoom = " a wall.";
        public static string Audio = "You hear";
        public static string Smell = "You smell";
        public static string Touch = "You feel";
		public static string DeathByTrap = "You expected firm ground, instead your feet gave into the void. Moments later, your life is forfeited, as you lay there, impaled...";
    }




    //#################################################
    // GAME OBJECTS CLASSES
    public abstract class GameObject : IPerceptible
    {
        public IVector2 Pos { get; set; } = new IVector2();
        public int InRoomIndex { get; set; }
        public abstract int Team { get; init; }
        public abstract bool LocalOnly { get; init; }
        internal void Spawn()
        {
            Console.WriteLine($"Spawn:{ToString()} @ {Pos.X},{Pos.Y}");
            Dungeon.Rooms[Pos.X, Pos.Y].Enter(this);
            Console.WriteLine($"Room Ent.: {Dungeon.Rooms[Pos.X, Pos.Y].Entities.Length}");
        }
        public abstract Stimuli Emit();
        //public override string ToString() => ClassName;

    }
    public class Player : GameObject, IController, IAlive, ICanInteract
    {
        public string NameSingular { get; init; }
        public string NamePlural { get; init; }
        public bool Alive { get; private set; } = true;
        public override int Team {get; init; } = Teams.Player;
        public override bool LocalOnly { get; init; } = true;
        public ICommand? Command { get; set; }
        public Player()
        {
            Pos.X = 0;
            Pos.Y = 0;

            Spawn();
        }

        public void Run()
        {
            Command?.Run(this);
        }

        public void Death(string deathMessage)
        {
            GameState.DeathMessage = deathMessage;
            Alive = false;
        }

        public void Interact()
        {
            throw new NotImplementedException();
        }

        public void Input()
        {
            TextEngine.DisplayInline($"({Pos.X},{Pos.Y}){Dialogs.InputPrompt}", MessageType.Question);
            string? input = TextEngine.Input();

            Command = input switch
            {
                "move north" => new MoveNorth(),
                "move east" => new MoveEast(),
                "move south" => new MoveSouth(),
                "move west" => new MoveWest(),
                "fountain activate" => new TurnOn(),
                "fountain deactivate" => new TurnOff(),
                "shoot north" => null,
                "shoot east" => null,
                "shoot south" => null,
                "shoot west" => null,
                _ => null
            };


        }

        public override Stimuli Emit()
        {
            return null;
        }
        public void Sense()
        {
            Stimuli[] stimuli = new Stimuli[0];
            ICanInteract[] interactables = new ICanInteract[0];
            Room[] rooms;
            rooms = Dungeon.GetAdjacentRooms(Pos.X, Pos.Y);
            foreach (Room room in rooms)
            {
                if (room.Entities?.Length == null) continue;
                foreach (GameObject? gameObject in room?.Entities)
                {
                    // if stimuli is localOnly and is not in same pos as player 
                    if (gameObject.LocalOnly && (gameObject.Pos.X != Pos.X || gameObject.Pos.Y != Pos.Y))
                    {
                        TextEngine.Display($"{gameObject.ToString()}", MessageType.Negative);
                        continue;
                    }
                    else
                    {
                        stimuli = stimuli.AddTo(gameObject.Emit());
                    }

                    ICanInteract interactableGameObject = gameObject as ICanInteract;
                    if ((gameObject.Pos.X == Pos.X && gameObject.Pos.Y == Pos.Y) && interactableGameObject != null)
                    {
                        interactables = interactables.AddTo((ICanInteract)gameObject);
                    }
                }
            }

            string[] text = new string[0];
            string[] processed = new string[0];
            if (stimuli.Length > 0)
            {
                foreach (Stimuli? stim in stimuli)
                {
                    if (stim == null) continue;
                    if (Array.Exists(processed, s => s == stim.Class)) continue;

                    string message = $"{typeof(Dialogs).GetField(stim.Type.ToString()).GetValue(null)} {stim.Dialog}";
                    processed = processed.AddTo(stim.Class);
                    text = text.AddTo(message);
                }

                TextEngine.Display(text, MessageType.Narrative);
            }

            if (interactables.Length > 0)
            {
                string textInter = "There is ";
                for (int i = 0; i < interactables.Length; i++)
                {
                    textInter += $"a {interactables[i].NameSingular}";
                    if (i < interactables.Length - 2)
                    {
                        textInter += ", ";
                    }

                    if (i == interactables.Length - 2)
                    {
                        textInter += " and ";
                        if (i == interactables.Length - 1)
                        {
                            textInter += ".";
                        }
                    }
                }

                TextEngine.Display(textInter, MessageType.Narrative);
            }
        }
    }

    public class Trap : GameObject, ICanAttack
    {
        public override bool LocalOnly { get; init; }
        public override int Team {get; init; } = Teams.Trap;
        public Trap(int x, int y)
        {
            Pos.X = x;
            Pos.Y = y;
            LocalOnly = false;

            Spawn();
			GameState.AddEnemy((GameObject)this);
        }
        public void Attack(IAlive target)
        {
            target.Death(Dialogs.DeathByTrap);
        }
        public IAlive SearchEnemy()
        {
            IAlive[] potentialTargets = new IAlive[0];
            foreach (GameObject gameObject in Dungeon.Rooms[Pos.X, Pos.Y].Entities)
            {
                if (gameObject == this) continue;
                if (gameObject.Team != Teams.WorldObject) potentialTargets = potentialTargets.AddTo<IAlive>((IAlive)gameObject);
            }

            return potentialTargets[0];
        }

        public override Stimuli Emit()
        {
            return new Stimuli(Dialogs.Trap, this.ToString(), StimuliType.Touch);
        }
    }

    public class Amarok : GameObject, ICanAttack, IAlive
    {
        public override bool LocalOnly { get; init; } = false;
        public override int Team {get; init; } = Teams.Monster;
        public Amarok(int x, int y)
        {
            Pos.X = x;
            Pos.Y = y;
            Spawn();
			GameState.AddEnemy((GameObject)this);
        }

        public void Attack(IAlive target)
        {
            throw new NotImplementedException();
        }

        public IAlive SearchEnemy()
        {
            throw new NotImplementedException();
        }

        public override Stimuli Emit()
        {
            return new Stimuli(Dialogs.AmarokAdjacent, this.ToString(), StimuliType.Smell);
        }

        public void Death(string deathMessage)
        {
            Dungeon.Rooms[Pos.X, Pos.Y].Exit(this);
            AmarokDead corpse = new AmarokDead(Pos.X, Pos.Y);
        }
    }

    public class AmarokDead : GameObject, ICanInteract
    {
        public override int Team {get; init; } = Teams.WorldObject;
        public string NameSingular { get; init; } = "Amarok";
        public string NamePlural { get; init; } = "Amaroks";
        public override bool LocalOnly { get; init; } = true;

        public AmarokDead(int x, int y)
        {
            Pos.X = x;
            Pos.Y = y;
            Spawn();
        }

        public override Stimuli Emit()
        {
            return new Stimuli(Dialogs.AmarokDead, this.ToString(), StimuliType.Touch);
        }

        public void Interact()
        {
        }
    }

    public class Entry : GameObject
    {
        public override bool LocalOnly { get; init; } = true;
        public override int Team {get; init; } = Teams.WorldObject;
        public Entry(int x, int y)
        {
            Pos.X = x;
            Pos.Y = y;
            LocalOnly = true;
            Spawn();
        }

        public override Stimuli Emit()
        {
            return new Stimuli(Dialogs.EntryRoom, this.ToString(), StimuliType.Touch);
        }
    }
    public class Fountain : GameObject, ICanInteract
    {
        public string NameSingular { get; init; } = "Fountain";
        public string NamePlural { get; init; } = "Fountains";
        public override int Team {get; init; } = Teams.WorldObject;
        public override bool LocalOnly { get; init; } = true;
        public bool Active { get; set; } = false;

        public Fountain(int x, int y)
        {
            Pos.X = x;
            Pos.Y = y;
            Spawn();
        }
        public override Stimuli Emit()
        {
            return Active ? new Stimuli(Dialogs.FountainOn, this.ToString(), StimuliType.Audio) : new Stimuli(Dialogs.FountainOff, this.ToString(), StimuliType.Audio);
        }

        public void Interact()
        {
        }

        public void Enable()
        {
            Active = true;
            GameState.ActivateFountain();
        }
        public void Disable()
        {
            Active = false;
            GameState.DeactivateFountain();
        }
    }

    public interface IAlive
    {
        public void Death(string deathMessage);
    }

    public interface IAi
    {
        public ICommand? Command { get; set; }
        public void Run(GameObject gameObject);
    }

    public interface ICanAttack
    {
        public void Attack(IAlive? target);
        public IAlive SearchEnemy();
    }

    public interface ICanInteract
    {
        public string NameSingular { get; init; }
        public string NamePlural { get; init; }
        public void Interact() { }

    }

    public interface IController
    {
        public void Input();
    }

    public interface IPerceptible
    {
        public bool LocalOnly { get; init; }
        public Stimuli Emit();
    }

    public class IVector2
    {
        public int X { get; set; }
        public int Y { get; set; }

    }
    //#################################################
    // COMMANDS
    public interface ICommand
    {
        void Run(GameObject gameObject);
    }
    public abstract class Command : ICommand
    {
        public abstract void Run(GameObject gameObject);
        public bool IsValidMove(int x, int y)
        {
            if (x < 0 || x >= Dungeon.Width)
            {
                TextEngine.Display("\nYou hit a wall, making an unsurprising *THUD* doing so." +
                                                                 "\nNothing feels different, apart from a bruised ego.", MessageType.Narrative); return false;
            }
            if (y < 0 || y >= Dungeon.Height)
            {
                TextEngine.Display("\nYou hit a wall, making an unsurprising *THUD* doing so." +
                                                                              "\nYou feel fine, bar a scratch on your pride.", MessageType.Narrative); return false;
            }

            return true;
        }
    }
    public class MoveNorth : Command
    {
        public override void Run(GameObject gameObject)
        {
            if (!IsValidMove(gameObject.Pos.X, gameObject.Pos.Y + 1)) return;
            gameObject.Pos.Y += 1;
        }
    }
    public class MoveSouth : Command
    {
        public override void Run(GameObject gameObject)
        {
            if (!IsValidMove(gameObject.Pos.X, gameObject.Pos.Y - 1)) return;
            gameObject.Pos.Y -= 1;
        }
    }
    public class MoveEast : Command
    {
        public override void Run(GameObject gameObject)
        {
            if (!IsValidMove(gameObject.Pos.X + 1, gameObject.Pos.Y)) return;
            gameObject.Pos.X += 1;
        }
    }
    public class MoveWest : Command
    {
        public override void Run(GameObject gameObject)
        {
            if (!IsValidMove(gameObject.Pos.X - 1, gameObject.Pos.Y)) return;
            gameObject.Pos.X -= 1;
        }
    }

    public class TurnOn : Command
    {
        public override void Run(GameObject gameObject)
        {
            Fountain? fountain = GetFoutain(gameObject);
            if (fountain == null) { TextEngine.Display(Dialogs.FountainNotPresent, MessageType.Narrative); return; }
            switch (fountain.Active)
            {
                case true:
                    TextEngine.Display(Dialogs.FountainOnActivate, MessageType.Narrative);
                    break;

                case false:
                    TextEngine.Display(Dialogs.FountainOffActivate, MessageType.Positive);
                    fountain.Enable();
                    break;
            }
        }

        private Fountain? GetFoutain(GameObject searcher)
        {
            Room[] rooms;
            rooms = Dungeon.GetAdjacentRooms(searcher.Pos.X, searcher.Pos.Y);
            foreach (Room room in rooms)
            {
                if (room.Entities?.Length == null) continue;
                foreach (GameObject? gameObject in room?.Entities)
                {
                    if ((gameObject.Pos.X == searcher.Pos.X && gameObject.Pos.Y == searcher.Pos.Y) && gameObject as Fountain != null)
                    {
                        return (Fountain)gameObject;
                    }
                }
            }

            return null;
        }
    }

    public class TurnOff : Command
    {
        public override void Run(GameObject gameObject)
        {
            Fountain? fountain = GetFoutain(gameObject);
            if (fountain == null) { TextEngine.Display(Dialogs.FountainNotPresent, MessageType.Narrative); return; }
            switch (fountain.Active)
            {
                case true:
                    TextEngine.Display(Dialogs.FountainOnDeactivate, MessageType.Negative);
                    fountain.Disable();
                    break;

                case false:
                    TextEngine.Display(Dialogs.FountainOffDeactivate, MessageType.Narrative);
                    break;
            }
        }

        private Fountain? GetFoutain(GameObject searcher)
        {
            Room[] rooms;
            rooms = Dungeon.GetAdjacentRooms(searcher.Pos.X, searcher.Pos.Y);
            foreach (Room room in rooms)
            {
                if (room.Entities?.Length == null) continue;
                foreach (GameObject? gameObject in room?.Entities)
                {
                    if ((gameObject.Pos.X == searcher.Pos.X && gameObject.Pos.Y == searcher.Pos.Y) && gameObject as Fountain != null)
                    {
                        return (Fountain)gameObject;
                    }
                }
            }

            return null;
        }
    }

    public class Interact : Command
    {
        public override void Run(GameObject gameObject)
        {

        }
    }


    //#################################################
    // STRUCTS
    public class Room
    {
        public IVector2 Pos { get; init; } = new IVector2();
        public GameObject[]? Entities { get; private set; }
        public Room(int x, int y)
        {
            Pos.X = x;
            Pos.Y = y;
            Entities = new GameObject[0];
        }
        public Room(int x, int y, GameObject[] gameObjects)
        {
            Pos.X = x;
            Pos.Y = y;
            Entities = gameObjects;
        }
        public void Enter(GameObject gameObject)
        {
            Entities = Entities.AddTo<GameObject>(gameObject);
        }
        public void Exit(GameObject gameObject)
        {
            Entities.RemoveAt<GameObject>(gameObject.InRoomIndex);
        }
    }

    public record Stimuli(string Dialog, string Class, StimuliType Type);

    // ENUMS
    public enum Direction { Current, North, East, South, West }
    public enum StimuliType { Audio, Smell, Touch }
}
