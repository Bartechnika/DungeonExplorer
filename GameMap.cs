using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.SqlServer.Server;

namespace DungeonExplorer
{
    public class GameMap
    {
        /// <value>
        /// Property <c>Rooms</c> maps all Rooms to a string identifier.
        /// </value>
        public Dictionary<string, Room> Rooms {  get; set; } = new Dictionary<string, Room>();

        /// <value>
        /// Property <c>CurRoom</c> points to the room currently occupied by the player.
        /// </value>
        private Room CurRoom { get; set; }

        /// <value>
        /// Property <c>NextRoom</c> points to the next room navigable by the player (i.e. the exit of CurRoom).
        /// </value>
        private Room NextRoom {  get; set; }

        /// <value>
        /// Property <c>Interactions</c> maps all interactions (base unit of play) to a string identifier.
        /// </value>
        private Dictionary<string, Interaction> Interactions {  get; set; }
        private PlayerManager PlayerManager {  get;}

        public GameMap(PlayerManager playerManager)
        {
            PlayerManager = playerManager ?? throw new ArgumentNullException(nameof(playerManager));

            Rooms = XMLManager.GetRoomXML(playerManager);

            CurRoom = Rooms.Values.ToArray()[0];
        }

        public void Update()
        {
            bool exit = false;
            while (!exit)
            {
                CurRoom.Enter();
                CurRoom = Rooms[CurRoom.NextRoom];
            }
        }

        public static Interaction GetInteraction(string type, string dialogue, PlayerManager playerManager, Id itemId=new Id(), int amount=0, Id monsterId=new Id())
        {
            switch (type)
            {
                case "dialogue":
                    return new Dialogue(dialogue, playerManager);
                case "found-item":
                    return new FoundItem(dialogue, playerManager, itemId, amount);
                case "fight-monster":
                    return new FightMonster(dialogue, playerManager, monsterId);
                case "mirror":
                    return new Mirror(dialogue, playerManager);
            }

            return new Dialogue("null", playerManager);
        }
    }
}
