using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using static DungeonExplorer.GameMap;

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
        public string Name { get; private set; }
        public string Description { get; private set; }

        /// <value>
        /// Property <c>entranceDialogue</c> is displayed when the user enters the room.
        /// </value>
        public string EntranceDialogue {  get; private set; }

        /// <value>
        /// Property <c>exitDialogue</c> is displayed when the user exists the room.
        /// </value>
        public string ExitDialogue {  get; private set; }

        /// <value>
        /// Property <c>nextRoom</c> is a pointer to the next room to enter upon leaving the current room.
        /// </value>
        public string NextRoom { get; private set; }

        /// <value>
        /// Property <c>CurLoc</c> stores the current location of the player within the room.
        /// </value>
        public location CurLoc { get; private set; }

        /// <value>
        /// Property <c>StartLoc</c> is the location to load when the room is first entered.
        /// </value>
        public location StartLoc {  get; private set; }

        /// <value>
        /// Property <c>exitLoc</c> maps each location to its string-identifier.
        /// </value>
        public Dictionary<string, location> Locations {  get; private set; } = new Dictionary<string, location>();
        public PlayerManager ThisPlayerManager {  get; private set; }

        public Room(string name, string description, string entranceDialogue, string exitDialogue, string nextRoom, Dictionary<string, location> locations, PlayerManager playerManager)
        {
            Name = name;
            Description = description;
            EntranceDialogue = entranceDialogue;
            ExitDialogue = exitDialogue;
            NextRoom = nextRoom;
            Locations = locations;
            StartLoc = locations.Values.ToArray()[0];
            ThisPlayerManager = playerManager ?? throw new ArgumentNullException(nameof(playerManager));
        }

        public void Enter()
        {
            UI.ClearConsole();
            string art = UI.GetArt("room", Game.locDir);
            art = UI.PopulateField(art, "{room-name~~~~~~~~~~}", Name);

            CurLoc = StartLoc;

            location nextLoc;
            while (true)
            {
                UI.GetLocation(CurLoc);
                nextLoc = CurLoc.Enter();
                if (CurLoc.exitRoom == true) { CurLoc.exitRoom = false; break; }
                CurLoc = nextLoc;

                /*
                // Default actions incl. CheckPockets() can be performed multiple times in a single location
                // until the user selects the option of changing location, in which case the loop is terminated.
                changeLoc = false;
                while (!changeLoc)
                {
                    nextLoc = CurLoc.Enter();

                    // Check if the player wants to exit the current room but current location is not changed as a result.
                    if (nextLoc.exit == true)
                    {
                        exit = true;
                        changeLoc = true;
                        break;
                    }
                    if (nextLoc != CurLoc || nextLoc.exit == true)
                    {
                        CurLoc = nextLoc;
                        changeLoc = true;
                    }
                }*/
            }

            Exit();
        }

        public void Exit()
        {
            UI.WriteDialogue(ExitDialogue);
        }

        public override string ToString()
        {
            return Description;
        }
    }
}