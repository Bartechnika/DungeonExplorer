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

        // The dialogue to be displayed when the user enters the room (if any)
        public string startDialogue;

        // The dialogue to be displayed when the user exits the room (if any)
        public string exitDialogue;

        // A pointer to the next room to enter when the player exists this room.
        public string nextRoom;

        // The current location of the player within the room.
        public location curLoc;

        // The location to load when the room is first entered.
        public location start;

        // A generic location with no purpose other than to leave the room.
        public location exitLoc;

        // A mapping between locations and their string-identifier.
        public Dictionary<string, location> locations;
        public PlayerManager ThisPlayerManager;
        public RoomManager ThisRoomManager;

        public Room(XElement room, PlayerManager playerManager, RoomManager roomManager)
        {
            this.locations = new Dictionary<string, location>();
            this.exitLoc = new location("exit", this);
            this.ThisPlayerManager = playerManager;
            this.ThisRoomManager = roomManager;

            InitialiseRoom(room);
            ThisRoomManager = roomManager;
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
                        throw new MissingFieldException("This interaction does not have an attribute corresponding to a defined interaction");

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
        /// 
        /// </summary>
        /// <para>
        /// This class stores all data relating to any location in the game. Every location has an associated interaction
        /// which may be dialogue, a creature battle, an item discovery - or any feature not yet implemented such as puzzles.
        /// </para>
        public class location
        {   
            public string Name;

            // Whether the location is currently accessible
            public string State;

            // The associated interaction
            public Interaction interaction;

            // The room the location is situated in
            public Room Room;

            public Dictionary<string, bool> adjacentLocations;

            public location(string name, Room room)
            {   
                this.Name = name;
                this.State = "closed";
                this.Room = room;
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
                Console.WriteLine($"{count+1}. Exit" );
            }

            public bool TriggerInteraction()
            {
                return interaction.Interact();
            }

            public location Navigate()
            {
                string[] options = new string[adjacentLocations.Count+1];
                for (int i = 0; i < options.Length; i++)
                {
                    options[i] = (i+1).ToString();
                }
                GetAdjacentLocations();
                int sel = int.Parse(Game.ValidateInputSelection("\nContinue to (1, 2...): ", options));

                location loc;
                if (sel == adjacentLocations.Count + 1)
                    loc = Room.exitLoc;
                else
                    loc = (Room.locations[adjacentLocations.Keys.ToArray()[sel - 1]]);

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
            while (!exit)
            {
                ThisPlayerManager.CalculateBoost();
                locArt = Game.GetArt("location");
                locArt = Game.PopulateField(locArt, "{location~~~~~~~~~~~}", curLoc.Name);
                Console.WriteLine(locArt);

                exit = curLoc.TriggerInteraction();
                if(exit)
                {
                    break;
                }

                curLoc = curLoc.Navigate();
                if(curLoc == exitLoc)
                {
                    break;
                }
            }

            Exit();
        }

        public void Exit()
        {
            Console.WriteLine("");
            Game.WriteDialogue(exitDialogue);   
        }
    }
}