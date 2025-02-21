using System;
using System.Media;

namespace DungeonExplorer
{
    internal class Game
    {
        private Player player;
        private Room currentRoom;

        public Game()
        {
            // Initialize the game with one room and one player
            

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
            Console.WriteLine(@"Welcome to ""...""");
            MainMenu();
        }

        private void MainMenu()
        {
            Console.WriteLine(@"Settings menu: ""...""");
            InitialiseGame();
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