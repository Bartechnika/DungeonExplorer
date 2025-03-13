using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Security.Policy;
using System.Threading;
using System.Xml;
using System.Xml.Linq;
using static DungeonExplorer.Creatures;
using static DungeonExplorer.Room;
using static DungeonExplorer.RoomManager;

namespace DungeonExplorer
{   
    /// <summary>
    /// 
    /// </summary>
    /// <para>
    /// This class represents the basic Room object and consists of a set of locations.
    /// Every room is instantiated by the RoomManager classes at the start of the game.
    /// </para>
    public class Room
    {
        public string name;
        public string description;

        /// <value>
        /// Property <c>startDialogue</c> is displayed when the user enters the room.
        /// </value>
        public string startDialogue;

        /// <value>
        /// Property <c>exitDialogue</c> is displayed when the user exists the room.
        /// </value>
        public string exitDialogue;

        /// <value>
        /// Property <c>nextRoom</c> is a pointer to the next room to enter upon leaving the current room.
        /// </value>
        public string nextRoom;

        /// <value>
        /// Property <c>curLoc</c> stores the current location of the player within the room.
        /// </value>
        public location curLoc;

        /// <value>
        /// Property <c>location</c> is the location to load when the room is first entered.
        /// </value>
        public location start;

        /// <value>
        /// Property <c>exitLoc</c> maps each location to its string-identifier.
        /// </value>
        public Dictionary<string, location> locations;
        public PlayerManager ThisPlayerManager;
        public RoomManager ThisRoomManager;

        public Room(XElement room, PlayerManager playerManager, RoomManager roomManager)
        {
            this.locations = new Dictionary<string, location>();
            this.ThisPlayerManager = playerManager ?? throw new ArgumentNullException(nameof(playerManager));
            this.ThisRoomManager = roomManager ?? throw new ArgumentNullException(nameof(roomManager));

            InitialiseRoom(room);
        }

        /// <summary>
        /// Method <c>InitialiseRoom</c> loads all necessary data for the room from the "rooms.xml" file.
        /// </summary>
        /// <param name="room"></param>
        /// <exception cref="MissingFieldException"></exception>
        public void InitialiseRoom(XElement room)
        {
            name = room.Element("name").Value.Trim();
            description = room.Element("description").Value.Trim();
            startDialogue = room.Element("start").Value.Trim();
            exitDialogue = room.Element("exit").Value.Trim();
            nextRoom = room.Element("next").Value.Trim();

            var locationsList = room.Elements("location");
            foreach (var loc in locationsList)
            {

                location newLocation = new location(loc.Attribute("name").Value.ToString(), this);
                XElement interactionElement = loc.Element("interaction");
                newLocation.adjacentLocations = ParseAdjacentLocations(loc);

                Interaction interaction;

                string dialogue;
                switch (interactionElement.Attribute("type").Value)
                {
                    case "dialogue":
                        interaction = ThisRoomManager.GetInteraction(interactionElement.Attribute("type").Value, interactionElement.Value);
                        break;
                    case "battle-demons":
                        XElement creatureElement = interactionElement.Element("creature");
                        string name = creatureElement.Attribute("name").Value;
                        dialogue = creatureElement.Value;
                        int of = int.Parse(creatureElement.Attribute("of").Value);
                        Creatures.Creature creature = new Creatures.SensoryCreature(name, dialogue, of, ThisPlayerManager);
                        interaction = ThisRoomManager.GetInteraction(interactionElement.Attribute("type").Value, interactionElement.Value, creature);
                        break;
                    case "found-item":
                        XElement foundItem = interactionElement.Element("item");
                        string itemId = foundItem.Attribute("id").Value;
                        int amount = int.Parse(foundItem.Attribute("amount").Value);
                        dialogue = foundItem.Value;
                        interaction = ThisRoomManager.GetInteraction(interactionElement.Attribute("type").Value, dialogue, itemId: itemId, amount: amount);
                        break;
                    case "mirror":
                        dialogue = interactionElement.Value;
                        interaction = ThisRoomManager.GetInteraction(interactionElement.Attribute("type").Value, dialogue);
                        break;
                    default:
                        throw new MissingFieldException("This interaction does not have an attribute corresponding to a defined interaction.");

                }

                newLocation.interaction = interaction;
                newLocation.adjacentLocations = ParseAdjacentLocations(loc);
                locations.Add(newLocation.Name, newLocation);

            }

            start = locations.Values.ToArray()[0];
        }

