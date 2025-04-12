using System;
using System.Collections.Generic;
using System.Linq;
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

        // The exit flag for this location
        public bool exit { get; set; }

        // The associated interaction
        public Interaction interaction { get; set; }

        // The room the location is situated in
        //public Room ThisRoom { get; set; }

        public Dictionary<string, bool> adjacentLocations;
        private const int numDefaultOptions = 2;

        /// <summary>
        /// Constructor <c>location</c> checks if location name or room is null.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="room"></param>
        /// <exception cref="MissingFieldException"></exception>
        public location(string name, Room room=null)
        {
            this.Name = name ?? throw new ArgumentNullException(nameof(name), "The name cannot be null");
            //this.ThisRoom = room ?? throw new ArgumentNullException(nameof(room), "The room cannot be null");
            this.exit = false;
            this.adjacentLocations = new Dictionary<string, bool>();
        }

        /// <summary>
        /// Method <c>GetAdjacentLocations</c> outputs a list of all neighbouring locations and their accessiblity level,
        /// which is either "unlocked" or "locked". A location may be unlocked once an objective has been completed, for example.
        /// </summary>
        public void GetAdjacentLocations()
        {
            ushort count = 0;
            string unlocked = "";
            Console.WriteLine("Where to next?");
            foreach (var loc in adjacentLocations)
            {
                count += 1;
                unlocked = loc.Value == true ? "unlocked" : "locked";
                string output = $"{count}. {loc.Key} : {unlocked} ";
                Console.WriteLine(output);
            }

            // Additional options available to user in every location:

            // Room Exit : Navigate to the exit location of the room.
            // Check Pockets : Rummage through pockets and describe each item in detail.
            Console.WriteLine($"{count + 1}. Rummage through pockets");
            Console.WriteLine($"{count + 2}. Exit");
        }

        public bool TriggerInteraction()
        {
            return interaction.Interact();
        }

        public location Navigate()
        {
            string[] options = new string[adjacentLocations.Count + numDefaultOptions];
            for (int i = 0; i < options.Length; i++)
            {
                options[i] = (i + 1).ToString();
            }
            GetAdjacentLocations();
            int sel = int.Parse(Game.ValidateInputSelection("Please select (1, 2...): ", options));

            location loc = this;

            // Handle default options
            if (sel == adjacentLocations.Count + 1) // Case Check Pockets
                //ThisRoom.ThisPlayerManager.CheckPockets();
            else if (sel == adjacentLocations.Count + 2) // Case Exit
                exit = true;
            else
                //loc = (ThisRoom.locations[adjacentLocations.Keys.ToArray()[sel - 1]]);

            return loc;

        }
    }
}
