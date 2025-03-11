using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DungeonExplorer;

namespace DungeonExplorer
{
    public class Creatures
    {
        public class Creature
        {
            public string Name;
            public string Type;
            public string Aura;
            public string CreatureDialogue;
            public int OverwhelmFactor;
            public bool defeated;
            public Player Player;

            public Creature(string name, string dialogue, int overwhelmFactor, Player player)
            {
                this.Name = name;
                this.CreatureDialogue = dialogue;
                this.OverwhelmFactor = overwhelmFactor;
                this.defeated = false;
                this.Player = player;
            }


            public void Attack()
            {
                Player.TakeDamage(OverwhelmFactor);
            }
        }
        public class SocialCreature : Creature
        {   
            public SocialCreature(string name, string dialogue, int overwhelmFactor, Player player) : base(name, dialogue, overwhelmFactor, player)
            {
                Type = "Social creature";
                Aura = "Countless voices clutter your mind as you find yourself unable to think clearly.";
            }
        }

        public class SensoryCreature : Creature
        {
            public SensoryCreature(string name, string dialogue, int overwhelmFactor, Player player) : base(name, dialogue, overwhelmFactor, player)
            {
                Type = "Sensory creature";
                Aura = "Your nerves tingle as your mind clouds over, clouded by its relentless stabbing.";
            }
        }

        public class InternalCreature : Creature
        {
            public InternalCreature(string name, string dialogue, int overwhelmFactor, Player player) : base(name, dialogue, overwhelmFactor, player)
            {
                Type = "Internal creature";
                Aura = "Your vision fades and the surroundings become a blur, you shake yourself from head to toe.";
            }
        }
    }
}
