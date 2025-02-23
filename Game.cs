using System;
using System.Media;
using System.IO;

namespace DungeonExplorer
{
    internal class Game
    {
        private Player player;
        private Room currentRoom;
        public string workingDir { get; set; }
        public string curDir { get; set; }
        public string artDir { get; set; }
        public Game()
        {
            // Initialise directy variables
            workingDir = Environment.CurrentDirectory;
            curDir = Directory.GetParent(workingDir).Parent.FullName;
            artDir = curDir + "\\art\\";

            // Initialize the game with one room and one player
            player = new Player("Barts", 100);
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

        private string GetText(string file)
        {
            string path = artDir + file + ".txt";
            if (File.Exists(path))
            {
                return File.ReadAllText(path);
            }
            else
            {
                return "File does not exist...";
            }
        }

        private void GameIntroduction()
        {
            Console.WriteLine(GetText("intro"));
            MainMenu();
        }

        private void MainMenu()
        {
            Console.WriteLine(GetText("menu"));
            //InitialiseGame();
        }

        private void InitialiseGame()
        {
            bool skipTutorial = false;
            Console.WriteLine(@"Starting a new adventure ""...""");
            Console.WriteLine("Enter SKIP to skip the tutorial");
            try
            {
                skipTutorial = (Console.ReadLine().Equals("SKIP"));
            }
            catch (Exception e)
            {
                Console.Write(e.ToString());
            }

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
            Console.WriteLine(@"Welcome new traveller! ""...""");
            GameCustomisation();
        }

        private void GameCustomisation()
        {
            Console.WriteLine(@"Please adjust your stats: ""...""");
        }
        private void GameLoop()
        {

        }
    }
}