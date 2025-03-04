using System;
using System.Media;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics.SymbolStore;
using System.ComponentModel;
using System.Threading;

namespace DungeonExplorer
{
    public class Game
    {
        private Player player;
        private Room currentRoom;
        public string workingDir { get; set; }
        public string curDir { get; set; }
        static public string artDir { get; set; }
        public Game()
        {
            // Initialise directory variables
            workingDir = Environment.CurrentDirectory;
            curDir = Directory.GetParent(workingDir).Parent.FullName;
            artDir = curDir + "\\art\\";

            // Initialize the game with one room and one player
            player = new Player("barts", 1);
            player.InventoryContents();
        }
        public static string GetText(string file)
        {
            string path = artDir + file + ".txt";
            if (File.Exists(path))
            {
                return File.ReadAllText(path);
            }
            else
            {
                throw new FileNotFoundException("File does not exist...");
            }
        }

        // Display dialogue appearing from left to right with a time delay.
        public static void WriteDialogue(string message, int wait = 1000)
        {
            int charDelay = 35;
            foreach(char c in message)
            {   
                System.Threading.Thread.Sleep(charDelay);
                Console.Write(c);
            }
            Console.Write('\n');
            System.Threading.Thread.Sleep(wait);
        }

        public static string ValidateInputSelection(string message, string[] options = null)
        {   
            // If the optional parameter "options" is not defined in the function call
            // it is set by default to provide the player with a yes/no choice.
            options = options ?? new string[] {"Y", "N"};
            string sel = null;
            bool validInput = false;
            while (!validInput)
            {
                Console.Write(message);
                sel = Console.ReadLine();
                if (Array.IndexOf(options, sel) > -1)
                    validInput = true;
                else
                    Console.WriteLine("User input not in range of selection! Please try again.\n");
            }
            return sel;
        }

        public void Start()
        {
            // Change the playing logic into true and populate the while loop
            bool playing = false;
            while (playing)
            {
                // Code your playing logic here
            }
            GameIntroduction();
        }

        private void GameIntroduction()
        {
            Console.WriteLine(GetText("intro"));
            MainMenu();
        }

        private void MainMenu()
        {
            Console.WriteLine(GetText("menu"));

            string[] options = new string[] { "1", "2", "3" };
            string sel = ValidateInputSelection("Please enter 1, 2 or 3 from the menu options: ", options);
            
            // For debugging purposes, the first menu option "~ hit bed ~" will run when either 1,2 or 3 is entered by the user.
            InitialiseGame();
        }

        private void InitialiseGame()
        {
            Console.WriteLine(GetText("start"));

            // Text to be stored in XML files at later date.
            WriteDialogue("*sporadically typing and deleting lines of code* \"if i use a struct instead of class definition...\"");
            WriteDialogue("oh. i forgot i need to update the function in Game. otherwise, ill finish this off tomorrow.");
            WriteDialogue("*sudden noise souding much louder than it should* \"gah!\" *glances at clock*");
            string time = ValidateInputSelection("what time do you see? (10:10, 10:45, 11:30, 12:00, 12:05) ", new string[] {"10:10", "10:45", "11:30", "12:00", "12:05"});
            WriteDialogue($"i have lectures at 12:00. probably shouldnt stay awake any longer");
            WriteDialogue("*time slips away* \"need to make sure it saves and. and.. the push can wait.\"");
            WriteDialogue("*tucks into bed* *shiver* *curls up tighter with blanket*");
            WriteDialogue("\"no new messages, must... sleep..\"");

            Console.WriteLine(GetText("play"));
        }

        private void GameCustomisation()
        {
        }
        private void GameLoop()
        {

        }
    }
}