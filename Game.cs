using System;
using System.Media;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics.SymbolStore;
using System.ComponentModel;
using System.Threading;
using System.Xml;
using System.Globalization;
using System.Xml.Linq;
using static System.Net.Mime.MediaTypeNames;
using System.Text.RegularExpressions;
using System.Runtime.ConstrainedExecution;
using System.Collections;
using System.Linq;
using System.CodeDom;

namespace DungeonExplorer
{
    public class Game
    {
        private PlayerManager playerManager;
        public string workingDir { get; set; }
        public string curDir { get; set; }
        public static string artDir { get; set; }
        public static string textDir {  get; set; }
        public Game()
        {
            // Initialise directory variables
            workingDir = Environment.CurrentDirectory;
            curDir = Directory.GetParent(workingDir).Parent.FullName;
            artDir = curDir + "\\assets\\art\\";
            textDir = curDir + "\\assets\\data\\";

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
        /// Method <c>GetArt></c> Retrieves an art file from the assets folder.
        /// </summary>
        /// <param name="file"></param>
        /// <returns> 
        /// The art file
        /// </returns>
        /// <exception cref="FileNotFoundException">
        /// Thrown when the art file is not found.
        /// </exception>
        public static string GetArt(string file)
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

        /// <summary>
        /// Method <c>PopulateField</c> replaces dummy text in an art file with the supplied parameter.
        /// </summary>
        /// <param name="art">the art file.</param>
        /// <param name="field">the field in the art file to populate.</param>
        /// <param name="para">the data to populate the field.</param>
        /// <returns> 
        /// A new art file 
        /// </returns>
        public static string PopulateField(string art, string field, string para)
        {
            para = para + new string(' ', field.Length - para.Length);
            return art.Replace(field, para);
        }
        
        /// <summary>
        /// Method <c>GetDialogue</c> writes dialogue to the console.
        /// </summary>
        /// <param name="header_name">the XElement with name <c>header_name</c> storing the dialogue to be retrieved.</param>
        /// <exception cref="FileNotFoundException">
        /// Thrown when the dialogue XML file is not found.
        /// </exception>
        public static void GetDialogue(string header_name)
        {
            string txt = "";
            string path = textDir + "dialogue.xml";
            if (File.Exists(path))
            {
                try
                {   
                    // TODO: Add further exception handling for case of Element("Text") returning null.
                    txt = XElement.Load(path).Element(header_name).Element("text").Value.Trim();
                }
                catch
                {
                    throw new NullReferenceException("Either dialogue does not exist or is not properly formatted");
                }
            }
            else
            {
                throw new FileNotFoundException("File does not exist...");
            }

            WriteDialogue(txt);
        }

        /// <summary>
        /// Method <c>StripText</c> splits input text into new lines.
        /// </summary>
        /// <param name="txt"></param>
        /// <returns>
        /// An array of string values, each representing a line of the input text.
        /// </returns>
        public static string[] StripText(string txt)
        {   
            // Split the text using either new line characters as delimiters.
            string[] line = txt.Split(new[] { '\r', '\n' });
            for (int i = 0; i < line.Length; i++)
            {
                line[i] = line[i].Trim();
            }
            return line;
        }

        /// <summary>
        /// Method <c>WriteDialogue</c> outputs text with a delay for ease of reading.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="wait"></param>
        public static void WriteDialogue(string txt, int wait = 10)
        {
            string[] dialogue = StripText(txt);
            int charDelay = 35;
            foreach(string s in dialogue)
            {
                if (s.Length > 1)
                {
                    foreach (char c in s)
                    {
                        //System.Threading.Thread.Sleep(charDelay);
                        Console.Write(c);
                    }
                    Console.Write('\n');
                    System.Threading.Thread.Sleep(wait);
                }
            }
        }

        /// <summary>
        /// Method <c>ValidateInputSelection</c> requests user input and checks it against a set of possible options.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="options"></param>
        /// <returns>
        /// The users validated input.
        /// </returns>
        public static string ValidateInputSelection(string message, string[] options = null)
        {   
            // If the optional parameter "options" is not defined in the function call
            // it defaults to provide the player with a yes/no choice.
            options = options ?? new string[] {"Y", "N"};
            string sel = "";
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
            Console.WriteLine(GetArt("intro"));
            MainMenu();
        }

        private void MainMenu()
        {
            Console.WriteLine(GetArt("menu"));

            string[] options = new string[] { "1", "2", "3" };
            string sel = ValidateInputSelection("Please enter 1, 2 or 3 from the menu options: ", options);

            // For debugging purposes, the first menu option "~ hit bed ~" will run when either 1,2 or 3 is entered by the user.
            InitialiseGame();
            //GameLoop();
        }

        private void InitialiseGame()
        {
            Console.WriteLine(GetArt("start"));
            GetDialogue("introduction-1");

            Dictionary<string, int> times = new Dictionary<string, int>
            {
                {"10:10", 80},
                {"10:45", 75},
                {"11:30", 70},
                {"12:00", 65},
                {"12:05", 60},

            };
            string time = ValidateInputSelection("what time do you see? (10:10, 10:45, 11:30, 12:00, 12:05) ", times.Keys.ToArray());
            playerManager.player.Energy.Value = times[time];

            Console.WriteLine("\n*New attribute unlocked: energy*\n");

            GetDialogue("introduction-2");

            Console.WriteLine(GetArt("play"));

            GetDialogue("introduction-3");
            GetDialogue("introduction-4");

            Console.WriteLine("\n*New attribute unlocked: resilience*\n");

            GetDialogue("introduction-5");
            GetDialogue("introduction-6");

            GameCustomisation();
        }

        /// <summary>
        /// Method <c>GameCustomisation</c> allows the user to personalise their experience with their own name and pronouns.
        /// </summary>
        private void GameCustomisation()
        {
            GetDialogue("introduction-7");

            playerManager.player.SetName();
            string id = GetArt("id");
            id = PopulateField(id, "{name~~~~~~~~~~~~~~~~~~~~~~~~~}", playerManager.player.Name);
            string pronouns = (playerManager.player.myPronouns.GetSubject() + "/" + playerManager.player.myPronouns.GetObject() + "/" + playerManager.player.myPronouns.GetPossessive());
            id = PopulateField(id, "{pronouns~~~~~~~~~~~~~~~~~}", pronouns);
            Console.WriteLine(id);

            GetDialogue("introduction-8");

            GameLoop();
        }

        private void GameLoop()
        {
            RoomManager roomManager = new RoomManager(playerManager);

            bool playing = true;
            while (playing)
            {
                roomManager.Update();
            }
        }
    }
}