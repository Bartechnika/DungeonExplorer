using System;
using System.IO;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Linq;
using System.Diagnostics;
using System.Xml.Schema;
using System.ComponentModel;

namespace DungeonExplorer
{
    public class Game
    {
        public static PlayerManager playerManager;
        public string workingDir { get; set; }
        public string curDir { get; set; }
        public static string artDir { get; set; }
        public static string textDir {  get; set; }
        public static string locDir { get; set; }
        public static string menuDir { get; set; }
        public static string monsterDir { get; set; }
        public static string abilitiesDir { get; set; }

        public Game()
        {
            // Initialise directory variables
            workingDir = Environment.CurrentDirectory;
            curDir = Directory.GetParent(workingDir).Parent.FullName;
            artDir = curDir + "\\assets\\art\\";
            textDir = curDir + "\\assets\\data\\";
            locDir = "\\location\\";
            menuDir = "\\main_menu\\";
            monsterDir = "\\monsters\\";
            abilitiesDir = "\\abilities\\";


            // Initialize the game with one room and one player
            playerManager = new PlayerManager();
        }

        /* --- Utility Functions ---
         * GetArt()
         * PopulateField()
         * GetDialogue()
         * StripText()
         * WriteDialogue()
         * ValidUserInput()
         */

        /// <summary>
        /// Method <c>ValidateInputSelection</c> requests user input and checks it against a set of possible options.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="options"></param>
        /// <returns>
        /// The users validated input.
        /// </returns>
        public static string ValidateInputSelection(string[] options = null)
        {   
            // If the optional parameter "options" is not defined in the function call
            // it defaults to provide the player with a yes/no choice.
            options = options ?? new string[] {"y", "n"};

            // Options menu is automatically converted to lowercase using LINQ expression
            options = options.Select(s => s.ToLower()).ToArray();

            string sel = "";
            bool validInput = false;
            while (!validInput)
            {
                Console.Write("-> ");
                sel = Console.ReadLine().ToLower();
                if (Array.IndexOf(options, sel) > -1)
                    validInput = true;
                else
                    Console.WriteLine("User input not in range of selection! Please try again.\n");
            }
            return sel;
        }

        public static string SelectOption(string message, Dictionary<string, string> ops)
        {
            Console.WriteLine($"\n{message}\n");
            foreach(KeyValuePair<string, string> op in ops)
            {
                Console.WriteLine($"({op.Key}) {op.Value}");
            }
            return ValidateInputSelection(ops.Keys.ToArray());
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
            Console.Write(UI.GetArt("title_screen"));
            Console.ReadKey();
            UI.GetMainMenu();
            InitialiseGame();
        }


        private void InitialiseGame()
        {
            UI.ClearConsole();
            Console.WriteLine(UI.GetArt("start"));
            UI.GetDialogue("introduction-1");

            Dictionary<string, int> times = new Dictionary<string, int>
            {
                {"10:10", 80},
                {"10:45", 75},
                {"11:30", 70},
                {"12:00", 65},
                {"12:05", 60},

            };
            Console.WriteLine("what time do you see? (10:10, 10:45, 11:30, 12:00, 12:05");
            string time = ValidateInputSelection(times.Keys.ToArray());
            playerManager.player.Energy.Value = times[time];

            Console.WriteLine("\n*New attribute unlocked: energy*\n");

            UI.GetDialogue("introduction-2");

            Console.WriteLine(UI.GetArt("play"));

            UI.GetDialogue("introduction-3");
            UI.GetDialogue("introduction-4");

            Console.WriteLine("\n*New attribute unlocked: resilience*\n");

            UI.GetDialogue("introduction-5");
            UI.ClearConsole();
            UI.GetDialogue("introduction-6");

            GameCustomisation();
        }

        /// <summary>
        /// Method <c>GameCustomisation</c> allows the user to personalise their experience with their own name and pronouns.
        /// </summary>
        private void GameCustomisation()
        {
            UI.GetDialogue("introduction-7");

            playerManager.player.SetName();
            string id = UI.GetArt("id");
            id = UI.PopulateField(id, "{name~~~~~~~~~~~~~~~~~~~~~~~~~}", playerManager.player.Name);
            string pronouns = (playerManager.player.myPronouns.GetSubject() + "/" + playerManager.player.myPronouns.GetObject() + "/" + playerManager.player.myPronouns.GetPossessive());
            id = UI.PopulateField(id, "{pronouns~~~~~~~~~~~~~~~~~}", pronouns);
            Console.WriteLine(id);

            UI.GetDialogue("introduction-8");

            GameLoop();
        }

        private void GameLoop()
        {
            GameMap.SetValues(playerManager);
            bool playing = true;
            while (playing)
            {
                GameMap.Update();
            }
        }

        public static void Wait(int seconds)
        {
            System.Threading.Thread.Sleep(seconds*0);
        }
    }
}