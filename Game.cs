using System;
using System.Media;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics.SymbolStore;
using System.ComponentModel;

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
        private string ValidateInputSelection(string message, List<string> options)
        {
            string sel = null;
            bool validInput = false;
            while (!validInput)
            {
                Console.Write(message);
                sel = Console.ReadLine();
                if (options.Contains(sel))
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

            List<string> options = new List<string> { "1", "2", "3" };
            string sel = ValidateInputSelection("Please enter 1, 2 or 3 from the menu options: ", options);
            
            // For debugging purposes, the first menu option "~ hit bed ~" will run when either 1,2 or 3 is entered by the user.
            InitialiseGame();
        }

        private void InitialiseGame()
        {
            bool skipTutorial = false;
            Console.WriteLine(GetText("play"));
            Console.WriteLine("Enter SKIP to skip the tutorial");
            skipTutorial = (Console.ReadLine().Equals("SKIP"));

            Console.WriteLine(GetText("start"));

            if (skipTutorial)
            {
                GameCustomisation();
            }
            else
            {
                StartTutorial();
            }

        }
        
        private void StartTutorial()
        {
            //Console.WriteLine(GetText("tutorial"));
            GameCustomisation();
        }

        private void GameCustomisation()
        {
            player.PlayerState();
            Console.WriteLine(player.InventoryContents());
        }
        private void GameLoop()
        {

        }
    }
}