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

namespace DungeonExplorer
{
    public class Game
    {
        private Player player;
        private Room currentRoom;
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
            player = new Player("barts", 1);
            player.InventoryContents();
            currentRoom = new Room("train-platform");
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

        public static string[] GetText(string file, string header_name)
        {
            string txt = "";
            string path = textDir + file + ".xml";
            if (File.Exists(path))
            {
                XmlTextReader reader = new XmlTextReader(path);
                while (reader.Read())
                {
                    if (reader.NodeType == XmlNodeType.Element)
                    {
                        if(reader.GetAttribute("name") == header_name)
                        {
                            var innerReader = reader.ReadSubtree();
                            while(innerReader.Read())
                            {   
                                if (innerReader.NodeType == XmlNodeType.Text)
                                {
                                    txt = (innerReader.Value.Trim());
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                throw new FileNotFoundException("File does not exist...");
            }

            string[] line = txt.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
            for (int i = 0; i < line.Length; i++)
            {
                line[i] = line[i].Trim();
            }
            return line;
        }

        // Display dialogue appearing from left to right with a time delay.
        public static void WriteDialogue(string[] message, int wait = 1000)
        {
            int charDelay = 35;
            foreach(string s in message)
            {   
                foreach(char c in s)
                {
                    System.Threading.Thread.Sleep(charDelay);
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
        }

        private void InitialiseGame()
        {
            Console.WriteLine(GetArt("start"));

            string[] txt = GetText("dialogue", "introduction-1");
            WriteDialogue(txt);
            
            string time = ValidateInputSelection("what time do you see? (10:10, 10:45, 11:30, 12:00, 12:05) ", new string[] {"10:10", "10:45", "11:30", "12:00", "12:05"});

            Console.WriteLine(GetArt("play"));

            txt = GetText("dialogue", "introduction-2");
            WriteDialogue(txt);
        }

        private void GameCustomisation()
        {
        }
        private void GameLoop()
        {

        }
    }
}