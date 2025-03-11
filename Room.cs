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
    public class Room
    {
        public string name;
        public string description;
        public string startDialogue;
        public string exitDialogue;
        public string nextRoom;
        public string prevRoom;
        public location curLoc;
        public location start;
        public location exitLoc;
        public Dictionary<string, location> locations;
        public Player Player;

        public Room(XElement room, Player player)
        {
            this.locations = new Dictionary<string, location>();
            this.exitLoc = new location("exit", this);

            InitialiseRoom(room);

        }

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

                    // For the interaction
                    Interaction interaction;

                switch (interactionElement.Attribute("type").Value)
                {
                    case "dialogue":
                        interaction = RoomManager.GetInteraction(interactionElement.Attribute("type").Value, interactionElement.Value);
                        break;
                    case "battle-demons":
                        XElement creatureElement = interactionElement.Element("creature");
                        string name = creatureElement.Attribute("name").Value;
                        string dialogue = creatureElement.Value;
                        int of = int.Parse(creatureElement.Attribute("of").Value);
                        Creatures.Creature creature = new Creatures.SensoryCreature(name, dialogue, of, Player); 
                        interaction = RoomManager.GetInteraction(interactionElement.Attribute("type").Value, interactionElement.Value, creature);
                        break;
                    default:
                        throw new MissingFieldException("This interaction has attribute corresponding to a defined interaction");
                        
                }
                newLocation.interaction = interaction;
                newLocation.adjacentLocations = ParseAdjacentLocations(loc);
                locations.Add(newLocation.Name, newLocation);
            }

            start = locations.Values.ToArray()[0];
        }

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

        public class location
        {
            public string Name;
            public string State;
            public Interaction interaction;
            public Room Room;
            public Dictionary<string, bool> adjacentLocations;

            public location(string name, Room room)
            {   
                this.Name = name;
                this.State = "closed";
                this.interaction = new RoomManager.Dialogue("default");
                this.Room = room;
                this.adjacentLocations = new Dictionary<string, bool>();
            }

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