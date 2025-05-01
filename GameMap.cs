using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.SqlServer.Server;

namespace DungeonExplorer
{
    public static class GameMap
    {
        /// <value>
        /// Property <c>Rooms</c> maps all Rooms to a string identifier.
        /// </value>
        public static Dictionary<string, Room> Rooms {  get; private set; } = new Dictionary<string, Room>();

        /// <value>
        /// Property <c>CurRoom</c> points to the room currently occupied by the player.
        /// </value>
        public static Room CurRoom { get; private set; }

        /// <value>
        /// Property <c>NextRoom</c> points to the next room navigable by the player (i.e. the exit of CurRoom).
        /// </value>
        public static Room NextRoom {  get; set; }

        /// <value>
        /// Property <c>Interactions</c> maps all interactions (base unit of play) to a string identifier.
        /// </value>
        private static Dictionary<string, Interaction> Interactions {  get; set; }
        private static PlayerManager PlayerManager { get; set; }

        // Interaction elements
        public static Dictionary<string, Item> Items { get; private set; }
        public static Dictionary<string, Monster> Monsters {  get; private set; }

        public static void SetValues(PlayerManager playerManager)
        {
            PlayerManager = playerManager ?? throw new ArgumentNullException(nameof(playerManager));

            Items = XMLManager.GetItemXML(playerManager);
            Monsters = XMLManager.GetMonsterXML(playerManager);

            Rooms = XMLManager.GetRoomXML(playerManager);

            CurRoom = Rooms.Values.ToArray()[0];
        }

        public static void Update()
        {
            bool exit = false;
            while (!exit)
            {
                CurRoom.Enter();
                CurRoom = Rooms[CurRoom.NextRoom];
            }
        }

        public static Item GetItem(Id id, string type, string name, string description, string boost)
        {
            Item item = null;
            switch(type)
            {
                case "comfort-toy":
                    {
                        item = new ComfortToy(id, name, int.Parse(boost), description);
                        break;
                    }
                case "book":
                    {
                        item = new Book(id, name, int.Parse(boost), description);
                        break;
                    }
                case "hat":
                    {
                        item = new Hat(id, name, int.Parse(boost), description);
                        break;
                    }
                case "snack":
                    {
                        item = new Snack(id, name, int.Parse(boost), description);
                        break;
                    }
                case "miscellaneous":
                    {
                        item = new Miscellaneous(id, name, description);
                        break;
                    }
                default:
                    {
                        throw new ArgumentException("Type is not valid");
                    }
            }
            return item;
        }

        public static Monster GetMonster(Id id, string type, string name, string dialogue, string energy, string damage)
        {
            Monster monster = null;
            switch (type)
            {
                case "internal":
                    monster = new InternalMonster(id, name, dialogue, int.Parse(energy), int.Parse(damage), PlayerManager);
                    break;
                case "sensory":
                    monster = new SensoryMonster(id, name, dialogue, int.Parse(energy), int.Parse(damage), PlayerManager);
                    break;
                case "social":
                    monster = new SocialMonster(id, name, dialogue, int.Parse(energy), int.Parse(damage), PlayerManager);
                    break;
            }
            return monster;
        }

        public static Interaction GetInteraction(string name, string type, string dialogue, PlayerManager playerManager, Id itemId, int amount, Id monsterId)
        {
            switch (type)
            {
                case "dialogue":
                    return new Dialogue(name, dialogue, playerManager);
                case "found-item":
                    return new FoundItem(name, dialogue, playerManager, itemId, amount);
                case "fight-monster":
                    return new FightMonster(monsterId, name, dialogue, playerManager);
                case "mirror":
                    return new Mirror(name, dialogue, playerManager);
            }

            return new Dialogue(name, "null", playerManager);
        }
    }
}
