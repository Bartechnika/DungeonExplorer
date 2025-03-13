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
    public class RoomManager
    {
        public Dictionary<string, Room> rooms;
        public Room curRoom;
        public Dictionary<string, Interaction> interactions;
        public PlayerManager PlayerManager;

        public RoomManager(PlayerManager playerManager)
        {
            PlayerManager = playerManager ?? throw new ArgumentNullException(nameof(playerManager));
            rooms = new Dictionary<string, Room>();

            /* Create a dictionary <rooms> and initialise
             * every room in the rooms.xml file. The dictionary stores
             * rooms as a key value pair where the key is the string
             * identifier (xml element name) for the room.
             */
            string path = Game.textDir + "rooms.xml";
            if (File.Exists(path))
            {
                XElement roomElement = XElement.Load(path);
                List<XElement> roomElements = roomElement.Elements().ToList();
                foreach (XElement room in roomElements)
                {
                    rooms.Add(room.Name.ToString(), new Room(room, playerManager, this));
                }
            }
            else
            {
                throw new FileNotFoundException("File does not exist...");
            }

            curRoom = rooms.Values.ToArray()[0];
        }

        public void Update()
        {
            bool exit = false;
            Room nextRoom = null;
            while (!exit)
            {
                curRoom.Enter();
                curRoom = rooms[curRoom.nextRoom];
            }
        }

        public Interaction GetInteraction(string interaction, string dialogue, Creatures.Creature creature = null, string itemId = null, int amount=0)
        {
            switch (interaction)
            {
                case "dialogue":
                    return new Dialogue(dialogue, PlayerManager);
                case "battle-demons":
                    return new BattleDemons(dialogue, creature, PlayerManager);
                case "found-item":
                    return new FoundItem(dialogue, itemId, amount, PlayerManager);
                case "mirror":
                    return new Mirror(dialogue, PlayerManager);
            }

            return new Dialogue("null", PlayerManager);
        }

        /// <summary>
        /// Class <c>Interaction</c> is the base class for every interaction: the basic unit of play for a location.
        /// </summary>
        public abstract class Interaction
        {
            public string dialogue;
            public PlayerManager PlayerManager;

            public Interaction(string dialogue, PlayerManager playerManager)
            {
                this.dialogue = dialogue ?? throw new ArgumentNullException(nameof(dialogue));
                this.PlayerManager = playerManager ?? throw new ArgumentNullException(nameof(playerManager));
            }
            public abstract bool Interact();
        }

        public class Dialogue : Interaction
        {
            public Dialogue(string dialogue, PlayerManager playerManager) : base(dialogue, playerManager)
            {

            }

            public override bool Interact()
            {
                Game.WriteDialogue(this.dialogue);
                Console.WriteLine("");
                return false;
            }
        }

        public class BattleDemons : Interaction
        {
            public Creatures.Creature Creature;
            public BattleDemons(string dialogue, Creatures.Creature creature, PlayerManager playerManager) : base(dialogue, playerManager)
            {
                Creature = creature ?? throw new ArgumentNullException(nameof(creature));
            }

            public override bool Interact()
            {
                return SummonDemon();
            }

            public bool SummonDemon()
            {
                bool flee = false;

                Game.WriteDialogue($"*{Creature.Aura}*");
                string art = Game.GetArt("creature");
                art = Game.PopulateField(art, "{name~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~}", Creature.Name);
                art = Game.PopulateField(art, "{type~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~}", Creature.Type);
                art = Game.PopulateField(art, "{OF~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~}", Creature.OverwhelmFactor.ToString());
                Console.WriteLine(art);

                Game.WriteDialogue(Creature.Dialogue);
                string sel =Game.ValidateInputSelection("Fight or flight? (fight/flee) ", new string[] {"fight", "flee"});

                switch (sel)
                {
                    case "fight":
                        FightDemon();
                        break;
                    case "flee":
                        flee = true;
                        RunAway();
                        break;
                }

                return flee;
            }

            public void RunAway()
            {
                Console.WriteLine("You need to get out of here - you flee\n");
            }

            public void FightDemon()
            {
                Console.WriteLine("You will stay and work through the pain");
                Creature.Attack();
            }
        }


        public class FoundItem : Interaction
        {
            string Id;
            int Amount;
            string Store;
            bool Found;
            public FoundItem(string dialogue, string id, int amount, PlayerManager playerManager) : base(dialogue, playerManager)
            {
                this.Id = id ?? throw new ArgumentNullException(nameof(id));

                this.Amount = amount;
                this.Found = false;
            }

            /// <summary>
            /// Method <c>Interact</c> triggers the <c>Playermanager.PickupItem()</c> method only if the item has not already been found.
            /// </summary>
            /// <returns>true if player flees room as a result; false otherwise</returns>
            public override bool Interact()
            {   
                if (Found)
                {
                    Console.WriteLine("Item found!\n");
                }
                else
                {
                    Game.WriteDialogue(this.dialogue);
                    string sel = Game.ValidateInputSelection("Store item in pockets or rucksack? (pockets/rucksack): ", new string[] { "pockets", "rucksack" });
                    PlayerManager.PickupItem(sel, Id, Amount);
                    Found = true;
                    return false;
                }
                return false;
            }
        }

        public class Mirror : Interaction
        {
            public Mirror(string dialogue, PlayerManager playerManager) : base(dialogue, playerManager)
            {

            }

            public override bool Interact()
            {
                Game.WriteDialogue(this.dialogue);
                PlayerManager.PlayerState();
                PlayerManager.InventoryContents();
                return false;
            }
        }


    }
}
