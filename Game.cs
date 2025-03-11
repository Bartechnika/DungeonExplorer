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

namespace DungeonExplorer
{
    public class Game
    {
        private Player player;
        private Room curRoom;
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
            player = new Player();
            player.InventoryContents();
        }

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

        public static string PopulateField(string art, string field, string para)
        {
            para = para + new string(' ', field.Length - para.Length);
            return art.Replace(field, para);
        }

        public static void GetDialogue(string header_name)
        {
            string txt = "";
            string path = textDir + "dialogue.xml";
            if (File.Exists(path))
            {
                txt = XElement.Load(path).Element(header_name).Element("text").Value.Trim();
            }
            else
            {
                throw new FileNotFoundException("File does not exist...");
            }

            WriteDialogue(txt);
        }

        public static string[] StripText(string txt)
        {
            string[] line = txt.Split(new[] { '\r', '\n' });
            for (int i = 0; i < line.Length; i++)
            {
                line[i] = line[i].Trim();
            }
            return line;
        }

        // Display dialogue appearing from left to right with a time delay.
        public static void WriteDialogue(string message, int wait = 1000)
        {
            string[] dialogue = StripText(message);
            int charDelay = 35;
            foreach(string s in dialogue)
            {   
                foreach(char c in s)
                {
                    //System.Threading.Thread.Sleep(charDelay);
                    Console.Write(c);
                }
                Console.Write('\n');
                System.Threading.Thread.Sleep(wait);
            }
        }

        public static string ValidateInputSelection(string message, string[] options = null)
        {   
            // If the optional parameter "options" is not defined in the function call
            // it is set by default to provide the player with a yes/no choice.
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
            //InitialiseGame();
            GameLoop();
        }

        private void InitialiseGame()
        {
            Console.WriteLine(GetArt("start"));
            GetDialogue("introduction-1");
            
            string time = ValidateInputSelection("what time do you see? (10:10, 10:45, 11:30, 12:00, 12:05) ", new string[] {"10:10", "10:45", "11:30", "12:00", "12:05"});
            player.Imagination.Value = 100;

            GetDialogue("introduction-2");
            GetDialogue("introduction-3");

            Console.WriteLine(GetArt("play"));

            GetDialogue("introduction-4");

            Console.WriteLine("\n*New attribute unlocked: hope (HP)*\n");

            GetDialogue("introduction-5");
            GetDialogue("introduction-6");

            GameCustomisation();
        }

        private void GameCustomisation()
        {
            string txt;
            GetDialogue("introduction-7");

            player.SetName();
            string id = GetArt("id");
            id = PopulateField(id, "{name~~~~~~~~~~~~~~~~~~~~~~~~~}", player.Name);
            id = PopulateField(id, "{pronouns~~~~~~~~~~~~~~~~~}", player.myPronouns.GetThird() + "/" + player.myPronouns.GetPossessive());
            Console.WriteLine(id);

            GameLoop();
        }

        private void GameLoop()
        {

            RoomManager roomManager = new RoomManager(player);

            bool playing = true;
            while (playing)
            {
                roomManager.Update();
            }
        }
    }
}