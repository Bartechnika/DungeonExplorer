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
        /// <summary>
        /// Abstract class <c>Creature</c> used as a base class for other creatures.
        /// </summary>
        public abstract class Creature
        {
            public string Name;

            /// <value>
            /// Sub-type the creature belongs to.
            /// </value>
            public string Type;

            /// <value>
            /// A phrase associated with each creature sub-type signalling its villainy.
            /// </value>
            public string Aura;

            /// <value>
            /// Any dialogue spoken around or providing context to the creature.
            /// </value>
            public string Dialogue;

            /// <value>
            /// Equivalent to attack damage: drains the players Energy.
            /// </value>
            public int OverwhelmFactor;

            /// <value>
            /// Whether the creature has been defeated.
            /// </value>
            public bool defeated;

            /// <value>
            /// Reference to the <c>PlayerManager</c> instance the creature may interacts with.
            /// </value>
            public PlayerManager PlayerManager;

            public Creature(string name, string dialogue, int overwhelmFactor, PlayerManager playerManager)
            {
                this.Name = name;
                this.Dialogue = dialogue;
                this.OverwhelmFactor = overwhelmFactor;
                this.defeated = false;
                this.PlayerManager = playerManager;
            }

            public void Attack()
            {
                PlayerManager.TakeDamage(OverwhelmFactor);
            }
        }

        public class SocialCreature : Creature
        {   
            public SocialCreature(string name, string dialogue, int overwhelmFactor, PlayerManager playerManager) : base(name, dialogue, overwhelmFactor, playerManager)
            {
                Type = "Social creature";
                Aura = "Countless voices clutter your mind as you find yourself unable to think clearly.";
            }
        }

        public class SensoryCreature : Creature
        {
            public SensoryCreature(string name, string dialogue, int overwhelmFactor, PlayerManager playerManager) : base(name, dialogue, overwhelmFactor, playerManager)
            {
                Type = "Sensory creature";
                Aura = "Your nerves tingle as your mind clouds over, clouded by its relentless stabbing.";
            }
        }

        public class InternalCreature : Creature
        {
            public InternalCreature(string name, string dialogue, int overwhelmFactor, PlayerManager playerManager) : base(name, dialogue, overwhelmFactor, playerManager)
            {
                Type = "Internal creature";
                Aura = "Your vision fades and the surroundings become a blur, you shake yourself from head to toe.";
            }
        }
    }
}
