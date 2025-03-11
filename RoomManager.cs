using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace DungeonExplorer
{
    public class RoomManager
    {
        public Dictionary<string, Room> rooms;
        public Room curRoom;
        public Dictionary<string, Interaction> interactions;
        public Player Player;

        public RoomManager(Player player)
        {   
            Player = player;
            rooms = new Dictionary<string, Room>();

            string path = Game.textDir + "rooms.xml";
            if (File.Exists(path))
            {
                XElement roomElement = XElement.Load(path);
                List<XElement> roomElements = roomElement.Elements().ToList();
                foreach (XElement room in roomElements)
                {
                    rooms.Add(room.Name.ToString(), new Room(room, player));
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

        public static Interaction GetInteraction(string interaction, string dialogue, Creatures.Creature creature = null)
        {
            switch (interaction)
            {
                case "dialogue":
                    return new Dialogue(dialogue);
                case "battle-demons":
                    return new BattleDemons(dialogue, creature);
            }

            return new Dialogue("null");
        }

        public abstract class Interaction
        {
            public string dialogue;

            public Interaction(string dialogue)
            {;
                this.dialogue = dialogue;
            }
            public abstract bool Interact();
        }
        public class Dialogue : Interaction
        {
            public Dialogue(string dialogue) : base(dialogue)
            {

                this.dialogue = dialogue;
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
            public BattleDemons(string dialogue, Creatures.Creature creature) : base(dialogue)
            {
                Creature = creature;
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

                Game.WriteDialogue(Creature.CreatureDialogue);
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
    }
}
