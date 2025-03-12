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
        public string Name { get; private set; }

        /// <summary>
        /// Struct <c>pronouns</c> allows the user to set any number of subject, object and possessive pronouns.
        /// </summary>
        public struct pronouns 
        {
            /// <value>
            /// Property <c>Subjects</c> is a list of all subject pronouns chosen by the user
            /// </value>
            /// <example>
            /// For example:
            /// <c>Subjects = new string[] {"She", "They"}</c>
            /// </example>
            private string[] Subjects;

            /// <value>
            /// Property <c>Objects</c> is a list of all object pronouns chosen by the user
            /// </value>
            /// <example>
            /// For example:
            /// <c>Objects = new string[] {"Her", "Them"}c>
            /// </example>
            private string[] Objects;

            /// <value>
            /// Property <c>Possessives</c> is a list of all possessive pronouns chosen by the user
            /// </value>
            /// <example>
            /// For example:
            /// <c>Posessives = new string[] {"Hers", "Theirs"}</c>
            /// </example>
            private string[] Possessives;

            private static Random rnd = new Random();

            /// <summary>
            /// Methods <c>GetSubject</c>, <c>GetObjects</c> and <c>GetPossessive</c> return a random pronoun of their respective type.
            /// </summary>
            public string GetSubject() => Subjects[rnd.Next(0, Subjects.Length)];
            public string GetObject() => Objects[rnd.Next(0, Objects.Length)];
            public string GetPossessive() => Possessives[rnd.Next(0, Possessives.Length)];

            public pronouns(string[] subjects, string[] objects, string[] possessives)
            {
                Subjects = subjects;
                Objects = objects;
                Possessives = possessives;

                if (subjects.Length == 0)
                    Subjects = new string[] { "404" };
                if (objects.Length == 0)
                    Objects = new string[] { "404" };
                if (possessives.Length == 0)
                    Possessives = new string[] { "404" };
            }
        }

        public pronouns myPronouns;

        public PlayerManager playerManager;

        /// <value>
        /// Property <c>Resilience</c> is a measure of the player's to persevere in the face of difficulty. 
        /// </value>
        public PlayerAttribute Resilience;

        /// <value>
        /// Property <c>Imagination</c> is a measure of the player's ability to think outside the box.
        /// </value>
        public PlayerAttribute Imagination;

        /// <value>
        /// Property <c>Energy</c> is a measure of how much more of this the player can take.
        /// </value>
        public PlayerAttribute Energy;

        public Player()
        {
            Resilience = new PlayerAttribute("Resilience", 0);
            Imagination = new PlayerAttribute("Imagination", 0);
            Energy = new PlayerAttribute("Energy", 0);
        }

        /// <summary>
        /// Method <c>SetName</c> performs input validation on the name chosen by the player.
        /// </summary>
        /// <remarks>
        /// Length of player name must be in range 1-30 characters and can only contain chars of type letter.
        /// </remarks>
        public void SetName()
        {
            string name = "0";
            bool validName = false;
            while (!validName)
            {
                Console.Write("\nthe name reads: ");
                name = Console.ReadLine();
                if (name.Length < 1 || name.Length > 30)
                    Console.WriteLine("Name must be between 1 and 30 characters long.");
                else if (!name.All(char.IsLetter))
                    Console.WriteLine("Name must not contain digits: only letters of English alphabet.");
                else
                    validName = true;
            }
            Name = name;
            SetPronouns();
        }

        /// <summary>
        /// Method <c>SetPronouns</c> constructs pronoun lists for the player.
        /// </summary>
        public void SetPronouns()
        {   
            List<string> Subjects = new List<string>();
            List<string> Objects = new List<string>();
            List<string> Possessives = new List<string>();

            bool addSubject = true;
            bool addObject = true;
            bool addPossessive = true;

            string next;

            Console.WriteLine("\nwith pronouns: ");
            while (addSubject || addObject || addPossessive)
            {
                if (addSubject)
                {
                    next = Game.ValidateInputSelection("Would you like to add another subject pronoun? Y/N ");
                    if (next == "N")
                        addSubject = false;
                    else
                    {
                        Console.Write("*subject pronoun* e.g. She/He/They/Xe ");
                        Subjects.Add(Console.ReadLine());
                    }
                }

                if(addObject)
                {
                    next = Game.ValidateInputSelection("Would you like to add another object pronoun? Y/N ");
                    if (next == "N")
                    {
                        addObject = false;
                    }
                    else
                    {
                        Console.Write("*object pronoun* e.g. Her/Him/Them/Xem ");
                        Objects.Add(Console.ReadLine());
                    }
                }

                if (addPossessive)
                {
                    next = Game.ValidateInputSelection("Would you like to add another possessive pronoun? Y/N ");
                    if (next == "N")
                    {
                        addPossessive = false;
                    }
                    else
                    {
                        Console.Write("*possessive pronoun* e.g. Hers/His/Theirs/Xyrs ");
                        Possessives.Add(Console.ReadLine());
                    }
                }
            }

            myPronouns = new pronouns(Subjects.ToArray(), Objects.ToArray(), Possessives.ToArray());
        }

        /// <summary>
        /// Class <c>PlayerAttribute</c> represents an attribute of the player.
        /// </summary>
        public class PlayerAttribute
        {
            public string Name { get; private set; }
            public Player player;
            private int _value;

            public int Value
            {
                get => _value;
                set
                {
                    _value = value;
                    if (value < 0 || value > 100)
                    {
                        _value = 0;
                    }
                }
            }
            public int Boost;

            public PlayerAttribute(string name, int value)
            {
                Name = name;
                Value = value;
            } 
        }

        /// <summary>
        /// Method <c>Rest</c> is a stub called when the player becomes exhuasted from lack of energy.
        /// </summary>
        public void Rest()
        {
            Console.WriteLine("Overwhelmed, you curl into a ball and hibernate for 5 minutes\n");
            System.Threading.Thread.Sleep(100000);
        }
    }
}