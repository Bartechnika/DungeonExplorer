using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Security.Policy;
using DungeonExplorer;

namespace DungeonExplorer
{
    public class Player
    {   
        public string Name { get; private set; }
        public struct pronouns 
        {   
            private string[] Possessives;
            private string[] Thirds; 

            private static Random rnd = new Random();

            public string GetPossessive() => Possessives[rnd.Next(0, Possessives.Length)];
            public string GetThird() => Thirds[rnd.Next(0, Thirds.Length)];

            public pronouns(string[] possessives, string[] thirds)
            {
                if (possessives == null)
                    possessives = new string[] { "/redacted/" };
                if (thirds == null)
                    thirds = new string[] { "/redacted/" };
                Possessives = possessives;  
                Thirds = thirds;
            }
        }

        public pronouns myPronouns;

        public Dictionary<Room, PlayerAttribute> rooms = new Dictionary<Room, PlayerAttribute>();
        public InventoryManager inventoryManager;
        public PlayerAttribute Hope;
        public PlayerAttribute Resilience;
        public PlayerAttribute Creativity;

        public Player(string name, int hope) 
        {
            Name = name;
            Hope = new PlayerAttribute("Hope", "100");
            Resilience = new PlayerAttribute("Resilience", "100");
            Creativity = new PlayerAttribute("Creativity", "100");
            inventoryManager = new InventoryManager();
        }

        public void SetPronouns()
        {
            List<string> possessives = new List<string>();
            List<string> thirds = new List<string>();

            bool addPossessive = true;
            bool addThird = true;

            string next;

            Console.WriteLine("Who... who am i? no. i know...");
            Console.WriteLine("please enter your preferred pronouns ");
            while (addPossessive || addThird)
            {
                string possessive = "";
                string third = "";

                next = Game.ValidateInputSelection("Would you like to add another third person pronoun? Y/N ");
                if (next == "N")
                    addThird = false;
                else
                {
                    Console.Write("*third person* e.g. they/he/xe ");
                    third = Console.ReadLine();
                    thirds.Add(third);
                }

                next = Game.ValidateInputSelection("Would you like to add another possessive? Y/N ");
                if (next == "N")
                    addPossessive = false;
                else
                {
                    Console.Write("*possessive* e.g. their/his/xer ");
                    possessive = Console.ReadLine();
                    possessives.Add(possessive);
                }
            }

            myPronouns = new pronouns(possessives.ToArray(), thirds.ToArray());
        }

        public class PlayerAttribute
        {
            public string Name { get; private set; }
            public string Value { get; private set; }

            public PlayerAttribute(string name, string value)
            {
                Name = name;
                Value = value;
            }
        }

        public void AccumulateStuff(string stuff)
        {
            Console.WriteLine($"{stuff} is stored safely in your pocket.");
        }
        public string InventoryContents()
        {   
            return inventoryManager.ToString();
        }

        /// <summary>
        /// Output a visual representation of the player's state.
        /// </summary>
        public void PlayerState()
        {
            string player_art = Game.GetArt("player");
            string s = player_art.Replace("{hope}", Hope.Value);
            s = s.Replace("{planning}", Resilience.Value);
            s = s.Replace("{creativity}", Creativity.Value);
            Console.WriteLine(s);
        }
    }
}