        /// <summary>
        /// Method <c>ParseAdjacentLocations</c> creates a mapping of every neighbouring location to its accessibility level.
        /// </summary>
        /// <param name="loc"></param>
        /// <returns></returns>
        /// <exception cref="MissingFieldException">
        /// Thrown if the location has no attribute specifying the neighbouring nodes.
        /// </exception>
        public Dictionary<string,bool> ParseAdjacentLocations(XElement loc)
        {   
            Dictionary<string, bool> adjLocsDict = new Dictionary<string, bool>();
            string adjLocs = loc.Attribute("adj") == null ? throw new MissingFieldException("This location of room has no attribute with name \"adj\"") : loc.Attribute("adj").Value;

            string[] adjLocsArr = adjLocs.Split(',');
            foreach(var adjLoc in adjLocsArr)
            {
                string[] adjLocData = adjLoc.Split(':');
                adjLocsDict.Add(adjLocData[0], bool.Parse(adjLocData[1]));
            }

            return adjLocsDict;
        }

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
            public Room ThisRoom { get; set; }

            public Dictionary<string, bool> adjacentLocations;
            private const int numDefaultOptions = 2;
            
            /// <summary>
            /// Constructor <c>location</c> checks if location name or room is null.
            /// </summary>
            /// <param name="name"></param>
            /// <param name="room"></param>
            /// <exception cref="MissingFieldException"></exception>
            public location(string name, Room room)
            {
                this.Name = name ?? throw new ArgumentNullException(nameof(name), "The name cannot be null");
                this.ThisRoom = room ?? throw new ArgumentNullException(nameof(room), "The room cannot be null");
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
                Console.WriteLine($"{count+1}. Rummage through pockets");
                Console.WriteLine($"{count+2}. Exit" );
            }

            public bool TriggerInteraction()
            {
                return interaction.Interact();
            }

            public location Navigate()
            {
                string[] options = new string[adjacentLocations.Count+numDefaultOptions];
                for (int i = 0; i < options.Length; i++)
                {
                    options[i] = (i+1).ToString();
                }
                GetAdjacentLocations();
                int sel = int.Parse(Game.ValidateInputSelection("Please select (1, 2...): ", options));

                location loc = ThisRoom.curLoc;

                // Handle default options
                if (sel == adjacentLocations.Count + 1) // Case Check Pockets
                    ThisRoom.ThisPlayerManager.CheckPockets();
                else if (sel == adjacentLocations.Count + 2) // Case Exit
                    exit = true;
                else
                    loc = (ThisRoom.locations[adjacentLocations.Keys.ToArray()[sel - 1]]);

                return loc;

            }
        }

        public void Enter()
        {
            string art = Game.GetArt("room");
            art = Game.PopulateField(art, "{room~~~~~~~~~~~~~~~}", name);
            Console.WriteLine(art);
            Game.WriteDialogue($"Description: {description}\n");
            Game.WriteDialogue(startDialogue);

            curLoc = start;
            string locArt = "";

            bool exit = false;
            bool changeLoc = false;
            location nextLoc;
            while (!exit)
            {
                ThisPlayerManager.CalculateBoost();
                locArt = Game.GetArt("location");
                locArt = Game.PopulateField(locArt, "{location~~~~~~~~~~~}", curLoc.Name);
                Console.WriteLine(locArt);

                exit = curLoc.TriggerInteraction();
                if (exit)
                {
                    break;
                }

                // Default actions incl. CheckPockets() can be performed multiple times in a single location
                // until the user selects the option of changing location, in which case the loop is terminated.
                changeLoc = false;
                while (!changeLoc)
                {
                    nextLoc = curLoc.Navigate();
                    
                    // Check if the player wants to exit the current room but current location is not changed as a result.
                    if (nextLoc.exit == true)
                    {
                        exit = true;
                        changeLoc = true;
                        break;
                    }
                    if (nextLoc != curLoc || nextLoc.exit == true)
                    {
                        curLoc = nextLoc;
                        changeLoc = true;
                    }
                }
            }

            Exit();
        }

        public void Exit()
        {
            Console.WriteLine("");
            Game.WriteDialogue(exitDialogue);   
        }

        public override string ToString()
        {
            return description;
        }
    }
}