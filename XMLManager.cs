using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using static DungeonExplorer.GameMap;

namespace DungeonExplorer
{
    public static class XMLManager
    {
        public static Dictionary<string, Room> GetRoomXML(PlayerManager playerManager)
        {
            /* Create a dictionary <Rooms> and initialise
             * every room in the Rooms.xml file. The dictionary stores
             * Rooms as a key value pair where the key is the string
             * identifier (xml element name) for the room.
             */
            List<XElement> roomXElements = new List<XElement>();
            Dictionary<string, Room> rooms = new Dictionary<string, Room>();
            string path = Game.textDir + "Rooms.xml";

            if (File.Exists(path))
            {
                XElement roomXElement = XElement.Load(path);
                List<XElement> roomElements = roomXElement.Elements().ToList();
                foreach (XElement room in roomElements)
                {
                    roomXElements.Add((room));
                }
            }
            else
            {
                throw new FileNotFoundException("\"Rooms.xml\" file does not exist...");
            }

            foreach (XElement roomXElement in roomXElements)
            {
                Room room = ParseRoomXML(roomXElement, playerManager);
                rooms.Add(roomXElement.Name.ToString(), room);
            }

            return rooms;
        }

        /// <summary>
        /// Method <c>ParseRoomXML</c> loads all necessary data for the room from the "rooms.xml" file.
        /// </summary>
        /// <param name="room"></param>
        /// <exception cref="MissingFieldException"></exception>
        public static Room ParseRoomXML(XElement roomX, PlayerManager playerManager)
        {
            string name = GetValue(roomX, "name");
            string description = GetValue(roomX, "description");
            string entraceDialogue = GetValue(roomX, "entranceDialogue");
            string exitDialogue = GetValue(roomX, "exitDialogue");
            string nextRoom = GetValue(roomX, "next");

            var locationsList = roomX.Elements("location");
            Dictionary<string, location> locations = new Dictionary<string, location>();
            foreach (var locX in locationsList)
            {
                location loc = ParseLocationXML(locX, playerManager);
                locations.Add(loc.Name, loc);
            }

            Room room = new Room(name, description, entraceDialogue, exitDialogue, nextRoom, locations, playerManager);

            foreach(var locX in locations.Values.ToArray())
            {
                locX.ThisRoom = room;
            }

            return room;
        }

        public static location ParseLocationXML(XElement locX, PlayerManager playerManager)
        {
            // For the location
            string name = locX.Attribute("name").Value.ToString();
            string description = locX.Element("description").Value.ToString();
            string dialogue = locX.Element("dialogue").Value.ToString();

            location loc = new location(name, description, dialogue);
            loc.interactions = ParseInteractionXML(locX, playerManager);
            loc.adjacentLocations = ParseAdjacentLocations(locX);

            return loc;
        }

        public static List<Interaction> ParseInteractionXML(XElement locX, PlayerManager playerManager)
        {
            List<Interaction> interactions = new List<Interaction>();
            Interaction interaction;
            var interactionsList = locX.Elements("interaction");
            foreach(var interactionX in interactionsList)
            {
                string interactName = interactionX.Attribute("name").Value.ToString();
                string interactType = interactionX.Attribute("type").Value.ToString();

                // For the dialogue
                string interactDialogue = interactionX.Element("dialogue").Value.ToString();

                // For the item
                XElement interactItem = interactionX.Element("item");
                string itemName = interactItem.Attribute("name").Value.ToString();
                string amount = interactItem.Attribute("amount").Value.ToString();
                int itemAmount = 0;
                if (amount != "") { itemAmount = int.Parse(amount); }

                Id itemId = new Id(interactItem.Attribute("id").Value.ToString());

                // For monster
                XElement interactMonster = interactionX.Element("monster");
                Id monsterId = new Id(interactMonster.Attribute("id").Value.ToString());

                interaction = GameMap.GetInteraction(interactName, interactType, interactDialogue, playerManager, itemId, itemAmount, monsterId);
                interactions.Add(interaction);
            }

            return interactions;
        }

        /// <summary>
        /// Method <c>ParseAdjacentLocations</c> creates a mapping of every neighbouring location to its accessibility level.
        /// </summary>
        /// <param name="loc"></param>
        /// <returns></returns>
        /// <exception cref="MissingFieldException">
        /// Thrown if the location has no attribute specifying the neighbouring nodes.
        /// </exception>
        public static Dictionary<string, bool> ParseAdjacentLocations(XElement loc)
        {
            Dictionary<string, bool> adjLocsDict = new Dictionary<string, bool>();
            string adjLocs = loc.Attribute("adj") == null ? throw new MissingFieldException("This location of room has no attribute with name \"adj\"") : loc.Attribute("adj").Value;

            string[] adjLocsArr = adjLocs.Split(',');
            foreach (var adjLoc in adjLocsArr)
            {
                string[] adjLocData = adjLoc.Split(':');
                adjLocsDict.Add(adjLocData[0], bool.Parse(adjLocData[1]));
            }

            return adjLocsDict;
        }

        public static string GetValue(XElement roomX, string value)
        {
            return roomX.Element(value) == null ? throw new XmlException($"Element {value} does not exist") : roomX.Element(value).Value;
        }


        /* Create a dictionary <items> and initialise
         * every item in the items.xml file. The dictionary stores
         * items as a key value pair where the key is the string
         * identifier (xml element id) for the item.
         */
        public static Dictionary<string, Item> GetItemXML(PlayerManager playerManager)
        {
            Dictionary<string, Item> Items = new Dictionary<string, Item>();
            string path = Game.textDir + "items.xml";

            if (File.Exists(path))
            {
                XElement itemsXElement = XElement.Load(path);
                List<XElement> itemXElements = itemsXElement.Elements().ToList();
                foreach (XElement itemX in itemXElements)
                {
                    string name = itemX.Attribute("name").Value;
                    string type = itemX.Attribute("type").Value;
                    string boost = itemX.Attribute("boost").Value;
                    string description = itemX.Value.ToString();
                    Id id = new Id(itemX.Attribute("id").Value);

                    Item item = GameMap.GetItem(id, type, name, description, boost);
                    Items.Add(id.Val, item);
                }
            }

            return Items;
        }

        public static Dictionary<string, Monster> GetMonsterXML(PlayerManager playerManager)
        {
            Dictionary<string, Monster> Monsters = new Dictionary<string, Monster>();
            string path = Game.textDir + "monsters.xml";

            if (File.Exists(path))
            {
                XElement monstersXElement = XElement.Load(path);
                List<XElement> monsterXElements = monstersXElement.Elements().ToList();
                Monster monster;
                foreach (XElement monsterX in monsterXElements)
                {
                    Id id = new Id(monsterX.Element("id").Value);
                    string type = monsterX.Element("type").Value;
                    string name = monsterX.Element("name").Value;
                    string dialogue = monsterX.Element("dialogue").Value;
                    string energy = monsterX.Element("energy").Value;
                    string damage = monsterX.Element("damage").Value;
                    monster = GameMap.GetMonster(id, type, name, dialogue, energy, damage);

                    Monsters.Add(id.Val, monster);
                }
            }

            return Monsters;
        }
    }
}
 