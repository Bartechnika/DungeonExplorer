using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Net.NetworkInformation;
using System.Security.Policy;
using DungeonExplorer;
using static System.Net.Mime.MediaTypeNames;

namespace DungeonExplorer
{
    public class Player
    {   
        public string Name 
        { 
            get; 
            private set;
        }

        public struct pronouns 
        {   
            private string[] Possessives;
            private string[] Thirds; 

            private static Random rnd = new Random();

            public string GetPossessive() => Possessives[rnd.Next(0, Possessives.Length)];
            public string GetThird() => Thirds[rnd.Next(0, Thirds.Length)];

            public pronouns(string[] possessives, string[] thirds)
            {
                if (possessives.Length == 0)
                    possessives = new string[] { "/redacted/" };
                if (thirds.Length == 0)
                    thirds = new string[] { "/redacted/" };
                Possessives = possessives;  
                Thirds = thirds;
            }
        }

        public pronouns myPronouns;

        public Dictionary<Room, PlayerAttribute> rooms = new Dictionary<Room, PlayerAttribute>();
        public InventoryManager inventoryManager;

        /// <summary>
        ///  Player attributes
        /// </summary>
        public PlayerAttribute Hope;
        public PlayerAttribute Imagination;
        public PlayerAttribute Energy;


        public Player()
        {
            Hope = new PlayerAttribute("Hope", 100);
            Imagination = new PlayerAttribute("Imagination", 100);
            Energy = new PlayerAttribute("Energy", 100);
            inventoryManager = new InventoryManager();
        }

        public void SetName()
        {
            string name = "0";
            bool validName = false;
            while (!validName)
            {
                Console.Write("\nthe name reads: ");
                name = Console.ReadLine();
                if (name.Length > 30)
                    Console.WriteLine("\nName must be less than 30 characters long.\n");
                else if (!name.All(char.IsLetter))
                    Console.WriteLine("\nName must not contain digits: only letters of English alphabet.\n");
                else
                    validName = true;
            }
            Name = name;
            SetPronouns();
        }

        public void SetPronouns()
        {   
            List<string> possessives = new List<string>();
            List<string> thirds = new List<string>();

            bool addPossessive = true;
            bool addThird = true;

            string next;

            Console.WriteLine("\nwith pronouns: ");
            while (addPossessive || addThird)
            {
                string possessive = "";
                string third = "";

                if (addThird)
                {
                    next = Game.ValidateInputSelection("Would you like to add another third person pronoun? Y/N ");
                    if (next == "N")
                        addThird = false;
                    else
                    {
                        Console.Write("*third person* e.g. they/he/xe ");
                        third = Console.ReadLine();
                        thirds.Add(third);
                    }
                }

                if(addPossessive)
                {
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
            }

            myPronouns = new pronouns(possessives.ToArray(), thirds.ToArray());
        }

        public class PlayerAttribute
        {
            public string Name { get; private set; }
            private int _value;
            public int Value
            {
                get => _value;
                set
                {
                    if(value < 0 || value > 100)
                    {
                        _value = 0;
                    }

                    _value = value;
                }
            }

            public PlayerAttribute(string name, int value)
            {
                Name = name;
                Value = value;
            }
        }

        public void TakeDamage(int overwhelmFactor)
        {
            Hope.Value -= overwhelmFactor;
            Console.WriteLine($"\nYour hope was reduced to {Hope.Value} by {overwhelmFactor} points.\n");
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
            string s = player_art.Replace("{hope}", Hope.Value.ToString());
            s = s.Replace("{creativity}", Imagination.Value.ToString());
            Console.WriteLine(s);
        }
    }
}