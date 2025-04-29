using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using static DungeonExplorer.GameMap;

namespace DungeonExplorer
{
    /// <summary>
    /// </summary>
    /// <para>
    /// This class stores all data relating to any location in the game. Every location has an associated interaction
    /// which may be dialogue, a creature battle, an item discovery - or any feature not yet implemented such as puzzles.
    /// </para>
    public class location
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Dialogue {  get; set; }

        // The exit flag for this location
        public bool exitLocation { get; set; }

        public bool exitRoom { get; set; }

        // The associated set of interactions
        public List<Interaction> interactions { get; set; }

        // The room the location is situated in
        public Room ThisRoom { get; set; }

        public Dictionary<string, bool> adjacentLocations;

        /// <summary>
        /// Constructor <c>location</c> checks if location name or room is null.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="room"></param>
        /// <exception cref="MissingFieldException"></exception>
        public location(string name, string description, string dialogue, Room room=null)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name), "The name cannot be null");
            Dialogue = dialogue;
            Description = description;
            //?? throw new ArgumentNullException(nameof(room), "The room cannot be null");
            ThisRoom = room;
            exitLocation = false;
            adjacentLocations = new Dictionary<string, bool>();
        }

        /// <summary>
        /// Method <c>GetAdjacentLocations</c> outputs a list of all neighbouring locations and their accessiblity level,
        /// which is either "unlocked" or "locked". A location may be unlocked once an objective has been completed, for example.
        /// </summary>
        public location GetAdjacentLocations()
        {
            location next = this;

            int count = 0;
            string unlocked = "";
            string[] ops = new string[adjacentLocations.Count+2];
            ops[ops.Length - 2] = "EXIT";
            ops[ops.Length - 1] = "¬";
            Console.WriteLine("\nTravelling to next location...\n");
            foreach (var loc in adjacentLocations)
            {
                ops[count] = (count+1).ToString();
                count += 1;
                unlocked = loc.Value == true ? "unlocked" : "locked";
                string output = $"({count}) {loc.Key} : {unlocked} ";
                Console.WriteLine(output);
            }
            Console.WriteLine("EXIT");
            Console.Write("¬ back");

            string sel;
            bool cont = true;

            sel = Game.ValidateInputSelection("\n-> ", ops);
            switch (sel)
            {
                case "exit":
                {
                        exitRoom = true;
                        break;
                }

                case "¬":
                {
                    UI.GetLocation(this);
                        break;
                }

                default:
                {
                    if (adjacentLocations[adjacentLocations.Keys.ToArray()[int.Parse(sel) - 1]] == true)
                    {
                        Console.WriteLine("\n/^`o`/^ Walking to next location...\n");
                        //Game.Wait(2);
                        exitLocation = true; // raise exit flag to leave location
                        next = ThisRoom.Locations[adjacentLocations.Keys.ToArray()[int.Parse(sel) - 1]];
                    }
                    else
                    {
                        Console.Write("Location is inaccessible! please pick another location\n");
                    }

                   break;
                }

            }

            return next;
        }

        public void GetInteraction()
        {
            int count = 0;
            string[] ops = new string[interactions.Count + 1];
            ops[ops.Length - 1] = "¬";
            Console.WriteLine("\nInteractions available are...\n");
            foreach (Interaction interaction in interactions)
            {
                ops[count] = (count+1).ToString();
                string output = $"({count+1}) {interaction.Name}";
                Console.WriteLine(output);
                count++;
            }
            Console.WriteLine("¬ back");

            string sel;
            sel = Game.ValidateInputSelection("\n-> ", ops);
            if(sel != "¬")
            {
                TriggerInteraction(interactions[int.Parse(sel)-1]);
            }
        }

        public void TriggerInteraction(Interaction interaction)
        {
            interaction.Interact();
        }

        public void GetPlayerActions()
        {
            string[] ops = new string[3];
            ops[ops.Length - 1] = "¬";

            Console.WriteLine("\nPlease select a player action (1, 2...)\n");
            Console.WriteLine("¬ back");
            for (int i = 0; i < 2; i++)
            {
                ops[i] = (i+1).ToString();
            }
            string sel;

            bool cont = true;
            sel = Game.ValidateInputSelection("\n-> ", ops);
            switch (sel)
            {
                case "¬":
                    {
                        cont = false;
                        break;
                    }
                case "1":
                    {
                        ThisRoom.ThisPlayerManager.CheckPockets();
                        cont = false;
                        break;
                    }
            }
        }

        public location Enter()
        {
            exitRoom = false;
            exitLocation = false;
            location loc = this;

            while(!exitRoom && !exitLocation)
            {
                string key = Game.ValidateInputSelection("-> ", new string[] { "A", "B", "C" });
                switch (key)
                {
                    case "a": { loc = GetAdjacentLocations(); break; }
                    case "b": { GetInteraction(); break; }
                    case "c": { GetPlayerActions(); break; }
                }
                UI.ClearConsole();
                UI.GetLocation(this);
            }
            return loc;
        }
    }
}
