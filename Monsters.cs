using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DungeonExplorer;

namespace DungeonExplorer
{   
    /// <summary>
    /// Abstract class <c>Monster</c> used as a base class for other Monsters.
    /// </summary>
    public abstract class Monster : Creature
    {
        /// <value>
        /// Sub-type the monster belongs to.
        /// </value>
        public string Type {  get; protected set; }

        /// <value>
        /// A phrase associated with each monster sub-type signalling its villainy.
        /// </value>
        public string Aura {  get; protected set; }

        /// <value>
        /// Any dialogue spoken around or providing context to the monster.
        /// </value>
        public string Dialogue {  get; protected set; }

        /// <value>
        /// Equivalent to attack damage: drains the players Energy.
        /// </value>
        public int OverwhelmFactor {  get; protected set; }

        /// <value>
        /// Whether the monster has been defeated.
        /// </value>
        public bool Defeated {  get; protected set; }

        /// <value>
        /// Reference to the <c>PlayerManager</c> instance the monster may interacts with.
        /// </value>
        public PlayerManager PlayerManager;

        public Monster(string name, string dialogue, int overwhelmFactor, PlayerManager playerManager)
        {
            this.Name = name ?? throw new ArgumentNullException(nameof(name), "The name cannot be null.");
            this.Dialogue = dialogue ?? throw new ArgumentNullException(nameof(dialogue), "The dialogue cannot be null.");
            this.PlayerManager = playerManager ?? throw new ArgumentNullException(nameof(dialogue), "The PlayerManager cannot be null.");

            this.OverwhelmFactor = overwhelmFactor;
            this.Defeated = false;
        }

        public void Attack()
        {
            PlayerManager.TakeDamage(OverwhelmFactor);
        }
    }

    public class SocialMonster : Monster
    {   
        public SocialMonster(string name, string dialogue, int overwhelmFactor, PlayerManager playerManager) : base(name, dialogue, overwhelmFactor, playerManager)
        {
            Type = "Social Monster";
            Aura = "Countless voices clutter your mind as you find yourself unable to think clearly.";
        }
    }

    public class SensoryMonster : Monster
    {
        public SensoryMonster(string name, string dialogue, int overwhelmFactor, PlayerManager playerManager) : base(name, dialogue, overwhelmFactor, playerManager)
        {
            Type = "Sensory Monster";
            Aura = "Your nerves tingle as your mind clouds over, clouded by its relentless stabbing.";
        }
    }

    public class InternalMonster : Monster
    {
        public InternalMonster(string name, string dialogue, int overwhelmFactor, PlayerManager playerManager) : base(name, dialogue, overwhelmFactor, playerManager)
        {
            Type = "Internal Monster";
            Aura = "Your vision fades and the surroundings become a blur, you shake yourself from head to toe.";
        }
    }
}
